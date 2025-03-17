# Blog API

## Overview

In Development

## Features

- **Entity Framework Core**: ORM for database interaction
- **JWT Authentication & Authorization**
- **RESTful Endpoints**
- **Postman Collection** for API testing
- **MVC Pattern**

## Technologies Used

- ASP.NET Core
- Entity Framework Core
- SQL Server
- JWT (JSON Web Token)
- Postman (for testing)

## Setup Instructions

### Prerequisites

- .NET SDK installed
- SQL Server configured
- Postman (optional for API testing)

### Installation Steps

1. Clone the repository:

   ```sh
   git clone https://github.com/your-username/your-repo.git
   cd your-repo
   ```

2. Install dependencies:

   ```sh
   dotnet restore
   ```

3. Update the database:

   ```sh
   dotnet ef database update
   ```

4. Run the application:

   ```sh
   dotnet run
   ```

## API Endpoints

### Authentication

- **`POST /api/auth/register`** - Register a new user
- **`POST /api/auth/login`** - Authenticate and receive a JWT token

### Blog Posts

- **`GET /api/posts`** - Get all posts
- **`POST /api/posts`** - Create a new post (requires authentication)
- **`GET /api/posts/{id}`** - Get a post by ID
- **`PUT /api/posts/{id}`** - Update a post (requires authentication)
- **`DELETE /api/posts/{id}`** - Delete a post (requires authentication)

### Users

- **`GET /api/users`** - Get all users (admin only)

## Authentication & Authorization

- Users must register and log in to receive a JWT token.
- The token must be included in the `Authorization` header as `Bearer <token>` for protected endpoints.
- Admin users have access to certain restricted endpoints.

## Testing with Postman

1. Import the provided Postman collection.
2. Register a new user and log in.
3. Copy the JWT token and use it in the Authorization header for protected routes.

## License

MIT License

