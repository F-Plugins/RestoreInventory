using Feli.RestoreInventory.Extensions;
using Feli.Rocket.Utils.Threading;
using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Feli.RestoreInventory.Commands;

public class RestoreInventoryListCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        var player = (UnturnedPlayer)caller;

        ThreadTool.RunOnThreadPool(async () =>
        {
            var inventories = (await RestoreInventoryPlugin.Instance.DatabaseProvider.GetPlayerInventoriesAsync(player.CSteamID.m_SteamID)).ToArray();

            ThreadTool.QueueOnMainThread(() =>
            {
                if (inventories.Length == 0)
                {
                    player.SendLocalizedMessage("RestoreInventoryListEmpty", Color.red);
                    return;
                }

                player.SendLocalizedMessage("RestoreInventoryListHeader", Color.cyan);

                for (var i = 0; i < inventories.Length; i++)
                {
                    var inventory = inventories[i];
                    player.SendLocalizedMessage("RestoreInventoryListItem", Color.cyan, i + 1, inventory.ItemCount, inventory.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                }
            });
        });
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;

    public string Name => "restoreinventorylist";

    public string Help => "List your saved inventories";

    public string Syntax => string.Empty;

    public List<string> Aliases => new() { "ril", "rinventories" };

    public List<string> Permissions => new();
}
