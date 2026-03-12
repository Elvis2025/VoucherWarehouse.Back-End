using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.Features.Dto;

public sealed record class AddFeatureToTenantsInputDto
{
    [Required]
    public string FeatureName { get; set; }

    [Required]
    public string Value { get; set; }

    [Required]
    public List<int> TenantIds { get; set; } = new();
}
