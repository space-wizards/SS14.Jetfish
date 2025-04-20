using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Core;
using SS14.Jetfish.Core.Extensions;
using SS14.Jetfish.Core.Repositories;
using SS14.Jetfish.Database;
using SS14.Jetfish.Security;

namespace SS14.Jetfish.Projects.Model;

public sealed partial class Project : IEntityTypeConfiguration<Project>, IResource, IRecord<Guid>, IValidatableObject
{
    public const int ProjectNameMaxLength = 64;

    public Guid Id { get; set; }
    public int Version { get; set; }

    [Required]
    [MaxLength(ProjectNameMaxLength)]
    public required string Name { get; set; }

    public ProjectBackgroundSpecifier BackgroundSpecifier { get; set; }

    [Required]
    [MaxLength(200)]
    public required string Background { get; set; }

    // TODO: Implement properties

    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ConfigureRowVersionGuid();
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (BackgroundSpecifier != ProjectBackgroundSpecifier.Color || HexColor().IsMatch(Background))
            return  [];

        return
        [
            new ValidationResult(
                "Background specifier is set to color, but background is not a valid color.",
                [nameof(Background)])
        ];
    }
    
    [GeneratedRegex(@"^#(?:[0-9a-fA-F]{3,4}){1,2}$")]
    private static partial Regex HexColor();

}