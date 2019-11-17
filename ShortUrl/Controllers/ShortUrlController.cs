using System.Data;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShortUrl.Domain.Enums;
using ShortUrl.Domain.Models;
using ShortUrl.Service.Interfaces;

namespace ShortUrl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShortUrlController : ControllerBase
    {
        private readonly IShortenerService _shortenerService;

        public ShortUrlController(IShortenerService shortenerService)
        {
            _shortenerService = shortenerService;
        }

        // POST api/ShortUrl
        [HttpPost]
        public ActionResult Post([FromBody] string url)
        {
            var operationResult = _shortenerService.Shorten(url);

            return operationResult.OperationStatus == OperationStatus.Succeed
                ? new OkObjectResult(operationResult.Value)
                : GetFailedActionResult(operationResult);
        }

        private static ActionResult GetFailedActionResult(OperationResult operationResult)
        {
            if (operationResult.Code == StatusCodes.Status400BadRequest.ToString())
                return new BadRequestObjectResult(operationResult.Value);

            if (operationResult.Code == StatusCodes.Status404NotFound.ToString())
                return new NotFoundObjectResult(operationResult.Value);

            return operationResult.Code == StatusCodes.Status409Conflict.ToString()
                ? (ActionResult) new ConflictObjectResult(new DBConcurrencyException(operationResult.Value))
                : new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
