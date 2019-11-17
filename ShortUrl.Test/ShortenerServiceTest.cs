using Xunit;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using ShortUrl.Domain.Enums;
using ShortUrl.Repository.Interfaces;
using ShortUrl.Service.Implementations;
using ShortUrl.Service.Helpers;
using ShortUrl.Service.Interfaces;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using ShortUrl.Repository.Entities;
using Microsoft.AspNetCore.Http;

namespace ShortUrl.Test
{
    public class ShortenerServiceTest
    {
        private static IConfiguration _configuration;
        private static ServiceHelper _serviceHelper;
        private static IValidationService _validationService;
        private static IUrlInfoRepository _urlInfoRepository;

        private const string InvalidLongUrl = "asdjad9ud_asdhj-sadkhsfdkljasf-asdkjkasdjkladknet+core+3.0&oq=.net+Core+&gs_l=psy-";
        private const string InvalidShortUrl = "https://s.my/jsidf87radjk-asjk";
        private const string LongUrl = "https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-core-3-0";
        private const string ShortUrlHost = "https://s.my/";
        private const string TestShortCode = "Gt9JioQ1";        

        public ShortenerServiceTest()
        {
            _configuration = TestHelper.GetConfiguration();
            _serviceHelper = TestHelper.GetServiceHelper();
            _validationService = TestHelper.GetValidationService();                        
        }

        [Fact(DisplayName = "Given a valid new long URL, will return a new valid shorter url with Status 200 OK")]
        public void TestShortenUrlForValidNewLongUrl()
        {
            //Arrange            
            _urlInfoRepository = Substitute.For<IUrlInfoRepository>();
            _urlInfoRepository.GetUrlInfoByOriginalUrl(Arg.Any<string>()).ReturnsNull();
            _urlInfoRepository.GetUrlInfoByShortCode(Arg.Any<string>()).ReturnsNull();
            _urlInfoRepository.Insert(Arg.Any<UrlInfoEntity>()).Returns(true);            
            var shortenerService = new ShortenerService(_configuration, _serviceHelper, _validationService, _urlInfoRepository);

            // Act
            var operationResult = shortenerService.Shorten(LongUrl);
            var shortCode = _serviceHelper.GetShortCodeFromUrl(operationResult.Value);
            var validationResult = _validationService.IsValidShortCode(shortCode);

            // Assert
            Assert.Equal(OperationStatus.Succeed, operationResult.OperationStatus);
            Assert.Equal(StatusCodes.Status200OK.ToString(), operationResult.Code);            
            Assert.Equal(OperationStatus.Succeed, validationResult.OperationStatus);
            Assert.StartsWith(ShortUrlHost, operationResult.Value);
        }

        [Fact(DisplayName = "Given valid short url (shorten before), will return the original long url with Status 200 OK")]
        public void TestShortenUrlForValidShortUrl()
        {
            //Arrange
            _urlInfoRepository = Substitute.For<IUrlInfoRepository>();
            _urlInfoRepository.GetUrlInfoByOriginalUrl(Arg.Any<string>()).ReturnsNull();
            _urlInfoRepository.GetUrlInfoByShortCode(Arg.Any<string>()).Returns(GetUrlInfoEntityStub());
            _urlInfoRepository.Update(Arg.Any<UrlInfoEntity>()).Returns(true);
            var shortenerService = new ShortenerService(_configuration, _serviceHelper, _validationService, _urlInfoRepository);

            // Act
            var operationResult = shortenerService.Shorten($"{ShortUrlHost}{TestShortCode}");
            
            // Assert
            Assert.Equal(OperationStatus.Succeed, operationResult.OperationStatus);
            Assert.Equal(StatusCodes.Status200OK.ToString(), operationResult.Code);
            Assert.Equal(LongUrl, operationResult.Value);
        }

        private static UrlInfoEntity GetUrlInfoEntityStub()
        {
            return new UrlInfoEntity
            {
                Id = 1,
                OriginalUrl = LongUrl,
                ShortCode = TestShortCode
            };
        }

        [Fact(DisplayName = "Given invalid long url, will return the fail operation with Status 400 BadRequest")]
        public void TestShortenUrlForInValidLongUrl()
        {
            //Arrange
            _urlInfoRepository = Substitute.For<IUrlInfoRepository>();            
            var shortenerService = new ShortenerService(_configuration, _serviceHelper, _validationService, _urlInfoRepository);

            // Act
            var operationResult = shortenerService.Shorten(InvalidLongUrl);

            // Assert
            Assert.Equal(OperationStatus.Failed, operationResult.OperationStatus);
            Assert.Equal(StatusCodes.Status400BadRequest.ToString(), operationResult.Code);            
        }

        [Fact(DisplayName = "Given invalid short url, will return the fail operation with Status 400 BadRequest")]
        public void TestShortenUrlForInValidShortUrl()
        {
            //Arrange
            _urlInfoRepository = Substitute.For<IUrlInfoRepository>();
            var shortenerService = new ShortenerService(_configuration, _serviceHelper, _validationService, _urlInfoRepository);

            // Act
            var operationResult = shortenerService.Shorten(InvalidShortUrl);

            // Assert
            Assert.Equal(OperationStatus.Failed, operationResult.OperationStatus);
            Assert.Equal(StatusCodes.Status400BadRequest.ToString(), operationResult.Code);
        }

        [Fact(DisplayName = "Given valid original url, the shortened URL (at least the relative URL/shortCode) will be shorter in length than the provided URL with Status 200 OK")]
        public void TestShortenUrlForUrlLength()
        {
            //Arrange
            _urlInfoRepository = Substitute.For<IUrlInfoRepository>();
            var shortenerService = new ShortenerService(_configuration, _serviceHelper, _validationService, _urlInfoRepository);
            const string urlWithLength4 = "t.co";

            // Act
            var operationResultForLongUrl = shortenerService.Shorten(LongUrl);
            var relativeUrlAsShortCodeForLongUrl = _serviceHelper.GetShortCodeFromUrl(operationResultForLongUrl.Value);

            var operationResultForUrlWithLength4 = shortenerService.Shorten(urlWithLength4);
            var relativeUrlAsShortCodeForUrlWithLength4 = _serviceHelper.GetShortCodeFromUrl(operationResultForUrlWithLength4.Value);

            // Assert
            Assert.Equal(OperationStatus.Succeed, operationResultForLongUrl.OperationStatus);            
            Assert.Equal(StatusCodes.Status200OK.ToString(), operationResultForLongUrl.Code);
            Assert.True(relativeUrlAsShortCodeForLongUrl.Length < LongUrl.Length);

            Assert.Equal(OperationStatus.Succeed, operationResultForUrlWithLength4.OperationStatus);            
            Assert.Equal(StatusCodes.Status200OK.ToString(), operationResultForUrlWithLength4.Code);
            Assert.True(relativeUrlAsShortCodeForUrlWithLength4.Length < urlWithLength4.Length);
        }
    }
}
