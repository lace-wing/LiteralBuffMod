using Terraria.ID;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.TypeConversion;
using Microsoft.Xna.Framework;

namespace LiteralBuffMod.Common
{
    public class LiteralGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public Vector2[] itemVelocity = new Vector2[16];
        public Vector2[] itemAcceleration = new Vector2[16];
        public float itemMass = 4;
        public int polarity;
        public float itemMFS = 0;

        public bool canBeAttractedByMagnet;
        public bool isMagnetic;

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            #region Set itemVelocity & itemAcceleration
            for (int i = 1; i < itemVelocity.Length - 1; i++)
            {
                itemVelocity[i] = itemVelocity[i - 1];
            }
            itemVelocity[0] = item.velocity;
            for (int j = 1; j < itemAcceleration.Length - 1; j++)
            {
                itemAcceleration[j] = itemAcceleration[j - 1];
            }
            itemAcceleration[0] = itemVelocity[0] - itemVelocity[1];
            #endregion
        }
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
