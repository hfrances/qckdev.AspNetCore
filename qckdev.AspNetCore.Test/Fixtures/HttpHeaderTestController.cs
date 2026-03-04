using Microsoft.AspNetCore.Mvc;
using qckdev.AspNetCore.Mvc.Headers;

namespace qckdev.AspNetCore.Test.Fixtures
{
    /// <summary>
    /// Test controller for HTTP Accept-Language header validation testing
    /// </summary>
    [ApiController]
    [Route("api/headers")]
    public class HttpHeaderTestController : ControllerBase
    {
        /// <summary>
        /// Endpoint that requires the Accept-Language header (mandatory)
        /// </summary>
        [HttpGet("with-mandatory-accept-language")]
        [HttpAcceptLanguageHeader(isAvailable: true, isMandatory: true)]
        public IActionResult GetWithMandatoryAcceptLanguage()
        {
            var languageHeader = HttpContext.Request.Headers["Accept-Language"].ToString();
            return Ok(new { message = "Accept-Language header is valid", language = languageHeader });
        }

        /// <summary>
        /// Endpoint with optional Accept-Language header
        /// </summary>
        [HttpGet("with-optional-accept-language")]
        [HttpAcceptLanguageHeader(isAvailable: true, isMandatory: false)]
        public IActionResult GetWithOptionalAcceptLanguage()
        {
            var languageHeader = HttpContext.Request.Headers["Accept-Language"].ToString();
            return Ok(new { message = "Accept-Language header is optional", language = languageHeader });
        }

        /// <summary>
        /// Endpoint with unavailable header (should pass through)
        /// </summary>
        [HttpGet("with-unavailable-header")]
        [HttpAcceptLanguageHeader(isAvailable: false, isMandatory: true)]
        public IActionResult GetWithUnavailableHeader()
        {
            return Ok(new { message = "Endpoint with unavailable header" });
        }

        /// <summary>
        /// Endpoint without any header attribute
        /// </summary>
        [HttpGet("without-header")]
        public IActionResult GetWithoutHeader()
        {
            return Ok(new { message = "No header required" });
        }
    }
}
