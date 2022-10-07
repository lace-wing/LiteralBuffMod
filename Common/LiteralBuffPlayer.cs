using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace LiteralBuffMod.Common
{
    public class LiteralBuffPlayer : ModPlayer
    {
        public Vector2[] plrVelocity = new Vector2[16];
        public Vector2[] plrAcceleration = new Vector2[16];
        public float plrMass = 16;
        public int polarity;
        public float plrMFS = 0;

        public int buffUpdateTime;

        public bool swiftnessSlipping;
        public bool longerPotionSickness;
        public bool canBeAttractedByMagnet;
        public bool equippedMagnet;

        public override void ResetEffects()
        {
            plrMass = 16;

            if (Player.CountBuffs() == 0 || buffUpdateTime < 0 || buffUpdateTime >= int.MaxValue - 1)
            {
                buffUpdateTime = 0;
            }

            swiftnessSlipping = false;
            longerPotionSickness = false;
            canBeAttractedByMagnet = false;
            equippedMagnet = false;
            polarity = 0;
        }
        public override void PostUpdate()
        {
            #region Set plrVelocity & plrAcceleration
            for (int i = 1; i < plrVelocity.Length - 1; i++)
            {
                plrVelocity[i] = plrVelocity[i - 1];
            }
            plrVelocity[0] = Player.velocity;
            for (int j = 1; j < plrAcceleration.Length - 1; j++)
            {
                plrAcceleration[j] = plrAcceleration[j - 1];
            }
            plrAcceleration[0] = plrVelocity[0] - plrVelocity[1];
            #endregion

            if (swiftnessSlipping)
            {
                Player.velocity -= plrAcceleration[0] * plrMass * 0.01f;
            }
        }
        public override void PostUpdateBuffs()
        {
            if (Player.HasBuff(BuffID.Swiftness))
            {
                swiftnessSlipping = true;
            }
            if (Player.HasBuff(BuffID.Regeneration))
            {
                longerPotionSickness = true;
            }
            if (Player.HasBuff(BuffID.Ironskin))
            {
                canBeAttractedByMagnet = true;
                plrMFS = 4;
                Player.moveSpeed -= 0.1f;
                Player.GetAttackSpeed(DamageClass.Melee) -= 0.05f;
                Player.statDefense += 4;
            }
        }
        public override void PostUpdateEquips()
        {
            if (Player.Male)
            {
                polarity = 1;
            }
            else polarity = -1;

            if (equippedMagnet || canBeAttractedByMagnet)
            {
                Vector2 magVelocity = Vector2.Zero;

                foreach (Player player in Main.player)
                {
                    if (player != Player && player.active)
                    {
                        magVelocity += LiteralPhysicsUtil.ForceToVelocity(Player, LiteralPhysicsUtil.ElectromagneticForce2(Player, player), player.position, player.Size);
                    }
                }

                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.type == ProjectileID.MagnetSphereBall)
                    {
                        magVelocity += LiteralPhysicsUtil.ForceToVelocity(Player, LiteralPhysicsUtil.ElectromagneticForce2(Player, proj), proj.position, proj.Size);
                    }
                }

                if (Main.GameUpdateCount % 60 == 0)
                    Main.NewText($"Player: {Player.velocity} {magVelocity}");
                Player.velocity += magVelocity;
            }
        }
    }
}
