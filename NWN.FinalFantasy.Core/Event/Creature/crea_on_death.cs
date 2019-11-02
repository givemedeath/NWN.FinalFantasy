﻿using NWN.FinalFantasy.Core;
using NWN.FinalFantasy.Core.Event.Creature;
using static NWN._;

// ReSharper disable once CheckNamespace
namespace NWN.Scripts
{
    public class crea_on_death
    {
        internal static void Main()
        {
            ExecuteScript("nw_c2_default7", NWGameObject.OBJECT_SELF);
            ScriptRunner.RunScriptEvents(NWGameObject.OBJECT_SELF, CreaturePrefix.OnDeath);
        }
    }
}