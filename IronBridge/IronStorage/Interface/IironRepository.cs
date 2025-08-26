using IronBridge.Models;

namespace IronBridge.IronStorage.Interface
{
    public interface IironRepository
    {
        Task<List<IronBridges>> GetIronBridge();
        Task<IronBridges> DeleteIronBridge(int id);
        Task<IronBridges> CreateIronBridgesAsync(IronBridges bridge);
        Task<IronBridges> UpdateIronBridgesAsync(int id, IronBridges bridge);
    }
}
