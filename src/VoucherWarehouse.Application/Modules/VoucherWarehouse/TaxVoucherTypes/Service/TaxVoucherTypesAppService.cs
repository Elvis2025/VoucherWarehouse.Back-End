using IBS.VoucherWarehouse.Common.Mapping.Helpers;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucher.Dto;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucherTypes.Dto;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucherTypes.Service;

public class TaxVoucherTypesAppService : VoucherWarehouseAppServiceBase, ITaxVoucherTypesAppService
{
    private readonly IRepository<TaxVouchersTypes, int> taxVoucherTypesRepository;

    public TaxVoucherTypesAppService(IRepository<Models.TaxVouchersTypes,int> taxVoucherTypesRepository)
    {
        this.taxVoucherTypesRepository = taxVoucherTypesRepository;
    }

    public async Task<TaxVoucherTypesOutputDto> CreateAsync(TaxVoucherTypesCreateDto input)
    {
        try
        {

            var te = Mapping<TaxVoucherTypesCreateDto, TaxVouchersTypes>.Auto.Map(input);

            var taxVoucher = await taxVoucherTypesRepository.InsertAsync(te);
            return Mapping<TaxVouchersTypes, TaxVoucherTypesOutputDto>.Auto.Map(taxVoucher);
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task DeleteAsync(EntityDto<int> input)
    {
        try
        {
            await taxVoucherTypesRepository.DeleteAsync(input.Id);
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<PagedResultDto<TaxVoucherTypesOutputDto>> GetAllAsync(TaxVoucherTypesInputDto input)
    {
        try
        {
            var taxVouchers = await taxVoucherTypesRepository.GetAllListAsync();

            return Mapping<TaxVouchersTypes, TaxVoucherTypesOutputDto>.Auto.MapToPagedResult(taxVouchers, taxVouchers.Count);
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<TaxVoucherTypesOutputDto> GetAsync(EntityDto<int> input)
    {
        try
        {
            var taxVoucher = await taxVoucherTypesRepository.GetAsync(input.Id);
            return Mapping<TaxVouchersTypes, TaxVoucherTypesOutputDto>.Auto.Map(taxVoucher);
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<TaxVoucherTypesOutputDto> UpdateAsync(TaxVoucherTypesUpdateDto input)
    {
        try
        {

            var te = Mapping<TaxVoucherTypesUpdateDto, TaxVouchersTypes>.Auto.Map(input);

            var taxVoucher = await taxVoucherTypesRepository.UpdateAsync(te);
            return Mapping<TaxVouchersTypes, TaxVoucherTypesOutputDto>.Auto.Map(taxVoucher);
        }
        catch (Exception)
        {

            throw;
        }
    }

  
   

 

  
    
}
