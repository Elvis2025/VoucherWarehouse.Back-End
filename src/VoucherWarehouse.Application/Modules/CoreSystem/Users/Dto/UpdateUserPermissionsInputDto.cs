namespace IBS.VoucherWarehouse.Modules.CoreSystem.Users.Dto;

public sealed record class UpdateUserPermissionsInputDto : IEntityDto
{
    public List<string> GrantedPermissionNames { get; set; } = new List<string>();
    public int Id { get; set; }
}
