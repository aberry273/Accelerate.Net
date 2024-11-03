
# Accounts
Drop-Database -context AccountsDbContext -project "Accelerate.Foundations.Accounts" 
Add-Migration Accounts_Migration_6 -context AccountsDbContext -project "Accelerate.Foundations.Accounts" 
Update-Database -context AccountsDbContext -project "Accelerate.Foundations.Accounts" 

# Funding
Drop-Database -context FundingDbContext -project "Accelerate.Foundations.Funding" 
Add-Migration Funding_Migration_1 -context FundingDbContext -project "Accelerate.Foundations.Funding" 
Update-Database -context FundingDbContext -project "Accelerate.Foundations.Funding" 

# Orders
Drop-Database -context OrdersDbContext -project "Accelerate.Foundations.Orders" 
Add-Migration Orders_Migration_1 -context OrdersDbContext -project "Accelerate.Foundations.Orders" 
Update-Database -context OrdersDbContext -project "Accelerate.Foundations.Orders" 

# Transactions
Drop-Database -context TransactionsDbContext -project "Accelerate.Foundations.Transactions" 
Add-Migration Transactions_Migration_1 -context TransactionsDbContext -project "Accelerate.Foundations.Transactions" 
Update-Database -context TransactionsDbContext -project "Accelerate.Foundations.Transactions" 

# Rates
Drop-Database -context RatesDbContext -project "Accelerate.Foundations.Rates" 
Add-Migration Rates_Migration_3 -context RatesDbContext -project "Accelerate.Foundations.Rates" 
Update-Database -context RatesDbContext -project "Accelerate.Foundations.Rates" 

# KYC
Drop-Database -context KycDbContext -project "Accelerate.Foundations.Kyc" 
Add-Migration Kyc_Migration_1 -context KycDbContext -project "Accelerate.Foundations.Kyc" 
Update-Database -context KycDbContext -project "Accelerate.Foundations.Kyc" 

# Users
Drop-Database -context UsersDbContext -project "Accelerate.Foundations.Users" 
Add-Migration Users_Migration_1 -context UsersDbContext -project "Accelerate.Foundations.Users" 
Update-Database -context UsersDbContext -project "Accelerate.Foundations.Users" 

# Settlements
Drop-Database -context SettlementsDbContext -project "Accelerate.Foundations.Settlements" 
Add-Migration Settlements_Migration_1 -context SettlementsDbContext -project "Accelerate.Foundations.Settlements" 
Update-Database -context SettlementsDbContext -project "Accelerate.Foundations.Settlements" 

# Webhooks - TODO
Drop-Database -context WebhooksDbContext -project "Accelerate.Foundations.Webhooks" 
Add-Migration Webhooks_Migration_1 -context WebhooksDbContext -project "Accelerate.Foundations.Webhooks" 
Update-Database -context WebhooksDbContext -project "Accelerate.Foundations.Webhooks" 




Scaffold-DbContext -context AccountDbContext -project  "Accelerate.Foundations.Account"  -Connection "Server=tcp:parrotmvp.database.windows.net,1433;Initial Catalog=Accounts;Persist Security Info=False;User ID=sysadmin;Password=West waves run !234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" -Provider "Microsoft.EntityFrameworkCore.SqlServer"
