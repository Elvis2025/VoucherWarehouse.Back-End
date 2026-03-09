using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace VoucherWarehouse.EntityFrameworkCore;

public static class VoucherWarehouseDbContextConfigurer
{
    public static void Configure(DbContextOptionsBuilder<VoucherWarehouseDbContext> builder, string connectionString)
    {
        builder.UseSqlServer(connectionString);
    }

    public static void Configure(DbContextOptionsBuilder<VoucherWarehouseDbContext> builder, DbConnection connection)
    {
        builder.UseSqlServer(connection);
    }
}
