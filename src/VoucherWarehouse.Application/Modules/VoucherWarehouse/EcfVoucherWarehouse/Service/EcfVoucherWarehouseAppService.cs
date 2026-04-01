using Abp.BackgroundJobs;
using Abp.Collections.Extensions;
using Abp.Domain.Uow;
using Abp.Runtime.Caching;
using Abp.Timing;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using IBS.VoucherWarehouse.Common.Constants;
using IBS.VoucherWarehouse.Common.GlobalHelpers;
using IBS.VoucherWarehouse.Common.Helpers;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfApiAuthentication.Service;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.BackgroundWorker;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.ExcelManager;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Mappers;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucher.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Service;
//[AbpAuthorize(VoucherWarehouseNamePermissions.EcfVoucherWarehouse.Default)]
public class EcfVoucherWarehouseAppService : VoucherWarehouseAppServiceBase, IEcfVoucherWarehouseAppService, IEcfVoucherRowProcessor
{
    private readonly IRepository<Models.EcfVoucherWarehouse, long> ecfVoucherWarehouseRepository;
    private readonly IEcfApiAuthenticationAppService ecfApiAuthenticationService;
    private readonly ITaxVoucherAppService taxVoucherAppService;
    private readonly IRepository<EcfVoucherDocumentJob, Guid> ecfVoucherDocumentJobRepository;
    private readonly IBackgroundJobManager backgroundJobManager;
    private readonly IWebHostEnvironment environment;
    private readonly IEcfVoucherDocumentJobManagerService ecfVoucherDocumentJobManagerService;

    public EcfVoucherWarehouseAppService(IRepository<Models.EcfVoucherWarehouse, long> ecfVoucherWarehouseRepository,
                                         IEcfApiAuthenticationAppService ecfApiAuthenticationService,
                                         ICacheManager cacheManager,
                                         ITaxVoucherAppService taxVoucherAppService,
                                         IRepository<EcfVoucherDocumentJob, Guid> ecfVoucherDocumentJobRepository,
                                         IBackgroundJobManager backgroundJobManager,
                                         IWebHostEnvironment environment,
                                         IEcfVoucherDocumentJobManagerService ecfVoucherDocumentJobManagerService

                                         )
    {
        this.ecfVoucherWarehouseRepository = ecfVoucherWarehouseRepository;
        this.ecfApiAuthenticationService = ecfApiAuthenticationService;
        this.taxVoucherAppService = taxVoucherAppService;
        this.ecfVoucherDocumentJobRepository = ecfVoucherDocumentJobRepository;
        this.backgroundJobManager = backgroundJobManager;
        this.environment = environment;
        this.ecfVoucherDocumentJobManagerService = ecfVoucherDocumentJobManagerService;
    }

    #region CRUD Async
    [AbpAuthorize(VoucherWarehouseNamePermissions.EcfVoucherWarehouse.Read)]
    public async Task<EcfVoucherWarehouseOutputDto> GetAsync(EntityDto<long> input)
    {
        try
        {
            var ecfVoucherWarehouse = await ecfVoucherWarehouseRepository.GetAsync(input.Id);

            return MapEntityToOutputTwoWay.Auto.Map(ecfVoucherWarehouse);
        }
        catch (Exception)
        {

            throw;
        }
    }
    public async Task<PagedResultDto<EcfVoucherWarehouseOutputDto>> GetAllAsync(EcfVoucherWarehouseInputDto input)
    {
        try
        {
            var ecfVoucherWarehouse = await ecfVoucherWarehouseRepository.GetAllListAsync();
            var ecfVoucherWarehouseFiltered = ecfVoucherWarehouse.OrderByDescending(x => x.Id)
                                                                 .ToList();

            if (input.FilterText is not null)
            {
                ecfVoucherWarehouseFiltered = ecfVoucherWarehouseFiltered.Where(x => x.TipoECF.Contains(input.FilterText) ||
                                                                             x.ENCF.Contains(input.FilterText) ||
                                                                             x.RazonSocialComprador.Contains(input.FilterText) ||
                                                                             x.RazonSocialEmisor.Contains(input.FilterText) ||
                                                                             x.RNCEmisor.Contains(input.FilterText))
                                                                 .ToList();

            }

            ecfVoucherWarehouseFiltered = ecfVoucherWarehouseFiltered.Skip(input.SkipCount)
                                                             .Take(input.MaxResultCount)
                                                             .ToList();





            return MapEntityToOutputTwoWay.Auto.MapToPagedResult(ecfVoucherWarehouseFiltered, ecfVoucherWarehouse.Count);
        }
        catch (Exception)
        {

            throw;
        }
    }
    [AbpAuthorize(VoucherWarehouseNamePermissions.EcfVoucherWarehouse.Create)]
    public async Task<EcfVoucherWarehouseOutputDto> CreateAsync(EcfVoucherWarehouseCreateDto input)
    {
        try
        {
            var enitity = MapEntityToCreateTwoWay.Auto.ReverseMap(input);

            var ecfVoucherWarehouse = await ecfVoucherWarehouseRepository.InsertAsync(MapEntityToCreateTwoWay.Auto.ReverseMap(input));

            return MapEntityToOutputTwoWay.Auto.Map(ecfVoucherWarehouse);
        }
        catch (Exception)
        {

            throw;
        }
    }
    [AbpAuthorize(VoucherWarehouseNamePermissions.EcfVoucherWarehouse.Update)]
    public async Task<EcfVoucherWarehouseOutputDto> UpdateAsync(EcfVoucherWarehouseUpdateDto input)
    {
        try
        {
            var enitity = MapEntityToUpdateTwoWay.Auto.ReverseMap(input);

            var ecfVoucherWarehouse = await ecfVoucherWarehouseRepository.UpdateAsync(MapEntityToUpdateTwoWay.Auto.ReverseMap(input));

            return MapEntityToOutputTwoWay.Auto.Map(ecfVoucherWarehouse);
        }
        catch (Exception)
        {

            throw;
        }
    }
    [AbpAuthorize(VoucherWarehouseNamePermissions.EcfVoucherWarehouse.Delete)]
    public async Task DeleteAsync(EntityDto<long> input)
    {
        try
        {

            await ecfVoucherWarehouseRepository.DeleteAsync(input.Id);
        }
        catch (Exception)
        {

            throw;
        }
    }


    #endregion



    public async Task<EcfVoucherOutputDto> SendCreditNoteEcfToDGIIAsync(ReceiveCreditNoteECFInputDto input)
    {

        //AuthenticateInputDto _authenticateAPIParams = new();
        var _authenticateAPIParams = await ecfApiAuthenticationService.GetFirstOrDefaultAsync();
        var __result = await ecfApiAuthenticationService.AuthenticateAPIAsync();
        string result = string.Empty;
        EcfVoucherOutputDto output = new EcfVoucherOutputDto();
        try
        {
            ReceiveCreditNoteECFInputDto objToSend = new ReceiveCreditNoteECFInputDto();

            string jsonObject = System.Text.Json.JsonSerializer.Serialize(input);
            string url = @_authenticateAPIParams.BaseUrl + "ReceiveCreditNoteEcf";

            var client = new System.Net.Http.HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", __result.Result.Token);
            var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            result = await response.Content.ReadAsStringAsync();

            output = JsonConvert.DeserializeObject<EcfVoucherOutputDto>(result);
            //Tomamos el response.StatusCode y el response.ReasonPhrase
            if (output.Result == null && output.Error == null)
            {
                output = new EcfVoucherOutputDto { Error = new ErrorDto { Code = ResponseCodeStatusAPI_IBS_DGII.UnHandledError, Message = L("UnHandledError") } };
            }
        }
        catch (System.Exception ex)
        {
            result = ex.ToString();
            output = new EcfVoucherOutputDto { Error = new ErrorDto { Code = ResponseCodeStatusAPI_IBS_DGII.UnHandledError, Message = result } };

        }
        return output;
    }

    public async Task<EcfVoucherOutputDto> SendDebitNoteEcfToDGIIAsync(ReceiveCreditNoteECFInputDto input)
    {
        //AuthenticateInputDto _authenticateAPIParams = new();
        var _authenticateAPIParams = await ecfApiAuthenticationService.GetFirstOrDefaultAsync();
        var __result = await ecfApiAuthenticationService.AuthenticateAPIAsync();
        string result = string.Empty;
        EcfVoucherOutputDto output = new EcfVoucherOutputDto();
        try
        {
            string jsonObject = System.Text.Json.JsonSerializer.Serialize(input);
            string url = @_authenticateAPIParams.BaseUrl + "ReceiveDebitNoteEcf";

            var client = new System.Net.Http.HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", __result.Result.Token);
            var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
            var response = client.PostAsync(url, content).Result;

            result = response.Content.ReadAsStringAsync().Result;

            output = JsonConvert.DeserializeObject<EcfVoucherOutputDto>(result);
            //Error no manejado
            if (output.Result == null && output.Error == null)
            {
                output = new EcfVoucherOutputDto { Error = new ErrorDto { Code = ResponseCodeStatusAPI_IBS_DGII.UnHandledError, Message = L("UnHandledError") } };
            }
        }
        catch (System.Exception ex)
        {
            result = ex.ToString();
            output = new EcfVoucherOutputDto { Error = new ErrorDto { Code = ResponseCodeStatusAPI_IBS_DGII.UnHandledError, Message = result } };
        }
        return output;
    }

    public async Task<EcfVoucherOutputDto> SendSalesEcfToDGIIAsync(ReceiveSalesEcfInputDto input)
    {
        //AuthenticateInputDto _authenticateAPIParams = new();
        var _authenticateAPIParams = await ecfApiAuthenticationService.GetFirstOrDefaultAsync();
        var __result = await ecfApiAuthenticationService.AuthenticateAPIAsync();
        string result = string.Empty;
        EcfVoucherOutputDto output = new EcfVoucherOutputDto();
        try
        {
            ReceiveSalesEcfInputDto objToSend = new ReceiveSalesEcfInputDto();

            string jsonObject = System.Text.Json.JsonSerializer.Serialize(input);
            string url = @_authenticateAPIParams.BaseUrl + "ReceiveSalesEcf";

            var client = new System.Net.Http.HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", __result.Result.Token);
            var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {

                output = JsonConvert.DeserializeObject<EcfVoucherOutputDto>(responseBody);
                await SaveEcfVoucherAsync(input, output);
                return output;
            }

            result = await response.Content.ReadAsStringAsync();

            output = JsonConvert.DeserializeObject<EcfVoucherOutputDto>(result);
            //Error no manejado
            if (output.Result == null && output.Error == null)
            {
                output = new EcfVoucherOutputDto { Error = new ErrorDto { Code = ResponseCodeStatusAPI_IBS_DGII.UnHandledError, Message = L("UnHandledError") } };
            }
            await SaveEcfVoucherAsync(input, output);
            //Justo aqui debajo debes proceder a insertar en la base de datos la transaccion que viajo a la DGII
        }
        catch (Exception ex)
        {
            result = ex.ToString();
            output = new EcfVoucherOutputDto { Error = new ErrorDto { Code = ResponseCodeStatusAPI_IBS_DGII.UnHandledError, Message = result } };

        }



        return output;

    }

    public async Task<EcfVoucherOutputDto> SendPurchaseEcfToDGIIAsync(ReceivePurchaseECFInputDto input)
    {

        //AuthenticateInputDto _authenticateAPIParams = new();
        var _authenticateAPIParams = await ecfApiAuthenticationService.GetFirstOrDefaultAsync();
        var __result = await ecfApiAuthenticationService.AuthenticateAPIAsync();
        string result = string.Empty;
        EcfVoucherOutputDto output = new EcfVoucherOutputDto();
        try
        {
            ReceivePurchaseECFInputDto objToSend = new ReceivePurchaseECFInputDto();

            string jsonObject = System.Text.Json.JsonSerializer.Serialize(input);
            string url = @_authenticateAPIParams.BaseUrl + "ReceivePurchaseECF";

            var client = new System.Net.Http.HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", __result.Result.Token);
            var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            result = await response.Content.ReadAsStringAsync();

            output = JsonConvert.DeserializeObject<EcfVoucherOutputDto>(result);

            //Error no manejado
            if (output.Result == null && output.Error == null)
            {
                output = new EcfVoucherOutputDto { Error = new ErrorDto { Code = ResponseCodeStatusAPI_IBS_DGII.UnHandledError, Message = L("UnHandledError") } };
            }
        }
        catch (System.Exception ex)
        {
            result = ex.ToString();
            output = new EcfVoucherOutputDto { Error = new ErrorDto { Code = ResponseCodeStatusAPI_IBS_DGII.UnHandledError, Message = result } };

        }
        return output;
    }


    public async Task<EcfVoucherOutputDto> SendCancelSequenceEcfToDGIIAsync(CancelSequenceEcfInputDto input)
    {
        //AuthenticateInputDto _authenticateAPIParams = new();
        var _authenticateAPIParams = await ecfApiAuthenticationService.GetFirstOrDefaultAsync();
        var __result = await ecfApiAuthenticationService.AuthenticateAPIAsync();
        string result = string.Empty;
        EcfVoucherOutputDto output = new EcfVoucherOutputDto();
        try
        {
            CancelSequenceEcfInputDto objToSend = new CancelSequenceEcfInputDto();

            string jsonObject = System.Text.Json.JsonSerializer.Serialize(input);
            string url = @_authenticateAPIParams.BaseUrl + "CancelSequencesECF";

            var client = new System.Net.Http.HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", __result.Result.Token);
            var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            result = await response.Content.ReadAsStringAsync();

            output = JsonConvert.DeserializeObject<EcfVoucherOutputDto>(result);

            //Tomamos el response.StatusCode y el response.ReasonPhrase
            if (output.Result == null && output.Error == null)
            {
                output = new EcfVoucherOutputDto { Error = new ErrorDto { Code = response.StatusCode.ToString(), Message = response.ReasonPhrase } };
            }
        }
        catch (System.Exception ex)
        {
            result = ex.Message;
            output = JsonConvert.DeserializeObject<EcfVoucherOutputDto>(result);
        }
        return output;
    }

    public async Task<EcfVoucherOutputDto> SendCommercialApprovalEcfToDGIIAsync(CommercialApprovalEcfInputDto input)
    {
        //AuthenticateInputDto _authenticateAPIParams = new();
        var _authenticateAPIParams = await ecfApiAuthenticationService.GetFirstOrDefaultAsync();
        var __result = await ecfApiAuthenticationService.AuthenticateAPIAsync();
        string result = string.Empty;
        EcfVoucherOutputDto output = new EcfVoucherOutputDto();
        try
        {
            ReceivePurchaseECFInputDto objToSend = new ReceivePurchaseECFInputDto();

            string jsonObject = System.Text.Json.JsonSerializer.Serialize(input);
            string url = @_authenticateAPIParams.BaseUrl + "ComercialApproval";

            var client = new System.Net.Http.HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", __result.Result.Token);
            var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            result = await response.Content.ReadAsStringAsync();

            output = JsonConvert.DeserializeObject<EcfVoucherOutputDto>(result);
        }
        catch (System.Exception ex)
        {
            result = ex.ToString();
            output = new EcfVoucherOutputDto { Error = new ErrorDto { Code = ResponseCodeStatusAPI_IBS_DGII.UnHandledError, Message = result } };

        }
        return output;
    }

    public async Task<EcfVoucherOutputDto> ReceiveSalesResumeECFAsync(ReceiveSalesEcfInputDto input)
    {
        //Llenar el ReceiveSaleResumeInputDto con la informacion que llegue
        ReceiveSaleResumeInputDto resumeInput = new ReceiveSaleResumeInputDto();
        resumeInput = new ReceiveSaleResumeInputDto
        {
            sendPrintedFile = false,
            printFormat = input.printFormat,

            encabezado = new EncabezadoResume
            {
                idDoc = new IdDocResume
                {
                    tipoeCF = input.encabezado.idDoc.tipoeCF,
                    eNCF = input.encabezado.idDoc.eNCF,
                    tipoIngresos = input.encabezado.idDoc.tipoIngresos,
                    tipoPago = input.encabezado.idDoc.tipoPago,
                    fechaLimitePago = input.encabezado.idDoc.fechaLimitePago,
                    // tablaFormasPago = input.encabezado.idDoc.tablaFormasPago, 
                    tipoCuentaPago = string.Empty
                },
                emisor = new EmisorResume
                {
                    rNCEmisor = input.encabezado.emisor.rNCEmisor,
                    razonSocialEmisor = input.encabezado.emisor.razonSocialEmisor,
                    fechaEmision = input.encabezado.emisor.fechaEmision ?? string.Empty,
                },
                //comprador = new EcfEVoucher.Dto.CompradorResume
                //{
                //    identificadorExtranjero = input.encabezado.comprador.identificadorExtranjero,
                //    rNCComprador = input.encabezado.comprador.rNCComprador,
                //    razonSocialComprador = input.encabezado.comprador.razonSocialComprador
                //},
                totales = new TotalesResume
                {
                    montoGravadoI1 = input.encabezado.totales.montoGravadoI1,
                    montoGravadoI2 = input.encabezado.totales.montoGravadoI2,
                    montoGravadoI3 = input.encabezado.totales.montoGravadoI3,
                    montoGravadoTotal = input.encabezado.totales.montoGravadoTotal,
                    montoExento = input.encabezado.totales.montoExento,
                    totalITBIS1 = input.encabezado.totales.totalITBIS1,
                    totalITBIS2 = input.encabezado.totales.totalITBIS2,
                    totalITBIS3 = input.encabezado.totales.totalITBIS3,
                    totalITBIS = input.encabezado.totales.totalITBIS,
                    montoImpuestoAdicional = input.encabezado.totales.montoImpuestoAdicional ?? new(),
                    impuestosAdicionales = input.encabezado.totales.impuestosAdicionales,
                    montoNoFacturable = input.encabezado.totales.montoNoFacturable,
                    montoTotal = input.encabezado.totales.montoTotal,
                    montoPeriodo = (input.encabezado.totales.montoTotal + input.encabezado.totales.montoNoFacturable)
                }
            }
        };


        var _authenticateAPIParams = await ecfApiAuthenticationService.GetFirstOrDefaultAsync();
        var __result = await ecfApiAuthenticationService.AuthenticateAPIAsync(); string result = string.Empty;
        EcfVoucherOutputDto output = new EcfVoucherOutputDto();
        try
        {
            ReceiveSalesEcfInputDto objToSend = new ReceiveSalesEcfInputDto();

            string jsonObject = System.Text.Json.JsonSerializer.Serialize(resumeInput);
            string url = @_authenticateAPIParams.BaseUrl + "ReceiveSalesResumeEcf";

            var client = new System.Net.Http.HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", __result.Result.Token);
            var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Status: {(int)response.StatusCode} - {response.StatusCode}\nBody: {responseBody}");
            }

            output = JsonConvert.DeserializeObject<EcfVoucherOutputDto>(result);

            //Error no manejado
            if (output.Result == null && output.Error == null)
            {
                output = new EcfVoucherOutputDto { Error = new ErrorDto { Code = ResponseCodeStatusAPI_IBS_DGII.UnHandledError, Message = L("UnHandledError") } };
            }
        }
        catch (System.Exception ex)
        {
            result = ex.Message;
            output = JsonConvert.DeserializeObject<EcfVoucherOutputDto>(result);
        }
        return output;
    }





    public async Task<Guid> LoadExcelAsync([FromForm] ImportDgiiExcelRequestDto input)
    {
        if (input?.File == null || input.File.Length == 0)
        {
            throw new UserFriendlyException("Debes enviar un archivo.");
        }

        var extension = Path.GetExtension(input.File.FileName)?.ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(extension) ||
            (extension != FileExtension.Xls &&
             extension != FileExtension.Xlsx &&
             extension != FileExtension.Csv &&
             extension != FileExtension.Txt))
        {
            throw new UserFriendlyException("Solo se aceptan archivos (.xls, .xlsx, .csv, .txt).");
        }

        var jobId = Guid.NewGuid();

        var uploadsFolder = Path.Combine(
            environment.ContentRootPath,
            "App_Data",
            "Imports",
            "EcfVoucher");

        Directory.CreateDirectory(uploadsFolder);

        var storedFileName = $"{jobId}{extension}";
        var filePath = Path.Combine(uploadsFolder, storedFileName);

        await using (var fileStream = new FileStream(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true))
        {
            await input.File.CopyToAsync(fileStream);
        }

        var job = new EcfVoucherDocumentJob
        {
            Id = jobId,
            TenantId = AbpSession.TenantId,
            FileName = input.File.FileName,
            FilePath = filePath,
            Status = JobStatus.Pending,
            TotalRows = 0,
            ProcessedRows = 0,
            SuccessRows = 0,
            FailedRows = 0,
            ErrorMessage = null,
            StartTime = null,
            EndTime = null
        };

        await ecfVoucherDocumentJobRepository.InsertAsync(job);
        await CurrentUnitOfWork.SaveChangesAsync();

        await backgroundJobManager.EnqueueAsync<DgiiProcessEcfVoucher, ProcessDgiiImportJobArgs>(new ProcessDgiiImportJobArgs
        {
            JobId = jobId
        });

        return jobId;
    }






    public ReceiveSalesEcfInputDto MapToSaleEcf(DgiiExcelImportDto source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        var numeroPedidoInterno = Random.Shared.Next(10000, 100000);

        var dto = new ReceiveSalesEcfInputDto
        {
            sendPrintedFile = false,
            printFormat = 1,
            encabezado = new EncabezadoSales
            {
                idDoc = new IdDoc
                {
                    tipoeCF = SafeString(source.TipoeCF),
                    eNCF = source.ENCF,
                    fechaVencimientoSecuencia = source.FechaVencimientoSecuencia,
                    indicadorNotaCredito = null,
                    indicadorMontoGravado = source.IndicadorMontoGravado,
                    tipoIngresos = SafeString(source.TipoIngresos),
                    tipoPago = source.TipoPago,
                    fechaLimitePago = null,
                    terminoPago = null,
                    tablaFormasPago = (source.FormasPago ?? null)
                        .Select(x => new TablaFormasPago
                        {
                            formaPago = x?.FormaPago ?? 0,
                            montoPago = x?.MontoPago ?? 0m
                        })
                        .ToList(),
                    tipoCuentaPago = "NA",
                    numeroCuentaPago = null,
                    bancoPago = null
                },

                emisor = new Emisor
                {
                    rNCEmisor = SafeString(source.RNCEmisor),
                    razonSocialEmisor = SafeString(source.RazonSocialEmisor),
                    nombreComercial = null,
                    direccionEmisor = SafeString(source.DireccionEmisor),
                    municipio = null,
                    provincia = null,
                    tablaTelefonoEmisor = null,

                    correoEmisor = null,
                    webSite = null,
                    codigoVendedor = null,

                    fechaEmision = DateTime.Now.ToString("dd-MM-yyyy"),
                    numeroFacturaInterna = $"IBS-VW-{numeroPedidoInterno}",
                    numeroPedidoInterno = numeroPedidoInterno.ToString(),
                    zonaVenta = null
                },

                comprador = new Comprador
                {
                    rNCComprador = SafeString(source.RNCComprador),
                    razonSocialComprador = SafeString(source.RazonSocialComprador),
                    fechaEntrega = DateTime.Now.ToString("dd-MM-yyyy"),
                },

                transporte = null,

                totales = new Totales
                {
                    montoGravadoTotal = source.MontoGravadoTotal,
                    montoGravadoI1 = source.MontoGravadoI1,

                    // Inicializados por defecto para que nunca vayan nulos
                    montoGravadoI2 = 0m,
                    montoGravadoI3 = 0m,
                    montoExento = 0m,

                    iTBIS1 = source.ITBIS1 ?? 0m,
                    totalITBIS = source.TotalITBIS ?? 0m,
                    totalITBIS1 = source.TotalITBIS1 ?? 0m,

                    totalITBIS2 = 0m,
                    totalITBIS3 = 0m,

                    // ESTE CAMPO TE ESTABA FALLANDO.
                    montoImpuestoAdicional = 0m,
                    impuestosAdicionales = null,
                    montoNoFacturable = 0m,
                    montoTotal = source.MontoTotal ?? 0m,
                    valorPagar = source.MontoTotal ?? 0m,
                    //montoPeriodo = source.MontoTotal ?? 0m
                }
            },

            detallesItems = (source.Items ?? null)
                .Select(x => new DetallesItem
                {
                    numeroLinea = x?.NumeroLinea ?? x?.Numero ?? 0,
                    tablaCodigosItem = null,
                    indicadorFacturacion = x?.IndicadorFacturacion ?? 0,
                    nombreItem = SafeString(x?.NombreItem),
                    indicadorBienoServicio = x?.IndicadorBienoServicio ?? 0,
                    // ESTE CAMPO TE ESTABA FALLANDO.
                    gradosAlcohol = 0m,

                    cantidadItem = x?.CantidadItem ?? 0m,
                    unidadMedida = x?.UnidadMedida ?? null,
                    precioUnitarioItem = x?.PrecioUnitarioItem ?? 0m,
                    montoItem = x?.MontoItem ?? 0m
                })
                .ToList()
        };

        return dto;
    }
    private static string SafeString(object value)
    {
        return value?.ToString()?.Trim() ?? null;
    }

    private async Task SaveEcfVoucherAsync(
    ReceiveSalesEcfInputDto input,
    EcfVoucherOutputDto output)
    {
        var entity = MapToEcfVoucherWarehouseEntity(input, output);

        await ecfVoucherWarehouseRepository.InsertAsync(entity);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    private Models.EcfVoucherWarehouse MapToEcfVoucherWarehouseEntity(
    ReceiveSalesEcfInputDto input,
    EcfVoucherOutputDto output)
    {

        int.TryParse(input.encabezado?.idDoc?.indicadorNotaCredito, out int indicadorNotaCredito);
        var entity = new Models.EcfVoucherWarehouse
        {
            // Root
            PrintFormat = input.printFormat,
            SendPrintedFile = input.sendPrintedFile,
            AuthenticationServiceUrl = input.authenticationServiceUrl,
            ReceptionServiceUrl = input.receptionServiceUrl,
            ComercialApprovalServiceUrl = input.comercialApprovalServiceUrl,

            // IdDoc
            TipoECF = input.encabezado?.idDoc?.tipoeCF,
            ENCF = input.encabezado?.idDoc?.eNCF,
            FechaVencimientoSecuencia = ParseNullableDate(input.encabezado?.idDoc?.fechaVencimientoSecuencia),
            IndicadorNotaCredito = indicadorNotaCredito,
            IndicadorMontoGravado = input.encabezado?.idDoc?.indicadorMontoGravado ?? 0,
            TipoIngresos = input.encabezado?.idDoc?.tipoIngresos,
            TipoPago = input.encabezado?.idDoc?.tipoPago ?? 0,
            FechaLimitePago = ParseNullableDate(input.encabezado?.idDoc?.fechaLimitePago),
            TerminoPago = input.encabezado?.idDoc?.terminoPago,
            TipoCuentaPago = input.encabezado?.idDoc?.tipoCuentaPago,
            NumeroCuentaPago = input.encabezado?.idDoc?.numeroCuentaPago,
            BancoPago = input.encabezado?.idDoc?.bancoPago,

            // Emisor
            RNCEmisor = input.encabezado?.emisor?.rNCEmisor,
            RazonSocialEmisor = input.encabezado?.emisor?.razonSocialEmisor,
            NombreComercial = input.encabezado?.emisor?.nombreComercial,
            DireccionEmisor = input.encabezado?.emisor?.direccionEmisor,
            MunicipioEmisor = input.encabezado?.emisor?.municipio,
            ProvinciaEmisor = input.encabezado?.emisor?.provincia,
            CorreoEmisor = input.encabezado?.emisor?.correoEmisor,
            WebSite = input.encabezado?.emisor?.webSite,
            CodigoVendedor = input.encabezado?.emisor?.codigoVendedor,
            FechaEmision = ParseNullableDate(input.encabezado?.emisor?.fechaEmision),
            NumeroFacturaInterna = input.encabezado?.emisor?.numeroFacturaInterna,
            NumeroPedidoInterno = input.encabezado?.emisor?.numeroPedidoInterno,
            ZonaVenta = input.encabezado?.emisor?.zonaVenta,

            // Comprador
            RNCComprador = input.encabezado?.comprador?.rNCComprador,
            RazonSocialComprador = input.encabezado?.comprador?.razonSocialComprador,
            IdentificadorExtranjero = input.encabezado?.comprador?.identificadorExtranjero,
            ContactoComprador = input.encabezado?.comprador?.contactoComprador,
            CorreoComprador = input.encabezado?.comprador?.correoComprador,
            DireccionComprador = input.encabezado?.comprador?.direccionComprador,
            MunicipioComprador = input.encabezado?.comprador?.municipioComprador,
            ProvinciaComprador = input.encabezado?.comprador?.provinciaComprador,
            FechaEntrega = ParseNullableDate(input.encabezado?.comprador?.fechaEntrega),
            FechaOrdenCompra = ParseNullableDate(input.encabezado?.comprador?.fechaOrdenCompra),
            NumeroOrdenCompra = input.encabezado?.comprador?.numeroOrdenCompra,
            CodigoInternoComprador = input.encabezado?.comprador?.codigoInternoComprador,
            ContactoEntrega = input.encabezado?.comprador?.contactoEntrega,
            DireccionEntrega = input.encabezado?.comprador?.direccionEntrega,
            TelefonoAdicional = input.encabezado?.comprador?.telefonoAdicional,

            // Totales
            MontoGravadoI1 = input.encabezado?.totales?.montoGravadoI1 ?? 0m,
            MontoGravadoI2 = input.encabezado?.totales?.montoGravadoI2 ?? 0m,
            MontoGravadoI3 = input.encabezado?.totales?.montoGravadoI3 ?? 0m,
            MontoGravadoTotal = input.encabezado?.totales?.montoGravadoTotal ?? 0m,
            MontoExento = input.encabezado?.totales?.montoExento ?? 0m,
            ITBIS1 = input.encabezado?.totales?.iTBIS1 ?? 0m,
            ITBIS2 = input.encabezado?.totales?.iTBIS2,
            ITBIS3 = input.encabezado?.totales?.iTBIS3,
            TotalITBIS1 = input.encabezado?.totales?.totalITBIS1 ?? 0m,
            TotalITBIS2 = input.encabezado?.totales?.totalITBIS2 ?? 0m,
            TotalITBIS3 = input.encabezado?.totales?.totalITBIS3 ?? 0m,
            TotalITBISRetenido = input.encabezado?.totales?.totalITBISRetenido ?? 0m,
            TotalISRRetencion = input.encabezado?.totales?.totalISRRetencion ?? 0m,
            TotalITBISPercepcion = input.encabezado?.totales?.totalITBISPercepcion ?? 0m,
            TotalISRPercepcion = input.encabezado?.totales?.totalISRPercepcion ?? 0m,
            TotalITBIS = input.encabezado?.totales?.totalITBIS ?? 0m,
            MontoImpuestoAdicional = input.encabezado?.totales?.montoImpuestoAdicional ?? 0m,
            MontoTotal = input.encabezado?.totales?.montoTotal ?? 0m,
            MontoNoFacturable = input.encabezado?.totales?.montoNoFacturable ?? 0m,
            ValorPagar = input.encabezado?.totales?.valorPagar ?? 0m,

            // Opcionales
            InformacionesAdicionalesJson = input.encabezado?.informacionesAdicionales != null
                ? JsonConvert.SerializeObject(input.encabezado.informacionesAdicionales)
                : null,
            TransporteJson = input.encabezado?.transporte != null
                ? JsonConvert.SerializeObject(input.encabezado.transporte)
                : null,

            // Control
            CreationTime = Clock.Now,
            LastModificationTime = null,
            Status = output?.Result?.Code,

            // Respuesta DGII
            DgiiTrackId = output?.Result?.TrackId,
            DgiiResponseCode = output?.Result?.Code,
            DgiiResponseMessage = output?.Result?.Message,
            DgiiQrCodeUrl = output?.Result?.QrCodeUrl,
            DgiiUsedSequence = output?.Result?.UsedSequence,
            DgiiReceivedDate = ParseNullableDate(DateTime.Now.ToString()),
            DgiiSecurityCode = output?.Result?.SecurityCode,
            DgiiSignatureDate = ParseNullableDate(output?.Result?.SignatureDate),
            DgiiPrintFile = string.Empty,

        };
        if (output.Error is not null)
        {
            entity.DgiiResponseMessage = $" {output.Error.Details} {output.Error.Message}";
        }
        MapPaymentForms(entity, input);
        MapEmitterPhones(entity, input);
        MapHeaderAdditionalTaxes(entity, input);
        MapDetails(entity, input);
        MapSubtotals(entity, input);
        MapGlobalAdjustments(entity, input);

        return entity;
    }

    private void MapPaymentForms(Models.EcfVoucherWarehouse entity, ReceiveSalesEcfInputDto input)
    {
        var forms = input?.encabezado?.idDoc?.tablaFormasPago;
        if (forms == null)
        {
            return;
        }

        foreach (var item in forms)
        {
            entity.PaymentForms.Add(new EcfVoucherWarehousePaymentForm
            {
                FormaPago = item.formaPago,
                MontoPago = item.montoPago
            });
        }
    }

    private void MapEmitterPhones(Models.EcfVoucherWarehouse entity, ReceiveSalesEcfInputDto input)
    {
        var phones = input?.encabezado?.emisor?.tablaTelefonoEmisor;
        if (phones == null)
        {
            return;
        }

        foreach (var phone in phones)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                continue;
            }

            entity.EmitterPhones.Add(new EcfVoucherWarehouseEmitterPhone
            {
                PhoneNumber = phone.Trim()
            });
        }
    }

    private void MapHeaderAdditionalTaxes(Models.EcfVoucherWarehouse entity, ReceiveSalesEcfInputDto input)
    {
        var taxes = input?.encabezado?.totales?.impuestosAdicionales;
        if (taxes == null) return;

        foreach (var item in taxes)
        {
            entity.AdditionalTaxes.Add(new EcfVoucherWarehouseAdditionalTax
            {
                TipoImpuesto = item.tipoImpuesto,
                TasaImpuesto = item.tasaImpuestoAdicional,
                MontoImpuesto = item.montoImpuestoSelectivoConsumoEspecifico
            });
        }
    }

    private void MapDetails(Models.EcfVoucherWarehouse entity, ReceiveSalesEcfInputDto input)
    {
        var details = input?.detallesItems;
        if (details == null)
        {
            return;
        }

        foreach (var item in details)
        {
            var detail = new EcfVoucherWarehouseDetails
            {
                NumeroLinea = item.numeroLinea,
                IndicadorFacturacion = item.indicadorFacturacion,
                NombreItem = item.nombreItem,
                IndicadorBienoServicio = item.indicadorBienoServicio,
                DescripcionItem = item.descripcionItem,
                CantidadItem = item.cantidadItem,
                UnidadMedida = item.unidadMedida ?? 0,
                CantidadReferencia = item.cantidadReferencia,
                UnidadReferencia = item.unidadReferencia,
                GradosAlcohol = item.gradosAlcohol ?? 0m,
                PrecioUnitarioReferencia = item.PrecioUnitarioReferencia,
                PrecioUnitarioItem = item.precioUnitarioItem,
                DescuentoMonto = item.descuentoMonto,
                RecargoMonto = item.recargoMonto,
                MontoItem = item.montoItem
            };

            entity.Details.Add(detail);
        }
    }

    private void MapSubtotals(Models.EcfVoucherWarehouse entity, ReceiveSalesEcfInputDto input)
    {
        var subtotals = input?.subtotales;
        if (subtotals == null)
        {
            return;
        }

        foreach (var item in subtotals)
        {
            entity.Subtotals.Add(new EcfVoucherWarehouseSubtotal
            {
                NumeroSubTotal = item.numeroSubTotal,
                DescripcionSubTotal = item.descripcionSubtotal,
                Orden = item.orden,
                SubTotal = item.montoSubTotal
            });
        }
    }

    private void MapGlobalAdjustments(Models.EcfVoucherWarehouse entity, ReceiveSalesEcfInputDto input)
    {
        var adjustments = input?.descuentosORecargos;
        if (adjustments == null)
        {
            return;
        }

        foreach (var item in adjustments)
        {
            entity.GlobalAdjustments.Add(new EcfVoucherWarehouseGlobalAdjustment
            {
                TipoAjuste = item.tipoAjuste,
                IndicadorNorma1007 = item.indicadorNorma1007 is null ? string.Empty : item.indicadorNorma1007.ToString(),
                DescripcionDescuentooRecargo = item.descripcionDescuentooRecargo,
                TipoValor = item.tipoValor.ToString(),
                ValorDecimal = item.valorDescuentooRecargo,
                MontoAjuste = item.montoDescuentooRecargo
            });
        }
    }

    private DateTime? ParseNullableDate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        DateTime parsed;
        var formats = new[]
        {
        "dd-MM-yyyy",
        "d-M-yyyy",
        "dd-MM-yyyy HH:mm:ss",
        "d-M-yyyy HH:mm:ss",
        "M/d/yyyy h:mm:ss tt",
        "MM/dd/yyyy h:mm:ss tt"
    };

        if (DateTime.TryParseExact(
            value,
            formats,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out parsed))
        {
            return parsed;
        }

        if (DateTime.TryParse(value, out parsed))
        {
            return parsed;
        }

        return null;
    }

    public async Task ProcessAsync(DgiiExcelImportDto row, int rowNumber)
    {
        if (row == null)
        {
            throw new ArgumentNullException(nameof(row), $"La fila {rowNumber} está vacía.");
        }

        var voucherSequence = await taxVoucherAppService.GenerateTaxVoucherAsync(
            row.TipoeCF.ToVoucherType());

        row.ENCF = voucherSequence.Number;
        row.FechaVencimientoSecuencia = voucherSequence.ExpirationDate;

        var ecfSale = MapToSaleEcf(row);

        await SendSalesEcfToDGIIAsync(ecfSale);
    }





    [UnitOfWork(false)]
    public async Task<EcfVoucherJobStatusDto> GetJobStatusAsync(Guid jobId)
    {
        if (jobId == Guid.Empty)
        {
            throw new UserFriendlyException("El JobId es requerido.");
        }

        var job = await ecfVoucherDocumentJobManagerService.GetAsync(jobId);

        if (job == null || job.IsDeleted)
        {
            throw new UserFriendlyException("No se encontró el job solicitado.");
        }

        ValidateTenantAccess(job);

        return MapToJobStatusDto(job);
    }

    [UnitOfWork(false)]
    public async Task<List<EcfVoucherJobStatusDto>> GetJobsStatusAsync(GetEcfVoucherJobsInputDto input)
    {
        input ??= new GetEcfVoucherJobsInputDto();

        var maxResultCount = input.MaxResultCount <= 0 ? 20 : input.MaxResultCount;
        if (maxResultCount > 100)
        {
            maxResultCount = 100;
        }

        var query = ecfVoucherDocumentJobRepository.GetAll()
            .Where(x => !x.IsDeleted);

        if (AbpSession.TenantId.HasValue)
        {
            query = query.Where(x => x.TenantId == AbpSession.TenantId);
        }

        if (input.OnlyActive)
        {
            query = query.Where(x =>
                x.Status == JobStatus.Pending ||
                x.Status == JobStatus.Processing ||
                x.Status == JobStatus.Cancelled);
        }

        var jobs = await query
            .OrderByDescending(x => x.CreationTime)
            .Take(maxResultCount)
            .ToListAsync();

        return jobs
            .Select(MapToJobStatusDto)
            .ToList();
    }

    public async Task CancelJobAsync(CancelEcfVoucherJobInputDto input)
    {
        if (input == null || input.JobId == Guid.Empty)
        {
            throw new UserFriendlyException("El JobId es requerido.");
        }

        var job = await ecfVoucherDocumentJobManagerService.GetAsync(input.JobId);

        if (job == null || job.IsDeleted)
        {
            throw new UserFriendlyException("No se encontró el job solicitado.");
        }

        ValidateTenantAccess(job);

        await ecfVoucherDocumentJobManagerService.RequestCancellationAsync(input.JobId);
    }

    private void ValidateTenantAccess(EcfVoucherDocumentJob job)
    {
        if (AbpSession.TenantId.HasValue && job.TenantId != AbpSession.TenantId)
        {
            throw new UserFriendlyException("No tienes permiso para acceder a este job.");
        }
    }

    private static EcfVoucherJobStatusDto MapToJobStatusDto(EcfVoucherDocumentJob job)
    {
        var progressPercentage = job.TotalRows <= 0
            ? 0
            : Math.Round((decimal)job.ProcessedRows * 100m / job.TotalRows, 2);

        var isCompleted =
            job.Status == JobStatus.Completed ||
            job.Status == JobStatus.CompletedWithErrors;

        var isFailed = job.Status == JobStatus.Failed;
        var isCancelled = job.Status == JobStatus.Cancelled;

        var isActive =
            job.Status == JobStatus.Pending ||
            job.Status == JobStatus.Processing ||
            job.Status == JobStatus.Cancelled;

        return new EcfVoucherJobStatusDto
        {
            JobId = job.Id,
            FileName = job.FileName,
            Status = job.Status,
            ErrorMessage = job.ErrorMessage,
            TotalRows = job.TotalRows,
            ProcessedRows = job.ProcessedRows,
            SuccessRows = job.SuccessRows,
            FailedRows = job.FailedRows,
            IsCancellationRequested = job.IsCancellationRequested,
            IsCompleted = isCompleted,
            IsFailed = isFailed,
            IsCancelled = isCancelled,
            IsActive = isActive,
            ProgressPercentage = progressPercentage,
            CreationTime = job.CreationTime,
            StartTime = job.StartTime,
            EndTime = job.EndTime,
            LastModificationTime = job.LastModificationTime
        };
    }
}

