using LiteralBuffMod.Content.Battles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using static LiteralBuffMod.Common.BattleSystem;
using static LiteralBuffMod.Content.Battles.BaseBattleSystem;

namespace LiteralBuffMod.Common
{
    public class BattleChallengeNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (SlimeRainBattleNPC.Contains(npc))
            {
                drawColor *= 0.25f;
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            if (SlimeRainBattleNPC.Contains(npc))
            {
                target.AddBuff(BuffID.Slimed, 360);
                if (target.HasBuff(BuffID.Slimed))
                {
                    npc.life += target.buffTime[target.FindBuffIndex(BuffID.Slimed)] / 30;
                    CombatText.NewText(npc.Hitbox, Colors.RarityGreen, target.buffTime[target.FindBuffIndex(BuffID.Slimed)] / 30);
                }
            }
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        {
            if (SlimeRainBattleNPC.Contains(npc))
            {
                if (target.HasBuff(BuffID.Slimed))
                {
                    damage = (int)(damage * 1.05f);
                    if (!crit && Main.rand.NextBool(20))
                    {
                        crit = true;
                    }
                }
            }
        }
    }
}
