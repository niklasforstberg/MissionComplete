This is the web api for the app MissionComplete, that lets a coach give the team challenges during the off-season and track their progress.

## Setup

1. Clone the repository
3. dotnet user-secrets init
4. dotnet user-secrets set "Jwt:Key" "your-super-secret-key-at-least-32-chars-long"
    You can generate a key with the command 'openssl rand -base64 32'
5. dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-database-connection-string"
6. dotnet user-secrets set "Jwt:Issuer" "http://localhost"
7. dotnet user-secrets set "Jwt:Audience" "http://localhost"
8. dotnet migrations add InitialCreate
9. dotnet database update
10. dotnet run

## Swagger

The swagger ui is available at https://localhost:5192/swagger

## Database

You need a database, I recommend using Docker to run a local instance of SQL Server.

You can also deploy this using the provided docker file. Do not forget to add the user secrets to the deployment target.