using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OpenMod.API.Users;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using Command = OpenMod.Core.Commands.Command;

namespace RestoreInventory.Commands
{
    public class Commands
    {
        [Command("restoreinventory")]
        [CommandAlias("restoreinv")]
        [CommandAlias("invrestore")]
        [CommandDescription("A command to restore a player inventory")]
        [CommandSyntax("/restoreinventory <playerName>")]
        public class RestoreInventoryCommand : Command
        {
            private readonly RestoreInventory m_Plugin;
            private readonly IUserManager m_UserManager;
            public RestoreInventoryCommand(RestoreInventory plugin, IUserManager userManager, IServiceProvider serviceProvider) : base(serviceProvider)
            {
                m_Plugin = plugin;
                m_UserManager = userManager;
            }

            protected override async Task OnExecuteAsync()
            {
                var uplayer = (UnturnedUser)Context.Actor;
                if (Context.Parameters.Length > 0)
                {
                    var player = PlayerTool.getSteamPlayer(await Context.Parameters.GetAsync<string>(0));
                    if (player == null)
                    {
                        await uplayer.PrintMessageAsync("Player not found !");
                        return;
                    }

                    if (m_Plugin.Restorer.ContainsKey(uplayer.Player.Player))
                    {
                        m_Plugin.Restorer.Remove(uplayer.Player.Player);
                    }

                    m_Plugin.Restorer.Add(uplayer.Player.Player, player);
                    var inventories = m_Plugin.GetInventories().Where(x => x.PlayerId == player.player.channel.owner.playerID.steamID.ToString()).OrderBy(x => x.Date).ToList();
                    var count = inventories.Count;
                    await UniTask.SwitchToMainThread();
                    switch (count)
                    {
                        case 0:
                            await uplayer.PrintMessageAsync("The player hasn't any saved inventory !");
                            break;
                        case 1:
                            EffectManager.sendUIEffect(3331, 323, uplayer.SteamId, true);
                            EffectManager.sendUIEffectText(323, uplayer.SteamId, true, "Fecha1", $"{inventories[0].Date.Day}/{inventories[0].Date.Month}/{inventories[0].Date.Year} - {inventories[0].Date.Hour}:{inventories[0].Date.Minute}");
                            EffectManager.sendUIEffectVisibility(323, uplayer.SteamId, true, "2", false);
                            EffectManager.sendUIEffectVisibility(323, uplayer.SteamId, true, "3", false);
                            uplayer.Player.Player.enablePluginWidgetFlag(EPluginWidgetFlags.Modal);
                            break;
                        case 2:
                            EffectManager.sendUIEffect(3331, 323, uplayer.SteamId, true);
                            EffectManager.sendUIEffectText(323, uplayer.SteamId, true, "Fecha1", $"{inventories[0].Date.Day}/{inventories[0].Date.Month}/{inventories[0].Date.Year} - {inventories[0].Date.Hour}:{inventories[0].Date.Minute}");
                            EffectManager.sendUIEffectText(323, uplayer.SteamId, true, "Fecha2", $"{inventories[1].Date.Day}/{inventories[1].Date.Month}/{inventories[1].Date.Year} - {inventories[1].Date.Hour}:{inventories[1].Date.Minute}");
                            uplayer.Player.Player.enablePluginWidgetFlag(EPluginWidgetFlags.Modal);
                            EffectManager.sendUIEffectVisibility(323, uplayer.SteamId, true, "3", false);
                            break;
                        case 3:
                            EffectManager.sendUIEffect(3331, 323, uplayer.SteamId, true);
                            EffectManager.sendUIEffectText(323, uplayer.SteamId, true, "Fecha1", $"{inventories[0].Date.Day}/{inventories[0].Date.Month}/{inventories[0].Date.Year} - {inventories[0].Date.Hour}:{inventories[0].Date.Minute}");
                            EffectManager.sendUIEffectText(323, uplayer.SteamId, true, "Fecha2", $"{inventories[1].Date.Day}/{inventories[1].Date.Month}/{inventories[1].Date.Year} - {inventories[1].Date.Hour}:{inventories[1].Date.Minute}");
                            EffectManager.sendUIEffectText(323, uplayer.SteamId, true, "Fecha3", $"{inventories[2].Date.Day}/{inventories[2].Date.Month}/{inventories[2].Date.Year} - {inventories[2].Date.Hour}:{inventories[2].Date.Minute}");
                            uplayer.Player.Player.enablePluginWidgetFlag(EPluginWidgetFlags.Modal);
                            break;
                    }
                }
                else
                {
                    await uplayer.PrintMessageAsync("usage: /restoreinventory <playerName>");
                }
            }
        }
    }
}
