# The Digital Cookbook

The Digital Cookbook is a web application for saving, organizing, and managing personal recipes. Users can register, log in, create recipe entries, edit existing recipes, view details, search their recipe list, and delete recipes they no longer need.

## Website
https://whu8wp7y43.us-east-1.awsapprunner.com/

## Features

- User registration and login
- Session-based authentication
- Create, edit, and delete recipes
- Recipe listing and detail views
- Recipe search/report view
- Ingredients and instructions stored as structured lists
- MySQL database support
- Swagger support in development
- Optional AWS Secrets Manager integration for database credentials

## Tech Stack

- **ASP.NET Core MVC** on .NET 8
- **Entity Framework Core**
- **MySQL** with Pomelo EntityFrameworkCore provider
- **AWS Secrets Manager** for secure configuration
- **Swagger / OpenAPI**
- **Session state** for user authentication

## Project Structure

- `Controllers/` — application controllers for auth, home, and recipes
- `Data/` — EF Core database context
- `DTOs/` — request/response models for authentication
- `Models/` — domain models such as users and recipes
- `Security/` — password hashing utilities
- `Views/` — Razor views for the web UI

## Prerequisites

- .NET 8 SDK
- MySQL 8.x or compatible database
- Optional: AWS account and Secrets Manager secret if using cloud-based credentials

## Configuration

The app can use either:

1. A local connection string from configuration, or
2. AWS Secrets Manager for database settings

### Local development

Add your connection string in `appsettings.json` or user secrets:
