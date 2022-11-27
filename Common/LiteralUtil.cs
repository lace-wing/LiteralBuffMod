using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Utilities;

namespace LiteralBuffMod.Common
{
    public class LiteralUtil
    {
        public struct TrySpawnPool
        {
            public int length;
            public bool randomType;
            public int totalAmount;
            public int[] type;
            public int[] amount;
            public int[] weight;
            public bool[] greedy;
            public bool[] canOverlap;
            public float[] ai0;
            public float[] ai1;
            public float[] ai2;
            public float[] ai3;
            public void Initialize(int poolLength)
            {
                length = poolLength;
                randomType = false;
                totalAmount = 0;
                type = new int[poolLength];
                amount = new int[poolLength];
                weight = new int[poolLength];
                greedy = new bool[poolLength];
                canOverlap = new bool[poolLength];
                ai0 = new float[poolLength];
                ai1 = new float[poolLength];
                ai2 = new float[poolLength];
                ai3 = new float[poolLength];

                for (int i = 0; i < poolLength; i++)
                {
                    amount[i] = 1;
                    weight[i] = 9;
                    greedy[i] = true;
                }
            }
            public void Set(bool setRandomType = false, int setTotalAmount = 0, int[] setType = default, int[] setAmount = default, int[] setWeight = default, bool[] setGreedy = default, bool[] setOverlap = default, float[] setAI0 = default, float[] setAI1 = default, float[] setAI2 = default, float[] setAI3 = default)
            {
                randomType = setRandomType;
                totalAmount = setTotalAmount;

                for (int i = 0; i < length; i++)
                {
                    if (setType != default)
                        type[i] = setType[i];
                    if (setAmount != default)
                        amount[i] = setAmount[i];
                    if (setWeight != default)
                        weight[i] = setWeight[i];
                    if (setGreedy != default)
                        greedy[i] = setGreedy[i];
                    if (setOverlap != default)
                        canOverlap[i] = setOverlap[i];
                    if (setAI0 != default)
                        ai0[i] = setAI0[i];
                    if (setAI1 != default)
                        ai1[i] = setAI1[i];
                    if (setAI2 != default)
                        ai2[i] = setAI2[i];
                    if (setAI3 != default)
                        ai3[i] = setAI3[i];
                }
            }
            public void RearrangeForRandom()
            {

            }
        };

        public static NPC[] TrySpawnNPC(IEntitySource source, Rectangle whiteArea, Rectangle blackArea, TrySpawnPool pool)
        {
            List<NPC> result = new List<NPC>();

            if (pool.length <= 0)
            {
                return result.ToArray();
            }

            Rectangle whiteTile = SafeTileRectangle(whiteArea); // 将传入的Rectangle转化为物块坐标的Rectangle
            Rectangle blackTile = SafeTileRectangle(blackArea);

            List<Point> pointToTry = new List<Point>(); // 要判定的点

            // 遍历范围内的物块
            for (int tX = 0; tX <= whiteTile.Width; tX++)
            {
                for (int tY = 0; tY <= whiteTile.Height; tY++)
                {
                    Tile tile = Main.tile[whiteTile.X + tX, whiteTile.Y + tY];
                    if (tile == null || !tile.HasUnactuatedTile || !Main.tileSolid[tile.TileType])
                    {
                        pointToTry.Add(new Point(whiteTile.X + tX, whiteTile.Y + tY)); // 此物块可以
                    }
                }
            }

            int i = 0;
            while (i < pointToTry.Count)
            {
                if (blackTile.Contains(pointToTry[i]))
                    pointToTry.Remove(pointToTry[i]);
                else
                    i++;
            }

            Dictionary<Rectangle, List<Point>> spaceToPoint = new Dictionary<Rectangle, List<Point>>(); // 空间对应的生成点
            List<(IEntitySource, int, int, int, int, float, float, float, float)> spawnInfo = 
                new List<(IEntitySource, int, int, int, int, float, float, float, float)>();
            List<Task> spawnTask = new List<Task>(); // 生成的任务List

            for (int n = 0; n <= (pool.randomType ? pool.totalAmount - 1 : pool.length); n++) // 尝试 length 或 totalAmount 次
            {
                Task taskPerType = new Task(() =>
                {
                    Vector2 pos; // 生成的位置
                    List<Point> spawnPoint = new List<Point>(); // 可以生成的点

                    int index = 0; // 选取要生成的NPC
                    if (pool.randomType)
                    {
                        int sum = pool.weight.Sum();
                        int rand = Main.rand.Next(sum) + 1;
                        sum = 0;
                        for (int i = 0; i < pool.length; i++)
                        {
                            sum += pool.weight[i];
                            if (sum >= rand)
                            {
                                index = i;
                                break;
                            }
                        }
                    }
                    else
                    {
                        index = n;
                    }
                    NPC npc = ContentSamples.NpcsByNetId[pool.type[index]]; // 获取要生成的NPC
                    Rectangle spaceNedded; // 需要的空间
                    spaceNedded = npc.Hitbox;
                    spaceNedded.Width = (int)(spaceNedded.Width / 16f);
                    spaceNedded.Height = (int)(spaceNedded.Height / 16f);

                    bool canSpawn;
                    lock (spaceToPoint)
                    {
                        if (!spaceToPoint.ContainsKey(spaceNedded))
                        {
                            spawnPoint = new List<Point>(); // 刷新可生成点
                            lock (pointToTry)
                            {
                                foreach (Point point in pointToTry) // 从要尝试的点中找出可生成的点
                                {
                                    canSpawn = true;
                                    for (int x = 0; x <= spaceNedded.Width; x++)
                                    {
                                        for (int y = 0; y <= spaceNedded.Height; y++)
                                        {
                                            if (!pointToTry.Contains(new Point(point.X + x, point.Y + y)))
                                            {
                                                canSpawn = false;
                                            }
                                            if (!canSpawn)
                                            {
                                                break;
                                            }
                                        }
                                        if (!canSpawn)
                                        {
                                            break;
                                        }
                                    }
                                    if (canSpawn)
                                    {
                                        spawnPoint.Add(point);
                                    }
                                }
                            }
                            spaceToPoint.Add(spaceNedded, spawnPoint);
                        }
                        else
                        {
                            spawnPoint = spaceToPoint.GetValueOrDefault(spaceNedded);
                        }
                    }

                    for (int a = 0; a < pool.amount[index]; a++)
                    {
                        if (spawnPoint.Count == 0) // 无可生成的点就结束
                        {
                            break;
                        }

                        Point point = new Point();
                        if (pool.greedy[index])
                        {
                            point = Main.rand.Next(spawnPoint); // 从可行的点中取，必定能生成
                        }
                        else
                        {
                            Point alt = new Point(whiteTile.X + Main.rand.Next(whiteTile.Width), whiteTile.Y + Main.rand.Next(whiteTile.Height));
                            if (!spawnPoint.Contains(alt)) // 先随机取点再判行不行，不行就跳过
                            {
                                continue;
                            }
                        }

                        pos = new Vector2(point.X * 16 + (int)(npc.Hitbox.Width / 2f), point.Y * 16 + npc.Hitbox.Height); // NewNPC的位置是NPC底边的中心
                        lock (spawnInfo)
                        {
                            spawnInfo.Add((source, (int)pos.X, (int)pos.Y, pool.type[index], 0, pool.ai0[index], pool.ai1[index], pool.ai2[index], pool.ai3[index]));
                        }

                        if (!pool.canOverlap[index]) // 不重叠
                        {
                            for (int x = 0; x <= spaceNedded.Width; x++)
                            {
                                for (int y = 0; y <= spaceNedded.Height; y++)
                                {
                                    Point used = new Point(point.X + x, point.Y + y);
                                    spawnPoint.Remove(used); // 从可生成的点中移除
                                    lock (pointToTry)
                                    {
                                        pointToTry.Remove(used); // 以后也不再尝试
                                    }
                                }
                            }
                        }
                    }
                });
                spawnTask.Add(taskPerType);
                taskPerType.Start();
            }

            foreach (Task task in spawnTask)
            {
                task.Wait();
            }
            foreach (Task task in spawnTask)
            {
                task.Dispose();
            }
            foreach(var info in spawnInfo)
            {
                NPC npc = NPC.NewNPCDirect(info.Item1, info.Item2, info.Item3, info.Item4, info.Item5, info.Item6, info.Item7, info.Item8, info.Item9);
                lock (result)
                {
                    result.Add(npc);
                }
            }
            return result.ToArray();
        }

        public static int SafeTileAxis(int n, bool isY = false)
        {
            return Math.Clamp((int)(n / 16f), 0, isY ? Main.maxTilesY : Main.maxTilesX);
        }

        public static Point SafeTileCoord(Vector2 coord)
        {
            int tileCoordX = Math.Clamp((int)(coord.X / 16f), 0, Main.maxTilesX);
            int tileCoordY = Math.Clamp((int)(coord.Y / 16f), 0, Main.maxTilesY);
            return new Point(tileCoordX, tileCoordY);
        }

        public static Rectangle SafeTileRectangle(Rectangle rectangle)
        {
            int x = Math.Clamp((int)(rectangle.X / 16f), 0, Main.maxTilesX);
            int y = Math.Clamp((int)(rectangle.Y / 16f), 0, Main.maxTilesY);
            int w = Math.Min((int)(rectangle.Width / 16f), Main.maxTilesX - x);
            int h = Math.Min((int)(rectangle.Height / 16f), Main.maxTilesY - y);
            return new Rectangle(x, y, w, h);
        }
    }
}
