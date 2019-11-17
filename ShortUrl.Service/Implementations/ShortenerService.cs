using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using ShortUrl.Domain.Enums;
using ShortUrl.Domain.Models;
using ShortUrl.Repository.Interfaces;
using ShortUrl.Service.Interfaces;
using ShortUrl.Service.Helpers;
using Microsoft.Extensions.Configuration;
using ShortUrl.Repository.Entities;

namespace ShortUrl.Service.Implementations
{
    public class ShortenerService : IShortenerService
    {
        private readonly ServiceHelper _serviceHelper;
        private readonly IUrlInfoRepository _urlInfoRepository;        
        private readonly IValidationService _validationService;

        private static string _defaultUser;
        private static int _expirationOfShortUrlInMonths;
        private static int _numberOfTryIfDuplicateShortCode;        

        private readonly object _lock = new object();

        public ShortenerService(
            IConfiguration config,
            ServiceHelper serviceHelper,
            IValidationService validationService,
            IUrlInfoRepository urlInfoRepository
        )
        {            
            _serviceHelper = serviceHelper;
            _validationService = validationService;            
            _urlInfoRepository = urlInfoRepository;
            _defaultUser = config.GetValue<string>("AppSettings:DefaultUser");
            _expirationOfShortUrlInMonths = config.GetValue<int>("AppSettings:ExpirationOfShortUrlInMonths");
            _numberOfTryIfDuplicateShortCode = config.GetValue<int>("AppSettings:NumberOfTryIfDuplicateShortCode");
        }

        public OperationResult Shorten(string url)
        {
            lock (_lock)
            {                
                var validationResult = _validationService.IsValidUrl(url);
                if (validationResult.Result == Result.Failed) return validationResult;

                // TODO: encodedUrl for safety
                var encodedUrl = WebUtility.UrlEncode(url);

                var operationResult = validationResult.Code == UrlType.LongUrl.ToString()
                    ? GetShortUrl(encodedUrl)
                    : GetOriginalUrl(encodedUrl);

                return operationResult;
            }
        }
        
        private OperationResult GetShortUrl(string originalUrl)
        {
            var urlInfo = _urlInfoRepository.GetUrlInfoByOriginalUrl(originalUrl);
            if(urlInfo != null) return GetSuccessOperationResultForShortUrl(originalUrl, urlInfo.ShortCode);

            var shortCode = PrepareShortCode(originalUrl);
            if (string.IsNullOrEmpty(shortCode)) return GetConflictOperationResult();

            _urlInfoRepository.Insert(CreateUrlInfoObject(originalUrl, shortCode));
            return GetSuccessOperationResultForShortUrl(originalUrl, shortCode);
        }
        
        private string PrepareShortCode(string url)
        {
            for (var @try = 0; @try < _numberOfTryIfDuplicateShortCode; @try++)
            {
                var shortCodeLength = _serviceHelper.GetShortCodeLength(url);
                var shortCode = _serviceHelper.GenerateShortCode(shortCodeLength);
                var urlInfoEntity = _urlInfoRepository.GetUrlInfoByShortCode(shortCode);
                if (urlInfoEntity == null) return shortCode;
            }

            return string.Empty;
        }

        private static UrlInfoEntity CreateUrlInfoObject(string url, string shortCode)
        {
            return new UrlInfoEntity
            {
                OriginalUrl = url,
                ShortCode = shortCode,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = _defaultUser,
                ExpireDate = DateTime.UtcNow.AddMonths(_expirationOfShortUrlInMonths),
                UrlHits = 1
            };
        }               

        private OperationResult GetOriginalUrl(string shortUrl)
        {
            var shortCode = _serviceHelper.GetShortCodeFromUrl(shortUrl);
            var result = _validationService.IsValidShortCode(shortCode);
            if (result.Result == Result.Failed) return result;

            var urlInfo = _urlInfoRepository.GetUrlInfoByShortCode(shortCode);
            return urlInfo == null
                ? GetNotFoundOperationResult()
                : GetSuccessOperationResultForOriginalUrl(urlInfo.OriginalUrl);
        }

        private OperationResult GetSuccessOperationResultForShortUrl(string url, string shortCode)
        {
            var shortUrlBaseAddress = _serviceHelper.GetShortUrlBaseAddress(url);
            var shortUrl = $"{shortUrlBaseAddress}{shortCode}";
            return new OperationResult
            {
                Result = Result.Succeed,
                Code = StatusCodes.Status200OK.ToString(),
                Message = shortUrl
            };
        }

        private static OperationResult GetSuccessOperationResultForOriginalUrl(string originalUrl)
        {
            return new OperationResult
            {
                Result = Result.Succeed,
                Code = StatusCodes.Status200OK.ToString(),
                Message = WebUtility.UrlDecode(originalUrl)
            };
        }

        private static OperationResult GetNotFoundOperationResult()
        {
            return new OperationResult
            {
                Result = Result.Failed,
                Message = "No record found!",
                Code = HttpStatusCode.NotFound.ToString()
            };
        }

        private static OperationResult GetConflictOperationResult()
        {
            return new OperationResult
            {
                Result = Result.Failed,
                Code = HttpStatusCode.Conflict.ToString(),
                Message = $"ShortCode conflict occured in DB with {_numberOfTryIfDuplicateShortCode} try!"
            };
        }
    }
}
