using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.Features.Dto;

public sealed record class AddFeaturesToOneTenantInputDto
{
    [Required]
    public int TenantId { get; set; }

    [Required]
    public List<FeatureValueInputDto> Features { get; set; } = new();
}

public class FeatureValueInputDto
{
    [Required]
    public string FeatureName { get; set; }

    [Required]
    public string Value { get; set; }
}
