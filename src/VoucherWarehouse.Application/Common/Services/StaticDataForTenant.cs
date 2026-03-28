using IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucherTypes;
using Microsoft.EntityFrameworkCore;

namespace IBS.VoucherWarehouse.Common.Services;

public class StaticDataForTenant
{
    private static readonly Lazy<StaticDataForTenant> instance = new Lazy<StaticDataForTenant>(() => new StaticDataForTenant());

    public static StaticDataForTenant Instance => instance.Value;
    private StaticDataForTenant() { }
    public void CreateAll() => _ = Task.Run(async() =>{ await CreateAllAsync(); });

    public async Task CreateAllAsync()
    {
        List<Task> tasks = new()
        {
            DefaultTaxVoucherTypes.Instance.CreateAsync(),
        };

        await Task.WhenAll(tasks);
    }

   


}
