using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VoucherWarehouse.Authorization.Roles;
using VoucherWarehouse.Authorization.Users;
using VoucherWarehouse.MultiTenancy;

namespace VoucherWarehouse.EntityFrameworkCore;

public class VoucherWarehouseDbContext : AbpZeroDbContext<Tenant, Role, User, VoucherWarehouseDbContext>
{
    /* Define a DbSet for each entity of the application */
    public virtual DbSet<TenantBranding> TenantBrandings { get; set; }
    public VoucherWarehouseDbContext(DbContextOptions<VoucherWarehouseDbContext> options)
        : base(options)
    {
    }
}
