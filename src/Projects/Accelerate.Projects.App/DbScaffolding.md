
# Content
Drop-Database -context ContentDbContext -project "Accelerate.Foundations.Content" 
Add-Migration InitialCreate_Content28 -context ContentDbContext -project "Accelerate.Foundations.Content" 
Update-Database -context ContentDbContext -project "Accelerate.Foundations.Content" 
# Media
Drop-Database -context MediaDbContext -project "Accelerate.Foundations.Media" 
Add-Migration InitialCreate_Media1 -context MediaDbContext -project "Accelerate.Foundations.Media" 
Update-Database -context MediaDbContext -project "Accelerate.Foundations.Media" 
# Authentication
Drop-Database -context AccountDbContext -project "Accelerate.Foundations.Account" 
Add-Migration Initial_Account4 -context AccountDbContext -project "Accelerate.Foundations.Account" 
Update-Database -context AccountDbContext -project "Accelerate.Foundations.Account" 

# Operations
Drop-Database -context OperationsDbContext -project "Accelerate.Foundations.Operations" 
Add-Migration Migration_Operations2 -context OperationsDbContext -project "Accelerate.Foundations.Operations" 
Update-Database -context OperationsDbContext -project "Accelerate.Foundations.Operations" 

Scaffold-DbContext -context AccountDbContext -project  "Accelerate.Foundations.Account"  -Connection "Server=tcp:parrotmvp.database.windows.net,1433;Initial Catalog=Accounts;Persist Security Info=False;User ID=sysadmin;Password=West waves run !234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" -Provider "Microsoft.EntityFrameworkCore.SqlServer"
