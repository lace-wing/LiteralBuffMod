using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;

namespace LiteralBuffMod.Common
{
    public class LiteralUtil
    {
        public static bool TrySpawnNPC(IEntitySource source, Vector2 center, int minX, int maxX, int minY, int maxY, int[] type, int maxTries = 9)
        {
            if (minX >= maxX)
            {
                minX = maxX - 1;
            }
            if (minY >= maxY)
            {
                minY = maxY - 1;
            }
            for (int i = 0; i <= maxTries; i++)
            {
                int x = Main.rand.Next(minX, maxX);
                int y = Main.rand.Next(minY, maxY);
                Vector2 pos = center + new Vector2(Main.rand.NextBool() ? x : -x, Main.rand.NextBool() ? y : -y);
                int pass = 0;
                for (int offX = -6; offX <= 6; offX++)
                {
                    for (int offY = -6; offY <= 3; offY++)
                    {
                        int tX = Math.Clamp((int)(pos.X / 16) + offX, 0, Main.maxTilesX);
                        int tY = Math.Clamp((int)(pos.Y / 16) + offY, 0, Main.maxTilesY);
                        Tile tile = Main.tile[tX, tY];
                        if (tile != null && (!tile.HasTile || tile.IsActuated))
                        {
                            pass++;
                        }
                    }
                }
                if (pass >= 130)
                {
                    NPC.NewNPC(source, (int)pos.X, (int)pos.Y, Main.rand.Next(type));
                    return true;
                }
            }
            return false;
        }
    }
}
