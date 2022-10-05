using Terraria.ID;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.TypeConversion;

namespace LiteralBuffMod.Common
{
    public class LiteralGlobalItem : GlobalItem
    {
        public bool canBeAttractedByMagnet;
        public bool isMagnetic;
        public int polarity;
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            LiteralBuffPlayer lbPlr = player.GetModPlayer<LiteralBuffPlayer>();

            if (item.type == ItemID.MagnetFlower || item.type == ItemID.CelestialMagnet || item.type == ItemID.TreasureMagnet)
            {
                lbPlr.equippedMagnet = true;
            }
        }
    }
}
