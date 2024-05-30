
# Content
Drop-Database -context ContentDbContext -project "Accelerate.Foundations.Content" 
Add-Migration InitialCreate_Content3 -context ContentDbContext -project "Accelerate.Foundations.Content" 
Update-Database -context ContentDbContext -project "Accelerate.Foundations.Content" 
# Media
Drop-Database -context MediaDbContext -project "Accelerate.Foundations.Media" 
Add-Migration InitialCreate_Media1 -context MediaDbContext -project "Accelerate.Foundations.Media" 
Update-Database -context MediaDbContext -project "Accelerate.Foundations.Media" 
# Authentication
Drop-Database -context AccountDbContext -project "Accelerate.Foundations.Account" 
Add-Migration Initial_Account1 -context AccountDbContext -project "Accelerate.Foundations.Account" 
Update-Database -context AccountDbContext -project "Accelerate.Foundations.Account" 

Scaffold-DbContext -context AccountDbContext -project  "Accelerate.Foundations.Account"  -Connection "Server=tcp:parrotmvp.database.windows.net,1433;Initial Catalog=Accounts;Persist Security Info=False;User ID=sysadmin;Password=West waves run !234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" -Provider "Microsoft.EntityFrameworkCore.SqlServer"
