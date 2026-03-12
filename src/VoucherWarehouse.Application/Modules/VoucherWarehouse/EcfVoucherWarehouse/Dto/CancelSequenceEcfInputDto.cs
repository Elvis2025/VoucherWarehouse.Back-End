namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;

public sealed record class CancelSequenceEcfInputDto
{
   public Encabezado encabezado { get; set; }
   public DetalleAnulacion[] detalleAnulacion { get; set; }
}

public sealed record class Encabezado
{        
    public string rncEmisor { get; set; }
    public int cantidadeNCFAnulados { get; set; }
    public string fechaHoraAnulacioneNCF { get; set; }

}

public sealed record class DetalleAnulacion
{
    public string [] noLinea { get; set; }
    public string tipoeCF { get; set; }       
    public int cantidadeNCFAnulados { get; set; }
    public RangoSequences[] tablaRangoSecuenciasAnuladaseNCF { get; set; }
}
public sealed record class RangoSequences
{
    public string secuenciaeNCFDesde { get; set; }
    public string secuenciaeNCFHasta { get; set; }
}
