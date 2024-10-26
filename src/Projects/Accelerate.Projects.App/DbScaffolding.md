
# Content
Drop-Database -context ContentDbContext -project "Accelerate.Foundations.Content" 
Add-Migration InitialCreate_Content28 -context ContentDbContext -project "Accelerate.Foundations.Content" 
Update-Database -context ContentDbContext -project "Accelerate.Foundations.Content" 
# Media
Drop-Database -context MediaDbContext -project "Accelerate.Foundations.Media" 
Add-Migration InitialCreate_Media1 -context MediaDbContext -project "Accelerate.Foundations.Media" 
Update-Database -context MediaDbContext -project "Accelerate.Foundations.Media" 

# Authentication
Drop-Database -context UsersDbContext -project "Accelerate.Foundations.Users" 
Add-Migration Initial_Users1 -context AccountDbContext -project "Accelerate.Foundations.Users" 
Update-Database -context AccountDbContext -project "Accelerate.Foundations.Users" 

# Operations
Drop-Database -context OperationsDbContext -project "Accelerate.Foundations.Operations" 
Add-Migration Migration_Operations2 -context OperationsDbContext -project "Accelerate.Foundations.Operations" 
Update-Database -context OperationsDbContext -project "Accelerate.Foundations.Operations" 




# Accounts
Drop-Database -context AccountsDbContext -project "Accelerate.Foundations.Accounts" 
Add-Migration Accounts_Migration_2 -context AccountsDbContext -project "Accelerate.Foundations.Accounts" 
Update-Database -context AccountsDbContext -project "Accelerate.Foundations.Accounts" 

# Orders
Drop-Database -context OrdersDbContext -project "Accelerate.Foundations.Orders" 
Add-Migration Orders_Migration_1 -context OrdersDbContext -project "Accelerate.Foundations.Orders" 
Update-Database -context OrdersDbContext -project "Accelerate.Foundations.Orders" 

# Transfers
Drop-Database -context TransfersDbContext -project "Accelerate.Foundations.Transfers" 
Add-Migration Transfers_Migration_1 -context TransfersDbContext -project "Accelerate.Foundations.Transfers" 
Update-Database -context TransfersDbContext -project "Accelerate.Foundations.Transfers" 

# Rates
Drop-Database -context RatesDbContext -project "Accelerate.Foundations.Rates" 
Add-Migration Rates_Migration_1 -context RatesDbContext -project "Accelerate.Foundations.Rates" 
Update-Database -context RatesDbContext -project "Accelerate.Foundations.Rates" 

# KYC
Drop-Database -context KycDbContext -project "Accelerate.Foundations.Kyc" 
Add-Migration Kyc_Migration_1 -context KycsDbContext -project "Accelerate.Foundations.Kyc" 
Update-Database -context KycsDbContext -project "Accelerate.Foundations.Kyc" 

# Users - TODO
Drop-Database -context UsersDbContext -project "Accelerate.Foundations.Users" 
Add-Migration Users_Migration_1 -context UsersDbContext -project "Accelerate.Foundations.Users" 
Update-Database -context UsersDbContext -project "Accelerate.Foundations.Users" 

# Webhooks - TODO
Drop-Database -context WebhooksDbContext -project "Accelerate.Foundations.Webhooks" 
Add-Migration Webhooks_Migration_1 -context WebhooksDbContext -project "Accelerate.Foundations.Webhooks" 
Update-Database -context WebhooksDbContext -project "Accelerate.Foundations.Webhooks" 




Scaffold-DbContext -context AccountDbContext -project  "Accelerate.Foundations.Account"  -Connection "Server=tcp:parrotmvp.database.windows.net,1433;Initial Catalog=Accounts;Persist Security Info=False;User ID=sysadmin;Password=West waves run !234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" -Provider "Microsoft.EntityFrameworkCore.SqlServer"
