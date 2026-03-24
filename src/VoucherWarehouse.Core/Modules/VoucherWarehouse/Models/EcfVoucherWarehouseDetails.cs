using IBS.VoucherWarehouse.Abstractions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;

[Table("EcfVoucherWarehouseDetails")]
public class EcfVoucherWarehouseDetails : BaseEntity<long>
{
    public EcfVoucherWarehouseDetails()
    {
        ItemCodes = new HashSet<EcfVoucherWarehouseDetailItemCode>();
        Subquantities = new HashSet<EcfVoucherWarehouseDetailSubquantity>();
        Discounts = new HashSet<EcfVoucherWarehouseDetailDiscount>();
        Surcharges = new HashSet<EcfVoucherWarehouseDetailSurcharge>();
        AdditionalTaxes = new HashSet<EcfVoucherWarehouseDetailAdditionalTax>();
        OtherCurrencies = new HashSet<EcfVoucherWarehouseDetailOtherCurrency>();
    }

    public long EcfVoucherWarehouseId { get; set; }

    public int NumeroLinea { get; set; }
    public int IndicadorFacturacion { get; set; }

    [Required]
    [StringLength(255)]
    public string NombreItem { get; set; }

    public int IndicadorBienoServicio { get; set; }

    [StringLength(1000)]
    public string DescripcionItem { get; set; }

    public decimal CantidadItem { get; set; }
    public int UnidadMedida { get; set; }
    public decimal CantidadReferencia { get; set; }
    public int UnidadReferencia { get; set; }
    public decimal GradosAlcohol { get; set; }
    public decimal PrecioUnitarioReferencia { get; set; }
    public decimal PrecioUnitarioItem { get; set; }
    public decimal DescuentoMonto { get; set; }
    public decimal RecargoMonto { get; set; }
    public decimal MontoItem { get; set; }

    public virtual EcfVoucherWarehouse EcfVoucherWarehouse { get; set; }
    public virtual EcfVoucherWarehouseDetailRetention Retention { get; set; }

    public virtual ICollection<EcfVoucherWarehouseDetailItemCode> ItemCodes { get; set; }
    public virtual ICollection<EcfVoucherWarehouseDetailSubquantity> Subquantities { get; set; }
    public virtual ICollection<EcfVoucherWarehouseDetailDiscount> Discounts { get; set; }
    public virtual ICollection<EcfVoucherWarehouseDetailSurcharge> Surcharges { get; set; }
    public virtual ICollection<EcfVoucherWarehouseDetailAdditionalTax> AdditionalTaxes { get; set; }
    public virtual ICollection<EcfVoucherWarehouseDetailOtherCurrency> OtherCurrencies { get; set; }
}
