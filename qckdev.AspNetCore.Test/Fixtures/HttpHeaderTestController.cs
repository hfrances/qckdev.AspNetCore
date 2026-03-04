using Microsoft.AspNetCore.Mvc;
using qckdev.AspNetCore.Mvc.Controllers;
using qckdev.AspNetCore.Mvc.Headers;

namespace qckdev.AspNetCore.Test.Fixtures
{
    /// <summary>
    /// Test controller for HTTP header validation testing
    /// </summary>
    [ApiController]
    [Route("api/headers")]
    public class HttpHeaderTestController : ControllerBase
    {
        /// <summary>
        /// Endpoint that requires the sw-user header (mandatory)
        /// </summary>
        [HttpGet("with-mandatory-user-header")]
        [HttpUserHeader(isAvailable: true, isMandatory: true)]
        public IActionResult GetWithMandatoryUserHeader()
        {
            var userHeader = HttpContext.Request.Headers["sw-user"].ToString();
            return Ok(new { message = "User header is valid", user = userHeader });
        }

        /// <summary>
        /// Endpoint with optional sw-user header
        /// </summary>
        [HttpGet("with-optional-user-header")]
        [HttpUserHeader(isAvailable: true, isMandatory: false)]
        public IActionResult GetWithOptionalUserHeader()
        {
            var userHeader = HttpContext.Request.Headers["sw-user"].ToString();
            return Ok(new { message = "User header is optional", user = userHeader });
        }

        /// <summary>
        /// Endpoint with unavailable header (should pass through)
        /// </summary>
        [HttpGet("with-unavailable-header")]
        [HttpUserHeader(isAvailable: false, isMandatory: true)]
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

        /// <summary>
        /// Endpoint that requires the sw-company header (mandatory)
        /// </summary>
        [HttpGet("with-mandatory-company-header")]
        [HttpCompanyHeader(isAvailable: true, isMandatory: true)]
        public IActionResult GetWithMandatoryCompanyHeader()
        {
            var companyHeader = HttpContext.Request.Headers["sw-company"].ToString();
            return Ok(new { message = "Company header is valid", company = companyHeader });
        }
    }
}
