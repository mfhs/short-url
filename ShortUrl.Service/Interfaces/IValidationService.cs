using ShortUrl.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShortUrl.Service.Interfaces
{
    public interface IValidationService
    {
        OperationResult IsValidUrl(string url);
        OperationResult IsValidShortCode(string shortCode);
    }
}
