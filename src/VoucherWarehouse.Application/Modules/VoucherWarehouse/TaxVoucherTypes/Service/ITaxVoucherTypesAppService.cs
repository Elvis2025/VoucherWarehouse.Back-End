using IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucherTypes.Dto;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucherTypes.Service;

public interface ITaxVoucherTypesAppService : IAsyncCrudAppService<TaxVoucherTypesOutputDto,int,TaxVoucherTypesInputDto,TaxVoucherTypesCreateDto,TaxVoucherTypesUpdateDto>
{
}
