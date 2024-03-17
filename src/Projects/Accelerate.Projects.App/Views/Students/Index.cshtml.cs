using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Accelerate.Projects.App.Data;
using Accelerate.Projects.App.Models;

namespace Accelerate.Projects.App.Pages.Students
{
    public class IndexModel : PageModel
    {
        private readonly Accelerate.Projects.App.Data.SchoolContext _context;

        public IndexModel(Accelerate.Projects.App.Data.SchoolContext context)
        {
            _context = context;
        }

        public IList<Student> Student { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Student = await _context.Students.ToListAsync();
        }
    }
}
