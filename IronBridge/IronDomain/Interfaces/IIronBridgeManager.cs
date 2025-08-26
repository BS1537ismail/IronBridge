using IronBridge.Models;

namespace IronBridge.IronDomain.Interfaces
{
    public interface IIronBridgeManager
    {
        Task<List<IronBridges>> GetIronBridge();
        Task<IronBridges> DeleteIronBridge(int id);
        Task<IronBridges> CreateIronBridgesAsync(IronBridges bridge);
        Task<IronBridges> UpdateIronBridgesAsync(int id, IronBridges bridge);
    }
}
