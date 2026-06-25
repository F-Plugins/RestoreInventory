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
        if (command.Length < 2 || !int.TryParse(command[1], out var inventoryNumber) || inventoryNumber < 1)
        {
            caller.SendLocalizedMessage("RestoreInventoryRestoreSyntax", Color.red);
            return;
        }

        var player = UnturnedPlayer.FromName(command[0]);

        if (player is null)
        {
            caller.SendLocalizedMessage("RestoreInventoryPlayerNotFound", Color.red, command[0]);
            return;
        }

        ThreadTool.RunOnThreadPool(async () =>
        {
            var inventories = (await RestoreInventoryPlugin.Instance.DatabaseProvider.GetPlayerInventoriesAsync(player.CSteamID.m_SteamID)).ToArray();

            if (inventories.Length == 0)
            {
                caller.SendLocalizedMessage("RestoreInventoryListEmpty", Color.red, player.DisplayName);
                return;
            }

            var inventory = inventories.Skip(inventoryNumber - 1).FirstOrDefault();

            if (inventory is null)
            {
                caller.SendLocalizedMessage("RestoreInventoryNotFound", Color.red, inventoryNumber, player.DisplayName);
                return;
            }

            ThreadTool.QueueOnMainThread(() =>
            {
                inventory.Restore(player.Player);
                caller.SendLocalizedMessage("RestoreInventoryRestored", Color.green, inventoryNumber, player.DisplayName);
            });
        });
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;

    public string Name => "restoreinventory";

    public string Help => "Restore a saved inventory to a player";

    public string Syntax => "<player> <number>";

    public List<string> Aliases => new() { "ri" };

    public List<string> Permissions => new();
}
