# Glorious MultiTenant
## Introduction
This project is a MultiTenant API for a mock transaction processing system.
## Purpose
The purpose is a Proof-Of-Conecpt MultiTenant API.
## Project
- Technology: ASP.NET Core 6.
## How To Run
1. Clone or download the repository.
2. Install MS SQL Server and SSMS.
3. Open the project containing folder in Visual Studio (preferably version 2022).
4. Remove the `Entity.Migrations` folder, if it exists. 
5. Set `Entity` as the startup project.
6. Open the Package Manager console.
7. Make sure there is a migration. If not, create one by typing `Add-Migration InitialMigration`.
8. Update the database with the command `Update-Database`.
9. Open SSMS and ensure that the database has been created.
10. Set the startup project to be `WebAPI`.
11. Build and run the project.

## Usage
You can use and test this API with the provided Swagger interface.