using Microsoft.AspNetCore.Mvc;
using qckdev.AspNetCore.Mvc.Controllers;
using System.Threading;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Test.Fixtures
{
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
