This is the web api for the app MissionComplete, that lets a coach give the team challenges during the off-season and track their progress.

## Setup

1. Clone the repository
3. dotnet user-secrets init
4. dotnet user-secrets set "Jwt:Key" "your-super-secret-key-at-least-32-chars-long"
    You can generate a key with the command 'openssl rand -base64 32'
5. dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-database-connection-string"
6. dotnet user-secrets set "Jwt:Issuer" "http://localhost"
7. dotnet user-secrets set "Jwt:Audience" "http://localhost"
8. dotnet user-secrets set "SmtpServer" "smtp.gmail.com"
9. dotnet user-secrets set "SmtpPort" "25"
10. dotnet user-secrets set "SmtpUsername" "your-email@gmail.com"
11. dotnet user-secrets set "SmtpPassword" "your-app-password"
12. dotnet user-secrets set "SmtpEnableSsl" "true"
13. dotnet migrations add InitialCreate
14. dotnet database update
15. dotnet run

## Swagger

The swagger ui is available at https://localhost:5192/swagger

## Database

You need a database, I recommend using Docker to run a local instance of SQL Server.

You can also deploy this using the provided docker file. Do not forget to add the user secrets to the deployment target.

## Email

You need to set up an email account to send emails from.

## TODO update appsettings.json:
The frontend base URL in emails is configurable via Frontend:BaseUrl in appsettings.json (defaults to http://localhost:8085).