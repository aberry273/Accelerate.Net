Drop-Database  -context SchoolContext
Add-Migration InitialCreate_School -context SchoolContext
Update-Database -context SchoolContext
# Content
Drop-Database -context ContentDbContext -project "Accelerate.Foundations.Content" 
Add-Migration InitialCreate_Content1 -context ContentDbContext -project "Accelerate.Foundations.Content" 
Update-Database -context ContentDbContext -project "Accelerate.Foundations.Content" 
# Authentication
Drop-Database -context AccountDbContext -project "Accelerate.Foundations.Account" 
Add-Migration Initial_Account1 -context AccountDbContext -project "Accelerate.Foundations.Account" 
Update-Database -context AccountDbContext -project "Accelerate.Foundations.Account" 

