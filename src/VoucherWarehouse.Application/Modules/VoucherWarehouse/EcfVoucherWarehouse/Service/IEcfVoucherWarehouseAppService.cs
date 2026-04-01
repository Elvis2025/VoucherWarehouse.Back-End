using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;
using Microsoft.AspNetCore.Mvc;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Service;

public interface IEcfVoucherWarehouseAppService : IApplicationService,IAsyncCrudAppService<EcfVoucherWarehouseOutputDto,long,EcfVoucherWarehouseInputDto,EcfVoucherWarehouseCreateDto,EcfVoucherWarehouseUpdateDto>
{
    Task<Guid> LoadExcelAsync([FromForm] ImportDgiiExcelRequestDto input);
    Task ProcessAsync(DgiiExcelImportDto row, int rowNumber);
    Task<EcfVoucherOutputDto> ReceiveSalesResumeECFAsync(ReceiveSalesEcfInputDto input);
    Task<EcfVoucherOutputDto> SendCancelSequenceEcfToDGIIAsync(CancelSequenceEcfInputDto input);
    Task<EcfVoucherOutputDto> SendCommercialApprovalEcfToDGIIAsync(CommercialApprovalEcfInputDto input);
    Task<EcfVoucherOutputDto> SendCreditNoteEcfToDGIIAsync(ReceiveCreditNoteECFInputDto input);
    Task<EcfVoucherOutputDto> SendDebitNoteEcfToDGIIAsync(ReceiveCreditNoteECFInputDto input);
    Task<EcfVoucherOutputDto> SendPurchaseEcfToDGIIAsync(ReceivePurchaseECFInputDto input);
    Task<EcfVoucherOutputDto> SendSalesEcfToDGIIAsync(ReceiveSalesEcfInputDto input);
}
