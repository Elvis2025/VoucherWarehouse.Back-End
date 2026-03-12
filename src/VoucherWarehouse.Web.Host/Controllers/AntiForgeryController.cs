using Abp.Web.Security.AntiForgery;
using IBS.VoucherWarehouse.Controllers;
using Microsoft.AspNetCore.Antiforgery;

namespace IBS.VoucherWarehouse.Web.Host.Controllers;

public class AntiForgeryController : VoucherWarehouseControllerBase
{
    private readonly IAntiforgery _antiforgery;
    private readonly IAbpAntiForgeryManager _antiForgeryManager;

    public AntiForgeryController(IAntiforgery antiforgery, IAbpAntiForgeryManager antiForgeryManager)
    {
        _antiforgery = antiforgery;
        _antiForgeryManager = antiForgeryManager;
    }

    public void GetToken()
    {
        _antiforgery.SetCookieTokenAndHeader(HttpContext);
    }

    public void SetCookie()
    {
        _antiForgeryManager.SetCookie(HttpContext);
    }
}
