using Microsoft.EntityFrameworkCore;
using AutoRepair.Data.Entities;

namespace AutoRepair.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<RoleModel> Role { get; set; }
        public DbSet<InfoModel> Info { get; set; }
        public DbSet<UserModel> User { get; set; }
        public DbSet<ClientModel> Client { get; set; }
        public DbSet<CarModel> Car { get; set; }
        public DbSet<AccountantModel> Accountant { get; set; }
        public DbSet<SpecialtyModel> Specialty { get; set; }
        public DbSet<MasterModel> Master { get; set; }
        public DbSet<ServiceModel> Service { get; set; }
        public DbSet<StatusModel> Status { get; set; }
        public DbSet<OrderModel> Order { get; set; }

        public ApplicationContext(DbContextOptions options) : base(options)
        {
            //Database.EnsureCreated();
        }
    }
}