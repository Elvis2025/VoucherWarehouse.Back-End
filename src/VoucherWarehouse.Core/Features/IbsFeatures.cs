using Abp.Application.Features;
using Abp.Runtime.Validation;
using Abp.UI.Inputs;
using System;
using VoucherWarehouse.Authorization.Abstractions;

namespace VoucherWarehouse.Features;

public sealed class IbsFeatures : FeatureBase
{
    private static readonly Lazy<IbsFeatures> instance = new Lazy<IbsFeatures>(() => new IbsFeatures());

    public static IbsFeatures Instance => instance.Value;

    private IbsFeatures() { }


    public override void Set(IFeatureDefinitionContext context)
    {
        var reports = context.Create(
            AppFeaturesDefinitions.Reports,
            defaultValue: "false",
            displayName: L("Reports"),
            inputType: new CheckboxInputType()
        );

        reports.CreateChildFeature(
            AppFeaturesDefinitions.Reports_ExportExcel,
            defaultValue: "false",
            displayName: L("ReportsExportExcel"),
            inputType: new CheckboxInputType()
        );

        reports.CreateChildFeature(
            AppFeaturesDefinitions.Reports_MaxRows,
            defaultValue: "1000",
            displayName: L("ReportsMaxRows"),
            inputType: new SingleLineStringInputType(new NumericValueValidator(1, 100000))
        );
    }
}
