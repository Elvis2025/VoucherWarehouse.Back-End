using Abp.Collections.Extensions;
using Abp.Runtime.Caching;
using Abp.Timing;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using IBS.VoucherWarehouse.Common.GlobalHelpers;
using IBS.VoucherWarehouse.Common.Helpers;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfApiAuthentication.Service;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Mappers;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucher.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Service;
//[AbpAuthorize(VoucherWarehouseNamePermissions.EcfVoucherWarehouse.Default)]
public class EcfVoucherWarehouseAppService : VoucherWarehouseAppServiceBase, IEcfVoucherWarehouseAppService
{
    private readonly IRepository<Models.EcfVoucherWarehouse,long> ecfVoucherWarehouseRepository;
    private readonly IEcfApiAuthenticationAppService ecfApiAuthenticationService;
    private readonly ICacheManager cacheManager;
    private readonly ITaxVoucherAppService taxVoucherAppService;

    public EcfVoucherWarehouseAppService(IRepository<Models.EcfVoucherWarehouse,long> ecfVoucherWarehouseRepository, 
                                         IEcfApiAuthenticationAppService ecfApiAuthenticationService, 
                                         ICacheManager cacheManager,
                                         ITaxVoucherAppService taxVoucherAppService)
    {
        this.ecfVoucherWarehouseRepository = ecfVoucherWarehouseRepository;
        this.ecfApiAuthenticationService = ecfApiAuthenticationService;
        this.cacheManager = cacheManager;
        this.taxVoucherAppService = taxVoucherAppService;
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

            if(input.FilterText is not null)
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



    private static readonly Regex IndexedColumnRegex = new(@"^(?<name>.+)\[(?<index>\d+)\]$", RegexOptions.Compiled);


    private List<DgiiExcelImportDto> Read(Stream stream)
    {
        using var workbook = new XLWorkbook(stream);
        var ws = workbook.Worksheet(1);

        const int headerRow = 1;
        const int dataStartRow = 2;

        var lastColumn = ws.LastColumnUsed().ColumnNumber();
        var lastRow = ws.LastRowUsed().RowNumber();

        var headers = new Dictionary<int, string>();
        for (int col = 1; col <= lastColumn; col++)
        {
            headers[col] = ws.Cell(headerRow, col).GetValue<string>()?.Trim();
        }

        var result = new List<DgiiExcelImportDto>();

        for (int row = dataStartRow; row <= lastRow; row++)
        {
            if (RowIsEmpty(ws, row, lastColumn))
            {
                continue;
            }

            var dto = new DgiiExcelImportDto
            {
                TipoeCF = GetInt(ws, row, headers, "TipoeCF") ?? 0,
                IndicadorMontoGravado = GetInt(ws, row, headers, "IndicadorMontoGravado"),
                TipoIngresos = GetString(ws, row, headers, "TipoIngresos"),
                TipoPago = GetInt(ws, row, headers, "TipoPago"),

                RNCEmisor = GetString(ws, row, headers, "RNCEmisor"),
                RazonSocialEmisor = GetString(ws, row, headers, "RazonSocialEmisor"),
                NombreComercial = GetString(ws, row, headers, "NombreComercial"),
                DireccionEmisor = GetString(ws, row, headers, "DireccionEmisor"),
                Municipio = GetString(ws, row, headers, "Municipio"),
                Provincia = GetString(ws, row, headers, "Provincia"),
                CorreoEmisor = GetString(ws, row, headers, "CorreoEmisor"),
                WebSite = GetString(ws, row, headers, "WebSite"),
                CodigoVendedor = GetString(ws, row, headers, "CodigoVendedor"),

                NumeroFacturaInterna = GetString(ws, row, headers, "NumeroFacturaInterna"),
                NumeroPedidoInterno = GetString(ws, row, headers, "NumeroPedidoInterno"),
                ZonaVenta = GetString(ws, row, headers, "ZonaVenta"),
                FechaEmision = GetString(ws, row, headers, "FechaEmision"),

                RNCComprador = GetString(ws, row, headers, "RNCComprador"),
                RazonSocialComprador = GetString(ws, row, headers, "RazonSocialComprador"),
                ContactoComprador = GetString(ws, row, headers, "ContactoComprador"),
                CorreoComprador = GetString(ws, row, headers, "CorreoComprador"),
                DireccionComprador = GetString(ws, row, headers, "DireccionComprador"),
                MunicipioComprador = GetString(ws, row, headers, "MunicipioComprador"),
                ProvinciaComprador = GetString(ws, row, headers, "ProvinciaComprador"),

                FechaEntrega = GetString(ws, row, headers, "FechaEntrega"),
                FechaOrdenCompra = GetString(ws, row, headers, "FechaOrdenCompra"),
                NumeroOrdenCompra = GetString(ws, row, headers, "NumeroOrdenCompra"),
                CodigoInternoComprador = GetString(ws, row, headers, "CodigoInternoComprador"),
                NumeroContenedor = GetString(ws, row, headers, "NumeroContenedor"),
                NumeroReferencia = GetString(ws, row, headers, "NumeroReferencia"),

                MontoGravadoTotal = GetDecimal(ws, row, headers, "MontoGravadoTotal"),
                MontoGravadoI1 = GetDecimal(ws, row, headers, "MontoGravadoI1"),
                ITBIS1 = GetDecimal(ws, row, headers, "ITBIS1"),
                TotalITBIS = GetDecimal(ws, row, headers, "TotalITBIS"),
                TotalITBIS1 = GetDecimal(ws, row, headers, "TotalITBIS1"),
                MontoTotal = GetDecimal(ws, row, headers, "MontoTotal")
            };

            dto.TelefonosEmisor = GetIndexedStrings(ws, row, headers, "TelefonoEmisor");
            dto.FormasPago = GetFormasPago(ws, row, headers);
            dto.Items = GetItems(ws, row, headers);

            result.Add(dto);
        }

        return result;
    }




    private static List<DgiiExcelFormaPagoDto> GetFormasPago(IXLWorksheet ws, int row, Dictionary<int, string> headers)
    {
        var result = new List<DgiiExcelFormaPagoDto>();
        var indexes = GetIndexes(headers, "FormaPago")
            .Union(GetIndexes(headers, "MontoPago"))
            .Distinct()
            .OrderBy(x => x);

        foreach (var index in indexes)
        {
            var formaPago = GetIndexedInt(ws, row, headers, "FormaPago", index);
            var montoPago = GetIndexedDecimal(ws, row, headers, "MontoPago", index);

            if (formaPago == null && montoPago == null)
                continue;

            result.Add(new DgiiExcelFormaPagoDto
            {
                Numero = index,
                FormaPago = formaPago,
                MontoPago = montoPago
            });
        }

        return result;
    }

    private static List<DgiiExcelDetalleDto> GetItems(IXLWorksheet ws, int row, Dictionary<int, string> headers)
    {
        var result = new List<DgiiExcelDetalleDto>();

        var indexes = GetIndexes(headers, "NumeroLinea")
            .Union(GetIndexes(headers, "IndicadorFacturacion"))
            .Union(GetIndexes(headers, "NombreItem"))
            .Union(GetIndexes(headers, "IndicadorBienoServicio"))
            .Union(GetIndexes(headers, "CantidadItem"))
            .Union(GetIndexes(headers, "UnidadMedida"))
            .Union(GetIndexes(headers, "PrecioUnitarioItem"))
            .Union(GetIndexes(headers, "MontoItem"))
            .Distinct()
            .OrderBy(x => x);

        foreach (var index in indexes)
        {
            var item = new DgiiExcelDetalleDto
            {
                Numero = index,
                NumeroLinea = GetIndexedInt(ws, row, headers, "NumeroLinea", index),
                IndicadorFacturacion = GetIndexedInt(ws, row, headers, "IndicadorFacturacion", index),
                NombreItem = GetIndexedString(ws, row, headers, "NombreItem", index),
                IndicadorBienoServicio = GetIndexedInt(ws, row, headers, "IndicadorBienoServicio", index),
                CantidadItem = GetIndexedDecimal(ws, row, headers, "CantidadItem", index),
                UnidadMedida = GetIndexedInt(ws, row, headers, "UnidadMedida", index),
                PrecioUnitarioItem = GetIndexedDecimal(ws, row, headers, "PrecioUnitarioItem", index),
                MontoItem = GetIndexedDecimal(ws, row, headers, "MontoItem", index)
            };

            if (string.IsNullOrWhiteSpace(item.NombreItem)
                && item.CantidadItem == null
                && item.PrecioUnitarioItem == null
                && item.MontoItem == null)
                continue;

            result.Add(item);
        }

        return result;
    }

    private static IEnumerable<int> GetIndexes(Dictionary<int, string> headers, string baseName)
    {
        foreach (var item in headers)
        {
            if (string.IsNullOrWhiteSpace(item.Value))
                continue;

            var match = IndexedColumnRegex.Match(item.Value);
            if (!match.Success)
                continue;

            if (string.Equals(match.Groups["name"].Value.Trim(), baseName, StringComparison.OrdinalIgnoreCase))
                yield return int.Parse(match.Groups["index"].Value);
        }
    }

    private static string GetString(IXLWorksheet ws, int row, Dictionary<int, string> headers, string headerName)
    {
        var col = headers.FirstOrDefault(x => string.Equals(x.Value, headerName, StringComparison.OrdinalIgnoreCase)).Key;
        return col == 0 ? null : ws.Cell(row, col).GetFormattedString().Trim();
    }

    private static int? GetInt(IXLWorksheet ws, int row, Dictionary<int, string> headers, string headerName)
    {
        var value = GetString(ws, row, headers, headerName);
        return int.TryParse(value, out var n) ? n : null;
    }

    private static decimal? GetDecimal(IXLWorksheet ws, int row, Dictionary<int, string> headers, string headerName)
    {
        var value = GetString(ws, row, headers, headerName);
        return ParseDecimal(value);
    }

    private static string GetIndexedString(IXLWorksheet ws, int row, Dictionary<int, string> headers, string baseName, int index)
    {
        var header = $"{baseName}[{index}]";
        var col = headers.FirstOrDefault(x => string.Equals(x.Value, header, StringComparison.OrdinalIgnoreCase)).Key;
        return col == 0 ? null : ws.Cell(row, col).GetFormattedString().Trim();
    }

    private static int? GetIndexedInt(IXLWorksheet ws, int row, Dictionary<int, string> headers, string baseName, int index)
    {
        var value = GetIndexedString(ws, row, headers, baseName, index);
        return int.TryParse(value, out var n) ? n : null;
    }

    private static decimal? GetIndexedDecimal(IXLWorksheet ws, int row, Dictionary<int, string> headers, string baseName, int index)
    {
        var value = GetIndexedString(ws, row, headers, baseName, index);
        return ParseDecimal(value);
    }

    private static List<string> GetIndexedStrings(IXLWorksheet ws, int row, Dictionary<int, string> headers, string baseName)
    {
        var result = new List<string>();
        foreach (var index in GetIndexes(headers, baseName).OrderBy(x => x))
        {
            var value = GetIndexedString(ws, row, headers, baseName, index);
            if (!string.IsNullOrWhiteSpace(value))
                result.Add(value);
        }

        return result;
    }

    private static decimal? ParseDecimal(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        value = value.Trim();

        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var d1))
            return d1;

        if (decimal.TryParse(value, NumberStyles.Any, new CultureInfo("es-DO"), out var d2))
            return d2;

        return null;
    }

    private static bool RowIsEmpty(IXLWorksheet ws, int row, int lastColumn)
    {
        for (int col = 1; col <= lastColumn; col++)
        {
            if (!string.IsNullOrWhiteSpace(ws.Cell(row, col).GetFormattedString()))
                return false;
        }

        return true;
    }



    public async Task LoadExcelAsync([FromForm] ImportDgiiExcelRequestDto input)
    {
        try
        {
            if (input.File == null || input.File.Length == 0)
            {
                throw new Exception("Debes enviar un archivo Excel.");
            }

            var extension = Path.GetExtension(input.File.FileName);
            if (string.IsNullOrWhiteSpace(extension) ||
                (extension.ToLower() != ".xlsx" && extension.ToLower() != ".xls"))
            {
                throw new Exception("El archivo debe ser Excel (.xlsx o .xls).");
            }

            using var stream = input.File.OpenReadStream();

            var importedRows = await ImportAsync(stream, input.File.FileName);
        }
        catch (Exception e)
        {
           
            throw;
        }

       
    }

    public class ImportDgiiExcelRequestDto
    {
        public IFormFile File { get; set; }
    }

    public async Task<List<DgiiExcelImportDto>> ImportAsync(Stream fileStream, string fileName)
    {
        if (fileStream == null)
        {
            throw new UserFriendlyException("No se recibió el archivo.");
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new UserFriendlyException("El nombre del archivo es inválido.");
        }

        var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
        if (extension != ".xlsx" && extension != ".xls")
        {
            throw new UserFriendlyException("Solo se permiten archivos Excel (.xlsx o .xls).");
        }

        using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var result = Read(memoryStream);

        foreach (var res in result)
        {
            var voucherSecuence = await taxVoucherAppService.GenerateTaxVoucherAsync(res.TipoeCF.ToVoucherType());
            res.ENCF = voucherSecuence.Number;
            res.FechaVencimientoSecuencia = voucherSecuence.ExpirationDate;
            var ecfSale = MapToSaleEcf(res);
            
            await SendSalesEcfToDGIIAsync(ecfSale);

        }
        return result;
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
                    indicadorMontoGravado = source.IndicadorMontoGravado ?? 0,
                    tipoIngresos = SafeString(source.TipoIngresos),
                    tipoPago = source.TipoPago ?? 0,

                    // ESTE CAMPO TE ESTABA FALLANDO.
                    // Debe ir inicializado siempre.
                    // Si tu catálogo usa otro valor por defecto válido, cámbialo aquí.
                    tipoCuentaPago = "NA",

                    tablaFormasPago = (source.FormasPago ?? new List<DgiiExcelFormaPagoDto>())
                        .Select(x => new TablaFormasPago
                        {
                            formaPago = x?.FormaPago ?? 0,
                            montoPago = x?.MontoPago ?? 0m
                        })
                        .ToList()
                },

                emisor = new Emisor
                {
                    rNCEmisor = SafeString(source.RNCEmisor),
                    razonSocialEmisor = SafeString(source.RazonSocialEmisor),
                    nombreComercial = SafeString(source.NombreComercial),
                    direccionEmisor = SafeString(source.DireccionEmisor),
                    municipio = SafeString(source.Municipio),
                    provincia = SafeString(source.Provincia),
                    correoEmisor = SafeString(source.CorreoEmisor),
                    webSite = SafeString(source.WebSite),
                    codigoVendedor = SafeString(source.CodigoVendedor),

                    // En tu JSON estaba nulo; mejor mandarlo inicializado.
                    fechaEmision = DateTime.Now.ToString("dd-MM-yyyy"),
                    numeroFacturaInterna = $"IBS-VW-{numeroPedidoInterno}",
                    numeroPedidoInterno = numeroPedidoInterno.ToString(),
                    tablaTelefonoEmisor = (source.TelefonosEmisor ?? new List<string>())
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Trim())
                        .ToArray()
                },

                comprador = new Comprador
                {
                    rNCComprador = SafeString(source.RNCComprador),
                    razonSocialComprador = SafeString(source.RazonSocialComprador),
                    contactoComprador = SafeString(source.ContactoComprador),
                    correoComprador = SafeString(source.CorreoComprador),
                    direccionComprador = SafeString(source.DireccionComprador),
                    municipioComprador = SafeString(source.MunicipioComprador),
                    provinciaComprador = SafeString(source.ProvinciaComprador),
                    fechaEntrega = DateTime.Now.ToString("dd-MM-yyyy"),
                    codigoInternoComprador = Random.Shared.Next(10000, 100000).ToString()
                },

                transporte = null,

                totales = new Totales
                {
                    montoGravadoTotal = source.MontoGravadoTotal ?? 0m,
                    montoGravadoI1 = source.MontoGravadoI1 ?? 0m,

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

            detallesItems = (source.Items ?? new List<DgiiExcelDetalleDto>())
                .Select(x => new DetallesItem
                {
                    numeroLinea = x?.NumeroLinea ?? x?.Numero ?? 0,
                    indicadorFacturacion = x?.IndicadorFacturacion ?? 0,
                    nombreItem = SafeString(x?.NombreItem),
                    tablaCodigosItem = new() {
                        new()
                        { 
                            tipoCodigo = "IBSVWINTERNA",
                            codigoItem = $"00{x.NumeroLinea}"
                        
                        }
                    },
                    // ESTE CAMPO TE ESTABA FALLANDO.
                    gradosAlcohol = 0m,

                    indicadorBienoServicio = x?.IndicadorBienoServicio ?? 0,
                    cantidadItem = x?.CantidadItem ?? 0m,
                    unidadMedida = x?.UnidadMedida ?? 0,
                    precioUnitarioItem = x?.PrecioUnitarioItem ?? 0m,
                    montoItem = x?.MontoItem ?? 0m
                })
                .ToList()
        };

        return dto;
    }
    private static string SafeString(object value)
    {
        return value?.ToString()?.Trim() ?? string.Empty;
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
        if(output.Error is not null)
        {
            entity.DgiiResponseMessage =$" {output.Error.Details} { output.Error.Message}";
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
        if (taxes == null)
        {
            return;
        }

        foreach (var item in taxes)
        {
            entity.AdditionalTaxes.Add(new EcfVoucherWarehouseAdditionalTax
            {
                TipoImpuesto = item.tipoImpuesto,
                TasaImpuesto = item.tasaImpuestoAdicional ,
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
                IndicadorFacturacion = item.indicadorFacturacion ,
                NombreItem = item.nombreItem,
                IndicadorBienoServicio = item.indicadorBienoServicio,
                DescripcionItem = item.descripcionItem,
                CantidadItem = item.cantidadItem ,
                UnidadMedida = item.unidadMedida ,
                CantidadReferencia = item.cantidadReferencia ,
                UnidadReferencia = item.unidadReferencia ,
                GradosAlcohol = item.gradosAlcohol ?? 0m ,
                PrecioUnitarioReferencia = item.PrecioUnitarioReferencia,
                PrecioUnitarioItem = item.precioUnitarioItem ,
                DescuentoMonto = item.descuentoMonto ,
                RecargoMonto = item.recargoMonto ,
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

}
