using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using ShortUrl.Domain;

namespace ShortUrl.Service.Helpers
{
    public sealed class ServiceHelper
    {
        private static int _shortCodeMaxLength;
        private static char[] _shortCodeCharacterSetArray;
        private static RNGCryptoServiceProvider _cryptoServiceProvider;
        private static int _characterBase;        

        public ServiceHelper(IConfiguration config)
        {            
            _shortCodeMaxLength = config.GetValue<int>("AppSettings:ShortCodeMaxLength");
            _shortCodeCharacterSetArray = config.GetValue<string>("AppSettings:ShortCodeCharacterSet").ToCharArray();
            _characterBase = _shortCodeCharacterSetArray.Length;
            _cryptoServiceProvider = new RNGCryptoServiceProvider();
        }
        public string GenerateShortCode(int length = 6)
        {   
            var byteArray = new byte[length];
            _cryptoServiceProvider.GetNonZeroBytes(byteArray);

            var shortCode = new StringBuilder(length);
            foreach (var currentByte in byteArray)
            {
                var position = currentByte % (_characterBase - 1);
                shortCode.Append(_shortCodeCharacterSetArray[position]);
            }

            return shortCode.ToString();
        }
        
        public int GetShortCodeLength(string url)
        {
            var shortCodeLength = url.Length > _shortCodeMaxLength ? _shortCodeMaxLength : url.Length - 1;
            return shortCodeLength;
        }

        public string GetShortUrlBaseAddress(string url)
        {
            // TODO: Instead of Constant, we can move to config file
            var baseAddress = url.StartsWith(ShortUrlInfo.SecureDomain)
                ? ShortUrlInfo.SecureDomain
                : ShortUrlInfo.HttpDomain;

            return baseAddress;
        }

        public bool IsShortUrl(string url)
        {
            return url.StartsWith(ShortUrlInfo.SecureDomain) ||
                   url.StartsWith(ShortUrlInfo.HttpDomain) ||
                   url.StartsWith(ShortUrlInfo.Host);
        }       

        public string GetShortCodeFromUrl(string shortUrl)
        {
            if (string.IsNullOrEmpty(shortUrl)) return string.Empty;
            var decodedShortUrl = WebUtility.UrlDecode(shortUrl);
            var shortCode = decodedShortUrl.Substring(decodedShortUrl.LastIndexOf(@"/", StringComparison.Ordinal) + 1);
            return shortCode;
        }
    }
}
