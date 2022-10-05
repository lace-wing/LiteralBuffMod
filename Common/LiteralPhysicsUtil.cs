using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteralBuffMod.Common
{
    internal static class LiteralPhysicsUtil
    {
        public static Vector2 ElectromagneticForce2(Entity entity1, Entity entity2)
        {
            bool canBeAttractedByMagnet1 = false;
            bool canBeAttractedByMagnet2 = false;
            bool isMagnetic1 = false;
            bool isMagnetic2 = false;
            int polarity1 = 0;
            int polarity2 = 0;

            float distance = Vector2.Distance(entity1.Center, entity2.Center);
            float force = 0;
            float forceMult = 0;

            if (entity1 is Player)
            {
                Player player1 = (Player)entity1;
                LiteralBuffPlayer lbPlr1 = player1.GetModPlayer<LiteralBuffPlayer>();
                canBeAttractedByMagnet1 = lbPlr1.canBeAttractedByMagnet;
                isMagnetic1 = lbPlr1.equippedMagnet;
                polarity1 = lbPlr1.polarity;
            }
            if (entity1 is Projectile)
            {
                Projectile projectile1 = (Projectile)entity1;
                LiteralGlobalProj lbProj1 = projectile1.GetGlobalProjectile<LiteralGlobalProj>();
                canBeAttractedByMagnet1 = lbProj1.canBeAttractedByMagnet;
                isMagnetic1 = lbProj1.isMagnetic;
                polarity1 = lbProj1.polarity;
            }
            if (entity1 is Item)
            {
                Item item1 = (Item)entity1;
                LiteralGlobalItem lbItem1 = item1.GetGlobalItem<LiteralGlobalItem>();
                canBeAttractedByMagnet1 = lbItem1.canBeAttractedByMagnet;
                isMagnetic1 = lbItem1.isMagnetic;
                polarity1 = lbItem1.polarity;
            }
            else return Vector2.Zero;

            if (entity2 is Player)
            {
                Player player2 = (Player)entity2;
                LiteralBuffPlayer lbPlr2 = player2.GetModPlayer<LiteralBuffPlayer>();
                canBeAttractedByMagnet2 = lbPlr2.canBeAttractedByMagnet;
                isMagnetic2 = lbPlr2.equippedMagnet;
                polarity2 = lbPlr2.polarity;
            }
            if (entity2 is Projectile)
            {
                Projectile projectile2 = (Projectile)entity2;
                LiteralGlobalProj lbProj2 = projectile2.GetGlobalProjectile<LiteralGlobalProj>();
                canBeAttractedByMagnet2 = lbProj2.canBeAttractedByMagnet;
                isMagnetic2 = lbProj2.isMagnetic;
                polarity2 = lbProj2.polarity;
            }
            if (entity2 is Item)
            {
                Item item2 = (Item)entity2;
                LiteralGlobalItem lbItem2 = item2.GetGlobalItem<LiteralGlobalItem>();
                canBeAttractedByMagnet2 = lbItem2.canBeAttractedByMagnet;
                isMagnetic2 = lbItem2.isMagnetic;
                polarity2 = lbItem2.polarity;
            }
            else return Vector2.Zero;

            int forceDir = polarity1 * polarity2;

            if (distance <= 3600)
            {

                if (isMagnetic1)
                {
                    force += 8;
                    forceMult += 0.5f;
                }
                if (canBeAttractedByMagnet1)
                {
                    force += 8;
                }
                if (isMagnetic2)
                {
                    force += 8;
                    forceMult += 0.5f;
                }
                if (canBeAttractedByMagnet2)
                {
                    force += 8;
                }

                force *= 3600 - distance / 3600f;

                return (entity1.Center - entity2.Center).SafeNormalize(Vector2.Zero) * forceDir * force * forceMult;
            }

            return Vector2.Zero;
        }
    }
}
