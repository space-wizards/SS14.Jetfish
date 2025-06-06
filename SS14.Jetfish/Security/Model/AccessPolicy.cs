﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Core.Extensions;
using SS14.Jetfish.Core.Repositories;

namespace SS14.Jetfish.Security.Model;

/// <summary>
/// Controls access policies for various endpoints and actions for Jetfish.
/// </summary>
public sealed class AccessPolicy : IEntityTypeConfiguration<AccessPolicy>, IRecord<int?>
{
    public const int AccessPolicyMaxNameLength = 200;
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? Id { get; set; } = null!;
    public int Version { get; set; }

    /// <summary>
    /// Areas this policy applies to.
    /// </summary>
    public List<Permission> Permissions { get; set; } = [];

    /// <summary>
    /// The name assigned to this policy.
    /// </summary>
    [MaxLength(AccessPolicyMaxNameLength)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the access policy can be assigned to a team role.
    /// </summary>
    /// <remarks>
    /// This is used for disallowing policies that are meant for site admins to be used in teams
    /// </remarks>
    public bool TeamAssignable { get; set; }

    public void Configure(EntityTypeBuilder<AccessPolicy> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ConfigureRowVersionInt();
    }
}