# Database Setup
1. Install EF cli-tools
> dotnet tool install --global dotnet-ef
2. Create migration
> Add-Migration InitialCreate
> Update-Databas

# Local secrets
1. Initial local dev secret storage
dotnet user-secrets init --project .\Projects\Accelerate.Projects.App\Accelerate.Projects.App.csproj
2. Update secret GUID in Accelerate.Projects.App > Right Click > Manage user secrets
3. Set secret
dotnet user-secrets set "appsettings.key" "value" --project .\Projects\Accelerate.Projects.App\Accelerate.Projects.App.csproj
1. Or 
type .\secrets.json | dotnet user-secrets set