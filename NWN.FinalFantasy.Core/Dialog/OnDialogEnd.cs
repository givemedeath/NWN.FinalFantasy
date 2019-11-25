﻿using NWN.FinalFantasy.Core.Contracts;

namespace NWN.FinalFantasy.Core.Dialog
{
    public static class OnDialogEnd
    {
        public static void Run()
        {
            var player = _.GetPCSpeaker();
            var playerID = _.GetGlobalID(player);
            var dialog = Conversation.GetActivePlayerDialog(playerID);
            var convo = Conversation.FindConversation(dialog.ActiveDialogName);
            convo.EndDialog();
            Conversation.End(player);
            _.DeleteLocalInt(player, "DIALOG_SYSTEM_INITIALIZE_RAN");
        }
    }
}