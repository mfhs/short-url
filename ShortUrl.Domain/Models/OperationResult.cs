using ShortUrl.Domain.Enums;

namespace ShortUrl.Domain.Models
{
    public class OperationResult
    {
        public OperationStatus OperationStatus { get; set; }

        public string Value { get; set; }

        public string Code { get; set; }
    }
}
