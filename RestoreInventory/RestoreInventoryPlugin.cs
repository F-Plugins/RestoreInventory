using Feli.RestoreInventory.Configuration;
using Feli.RestoreInventory.Models;
using Feli.RestoreInventory.Providers;
using Feli.Rocket.Utils.Threading;
using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;

namespace Feli.RestoreInventory;

public class RestoreInventoryPlugin : RocketPlugin<RestoreInventoryPluginConfiguration>
{
    public static RestoreInventoryPlugin Instance { get; set; }
    public IRestoreInventoryDatabaseProvider DatabaseProvider { get; set; }

    protected override void Load()
    {
        Logger.Log("Loading RestoreInventory by Feli...");

        Instance = this;
        DatabaseProvider = new LiteDBRestoreInventoryDatabaseProvider(Directory);

        UnturnedPlayerEvents.OnPlayerDeath += OnPlayerDeath;
    }

    protected override void Unload()
    {
        UnturnedPlayerEvents.OnPlayerDeath -= OnPlayerDeath;

        DatabaseProvider = null;
        Instance = null;
    }

    private void OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
    {
        var inventory = StoredInventory.FromPlayer(player.Player);

        if (inventory.ItemCount == 0)
            return;

        var maxInventoriesToKeep = Configuration.Instance.GetMaxInventoriesToKeep();

        ThreadTool.RunOnThreadPool(async () =>
        {
            try
            {
                await DatabaseProvider.AddInventoryAsync(inventory, maxInventoriesToKeep);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Failed to store player inventory");
            }
        });
    }

    public override TranslationList DefaultTranslations => new()
    {
        {"RestoreInventoryListSyntax", "Correct usage: /restoreinventorylist <player>" },
        {"RestoreInventoryPlayerNotFound", "Could not find player {0}." },
        {"RestoreInventoryListEmpty", "{0} doesn't have any saved inventories." },
        {"RestoreInventoryListHeader", "Saved inventories for {0}:" },
        {"RestoreInventoryListItem", "{0}. {1} items - {2}" },
        {"RestoreInventoryRestoreSyntax", "Correct usage: /restoreinventory <player> <number>" },
        {"RestoreInventoryInvalidNumber", "Invalid inventory number." },
        {"RestoreInventoryNotFound", "Could not find inventory #{0} for {1}." },
        {"RestoreInventoryRestored", "Successfully restored inventory #{0} to {1}. Items that did not fit were dropped." }
    };
}
