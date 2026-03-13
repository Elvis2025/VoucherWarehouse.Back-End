using IBS.VoucherWarehouse.Abstractions;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfApiAuthentication.Dto;

[AutoMapper.AutoMap(typeof(Models.EcfApiAuthentication))]
public sealed record class EcfApiAuthenticationDto : BaseEntityDto<int>
{
    public string TenancyName { get; set; }
    public string UsernameOrEmailAddress { get; set; }
    public string Password { get; set; }
    public string AuthUrl { get; set; }
    public string BaseUrl { get; set; }
}
