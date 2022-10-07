using Terraria.ID;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace LiteralBuffMod.Common
{
    public class LiteralGlobalProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public Vector2[] projVelocity = new Vector2[16];
        public Vector2[] projAcceleration = new Vector2[16];
        public float projMass = 4;

        public bool canBeAttractedByMagnet = false;
        public bool hasMagnetism = false;
        public int polarity = 0;
        public float projMFS = 0;
        public bool magnefiedByMagnetFlower;

        public void PostAIResetProjEffect(Projectile projectile)
        {
            projMass = 4;

            canBeAttractedByMagnet = false;
            hasMagnetism = false;
            polarity = 0;
            projMFS = 0;
            magnefiedByMagnetFlower = false;
        }

        internal void SetProjMagProperty(Projectile proj)
        {
            foreach (Player player in Main.player)
            {
                if (player.active)
                {
                    LiteralBuffPlayer lbPlr = player.GetModPlayer<LiteralBuffPlayer>();
                    if (Vector2.DistanceSquared(proj.Center, player.Center) <= 129600 && lbPlr.equippedMagnetFlower)
                    {
                        magnefiedByMagnetFlower = true;
                    }
                }
            }

            if (proj.type == ProjectileID.MagnetSphereBall)
            {
                canBeAttractedByMagnet = true;
                hasMagnetism = true;
                projMFS = 8;
                projMass = 32;
            }

            if (magnefiedByMagnetFlower)
            {
                canBeAttractedByMagnet = true;
                projMFS -= 1;
            }
        }

        internal void CheckIfProjIsMagnetic(Projectile proj)
        {
            if ((proj == null || !proj.active) && LiteralPhysicsSystem.magneticEntity.Contains(proj))
            {
                LiteralPhysicsSystem.magneticEntity.Remove(proj);
            }

            if (!LiteralPhysicsSystem.magneticEntity.Contains(proj) && (canBeAttractedByMagnet || hasMagnetism))
            {
                LiteralPhysicsSystem.magneticEntity.Add(proj);
            }
            if (LiteralPhysicsSystem.magneticEntity.Contains(proj) && !canBeAttractedByMagnet && !hasMagnetism)
            {
                LiteralPhysicsSystem.magneticEntity.Remove(proj);
            }
        }

        public override void PostAI(Projectile projectile)
        {
            #region Set projVelocity & projAcceleration
            for (int i = 1; i < projVelocity.Length - 1; i++)
            {
                projVelocity[i] = projVelocity[i - 1];
            }
            projVelocity[0] = projectile.velocity;
            for (int j = 1; j < projAcceleration.Length - 1; j++)
            {
                projAcceleration[j] = projAcceleration[j - 1];
            }
            projAcceleration[0] = projVelocity[0] - projVelocity[1];
            #endregion

            PostAIResetProjEffect(projectile);

            SetProjMagProperty(projectile);

            CheckIfProjIsMagnetic(projectile);
        }
    }
}
