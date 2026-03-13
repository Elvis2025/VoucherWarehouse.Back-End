namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;


public class ReceivePurchaseECFInputDto
{
    public int printFormat { get; set; }
    public bool sendPrintedFile { get; set; }
    public EncabezadoPurchase encabezado { get; set; }
    public List<DetallesItem> detallesItems { get; set; }

    public List<Subtotales> subtotales { get; set; }
    public List<DescuentosORecargo> descuentosORecargos { get; set; }

}

public class EncabezadoPurchase : EncabezadoSales
{
    //public decimal totalITBISRetenido { get; set; }
    //public decimal totalISRRetencion { get; set; }
    //public decimal totalITBISPercepcion { get; set; }
    //public decimal totalISRPercepcion { get; set; }
}
