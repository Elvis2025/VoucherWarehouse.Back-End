using Abp.Runtime.Caching;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfApiAuthentication.Service;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Mappers;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Service;
[AbpAuthorize(VoucherWarehouseNamePermissions.EcfVoucherWarehouse.Default)]
public class EcfVoucherWarehouseAppService : VoucherWarehouseAppServiceBase, IEcfVoucherWarehouseAppService
{
    private readonly IRepository<Models.EcfVoucherWarehouse> ecfVoucherWarehouseRepository;
    private readonly IEcfApiAuthenticationAppService ecfApiAuthenticationService;
    private readonly ICacheManager cacheManager;

    public EcfVoucherWarehouseAppService(IRepository<Models.EcfVoucherWarehouse> ecfVoucherWarehouseRepository, IEcfApiAuthenticationAppService ecfApiAuthenticationService, ICacheManager cacheManager)
    {
        this.ecfVoucherWarehouseRepository = ecfVoucherWarehouseRepository;
        this.ecfApiAuthenticationService = ecfApiAuthenticationService;
        this.cacheManager = cacheManager;
    }

    #region CRUD Async
    [AbpAuthorize(VoucherWarehouseNamePermissions.EcfVoucherWarehouse.Read)]
    public async Task<EcfVoucherWarehouseOutputDto> GetAsync(EntityDto<int> input)
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
    [AbpAuthorize(VoucherWarehouseNamePermissions.EcfVoucherWarehouse.Read)]
    public async Task<PagedResultDto<EcfVoucherWarehouseOutputDto>> GetAllAsync(EcfVoucherWarehouseInputDto input)
    {
        try
        {
            var ecfVoucherWarehouse = await ecfVoucherWarehouseRepository.GetAllListAsync();

            return MapEntityToOutputTwoWay.Auto.MapToPagedResult(ecfVoucherWarehouse, ecfVoucherWarehouse.Count);
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
    public async Task DeleteAsync(EntityDto<int> input)
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

        AuthenticateInputDto _authenticateAPIParams = new();
        _authenticateAPIParams = await ecfApiAuthenticationService.GetEcfUserAuthenticationAsync();
        var __result = await ecfApiAuthenticationService.AuthenticateAPIAsync(new LoginInputDto { TenancyName = _authenticateAPIParams.TenancyName, UsernameOrEmailAddress = _authenticateAPIParams.UsernameOrEmailAddress, Password = _authenticateAPIParams.Password });
        string result = string.Empty;
        EcfVoucherOutputDto output = new EcfVoucherOutputDto();
        try
        {
            ReceiveCreditNoteECFInputDto objToSend = new ReceiveCreditNoteECFInputDto();

            string jsonObject = System.Text.Json.JsonSerializer.Serialize(input);
            string url = @_authenticateAPIParams.BaseUrlIbsApiDgii + "ReceiveCreditNoteEcf";

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
        AuthenticateInputDto _authenticateAPIParams = new();
        _authenticateAPIParams = await ecfApiAuthenticationService.GetEcfUserAuthenticationAsync();
        var __result = await ecfApiAuthenticationService.AuthenticateAPIAsync(new LoginInputDto { TenancyName = _authenticateAPIParams.TenancyName, UsernameOrEmailAddress = _authenticateAPIParams.UsernameOrEmailAddress, Password = _authenticateAPIParams.Password });
        string result = string.Empty;
        EcfVoucherOutputDto output = new EcfVoucherOutputDto();
        try
        {
            string jsonObject = System.Text.Json.JsonSerializer.Serialize(input);
            string url = @_authenticateAPIParams.BaseUrlIbsApiDgii + "ReceiveDebitNoteEcf";

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
        AuthenticateInputDto _authenticateAPIParams = new();
        _authenticateAPIParams = await ecfApiAuthenticationService.GetEcfUserAuthenticationAsync();
        var __result = await ecfApiAuthenticationService.AuthenticateAPIAsync(new LoginInputDto { TenancyName = _authenticateAPIParams.TenancyName, UsernameOrEmailAddress = _authenticateAPIParams.UsernameOrEmailAddress, Password = _authenticateAPIParams.Password });
        string result = string.Empty;
        EcfVoucherOutputDto output = new EcfVoucherOutputDto();
        try
        {
            ReceiveSalesEcfInputDto objToSend = new ReceiveSalesEcfInputDto();

            string jsonObject = System.Text.Json.JsonSerializer.Serialize(input);
            string url = @_authenticateAPIParams.BaseUrlIbsApiDgii + "ReceiveSalesEcf";

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
        catch (Exception ex)
        {
            result = ex.ToString();
            output = new EcfVoucherOutputDto { Error = new ErrorDto { Code = ResponseCodeStatusAPI_IBS_DGII.UnHandledError, Message = result } };

        }



        return output;

    }

    public async Task<EcfVoucherOutputDto> SendPurchaseEcfToDGIIAsync(ReceivePurchaseECFInputDto input)
    {

        AuthenticateInputDto _authenticateAPIParams = new();
        _authenticateAPIParams = await ecfApiAuthenticationService.GetEcfUserAuthenticationAsync();
        var __result = await ecfApiAuthenticationService.AuthenticateAPIAsync(new LoginInputDto { TenancyName = _authenticateAPIParams.TenancyName, UsernameOrEmailAddress = _authenticateAPIParams.UsernameOrEmailAddress, Password = _authenticateAPIParams.Password });
        string result = string.Empty;
        EcfVoucherOutputDto output = new EcfVoucherOutputDto();
        try
        {
            ReceivePurchaseECFInputDto objToSend = new ReceivePurchaseECFInputDto();

            string jsonObject = System.Text.Json.JsonSerializer.Serialize(input);
            string url = @_authenticateAPIParams.BaseUrlIbsApiDgii + "ReceivePurchaseECF";

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
        AuthenticateInputDto _authenticateAPIParams = new();
        _authenticateAPIParams = await ecfApiAuthenticationService.GetEcfUserAuthenticationAsync();
        var __result = await ecfApiAuthenticationService.AuthenticateAPIAsync(new LoginInputDto { TenancyName = _authenticateAPIParams.TenancyName, UsernameOrEmailAddress = _authenticateAPIParams.UsernameOrEmailAddress, Password = _authenticateAPIParams.Password });
        string result = string.Empty;
        EcfVoucherOutputDto output = new EcfVoucherOutputDto();
        try
        {
            CancelSequenceEcfInputDto objToSend = new CancelSequenceEcfInputDto();

            string jsonObject = System.Text.Json.JsonSerializer.Serialize(input);
            string url = @_authenticateAPIParams.BaseUrlIbsApiDgii + "CancelSequencesECF";

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
        AuthenticateInputDto _authenticateAPIParams = new AuthenticateInputDto();
        _authenticateAPIParams = await ecfApiAuthenticationService.GetEcfUserAuthenticationAsync();
        var __result = await ecfApiAuthenticationService.AuthenticateAPIAsync(new LoginInputDto { TenancyName = _authenticateAPIParams.TenancyName, UsernameOrEmailAddress = _authenticateAPIParams.UsernameOrEmailAddress, Password = _authenticateAPIParams.Password });
        string result = string.Empty;
        EcfVoucherOutputDto output = new EcfVoucherOutputDto();
        try
        {
            ReceivePurchaseECFInputDto objToSend = new ReceivePurchaseECFInputDto();

            string jsonObject = System.Text.Json.JsonSerializer.Serialize(input);
            string url = @_authenticateAPIParams.BaseUrlIbsApiDgii + "ComercialApproval";

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



}
