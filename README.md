# Document Management System
## Prerequisites

- .NET Core 5.0 SDK
- A web browser

## Features

- User-friendly interface
- Tracking of document status (sent/delivered) between users

## Tech Stack

- N-Tier architecture
- .NET Core 5.0 MVC

## Installation

1. Clone the repository or download file: `git clone https://github.com/Yaooo28/DocumentManagementSystem.git`
2. Navigate to the project directory: `cd DocumentManagementSystem`
3. Restore the dependencies: `dotnet restore`
4. Set up the database:
   1. Open the database template file located in the `db` folder.
   2. Execute the SQL script to create the necessary tables and sample data.
   3. Update the connection string in the `appsettings.json` file located in the `ui` tier with the correct database connection details.
5. Run the application: `dotnet run`

## Usage

1. Log in with the following pre-populated accounts:
   - Admin:
     - username: `admin`
     - password: `123`
   - Moderator:
     - username: `secretary`
     - password: `123`
   -Member:
      -username: `example`
      -password: `123`
2. Add users and documents.
3. Track the status of documents as they are sent and delivered between users.
