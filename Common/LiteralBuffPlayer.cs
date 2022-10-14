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
        public Vector2[] plrVelocity = new Vector2[16];
        public Vector2[] plrAcceleration = new Vector2[16];
        public float plrMass = 16;

        public int buffUpdateTime;

        public bool swiftnessSlipping;
        public bool longerPotionSickness;

        public override void ResetEffects()
        {
            plrMass = 16;

            if (Player.CountBuffs() == 0 || buffUpdateTime < 0 || buffUpdateTime >= int.MaxValue)
            {
                buffUpdateTime = 0;
            }

            swiftnessSlipping = false;
            longerPotionSickness = false;
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
                Player.velocity -= plrAcceleration[0] * plrMass * 0.02f;
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
                Player.moveSpeed -= 0.1f;
                Player.GetAttackSpeed(DamageClass.Melee) -= 0.05f;
                Player.statDefense += 4;
            }
        }
    }
}
