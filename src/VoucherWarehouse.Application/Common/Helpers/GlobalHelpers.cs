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
