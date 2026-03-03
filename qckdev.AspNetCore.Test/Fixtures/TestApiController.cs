using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using qckdev.AspNetCore.Mvc.Controllers;

namespace qckdev.AspNetCore.Test.Fixtures
{
    /// <summary>
    /// Test controller for API testing with concrete Send methods
    /// </summary>
    [ApiController]
    [Route("api/test")]
    public class TestApiController : ApiControllerBase
    {
        [HttpPost("send-request")]
        public async Task<TestDataResponse> SendRequest(TestGetDataQuery request, CancellationToken cancellationToken = default)
            => await Send(request, cancellationToken);

        [HttpPost("send-void")]
        public async Task SendVoidRequest(TestVoidCommand request, CancellationToken cancellationToken = default)
            => await Send(request, cancellationToken);

        [HttpGet("health")]
        public IActionResult Health()
            => Ok(new { status = "healthy" });
    }
}
