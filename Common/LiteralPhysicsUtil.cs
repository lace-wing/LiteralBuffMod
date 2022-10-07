using Terraria.ID;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteralBuffMod.Common
{
    public static class LiteralPhysicsUtil
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
            }
            else if (entity is Projectile)
            {
                Projectile projectile = (Projectile)entity;
                LiteralGlobalProj lbProj = projectile.GetGlobalProjectile<LiteralGlobalProj>();
                mass = lbProj.projMass;
            }
            else if (entity is Item)
            {
                Item item = (Item)entity;
                LiteralGlobalItem lbItem = item.GetGlobalItem<LiteralGlobalItem>();
                mass = lbItem.itemMass;
            }
            else if (entity is NPC)
            {
                NPC npc = (NPC)entity;
                LiteralGlobalNPC lbNPC = npc.GetGlobalNPC<LiteralGlobalNPC>();
                mass = lbNPC.npcMass;
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
            if (!entity1.active || !entity2.active)
            {
                return Vector2.Zero;
            }

            float rangeFactor = 1800;
            float rangeSqd = 0;
            bool canBeAttractedByMagnet1 = false;
            bool canBeAttractedByMagnet2 = false;
            bool hasMagnetism1 = false;
            bool hasMagnetism2 = false;
            int polarity1 = 0;
            int polarity2 = 0;
            float mfs1 = 0;
            float mfs2 = 0;
            int forceDir = 0;
            float distanceSqd = Vector2.DistanceSquared(entity1.Center, entity2.Center);
            float force = 0;
            float forceMult = 0;

            if (entity1 is Player)
            {
                Player player1 = (Player)entity1;
                LiteralBuffPlayer lbPlr1 = player1.GetModPlayer<LiteralBuffPlayer>();
                canBeAttractedByMagnet1 = lbPlr1.canBeAttractedByMagnet;
                hasMagnetism1 = lbPlr1.equippedMagnet;
                polarity1 = lbPlr1.polarity;
                mfs1 = lbPlr1.plrMFS;
            }
            else if (entity1 is Projectile)
            {
                Projectile projectile1 = (Projectile)entity1;
                LiteralGlobalProj lbProj1 = projectile1.GetGlobalProjectile<LiteralGlobalProj>();
                canBeAttractedByMagnet1 = lbProj1.canBeAttractedByMagnet;
                hasMagnetism1 = lbProj1.hasMagnetism;
                polarity1 = lbProj1.polarity;
                mfs1 = lbProj1.projMFS;
            }
            else if (entity1 is Item)
            {
                Item item1 = (Item)entity1;
                LiteralGlobalItem lbItem1 = item1.GetGlobalItem<LiteralGlobalItem>();
                canBeAttractedByMagnet1 = lbItem1.canBeAttractedByMagnet;
                hasMagnetism1 = lbItem1.hasMagnetism;
                polarity1 = lbItem1.polarity;
                mfs1 = lbItem1.itemMFS;
            }
            else if (entity1 is NPC)
            {
                NPC npc1 = (NPC)entity1;
                LiteralGlobalNPC lbNPC1 = npc1.GetGlobalNPC<LiteralGlobalNPC>();
                canBeAttractedByMagnet1 = lbNPC1.canBeAttractedByMagnet;
                hasMagnetism1 = lbNPC1.hasMagnetism;
                polarity1 = lbNPC1.polarity;
                mfs1 = lbNPC1.npcMFS;
            }
            else return Vector2.Zero;

            if (entity2 is Player)
            {
                Player player2 = (Player)entity2;
                LiteralBuffPlayer lbPlr2 = player2.GetModPlayer<LiteralBuffPlayer>();
                canBeAttractedByMagnet2 = lbPlr2.canBeAttractedByMagnet;
                hasMagnetism2 = lbPlr2.equippedMagnet;
                polarity2 = lbPlr2.polarity;
                mfs2 = lbPlr2.plrMFS;
            }
            else if (entity2 is Projectile)
            {
                Projectile projectile2 = (Projectile)entity2;
                LiteralGlobalProj lbProj2 = projectile2.GetGlobalProjectile<LiteralGlobalProj>();
                canBeAttractedByMagnet2 = lbProj2.canBeAttractedByMagnet;
                hasMagnetism2 = lbProj2.hasMagnetism;
                polarity2 = lbProj2.polarity;
                mfs2 = lbProj2.projMFS;
            }
            else if (entity2 is Item)
            {
                Item item2 = (Item)entity2;
                LiteralGlobalItem lbItem2 = item2.GetGlobalItem<LiteralGlobalItem>();
                canBeAttractedByMagnet2 = lbItem2.canBeAttractedByMagnet;
                hasMagnetism2 = lbItem2.hasMagnetism;
                polarity2 = lbItem2.polarity;
                mfs2 = lbItem2.itemMFS;
            }
            else if (entity2 is NPC)
            {
                NPC npc2 = (NPC)entity2;
                LiteralGlobalNPC lbNPC2 = npc2.GetGlobalNPC<LiteralGlobalNPC>();
                canBeAttractedByMagnet2 = lbNPC2.canBeAttractedByMagnet;
                hasMagnetism2 = lbNPC2.hasMagnetism;
                polarity2 = lbNPC2.polarity;
                mfs2 = lbNPC2.npcMFS;
            }
            else return Vector2.Zero;

            if (!hasMagnetism1 && !hasMagnetism2)
            {
                return Vector2.Zero;
            }

            rangeSqd = MathF.Max((MathF.Pow(mfs1 < 0 ? 0 : mfs1, 2) + MathF.Pow(mfs2 < 0 ? 0 : mfs2, 2)) * rangeFactor, 0.01f);

            forceDir = polarity1 * polarity2;
            forceDir = forceDir == 0 ? -1 : forceDir;

            if (distanceSqd <= rangeSqd)
            {

                if (hasMagnetism1)
                {
                    force += mfs1;
                    forceMult += 0.4f;
                }
                if (canBeAttractedByMagnet1)
                {
                    force += mfs1 * 0.5f;
                    forceMult += 0.2f;
                }
                if (hasMagnetism2)
                {
                    force += mfs2;
                    forceMult += 0.4f;
                }
                if (canBeAttractedByMagnet2)
                {
                    force += mfs2 * 0.5f;
                    forceMult += 0.2f;
                }

                force *= (rangeSqd - distanceSqd) / rangeSqd;
            }

            return (entity1.Center - entity2.Center).SafeNormalize(Vector2.Zero) * forceDir * force * forceMult;
        }

        public static void MagnetSphereTransformProj(Projectile msProj, Projectile targetProj)
        {
            if (msProj.type == ProjectileID.MagnetSphereBall && !targetProj.friendly && Collision.CheckAABBvAABBCollision(msProj.position, msProj.Size, targetProj.position, targetProj.Size))
            {
                targetProj.hostile = false;
                targetProj.GetGlobalProjectile<LiteralGlobalProj>().hasMagnetism = true;
                targetProj.GetGlobalProjectile<LiteralGlobalProj>().projMFS += 1;
                msProj.damage += (int)(targetProj.damage * 0.02f);
                msProj.CritChance += 1;
                msProj.timeLeft += msProj.damage > 600 ? -3 : msProj.damage > 300 ? 0 : msProj.damage > 150 ? 1 : 2;
            }
        }
    }
}
