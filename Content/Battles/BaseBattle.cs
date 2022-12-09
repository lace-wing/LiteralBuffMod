using IL.Terraria.GameContent.Bestiary;
using LiteralBuffMod.Common;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Default;
using Terraria.ID;

namespace LiteralBuffMod.Content.Battles
{
    public enum State
    {
        Starting, 
        Progressing, 
        Ending
    }
    public abstract class BaseBattle
    {
        /// <summary>
        /// Name of the battle
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description of the battle
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Controls whether the methods in UpdateBattle will be called or not
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// Counts down at last regardless of Active
        /// </summary>
        public int Delay { get; set; }
        /// <summary>
        /// State of the battle
        /// </summary>
        public State BattleState { get; set; }
        /// <summary>
        /// +1 after PostUpdateWave when the battle is Progressing
        /// </summary>
        public int BattleTimer { get; set; }
        /// <summary>
        /// Current level
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// Current wave number
        /// </summary>
        public int Wave { get; set; }
        /// <summary>
        /// Battle ends if Wave > MaxWave
        /// </summary>
        public int MaxWave { get; set; }
        /// <summary>
        /// State of the current wave
        /// </summary>
        public State WaveState { get; set; }
        /// <summary>
        /// +1 before PostUpdateWave when the battle is Progressing
        /// </summary>
        public int WaveTimer { get; set; }
        /// <summary>
        /// Wave ends if WaveCounter > MaxWaveCounter
        /// </summary>
        public int MaxWaveTimer { get; set; }
        /// <summary>
        /// Counters which can be used for any purpose
        /// </summary>
        public float[] Counter = new float[4];
        /// <summary>
        /// Called in PostSetupContent
        /// </summary>
        public virtual void SetDefaults()
        {
            Name = string.Empty;
            Description = string.Empty;
            MaxWave = 1;
            MaxWaveTimer = 60;
            Delay = 3600;
        }
        /// <summary>
        /// Update the battle
        /// <br>Called in PreUpdateInvasions, which means it won't be called on MP client</br>
        /// <br>Can be manually called for extra updates</br>
        /// </summary>
        /// <param name="battlers">Players in this battle</param>
        public void UpdateBattle(Player[] battlers)
        {
            if (Active)
            {
                if (BattleState == State.Starting)
                {
                    ResetBattle(true);
                    OnBattleStart(battlers);
                    BattleState = State.Progressing;
                }
                else if (BattleState == State.Progressing)
                {
                    if (PreUpdateWave(battlers))
                    {
                        if (WaveState == State.Starting)
                        {
                            OnWaveStart(battlers);
                            WaveState = State.Progressing;
                        }
                        else if (WaveState == State.Progressing)
                        {
                            InWave(battlers);
                            WaveTimer = Math.Max(++WaveTimer, 0);
                            if (MaxWaveTimer >= 0 && WaveTimer > MaxWaveTimer)
                            {
                                WaveTimer = 0;
                                WaveState = State.Ending;
                            }
                        }
                        else if (WaveState == State.Ending)
                        {
                            OnWaveEnd(battlers);
                            ResetWave(++Wave);
                        }
                    }
                    PostUpdateWave(battlers);
                    if (MaxWave >= 0 && Wave > MaxWave)
                    {
                        BattleState = State.Ending;
                    }
                    BattleTimer = Math.Max(++BattleTimer, 0);
                }
                else if (BattleState == State.Ending)
                {
                    OnBattleEnd(battlers);
                    ResetBattle();
                }
            }
            else
            {
                ResetBattle();
                Delay = Math.Max(--Delay, 0);
            }
        }
        /// <summary>
        /// Called only if the battle is Starting
        /// <br>The base function resets the battle and proceeds to Progressing</br>
        /// </summary>
        /// <param name="players">Players in this battle</param>
        public virtual void OnBattleStart(Player[] players) { }
        /// Called only if the battle is Ending
        /// <br>The base function resets the battle and deactivates it</br>
        /// </summary>
        /// <param name="players">Players in this battle</param>
        public virtual void OnBattleEnd(Player[] players) { }
        /// <summary>
        /// Called before UpdateWave when the battle is Progressing
        /// </summary>
        /// <param name="players">Players in this battle</param>
        /// <returns>true to allow UpdateWave to be called, true by default</returns>
        public virtual bool PreUpdateWave(Player[] players) { return true; }
        /// <summary>
        /// Called after PreUpdateWave and before PostUpdateWave when the battle is Progressingand the Wave is Starting
        /// <br>Won't be called if PreUpdateWave returns false</br>
        /// </summary>
        /// <param name="players">Players in this battle</param>
        public virtual void OnWaveStart(Player[] players) { }
        /// <summary>
        /// Called after PreUpdateWave and before PostUpdateWave when the battle is Progressing and the Wave is Progressing
        /// <br>Won't be called if PreUpdateWave returns false</br>
        /// </summary>
        /// <param name="players">Players in this battle</param>
        public virtual void InWave(Player[] players) { }
        /// <summary>
        /// Called after PreUpdateWave and before PostUpdateWave when the battle is Progressingand the Wave is Ending
        /// <br>Won't be called if PreUpdateWave returns false</br>
        /// </summary>
        /// <param name="players">Players in this battle</param>
        public virtual void OnWaveEnd(Player[] players) { }
        /// <summary>
        /// Called after UpdateWave and after counting up WaveCounter when the battle is Progressing regardless of PreUpdateWave
        /// </summary>
        /// <param name="players">Players in this battle</param>
        public virtual void PostUpdateWave(Player[] players) { }
        /// <summary>
        /// Starts the battle with a certain Level and Wave
        /// </summary>
        /// <param name="level"></param>
        /// <param name="wave"></param>
        public bool TryStartBattle(int level = 0, int wave = 0)
        {
            if (!Active && Delay <= 0)
            {
                Active = true;
                BattleState = State.Starting;
                Level = level;
                Wave = wave;
                WaveState = State.Starting;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Resets the battle. Level, Wave and all counters are set to 0, all states are set to Starting
        /// </summary>
        /// <param name="active">What Active is set to</param>
        public void ResetBattle(bool active = false)
        {
            Active = active;
            BattleState = State.Starting;
            BattleTimer = 0;
            Level = 0;
            Wave = 0;
            WaveState = State.Starting;
            WaveTimer = 0;
            for (int i = 0; i < Counter.Length; i++)
            {
                Counter[i] = 0;
            }
        }
        /// <summary>
        /// Resets the wave. WaveCounter is set to 0 and WaveState is set to Starting
        /// </summary>
        /// <param name="wave">Waht Wave is set to</param>
        public void ResetWave(int wave = 0)
        {
            Wave = wave;
            WaveState = State.Starting;
            WaveTimer = 0;
        }
        /// <summary>
        /// Get type (index) of the battle
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>index of the battle in BaseBattleSystem.Battles, -1 if failed</returns>
        public static int BattleType<T>()
        {
            for (int i = 0; i < BaseBattleSystem.Battles.Length; i++)
            {
                if (BaseBattleSystem.Battles[i].GetType() == typeof(T))
                {
                    return i;
                }
            }
            return -1;
        }
    }

    public class BaseBattleSystem : ModSystem
    {
        public static BaseBattle[] Battles = new BaseBattle[] { };
        public static Dictionary<BaseBattle, List<Player>> Battlers = new Dictionary<BaseBattle, List<Player>>();

        public static bool IsInBattle(Player player)
        {
            foreach (BaseBattle battle in Battlers.Keys)
            {
                if (Battlers[battle].Contains(player) && battle.Active) return true;
            }
            return false;
        }
        public override void PostSetupContent()
        {
            Type[] types = GetType().Assembly.GetTypes();
            Type battleType = typeof(BaseBattle);
            List<BaseBattle> battleList = new List<BaseBattle>();
            foreach (Type type in types)
            {
                if (type.IsSubclassOf(battleType))
                {
                    object obj = Activator.CreateInstance(type);
                    if (obj != null)
                    {
                        BaseBattle aBattle = obj as BaseBattle;
                        battleList.Add(aBattle);
                    }
                }
            }
            Battles = battleList.ToArray();
            for (int i = 0; i < Battles.Length; i++)
            {
                Battles[i].SetDefaults();
                Battlers.Add(Battles[i], new List<Player>());
            }
        }
        public override void PostUpdatePlayers()
        {
            foreach (BaseBattle battle in Battlers.Keys)
            {
                Battlers[battle].Clear();
            }
            foreach (Player player in Main.player)
            {
                if (player != null && player.active && player.HasBuff(BuffID.Battle))
                {
                    if (!IsInBattle(player))
                    {
                        BaseBattle b = Main.rand.Next(Battlers.Keys.ToArray());
                        Battlers[b].Add(player);
                        b.TryStartBattle();
                    }
                }
            }
        }
        public override void PreUpdateInvasions()
        {
            for (int i = 0; i < Battles.Length; i++)
            {
                Battles[i].UpdateBattle(Battlers[Battles[i]].ToArray());
            }
        }
    }
}
