namespace VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;

public sealed record class ValidateTrackIdDto
{
    public long Id { get; set; }

    public int StatusId { get; set; }
    public string TrackId { get; set; }
}
