namespace Codestellation.Galaxy.WebEnd.Api
{
    public class Error
    {
        public static readonly Error Ok = new Error {Code = 0};

        public int Code;
        public string Message;
    }
}