﻿using NWN.FinalFantasy.Core;
using NWN.FinalFantasy.Core.Event.Placeable;

// ReSharper disable once CheckNamespace
namespace NWN.Scripts
{
    public class plc_on_splcast
    {
        internal static void Main()
        {
            ScriptRunner.RunScriptEvents(NWGameObject.OBJECT_SELF, PlaceablePrefix.OnSpellCastAt);
        }
    }
}