using IronBridge.IronDomain.Interfaces;
using IronBridge.IronStorage.Interface;
using IronBridge.Models;
using Mapster;

namespace IronBridge.IronDomain.Managers
{
    public class IronBridgeManager : IIronBridgeManager
    {
        private readonly IironRepository iironRepository;
        private readonly ICacheService _cacheService;

        public IronBridgeManager(IironRepository iironRepository, ICacheService cacheService)
        {
            this.iironRepository = iironRepository;
            _cacheService = cacheService;
        }
        public async Task<List<IronBridges>> GetIronBridge()
        {
            const string cacheKey = "all_iron_bridges";
            
            var cachedData = await _cacheService.GetAsync<List<IronBridges>>(cacheKey);
            if (cachedData != null)
                return cachedData;
            
            var data = await iironRepository.GetIronBridge();
            var result = data.Adapt<List<IronBridges>>();
            
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));
            return result;
        }

        public async Task<IronBridges> DeleteIronBridge(int id)
        {
            var bridge = await iironRepository.DeleteIronBridge(id);

            if (bridge == null)
            {
                throw new ArgumentException($"Bridge with id '{id}' doesn't exist.");
            }

            await _cacheService.RemoveAsync("all_iron_bridges");
            return bridge.Adapt<IronBridges>();
        }
        public async Task<IronBridges> CreateIronBridgesAsync(IronBridges bridge)
        {
            var createdBridge = await iironRepository.CreateIronBridgesAsync(bridge);

            await _cacheService.RemoveAsync("all_iron_bridges");
            return createdBridge.Adapt<IronBridges>();
        }

        public async Task<IronBridges> UpdateIronBridgesAsync(int id, IronBridges bridge)
        {
            var existingBridge = await iironRepository.UpdateIronBridgesAsync(id, bridge);
            if (existingBridge == null)
            {
                throw new ArgumentException($"Bridge with id '{id}' doesn't exist.");
            }

            await _cacheService.RemoveAsync("all_iron_bridges");
            return existingBridge.Adapt<IronBridges>();
        }
    }
}
