using IBS.VoucherWarehouse.Common.Mapping.Helpers;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucher.Dto;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucher.Service;

public class TaxVoucherAppService :VoucherWarehouseAppServiceBase, ITaxVoucherAppService
{
    private readonly IRepository<TaxVouchers, int> taxVoucherRepository;
    private readonly IRepository<TaxVouchersTypes, int> taxVouchersTypeRepository;

    public TaxVoucherAppService(IRepository<Models.TaxVouchers,int> taxVoucherRepository,IRepository<Models.TaxVouchersTypes,int> taxVouchersTypeRepository)
    {
        this.taxVoucherRepository = taxVoucherRepository;
        this.taxVouchersTypeRepository = taxVouchersTypeRepository;
    }

    public async Task<TaxVoucherOutputDto> CreateAsync(TaxVoucherCreateDto input)
    {
        try
        {

            var te = Mapping<TaxVoucherCreateDto, TaxVouchers>.Auto.Map(input);

           var taxVoucher = await taxVoucherRepository.InsertAsync(te);
            return Mapping<TaxVouchers, TaxVoucherOutputDto>.Auto.Map(taxVoucher);
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
            await taxVoucherRepository.DeleteAsync(input.Id);
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<PagedResultDto<TaxVoucherOutputDto>> GetAllAsync(TaxVoucherInputDto input)
    {
        try
        {
            var taxVouchers = await taxVoucherRepository.GetAllListAsync();

            return Mapping<TaxVouchers, TaxVoucherOutputDto>.Auto.MapToPagedResult(taxVouchers,taxVouchers.Count);
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<TaxVoucherOutputDto> GetAsync(EntityDto<int> input)
    {
        try
        {
            var taxVoucher = await taxVoucherRepository.GetAsync(input.Id);
            return Mapping<TaxVouchers, TaxVoucherOutputDto>.Auto.Map(taxVoucher);
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<TaxVoucherOutputDto> UpdateAsync(TaxVoucherUpdateDto input)
    {
        try
        {
            var taxVoucher = await taxVoucherRepository.UpdateAsync(Mapping<TaxVoucherUpdateDto, TaxVouchers>.Auto.Map(input));
            return Mapping<TaxVouchers, TaxVoucherOutputDto>.Auto.Map(taxVoucher);
        }
        catch (Exception)
        {

            throw;
        }
    }
}
