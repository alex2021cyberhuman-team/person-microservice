using System.Net;

namespace Conduit.Person.BusinessLogic
{
    public readonly struct ExecutionResult<T>
    {
        public ExecutionResult(
            T? response = default,
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            Response = response;
            StatusCode = statusCode;
        }

        public T? Response { get; }

        public HttpStatusCode StatusCode { get; }
    }
}
