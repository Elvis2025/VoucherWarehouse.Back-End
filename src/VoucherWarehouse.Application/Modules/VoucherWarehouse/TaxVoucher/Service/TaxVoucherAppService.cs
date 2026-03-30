using IBS.VoucherWarehouse.Common.Enums;
using IBS.VoucherWarehouse.Common.Helpers;
using IBS.VoucherWarehouse.Common.Mapping.Helpers;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucher.Dto;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucherTypes.Dto;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucher.Service;

public class TaxVoucherAppService : VoucherWarehouseAppServiceBase, ITaxVoucherAppService
{
    private readonly IRepository<TaxVouchers, int> taxVoucherRepository;
    private readonly IRepository<TaxVouchersTypes, int> taxVouchersTypeRepository;

    public TaxVoucherAppService(IRepository<TaxVouchers, int> taxVoucherRepository, IRepository<TaxVouchersTypes, int> taxVouchersTypeRepository)
    {
        this.taxVoucherRepository = taxVoucherRepository;
        this.taxVouchersTypeRepository = taxVouchersTypeRepository;
    }

    public async Task<TaxVoucherOutputDto> CreateAsync(TaxVoucherCreateDto input)
    {
        try
        {

            var te = Mapping<TaxVoucherCreateDto, TaxVouchers>.Auto.Map(input);
            await taxVouchersTypeRepository.GetAsync(input.TaxVoucherTypeId);
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
            var taxVouchers = (await taxVoucherRepository.GetAllIncludingAsync(x => x.TaxVouchersTypes)).ToList()!;
            var taxVouchersDto = Mapping<TaxVouchers, TaxVoucherOutputDto>.Auto.MapToPagedResult(taxVouchers, taxVouchers.Count);

            foreach (var taxVoucher in taxVouchersDto.Items)
            {
                foreach (var taxVouche in taxVouchers)
                {
                    if (taxVouche.Id == taxVoucher.Id)
                    {
                        taxVoucher.TaxVoucherType = Mapping<TaxVouchersTypes, TaxVoucherTypesOutputDto>.Auto.Map(taxVouche.TaxVouchersTypes);
                    }
                }
            }
            return taxVouchersDto;
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

    public async Task<TaxVoucherSecuenceDto> GenerateTaxVoucherAsync(VoucherType voucherType)
    {
        int currentSequence = 0, currentRemainingQuantity = 0;

        var taxVoucherType = await taxVouchersTypeRepository.FirstOrDefaultAsync(x => x.Code == voucherType.GetCode() && x.IsActive);

        if (taxVoucherType is null)
            throw new UserFriendlyException($"No se encontró tipo de comprobante: {voucherType.GetFullName()}");

        var taxVoucher = await taxVoucherRepository.GetAll().FirstOrDefaultAsync(x => x.TaxVoucherTypeId == taxVoucherType.Id);
    
        if (taxVoucher is null) 
            throw new UserFriendlyException(L("TaxVoucherNotRegistered", voucherType.GetFullName()));

        if (taxVoucher.CurrentSequence > taxVoucher.FinalSequence)
            throw new UserFriendlyException(L("ExhaustedTaxVoucher", voucherType.GetFullName()));
        if (taxVoucher.RemainingQuantity <= 0)
            throw new UserFriendlyException(L("ThereIsNotAvailableSequenceFor",$"{voucherType.GetFullName()}\n Cantidad de Secuencias: {taxVoucher.RemainingQuantity}"));
        //if (taxVoucher.ExpirationDate > DateTime.Now)
        //    throw new UserFriendlyException(L("ThereIsNotAvailableSequenceFor",$"{voucherType.GetFullName()}\n Cantidad de Secuencias: {taxVoucher.RemainingQuantity}"));

        currentSequence = ++taxVoucher.CurrentSequence;


        currentRemainingQuantity = --taxVoucher.RemainingQuantity;

        if (taxVoucher.RemainingQuantity <= taxVoucher.MinimumToAlert)
        {
            voucherType.SendAlert(currentRemainingQuantity);
        }

        var taxVoucherNumber = voucherType.GenerateTaxVoucherNumber(currentSequence);

        taxVoucher = await taxVoucherRepository.GetAsync(taxVoucher.Id);
        taxVoucher.CurrentSequence = currentSequence;
        taxVoucher.RemainingQuantity = currentRemainingQuantity;
        await taxVoucherRepository.UpdateAsync(taxVoucher);

        
        return new() {  Number = taxVoucherNumber, 
                        ExpirationDate = taxVoucher.ExpirationDate.ToString() 
                     };
    }

  
}
