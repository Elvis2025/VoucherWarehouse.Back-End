namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;

public sealed record class AuthenticationResponseOutputDto
{
    public bool IsSuccess { get; set; }
    public bool UnAuthorizedRequest { get; set; }
    public ResultResponse Result  { get; set; }
    public ErrorResponse Error { get; set; }
}

public sealed record  class ResultResponse
{
    public DateTime Expires { get; set; }
    public DateTime Issued { get; set; }
    public string PasswordResetCode { get; set; }
    public string Token { get; set; }
}

public sealed record  class ErrorResponse
{
    public string Code { get; set; }
    public string Message { get; set; }
    public string[] ValidationErrors { get; set; }
}
