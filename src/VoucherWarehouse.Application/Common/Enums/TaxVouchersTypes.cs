namespace IBS.VoucherWarehouse.Common.Enums;
/// <summary>
/// Tipos de comprobantes validos por la DGII
/// </summary>
public enum VoucherType
{
    #region e-CF Types
   
    /// <summary>
    /// Factura de Crédito Fiscal Electrónica - Formato: E31XXXXXXXXXX
    /// </summary>
    E31 = 31,
    /// <summary>
    /// Factura de Consumo Electrónica - Formato: E32XXXXXXXXXX
    /// </summary>
    E32,
    /// <summary>
    /// Nota de Débito Electrónica - Formato: E33XXXXXXXXXX
    /// </summary>
    E33,
    /// <summary>
    /// Nota de Crédito Electrónica - Formato: E34XXXXXXXXXX
    /// </summary>
    E34,
    /// <summary>
    /// Comprobante de Compras Electrónico - Formato: E41XXXXXXXXXX
    /// </summary>
    E41 = 41,
    /// <summary>
    /// Registro de Gastos Menores Electrónico - Formato: E43XXXXXXXXXX
    /// </summary>
    E43 = 43,
    /// <summary>
    /// Regímenes Especiales de Tributación Electrónico - Formato: E44XXXXXXXXXX
    /// </summary>
    E44,
    /// <summary>
    /// Comprobante Gubernamental Electrónico - Formato: E45XXXXXXXXXX
    /// </summary>
    E45,
    /// <summary>
    /// Comprobante de Exportaciones Electrónico - Formato: E46XXXXXXXXXX
    /// </summary>
    E46,
    /// <summary>
    /// Comprobante para Pagos al Exterior Electrónico - Formato: E47XXXXXXXXXX
    /// </summary>
    E47,
    #endregion

    #region NCF Types
    
    /// <summary>
    /// Factura de Crédito Fiscal - Formato: B01XXXXXXXX
    /// </summary>
    B01 = 01,
    /// <summary>
    /// Factura de Consumo - Formato: B02XXXXXXXX
    /// </summary>
    B02,
    /// <summary>
    /// Nota de Débito - Formato: B03XXXXXXXX
    /// </summary>
    B03,
    /// <summary>
    /// Nota de Crédito - Formato: B04XXXXXXXX
    /// </summary>
    B04,
    /// <summary>
    /// Comprobante de Compras - Formato: B11XXXXXXXX
    /// </summary>
    B11 = 11,
    /// <summary>
    /// Registro Único de Ingresos (No tiene versión electrónica) - Formato: B12XXXXXXXX
    /// </summary>
    B12,
    /// <summary>
    /// Registro de Gastos Menores - Formato: B13XXXXXXXX
    /// </summary>
    B13,
    /// <summary>
    /// Regímenes Especiales de Tributación - Formato: B14XXXXXXXX
    /// </summary>
    B14,
    /// <summary>
    /// Comprobante Gubernamental - Formato: B15XXXXXXXX
    /// </summary>
    B15,
    /// <summary>
    /// Comprobante de Exportaciones - Formato: B16XXXXXXXX
    /// </summary>
    B16,
    /// <summary>
    /// Comprobante para Pagos al Exterior - Formato: B17XXXXXXXX
    /// </summary>
    B17
    #endregion
}