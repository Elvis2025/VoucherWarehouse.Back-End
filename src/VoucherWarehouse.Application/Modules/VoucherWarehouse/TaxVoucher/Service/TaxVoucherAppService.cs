using IBS.VoucherWarehouse.Common.GlobalHelpers;
using IBS.VoucherWarehouse.Common.Mapping.Helpers;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucher.Dto;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucherTypes.Dto;

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
            var taxVouchers = (await taxVoucherRepository.GetAllIncludingAsync( x => x.TaxVouchersTypes)).ToList()!;
            var taxVouchersDto = Mapping<TaxVouchers, TaxVoucherOutputDto>.Auto.MapToPagedResult(taxVouchers, taxVouchers.Count);

            foreach (var taxVoucher in taxVouchersDto.Items)
            {
                foreach(var taxVouche in taxVouchers)
                {
                    if(taxVouche.Id == taxVoucher.Id)
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

    public async Task<Tuple<string, DateTime>> GenerateTaxVoucherAsync(string TaxVoucherCode)
    {
        int? tenantId = CurrentUnitOfWork.GetTenantId();

        DateTime ExpirationDate = new DateTime();
        int initialSequence = 0, currentSequence = 0, finalSequence = 0, registeredQuantity = 0, remainingQuantity = 0, minimumToAlert = 0, statusId = 0;

        string sufitx = null, taxVoucherNumber = null;
        //Checking if TaskVoucher use is Active
        bool useTaxVouchers = false;
        //Task Voucher Length
        int TaxVoucherNumberLength = 0;
        bool isElectronicBilling = false;
        bool NothaveElectroniceCF = false;
        if (tenantId.HasValue)
        {
            useTaxVouchers = SettingManager.GetSettingValueForTenant<bool>(AppSettings.TaxVouchers.UseTaxVouchers, tenantId.Value);
            TaxVoucherNumberLength = SettingManager.GetSettingValueForTenant<int>(AppSettings.TaxVouchers.TaxVoucherNumberLength, tenantId.Value);
            isElectronicBilling = SettingManager.GetSettingValueForTenant<bool>(AppSettings.ElectronicBillingSettings.Activeelectronicbilling, tenantId.Value);
            NothaveElectroniceCF = SettingManager.GetSettingValueForTenant<bool>(AppSettings.ElectronicBillingSettings.NothaveElectroniceCF, tenantId.Value);
        }
        else
        {
            useTaxVouchers = SettingManager.GetSettingValue<bool>(AppSettings.TaxVouchers.UseTaxVouchers);
            TaxVoucherNumberLength = SettingManager.GetSettingValue<int>(AppSettings.TaxVouchers.TaxVoucherNumberLength);
            isElectronicBilling = SettingManager.GetSettingValue<bool>(AppSettings.ElectronicBillingSettings.Activeelectronicbilling);
            NothaveElectroniceCF = SettingManager.GetSettingValue<bool>(AppSettings.ElectronicBillingSettings.NothaveElectroniceCF);
        }


        var taxVoucherType = _taxVouchersTypeRepository.FirstOrDefault(x => x.Code == TaxVoucherCode);
        if (taxVoucherType == null)
        {
            throw new Abp.UI.UserFriendlyException(L("InvalidTaxVoucherCode", TaxVoucherCode.ToString()));
        }
        //SI EL TIPO DE COMPROBANTE ESTA ACTIVO Y ESTA ACTIVO EL SETTING DE GENERAR COMPROBANTE FISCAL, LO GENERA          
        if (useTaxVouchers && taxVoucherType.IsActive)
        {
            var currentTaxVoucher = _taxVoucherRepository.GetAll().Where(x => x.IsActive == true && x.TaxVoucherTypeId == taxVoucherType.Id).FirstOrDefault();

            if (currentTaxVoucher == null)
                throw new Abp.UI.UserFriendlyException(L("TaxVoucherNotRegistered", TaxVoucherCode.ToString()));

            if (currentTaxVoucher.CurrentSequence > currentTaxVoucher.FinalSequence)
                throw new Abp.UI.UserFriendlyException(L("ExhaustedTaxVoucher", TaxVoucherCode.ToString()));

            currentSequence = currentTaxVoucher.CurrentSequence;

            ExpirationDate = currentTaxVoucher.ExpirationDate;
            string prefix = currentTaxVoucher.Prefix;
            remainingQuantity = currentTaxVoucher.RemainingQuantity;


            //Modified to take the taxvouchertype length
            TaxVoucherNumberLength = (currentTaxVoucher.TaxVouchersTypes.TaxVoucherLenght - prefix.Length);

            //MINIMUM TO ALERT
            if (remainingQuantity < currentTaxVoucher.MinimumToAlert)
            {
                //SEND AN ALERT

                long number = remainingQuantity;
                string Message = L("MinimumAlertTaxVoucher{0}{1}", taxVoucherType.CodeAndDescription, remainingQuantity);

                var customData = new PersonalizedNotification.AppNotificationCustomData(AbpSession.TenantId,
                    ItcCoreSystemNotificationName.MinimumAlertTaxVoucher,
                    Message, type: AppNotificationType.All,
                    severity: Abp.Notifications.NotificationSeverity.Warn);

                customData.MessageProperties.Add("taxVoucherCode", TaxVoucherCode);
                customData.SmsProperties.Add(AppNotificationCustomDataConst.ShortMessageSms, Message);

                await _appSystemNotifier.SendNotificationGroupAsync(customData);

            }

            if (remainingQuantity > 0)
            {

                currentSequence++;
                char _char = Convert.ToChar("0");
                string _sufix = currentSequence.ToString().PadLeft(TaxVoucherNumberLength, _char);
                taxVoucherNumber = prefix + _sufix;

            }
            else if (isElectronicBilling && taxVoucherType.IsElectronic && NothaveElectroniceCF)
            {
                var voucherResult = new Tuple<string, DateTime>(taxVoucherNumber, ExpirationDate);
                switch (taxVoucherType.Code)
                {
                    case IBS.itcSystemGlobalVariables.TaxVouchersTypes.FacturaCréditoFiscalElectrónico:
                        voucherResult = await GenerateTaxVoucherAsync(GlobalHelpers.TaxVouchersTypes.FacturasDeCreditoFiscal);
                        break;
                    case IBS.itcSystemGlobalVariables.TaxVouchersTypes.FacturaConsumoElectrónica:
                        voucherResult = await GenerateTaxVoucherAsync(IBS.itcSystemGlobalVariables.TaxVouchersTypes.FacturasDeConsumo);
                        break;
                    case IBS.itcSystemGlobalVariables.TaxVouchersTypes.NotaDebitoElectronica:
                        voucherResult = await GenerateTaxVoucherAsync(IBS.itcSystemGlobalVariables.TaxVouchersTypes.NotaDebito);
                        break;
                    case IBS.itcSystemGlobalVariables.TaxVouchersTypes.NotaCreditoElectronica:
                        voucherResult = await GenerateTaxVoucherAsync(IBS.itcSystemGlobalVariables.TaxVouchersTypes.NotaCredito);
                        break;
                    case IBS.itcSystemGlobalVariables.TaxVouchersTypes.ComprasElectrónico:
                        voucherResult = await GenerateTaxVoucherAsync(IBS.itcSystemGlobalVariables.TaxVouchersTypes.ProveedoresInformales);
                        break;
                    case IBS.itcSystemGlobalVariables.TaxVouchersTypes.GastosMenoresElectrónico:
                        voucherResult = await GenerateTaxVoucherAsync(IBS.itcSystemGlobalVariables.TaxVouchersTypes.GastosMenores);
                        break;
                    case IBS.itcSystemGlobalVariables.TaxVouchersTypes.RegímenesEspecialesElectrónico:
                        voucherResult = await GenerateTaxVoucherAsync(IBS.itcSystemGlobalVariables.TaxVouchersTypes.RegimenesEspecialesDeTributacion);
                        break;
                    case IBS.itcSystemGlobalVariables.TaxVouchersTypes.GubernamentalElectrónico:
                        voucherResult = await GenerateTaxVoucherAsync(IBS.itcSystemGlobalVariables.TaxVouchersTypes.ComprobantesGubernamentales);
                        break;
                    case IBS.itcSystemGlobalVariables.TaxVouchersTypes.ExportaciónElectrónico:
                        voucherResult = await GenerateTaxVoucherAsync(IBS.itcSystemGlobalVariables.TaxVouchersTypes.ComprobanteExportaciones);
                        break;
                    case IBS.itcSystemGlobalVariables.TaxVouchersTypes.PagosExteriorElectrónico:
                        voucherResult = await GenerateTaxVoucherAsync(IBS.itcSystemGlobalVariables.TaxVouchersTypes.ComprobantesInternacionales);
                        break;
                }
                return new Tuple<string, DateTime>(voucherResult.Item1, voucherResult.Item2);
            }
            else
            {
                throw new Abp.UI.UserFriendlyException(L("ThereIsNotAvailableSequenceFor", taxVoucherType.CodeAndDescription.ToString(), remainingQuantity.ToString()));

            }
            //Updating TaxVoucher Sequences
            remainingQuantity -= 1;
            var taxVouchers = _taxVoucherRepository.Get(currentTaxVoucher.Id);
            taxVouchers.CurrentSequence = currentSequence;
            taxVouchers.RemainingQuantity = remainingQuantity;
            _taxVoucherRepository.Update(taxVouchers);

        }


        return new Tuple<string, DateTime>(taxVoucherNumber, ExpirationDate);
    }
}
