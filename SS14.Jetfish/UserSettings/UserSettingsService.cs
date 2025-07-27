using System.Collections.Immutable;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor;
using SS14.Jetfish.Database;

namespace SS14.Jetfish.UserSettings;

public class UserSettingsService
{
    public const string CategoryAccessibility = "Accessibility";

    /// <summary>
    /// Contains the reflected settings via the <see cref="UserSettingAttribute"/>
    /// </summary>
    public static ImmutableDictionary<string, List<(PropertyInfo property, string name)>> ReflectedSettings { get; private set; } = ImmutableDictionary<string, List<(PropertyInfo setting, string name)>>.Empty;

    public static void DiscoverSettings(params Type[] types)
    {
        if (!ReflectedSettings.IsEmpty)
            throw new InvalidOperationException("Settings can only be discovered once.");

        var dict = new Dictionary<string, List<(PropertyInfo settingType, string name)>>();

        foreach (var type in types)
        {
            var members = type.GetProperties();
            foreach (var member in members)
            {
                var settingsAttribute = member.GetCustomAttribute<UserSettingAttribute>();
                if (settingsAttribute == null)
                    continue;

                if (!dict.TryAdd(settingsAttribute.Category, [(member, settingsAttribute.Name)]))
                    dict[settingsAttribute.Category].Add((member, settingsAttribute.Name));
            }
        }

        ReflectedSettings = dict.ToImmutableDictionary();
    }

    private readonly IJSRuntime _jsRuntime;
    private readonly ApplicationDbContext _context;
    private readonly ISnackbar _yummybar;

    public UserSettingsService(IJSRuntime jsRuntime, ApplicationDbContext context, ISnackbar snackbar)
    {
        _jsRuntime = jsRuntime;
        _context = context;
        _yummybar = snackbar;
    }


    public async Task FillSettings(Guid userId)
    {
        var user = _context.User
            .AsNoTracking()
            .First(id => id.Id == userId);

        foreach (var (cat, settingsInCategory) in ReflectedSettings)
        {
            foreach (var setting in settingsInCategory)
            {
                var value = setting.property.GetValue(user);
                await _jsRuntime.InvokeVoidAsync("settings.setSetting", setting.property.Name, value);
            }
        }
    }

    public async Task SetSetting((PropertyInfo property, string name) setting, Guid userId, object value)
    {
        var user = await _context.User
            .FirstAsync(id => id.Id == userId);

        setting.property.SetValue(user, value);
        await _jsRuntime.InvokeVoidAsync("settings.setSetting", setting.property.Name, value);
        await _context.SaveChangesAsync();

        _yummybar.Add("Settings saved!", Severity.Success);
    }
}
