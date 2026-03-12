using Abp.Application.Features;
using Abp.UI.Inputs;
using IBS.VoucherWarehouse.Authorization.Abstractions;
using System;

namespace IBS.VoucherWarehouse.Authorization.Modules.CoreSystem;

public sealed class CoreSystemFeatures : FeatureBase
{
    private static readonly Lazy<CoreSystemFeatures> instance = new Lazy<CoreSystemFeatures>(() => new CoreSystemFeatures());

    public static CoreSystemFeatures Instance => instance.Value;

    private CoreSystemFeatures() { }

    public override void Set(IFeatureDefinitionContext context)
    {

        context.Create(CoreSystemFeatureNames.GroupName,
                        defaultValue: "false",
                        displayName: L("CoreSystem"),
                        inputType: new CheckboxInputType());

    }
}
