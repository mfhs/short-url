using Microsoft.Extensions.Configuration;
using ShortUrl.Service.Helpers;
using ShortUrl.Service.Implementations;
using ShortUrl.Service.Interfaces;

namespace ShortUrl.Test
{
    public class TestHelper
    {
        public static IConfiguration GetConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();

            return config;
        }

        public static ServiceHelper GetServiceHelper()
        {
            return new ServiceHelper(GetConfiguration());
        }

        public static IValidationService GetValidationService()
        {
            return new ValidationService(GetConfiguration(), GetServiceHelper());
        }
    }
}
