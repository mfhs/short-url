using ShortUrl.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace ShortUrl.Service.Interfaces
{
    public interface IShortenerService
    {
        OperationResult Shorten(string url);
    }
}
