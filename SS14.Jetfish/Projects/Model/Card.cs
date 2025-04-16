using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Core.Repositories;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Projects.Model;

public sealed class Card : IEntityTypeConfiguration<Card>, IRecord<Guid>
{
    public Guid Id { get; set; }
    public int Version { get; set; }
    
    public User Author { get; set; } = null!;
    
    public void Configure(EntityTypeBuilder<Card> builder)
    {
    }
}