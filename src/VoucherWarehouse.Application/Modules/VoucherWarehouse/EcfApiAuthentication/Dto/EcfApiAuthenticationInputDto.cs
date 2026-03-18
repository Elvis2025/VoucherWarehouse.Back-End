using IBS.VoucherWarehouse.Abstractions;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfApiAuthentication.Dto;

public sealed record class EcfApiAuthenticationInputDto : BaseInputEntityDto<int>, IShouldNormalize
{
    public void Normalize()
    {
        
    }
}
