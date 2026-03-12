using System.ComponentModel.DataAnnotations;

namespace IBS.VoucherWarehouse.Modules.CoreSystem.Users.Dto;

public class ChangeUserLanguageDto
{
    [Required]
    public string LanguageName { get; set; }
}