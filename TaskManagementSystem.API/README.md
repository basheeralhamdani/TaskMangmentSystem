# Task Management System API

A modern task management system built with ASP.NET Core 9.0, following clean architecture principles.

## Features

- User authentication and authorization with role-based access control
- Task creation, assignment, and tracking
- Task comments and notifications
- Reporting and analytics
- Responsive UI with smooth animations

## Technologies Used

- ASP.NET Core 9.0
- Entity Framework Core 9.0
- SQL Server
- Clean Architecture
- Repository Pattern
- Unit of Work Pattern

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- SQL Server (LocalDB or full instance)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/TaskManagementSystem.API.git
   cd TaskManagementSystem.API
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Update the database:
   ```bash
   dotnet ef database update
   ```

4. Run the application:
   ```bash
   dotnet run
   ```

### Configuration

The application uses SQL Server for data storage. By default, it connects to LocalDB. To configure a different database connection, update the `ConnectionStrings` section in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your_server;Database=your_database;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

## API Endpoints

The application includes the following API endpoints:

- `/api/tasks` - Task management endpoints
- `/api/users` - User management endpoints
- `/api/reports` - Reporting endpoints

## Authentication

The application uses cookie-based authentication. Users can log in through the `/Account/Login` endpoint.

## Roles

The system supports the following user roles:

- User - Can view and manage assigned tasks
- Manager - Can view and manage tasks for team members
- Task Administrator - Can manage tasks and users
- System Administrator - Full system access

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.
