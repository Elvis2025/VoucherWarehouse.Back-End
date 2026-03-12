namespace VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;

public sealed record class ResultDto
{
    public long Id { get; set; }

    public string TrackId { get; set; }
    public Exception Exception { get; set; }

    public string Message{ get; set; }

    public string MessageError { get; set; }
    public string MessageInfo { get; set; }

    public string Code { get; set; }

    public string QrCodeUrl { get; set; }
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

    public bool UsedSequence { get; set; }
    public bool Success { get; set; }
}
