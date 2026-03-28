using IBS.VoucherWarehouse.Common.Enums;

namespace IBS.VoucherWarehouse.Common.Helpers;

public static class VoucherTypesExtentions
{
    private  const int eCFLenght = 10;
    private  const int nCFLenght = 8;
    public static string GetName(this VoucherType vouchersTypes)
    {
        return (vouchersTypes) switch
        {
            VoucherType.E31 or VoucherType.B01 => "Factura de Crédito Fiscal",
            VoucherType.E32 or VoucherType.B02 => "Factura de Consumo",
            VoucherType.E33 or VoucherType.B03 => "Nota de Débito",
            VoucherType.E34 or VoucherType.B04 => "Nota de Crédito",
            VoucherType.E41 or VoucherType.B11 => "Comprobante de Compras",
            VoucherType.B12 => "Registro Único de Ingresos",
            VoucherType.E43 or VoucherType.B13 => "Registro de Gastos Menores",
            VoucherType.E44 or VoucherType.B14 => "Regímenes Especiales de Tributación",
            VoucherType.E45 or VoucherType.B15 => "Comprobante Gubernamental",
            VoucherType.E46 or VoucherType.B16 => "Comprobante de Exportaciones",
            VoucherType.E47 or VoucherType.B17 => "Comprobante para Pagos al Exterior",

            _ => throw new ArgumentOutOfRangeException(vouchersTypes.GetCode(), vouchersTypes.GetFullName(), null)

        };
    }
    public static string GetCode(this VoucherType vouchersTypes)
    {
        string code = vouchersTypes.ToString();
        return code;
    }

    public static string GetFullName(this VoucherType vouchersTypes)
    {
        
        return $"{vouchersTypes.GetName()} - {vouchersTypes.GetCode()}";
    }

    public static void SendAlert(this VoucherType voucherType, int currentremainingQuantity)
    {
        Task.Run(async () =>
        {
            long number = currentremainingQuantity;
           // string Message = L("MinimumAlertTaxVoucher{0}{1}", voucherType.GetFullName(), currentremainingQuantity);

            //var customData = new PersonalizedNotification.AppNotificationCustomData(AbpSession.TenantId,
            //    ItcCoreSystemNotificationName.MinimumAlertTaxVoucher,
            //    Message, type: AppNotificationType.All,
            //    severity: Abp.Notifications.NotificationSeverity.Warn);

            //customData.MessageProperties.Add("taxVoucherCode", TaxVoucherCode);
            //customData.SmsProperties.Add(AppNotificationCustomDataConst.ShortMessageSms, Message);

            //await _appSystemNotifier.SendNotificationGroupAsync(customData);
        });
    }

    public static bool IsElectronic(this VoucherType vouchersTypes)
    {
        return vouchersTypes.ToString().Contains("E");
    }

    public static int SecuenceLenght(this VoucherType vouchersTypes)
    {
        return vouchersTypes.IsElectronic() ? eCFLenght : nCFLenght;
    }
    
    public static string GenerateTaxVoucherNumber(this VoucherType voucherType, int currentSecuence = 0)
    {
        string currentSecuenceParsed = currentSecuence.ToString().PadLeft(voucherType.SecuenceLenght(), '0');
        string taxVoucherNumber = $"{voucherType.GetCode()}{currentSecuenceParsed}".Trim();
        return taxVoucherNumber;
    }
    
    public static string Format(this VoucherType vouchersTypes)
    {
        return vouchersTypes.GetCode();
    }
    
    public static VoucherType ToVoucherType(this string voucherTypeCode)
    {
        if (Enum.TryParse<VoucherType>(voucherTypeCode, true, out var result))
            return result;

        throw new ArgumentException($"Tipo de comprobante inválido: {voucherTypeCode}");
    }
    
    public static VoucherType ToVoucherType(this int voucherTypeCode)
    {
        if (Enum.TryParse<VoucherType>($"{voucherTypeCode}", true, out var result))
            return result;

        throw new ArgumentException($"Tipo de comprobante inválido: {voucherTypeCode}");
    }
    


}
