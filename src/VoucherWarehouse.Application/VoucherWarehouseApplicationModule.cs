using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;
using IBS.VoucherWarehouse.AI.Ollama.Services.Implementations;
using IBS.VoucherWarehouse.Authorization;

namespace IBS.VoucherWarehouse;

[DependsOn(
    typeof(VoucherWarehouseCoreModule),
    typeof(AbpAutoMapperModule))]
public class VoucherWarehouseApplicationModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Authorization.Providers.Add<VoucherWarehouseAuthorizationProvider>();
    }

    public override void Initialize()
    {
        var thisAssembly = typeof(VoucherWarehouseApplicationModule).GetAssembly();

        IocManager.RegisterAssemblyByConvention(thisAssembly);

        //IocManager.Register<IDocumentIndexQueue, DocumentIndexQueue>(DependencyLifeStyle.Singleton);
        //IocManager.Register<IDocumentIndexingService, DocumentIndexingService>(DependencyLifeStyle.Transient);
        //IocManager.Register<IDocumentRegistryStore, DocumentRegistryStore>(DependencyLifeStyle.Transient);
        //IocManager.Register<IDocumentTextExtractorResolver, DocumentTextExtractorResolver>(DependencyLifeStyle.Transient);

        //IocManager.Register<IDocumentTextExtractor, DocxDocumentTextExtractor>(DependencyLifeStyle.Transient);
        //IocManager.Register<IDocumentTextExtractor, HtmlDocumentTextExtractor>(DependencyLifeStyle.Transient);
        //IocManager.Register<IDocumentTextExtractor, PdfDocumentTextExtractor>(DependencyLifeStyle.Transient);
        //IocManager.Register<IDocumentTextExtractor, PlainTextDocumentTextExtractor>(DependencyLifeStyle.Transient);
        //IocManager.Register<IDocumentTextExtractor, XlsxDocumentTextExtractor>(DependencyLifeStyle.Transient);

        //IocManager.Register<ITextChunker, TextChunker>(DependencyLifeStyle.Transient);
        //IocManager.Register<IRagQueryService, RagQueryService>(DependencyLifeStyle.Transient);
        //IocManager.Register<IQdrantVectorStore, QdrantVectorStore>(DependencyLifeStyle.Transient);
        //IocManager.Register<IOllamaEmbeddingClient, OllamaEmbeddingClient>(DependencyLifeStyle.Transient);
        //IocManager.Register<IOllamaChatClient, OllamaChatClient>(DependencyLifeStyle.Transient);
        //IocManager.Register<IFileHashService, FileHashService>(DependencyLifeStyle.Transient);

        Configuration.Modules.AbpAutoMapper().Configurators.Add(
            cfg => cfg.AddMaps(thisAssembly)
        );
    }
}