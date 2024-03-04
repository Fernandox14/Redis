namespace redis_api
{
    public class ResponseModel
    {
        public object result { get; set; }
        public int status { get; set; }
        public string errorMessage { get; set; }
    }
}
