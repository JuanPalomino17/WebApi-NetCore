using ApiVehiculos.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace ApiVehiculos.Context
{
    public class AppDbContext: DbContext

    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        public DbSet<Vehiculo>  Vehiculos { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
