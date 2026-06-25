using Feli.Rocket.Utils.Threading;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;

namespace Feli.RestoreInventory.Extensions;

internal static class IRocketPlayerExtensions
{
    internal static void SendLocalizedMessage(this IRocketPlayer player, string translationKey, Color color, params object[] args)
    {
        ThreadTool.QueueOnMainThread(() => UnturnedChat.Say(player, RestoreInventoryPlugin.Instance.Translate(translationKey, args), color, true));
    }
}
