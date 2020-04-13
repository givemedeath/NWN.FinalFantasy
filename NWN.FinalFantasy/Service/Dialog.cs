﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NWN.FinalFantasy.Core;
using NWN.FinalFantasy.Core.NWScript.Enum;
using NWN.FinalFantasy.Service.DialogService;
using static NWN.FinalFantasy.Core.NWScript.NWScript;

namespace NWN.FinalFantasy.Service
{
    public static class Dialog
    {
        private const int NumberOfDialogs = 255;
        private const int NumberOfResponsesPerPage = 12;
        private static Dictionary<string, PlayerDialog> PlayerDialogs { get; } = new Dictionary<string, PlayerDialog>();
        private static Dictionary<int, bool> DialogFilesInUse { get; } = new Dictionary<int, bool>();
        private static readonly Dictionary<string, IConversation> _conversations = new Dictionary<string, IConversation>();

        /// <summary>
        /// When the module is loaded, the assembly will be searched for conversations.
        /// These will be added to the cache for use at a later time.
        /// </summary>
        [NWNEventHandler("mod_load")]
        public static void RegisterConversations()
        {
            // Use reflection to get all of the conversation implementations.
            var classes = Assembly.GetCallingAssembly().GetTypes()
                .Where(p => typeof(IConversation).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract).ToArray();
            foreach (var type in classes)
            {
                IConversation instance = Activator.CreateInstance(type) as IConversation;
                if (instance == null)
                {
                    throw new NullReferenceException("Unable to activate instance of type: " + type);
                }
                _conversations.Add(type.Name, instance);
            }
        }

        [NWNEventHandler("mod_load")]
        public static void InitializeDialogs()
        {
            for (int x = 1; x <= NumberOfDialogs; x++)
            {
                DialogFilesInUse.Add(x, false);
            }
        }

        /// <summary>
        /// Retrieves a conversation from the cache.
        /// </summary>
        /// <param name="key">The name of the conversation.</param>
        /// <returns>A conversation instance</returns>
        public static IConversation GetConversation(string key)
        {
            if (!_conversations.ContainsKey(key))
            {
                throw new KeyNotFoundException("Conversation '" + key + "' is not registered. Did you create a class for it?");
            }

            return _conversations[key];
        }

        /// <summary>
        /// Handles when a dialog is started.
        /// </summary>
        [NWNEventHandler("dialog_start")]
        public static void Start()
        {
            var player = GetLastUsedBy();
            if (!GetIsObjectValid(player)) player = (GetPCSpeaker());

            string conversation = GetLocalString(OBJECT_SELF, "CONVERSATION");

            if (!string.IsNullOrWhiteSpace(conversation))
            {
                var objectType = GetObjectType(OBJECT_SELF);
                if (objectType == ObjectType.Placeable)
                {
                    var talkTo = OBJECT_SELF;
                    StartConversation(player, talkTo, conversation);
                }
                else
                {
                    var talkTo = (OBJECT_SELF);
                    StartConversation(player, talkTo, conversation);
                }
            }
            else
            {
                ActionStartConversation(player, "", true, false);
            }

        }

        [NWNEventHandler("dialog_action_0")]
        public static void NodeAction0()
        {
            ActionsTaken(0);
        }

        [NWNEventHandler("dialog_action_1")]
        public static void NodeAction1()
        {
            ActionsTaken(1);
        }

        [NWNEventHandler("dialog_action_2")]
        public static void NodeAction2()
        {
            ActionsTaken(2);
        }

        [NWNEventHandler("dialog_action_3")]
        public static void NodeAction3()
        {
            ActionsTaken(3);
        }

        [NWNEventHandler("dialog_action_4")]
        public static void NodeAction4()
        {
            ActionsTaken(4);
        }

        [NWNEventHandler("dialog_action_5")]
        public static void NodeAction5()
        {
            ActionsTaken(5);
        }

        [NWNEventHandler("dialog_action_6")]
        public static void NodeAction6()
        {
            ActionsTaken(6);
        }

        [NWNEventHandler("dialog_action_7")]
        public static void NodeAction7()
        {
            ActionsTaken(7);
        }

        [NWNEventHandler("dialog_action_8")]
        public static void NodeAction8()
        {
            ActionsTaken(8);
        }

        [NWNEventHandler("dialog_action_9")]
        public static void NodeAction9()
        {
            ActionsTaken(9);
        }

        [NWNEventHandler("dialog_action_10")]
        public static void NodeAction10()
        {
            ActionsTaken(10);
        }

        [NWNEventHandler("dialog_action_11")]
        public static void NodeAction11()
        {
            ActionsTaken(11);
        }

        [NWNEventHandler("dialog_appears_0")]
        public static int NodeAppears0()
        {
            return AppearsWhen(2, 0) ? 1 : 0;
        }

        [NWNEventHandler("dialog_appears_1")]
        public static int NodeAppears1()
        {
            return AppearsWhen(2, 1) ? 1 : 0;
        }

        [NWNEventHandler("dialog_appears_2")]
        public static int NodeAppears2()
        {
            return AppearsWhen(2, 2) ? 1 : 0;
        }

        [NWNEventHandler("dialog_appears_3")]
        public static int NodeAppears3()
        {
            return AppearsWhen(2, 3) ? 1 : 0;
        }

        [NWNEventHandler("dialog_appears_4")]
        public static int NodeAppears4()
        {
            return AppearsWhen(2, 4) ? 1 : 0;
        }

        [NWNEventHandler("dialog_appears_5")]
        public static int NodeAppears5()
        {
            return AppearsWhen(2, 5) ? 1 : 0;
        }

        [NWNEventHandler("dialog_appears_6")]
        public static int NodeAppears6()
        {
            return AppearsWhen(2, 6) ? 1 : 0;
        }

        [NWNEventHandler("dialog_appears_7")]
        public static int NodeAppears7()
        {
            return AppearsWhen(2, 7) ? 1 : 0;
        }

        [NWNEventHandler("dialog_appears_8")]
        public static int NodeAppears8()
        {
            return AppearsWhen(2, 8) ? 1 : 0;
        }

        [NWNEventHandler("dialog_appears_9")]
        public static int NodeAppears9()
        {
            return AppearsWhen(2, 9) ? 1 : 0;
        }

        [NWNEventHandler("dialog_appears10")]
        public static int NodeAppears10()
        {
            return AppearsWhen(2, 10) ? 1 : 0;
        }

        [NWNEventHandler("dialog_appears11")]
        public static int NodeAppears11()
        {
            return AppearsWhen(2, 11) ? 1 : 0;
        }

        [NWNEventHandler("dialog_appears_h")]
        public static int HeaderAppearsWhen()
        {
            return AppearsWhen(1, 0) ? 1 : 0;
        }

        [NWNEventHandler("dialog_appears_n")]
        public static int NextAppearsWhen()
        {
            return AppearsWhen(3, 12) ? 1 : 0;
        }

        [NWNEventHandler("dialog_action_n")]
        public static void NextAction()
        {
            ActionsTaken(12);
        }

        [NWNEventHandler("dialog_appears_p")]
        public static int PreviousAppearsWhen()
        {
            return AppearsWhen(4, 13) ? 1 : 0;
        }

        [NWNEventHandler("dialog_action_p")]
        public static void PreviousAction()
        {
            ActionsTaken(13);
        }

        [NWNEventHandler("dialog_appears_b")]
        public static int BackAppearsWhen()
        {
            return AppearsWhen(5, 14) ? 1 : 0;
        }

        [NWNEventHandler("dialog_action_b")]
        public static void BackAction()
        {
            ActionsTaken(14);
        }

        [NWNEventHandler("dialog_end")]
        public static void End()
        {
            var player = GetPCSpeaker();
            var playerId = GetObjectUUID(player);
            if (!HasPlayerDialog(playerId)) return;

            var dialog = LoadPlayerDialog(playerId);

            foreach (var endAction in dialog.EndActions)
            {
                endAction();
            }

            RemovePlayerDialog(playerId);
            DeleteLocalInt(player, "DIALOG_SYSTEM_INITIALIZE_RAN");

        }

        private static bool AppearsWhen(int nodeType, int nodeId)
        {
            var player = GetPCSpeaker();
            var playerId = GetObjectUUID(player);
            bool hasDialog = HasPlayerDialog(playerId);
            if (!hasDialog) return false;
            var dialog = LoadPlayerDialog(playerId);

            var page = dialog.CurrentPage;

            // The AppearsWhen call happens for every node.
            // We only want to load the header and responses time so ensure it only happens for the first node.
            if (nodeType == 1 && nodeId == 0)
            {
                page.Header = string.Empty;
                page.Responses.Clear();
                page.PageInit?.Invoke(page);
            }

            int currentSelectionNumber = nodeId + 1;
            bool displayNode = false;
            string newNodeText = string.Empty;
            int dialogOffset = (NumberOfResponsesPerPage + 1) * (dialog.DialogNumber - 1);

            if (currentSelectionNumber == NumberOfResponsesPerPage + 1) // Next page
            {
                int displayCount = page.NumberOfResponses - (NumberOfResponsesPerPage * dialog.PageOffset);

                if (displayCount > NumberOfResponsesPerPage)
                {
                    displayNode = true;
                }
            }
            else if (currentSelectionNumber == NumberOfResponsesPerPage + 2) // Previous Page
            {
                if (dialog.PageOffset > 0)
                {
                    displayNode = true;
                }
            }
            else if (currentSelectionNumber == NumberOfResponsesPerPage + 3) // Back
            {
                if (dialog.NavigationStack.Count > 0 && dialog.EnableBackButton)
                {
                    displayNode = true;
                }
            }
            else if (nodeType == 2)
            {
                int responseID = (dialog.PageOffset * NumberOfResponsesPerPage) + nodeId;
                if (responseID + 1 <= page.NumberOfResponses)
                {
                    var response = page.Responses[responseID];

                    if (response != null)
                    {
                        newNodeText = response.Text;
                        displayNode = response.IsActive;
                    }
                }
            }
            else if (nodeType == 1)
            {
                if (GetLocalInt(player, "DIALOG_SYSTEM_INITIALIZE_RAN") != 1)
                {
                    foreach (var initializationAction in dialog.InitializationActions)
                    {
                        initializationAction();
                    }

                    SetLocalInt(player, "DIALOG_SYSTEM_INITIALIZE_RAN", 1);
                }

                if (dialog.IsEnding)
                {
                    foreach (var endAction in dialog.EndActions)
                    {
                        endAction();
                    }
                    RemovePlayerDialog(playerId);
                    DeleteLocalInt(player, "DIALOG_SYSTEM_INITIALIZE_RAN");
                    return false;
                }

                page = dialog.CurrentPage;
                newNodeText = page.Header;

                SetCustomToken(90000 + dialogOffset, newNodeText);
                return true;
            }

            SetCustomToken(90001 + nodeId + dialogOffset, newNodeText);
            return displayNode;
        }

        private static void ActionsTaken(int nodeId)
        {
            var player = GetPCSpeaker();
            var playerId = GetObjectUUID(player);
            var dialog = LoadPlayerDialog(playerId);

            int selectionNumber = nodeId + 1;
            int responseID = nodeId + (NumberOfResponsesPerPage * dialog.PageOffset);

            if (selectionNumber == NumberOfResponsesPerPage + 1) // Next page
            {
                dialog.PageOffset = dialog.PageOffset + 1;
            }
            else if (selectionNumber == NumberOfResponsesPerPage + 2) // Previous page
            {
                dialog.PageOffset = dialog.PageOffset - 1;
            }
            else if (selectionNumber == NumberOfResponsesPerPage + 3) // Back
            {
                string currentPageName = dialog.CurrentPageName;
                var previous = dialog.NavigationStack.Pop();

                // This might be a little confusing but we're passing the active page as the "old page" to the Back() method.
                // This is because we need to run any dialog-specific clean up prior to moving the conversation backwards.
                foreach (var action in dialog.BackActions)
                {
                    action(currentPageName, previous.PageName);
                }


                // Previous page was in a different conversation. Switch to it.
                if (previous.DialogName != dialog.ActiveDialogName)
                {
                    LoadConversation(player, dialog.DialogTarget, previous.DialogName, dialog.DialogNumber);
                    dialog = LoadPlayerDialog(playerId);
                    dialog.ResetPage();

                    dialog.CurrentPageName = previous.PageName;
                    dialog.PageOffset = 0;
                    
                    foreach (var initializationAction in dialog.InitializationActions)
                    {
                        initializationAction();
                    }

                    SetLocalInt(player, "DIALOG_SYSTEM_INITIALIZE_RAN", 1);
                }
                // Otherwise it's in the same conversation. Switch to that.
                else
                {
                    dialog.CurrentPageName = previous.PageName;
                    dialog.PageOffset = 0;
                }
            }
            else if (selectionNumber != NumberOfResponsesPerPage + 4) // End
            {
                dialog.Pages[dialog.CurrentPageName].Responses[responseID].Action.Invoke();
            }
        }


        public static void StartConversation(uint player, uint talkTo, string @class)
        {
            var playerId = GetObjectUUID(player);
            LoadConversation(player, talkTo, @class, -1);
            var dialog = PlayerDialogs[playerId];

            // NPC conversations

            if (GetObjectType(talkTo) == ObjectType.Creature &&
                !GetIsPC(talkTo) &&
                !GetIsDM(talkTo))
            {
                BeginConversation("dialog" + dialog.DialogNumber, OBJECT_INVALID);
            }
            // Everything else
            else
            {
                AssignCommand(player, () => ActionStartConversation(talkTo, "dialog" + dialog.DialogNumber, true, false));
            }
        }


        public static bool HasPlayerDialog(string playerId)
        {
            return PlayerDialogs.ContainsKey(playerId);
        }

        public static PlayerDialog LoadPlayerDialog(string playerId)
        {
            if (!PlayerDialogs.ContainsKey(playerId)) throw new Exception(nameof(playerId) + " '" + playerId + "' could not be found. Be sure to call " + nameof(LoadConversation) + " first.");

            return PlayerDialogs[playerId];
        }

        public static void RemovePlayerDialog(string playerId)
        {
            var dialog = PlayerDialogs[playerId];
            DialogFilesInUse[dialog.DialogNumber] = false;

            PlayerDialogs.Remove(playerId);
        }

        public static void LoadConversation(uint player, uint talkTo, string @class, int dialogNumber)
        {
            if (string.IsNullOrWhiteSpace(@class)) throw new ArgumentException(nameof(@class), nameof(@class) + " cannot be null, empty, or whitespace.");
            if (dialogNumber != -1 && (dialogNumber < 1 || dialogNumber > NumberOfDialogs)) throw new ArgumentOutOfRangeException(nameof(dialogNumber), nameof(dialogNumber) + " must be between 1 and " + NumberOfDialogs);

            var convo = GetConversation(@class);
            var dialog = convo.SetUp(player);
            var playerId = GetObjectUUID(player);

            if (dialog == null)
            {
                throw new NullReferenceException(nameof(dialog) + " cannot be null.");
            }

            if (dialogNumber > 0)
                dialog.DialogNumber = dialogNumber;

            dialog.ActiveDialogName = @class;
            dialog.DialogTarget = talkTo;
            StorePlayerDialog(playerId, dialog);
        }

        private static void StorePlayerDialog(string globalID, PlayerDialog dialog)
        {
            if (dialog.DialogNumber <= 0)
            {
                for (int x = 1; x <= NumberOfDialogs; x++)
                {
                    var existingDialog = PlayerDialogs.SingleOrDefault(d => d.Value.DialogNumber == x);
                    if (!DialogFilesInUse[x] || existingDialog.Value == null)
                    {
                        DialogFilesInUse[x] = true;
                        dialog.DialogNumber = x;
                        break;
                    }
                }
            }

            // Couldn't find an open dialog file. Throw error.
            if (dialog.DialogNumber <= 0)
            {
                Console.WriteLine("ERROR: Unable to locate a free dialog. Add more dialog files, update their custom tokens, and update Dialog.cs");
                return;
            }

            PlayerDialogs[globalID] = dialog;
        }


        public static void EndConversation(uint player)
        {
            var playerId = GetObjectUUID(player);
            PlayerDialog playerDialog = LoadPlayerDialog(playerId);
            playerDialog.IsEnding = true;
            StorePlayerDialog(playerId, playerDialog);
        }

    }
}
