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

        public bool canBeAttractedByMagnet = false;
        public bool isMagnetic = false;
        public int polarity = 0;
        public override void PostAI(Projectile projectile)
        {
            if (projectile.type == ProjectileID.MagnetSphereBall)
            {
                Vector2 magVelocity = Vector2.Zero;

                foreach (Player player in Main.player)
                {
                    if (Vector2.Distance(projectile.Center, player.Center) <= 3600)
                    {
                        magVelocity += LiteralPhysicsUtil.ElectromagneticForce2(projectile, player);
                    }
                }

                projectile.velocity += magVelocity;

            }
        }
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.type == ProjectileID.MagnetSphereBall)
            {
                canBeAttractedByMagnet = true;
                isMagnetic = true;
                polarity = Main.dayTime ? 1 : -1;
            }
        }
    }
}
