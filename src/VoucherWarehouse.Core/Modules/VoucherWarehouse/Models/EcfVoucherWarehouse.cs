using IBS.VoucherWarehouse.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;
[Table("EcfVoucherWarehouse")]
public class EcfVoucherWarehouse : BaseEntity<long>
{
    public EcfVoucherWarehouse()
    {
        PaymentForms = new HashSet<EcfVoucherWarehousePaymentForm>();
        EmitterPhones = new HashSet<EcfVoucherWarehouseEmitterPhone>();
        AdditionalTaxes = new HashSet<EcfVoucherWarehouseAdditionalTax>();
        Details = new HashSet<EcfVoucherWarehouseDetails>();
        Subtotals = new HashSet<EcfVoucherWarehouseSubtotal>();
        GlobalAdjustments = new HashSet<EcfVoucherWarehouseGlobalAdjustment>();
    }

    // Root
    public int PrintFormat { get; set; }
    public bool SendPrintedFile { get; set; }

    [StringLength(500)]
    public string AuthenticationServiceUrl { get; set; }

    [StringLength(500)]
    public string ReceptionServiceUrl { get; set; }

    [StringLength(500)]
    public string ComercialApprovalServiceUrl { get; set; }

    // IdDoc
    [Required]
    [StringLength(10)]
    public string TipoECF { get; set; }

    [Required]
    [StringLength(50)]
    public string ENCF { get; set; }

    public DateTime? FechaVencimientoSecuencia { get; set; }
    public int? IndicadorNotaCredito { get; set; }
    public int IndicadorMontoGravado { get; set; }

    [StringLength(10)]
    public string TipoIngresos { get; set; }

    public int TipoPago { get; set; }
    public DateTime? FechaLimitePago { get; set; }

    [StringLength(200)]
    public string TerminoPago { get; set; }

    [StringLength(10)]
    public string TipoCuentaPago { get; set; }

    [StringLength(100)]
    public string NumeroCuentaPago { get; set; }

    [StringLength(200)]
    public string BancoPago { get; set; }

    // Emisor
    [Required]
    [StringLength(20)]
    public string RNCEmisor { get; set; }

    [Required]
    [StringLength(255)]
    public string RazonSocialEmisor { get; set; }

    [StringLength(255)]
    public string NombreComercial { get; set; }

    [StringLength(500)]
    public string DireccionEmisor { get; set; }

    [StringLength(20)]
    public string MunicipioEmisor { get; set; }

    [StringLength(20)]
    public string ProvinciaEmisor { get; set; }

    [StringLength(255)]
    public string CorreoEmisor { get; set; }

    [StringLength(255)]
    public string WebSite { get; set; }

    [StringLength(100)]
    public string CodigoVendedor { get; set; }

    public DateTime? FechaEmision { get; set; }

    [StringLength(50)]
    public string NumeroFacturaInterna { get; set; }

    [StringLength(50)]
    public string NumeroPedidoInterno { get; set; }

    [StringLength(50)]
    public string ZonaVenta { get; set; }

    // Comprador
    [StringLength(20)]
    public string RNCComprador { get; set; }

    [StringLength(255)]
    public string RazonSocialComprador { get; set; }

    [StringLength(50)]
    public string IdentificadorExtranjero { get; set; }

    [StringLength(255)]
    public string ContactoComprador { get; set; }

    [StringLength(255)]
    public string CorreoComprador { get; set; }

    [StringLength(500)]
    public string DireccionComprador { get; set; }

    [StringLength(20)]
    public string MunicipioComprador { get; set; }

    [StringLength(20)]
    public string ProvinciaComprador { get; set; }

    public DateTime? FechaEntrega { get; set; }
    public DateTime? FechaOrdenCompra { get; set; }

    [StringLength(50)]
    public string NumeroOrdenCompra { get; set; }

    [StringLength(100)]
    public string CodigoInternoComprador { get; set; }

    [StringLength(255)]
    public string ContactoEntrega { get; set; }

    [StringLength(500)]
    public string DireccionEntrega { get; set; }

    [StringLength(50)]
    public string TelefonoAdicional { get; set; }

    // Totales
    public decimal MontoGravadoI1 { get; set; }
    public decimal MontoGravadoI2 { get; set; }
    public decimal MontoGravadoI3 { get; set; }
    public decimal MontoGravadoTotal { get; set; }
    public decimal MontoExento { get; set; }
    public decimal ITBIS1 { get; set; }
    public decimal? ITBIS2 { get; set; }
    public decimal? ITBIS3 { get; set; }
    public decimal TotalITBIS1 { get; set; }
    public decimal TotalITBIS2 { get; set; }
    public decimal TotalITBIS3 { get; set; }
    public decimal TotalITBISRetenido { get; set; }
    public decimal TotalISRRetencion { get; set; }
    public decimal TotalITBISPercepcion { get; set; }
    public decimal TotalISRPercepcion { get; set; }
    public decimal TotalITBIS { get; set; }
    public decimal MontoImpuestoAdicional { get; set; }
    public decimal MontoTotal { get; set; }
    public decimal MontoNoFacturable { get; set; }
    public decimal ValorPagar { get; set; }

    // Otra moneda
    [StringLength(10)]
    public string OtraMonedaTipoMoneda { get; set; }

    public decimal? OtraMonedaTipoCambio { get; set; }
    public decimal? OtraMonedaMontoGravadoTotal { get; set; }
    public decimal? OtraMonedaMontoTotal { get; set; }

    // Bloques opcionales
    public string InformacionesAdicionalesJson { get; set; }
    public string TransporteJson { get; set; }



    [StringLength(50)]
    public string Status { get; set; }

    // Respuesta DGII
    [StringLength(100)]
    public string DgiiTrackId { get; set; }

    [StringLength(20)]
    public string DgiiResponseCode { get; set; }

    [StringLength(1000)]
    public string DgiiResponseMessage { get; set; }

    [StringLength(2000)]
    public string DgiiQrCodeUrl { get; set; }

    public bool? DgiiUsedSequence { get; set; }

    public DateTime? DgiiReceivedDate { get; set; }

    [StringLength(100)]
    public string DgiiSecurityCode { get; set; }

    public DateTime? DgiiSignatureDate { get; set; }

    public string DgiiPrintFile { get; set; }

    // Navigation
    public virtual ICollection<EcfVoucherWarehousePaymentForm> PaymentForms { get; set; }
    public virtual ICollection<EcfVoucherWarehouseEmitterPhone> EmitterPhones { get; set; }
    public virtual ICollection<EcfVoucherWarehouseAdditionalTax> AdditionalTaxes { get; set; }
    public virtual ICollection<EcfVoucherWarehouseDetails> Details { get; set; }
    public virtual ICollection<EcfVoucherWarehouseSubtotal> Subtotals { get; set; }
    public virtual ICollection<EcfVoucherWarehouseGlobalAdjustment> GlobalAdjustments { get; set; }
}

//IDE