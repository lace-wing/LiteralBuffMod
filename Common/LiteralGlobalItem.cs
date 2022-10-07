using Terraria.ID;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.TypeConversion;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace LiteralBuffMod.Common
{
    public class LiteralGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public Vector2[] itemVelocity = new Vector2[16];
        public Vector2[] itemAcceleration = new Vector2[16];
        public float itemMass = 4;
        public float itemMFS = 0;

        public bool canBeAttractedByMagnet;
        public bool hasMagnetism;
        public int polarity = 0;
        public bool magnefiedByMagnetFlower;

        internal void UpdateResetItemEffect(Item item)
        {
            itemMass = 4;

            itemMFS = 0;
            canBeAttractedByMagnet = false;
            hasMagnetism = false;
            polarity = 0;
            magnefiedByMagnetFlower = false;

        }

        internal void SetItemMagProperty(Item item)
        {
            if (item.type == ItemID.MagnetFlower || item.type == ItemID.MagnetSphere || item.type == ItemID.CelestialMagnet || item.type == ItemID.TreasureMagnet)
            {
                itemMFS = 8;
                canBeAttractedByMagnet = true;
                hasMagnetism = true;
            }
        }

        internal void CheckIfItemIsMagnetic(Item item)
        {
            if ((item == null || !item.active) && LiteralPhysicsSystem.magneticEntity.Contains(item))
            {
                LiteralPhysicsSystem.magneticEntity.Remove(item);
            }

            if (!LiteralPhysicsSystem.magneticEntity.Contains(item) && (canBeAttractedByMagnet || hasMagnetism))
            {
                LiteralPhysicsSystem.magneticEntity.Add(item);
            }
            else if (LiteralPhysicsSystem.magneticEntity.Contains(item) && !canBeAttractedByMagnet && !hasMagnetism)
            {
                LiteralPhysicsSystem.magneticEntity.Remove(item);
            }
        }
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

            UpdateResetItemEffect(item);

            SetItemMagProperty(item);

            CheckIfItemIsMagnetic(item);
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            UpdateResetItemEffect(item);

            SetItemMagProperty(item);

            LiteralPhysicsSystem.magneticEntity.Remove(item);

            LiteralBuffPlayer lbPlr = player.GetModPlayer<LiteralBuffPlayer>();
            int overallPolarity = 0;

            if (item.type == ItemID.MagnetFlower || item.type == ItemID.CelestialMagnet || item.type == ItemID.TreasureMagnet)
            {
                lbPlr.equippedMagnet = true;
                overallPolarity += polarity;
                lbPlr.plrMFS += 4;
            }

            if (item.type == ItemID.MagnetFlower)
            {
                lbPlr.equippedMagnetFlower = true;
                lbPlr.equippedMagnet = false;
            }

            lbPlr.polarity += Math.Clamp(overallPolarity, -1, 1);
        }

        public override void UpdateInventory(Item item, Player player)
        {
            UpdateResetItemEffect(item);
            
            SetItemMagProperty(item);
            
            LiteralPhysicsSystem.magneticEntity.Remove(item);
        }

        public override bool OnPickup(Item item, Player player)
        {
            LiteralPhysicsSystem.magneticEntity.Remove(item);
            return base.OnPickup(item, player);
        }
    }
}
