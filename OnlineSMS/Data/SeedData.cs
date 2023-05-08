using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineSMS.Models;
using System.Net.WebSockets;

namespace OnlineSMS.Data
{
    public class SeedData
    {
        public static async void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new OnlineSMSContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<OnlineSMSContext>>()))
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!roleManager.RoleExistsAsync("Root").Result)
                {
                    roleManager.CreateAsync(new IdentityRole("Root")).Wait();
                }

                if (!roleManager.RoleExistsAsync("Admin").Result)
                {
                    roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
                }

                if (!roleManager.RoleExistsAsync("User").Result)
                {
                    roleManager.CreateAsync(new IdentityRole("User")).Wait();
                }
            }
        }
    }
}
