# Inventory Service Demo

Demo from this blog post [here](https://www.syncfusion.com/blogs/post/how-to-build-crud-rest-apis-with-asp-net-core-3-1-and-entity-framework-core-create-jwt-tokens-and-secure-apis.aspx)
## Scaffold Models folder
Run the following command on the Nuget Package Manager Console
```
Scaffold-DbContext "Server=localhost,1433;Database=Inventory;User=sa;Password=reallyStrongPwd123;Trusted_Connection=False" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
```

## Restore dependencies specified in the project
```
dotnet restore
```

## Build project
```
dotnet build
```
## Run project
```
dotnet run
```
