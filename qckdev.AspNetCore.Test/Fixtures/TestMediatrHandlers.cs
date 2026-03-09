using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Test.Fixtures
{
    /// <summary>
    /// Test query for retrieving data
    /// </summary>
    public class TestGetDataQuery : IRequest<TestDataResponse>
    {
        public int Id { get; set; }
        
        public TestGetDataQuery(int id = 1)
        {
            Id = id;
        }
    }

    /// <summary>
    /// Test command
    /// </summary>
    public class TestVoidCommand : IRequest
    {
        public string Message { get; set; }

        public TestVoidCommand(string message = "test")
        {
            Message = message;
        }
    }

    /// <summary>
    /// Test response model
    /// </summary>
    public class TestDataResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public TestDataResponse()
        {
            Name = "Test Data";
            Description = "This is test data";
        }
    }

    /// <summary>
    /// Handler for test query
    /// </summary>
    public class TestGetDataQueryHandler : IRequestHandler<TestGetDataQuery, TestDataResponse>
    {
        public Task<TestDataResponse> Handle(TestGetDataQuery request, CancellationToken cancellationToken)
        {
            var response = new TestDataResponse { Id = request.Id };
            return Task.FromResult(response);
        }
    }

    /// <summary>
    /// Handler for test void command
    /// </summary>
    public class TestVoidCommandHandler : IRequestHandler<TestVoidCommand>
    {
#if MEDIATR_LEGACY
        public Task<Unit> Handle(TestVoidCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Unit.Value);
        }
#else
        public Task Handle(TestVoidCommand request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
#endif
    }
}
