﻿using NWN.FinalFantasy.Core.NWScript.Enumerations;
using NWN.FinalFantasy.Job.Enumeration;

namespace NWN.FinalFantasy.Job.AbilityDefinition.Protection
{
    internal class ProvokeMastery: IAbilityDefinition
    {
        public string Name => "Provoke Mastery";
        public Feat Feat => Feat.ProvokeMastery1;
        public AbilityCategory Category => AbilityCategory.Trait;
        public AbilityGroup Group => AbilityGroup.Individual;
        public bool IsEquippable => false;
        public int APRequired => 100;

        public JobLevel[] JobRequirements => new[]
        {
            new JobLevel(ClassType.Warrior, 16), 
        };

        public int MP(NWGameObject user)
        {
            return 0;
        }

        public string CanUse(NWGameObject user, NWGameObject target)
        {
            return null;
        }

        public float CastingTime(NWGameObject user)
        {
            return 0;
        }

        public float CooldownTime(NWGameObject user)
        {
            return 0;
        }

        public void Apply(NWGameObject user)
        {
        }

        public void Impact(NWGameObject user, NWGameObject target)
        {
        }
    }
}