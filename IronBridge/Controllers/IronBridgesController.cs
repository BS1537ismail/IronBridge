using IronBridge.IronDomain.Interfaces;
using IronBridge.IronStorage.Interface;
using IronBridge.Models;
using IronBridge.ModelsDTO;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IronBridge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IronBridgesController : ControllerBase
    {
        private readonly ILogger<IronBridgesController> _logger;
        private readonly IIronBridgeManager ironBridgeManager;

        public IronBridgesController(ILogger<IronBridgesController> logger, IIronBridgeManager ironBridgeManager)
        {
            _logger = logger;
            this.ironBridgeManager = ironBridgeManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetIronBridgesAsync()
        {
            var result = await ironBridgeManager.GetIronBridge();
            return Ok(result.Adapt<List<IronBridges>>());
        }

        [HttpPost]
        public async Task<IActionResult> CreateIronBridgesAsync(CreateIronBridgeDto bridge)
        {
            var result = await ironBridgeManager.CreateIronBridgesAsync(bridge.Adapt<IronBridges>());
            return Ok(result);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateIronBridgesAsync(int id, CreateIronBridgeDto bridge)
        {
            var result = await ironBridgeManager.UpdateIronBridgesAsync(id, bridge.Adapt<IronBridges>());
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteIronBridgesByIdAsync(int id)
        {
            var result = await ironBridgeManager.DeleteIronBridge(id);
            return Ok(result.Adapt<IronBridges>());
        }
    }
}
