using Feli.RestoreInventory.Extensions;
using Feli.Rocket.Utils.Threading;
using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Feli.RestoreInventory.Commands;

public class RestoreInventoryRestoreCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length == 0 || !int.TryParse(command[0], out var inventoryNumber) || inventoryNumber < 1)
        {
            caller.SendLocalizedMessage("RestoreInventoryRestoreSyntax", Color.red);
            return;
        }

        var player = (UnturnedPlayer)caller;

        ThreadTool.RunOnThreadPool(async () =>
        {
            var inventories = (await RestoreInventoryPlugin.Instance.DatabaseProvider.GetPlayerInventoriesAsync(player.CSteamID.m_SteamID)).ToArray();

            if (inventories.Length == 0)
            {
                player.SendLocalizedMessage("RestoreInventoryListEmpty", Color.red);
                return;
            }

            var inventory = inventories.Skip(inventoryNumber - 1).FirstOrDefault();

            if (inventory is null)
            {
                player.SendLocalizedMessage("RestoreInventoryNotFound", Color.red, inventoryNumber);
                return;
            }

            ThreadTool.QueueOnMainThread(() =>
            {
                inventory.Restore(player.Player);
                player.SendLocalizedMessage("RestoreInventoryRestored", Color.green, inventoryNumber);
            });
        });
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;

    public string Name => "restoreinventory";

    public string Help => "Restore a saved inventory";

    public string Syntax => "<number>";

    public List<string> Aliases => new() { "ri" };

    public List<string> Permissions => new();
}
