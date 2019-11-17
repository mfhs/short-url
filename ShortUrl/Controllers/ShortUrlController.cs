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

            return operationResult.Result == Result.Succeed
                ? new OkObjectResult(operationResult.Message)
                : GetFailedActionResult(operationResult);
        }

        private static ActionResult GetFailedActionResult(OperationResult operationResult)
        {
            if (operationResult.Code == HttpStatusCode.BadRequest.ToString())
                return new BadRequestObjectResult(operationResult.Message);

            if (operationResult.Code == HttpStatusCode.NotFound.ToString())
                return new NotFoundObjectResult(operationResult.Message);

            return operationResult.Code == HttpStatusCode.Conflict.ToString()
                ? (ActionResult) new ConflictObjectResult(new DBConcurrencyException(operationResult.Message))
                : new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
