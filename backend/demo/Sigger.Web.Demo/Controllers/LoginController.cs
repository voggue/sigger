using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity.UI.V5.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Sigger.Web.Demo.Auth;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Sigger.Web.Demo.Controllers;

public class LoginController : Controller
{
    private readonly ILogger<LoginController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailSender _sender;

    public LoginController(
        ILogger<LoginController> logger,
        UserManager<ApplicationUser> userManager,
        IEmailSender sender, SignInManager<ApplicationUser> signInManager)
    {
        _logger = logger;
        _userManager = userManager;
        _sender = sender;
        _signInManager = signInManager;
    }

    public async Task<SignInResult> SignIn(LoginModel.InputModel loginModel)
    {
        if (!ModelState.IsValid) 
            return SignInResult.Failed;
        
        // This doesn't count login failures towards account lockout
        // To enable password failures to trigger account lockout, 
        // set lockoutOnFailure: true
        var result = await _signInManager.PasswordSignInAsync(loginModel.Email,
            loginModel.Password, loginModel.RememberMe, lockoutOnFailure: true);
            
        return result;

        // If we got this far, something failed, redisplay form
    }
}