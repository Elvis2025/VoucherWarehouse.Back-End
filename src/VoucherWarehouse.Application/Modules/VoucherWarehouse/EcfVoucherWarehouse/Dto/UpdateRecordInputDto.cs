namespace VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;

public sealed record class UpdateRecordInputDto
{
    public string TrackId { get; set; }
    public string QrCodeUrl { get; set; }
    public long EntityId { get; set; }
    public string Code { get; set; }
    public string Message { get; set; }
    /// <summary>
    /// SecurityCode
    /// Codigo de seguridad formado con los ultimos 6 Digitos del Has que la firma digital
    /// </summary>
    public string SecurityCode { get; set; }
    /// <summary>
    /// SignatureDate
    /// Fecha y hora de la firma Digital del documento en formato dd-MM-yyyy HH:mm:ss; Zona horaria  GMT -4 
    /// </summary>
    public string SignatureDate { get; set; }

    public int? StatusId { get; set; }
    public string EntityName { get; set; }

    /// <summary>
    /// Indica si la Secuencia del Comprobante fue aceptada por la DGII
    /// </summary>
    public bool UsedSequence { get; set; }
}
