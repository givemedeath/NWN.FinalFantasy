using System.Collections.Generic;
using NWN.FinalFantasy.Core.NWScript.Enum;
using NWN.FinalFantasy.Core.NWScript.Enum.VisualEffect;
using NWN.FinalFantasy.Enumeration;
using NWN.FinalFantasy.Service;
using NWN.FinalFantasy.Service.AbilityService;
using NWN.FinalFantasy.Core.NWNX;
using static NWN.FinalFantasy.Core.NWScript.NWScript;
using NWN.FinalFantasy.Core;

namespace NWN.FinalFantasy.Feature.AbilityDefinition
{
    public class ArmShotAbilityDefinition : IAbilityListDefinition
    {
        private const string ArmShotVariableName = "ARM_SHOT";
        private const string ArmShotActivatorVariableName = "ARM_SHOT_ACTIVATOR";
        private const float duration = 6.0f;

        public Dictionary<Feat, AbilityDetail> BuildAbilities()
        {
            var builder = new AbilityBuilder();
            ArmShot(builder);

            return builder.Build();
        }

        private static void ArmShot(AbilityBuilder builder)
        {
            builder.Create(Feat.ArmShot, PerkType.ArmShot)
                .Name("Arm Shot")
                .HasRecastDelay(RecastGroup.ArmShot, 12.0f)
                .RequirementStamina(10)
                .UsesActivationType(AbilityActivationType.Weapon)
                .HasImpactAction((activator, target, level) =>
                {
                    SetLocalBool(target, ArmShotVariableName, true);
                    SetLocalObject(target, ArmShotActivatorVariableName, activator);

                    DelayCommand(duration, () => { DeleteLocalBool(activator, ArmShotVariableName); });
                    DelayCommand(duration, () => { DeleteLocalObject(activator, ArmShotActivatorVariableName); });                    
                });
        }

        /// <summary>
        /// When damage is applied, if the target has a Life Steal variable set,
        /// that percent of the damage dealt is restored on the activator.
        /// </summary>
        [NWNEventHandler("on_nwnx_dmg")]
        public static void ApplyArmShot()
        {
            var target = OBJECT_SELF;
            var damageDetails = Damage.GetDamageEventData();

            if (!GetLocalBool(damageDetails.Damager, ArmShotVariableName)) return;
            if (damageDetails.Total <= 0.0f) return;

            ApplyEffectToObject(DurationType.Temporary, EffectAttackDecrease(-5), target, duration);         
            ApplyEffectToObject(DurationType.Temporary, EffectVisualEffect(VisualEffect.Vfx_Dur_Glow_Light_Red), target, duration);

            Enmity.ModifyEnmity(damageDetails.Damager, target, 4);
            CombatPoint.AddCombatPoint(damageDetails.Damager, target, SkillType.Marksmanship, 2);

            DeleteLocalBool(damageDetails.Damager, ArmShotVariableName);
            DeleteLocalObject(damageDetails.Damager, ArmShotActivatorVariableName);
        }
    }
}