namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;

public sealed record class AuthenticateInputDto
{
    public string BaseUrlIbsApiDgii { get; set; }
    public string AuthenticateUrlIbsApiDgii { get; set; }
    public string TenancyName { get; set; }
    public string UsernameOrEmailAddress { get; set; }
    public string Password { get; set; }
}
