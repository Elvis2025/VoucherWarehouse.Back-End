using IBS.VoucherWarehouse.Debugging;

namespace IBS.VoucherWarehouse;

public class VoucherWarehouseConsts
{
    public const string LocalizationSourceName = "IBS.VoucherWarehouse";

    public const string ConnectionStringName = "Default";

    public const bool MultiTenancyEnabled = true;


    /// <summary>
    /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
    /// </summary>
    public static readonly string DefaultPassPhrase =
        DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "3105608d742f431d97bacdb1af8f4805";
}

public static class ResponseCodeStatusAPI_IBS_DGII
{
    public const string Success = "000";
    public const string InvalidEcfType = "001";
    public const string InvalidAmountForEcTypef32 = "002";
    public const string InvalidTaxVoucherTypeFormat = "003";
    public const string InvalidDateFormat = "004";
    public const string ReferenceInformationNotFound = "005";
    public const string AditionalTaxTypeInvalid = "006";
    public const string InvalidIndentificationNumberEmisor = "007";
    public const string ErrorAutenticateExternalService = "008";
    public const string VoucherInformationNotValid = "009";
    public const string XmlFilesPathNotFound = "010";
    public const string InvalidXmlStructure = "011";
    public const string CancelSequencesNoSuccess = "012";
    public const string ErrorSigningXmlFile = "013";
    public const string ErrorOnSerializedDtoToXml = "014";
    public const string CertificateSettingNotFound = "015";
    public const string TrackIdNotFound = "016";
    public const string TrackIdRejected = "017";
    public const string TrackIdInProcess = "018";
    public const string ConditionallyAccepted = "019";
    public const string UnHandledError = "999";
}

public static class ITBISForDGII
{
    public const decimal ITBIS1 = 18;
    public const decimal ITBIS2 = 16;
    public const decimal ITBIS3 = 0;
}