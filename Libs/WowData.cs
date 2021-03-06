﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Libs
{
    public class WowData
    {
        public List<DataFrame> frames { get; private set; } = new List<DataFrame>();
        public IAddonReader AddonReader { get; private set; }
        private readonly ISquareReader squareReader;
        public PlayerReader PlayerReader { get; private set; }
        public BagReader bagReader { get; private set; }
        public EquipmentReader equipmentReader { get; private set; }
        public bool Active { get; set; } = true;

        public event EventHandler? AddonDataChanged;

        public WowData(IColorReader colorReader, List<DataFrame> frames, ILogger logger)
        {
            this.frames = frames;

            var width = frames.Last().point.X + 1;
            var height = frames.Max(f => f.point.Y) + 1;
            this.AddonReader = new AddonReader(colorReader, frames, width, height, logger);

            this.squareReader = new SquareReader(AddonReader);

            this.bagReader = new BagReader(squareReader, 20);
            this.equipmentReader = new EquipmentReader(squareReader, 30);
            this.PlayerReader = new PlayerReader(squareReader, logger);

            this.AddonReader.PlayerReader = this.PlayerReader;
        }

        private int seq = 0;

        public void AddonRefresh()
        {
            AddonReader.Refresh();

            // 20 - 29
            var bagItems = bagReader.Read();

            // 30 - 31
            var equipment = equipmentReader.Read();

            //logger.LogInformation($"X: {PlayerReader.XCoord.ToString("0.00")}, Y: {PlayerReader.YCoord.ToString("0.00")}, Direction: {PlayerReader.Direction.ToString("0.00")}, Zone: {PlayerReader.Zone}, Gold: {PlayerReader.Gold}");

            //logger.LogInformation($"Enabled: {PlayerReader.ActionBarEnabledAction.value}, NotEnoughMana: {PlayerReader.ActionBarNotEnoughMana.value}, NotOnCooldown: {PlayerReader.ActionBarNotOnCooldown.value}, Charge: {PlayerReader.SpellInRange.Charge}, Rend: {PlayerReader.SpellInRange.Rend}, Shoot gun: {PlayerReader.SpellInRange.ShootGun}");
            seq++;

            if (seq >= 10)
            {
                seq = 0;
                AddonDataChanged?.Invoke(AddonReader, new EventArgs());
            }
            System.Threading.Thread.Sleep(10);
        }
    }
}