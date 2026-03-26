using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.Common.GlobalHelpers;

public static class GlobalHelpers
{
    public static class Error
    {
        /// <summary>
        /// Get all inner errors asociated to this exception
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        //public static string GetInnerExceptionError(Exception ex)
        //{
        //    string result = "";
        //    if (ex is System.Data.Entity.Validation.DbEntityValidationException _ex)
        //    {
        //        result += $"{ex.Message}\n";
        //        foreach (var item in _ex.EntityValidationErrors)
        //        {
        //            result += $"{item.Entry.Entity.GetType().Name},IsValid:{item.IsValid}, {string.Join(",", item.ValidationErrors.Select(t => t.PropertyName + ":" + t.ErrorMessage))}";
        //            if (_ex.InnerException != null)
        //                result += "\n" + GetInnerExceptionError(_ex.InnerException);
        //        }

        //    }
        //    else
        //    {
        //        result += $"{ex.Message}";
        //        if (ex.InnerException != null)
        //            result += "\n" + GetInnerExceptionError(ex.InnerException);
        //    }

        //    return result;
        //}
    }

    public static class TaxVouchersTypes
    {
        public const string FacturasDeCreditoFiscal = "01";
        public const string FacturasDeConsumo = "02";
        public const string NotaDebito = "03";
        public const string NotaCredito = "04";
        public const string ProveedoresInformales = "11";
        public const string RegistroUnicoIngresos = "12";
        public const string GastosMenores = "13";
        public const string RegimenesEspecialesDeTributacion = "14";
        public const string ComprobantesGubernamentales = "15";
        public const string ComprobanteExportaciones = "16";
        public const string ComprobantesInternacionales = "17";

        ///Tipos electronicos
        public const string NotaCreditoElectronica = "34";
        public const string NotaDebitoElectronica = "33";
        public const string FacturaCréditoFiscalElectrónico = "31";
        public const string FacturaConsumoElectrónica = "32";
        public const string ComprasElectrónico = "41";
        public const string GastosMenoresElectrónico = "43";
        public const string RegímenesEspecialesElectrónico = "44";
        public const string GubernamentalElectrónico = "45";
        public const string ExportaciónElectrónico = "46";
        public const string PagosExteriorElectrónico = "47";


    }
    public static string FormattedStatusDgii(this string value)
    {
        switch (value)
        {
            case "000":
                return "Aceptado";
            case "019":
                return "Aceptado Condicional";


            default:
                return "Rechazado";
        } 
    }


}
