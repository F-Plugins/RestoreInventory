using Feli.RestoreInventory.Models;
using LiteDB;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Feli.RestoreInventory.Providers;

public class LiteDBRestoreInventoryDatabaseProvider : IRestoreInventoryDatabaseProvider
{
    private ILiteDatabase Database => new LiteDatabase(Path.Combine(_workingDirectory, "restoreinventory.db"));
    private readonly string _workingDirectory;
    private readonly object _lock = new();

    public LiteDBRestoreInventoryDatabaseProvider(string workingDirectory)
    {
        _workingDirectory = workingDirectory;

        try
        {
            var mapper = BsonMapper.Global;

            mapper.Entity<StoredInventory>()
                .Id(x => x.Id)
                .Ignore(x => x.ItemCount);
        }
        catch
        {
        }
    }

    public Task AddInventoryAsync(StoredInventory inventory, int maxInventoriesToKeep)
    {
        lock (_lock)
        {
            using var database = Database;
            var collection = database.GetCollection<StoredInventory>("inventories");
            collection.EnsureIndex(x => x.PlayerId);
            collection.EnsureIndex(x => x.CreatedAt);

            collection.Insert(inventory);

            if (maxInventoriesToKeep != -1)
            {
                var inventoriesToDelete = collection.Query()
                    .Where(x => x.PlayerId == inventory.PlayerId)
                    .OrderByDescending(x => x.CreatedAt)
                    .Offset(maxInventoriesToKeep)
                    .ToList();

                foreach (var inventoryToDelete in inventoriesToDelete)
                    collection.Delete(inventoryToDelete.Id);
            }

            return Task.CompletedTask;
        }
    }

    public Task<IEnumerable<StoredInventory>> GetPlayerInventoriesAsync(ulong playerId)
    {
        lock (_lock)
        {
            using var database = Database;
            var collection = database.GetCollection<StoredInventory>("inventories");
            var result = collection.Query()
                .Where(x => x.PlayerId == playerId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return Task.FromResult(result as IEnumerable<StoredInventory>);
        }
    }
}
