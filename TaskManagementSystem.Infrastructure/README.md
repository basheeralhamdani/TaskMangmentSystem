# Task Management System Infrastructure

The data access layer for the Task Management System application. This project contains the implementation of repositories, database context, and other infrastructure components.

## Architecture

This project implements the Repository Pattern and Unit of Work Pattern to abstract data access operations:

1. **Data Access** - Entity Framework Core implementation
2. **Repositories** - Concrete implementations of repository interfaces
3. **Unit of Work** - Manages database transactions and coordinates repositories

## Components

### Data

- **ApplicationDbContext** - Entity Framework Core DbContext for database operations
- **UnitOfWork** - Implements Unit of Work pattern for managing repositories
- **IUnitOfWork** - Interface for the Unit of Work pattern

### Repositories

- **TaskRepository** - Implements ITaskRepository for data access operations related to tasks
- **UserRepository** - Implements IUserRepository for data access operations related to users

## Configuration

The application uses SQL Server for data storage. The connection string is configured in the API project's `appsettings.json` file.

## Database Migrations

To create or update the database schema:

1. Add a new migration:
   ```bash
   dotnet ef migrations add MigrationName --project ../TaskManagementSystem.Infrastructure
   ```

2. Update the database:
   ```bash
   dotnet ef database update --project ../TaskManagementSystem.Infrastructure
   ```

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.
