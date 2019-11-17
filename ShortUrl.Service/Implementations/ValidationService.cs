using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using ShortUrl.Domain;
using ShortUrl.Domain.Enums;
using ShortUrl.Domain.Models;
using ShortUrl.Service.Helpers;
using ShortUrl.Service.Interfaces;

namespace ShortUrl.Service.Implementations
{
    public class ValidationService: IValidationService
    {
        private static string _urlPattern;
        private static int _shortCodeMaxLength;
        private static int _shortCodeMinLength;

        private static ServiceHelper _serviceHelper;

        public ValidationService(IConfiguration config, ServiceHelper serviceHelper)
        {
            _urlPattern = config.GetValue<string>("AppSettings:UrlValidationRegex");
            _shortCodeMinLength = config.GetValue<int>("AppSettings:ShortCodeMinLength");
            _shortCodeMaxLength = config.GetValue<int>("AppSettings:ShortCodeMaxLength");
            _serviceHelper = serviceHelper;
        }

        public OperationResult IsValidUrl(string url)
        {
            return IsMatch(url) ? GetOperationResultObjectForValidUrl(url) : GetOperationResultObjectForInvalidUrl("Invalid url!");
        }
        
        private static bool IsMatch(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;            
            var reg = new Regex(_urlPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return reg.IsMatch(url);

            // return Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute);
        }

        private static OperationResult GetOperationResultObjectForValidUrl(string url)
        {
            return new OperationResult
            {
                Result = Result.Succeed,
                Message = "Valid url.",
                Code = _serviceHelper.IsShortUrl(url) ? UrlType.ShortUrl.ToString() : UrlType.LongUrl.ToString()
            };
        }

        private static OperationResult GetOperationResultObjectForInvalidUrl(string message)
        {
            return new OperationResult
            {
                Result = Result.Failed,
                Message = message,
                Code = HttpStatusCode.BadRequest.ToString()
            };
        }

        public OperationResult IsValidShortCode(string shortCode)
        {
            var pattern = $"^[a-zA-Z0-9]{{{_shortCodeMinLength},{_shortCodeMaxLength}}}$";
            var reg = new Regex(pattern, RegexOptions.Compiled);
            var isMatch = reg.IsMatch(shortCode);
            return isMatch
                ? GetOperationResultObjectForValidShortCode()
                : GetOperationResultObjectForInvalidUrl("Invalid short url!");

        }

        private static OperationResult GetOperationResultObjectForValidShortCode()
        {
            return new OperationResult
            {
                Result = Result.Succeed,
                Message = "Valid short code.",
                Code = UrlType.ShortUrl.ToString()
            };
        }
    }
}
