using System.ComponentModel.DataAnnotations;

namespace VoucherWarehouse.Users.Dto;

public class ChangeUserLanguageDto
{
    [Required]
    public string LanguageName { get; set; }
}