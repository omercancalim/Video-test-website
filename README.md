# Video Upload Website

This ASP.NET Core MVC application allows users to register, log in, upload videos, and view their own and other users' videos. It includes functionality for video processing such as thumbnail extraction using FFmpeg.

## Features

- **User Authentication:** Register and log in functionality.
- **Video Upload:** Users can upload videos which are processed to extract thumbnails.
- **Video Gallery:** Users can view all uploaded videos with details and thumbnails.

## Prerequisites

Before you begin, ensure you have met the following requirements:
- **.NET 6.0 SDK:** Download and install from [Microsoft .NET Download](https://dotnet.microsoft.com/en-us/download/dotnet/6.0).
- **SQL Server:** Download and install from [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads).
- **FFmpeg:** This is required for video processing (thumbnail extraction). Download and install from [FFmpeg.org](https://ffmpeg.org/download.html).

## Dependencies

This project uses the following packages:
- `Xabe.FFmpeg`: For video processing tasks.
- `Microsoft.EntityFrameworkCore.Tools`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.EntityFrameworkCore`
- `Microsoft.AspNetCore.Identity.UI`
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore`

## Setup Instructions

### Database Setup

1. **Create Database:**
   - Open SQL Server Management Studio (SSMS).
   - Connect to your SQL Server instance.
   - Create a new database named `test97`.

2. **Update Connection String:**
   - Navigate to `appsettings.json` in your project.
   - Modify the `DefaultConnection` string with your SQL Server details:

    ```json
    "ConnectionStrings": {
        "DefaultConnection": "Server=localhost;Database=test97;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
    ```

## Entity Framework Migrations

Use Entity Framework migrations to automatically create and manage your database schema:

### Using Package Manager Console

1. **Open Package Manager Console**:
   - Go to `Tools` → `NuGet Package Manager` → `Package Manager Console` in Visual Studio.

2. **Add Migration**:
   - Run the following command to scaffold a migration that creates the initial set of tables as defined in your models:
     ```powershell
     Add-Migration InitialCreate -Project YourProjectName
     ```

3. **Update Database**:
   - Apply the migration to the database to create the schema by executing:
     ```powershell
     Update-Database -Project YourProjectName
     ```


### Configure FFmpeg

Set the path for FFmpeg executables in your application startup/program method:

```csharp
FFmpeg.SetExecutablesPath(@"C:\ffmpeg\bin");
