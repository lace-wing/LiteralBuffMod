using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace LiteralBuffMod.Common
{
    public class LiteralBuffPlayer : ModPlayer
    {
        public int plrActiveTime = 0;
        public int buffUpdateTime = 0;

        public Vector2[] plrVelocity = new Vector2[16];
        public Vector2[] plrAcceleration = new Vector2[16];
        public float plrMass = 64;

        public bool onGround;
        public Vector2 nonGroundVelocity;

        public bool swiftnessSlipping;
        public float slippery;
        public int slippingIndex;

        public bool longerPotionSickness;

        public bool gillDrown;

        public override void ResetEffects()
        {
            if (plrActiveTime >= int.MaxValue || plrActiveTime < 0)
            {
                plrActiveTime = 0;
            }
            plrActiveTime++;

            if (Player.CountBuffs() == 0 || buffUpdateTime < 0 || buffUpdateTime >= int.MaxValue)
            {
                buffUpdateTime = 0;
            }

            plrMass = 64;

            #region Check if onGround
            onGround = false;
            for (int i = 0; i < (int)(Player.width / 16f) + 2; i++)
            {
                int x = (int)(Player.position.X / 16f);
                int y = (int)((Player.position.Y + Player.height) / 16f);
                if (Main.tile[x + i, y] != null)
                {
                    Tile tileUnderPlr = Main.tile[x + i, y];
                    Vector2 tilePos = new Vector2((x + i) * 16, y * 16);
                    if (tileUnderPlr.HasTile && !tileUnderPlr.IsActuated && Player.position.Y + Player.height >= (tileUnderPlr.IsHalfBlock ? tilePos.Y + 8 : tilePos.Y))
                    {
                        onGround = true;
                    }
                }
            }
            #endregion

            swiftnessSlipping = false;
            slippery = 0;
            slippingIndex = 0;

            longerPotionSickness = false;

            gillDrown = false;
        }

        public override void PostUpdate()
        {
            #region Set plrVelocity & plrAcceleration
            for (int i = plrVelocity.Length - 1; i > 0; i--)
            {
                plrVelocity[i] = plrVelocity[i - 1];
            }
            plrVelocity[0] = Player.velocity;
            for (int j = 1; j < plrAcceleration.Length; j++)
            {
                plrAcceleration[j] = plrAcceleration[j - 1];
            }
            plrAcceleration[0] = plrVelocity[0] - plrVelocity[1];
            #endregion

            if (swiftnessSlipping && onGround && plrAcceleration[0] != Vector2.Zero && Player.dashDelay != -1) // 敏捷打滑
            {
                if (!onGround)
                {
                    nonGroundVelocity = Player.velocity;
                }

                if (nonGroundVelocity != Vector2.Zero)// && Player.velocity.LengthSquared() < nonGroundVelocity.LengthSquared())
                {
                    Player.velocity.X += nonGroundVelocity.X * 0.05f;
                    nonGroundVelocity = Vector2.Zero;
                }

                Player.velocity.X = MathHelper.Lerp(plrVelocity[0].X, plrVelocity[slippingIndex].X, Math.Clamp(plrMass * slippery / 128f, 0, 1));
            }
        }
        public override void PostUpdateBuffs()
        {
            if (Player.HasBuff(BuffID.Swiftness)) //
            {
                swiftnessSlipping = true;
                slippery = 0.8f;
                slippingIndex = 3;
                if (Math.Abs(plrVelocity[0].X) <= 0.01f)
                {
                    slippingIndex = 0;
                }
                if (plrVelocity[0].LengthSquared() < plrVelocity[1].LengthSquared())
                {
                    slippingIndex = 7;
                }
                Player.moveSpeed += 0.2f;
            }
            if (Player.HasBuff(BuffID.Regeneration)) // 耐药性更长
            {
                longerPotionSickness = true;
            }
            if (Player.HasBuff(BuffID.Ironskin)) // 铁皮减速减攻速加防御
            {
                Player.moveSpeed -= 0.1f;
                Player.GetAttackSpeed(DamageClass.Melee) -= 0.05f;
                Player.statDefense += 4;
            }
            //TODO 脚蹼饰品
            if (Player.HasBuff(BuffID.Flipper)) // 脚蹼在地上减速
            {
                if (!Player.wet && !Player.honeyWet && !Player.lavaWet && onGround)
                {
                    Player.moveSpeed *= 0.66f;
                    Player.runAcceleration *= 0.8f;
                    Player.runSlowdown *= 1.2f;
                }
            }
            if (Player.gills) // 在空气中窒息
            {
                if (!Player.wet && !Player.HasBuff(BuffID.Wet))
                {
                    if (Player.breath >= -1 && plrActiveTime % 3 == 0)
                    {
                        Player.breath -= 4;
                    }
                    if (Player.breath < 0)
                    {
                        gillDrown = true;
                        if (plrActiveTime % 6 == 0)
                        {
                            Player.statLife--;
                        }
                    }
                }
            }
        }

        public override void UpdateBadLifeRegen()
        {
            if (gillDrown)
            {
                Player.lifeRegenTime = 0;
            }
        }
    }
}
