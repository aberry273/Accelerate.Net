Drop-Database  -context SchoolContext
Add-Migration InitialCreate_School -context SchoolContext
Update-Database -context SchoolContext
# Content
Drop-Database -context ContentDbContext -project "Accelerate.Foundations.Content" 
Add-Migration InitialCreate_Content -context ContentDbContext -project "Accelerate.Foundations.Content" 
Update-Database -context ContentDbContext -project "Accelerate.Foundations.Content" 
# Authentication
Drop-Database -context AccountDbContext -project "Accelerate.Features.Account" 
Add-Migration InitialCreate_Account -context AccountDbContext -project "Accelerate.Features.Account" 
Update-Database -context AccountDbContext -project "Accelerate.Features.Account" 

