﻿using System;
using System.Collections.Generic;
using System.Linq;
using NWN.FinalFantasy.Core.NWNX;
using NWN.FinalFantasy.Entity;
using NWN.FinalFantasy.Feature.DialogDefinition;
using static NWN.FinalFantasy.Core.NWScript.NWScript;
using Player = NWN.FinalFantasy.Entity.Player;

namespace NWN.FinalFantasy.Service.QuestService
{
    public delegate void AcceptQuestDelegate(uint player, uint questSourceObject);
    public delegate void AdvanceQuestDelegate(uint player, uint questSourceObject, int questState);
    public delegate void CompleteQuestDelegate(uint player, uint questSourceObject);

    public class QuestDetail
    {
        public string QuestId { get; set; }
        public string Name { get; set; }
        public string JournalTag { get; set; }
        public bool IsRepeatable { get; set; }
        public bool AllowRewardSelection { get; set; }

        public List<IQuestReward> Rewards { get; } = new List<IQuestReward>();
        public List<IQuestPrerequisite> Prerequisites { get; } = new List<IQuestPrerequisite>();

        public Dictionary<int, QuestStateDetail> States { get; } = new Dictionary<int, QuestStateDetail>();
        public List<AcceptQuestDelegate> OnAcceptActions { get; } = new List<AcceptQuestDelegate>();
        public List<AdvanceQuestDelegate> OnAdvanceActions { get; } = new List<AdvanceQuestDelegate>();
        public List<CompleteQuestDelegate> OnCompleteActions { get; } = new List<CompleteQuestDelegate>();

        /// <summary>
        /// Adds a quest state to this quest.
        /// </summary>
        /// <returns>The newly created quest state.</returns>
        protected QuestStateDetail AddState()
        {
            int index = States.Count;
            States[index] = new QuestStateDetail();
            return States[index];
        }

        /// <summary>
        /// Retrieves a state by its index.
        /// </summary>
        /// <param name="state">The index of the state.</param>
        /// <returns>The quest state at a specified index</returns>
        protected QuestStateDetail GetState(int state)
        {
            return States[state];
        }

        /// <summary>
        /// Retrieves the list of quest states ordered by their sequence.
        /// </summary>
        /// <returns>A list of quest states ordered by their sequence</returns>
        protected Dictionary<int, QuestStateDetail> GetStates()
        {
            return States.OrderBy(o => o.Key).ToDictionary(x => x.Key, y => y.Value);
        }

        /// <summary>
        /// Returns true if player can accept this quest. Returns false otherwise.
        /// </summary>
        /// <param name="player">The player to check</param>
        /// <returns>true if player can accept, false otherwise</returns>
        private bool CanAccept(uint player)
        {
            // Retrieve the player's current quest status for this quest.
            // If they haven't accepted it yet, this will be null.
            var playerId = GetObjectUUID(player);
            var dbPlayer = DB.Get<Player>(playerId);
            var quest = dbPlayer.Quests.ContainsKey(playerId) ? dbPlayer.Quests[QuestId] : null;

            // If the status is null, it's assumed that the player hasn't accepted it yet.
            if (quest != null)
            {
                // If the quest isn't repeatable, prevent the player from accepting it after it's already been completed.
                if (quest.TimesCompleted > 0)
                {
                    // If it's repeatable, then we don't care if they've already completed it.
                    if (!IsRepeatable)
                    {
                        SendMessageToPC(player, "You have already completed this quest.");
                        return false;
                    }
                }
                // If the player already accepted the quest, prevent them from accepting it again.
                else
                {
                    SendMessageToPC(player, "You have already accepted this quest.");
                    return false;
                }
            }

            // Check whether the player meets all necessary prerequisites.
            foreach (var prereq in Prerequisites)
            {
                if (!prereq.MeetsPrerequisite(player))
                {
                    SendMessageToPC(player, "You do not meet the prerequisites necessary to accept this quest.");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns true if player can complete this quest. Returns false otherwise.
        /// </summary>
        /// <param name="player">The player to check</param>
        /// <returns>true if player can complete, false otherwise</returns>
        private bool CanComplete(uint player)
        {
            // Has the player even accepted this quest?
            var playerId = GetObjectUUID(player);
            var dbPlayer = DB.Get<Player>(playerId);
            var quest = dbPlayer.Quests.ContainsKey(QuestId) ? dbPlayer.Quests[QuestId] : null;

            if (quest == null) return false;

            // Is the player on the final state of this quest?
            if (quest.CurrentState != GetStates().Count()) return false;

            var state = GetState(quest.CurrentState);
            // Are all objectives complete?
            foreach (var objective in state.GetObjectives())
            {
                if (!objective.IsComplete(player, QuestId))
                {
                    return false;
                }
            }

            // Met all requirements. We can complete this quest.
            return true;
        }

        /// <summary>
        /// Opens the reward selection menu wherein players can select the reward they want.
        /// If quest is not configured to allow reward selection, quest will be marked complete instead
        /// and all rewards will be given to the player.
        /// </summary>
        /// <param name="player">The player to request a reward from</param>
        /// <param name="questSource">The source of the quest reward giver</param>
        private void RequestRewardSelectionFromPC(uint player, uint questSource)
        {
            if (!GetIsPC(player) || GetIsDM(player)) return;

            if (AllowRewardSelection)
            {
                SetLocalString(player, "QST_REWARD_SELECTION_QUEST_ID", QuestId);
                Dialog.StartConversation(player, player, nameof(QuestRewardSelectionDialog));
            }
            else
            {
                Complete(player, questSource, null);
            }
        }

        /// <summary>
        /// Returns the rewards given for completing this quest.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IQuestReward> GetRewards()
        {
            return Rewards;
        }

        /// <summary>
        /// Gives all rewards for this quest to the player.
        /// </summary>
        /// <param name="player">The player receiving the rewards.</param>
        public void GiveRewards(uint player)
        {
            foreach (var reward in Rewards)
            {
                reward.GiveReward(player);
            }
        }

        /// <summary>
        /// Accepts a quest using the configured settings.
        /// </summary>
        /// <param name="player">The player accepting the quest.</param>
        /// <param name="questSource">The source of the quest giver</param>
        public void Accept(uint player, uint questSource)
        {
            if (!GetIsPC(player) || GetIsDM(player)) return;

            if (!CanAccept(player))
            {
                return;
            }

            // By this point, it's assumed the player will accept the quest.
            var playerId = GetObjectUUID(player);
            var dbPlayer = DB.Get<Player>(playerId);
            var quest = dbPlayer.Quests.ContainsKey(QuestId) ? dbPlayer.Quests[QuestId] : new PlayerQuest();

            // Retrieve the first quest state for this quest.
            quest.CurrentState = 1;
            quest.DateLastCompleted = null;
            dbPlayer.Quests[QuestId] = quest;
            DB.Set(playerId, dbPlayer);

            var state = GetState(1);
            foreach (var objective in state.GetObjectives())
            {
                objective.Initialize(player, QuestId);
            }

            // Add the journal entry to the player.
            AddJournalQuestEntry(JournalTag, 1, player, false);

            // Notify them that they've accepted a quest.
            SendMessageToPC(player, "Quest '" + Name + "' accepted. Refer to your journal for more information on this quest.");

            // Run any quest-specific code.
            foreach (var action in OnAcceptActions)
            {
                action.Invoke(player, questSource);
            }
        }

        /// <summary>
        /// Advances the player to the next quest state.
        /// </summary>
        /// <param name="player">The player advancing to the next quest state</param>
        /// <param name="questSource">The source of quest advancement</param>
        public void Advance(uint player, uint questSource)
        {
            if (!GetIsPC(player) || GetIsDM(player)) return;

            // Retrieve the player's current quest state.
            var playerId = GetObjectUUID(player);
            var dbPlayer = DB.Get<Player>(playerId);
            var questStatus = dbPlayer.Quests.ContainsKey(QuestId) ? dbPlayer.Quests[QuestId] : new PlayerQuest();

            // Can't find a state? Notify the player they haven't accepted the quest.
            if (questStatus.CurrentState <= 0)
            {
                SendMessageToPC(player, "You have not accepted this quest yet.");
                return;
            }

            // If this quest has already been completed, exit early.
            // This is used in case a module builder incorrectly configures a quest.
            // We don't want to risk giving duplicate rewards.
            if (questStatus.TimesCompleted > 0 && !IsRepeatable) return;

            var currentState = GetState(questStatus.CurrentState);

            // Check quest objectives. If not complete, exit early.
            foreach (var objective in currentState.GetObjectives())
            {
                if (!objective.IsComplete(player, QuestId))
                    return;
            }

            var lastState = GetStates().Last();

            // If this is the last state, the assumption is that it's time to complete the quest.
            if (questStatus.CurrentState == lastState.Key)
            {
                RequestRewardSelectionFromPC(player, questSource);
            }
            else
            {
                // Progress player's quest status to the next state.
                questStatus.CurrentState++;
                var nextState = GetState(questStatus.CurrentState);

                // Update the player's journal
                AddJournalQuestEntry(JournalTag, questStatus.CurrentState, player, false);

                // Notify the player they've progressed.
                SendMessageToPC(player, "Objective for quest '" + Name + "' complete! Check your journal for information on the next objective.");

                // Save changes
                dbPlayer.Quests[QuestId] = questStatus;
                DB.Set(playerId, dbPlayer);

                // Create any extended data entries for the next state of the quest.
                foreach (var objective in nextState.GetObjectives())
                {
                    objective.Initialize(player, QuestId);
                }

                // Run any quest-specific code.
                foreach (var action in OnAdvanceActions)
                {
                    action.Invoke(player, questSource, questStatus.CurrentState);
                }
            }

        }

        /// <summary>
        /// Completes a quest for a player. If a reward is selected, that reward will be given to the player.
        /// Otherwise, all rewards configured for this quest will be given to the player.
        /// </summary>
        /// <param name="player">The player completing the quest.</param>
        /// <param name="questSource">The source of the quest completion</param>
        /// <param name="selectedReward">The reward selected by the player</param>
        public void Complete(uint player, uint questSource, IQuestReward selectedReward)
        {
            if (!GetIsPC(player) || GetIsDM(player)) return;
            if (!CanComplete(player)) return;

            var playerId = GetObjectUUID(player);
            var dbPlayer = DB.Get<Player>(playerId);
            var quest = dbPlayer.Quests.ContainsKey(QuestId) ? dbPlayer.Quests[QuestId] : new PlayerQuest();

            // Mark player as being on the last state of the quest.
            quest.CurrentState = GetStates().Count();
            quest.TimesCompleted++;

            // No selected reward, simply give all available rewards to the player.
            if (selectedReward == null)
            {
                foreach (var reward in Rewards)
                {
                    reward.GiveReward(player);
                }
            }
            // There is a selected reward. Give that reward and any rewards which are not selectable to the player.
            else
            {
                // Non-selectable rewards (gold, GP, etc) are granted to the player.
                foreach (var reward in Rewards.Where(x => !x.IsSelectable))
                {
                    reward.GiveReward(player);
                }

                selectedReward.GiveReward(player);
            }

            quest.ItemProgresses.Clear();
            quest.KillProgresses.Clear();
            quest.DateLastCompleted = DateTime.UtcNow;
            dbPlayer.Quests[QuestId] = quest;
            DB.Set(playerId, dbPlayer);

            foreach (var action in OnCompleteActions)
            {
                action.Invoke(player, questSource);
            }

            SendMessageToPC(player, "Quest '" + Name + "' complete!");
            RemoveJournalQuestEntry(JournalTag, player, false);

            Events.SignalEvent("FFO_COMPLETE_QUEST", player);
        }
    }
}
