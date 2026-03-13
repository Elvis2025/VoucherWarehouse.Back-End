using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IBS.VoucherWarehouse.Authorization.Roles;
using IBS.VoucherWarehouse.Authorization.Users;
using IBS.VoucherWarehouse.MultiTenancy;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;

namespace IBS.VoucherWarehouse.EntityFrameworkCore;

public class VoucherWarehouseDbContext : AbpZeroDbContext<Tenant, Role, User, VoucherWarehouseDbContext>
{
    /* Define a DbSet for each entity of the application */
    public virtual DbSet<TenantBranding> TenantBrandings { get; set; }
    public virtual DbSet<TaxVouchers> TaxVouchers { get; set; }
    public virtual DbSet<TaxVouchersTypes> TaxVouchersTypes { get; set; }
    public virtual DbSet<EcfApiAuthentication> EcfApiAuthentication { get; set; }
    public VoucherWarehouseDbContext(DbContextOptions<VoucherWarehouseDbContext> options)
        : base(options)
    {
    }
}
