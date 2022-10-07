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
        public int polarity = 0;
        public float projMFS = 0;

        public bool canBeAttractedByMagnet = false;
        public bool isMagnetic = false;

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

            if (projectile.type == ProjectileID.MagnetSphereBall)
            {
                Vector2 magVelocity = Vector2.Zero;

                foreach (Player player in Main.player)
                {
                    if (Vector2.Distance(projectile.Center, player.Center) <= 3600)
                    {
                        magVelocity += LiteralPhysicsUtil.ForceToVelocity(projectile, LiteralPhysicsUtil.ElectromagneticForce2(projectile, player), player.position, player.Size);
                    }
                }

                if (Main.GameUpdateCount % 60 == 0)
                    Main.NewText($"Proj: {magVelocity}");
                projectile.velocity += magVelocity;

            }
        }
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.type == ProjectileID.MagnetSphereBall)
            {
                canBeAttractedByMagnet = true;
                isMagnetic = true;
                projMFS = 16;
                polarity = Main.dayTime ? 1 : -1;
            }
        }
    }
}
