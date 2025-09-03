// Controllers/AuthController.cs - المصحح بالكامل
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UShop.Models;

namespace UShop.Controllers
{
	public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Register model)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "يوجد حساب بهذا البريد الإلكتروني بالفعل.");
                    return View(model);
                }

                var user = new IdentityUser
                {
                    Email = model.Email.ToLower().Trim(),
                    UserName = model.Email.ToLower().Trim(),
                    EmailConfirmed = true 
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("تم إنشاء حساب جديد للمستخدم {Email}", model.Email);

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    TempData["Success"] = "مرحباً! تم إنشاء حسابك بنجاح.";
                    return RedirectToAction("Index", "Home");
                }

                // إضافة أخطاء التسجيل للنموذج
                foreach (var error in result.Errors)
                {
                    string errorMessage = TranslateIdentityError(error.Code, error.Description);
                    ModelState.AddModelError("", errorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إنشاء الحساب للمستخدم {Email}", model.Email);
                ModelState.AddModelError("", "حدث خطأ أثناء إنشاء الحساب. يرجى المحاولة مرة أخرى.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login model, string? returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (User.Identity?.IsAuthenticated == true)
            {
                return LocalRedirect(returnUrl);
            }

            if (!ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            try
            {
                // البحث عن المستخدم أولاً
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "البريد الإلكتروني أو كلمة المرور غير صحيحة.");
                    ViewData["ReturnUrl"] = returnUrl;
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(
                    model.Email.ToLower().Trim(),
                    model.Password,
                    isPersistent: false,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    _logger.LogInformation("المستخدم {Email} سجل دخول بنجاح", model.Email);
                    TempData["Success"] = $"مرحباً بك {user.Email}!";
                    return LocalRedirect(returnUrl);
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("حساب المستخدم {Email} مقفل", model.Email);
                    ModelState.AddModelError("", "تم قفل حسابك مؤقتاً بسبب محاولات دخول فاشلة متعددة. يرجى المحاولة لاحقاً.");
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "حسابك غير مفعل. يرجى التواصل مع الإدارة.");
                }
                else
                {
                    _logger.LogWarning("محاولة دخول فاشلة للمستخدم {Email}", model.Email);
                    ModelState.AddModelError("", "البريد الإلكتروني أو كلمة المرور غير صحيحة.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تسجيل الدخول للمستخدم {Email}", model.Email);
                ModelState.AddModelError("", "حدث خطأ أثناء تسجيل الدخول. يرجى المحاولة مرة أخرى.");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("المستخدم قام بتسجيل الخروج بنجاح.");
            TempData["Success"] = "تم تسجيل الخروج بنجاح.";
            return RedirectToAction("Index", "Home");
        }
        private string TranslateIdentityError(string errorCode, string description)
        {
            // يمكنك إضافة ترجمات أخرى هنا
            switch (errorCode)
            {
                case "PasswordRequiresNonAlphanumeric":
                    return "يجب أن تحتوي كلمة المرور على حرف غير أبجدي رقمي.";
                case "PasswordRequiresDigit":
                    return "يجب أن تحتوي كلمة المرور على رقم ('0'-'9').";
                case "PasswordRequiresLower":
                    return "يجب أن تحتوي كلمة المرور على حرف صغير ('a'-'z').";
                case "PasswordRequiresUpper":
                    return "يجب أن تحتوي كلمة المرور على حرف كبير ('A'-'Z').";
                case "PasswordTooShort":
                    return "كلمة المرور قصيرة جداً. يجب أن تكون على الأقل 6 أحرف.";
                case "InvalidUserName":
                    return "اسم المستخدم غير صالح. يمكن أن يحتوي فقط على أحرف وأرقام.";
                case "DuplicateUserName":
                    return "اسم المستخدم هذا موجود بالفعل.";
                case "InvalidEmail":
                    return "البريد الإلكتروني غير صالح.";
                case "DuplicateEmail":
                    return "يوجد حساب بهذا البريد الإلكتروني بالفعل.";
                default:
                    return description; // في حالة عدم وجود ترجمة، يتم عرض الوصف الأصلي للخطأ.
            }
        }
    }

}
    
