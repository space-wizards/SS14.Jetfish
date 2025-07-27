namespace SS14.Jetfish.UserSettings;

[AttributeUsage(AttributeTargets.Property)]
public class UserSettingAttribute(string name, string category) : Attribute
{
    public readonly string Name = name;
    public readonly string Category = category;
}
