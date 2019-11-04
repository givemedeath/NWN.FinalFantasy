﻿using NWN.FinalFantasy.Core.NWNX;
using static NWN._;

namespace NWN.FinalFantasy.Auditing.DM
{
    public class OnToggleLock : DMAudit
    {
        public static void Main()
        {
            var dm = NWGameObject.OBJECT_SELF;
            var target = NWNXEvents.OnDMPossess_GetTarget();
            var targetName = GetName(target);

            WriteLog(dm, "Toggle Lock", $"Target: {targetName}");
        }
    }
}