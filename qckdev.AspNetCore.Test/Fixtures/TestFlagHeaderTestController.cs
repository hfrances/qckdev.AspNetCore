using Microsoft.AspNetCore.Mvc;
using qckdev.AspNetCore.Mvc.Headers;

namespace qckdev.AspNetCore.Test.Fixtures
{
    /// <summary>
    /// Test controller for test-flag header validation testing
    /// </summary>
    [ApiController]
    [Route("api/testflag")]
    public class TestFlagHeaderTestController : ControllerBase
    {
        /// <summary>
        /// Endpoint that requires the test-flag header (mandatory)
        /// </summary>
        [HttpGet("mandatory-flag")]
        [HttpTestFlagHeaderAttribute(isAvailable: true, isMandatory: true)]
        public IActionResult GetWithMandatoryTestFlag()
        {
            var flagHeader = HttpContext.Request.Headers["test-flag"].ToString();
            return Ok(new { message = "Test-flag header is required", flag = flagHeader });
        }

        /// <summary>
        /// Endpoint with optional test-flag header
        /// </summary>
        [HttpGet("optional-flag")]
        [HttpTestFlagHeaderAttribute(isAvailable: true, isMandatory: false)]
        public IActionResult GetWithOptionalTestFlag()
        {
            var flagHeader = HttpContext.Request.Headers["test-flag"].ToString();
            return Ok(new { message = "Test-flag header is optional", flag = flagHeader });
        }

        /// <summary>
        /// Endpoint with unavailable test-flag header (should pass through)
        /// </summary>
        [HttpGet("unavailable-flag")]
        [HttpTestFlagHeaderAttribute(isAvailable: false, isMandatory: true)]
        public IActionResult GetWithUnavailableTestFlag()
        {
            return Ok(new { message = "Endpoint with unavailable test-flag header" });
        }

        /// <summary>
        /// Endpoint without test-flag header
        /// </summary>
        [HttpGet("no-header")]
        public IActionResult GetWithoutTestFlag()
        {
            return Ok(new { message = "No test-flag header required" });
        }
    }
}
