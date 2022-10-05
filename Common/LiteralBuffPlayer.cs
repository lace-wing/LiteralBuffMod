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
        public Vector2[] pVelocity = new Vector2[16];
        public Vector2[] pAcceleration = new Vector2[16];
        public float pInertia = 16;

        public int buffUpdateTime;

        public bool swiftnessSlipping;
        public bool longerPotionSickness;
        public bool canBeAttractedByMagnet;
        public bool equippedMagnet;
        public int polarity;
        public override void ResetEffects()
        {
            pInertia = 16;

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
            for (int i = 1; i < pVelocity.Length - 1; i++)
            {
                pVelocity[i] = pVelocity[i - 1];
            }
            pVelocity[0] = Player.velocity;
            for (int j = 1; j < pAcceleration.Length - 1; j++)
            {
                pAcceleration[j] = pAcceleration[j - 1];
            }
            pAcceleration[0] = pVelocity[0] - pVelocity[1];

            if (swiftnessSlipping)
            {
                Player.velocity -= pAcceleration[0] * pInertia * 0.01f;
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
                    if (player != Player)
                    {
                        magVelocity += LiteralPhysicsUtil.ElectromagneticForce2(Player, player);
                        /*LiteralBuffPlayer otherLBPlr = player.GetModPlayer<LiteralBuffPlayer>();
                        float distance = Vector2.Distance(Player.Center, player.Center);
                        if (distance <= 3600)
                        {
                            float force = 0;
                            float forceMult = 0;
                            int forceDir = polarity * otherLBPlr.polarity;

                            if (equippedMagnet)
                            {
                                force += 8;
                                forceMult += 0.5f;
                            }
                            if (canBeAttractedByMagnet)
                            {
                                force += 8;
                            }
                            if (otherLBPlr.equippedMagnet)
                            {
                                force += 8;
                                forceMult += 0.5f;
                            }
                            if (otherLBPlr.canBeAttractedByMagnet)
                            {
                                force += 8;
                            }

                            force *= 3600 - distance / 3600f; 

                            Player.velocity += (Player.Center - player.Center).SafeNormalize(Vector2.Zero) * forceDir * force * forceMult;
                        }*/
                    }
                }

                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.type == ProjectileID.MagnetSphereBall)
                    {
                        magVelocity += LiteralPhysicsUtil.ElectromagneticForce2(Player, proj);
                    }
                }

                Player.velocity += magVelocity;
            }
        }
    }
}
