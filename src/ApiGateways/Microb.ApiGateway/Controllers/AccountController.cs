using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microb.Identity.Api.Controllers;

public class AccountController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        return Ok();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Username)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));

            if (Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return Ok();
        }

        return BadRequest();
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();

        return Ok();
    }
}

public class LoginViewModel
{
    [Required] public string Username { get; set; }
    [Required] public string Password { get; set; }
    public string ReturnUrl { get; set; }
}
