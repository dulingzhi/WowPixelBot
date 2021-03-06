﻿using Libs.GOAP;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Libs.Actions
{
    public class BuffAction : GoapAction
    {
        private readonly WowProcess wowProcess;
        private readonly PlayerReader playerReader;
        private readonly StopMoving stopMoving;
        private ILogger logger;

        private DateTime LastBuffed = DateTime.Now.AddDays(-1);

        public BuffAction(WowProcess wowProcess, PlayerReader playerReader, StopMoving stopMoving, ILogger logger)
        {
            this.wowProcess = wowProcess;
            this.playerReader = playerReader;
            this.stopMoving = stopMoving;
            this.logger = logger;

            AddPrecondition(GoapKey.incombat, false);
        }

        public override float CostOfPerformingAction { get => 1f; }

        public override async Task PerformAction()
        {
            await this.stopMoving.Stop();

            if (playerReader.PlayerBitValues.IsMounted)
            {
                await wowProcess.Dismount();
            }

            await Task.Delay(1000);

            await wowProcess.KeyPress(ConsoleKey.F1, 500);

            for (int i = 0; i < 7; i++)
            {
                await Task.Delay(1000);
                if (playerReader.PlayerBitValues.PlayerInCombat) { return; }
            }

            await wowProcess.KeyPress(ConsoleKey.F2, 500);

            for (int i = 0; i < 7; i++)
            {
                await Task.Delay(1000);
                if (playerReader.PlayerBitValues.PlayerInCombat) { return; }
            }

            LastBuffed = DateTime.Now;
        }

        public override bool CheckIfActionCanRun()
        {
            return (DateTime.Now - LastBuffed).TotalMinutes > 31;
        }

        public override string Description()
        {
            if (!CheckIfActionCanRun())
            {
                var timespan = LastBuffed.AddMinutes(31) - DateTime.Now;
                return " - F1/F2 - "+ DateTime.Now.Date.AddSeconds(timespan.TotalSeconds).ToString("mm:ss");
            }
            else
            {
                return " - F1/F2 - Pending";
            }
        }
    }
}
