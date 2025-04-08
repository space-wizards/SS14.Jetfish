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
  - GitHub integration to automatically move items to "done"
- File hosting for images etc.
  - Caching
  - File administration
- Administration
  - Role management only through SSO?


## What does it NOT do?

- Time tables (Think of "For when is something supposed to be done")
- Bug/Ticketing features
  - Use github issues for that

## What we need to do better than others?

- Do not leak users PII to other users
- Made in a real programing language (C#)
- Not look like absolute ass

# System Design

*scream*

- Base/Program.cs
  - Configuration
  - Services
  - DB setup
  - Auth setup
  - Server configuration setup
    - CORS
    - Startup migration
    - Forward headers
    - UseHttps
    - PathBase
    - Language
  - Systemd
- Base/Database
    - Postgres
    - Tables:
      - TODO