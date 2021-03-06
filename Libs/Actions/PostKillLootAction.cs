﻿using Libs.GOAP;
using Libs.Looting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Libs.Actions
{
    public class PostKillLootAction : LootAction
    {
        public PostKillLootAction(WowProcess wowProcess, PlayerReader playerReader, BagReader bagReader, StopMoving stopMoving, ILogger logger) 
            : base(wowProcess, playerReader, bagReader, stopMoving, logger)
        {
        }

        protected override void AddPreconditions()
        {
            AddPrecondition(GoapKey.incombat, false);
            AddPrecondition(GoapKey.hastarget, false);
            AddPrecondition(GoapKey.shouldloot, true);
        }

        public override float CostOfPerformingAction { get => 5f; }

        public override async Task PerformAction()
        {
            await Task.Delay(1000);
            await base.PerformAction();
        }
    }
}