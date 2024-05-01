Drop-Database  -context SchoolContext
Add-Migration InitialCreate_School -context SchoolContext
Update-Database -context SchoolContext
# Content
Drop-Database -context ContentDbContext -project "Accelerate.Foundations.Content" 
Add-Migration InitialCreate_Content7 -context ContentDbContext -project "Accelerate.Foundations.Content" 
Update-Database -context ContentDbContext -project "Accelerate.Foundations.Content" 
# Media
Drop-Database -context MediaDbContext -project "Accelerate.Foundations.Media" 
Add-Migration InitialCreate_Media3 -context MediaDbContext -project "Accelerate.Foundations.Media" 
Update-Database -context MediaDbContext -project "Accelerate.Foundations.Media" 
# Authentication
Drop-Database -context AccountDbContext -project "Accelerate.Foundations.Account" 
Add-Migration Initial_Account2 -context AccountDbContext -project "Accelerate.Foundations.Account" 
Update-Database -context AccountDbContext -project "Accelerate.Foundations.Account" 

