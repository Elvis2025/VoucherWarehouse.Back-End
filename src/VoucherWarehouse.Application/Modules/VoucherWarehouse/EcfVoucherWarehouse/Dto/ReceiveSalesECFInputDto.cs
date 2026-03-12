using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcSystem.EcfEVoucher.Dto
{
    public  class ReceiveSalesEcfInputDto
    {
        public int printFormat { get; set; }
        public bool sendPrintedFile { get; set; }
        public string authenticationServiceUrl { get; set; }
        public string receptionServiceUrl { get; set; }
        public string comercialApprovalServiceUrl { get; set; }
        public EncabezadoSales encabezado { get; set; }
        public List<DetallesItem> detallesItems { get; set; }

        public List<Subtotales> subtotales { get; set; }
        public List<DescuentosORecargo> descuentosORecargos { get; set; }
    }

    public class DescuentosORecargo
    {
        public int numeroLinea { get; set; }
        [StringLength(1)]
        public string tipoAjuste { get; set; }
        public int? indicadorNorma1007 { get; set; }
        [StringLength(45)]
        public string descripcionDescuentooRecargo { get; set; }
        public tipoSubDescuento tipoValor { get; set; }
        public decimal valorDescuentooRecargo { get; set; }
        public decimal montoDescuentooRecargo { get; set; }
        public decimal montoDescuentooRecargoOtraMoneda { get; set; }
        public int indicadorFacturacionDescuentooRecargo { get; set; }
    }

    public class DetallesItem
    {

        public int numeroLinea { get; set; }
        public List<TablaCodigosItem> tablaCodigosItem { get; set; }
        public int indicadorFacturacion { get; set; }
        [StringLength(80)]
        public string nombreItem { get; set; }
        public int indicadorBienoServicio { get; set; }

        public string descripcionItem { get; set; }
        public decimal cantidadItem { get; set; }
        public int unidadMedida { get; set; }
        public decimal cantidadReferencia { get; set; }
        public int unidadReferencia { get; set; }

        public List<TablaSubcantidad> tablaSubcantidad { get; set; }
        public decimal? gradosAlcohol { get; set; }
        public decimal PrecioUnitarioReferencia { get; set; }
        public decimal precioUnitarioItem { get; set; }
        public decimal descuentoMonto { get; set; }
        public List<TablaSubDescuento> tablaSubDescuento { get; set; }
        public decimal recargoMonto { get; set; }
        public List<TablaSubRecargo> tablaSubRecargo { get; set; }
        public List<TablaImpuestoAdicional> tablaImpuestoAdicional { get; set; }
        public OtramonedasDetalles otramonedasDetalles { get; set; }
        public Retencion retencion { get; set; }
        public decimal montoItem { get; set; }
    }

    public class Retencion
    {
        public int indicadorAgenteRetencionoPercepcion { get; set; }
        public decimal montoISRRetenido { get; set; }
        public decimal montoITBISRetenido { get; set; }
    }

    public class TablaCodigosItem
    {
        public string tipoCodigo { get; set; }
        public string codigoItem { get; set; }
    }

    public class EncabezadoSales
    {
        public IdDoc idDoc { get; set; }
        [Required]
        public Emisor emisor { get; set; }
        public Comprador comprador { get; set; }
        [Required]
        public Totales totales { get; set; }
        public Otramonedas otraMoneda { get; set; }

        public InformacionesAdicionales informacionesAdicionales { get; set; }
        public Transporte transporte { get; set; }
    }

    public class Comprador
    {
        public string rNCComprador { get; set; }
        //[Required]
        [StringLength(150)]
        public string razonSocialComprador { get; set; }
        public string identificadorExtranjero { get; set; }
        public string contactoComprador { get; set; }
        public string correoComprador { get; set; }
        public string direccionComprador { get; set; }
        public string municipioComprador { get; set; }
        public string provinciaComprador { get; set; }
        public string fechaEntrega { get; set; }
        public string fechaOrdenCompra { get; set; }
        public string numeroOrdenCompra { get; set; }
        public string codigoInternoComprador { get; set; }
        public string contactoEntrega { get; set; }
        public string direccionEntrega { get; set; }
        public string telefonoAdicional { get; set; }
    }

    public class Emisor
    {
        [Required]
        public string rNCEmisor { get; set; }
        [Required]
        [StringLength(150)]
        public string razonSocialEmisor { get; set; }
        [StringLength(150)]
        public string nombreComercial { get; set; }
        [Required]
        [StringLength(100)]
        public string direccionEmisor { get; set; }
        public string municipio { get; set; }
        public string provincia { get; set; }
        public string[] tablaTelefonoEmisor { get; set; }
        public string correoEmisor { get; set; }

        public string webSite { get; set; }
        public string codigoVendedor { get; set; }
        [Required]
        [StringLength(10)]
        public string fechaEmision { get; set; }
        [StringLength(20)]
        public string numeroFacturaInterna { get; set; }
        public string numeroPedidoInterno { get; set; }
        public string zonaVenta { get; set; }
    }

    public class IdDoc
    {
        [Required]
        [StringLength(2)]
        public string tipoeCF { get; set; }
        [Required]
        [StringLength(13)]
        public string eNCF { get; set; }
        [StringLength(10)]
        public string fechaVencimientoSecuencia { get; set; }
        public string indicadorNotaCredito { get; set; }
        public int? indicadorMontoGravado { get; set; }
        [StringLength(2)]
        public string tipoIngresos { get; set; }
        [Required]
        public int tipoPago { get; set; }
        [StringLength(10)]
        public string fechaLimitePago { get; set; }
        public string terminoPago { get; set; }
        public List<TablaFormasPago> tablaFormasPago { get; set; }
        [StringLength(2)]
        public string tipoCuentaPago { get; set; }
        [StringLength(28)]
        public string numeroCuentaPago { get; set; }
        [StringLength(75)]
        public string bancoPago { get; set; }
    }

    public class Totales
    {

        public decimal montoGravadoI1 { get; set; }
        public decimal montoGravadoI2 { get; set; }
        public decimal montoGravadoI3 { get; set; }
        public decimal montoGravadoTotal { get; set; }
        public decimal montoExento { get; set; }
        public decimal? iTBIS1 { get; set; }
        public decimal? iTBIS2 { get; set; }
        public decimal? iTBIS3 { get; set; }
        public decimal totalITBIS1 { get; set; }
        public decimal totalITBIS2 { get; set; }
        public decimal totalITBIS3 { get; set; }

        public decimal totalITBISRetenido { get; set; }
        public decimal totalISRRetencion { get; set; }
        public decimal totalITBISPercepcion { get; set; }
        public decimal totalISRPercepcion { get; set; }


        public decimal totalITBIS { get; set; }
        public decimal? montoImpuestoAdicional { get; set; }
        public List<ImpuestosAdicional> impuestosAdicionales { get; set; }
        [Required]
        public decimal montoTotal { get; set; }
        public decimal montoNoFacturable { get; set; }

        public decimal valorPagar { get; set; }
    }

    public class ImpuestosAdicional
    {
        [StringLength(3)]
        public string tipoImpuesto { get; set; }
        public decimal tasaImpuestoAdicional { get; set; }
        public decimal montoImpuestoSelectivoConsumoEspecifico { get; set; }
        public decimal montoImpuestoSelectivoConsumoAdvalorem { get; set; }
        public decimal? otrosImpuestosAdicionales { get; set; }
    }

    public class Otramonedas
    {
        [StringLength(3)]
        public string tipoMoneda { get; set; }
        public decimal tipoCambio { get; set; }
        public decimal montoGravadoTotalOtraMoneda { get; set; }
        public decimal MontoGravado1OtraMoneda { get; set; }
        public decimal MontoGravado2OtraMoneda { get; set; }
        public decimal MontoGravado3OtraMoneda { get; set; }
        public decimal montoExentoOtraMoneda { get; set; }
        public decimal totalITBISOtraMoneda { get; set; }
        public decimal totalITBIS1OtraMoneda { get; set; }
        public decimal totalITBIS2OtraMoneda { get; set; }
        public decimal totalITBIS3OtraMoneda { get; set; }
        public decimal? montoImpuestoAdicionalOtraMoneda { get; set; }
        public List<impuestosAdicionalesOtraMoneda> impuestosAdicionalesOtraMoneda { get; set; }
        public decimal montoTotalOtraMoneda { get; set; }
    }

    public class impuestosAdicionalesOtraMoneda
    {
        [StringLength(3)]
        public string tipoImpuestoOtraMoneda { get; set; }
        public decimal tasaImpuestoAdicionalOtraMoneda { get; set; }
        public decimal montoImpuestoAdicionalOtraMoneda { get; set; }
        public decimal montoImpuestoSelectivoConsumoAvaloremOtraMoneda { get; set; }
        public decimal otrosImpuestosAdicionalesOtraMoneda { get; set; }
    }

    public class TablaSubDescuento
    {
        public tipoSubDescuento tipoSubDescuento { get; set; }
        public decimal subDescuentoPorcentaje { get; set; }
        public decimal montoSubDescuento { get; set; }
    }

    public class TablaSubRecargo
    {
        public tipoSubDescuento tipoSubRecargo { get; set; }
        public decimal subRecargoPorcentaje { get; set; }
        public decimal montoSubRecargo { get; set; }
    }

    public class TablaImpuestoAdicional {
        [StringLength(3)]
        public string tipoImpuesto { get; set; }
    }

    public class OtramonedasDetalles
    {
        public decimal precioOtraMoneda { get; set; }
        public decimal descuentoOtraMoneda { get; set; }
        public decimal recargoOtraMoneda { get; set; }
        public decimal montoItemOtraMoneda { get; set; }
    }

    public class TablaSubcantidad
    {
        public decimal subcantidad { get; set; }
        public int codigoSubcantidad { get; set; }
    }

    public class InformacionesAdicionales
    {
        public string numeroContenedor { get; set; }
        public string numeroReferencia { get; set; }
        public string fechaEmbarque { get; set; }
        public string numeroEmbarque { get; set; }
        public string nombrePuertoEmbarque { get; set; }
        public string condicionesEntrega { get; set; }
        public decimal totalFob { get; set; }
        public decimal seguro { get; set; }
        public decimal flete { get; set; }
        public decimal totalCif { get; set; }
        public string regimenAduanero { get; set; }
        public string nombrePuertoSalida { get; set; }
        public string nombrePuertoDesembarque { get; set; }
        public decimal pesoBruto { get; set; }
        public decimal pesoNeto { get; set; }
        public int unidadPesoBruto { get; set; }
        public int unidadPesoNeto { get; set; }
        public decimal cantidadBulto { get; set; }
        public int unidadBulto { get; set; }
        public decimal volumenBulto { get; set; }
        public int unidadVolumen { get; set; }
    }
    public class Transporte
    {
        public int viaTransporte { get; set; }
        public string paisOrigen { get; set; }
        public string direccionDestino { get; set; }
        public string paisDestino { get; set; }
        public string numeroAlbaran { get; set; }
    }

    public class TablaFormasPago
    {
        public int formaPago { get; set; }

        public decimal montoPago { get; set; }

    }

    public class Subtotales
    {
        public int numeroSubTotal { get; set; }

        public string descripcionSubtotal { get; set; }

        public int orden { get; set; }

        public decimal subTotalExento { get; set; }

        public decimal montoSubTotal { get; set; }

        public int lineas { get; set; }

    }

    public enum tipoSubDescuento
    {
        /// <summary>
        /// $
        /// </summary>
            Amount = 0,
        /// <summary>
        /// %
        /// </summary>
            Percenet = 1
    }
}
