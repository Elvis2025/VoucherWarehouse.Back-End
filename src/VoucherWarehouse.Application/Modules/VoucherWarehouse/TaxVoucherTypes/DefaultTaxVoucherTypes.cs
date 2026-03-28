using IBS.VoucherWarehouse.Common.Abstraction;
using IBS.VoucherWarehouse.Common.Enums;
using IBS.VoucherWarehouse.Common.Helpers;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucherTypes;

public class DefaultTaxVoucherTypes : DefaultData<IRepository<TaxVouchersTypes,int>>
{
    private static readonly Lazy<DefaultTaxVoucherTypes> instance = new Lazy<DefaultTaxVoucherTypes> (() => new DefaultTaxVoucherTypes());

    public static IEnumerable<TaxVouchersTypes> TaxVouchersTypes =  Enum.GetValues<VoucherType>()
                                                                        .Select(x => new TaxVouchersTypes()
                                                                        {
                                                                            Code = x.GetCode(),
                                                                            Name = x.GetName(),
                                                                            IsActive = true,
                                                                        });
    public static DefaultTaxVoucherTypes Instance => instance.Value;

    public override async Task CreateAsync()
    {
        if(await Entity.CountAsync() > 0) return;
        await Entity.InsertOrUpdateRangeAsync(TaxVouchersTypes);
    }

    
   
}
