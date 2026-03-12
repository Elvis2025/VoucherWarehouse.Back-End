using Abp.Application.Features;
using VoucherWarehouse.Authorization.Modules.CoreSystem;
using VoucherWarehouse.Features;

namespace IBS.IThotSystem.Features;

public class AppFeatureProvider : FeatureProvider
{
    public override void SetFeatures(IFeatureDefinitionContext context)
    {
        IbsFeatures.Instance.Set(context);
        CoreSystemFeatures.Instance.Set(context);

    }

}
