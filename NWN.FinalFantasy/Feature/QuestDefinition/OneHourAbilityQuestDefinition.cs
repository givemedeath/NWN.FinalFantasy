﻿using System.Collections.Generic;
using NWN.FinalFantasy.Enumeration;
using NWN.FinalFantasy.Service.QuestService;

namespace NWN.FinalFantasy.Feature.QuestDefinition
{
    public class OneHourAbilityQuestDefinition : IQuestListDefinition
    {
        public Dictionary<string, QuestDetail> BuildQuests()
        {
            var builder = new QuestBuilder();

            AKnightsTest(builder);
            AMonksTest(builder);
            AThiefsTest(builder);
            ABlackMagesTest(builder);
            AWhiteMagesTest(builder);
            ARedMagesTest(builder);
            ARangersTest(builder);
            ANinjasTest(builder);
            ASpecialistsTest(builder);
            ASnipersTest(builder);
            ADarkKnightsTest(builder);

            return builder.Build();
        }

        private static void AKnightsTest(QuestBuilder builder)
        {
            builder.Create("a_knights_test", "A Knight's Test", "a_knights_test");
        }

        private static void AMonksTest(QuestBuilder builder)
        {
            builder.Create("a_monks_test", "A Monk's Test", "a_monks_test");
        }

        private static void AThiefsTest(QuestBuilder builder)
        {
            builder.Create("a_thiefs_test", "A Thief's Test", "a_thiefs_test");
        }

        private static void ABlackMagesTest(QuestBuilder builder)
        {
            builder.Create("a_black_mages_test", "A Black Mage's Test", "a_black_mages_test");
        }

        private static void AWhiteMagesTest(QuestBuilder builder)
        {
            builder.Create("a_white_mages_test", "A White Mage's Test", "a_white_mages_test");
        }

        private static void ARedMagesTest(QuestBuilder builder)
        {
            builder.Create("a_red_mages_test", "A Red Mage's Test", "a_red_mages_test");
        }

        private static void ARangersTest(QuestBuilder builder)
        {
            builder.Create("a_rangers_test", "A Ranger's Test", "a_rangers_test");
        }

        private static void ANinjasTest(QuestBuilder builder)
        {
            builder.Create("a_ninjas_test", "A Ninja's Test", "a_ninjas_test");
        }

        private static void ASpecialistsTest(QuestBuilder builder)
        {
            builder.Create("a_specialists_test", "A Specialist's Test", "a_specialists_test");
        }

        private static void ASnipersTest(QuestBuilder builder)
        {
            builder.Create("a_snipers_test", "A Sniper's Test", "a_snipers_test");
        }

        private static void ADarkKnightsTest(QuestBuilder builder)
        {
            builder.Create("a_dark_knights_test", "A Dark Knight's Test", "a_dark_knights_test");
        }
    }
}