using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteralBuffMod.Content.Items;

namespace LiteralBuffMod.Common
{
    public class LiteralAccPlayer : ModPlayer
    {
        public bool literalSwordAcc;

        public override void ResetEffects()
        {
            literalSwordAcc = false;
        }

        public override void UpdateEquips()
        {
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            base.ModifyHitByNPC(npc, ref damage, ref crit);
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            base.ModifyHitByProjectile(proj, ref damage, ref crit);
        }
    }
}
