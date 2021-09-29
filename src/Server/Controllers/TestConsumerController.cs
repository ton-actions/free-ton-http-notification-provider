﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Server.SignalR;

namespace Server.Controllers
{
    [ApiController]
    [Route("test-consumer")]
    public class TestConsumerController : ControllerBase
    {
        private readonly ILogger<TestConsumerController> _logger;
        private readonly IHubContext<TestConsumerHub> _testConsumerHub;

        public TestConsumerController(ILogger<TestConsumerController> logger, IHubContext<TestConsumerHub> testConsumerHub)
        {
            _logger = logger;
            _testConsumerHub = testConsumerHub;
        }

        [HttpPost("{consumer}")]
        [Consumes("text/plain")]
        public async Task<IActionResult> Receive(string consumer, [FromBody] string message)
        {
            _logger.LogTrace("Received message: {Consumer} {Message}", consumer, message);
            await _testConsumerHub.Clients.User(consumer).SendAsync("ReceiveMessage", message);
            return Ok(message);
        }
    }
}