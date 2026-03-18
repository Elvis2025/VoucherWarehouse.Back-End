
using Abp.Collections.Extensions;
using Abp.Runtime.Caching;
using IBS.VoucherWarehouse.Common.Mapping.Extensions;
using IBS.VoucherWarehouse.Common.Mapping.Helpers;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfApiAuthentication.Dto;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfApiAuthentication.Mappers;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Text;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfApiAuthentication.Service;

public class EcfApiAuthenticationAppService : VoucherWarehouseAppServiceBase,IEcfApiAuthenticationAppService
{
    /*
     BaseUrl: https://test.ibsystems.com.do/api/Account/
    UrlAuth: https://test.ibsystems.com.do/api/services/app/ecfVoucher/
     UserName: adminIBS
    Password: 123qwe
    TenantcyName: eIBS 
     */


    private readonly IRepository<Models.EcfApiAuthentication, int> ecfApiAuthenticationRepository;
    private readonly ICacheManager cacheManager;

    public EcfApiAuthenticationAppService(IRepository<Models.EcfApiAuthentication, int> ecfApiAuthenticationRepository, 
                                          ICacheManager cacheManager)
    {
        this.ecfApiAuthenticationRepository = ecfApiAuthenticationRepository;
        this.cacheManager = cacheManager;
    }

    public async Task<AuthenticateInputDto> GetEcfUserAuthenticationAsync()
    {
        var ecfApiAuthentication = await ecfApiAuthenticationRepository.GetAll()
                                                                       .FirstOrDefaultAsync();
        if (ecfApiAuthentication == null)
        {
            throw new UserFriendlyException("Ecf API Authentication details not found.");
        }

        return new()
        {
            AuthenticateUrlIbsApiDgii = ecfApiAuthentication.AuthUrl,
            BaseUrlIbsApiDgii = ecfApiAuthentication.BaseUrl,
            Password = ecfApiAuthentication.Password,
            TenancyName = ecfApiAuthentication.TenancyName,
            UsernameOrEmailAddress = ecfApiAuthentication.UsernameOrEmailAddress
        };
    }

    public async Task<AuthenticationResponseOutputDto> AuthenticateAPIAsync(LoginInputDto loginViewModel)
    {
        AuthenticateInputDto _authenticateAPIParams = new AuthenticateInputDto();
        _authenticateAPIParams = await GetEcfUserAuthenticationAsync();
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

    
    #region Internal Helpers
    private void SavingAuthenticateDataToCACHE(ResultResponse input)
    {

        if (!string.IsNullOrEmpty(input.Token))
        {
            //Se le restan 10 Segundos a la Expiracion del Token para Tener ese Margen al Momento de la Validacoin
            input.Expires = input.Expires.AddSeconds(-10);
            input.Issued = input.Issued.AddSeconds(-10);
            cacheManager.GetCache("MyCache").Set("ResultResponse", input, TimeSpan.FromMinutes(60.0));

        }
    }
    private ResultResponse GetAuthenticateDataFromCACHE()
    {
        ResultResponse output = new ResultResponse();

        var _result = cacheManager.GetCache("MyCache").GetOrDefault("ResultResponse");
        if (_result != null) { output = (ResultResponse)_result; }

        return output;
    }


    #endregion

    #region CRUD Methods 
    public async Task<EcfApiAuthenticationOutputDto> GetAsync(EntityDto<int> input)
    {
        try
        {
            var ecfApiAuthentication = await ecfApiAuthenticationRepository.GetAsync(input.Id);
            return Mapping<Models.EcfApiAuthentication, EcfApiAuthenticationOutputDto>.Auto.Map(ecfApiAuthentication);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error in GetAsync: {e.Message}");
            throw;
        }
    }

    public async Task<EcfApiAuthenticationOutputDto> GetFirstOrDefaultAsync()
    {
        try
        {
            var ecfApiAuthentication = await ecfApiAuthenticationRepository.GetFisrtOrDefaultAsync();
            return Mapping<Models.EcfApiAuthentication, EcfApiAuthenticationOutputDto>.Auto.Map(ecfApiAuthentication);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error in GetAsync: {e.Message}");
            throw;
        }
    }

    public async Task<PagedResultDto<EcfApiAuthenticationOutputDto>> GetAllAsync(EcfApiAuthenticationInputDto input)
    {

        try
        {
            var ecfApiAuthentication = await ecfApiAuthenticationRepository.GetAllListAsync();
            var ecfApiAuthenticationSorting = ecfApiAuthentication.Take(input.MaxResultCount)
                                                       .Skip(input.SkipCount)
                                                       .WhereIf(!input.Sorting.IsNullOrWhiteSpace(),x => x.TenancyName.Contains(input.Sorting ?? string.Empty) ||
                                                                   x.UsernameOrEmailAddress.Contains(input.Sorting ?? string.Empty))
                                                       .ToList();

            return Mapping<Models.EcfApiAuthentication, EcfApiAuthenticationOutputDto>.Auto.MapToPagedResult(ecfApiAuthenticationSorting, ecfApiAuthentication.Count);

        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error in GetAllAsync: {e.Message}");
            throw;
        }
    }

    public async Task<EcfApiAuthenticationOutputDto> CreateAsync(EcfApiAuthenticationCreateDto input)
    {
        try
        {
            var exist = await ecfApiAuthenticationRepository.CountAsync();

            if (exist > 0)
            {
                throw new UserFriendlyException("Only one Ecf API Authentication record is allowed.");
            }

            var ecfApiAuthentication = Mapping<EcfApiAuthenticationCreateDto, Models.EcfApiAuthentication>.Auto.Map(input);
           var result = await ecfApiAuthenticationRepository.InsertAsync(ecfApiAuthentication);

            return Mapping<Models.EcfApiAuthentication, EcfApiAuthenticationOutputDto>.Auto.Map(result);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error in CreateAsync: {e.Message}");
            throw;
        }
    }

    public async Task<EcfApiAuthenticationOutputDto> UpdateAsync(EcfApiAuthenticationUpdateDto input)
    {
        try
        {
            if (input == null || input.Id <= 0)
            {
                throw new UserFriendlyException("Invalid input for update.");
            }
            var ecfApiAuthentication = Mapping<EcfApiAuthenticationUpdateDto, Models.EcfApiAuthentication>.Auto.Map(input);

           var result = await ecfApiAuthenticationRepository.UpdateAsync(ecfApiAuthentication);
            return Mapping<Models.EcfApiAuthentication, EcfApiAuthenticationOutputDto>.Auto.Map(result);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error in UpdateAsync: {e.Message}");
            throw;
        }
    }

    public async Task DeleteAsync(EntityDto<int> input)
    {
        try
        {
            await ecfApiAuthenticationRepository.DeleteAsync(input.Id);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error in DeleteAsync: {e.Message}");
            throw;
        }
    }

   
    #endregion
}
