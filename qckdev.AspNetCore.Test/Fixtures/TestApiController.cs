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
    [Route("api/[controller]")]
    public class TestApiController : ApiControllerBase
    {
        [HttpPost("send-request")]
        public async Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
            => await Send(request, cancellationToken);

        [HttpPost("send-void")]
        public async Task SendVoidRequest(IRequest request, CancellationToken cancellationToken = default)
            => await Send(request, cancellationToken);

        [HttpGet("health")]
        public IActionResult Health()
            => Ok(new { status = "healthy" });
    }
}
