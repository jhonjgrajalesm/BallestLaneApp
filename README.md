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
