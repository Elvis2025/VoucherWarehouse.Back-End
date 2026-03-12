namespace IBS.VoucherWarehouse.Features.Dto;

public sealed record class FeatureDto
{
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public string DefaultValue { get; set; }

    public string CurrentValue { get; set; }

    public bool HasCustomValueForTenant { get; set; }
}
