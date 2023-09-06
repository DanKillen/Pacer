using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Pacer.Data.Services;

namespace Pacer.Web.Components;

public class NavbarViewComponent : ViewComponent
{
    private readonly IUserService _svc;

    public NavbarViewComponent(IUserService svc)
    {
        _svc = svc;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userEmail = HttpContext.User.FindFirstValue(ClaimTypes.Email);
        var user = await _svc.AsyncGetUserByEmail(userEmail);

        var navbarViewModel = new NavbarViewModel
        {
            User = user,
        };

        return View(navbarViewModel);
    }

}
