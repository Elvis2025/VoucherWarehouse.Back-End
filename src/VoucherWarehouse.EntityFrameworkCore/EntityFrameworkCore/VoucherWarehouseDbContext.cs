using Abp.Zero.EntityFrameworkCore;
using IBS.VoucherWarehouse.Authorization.Roles;
using IBS.VoucherWarehouse.Authorization.Users;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;
using IBS.VoucherWarehouse.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace IBS.VoucherWarehouse.EntityFrameworkCore;

public class VoucherWarehouseDbContext : AbpZeroDbContext<Tenant, Role, User, VoucherWarehouseDbContext>
{
    /* Define a DbSet for each entity of the application */
    public virtual DbSet<TenantBranding> TenantBrandings { get; set; }
    public virtual DbSet<TaxVouchers> TaxVouchers { get; set; }
    public virtual DbSet<TaxVouchersTypes> TaxVouchersTypes { get; set; }
    public virtual DbSet<EcfApiAuthentication> EcfApiAuthentication { get; set; }
    public virtual DbSet<EcfVoucherWarehouse> EcfVoucherWarehouses { get; set; }
    public virtual DbSet<EcfVoucherWarehouseDetails> EcfVoucherWarehouseDetails { get; set; }
    public virtual DbSet<EcfVoucherWarehousePaymentForm> EcfVoucherWarehousePaymentForms { get; set; }
    public virtual DbSet<EcfVoucherWarehouseEmitterPhone> EcfVoucherWarehouseEmitterPhones { get; set; }
    public virtual DbSet<EcfVoucherWarehouseAdditionalTax> EcfVoucherWarehouseAdditionalTaxes { get; set; }
    public virtual DbSet<EcfVoucherWarehouseSubtotal> EcfVoucherWarehouseSubtotals { get; set; }
    public virtual DbSet<EcfVoucherWarehouseGlobalAdjustment> EcfVoucherWarehouseGlobalAdjustments { get; set; }
    public virtual DbSet<EcfVoucherWarehouseDetailItemCode> EcfVoucherWarehouseDetailItemCodes { get; set; }
    public virtual DbSet<EcfVoucherWarehouseDetailSubquantity> EcfVoucherWarehouseDetailSubquantities { get; set; }
    public virtual DbSet<EcfVoucherWarehouseDetailDiscount> EcfVoucherWarehouseDetailDiscounts { get; set; }
    public virtual DbSet<EcfVoucherWarehouseDetailSurcharge> EcfVoucherWarehouseDetailSurcharges { get; set; }
    public virtual DbSet<EcfVoucherWarehouseDetailAdditionalTax> EcfVoucherWarehouseDetailAdditionalTaxes { get; set; }
    public virtual DbSet<EcfVoucherWarehouseDetailOtherCurrency> EcfVoucherWarehouseDetailOtherCurrencies { get; set; }
    public virtual DbSet<EcfVoucherWarehouseDetailRetention> EcfVoucherWarehouseDetailRetentions { get; set; }
    public VoucherWarehouseDbContext(DbContextOptions<VoucherWarehouseDbContext> options)
        : base(options)
    {
    }
}
