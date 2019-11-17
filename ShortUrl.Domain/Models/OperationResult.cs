using ShortUrl.Domain.Enums;

namespace ShortUrl.Domain.Models
{
    public class OperationResult
    {
        public Result Result { get; set; }

        public string Message { get; set; }

        public string Code { get; set; }
    }
}
