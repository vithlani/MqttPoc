using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MqttPoc.Repositories;

namespace MqttPOC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceTelemetryRepository _repository;

        public DevicesController(IDeviceTelemetryRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var data = await _repository.GetLatestTelemetryAsync();
            return Ok(data);
        }
    }
}
