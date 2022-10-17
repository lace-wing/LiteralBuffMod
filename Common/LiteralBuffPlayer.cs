using IL.Terraria.Utilities;
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
        /// <summary>
        /// Use as a general timer for LiteralBuffPlayer
        /// </summary>
        public int plrActiveTime = 0;
        /// <summary>
        /// A timer which works when player has buff(s)
        /// </summary>
        public int buffUpdateTime = 0;

        /// <summary>
        /// Records player's velocity in the past 16 ticks, only records the current velocity when dashing
        /// </summary>
        public Vector2[] plrVelocity = new Vector2[16];
        /// <summary>
        /// Records player's acceleration in the past 16 ticks, calculated using player's velocity
        /// </summary>
        public Vector2[] plrAcceleration = new Vector2[16];
        /// <summary>
        /// Whether the player is decelerating in X direction
        /// </summary>
        public bool xDecelerating;
        /// <summary>
        /// Whether the player is decelerating in Y direction
        /// </summary>
        public bool yDecelerating;
        /// <summary>
        /// Player's 'mass'
        /// </summary>
        public float plrMass = 64;

        /// <summary>
        /// How 'watter' the player is
        /// </summary>
        public int wetTimer;
        /// <summary>
        /// How 'honey' the player is
        /// </summary>
        public int honeyWetTimer;

        /// <summary>
        /// Whether the player is on the ground, not very accurate
        /// </summary>
        public bool onGround;
        /// <summary>
        /// Check using this timer allows the player to shortly leave the ground
        /// </summary>
        public int onGroundTimer;

        /// <summary>
        /// Should the player slip due to the Swiftness buff
        /// </summary>
        public bool swiftnessSlipping;
        /// <summary>
        /// Should the player slip due to wetness
        /// </summary>
        public bool wetSlipping;
        /// <summary>
        /// Should the player slip due to 'honeyness'
        /// </summary>
        public bool honeyWetSlipping;
        /// <summary>
        /// Should the player slip due to the Wet debuff
        /// </summary>
        public bool wetBuffSlipping;

        /// <summary>
        /// How slippery the player is
        /// </summary>
        public float slipperiness;
        /// <summary>
        /// Index of the velocity in plrVelocity which the player approaches to
        /// </summary>
        public int slippingIndex;

        /// <summary>
        /// Whether the Potion Sickness debuff on the player ticks in a different way, which shorter initial durations take an advantage
        /// </summary>
        public bool recoverPotionSickness;

        /// <summary>
        /// Whether the player should drown in air
        /// </summary>
        public bool gillDrown;
        /// <summary>
        /// Whether the player is drowning in air
        /// </summary>
        public bool airDrown;

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
            if (Player.CountBuffs() > 0)
            {
                buffUpdateTime++;
            }

            xDecelerating = false;
            plrMass = 64;

            #region Set various wet timers
            if (Player.wet)
            {
                wetTimer += 6;
            }
            else wetTimer--;
            wetTimer = Math.Clamp(wetTimer, -120, 1200);
            if (Player.honeyWet)
            {
                honeyWetTimer += 12;
                wetTimer -= 4;
            }
            else honeyWetTimer--;
            honeyWetTimer = Math.Clamp(honeyWetTimer, -180, 1800);
            #endregion

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
                        onGroundTimer += 6;
                        onGroundTimer = Math.Clamp(onGroundTimer, 0, 120);
                    }
                    else onGroundTimer--;
                }
            }
            #endregion

            swiftnessSlipping = false;
            wetSlipping = false;
            honeyWetSlipping = false;
            wetBuffSlipping = false;

            slipperiness = 0;
            slippingIndex = 0;

            recoverPotionSickness = false;

            airDrown = false;
            gillDrown = false;
        }

        public override void PostUpdate()
        {
            #region Set plrVelocity & plrAcceleration
            for (int i = plrVelocity.Length - 1; i > 0; i--) // 记录16帧的速度
            {
                plrVelocity[i] = plrVelocity[i - 1];
            }
            plrVelocity[0] = Player.velocity;
            for (int j = 1; j < plrAcceleration.Length; j++)
            {
                plrAcceleration[j] = plrAcceleration[j - 1];
            }
            plrAcceleration[0] = plrVelocity[0] - plrVelocity[1];

            if (Math.Sign(plrAcceleration[0].X) != Math.Sign(plrVelocity[0].X)) // x与y轴上是否减速
            {
                xDecelerating = true;
            }
            if (Math.Sign(plrAcceleration[0].Y) != Math.Sign(plrVelocity[0].Y))
            {
                yDecelerating = true;
            }

            if (Player.dashDelay == -1) // 避免冲刺后打滑导致鬼畜
            {
                for (int i = 1; i < plrVelocity.Length; i++)
                {
                    plrVelocity[i] = plrVelocity[0];
                }
            }
            #endregion

            if (wetTimer > 0)
            {
                wetSlipping = true;
                if (wetSlipping && !wetBuffSlipping && onGroundTimer > 0) //TODO Allow players to disable this effect
                {
                    slipperiness += 0.8f * wetTimer / 1200f;
                    slippingIndex += 3;
                    if (xDecelerating)
                    {
                        slippingIndex += 3;
                    }
                    Player.maxRunSpeed *= 1.33f;
                    if (Main.rand.NextBool(3))
                    {
                        Dust.NewDust(Player.position, Player.width, Player.height, DustID.Water, Scale: 3f * wetTimer / 1200f);
                    }
                }
            }
            if (honeyWetTimer > 0)
            {
                honeyWetSlipping = true;
                if (honeyWetSlipping && onGroundTimer > 0) //TODO Allow players to disable this effect
                {
                    slipperiness += 0.6f * honeyWetTimer / 1800f;
                    slippingIndex += 1;
                    if (xDecelerating)
                    {
                        slippingIndex -= 2;
                    }
                    else slippingIndex += 2;
                    if (Main.rand.NextBool(2))
                    {
                        Dust.NewDust(Player.position, Player.width, Player.height, DustID.Honey, Scale: 3f * wetTimer / 1800f);
                    }
                }
            }

            slippingIndex = Math.Clamp(slippingIndex, 0, 15); // 防超界
            if (Player.dashDelay != -1) // 打滑
            {
                Player.velocity.X = MathHelper.Lerp(plrVelocity[0].X, plrVelocity[slippingIndex].X, Math.Clamp(plrMass * slipperiness / 128f, 0, 0.98f));
            }
        }

        public override void PreUpdateBuffs()
        {
            if (Player.HasBuff(BuffID.Regeneration)) // 再生耐药性更长
            {
                recoverPotionSickness = true;
            }

            if (Player.HasBuff(BuffID.PotionSickness) && recoverPotionSickness) // 耐药性更长
            {
                if (buffUpdateTime % 20 == 0)
                {
                    int psIndex = Player.FindBuffIndex(BuffID.PotionSickness);
                    int psTime = Player.buffTime[psIndex];
                    Player.buffTime[psIndex] += Math.Min((int)(Math.Pow(1.8, psTime / 1200f) * 5.4f - 12f), 19);
                }
            }
        }

        public override void PostUpdateBuffs()
        {
            if (Player.HasBuff(BuffID.Swiftness)) // 敏捷加速但是设置打滑属性
            {
                swiftnessSlipping = true;
                if (swiftnessSlipping && onGroundTimer > 0) //TODO Allow players to disable this effect
                {
                    slipperiness += 0.4f;
                    slippingIndex += 3;
                    if (Math.Abs(plrVelocity[0].X) <= 0.01f)
                    {
                        slippingIndex -= 3;
                    }
                    if (xDecelerating)
                    {
                        slippingIndex += 3;
                    }
                    Player.moveSpeed += 0.33f;
                    Player.maxRunSpeed *= 1.66f;
                }
            }
            if (Player.HasBuff(BuffID.Wet)) // 潮湿打滑
            {
                wetBuffSlipping = true;
                if (wetBuffSlipping && onGroundTimer > 0) //TODO Allow players to disable this effect
                {
                    slipperiness += 0.8f;
                    slippingIndex += 3;
                    if (xDecelerating)
                    {
                        slippingIndex += 3;
                    }
                    Player.maxRunSpeed *= 1.33f;
                }
            }
            if (Player.HasBuff(BuffID.Slimed)) // 粘液打滑减速 
            {
                slipperiness += 0.4f;
                slippingIndex += 2;
                if (!xDecelerating)
                {
                    slippingIndex += 3;
                }
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
            if (Player.gills)
            {
                gillDrown = true; //TODO Allow players to disable this effect
                if (gillDrown) // 在空气中溺水
                {
                    Main.NewText($"gill: {gillDrown} drown: {airDrown} breath: {Player.breath} wet: {Player.wet} water: {wetTimer} wet buff: {Player.HasBuff(BuffID.Wet)}");
                    if (!Player.wet && wetTimer < 360 && !Player.HasBuff(BuffID.Wet))
                    {
                        if (Player.breath >= 0)
                        {
                            Player.breath -= plrActiveTime % 7 == 0 ? 4 : 3;
                        }
                        if (Player.breath <= 0)
                        {
                            Player.breath = 0;
                            airDrown = true;
                            //Player.statLife -= 2;
                            //if (Player.statLife <= 0)
                            //{
                            //    Player.statLife = 0;
                            //    Player.KillMe(PlayerDeathReason.ByOther(1), 10.0, 0);
                            //}
                        }
                    }
                }
            }
        }

        public override void UpdateBadLifeRegen()
        {
            if (airDrown) // 鱼鳃溺水掉血
            {
                if (Player.lifeRegen > 0)
                {
                    Player.lifeRegen = 0;
                }
                Player.lifeRegenTime = 0;
                Player.lifeRegen -= Player.HasBuff(BuffID.Honey) ? 10 : 8;
                if (Player.statLife <= 1)
                {
                    Player.statLife = 0;
                    Player.KillMe(PlayerDeathReason.ByOther(1), 10.0, 0);
                }
            }
            if (Player.HasBuff(BuffID.Regeneration)) // 再生额外回血
            {
                Player.lifeRegen += 2;
            }
        }
    }
}
