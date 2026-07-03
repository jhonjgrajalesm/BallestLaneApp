# BallestLaneApp

BallestLaneApp is a simple full-stack task management application created for the .NET technical interview challenge. The application allows users to register, log in, and manage their own tasks using CRUD operations.

The project demonstrates:

- Clean Architecture principles
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server persistence
- Repository and Unit of Work patterns
- JWT authentication
- Angular frontend
- Bootstrap UI
- Seeded demo data and credentials
- User-friendly API responses for Angular

---

## Business Goal

The goal of BallestLaneApp is to allow registered users to securely manage their personal tasks.

Users can:

- Create an account
- Log in
- View their own tasks
- Create new tasks
- Edit existing tasks
- Delete tasks
- Filter tasks by status

Each task belongs to a specific user and includes:

- Title
- Description
- Status
- Due date

---

## Demo Credentials

The application automatically seeds a demo user when the backend starts in Development mode.

Use these credentials to test the application:

```txt
Email: demo@ballestlane.com
Password: Demo123!
```

If you prefer to create your own user, you can use the registration page in the Angular app.

---

## Backend Configuration

Before running the backend, update the database connection string in `appsettings.json`.

File:

```txt
BallestLaneApp.Server/appsettings.json
```

Example connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=BallestLaneAppDb;TrustServerCertificate=True;MultipleActiveResultSets=True;Integrated Security=SSPI;Persist Security Info=False;"
  }
}
```

If your SQL Server instance is different, change the `Server` value.

Examples:

```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=BallestLaneAppDb;TrustServerCertificate=True;MultipleActiveResultSets=True;Integrated Security=SSPI;"
```

or:

```json
"DefaultConnection": "Server=localhost;Database=BallestLaneAppDb;TrustServerCertificate=True;MultipleActiveResultSets=True;Integrated Security=SSPI;"
```

If your SQL Server requires SQL authentication instead of Windows authentication, use a connection string with user and password:

```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=BallestLaneAppDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;MultipleActiveResultSets=True;"
```

Replace:

```txt
YOUR_USER
YOUR_PASSWORD
```

with your local SQL Server credentials.

---

## JWT Configuration

The API uses JWT authentication.

Add or verify this section in `appsettings.json`:

```json
{
  "Jwt": {
    "SecretKey": "BALLEST_LANE_APP_DEV_SECRET_KEY_2026_CHANGE_ME_1234567890",
    "Issuer": "BallestLaneApp.Server",
    "Audience": "BallestLaneApp.Client",
    "ExpirationMinutes": 120
  }
}
```

This key is only for local development and testing. Do not use it in production.

---

## Automatic Database Creation and Seed Data

When the backend starts in Development mode, it applies pending Entity Framework migrations and seeds demo data.

The seeded data creates:

- The `BallestLaneAppDb` database if it does not exist
- The required tables
- A demo user
- Demo tasks associated with the demo user

Demo user:

```txt
Email: demo@ballestlane.com
Password: Demo123!
```
## Thought Process During the Exercise

The goal of this exercise was not only to build a working CRUD application, but also to demonstrate a clear architectural approach, good separation of concerns, and practical decision-making during development.

I decided to build the solution as a full-stack application using **Angular** for the frontend and **.NET 10** for the backend. The backend was implemented with a clean layered structure, separating domain entities, application services, repositories, infrastructure concerns, API controllers, and persistence logic.

One important architectural decision was to use the **Unit of Work pattern** together with repositories. I chose this approach because it provides a cleaner way to coordinate persistence operations and keeps transaction control in the service layer instead of spreading `SaveChanges()` calls across multiple repositories. This made the application easier to reason about and better aligned with the idea of keeping business use cases centralized.

During implementation, once I started using the application, I noticed that filtering tasks by status would make the user experience much better. Because of that, I added a status filter at the top of the task list. This allows the user to quickly focus on pending, in-progress, completed, or cancelled tasks without needing to scan the full list manually.

I also added an information icon next to each task title. Instead of showing long descriptions directly in the table, the description is displayed as a tooltip-style bubble when the user hovers over the information icon. I chose this because it keeps the task list cleaner and easier to read while still making the full task description available when needed.

The application became useful enough that I would personally use it to manage my own tasks. This was a good sign that the project had moved beyond a basic technical exercise and had become a small but practical productivity tool.

From an architecture perspective, I believe the application is well distributed across layers. The backend separates responsibilities clearly between controllers, services, repositories, unit of work, domain entities, Entity Framework context, and security concerns such as JWT generation.

For the Angular frontend, I did not follow the Angular recommended structure 100% strictly. Instead, I organized the project in a way that felt cleaner and more readable for this specific exercise. The frontend is still separated by features, shared services, models, guards, interceptors, and layout components, but I prioritized clarity and maintainability over following every convention exactly.

Overall, my thought process was to keep the business domain simple, but implement it with professional practices: authentication, task ownership, validation, clean API responses, Angular-friendly error handling, responsive UI, and a maintainable backend architecture.