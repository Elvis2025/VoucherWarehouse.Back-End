using System.ComponentModel.DataAnnotations;

namespace IBS.VoucherWarehouse.Configuration.Dto;

public class ChangeUiThemeInput
{
    [Required]
    [StringLength(32)]
    public string Theme { get; set; }
}
