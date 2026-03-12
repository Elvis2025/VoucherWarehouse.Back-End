namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;

public sealed record class ErrorDto
{
    public string Code { get; set; }
    public string Details { get; set; }
    public string Message { get; set; }
    List<ValidationError> ValidationErrors = new List<ValidationError>();
}

public sealed record class ValidationError
{
    public string Message { get; set; }
    public string Members { get; set; }
}

