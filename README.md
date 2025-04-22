# SS14.Jetfish

SS14.Jetfish is our solution to enterprise confusion. It is a Project Management tool.

This is a work in progress.

## What does it do? (Project Outline)

- SSO support with options for required claims / roles.
- Teams
- Kanban boards (global and under a team)
  - Comments
    - Proper system for replies so you can follow a conversation along
  - Labels for items
  - Assigned Users
  - Rich Display (See trello cards)?
    - Emojis
  - GitHub integration to automatically move items to "done"
- File hosting for images etc.
  - Caching / Etags
  - File administration
  - Compression!!!!
  - Animated WebP Support (should work ootb)
- Administration
  - IDP claim role mapping for global policies
  - Fine grained access control using Global and resource-based access policies and roles
  - Team and user management allowing admin teams, project focused teams etc.
- Minimal use of PII
  - Development starts with the assumption that emails won't be required, though no promise is made to keep it that way

## What does it NOT do?

- Time tables (Think of "For when is something supposed to be done")
- Bug/Ticketing features
  - Use github issues for that

## What we need to do better than others?

- Do not leak users PII to other users
- Made in a real programing language (C#)
- Not look like absolute ass

## Things to consider after v1.0
 
- Localization using [Fluent.net](https://www.nuget.org/packages/Fluent.Net) by implementing [IStringLocalizer](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.localization.istringlocalizer?view=netstandard-2.0-pp)