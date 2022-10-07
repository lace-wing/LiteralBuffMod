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
        public static Vector2 ForceToVelocity(Entity entity, Vector2 force, Vector2 targetPosition = default, Vector2 targetRectangle = default)
        {
            if (force == Vector2.Zero || !entity.active)
            {
                return Vector2.Zero;
            }

            float mass = 0;
            Vector2[] histVelocity = new Vector2[] { };
            Vector2[] histAcceleration = new Vector2[] { };
            Vector2 velocity = Vector2.Zero;
            bool shouldStop = false;

            if (entity is Player)
            {
                Player player = (Player)entity;
                LiteralBuffPlayer lbPlr = player.GetModPlayer<LiteralBuffPlayer>();
                mass = lbPlr.plrMass;
                histVelocity = lbPlr.plrVelocity;
                histAcceleration = lbPlr.plrAcceleration;
            }
            else if (entity is Projectile)
            {
                Projectile projectile = (Projectile)entity;
                LiteralGlobalProj lbProj = projectile.GetGlobalProjectile<LiteralGlobalProj>();
                mass = lbProj.projMass;
                histVelocity = lbProj.projVelocity;
                histAcceleration = lbProj.projAcceleration;
            }
            else if (entity is Item)
            {
                Item item = (Item)entity;
                LiteralGlobalItem lbItem = item.GetGlobalItem<LiteralGlobalItem>();
                mass = lbItem.itemMass;
                histVelocity = lbItem.itemVelocity;
                histAcceleration = lbItem.itemAcceleration;
            }

            if (Collision.CheckAABBvAABBCollision(entity.position, entity.Size, targetPosition, targetRectangle))
            {
                shouldStop = true;
            }

            velocity = force / mass;

            if (shouldStop)
            {
                velocity *= 0;
            }

            return velocity;
        }
        public static Vector2 ElectromagneticForce2(Entity entity1, Entity entity2)
        {
            bool canBeAttractedByMagnet1 = false;
            bool canBeAttractedByMagnet2 = false;
            bool isMagnetic1 = false;
            bool isMagnetic2 = false;
            int polarity1 = 0;
            int polarity2 = 0;
            float mfs1 = 0;
            float mfs2 = 0;

            float distance = Vector2.Distance(entity1.Center, entity2.Center);
            float force = 0;
            float forceMult = 0;

            if (entity1 is Player && entity1.active)
            {
                Player player1 = (Player)entity1;
                LiteralBuffPlayer lbPlr1 = player1.GetModPlayer<LiteralBuffPlayer>();
                canBeAttractedByMagnet1 = lbPlr1.canBeAttractedByMagnet;
                isMagnetic1 = lbPlr1.equippedMagnet;
                polarity1 = lbPlr1.polarity;
                mfs1 = lbPlr1.plrMFS;
            }
            else if (entity1 is Projectile && entity1.active)
            {
                Projectile projectile1 = (Projectile)entity1;
                LiteralGlobalProj lbProj1 = projectile1.GetGlobalProjectile<LiteralGlobalProj>();
                canBeAttractedByMagnet1 = lbProj1.canBeAttractedByMagnet;
                isMagnetic1 = lbProj1.isMagnetic;
                polarity1 = lbProj1.polarity;
                mfs1 = lbProj1.projMFS;
            }
            else if (entity1 is Item && entity1.active)
            {
                Item item1 = (Item)entity1;
                LiteralGlobalItem lbItem1 = item1.GetGlobalItem<LiteralGlobalItem>();
                canBeAttractedByMagnet1 = lbItem1.canBeAttractedByMagnet;
                isMagnetic1 = lbItem1.isMagnetic;
                polarity1 = lbItem1.polarity;
                mfs1 = lbItem1.itemMFS;
            }
            else return Vector2.Zero;

            if (entity2 is Player && entity2.active)
            {
                Player player2 = (Player)entity2;
                LiteralBuffPlayer lbPlr2 = player2.GetModPlayer<LiteralBuffPlayer>();
                canBeAttractedByMagnet2 = lbPlr2.canBeAttractedByMagnet;
                isMagnetic2 = lbPlr2.equippedMagnet;
                polarity2 = lbPlr2.polarity;
                mfs2 = lbPlr2.plrMFS;
            }
            else if (entity2 is Projectile && entity2.active)
            {
                Projectile projectile2 = (Projectile)entity2;
                LiteralGlobalProj lbProj2 = projectile2.GetGlobalProjectile<LiteralGlobalProj>();
                canBeAttractedByMagnet2 = lbProj2.canBeAttractedByMagnet;
                isMagnetic2 = lbProj2.isMagnetic;
                polarity2 = lbProj2.polarity;
                mfs2 = lbProj2.projMFS;
            }
            else if (entity2 is Item && entity2.active)
            {
                Item item2 = (Item)entity2;
                LiteralGlobalItem lbItem2 = item2.GetGlobalItem<LiteralGlobalItem>();
                canBeAttractedByMagnet2 = lbItem2.canBeAttractedByMagnet;
                isMagnetic2 = lbItem2.isMagnetic;
                polarity2 = lbItem2.polarity;
                mfs2 = lbItem2.itemMFS;
            }
            else return Vector2.Zero;

            if (!isMagnetic1 && !isMagnetic2)
            {
                return Vector2.Zero;
            }

            int forceDir = polarity1 * polarity2;

            if (distance <= 3600)
            {

                if (isMagnetic1)
                {
                    force += mfs1;
                    forceMult += 0.4f;
                }
                if (canBeAttractedByMagnet1)
                {
                    forceMult += 0.2f;
                }
                if (isMagnetic2)
                {
                    force += mfs2;
                    forceMult += 0.4f;
                }
                if (canBeAttractedByMagnet2)
                {
                    forceMult += 0.2f;
                }

                force *= (3600f - distance) / 3600f;// * 0.2f;

                return (entity1.Center - entity2.Center).SafeNormalize(Vector2.Zero) * forceDir * force * forceMult;
            }

            return Vector2.Zero;
        }
    }
}
