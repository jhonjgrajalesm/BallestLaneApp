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
---

## Entity Framework Migrations

If migrations do not exist yet, create them from the backend project folder:

```bash
dotnet ef migrations add InitialCreate --context AppDbContext --output-dir Infrastructure/Persistence/Migrations
```

Then apply the migration:

```bash
dotnet ef database update --context AppDbContext
```

If you want to recreate the database:

```bash
dotnet ef database drop --context AppDbContext
dotnet ef database update --context AppDbContext
```

The application can also apply migrations automatically during startup in Development mode.

---

## Backend Setup

Navigate to the backend project:

```bash
cd BallestLaneApp.Server
```

Restore packages:

```bash
dotnet restore
```

Build the project:

```bash
dotnet build
```

Run the API:

```bash
dotnet run
```

The API should start on a local HTTPS port, for example:

```txt
https://localhost:7120
```

---

## Backend API Endpoints

Authentication:

```txt
POST /api/auth/register
POST /api/auth/login
GET  /api/auth/public
```

Users:

```txt
GET /api/users/me
```

Tasks:

```txt
GET    /api/tasks
GET    /api/tasks/{id}
POST   /api/tasks
PUT    /api/tasks/{id}
DELETE /api/tasks/{id}
```

Protected endpoints require this header:

```txt
Authorization: Bearer <JWT_TOKEN>
```

---

## API Response Format

The backend returns a consistent response format designed to be easy to consume from Angular.

Success example:

```json
{
  "success": true,
  "message": "Tasks retrieved successfully.",
  "data": [],
  "errors": []
}
```

Error example:

```json
{
  "success": false,
  "message": "Invalid credentials.",
  "data": null,
  "errors": [
    "Email or password is incorrect."
  ]
}
```

---

## Frontend Setup

Navigate to the Angular project:

```bash
cd BallestLaneApp.Client
```

Install dependencies:

```bash
npm install
```

Run the Angular app:

```bash
ng serve
```

Open:

```txt
http://localhost:4200
```

---

## Angular Proxy Configuration

The Angular app should use a proxy so requests like `/api/auth/login` are forwarded to the .NET backend.

Example `proxy.conf.js`:

```js
const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT
  ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
  : env.ASPNETCORE_URLS
    ? env.ASPNETCORE_URLS.split(';')[0]
    : 'https://localhost:7120';

const PROXY_CONFIG = [
  {
    context: [
      '/api'
    ],
    target,
    secure: false,
    changeOrigin: true,
    logLevel: 'debug'
  }
];

module.exports = PROXY_CONFIG;
```

Make sure `angular.json` references the proxy:

```json
"serve": {
  "options": {
    "proxyConfig": "src/proxy.conf.js"
  }
}
```

After changing the proxy, restart Angular:

```bash
ng serve
```

---

## Bootstrap and Icons

The Angular app uses Bootstrap and Bootstrap Icons.

Install them with:

```bash
npm install bootstrap bootstrap-icons @popperjs/core
```

Make sure `angular.json` includes:

```json
"styles": [
  "node_modules/bootstrap/dist/css/bootstrap.min.css",
  "node_modules/bootstrap-icons/font/bootstrap-icons.css",
  "src/styles.css"
],
"scripts": [
  "node_modules/bootstrap/dist/js/bootstrap.bundle.min.js"
]
```

---

## Login Flow

1. Open the Angular app.
2. Log in with the seeded demo user:

```txt
Email: demo@ballestlane.com
Password: Demo123!
```

3. After login, Angular stores the JWT token.
4. The token is sent in the `Authorization` header using an HTTP interceptor.
5. The user is redirected to `/tasks`.

---

## Registration Flow

If you do not want to use the seeded user, create a new account from the registration page.

The registration page calls:

```txt
POST /api/auth/register
```

After successful registration:

- The user is created
- A JWT token is returned
- The user session is saved in Angular
- The app redirects to the tasks page

---

## Task Features

The Angular task screen supports:

- List tasks
- Create task
- Edit task
- Delete task
- Filter by status
- Display status as text
- Display due date in a readable format
- Show task description in an information tooltip

Task statuses:

```txt
1 = Pending
2 = In Progress
3 = Completed
4 = Cancelled
```

---

## Troubleshooting

### SQL Server certificate error

If you see an error like:

```txt
The certificate chain was issued by an authority that is not trusted.
```

Use:

```txt
TrustServerCertificate=True
```

in your connection string.

Example:

```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=BallestLaneAppDb;TrustServerCertificate=True;MultipleActiveResultSets=True;Integrated Security=SSPI;Persist Security Info=False;"
```

---

### Angular shows 401 in Network tab

This is normal when login credentials are invalid.

The browser Network tab will still show:

```txt
401 Unauthorized
```

The Angular UI handles this response and displays a friendly message to the user.

---

### API endpoints are not being reached from Angular

Check the proxy config.

The proxy must include:

```js
context: [
  '/api'
]
```

not only:

```js
context: [
  '/weatherforecast'
]
```

---

### EF Core pending model changes warning

Do not generate password hashes dynamically inside `HasData()`.

Use the runtime `DatabaseSeeder` approach:

```csharp
await DatabaseSeeder.SeedAsync(dbContext);
```

This avoids dynamic seed values in EF migrations.

---

## Suggested Demo Script

1. Start SQL Server.
2. Start the backend API.
3. Confirm the database is created automatically.
4. Open the Angular app.
5. Login with:

```txt
demo@ballestlane.com
Demo123!
```

6. Show task list.
7. Filter tasks by status.
8. Create a new task.
9. Edit the task.
10. Delete the task.
11. Log out.
12. Register a new user.
13. Show that the new user can manage their own tasks.

---

## Technical Summary

BallestLaneApp demonstrates a simple business use case implemented with professional architecture.

Backend:

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Clean Architecture style organization
- Repository pattern
- Unit of Work
- JWT authentication
- Consistent API responses

Frontend:

- Angular
- Bootstrap
- Reactive Forms
- Auth Guard
- HTTP Interceptor
- JWT session storage
- User-friendly error handling
- Task CRUD UI

---

## Demo Credentials

```txt
Email: demo@ballestlane.com
Password: Demo123!
```
