# Task Management System

A modular monolith Task Management System built with ASP.NET Core MVC, Entity Framework Core (SQL Server), and SignalR for real-time notifications. Includes role-based authorization, task assignment, comments, and a polished Tailwind-style UI.

## Features
- Task CRUD (create, edit, delete, details, list)
- Assign tasks to users
- Role-based authorization (SystemAdministrator, Manager, TaskAdministrator, User)
- Comments on tasks (users can add completion notes)
- Real-time notifications (SignalR):
  - Task assigned
  - Task status changed
  - Task due soon / overdue
  - New comment on a task (sent to the assignee and to anyone viewing the task details page)
- Task status updates from the task Details page (admin/manager roles)
- Reports/Dashboard views

## Tech Stack
- .NET SDK 9.0
- ASP.NET Core MVC
- Entity Framework Core (SQL Server)
- SignalR (in-app real-time notifications)
- Tailwind-style utility classes in Razor views (no Bootstrap)

## Getting Started

### Prerequisites
- .NET SDK 9.0+
- SQL Server (localdb works)

### Configure Database
Update connection string in:
- `TaskManagementSystem.API/appsettings.json` → `ConnectionStrings:DefaultConnection`

EF model is configured to auto create tables at startup (EnsureCreated). No migrations are required for initial run. An admin user is auto-seeded.

### Seeded Admin User
At startup we seed one System Administrator:
- Username: `admin`
- Email: `admin@example.com`
- Password (demo): `Admin123!`

### Build and Run
From the repository root:

- Build:
  - `dotnet build TaskManagementSystem.API/TaskManagementSystem.API.csproj -c Debug`

- Run (HTTP profile):
  - `dotnet run --project TaskManagementSystem.API --launch-profile http`

If you change code while an instance is running, stop it before rebuilding to avoid file-lock errors:
- On Windows: Stop the running TaskManagementSystem.API process (e.g., in Task Manager or PowerShell `Stop-Process -Id <pid> -Force`).

### Login and Explore
1. Browse to the app URL printed by `dotnet run` (e.g., http://localhost:5000)
2. Sign in as the seeded admin (see credentials above)
3. Create users and tasks
4. Assign tasks and try comments/notifications

## Real-time Notifications
- The SignalR hub is mapped at `/hubs/notifications`.
- The bell icon in the top bar receives global per-user notifications.
- The task Details page also joins a task-specific SignalR group to receive inline updates when new comments are added.

### When do notifications fire?
- Task assigned to a user
- Task status changed by an admin/manager
- Due soon/Overdue checks (trigger points can be extended)
- New comment added to a task:
  - The assigned user receives a bell notification
  - Anyone viewing the task Details page receives an inline update without refreshing

## Project Structure (high level)
- `TaskManagementSystem.API/` – MVC web app (controllers, views, SignalR hub, DI setup)
- `TaskManagementSystem.Core/` – domain models, interfaces, services (business logic)
- `TaskManagementSystem.Infrastructure/` – EF Core DbContext, repositories, migrations snapshot
- `TaskManagementSystem.Tests/` – unit and integration test placeholders

## Common Issues
- File lock errors on build (MSB3026/MSB3021): Stop the running API process before rebuilding.
- Connection string issues: Ensure SQL Server is reachable and the connection string is correct.

## Roadmap / Ideas
- Persist notifications to DB (read/unread, history)
- Kanban board UI with drag-and-drop
- Full-text search and filters
- Attachments on comments
- Email delivery provider for production

## License
This project is provided as-is for demonstration and can be adapted to your needs.

