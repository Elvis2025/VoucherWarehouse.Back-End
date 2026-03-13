using Abp.Runtime.Caching;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfApiAuthentication.Service;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Service;

public class EcfVoucherWarehouseAppService : VoucherWarehouseAppServiceBase, IEcfVoucherWarehouseAppService
{
    private readonly IEcfApiAuthenticationAppService ecfApiAuthenticationService;
    private readonly ICacheManager cacheManager;

    public EcfVoucherWarehouseAppService(IEcfApiAuthenticationAppService ecfApiAuthenticationService, ICacheManager cacheManager)
    {
        this.ecfApiAuthenticationService = ecfApiAuthenticationService;
        this.cacheManager = cacheManager;
    }




    public async Task<AuthenticationResponseOutputDto> AuthenticateAPIAsync(LoginInputDto loginViewModel)
    {
        AuthenticateInputDto _authenticateAPIParams = new AuthenticateInputDto();
        _authenticateAPIParams = await ecfApiAuthenticationService.GetEcfUserAuthenticationAsync(); 
        string result = string.Empty;
        AuthenticationResponseOutputDto _result = new AuthenticationResponseOutputDto();

        try
        {
            //Validate Token Expiration
            var getCache = GetAuthenticateDataFromCACHE();
            if (getCache != null && !string.IsNullOrEmpty(getCache.Token) && getCache.Expires > DateTime.Now)
            {
                //Si el Token sigue Vigente de vuelvo el Token Existente, De lo contrario Genero uno Nuevo
                _result.Result = getCache;
            }
            else
            {
                string jsonObject = System.Text.Json.JsonSerializer.Serialize(loginViewModel);
                string url = @_authenticateAPIParams.AuthenticateUrlIbsApiDgii + "Authenticate";

                var client = new System.Net.Http.HttpClient();
                var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);

                result = await response.Content.ReadAsStringAsync();

                _result = JsonConvert.DeserializeObject<AuthenticationResponseOutputDto>(result);

                //Saving New Token to Chache
                if (_result.Result != null)
                {
                    if (!string.IsNullOrEmpty(_result.Result.Token))
                    {
                        SavingAuthenticateDataToCACHE(_result.Result);

                    }
                }
            }

        }
        catch (System.Exception ex)
        {
            result = ex.Message.ToString();
            _result = new AuthenticationResponseOutputDto { Error = new ErrorResponse { Code = ResponseCodeStatusAPI_IBS_DGII.UnHandledError, Message = result } };
        }

        return _result;
    }
    public void SavingAuthenticateDataToCACHE(ResultResponse input)
    {

        if (!string.IsNullOrEmpty(input.Token))
        {
            //Se le restan 10 Segundos a la Expiracion del Token para Tener ese Margen al Momento de la Validacoin
            input.Expires = input.Expires.AddSeconds(-10);
            input.Issued = input.Issued.AddSeconds(-10);
            cacheManager.GetCache("MyCache").Set("ResultResponse", input, TimeSpan.FromMinutes(60.0));

        }
    }
    public ResultResponse GetAuthenticateDataFromCACHE()
    {
        ResultResponse output = new ResultResponse();

        var _result = cacheManager.GetCache("MyCache").GetOrDefault("ResultResponse");
        if (_result != null) { output = (ResultResponse)_result; }

        return output;
    }


    public async Task<EcfVoucherOutputDto> ReceiveCreditNoteECFAsync(ReceiveCreditNoteECFInputDto input)
    {

        AuthenticateInputDto _authenticateAPIParams = new();
        _authenticateAPIParams = await ecfApiAuthenticationService.GetEcfUserAuthenticationAsync();
        var __result = await AuthenticateAPIAsync(new LoginInputDto { TenancyName = _authenticateAPIParams.TenancyName, UsernameOrEmailAddress = _authenticateAPIParams.UsernameOrEmailAddress, Password = _authenticateAPIParams.Password });
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

    public async Task<EcfVoucherOutputDto> ReceiveCommercialApprovalECF(CommercialApprovalEcfInputDto input)
    {
        AuthenticateInputDto _authenticateAPIParams = new AuthenticateInputDto();
        _authenticateAPIParams = await ecfApiAuthenticationService.GetEcfUserAuthenticationAsync();
        var __result = await AuthenticateAPIAsync(new LoginInputDto { TenancyName = _authenticateAPIParams.TenancyName, UsernameOrEmailAddress = _authenticateAPIParams.UsernameOrEmailAddress, Password = _authenticateAPIParams.Password });
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
