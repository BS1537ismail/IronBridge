using IronBridge.IronStorage.Interface;
using IronBridge.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace IronBridge.IronStorage.Repository
{
    public class IronRepository : IironRepository
    {
        private readonly ApplicationDbContext context;

        public IronRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<List<IronBridges>> GetIronBridge()
        {
            var data = await context.IronBridges.ToListAsync();
            return data;
        }

        public async Task<IronBridges> DeleteIronBridge(int id)
        {
            var bridge = await context.IronBridges.FindAsync(id);

            if (bridge == null)
            {
                throw new ArgumentException($"'{bridge.Id}' doesn't exist.");
            }

            context.IronBridges.Remove(bridge);
            await context.SaveChangesAsync();

            return bridge.Adapt<IronBridges>();
        }
        public async Task<IronBridges> CreateIronBridgesAsync(IronBridges bridge)
        {
            await context.IronBridges.AddAsync(bridge);
            await context.SaveChangesAsync();

            return bridge;
        }

        public async Task<IronBridges> UpdateIronBridgesAsync(int id, IronBridges bridge)
        {
            var existingBridge = await context.IronBridges.FindAsync(id);
            if (existingBridge == null)
            {
                throw new ArgumentException($"'{bridge.Id}' doesn't exist.");
            }
            existingBridge.Name = bridge.Name;
            await context.SaveChangesAsync();

            return existingBridge;
        }
    }
}
