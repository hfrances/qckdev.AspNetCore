using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Test.Fixtures
{
    public class TestGetDataQuery : IRequest<TestDataResponse>
    {
        public int Id { get; set; }

        public TestGetDataQuery(int id = 1)
        {
            Id = id;
        }
    }

    public class TestVoidCommand : IRequest
    {
        public string Message { get; set; }

        public TestVoidCommand(string message = "test")
        {
            Message = message;
        }
    }

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

    public class TestGetDataQueryHandler : IRequestHandler<TestGetDataQuery, TestDataResponse>
    {
        public Task<TestDataResponse> Handle(TestGetDataQuery request, CancellationToken cancellationToken)
        {
            var response = new TestDataResponse { Id = request.Id };
            return Task.FromResult(response);
        }
    }

    public class TestVoidCommandHandler : IRequestHandler<TestVoidCommand>
    {
        public Task Handle(TestVoidCommand request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
