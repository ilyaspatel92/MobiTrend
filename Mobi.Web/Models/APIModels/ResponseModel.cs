namespace Mobi.Web.Models.APIModels
{
    public class ResponseModel<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }

        public Exception Exception { get; set; }


        public ResponseModel()
        {
            Success = true;
        }
    }
}
