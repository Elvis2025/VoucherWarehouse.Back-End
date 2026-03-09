using Abp.Zero.EntityFrameworkCore;
using VoucherWarehouse.Authorization.Roles;
using VoucherWarehouse.Authorization.Users;
using VoucherWarehouse.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace VoucherWarehouse.EntityFrameworkCore;

public class VoucherWarehouseDbContext : AbpZeroDbContext<Tenant, Role, User, VoucherWarehouseDbContext>
{
    /* Define a DbSet for each entity of the application */

    public VoucherWarehouseDbContext(DbContextOptions<VoucherWarehouseDbContext> options)
        : base(options)
    {
    }
}
