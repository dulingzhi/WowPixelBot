﻿using Libs.GOAP;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Libs.Actions
{
    public class EatOrBandageAction : GoapAction
    {
        private readonly WowProcess wowProcess;
        private readonly PlayerReader playerReader;
        private readonly StopMoving stopMoving;
        private ILogger logger;

        public EatOrBandageAction(WowProcess wowProcess, PlayerReader playerReader, StopMoving stopMoving, ILogger logger)
        {
            this.wowProcess = wowProcess;
            this.playerReader = playerReader;
            this.stopMoving = stopMoving;
            this.logger = logger;

            AddPrecondition(GoapKey.incombat, false);
            AddPrecondition(GoapKey.shouldheal, true);
        }

        public override float CostOfPerformingAction { get => 4f; }

        private ConsoleKey Eat => playerReader.PlayerClass switch
        {
            PlayerClassEnum.Warrior => this.playerReader.ActionBarUseable_73To96.HotKey7 ? ConsoleKey.D7 : ConsoleKey.Escape,
            PlayerClassEnum.Rogue => this.playerReader.ActionBarUseable_1To24.HotKey7 ? ConsoleKey.D7 : ConsoleKey.Escape,
            _ => ConsoleKey.Escape
        };

        private ConsoleKey Bandage => playerReader.PlayerClass switch
        {
            PlayerClassEnum.Warrior => this.playerReader.ActionBarUseable_73To96.HotKey8 ? ConsoleKey.D8 : ConsoleKey.Escape,
            _ => ConsoleKey.Escape
        };

        public override async Task PerformAction()
        {
            await stopMoving.Stop();

            if ((this.playerReader.HealthPercent < 40|| Bandage== ConsoleKey.Escape) && Eat != ConsoleKey.Escape)
            {
                await PressKeyAndWait(Eat, 27);
                await wowProcess.KeyPress(ConsoleKey.Spacebar, 500);
            }
            else
            {
                await PressKeyAndWait(Bandage, 13);
            }
        }

        private async Task PressKeyAndWait(ConsoleKey key, int seconds)
        {
            if (key != ConsoleKey.Escape)
            {
                await wowProcess.KeyPress(key, 500);
            }

            for (int i = 0; i < seconds; i++)
            {
                if (this.playerReader.HealthPercent > 98 || this.playerReader.PlayerBitValues.PlayerInCombat)
                {
                    break;
                }
                await Task.Delay(1000);
            }
        }
    }
}
