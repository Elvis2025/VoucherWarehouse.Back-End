using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;
using Microsoft.AspNetCore.Mvc;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Service;

public interface IEcfVoucherWarehouseAppService : IApplicationService,IAsyncCrudAppService<EcfVoucherWarehouseOutputDto,int,EcfVoucherWarehouseInputDto,EcfVoucherWarehouseCreateDto,EcfVoucherWarehouseUpdateDto>
{
    Task LoadExcelAsync([FromForm] EcfVoucherWarehouseAppService.ImportDgiiExcelRequestDto input);
    Task<EcfVoucherOutputDto> ReceiveSalesResumeECFAsync(ReceiveSalesEcfInputDto input);
    Task<EcfVoucherOutputDto> SendCancelSequenceEcfToDGIIAsync(CancelSequenceEcfInputDto input);
    Task<EcfVoucherOutputDto> SendCommercialApprovalEcfToDGIIAsync(CommercialApprovalEcfInputDto input);
    Task<EcfVoucherOutputDto> SendCreditNoteEcfToDGIIAsync(ReceiveCreditNoteECFInputDto input);
    Task<EcfVoucherOutputDto> SendDebitNoteEcfToDGIIAsync(ReceiveCreditNoteECFInputDto input);
    Task<EcfVoucherOutputDto> SendPurchaseEcfToDGIIAsync(ReceivePurchaseECFInputDto input);
    Task<EcfVoucherOutputDto> SendSalesEcfToDGIIAsync(ReceiveSalesEcfInputDto input);
}
