using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Service;

public interface IEcfVoucherWarehouseAppService : IApplicationService,IAsyncCrudAppService<EcfVoucherWarehouseOutputDto,int,EcfVoucherWarehouseInputDto,EcfVoucherWarehouseCreateDto,EcfVoucherWarehouseUpdateDto>
{
    Task<EcfVoucherOutputDto> SendCancelSequenceEcfToDGIIAsync(CancelSequenceEcfInputDto input);
    Task<EcfVoucherOutputDto> SendCommercialApprovalEcfToDGIIAsync(CommercialApprovalEcfInputDto input);
    Task<EcfVoucherOutputDto> SendCreditNoteEcfToDGIIAsync(ReceiveCreditNoteECFInputDto input);
    Task<EcfVoucherOutputDto> SendDebitNoteEcfToDGIIAsync(ReceiveCreditNoteECFInputDto input);
    Task<EcfVoucherOutputDto> SendPurchaseEcfToDGIIAsync(ReceivePurchaseECFInputDto input);
    Task<EcfVoucherOutputDto> SendSalesEcfToDGIIAsync(ReceiveSalesEcfInputDto input);
}
