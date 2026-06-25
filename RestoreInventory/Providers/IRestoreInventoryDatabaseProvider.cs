using Feli.RestoreInventory.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Feli.RestoreInventory.Providers;

public interface IRestoreInventoryDatabaseProvider
{
    Task AddInventoryAsync(StoredInventory inventory, int maxInventoriesToKeep);
    Task<IEnumerable<StoredInventory>> GetPlayerInventoriesAsync(ulong playerId);
}
