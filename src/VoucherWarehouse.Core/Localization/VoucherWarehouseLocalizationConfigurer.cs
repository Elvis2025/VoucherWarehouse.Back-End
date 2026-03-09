using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace VoucherWarehouse.Localization;

public static class VoucherWarehouseLocalizationConfigurer
{
    public static void Configure(ILocalizationConfiguration localizationConfiguration)
    {
        localizationConfiguration.Sources.Add(
            new DictionaryBasedLocalizationSource(VoucherWarehouseConsts.LocalizationSourceName,
                new XmlEmbeddedFileLocalizationDictionaryProvider(
                    typeof(VoucherWarehouseLocalizationConfigurer).GetAssembly(),
                    "VoucherWarehouse.Localization.SourceFiles"
                )
            )
        );
    }
}
