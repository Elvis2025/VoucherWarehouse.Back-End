using IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucher.Dto;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucher.Service;

public interface ITaxVoucherAppService : IAsyncCrudAppService<TaxVoucherOutputDto,int,TaxVoucherInputDto,TaxVoucherCreateDto,TaxVoucherUpdateDto>
{

}
