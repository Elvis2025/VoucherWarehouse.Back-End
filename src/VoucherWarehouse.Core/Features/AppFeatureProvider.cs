using Abp.Application.Features;
using IBS.VoucherWarehouse.Authorization.Modules.CoreSystem;
using IBS.VoucherWarehouse.Features;
      
namespace IBS.VoucherWarehouse.Features;

public class AppFeatureProvider : FeatureProvider
{
    public override void SetFeatures(IFeatureDefinitionContext context)
    {
        IbsFeatures.Instance.Set(context);
        CoreSystemFeatures.Instance.Set(context);

    }

}
