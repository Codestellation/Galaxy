namespace Codestellation.Galaxy.WebEnd.Api
{
    public static class Response
    {
        public static Response<T> For<T>(T data)
        {
            return new Response<T>(data);
        }
    }
    
    public class Response<T>
    {
        public Response(Error error)
        {
            Error = error;
        }

        public Response(T data) : this(Error.Ok)
        {
            Data = data;
        }

        public Error Error { get; set; }
        public T Data { get; set; }
    }
}