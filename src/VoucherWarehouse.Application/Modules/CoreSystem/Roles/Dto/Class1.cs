namespace VoucherWarehouse.Modules.CoreSystem.Roles.Dto;

public sealed record class PermissionTreeDto
{
    public PermissionTreeDto()
    {
        Children = new List<PermissionTreeDto>();
    }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public bool IsGranted { get; set; }

    public List<PermissionTreeDto> Children { get; set; }
}
