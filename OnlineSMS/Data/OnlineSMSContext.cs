using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineSMS.Models;

namespace OnlineSMS.Data
{
    public class OnlineSMSContext : IdentityDbContext<User>
    {
        public OnlineSMSContext (DbContextOptions<OnlineSMSContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Product { get; set; } = default!;
    }
}
