using Microsoft.AspNetCore.Http;

namespace OnlineMarket.Domain.Response
{
    public class Response<T>
    {
        public int StatusCode { get; private set; }
        public T Data { get; private set; }
        public Exception Exception { get; private set; }

        public void SetOk(T data)
        {
            Data = data;
            StatusCode = StatusCodes.Status200OK;
        }
        public void SetInternalError(Exception ex)
        {
            Exception = ex;
            StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}
