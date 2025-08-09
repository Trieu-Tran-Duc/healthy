# healthy application

## Configuration
- .NET 8 SDK (https://dotnet.microsoft.com/download)
- Microsoft SQL Server 2022 (RTM) - 16.0.1000.6 (X64)   
- Visual Studio 2022 (>= 17.8) or Visual Studio Code

## Install EF Core CLI Tools
```bash
dotnet tool install --global dotnet-ef
```
---

## Configure Connection String

In **API Project** (`appsettings.json`) and **Web Project** (`appsettings.json`):

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=server;Uid=Uid;Pwd=Pwd;Database=Database;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

**Parameters:**
- **Server** → SQL Server name or IP address  
- **Uid / Pwd** → SQL Authentication credentials  
- **Database** → Target database name  
- **Trusted_Connection=True** → Enables Windows Authentication (omit `Uid` & `Pwd` if using this)  
- **TrustServerCertificate=True** → Ignores SSL certificate validation  
--

## Run Database Migration

### Create Migration
```bash
dotnet ef migrations add InitialCreate --project Infrastructure --startup-project Api
```
### Apply Migration
```bash
dotnet ef database update --project Infrastructure --startup-project Api
```
---

## Run the API Project
### Restore Packages
```bash
dotnet restore Api
```
### Build the Project
```bash
dotnet build Api
```
### Run the Project
```bash
dotnet run --project Api
```
---
## Run the Web Project
### Restore Packages
```bash
dotnet restore Web
```
### Build the Project
```bash
dotnet build Web
```
### Run the Project
```bash
dotnet run --project Web
```