# Task Management System Core

The core business logic layer for the Task Management System application. This project contains domain models, interfaces, and services that implement the business rules of the application.

## Architecture

This project follows the principles of Clean Architecture, with clear separation of concerns between:

1. **Domain Models** - The core entities and value objects of the system
2. **Interfaces** - Contracts for services and repositories
3. **Services** - Business logic implementation

## Domain Models

- **User** - Represents a user in the system
- **TaskItem** - Represents a task that can be assigned to users
- **TaskComment** - Represents comments on tasks
- **Enums** - PriorityLevel, TaskStatus, UserRole

## Interfaces

- **ITaskService** - Contract for task management operations
- **IUserService** - Contract for user management operations
- **ITaskRepository** - Contract for data access operations related to tasks
- **IUserRepository** - Contract for data access operations related to users
- **INotificationService** - Contract for sending notifications
- **IReportService** - Contract for generating reports

## Services

- **TaskService** - Implements ITaskService for business logic related to tasks
- **UserService** - Implements IUserService for business logic related to users
- **EmailNotificationService** - Implements INotificationService for sending email notifications

## Usage

This project is referenced by the Infrastructure and API projects. It contains no implementation details, only contracts and business logic.

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.
