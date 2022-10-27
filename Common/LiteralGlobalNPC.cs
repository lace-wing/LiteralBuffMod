using Terraria.ID;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace LiteralBuffMod.Common
{
    public class LiteralGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public int npcActiveTime;

        public bool npcDrown;

        public override void PostAI(NPC npc)
        {
            if (npcActiveTime >= int.MaxValue || npcActiveTime < 0)
            {
                npcActiveTime = 0;
            }
            npcActiveTime++;

            if (npc.noTileCollide)
            {
                if (Collision.WetCollision(npc.position, npc.width, npc.height))
                {
                    npc.wet = true;
                }
                else npc.wet = false;
            }

            if (LiteralSets.aquaticNPCTypes.Contains(npc.type)) // 水生生物离水溺水
            {
                if (npc.wet)
                {
                    npc.breath += 3;
                    npc.breath = Math.Clamp(npc.breath, 0, 200);
                }
                if (!npc.wet)
                {
                    if (npc.breath >= 0)
                    {
                        npc.breath -= npcActiveTime % 7 == 0 ? 3 : 0;
                    }
                    if (npc.breath <= 0)
                    {
                        npc.breath = 0;
                        npcDrown = true;
                    }
                }
            }
            else if (!npc.wet) // 不然就回复breath（原版不会）
            {
                npc.breath += 3;
                npc.breath = Math.Clamp(npc.breath, 0, 200);
            }
            else if (npc.wet)
            {
                if (npc.breath >= 0)
                {
                    npc.breath -= npcActiveTime % 7 == 0 ? 3 : 0;
                }
                if (npc.breath <= 0)
                {
                    npc.breath = 0;
                    npcDrown = true;
                }
            }

            if (npc.breath > 0)
            {
                npcDrown = false;
            }

            if (npcActiveTime % 60 == 0)
                CombatText.NewText(new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height), Color.LightBlue, $"life: {npc.life}, breath: {npc.breath}, regen: {npc.lifeRegen}, wet: {npc.wet}");
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (npcDrown) // 溺水掉血
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= Main.hardMode ? 36 : 12;
                damage += Main.hardMode ? 18 : 6;
                if (npc.life <= 1)
                {
                    npc.life = 1;
                    npc.StrikeNPCNoInteraction(4, 0f, 0);
                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, 2f);
                    }
                }
            }
        }
    }
}
