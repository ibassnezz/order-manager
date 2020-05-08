namespace OrderManager.Integration
{
    public class ApiResponse
    {
        public bool IsSuccess { get; }
        public string Content { get; }

        public ApiResponse(bool isSuccess, string content)
        {
            IsSuccess = isSuccess;
            Content = content;
        }
    }
}
