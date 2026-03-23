using Abp.AspNetCore;
using Abp.AspNetCore.Mvc.Antiforgery;
using Abp.AspNetCore.SignalR.Hubs;
using Abp.Castle.Logging.Log4Net;
using Abp.Extensions;
using Castle.Facilities.Logging;
using IBS.VoucherWarehouse.AI.Ollama.Dto;
using IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;
using IBS.VoucherWarehouse.AI.Ollama.Services.Implementations;
using IBS.VoucherWarehouse.Configuration;
using IBS.VoucherWarehouse.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
namespace IBS.VoucherWarehouse.Web.Host.Startup;

public class Startup
{
    private const string _defaultCorsPolicyName = "localhost";

    private const string _swaggerApiVersion = "v1";
    private const string _ibsApiVersion = "v1.0.0.0";

    private readonly IConfigurationRoot _appConfiguration;
    private readonly IWebHostEnvironment _hostingEnvironment;

    public Startup(IWebHostEnvironment env)
    {
        _hostingEnvironment = env;
        _appConfiguration = env.GetAppConfiguration();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        //MVC
        services.AddControllersWithViews(options =>
        {
            options.Filters.Add(new AbpAutoValidateAntiforgeryTokenAttribute());
        });

        IdentityRegistrar.Register(services);
        AuthConfigurer.Configure(services, _appConfiguration);

        services.AddSignalR();

        // Configure CORS for angular2 UI
        services.AddCors(
            options => options.AddPolicy(
                _defaultCorsPolicyName,
                builder => builder
                    .WithOrigins(
                        // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                        _appConfiguration["App:CorsOrigins"]
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.RemovePostFix("/"))
                            .ToArray()
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
            )
        );

        // Swagger - Enable this line and the related lines in Configure method to enable swagger UI
        ConfigureSwagger(services);

        // Configure Abp and Dependency Injection
        services.AddAbpWithoutCreatingServiceProvider<VoucherWarehouseWebHostModule>(
            // Configure Log4Net logging
            options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                f => f.UseAbpLog4Net().WithConfig(_hostingEnvironment.IsDevelopment()
                    ? "log4net.config"
                    : "log4net.Production.config"
                )
            )
        );

        services.AddHttpContextAccessor();

        services.AddSingleton<IDocumentIndexQueue, DocumentIndexQueue>();
        services.AddTransient<IDocumentIndexingService, DocumentIndexingService>();
        services.AddTransient<IDocumentRegistryStore, DocumentRegistryStore>();
        services.AddTransient<IDocumentTextExtractorResolver, DocumentTextExtractorResolver>();

        services.AddTransient<IDocumentTextExtractor, DocxDocumentTextExtractor>();
        services.AddTransient<IDocumentTextExtractor, HtmlDocumentTextExtractor>();
        services.AddTransient<IDocumentTextExtractor, PdfDocumentTextExtractor>();
        services.AddTransient<IDocumentTextExtractor, PlainTextDocumentTextExtractor>();
        services.AddTransient<IDocumentTextExtractor, XlsxDocumentTextExtractor>();

        services.AddTransient<ITextChunker, TextChunker>();
        services.AddTransient<IRagQueryService, RagQueryService>();
        services.AddTransient<IFileHashService, FileHashService>();

        services.Configure<AiDocumentIndexingOptionsDto>(
           _appConfiguration.GetSection(AiDocumentIndexingOptionsDto.SectionName));

        services.Configure<OllamaOptionsDto>(
            _appConfiguration.GetSection(OllamaOptionsDto.SectionName));

        services.Configure<QdrantOptionsDto>(
            _appConfiguration.GetSection(QdrantOptionsDto.SectionName));


        // HttpClient - Ollama Embeddings
        services.AddHttpClient<IOllamaEmbeddingClient, OllamaEmbeddingClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<OllamaOptionsDto>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromMinutes(options.EmbeddingTimeoutMinutes);
        });

        // HttpClient - Ollama Chat
        services.AddHttpClient<IOllamaChatClient, OllamaChatClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<OllamaOptionsDto>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromMinutes(options.ChatTimeoutMinutes);
        });

        // HttpClient - Qdrant
        services.AddHttpClient<IQdrantVectorStore, QdrantVectorStore>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<QdrantOptionsDto>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromMinutes(2);
        });

        // Hosted services
        services.AddHostedService<AiWarmupHostedService>();
        services.AddHostedService<FolderBootstrapIndexerHostedService>();
        services.AddHostedService<FolderWatcherHostedService>();
        services.AddHostedService<DocumentIndexWorkerHostedService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        app.UseAbp(options => { options.UseAbpRequestLocalization = false; }); // Initializes ABP framework.

        app.UseCors(_defaultCorsPolicyName); // Enable CORS!

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseAbpRequestLocalization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<AbpCommonHub>("/signalr");
            endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            endpoints.MapControllerRoute("defaultWithArea", "{area}/{controller=Home}/{action=Index}/{id?}");
        });

        // Enable middleware to serve generated Swagger as a JSON endpoint
        app.UseSwagger(c => { c.RouteTemplate = "swagger/{documentName}/swagger.json"; });

        // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
        app.UseSwaggerUI(options =>
        {
            // specifying the Swagger JSON endpoint.
            options.SwaggerEndpoint($"/swagger/{_swaggerApiVersion}/swagger.json", $"IBS · Voucher Warehouse API {_ibsApiVersion}");
            options.IndexStream = () => Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("IBS.VoucherWarehouse.Web.Host.wwwroot.swagger.ui.index.html");
            options.DisplayRequestDuration(); // Controls the display of the request duration (in milliseconds) for "Try it out" requests.
        }); // URL: /swagger
    }

    private void ConfigureSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(_swaggerApiVersion, new OpenApiInfo
            {
                Version = _ibsApiVersion,
                Title = "IBS · Voucher Warehouse API",
                Description = "Voucher Warehouse",
                // uncomment if needed TermsOfService = new Uri("https://example.com/terms"),
                Contact = new OpenApiContact
                {
                    Name = "IBS · Voucher Warehouse",
                    Email = string.Empty,
                    Url = new Uri("http://localhost:4200"),
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://github.com/aspnetboilerplate/aspnetboilerplate/blob/dev/LICENSE.md"),
                }
            });
            options.DocInclusionPredicate((docName, description) => true);

            // Define the BearerAuth scheme that's in use
            options.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme()
            {
                Description =
                    "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });

            //add summaries to swagger
            bool canShowSummaries = _appConfiguration.GetValue<bool>("Swagger:ShowSummaries");
            if (canShowSummaries)
            {
                var hostXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var hostXmlPath = Path.Combine(AppContext.BaseDirectory, hostXmlFile);
                options.IncludeXmlComments(hostXmlPath);

                var applicationXml = $"IBS.VoucherWarehouse.Application.xml";
                var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, applicationXml);
                options.IncludeXmlComments(applicationXmlPath);

                var webCoreXmlFile = $"IBS.VoucherWarehouse.Web.Core.xml";
                var webCoreXmlPath = Path.Combine(AppContext.BaseDirectory, webCoreXmlFile);
                options.IncludeXmlComments(webCoreXmlPath);
            }
        });
    }
}
