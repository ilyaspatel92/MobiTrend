using Microsoft.AspNetCore.Mvc;

namespace Mobi.Web.Controllers
{
    public class LanguageController : BasePublicController
    {
        [HttpPost]
        public IActionResult SetLanguage([FromBody] string language)
        {
            if (language == "1")
                Response.Cookies.Delete("language");            
            else if (language == "2") 
            {
                // Set the language cookie to Arabic
                Response.Cookies.Append("language", language, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1), // Cookie expiration
                    HttpOnly = true,
                    Secure = false, // Use false for local development (ensure not on HTTPS)
                    SameSite = SameSiteMode.Lax, // Allow the cookie to be sent across different domains
                    Path = "/" // Ensure the cookie is available for the entire site
                });
            }

            return Ok(new { message = "Language preference updated" });

        }
    }
}