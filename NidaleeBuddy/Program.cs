namespace NidaleeBuddy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Drawing;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using SharpDX;

    internal class Program
    {
        /// <summary>
        /// Nidalee's Name
        /// </summary>
        private const string ChampionName = "Nidalee";

        /// <summary>
        /// Javelin Toss
        /// </summary>
        private static Spell.Skillshot QHuman;

        /// <summary>
        /// Bushwhack
        /// </summary>
        private static Spell.Skillshot WHuman;

        /// <summary>
        /// Primal Surge
        /// </summary>
        private static Spell.Targeted EHuman;
        
        /// <summary>
        /// Takedown
        /// </summary>
        private static Spell.Targeted QCat;
        
        /// <summary>
        /// Pounce
        /// </summary>
        private static Spell.Skillshot WCat;
        
        /// <summary>
        /// Swipe
        /// </summary>
        private static Spell.Skillshot ECat;

        /// <summary>
        /// Javelin -> Pounce
        /// </summary>
        private static Spell.Skillshot WBOTH;

        /// <summary>
        /// Aspect of the Cougar
        /// </summary>
        private static Spell.Active R;

        /// <summary>
        /// Smite
        /// </summary>
        private static Spell.Targeted Smite;

        /// <summary>
        /// Attack range for Human Form
        /// </summary>
        private static float HumanRange = 525;

        /// <summary>
        /// Attack range for Cougar Form
        /// </summary>
        private static float CatRange = 125;

        /// <summary>
        /// Initializes the Menu
        /// </summary>
        private static Menu NidaleeBuddy, ComboMenu, LaneClearMenu, JungleClearMenu, HarassMenu, KillStealMenu, JungleStealMenu, FleeMenu, DrawingMenu, MiscMenu;

        /// <summary>
        /// Gets the Player
        /// </summary>
        private static AIHeroClient PlayerInstance
        {
            get { return Player.Instance; }
        }

        /// <summary>
        /// Checks if Nidalee is in Cougar Form
        /// </summary>
        /// <returns></returns>
        private static bool CatForm()
        {
            return Player.GetSpell(SpellSlot.Q).Name != "JavelinToss";
        }

        /// <summary>
        /// Checks if target has Nidalee Mark
        /// </summary>
        /// <param name="target">The Target</param>
        /// <returns>If target has Nidalee Mark</returns>
        private static bool IsHunted(Obj_AI_Base target)
        {
            return target.HasBuff("nidaleepassivehunted");
        }

        /// <summary>
        /// Returns true if the spell is ready via game time.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private static bool IsReady(float time, float extra = 0f)
        {
            return time < 1 + extra;
        }

        /// <summary>
        /// Returns if there is a summoner spell with Name Provided. (Taken from ActivatorBuddy)
        /// </summary>
        /// <param name="s">String</param>
        /// <returns>Returns if there is a summoner spell with Name Provided.</returns>
        private static bool HasSpell(string s)
        {
            return Player.Spells.FirstOrDefault(o => o.SData.Name.Contains(s)) != null;
        }

        /// <summary>
        /// Gets the amount of damage smite does. (Taken from ActivatorBuddy)
        /// </summary>
        /// <returns></returns>
        private static float GetSmiteDamage()
        {
            int level = ObjectManager.Player.Level;
            float[] smitedamage =
            {
                20*level + 370,
                30*level + 330,
                40*level + 240,
                50*level + 100
            };
            return smitedamage.Max();
        }

        /// <summary>
        /// Smite Purple ID's (Taken from ActivatorBuddy)
        /// </summary>
        private static readonly int[] SmitePurple = { 3713, 3726, 3725, 3724, 3723, 3933 };

        /// <summary>
        /// Smite Grey ID's (Taken from ActivatorBuddy)
        /// </summary>
        private static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719, 3932 };

        /// <summary>
        /// Smite Red ID's (Taken from ActivatorBuddy)
        /// </summary>
        private static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714, 3931 };

        /// <summary>
        /// Smite Blue ID's (Taken from ActivatorBuddy)
        /// </summary>
        private static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707, 3930 };

        /// <summary>
        /// (Taken from ActivatorBuddy)
        /// </summary>
        public static void SetSmiteSlot()
        {
            SpellSlot smiteSlot;
            if (SmiteBlue.Any(x => ObjectManager.Player.InventoryItems.FirstOrDefault(a => a.Id == (ItemId)x) != null))
                smiteSlot = ObjectManager.Player.GetSpellSlotFromName("s5_summonersmiteplayerganker");
            else if (SmiteRed.Any(x => ObjectManager.Player.InventoryItems.FirstOrDefault(a => a.Id == (ItemId)x) != null))
                smiteSlot = ObjectManager.Player.GetSpellSlotFromName("s5_summonersmiteduel");
            else if (SmiteGrey.Any(x => ObjectManager.Player.InventoryItems.FirstOrDefault(a => a.Id == (ItemId)x) != null))
                smiteSlot = ObjectManager.Player.GetSpellSlotFromName("s5_summonersmitequick");
            else if (SmitePurple.Any(x => ObjectManager.Player.InventoryItems.FirstOrDefault(a => a.Id == (ItemId)x) != null))
                smiteSlot = ObjectManager.Player.GetSpellSlotFromName("itemsmiteaoe");
            else
                smiteSlot = ObjectManager.Player.GetSpellSlotFromName("summonersmite");
            Smite = new Spell.Targeted(smiteSlot, 500);
        }

        /// <summary>
        /// Jungle Mob List 
        /// </summary>
        private static string[] JungleMobsList = { "SRU_Red", "SRU_Blue", "SRU_Dragon", "SRU_Baron", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", "SRU_Krug", "Sru_Crab" };

        /// <summary>
        /// Jungle Mob List for Twisted Treeline
        /// </summary>
        private static string[] TTJungleMobsList = { "TT_NWraith1.1", "TT_NWraith4.1", "TT_NGolem2.1", "TT_NGolem5.1", "TT_NWolf3.1", "TT_NWolf6.1", "TT_Spiderboss8.1" };

        /// <summary>
        /// Stores the current tickcount of the spell.
        /// </summary>
        private static Dictionary<string, float> SpellTimer = new Dictionary<string, float>
        {
            { "Takedown", 0f },
            { "Pounce", 0f },
            { "ExPounce", 0f },
            { "Swipe", 0f },
            { "Javelin", 0f },
            { "Bushwhack", 0f },
            { "Primalsurge", 0f },
            { "Aspect", 0f  }
        };

        /// <summary>
        /// Stores when the last spell was used.
        /// </summary>
        private static Dictionary<string, float> TimeStamp = new Dictionary<string, float>
        {
            { "Takedown", 0f },
            { "Pounce", 0f },
            { "Swipe", 0f },
            { "Javelin", 0f },
            { "Bushwhack", 0f },
            { "Primalsurge", 0f },
        };

        /// <summary>
        /// Runs when Program Starts
        /// </summary>
        /// <param name="args">The run arguments</param>
        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        /// <summary>
        /// Runs when Loading is Complete
        /// </summary>
        /// <param name="args">The args</param>
        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (ChampionName != PlayerInstance.BaseSkinName)
            {
                // Just to make sure it won't spam someone's chat.

                bool MessagePrint = true;

                if (MessagePrint)
                {
                    Chat.Print("NidaleeBuddy: Champion is not supported.");
                    MessagePrint = false;
                }
                return;
            }

            // Attempts to Set Spell Data
            try
            {
                // Human Form
                QHuman = new Spell.Skillshot(SpellSlot.Q, 1500, SkillShotType.Linear, 250, 1300, 40);
                WHuman = new Spell.Skillshot(SpellSlot.W, 875, SkillShotType.Circular, 250, int.MaxValue, 100);
                EHuman = new Spell.Targeted(SpellSlot.E, 600);
                R = new Spell.Active(SpellSlot.R, int.MaxValue);

                // Javelin Toss -> Pounce
                WBOTH = new Spell.Skillshot(SpellSlot.W, 740, SkillShotType.Circular, 500, int.MaxValue, 400);

                // Cougar Form
                QCat = new Spell.Targeted(SpellSlot.Q, 400);
                WCat = new Spell.Skillshot(SpellSlot.W, 375, SkillShotType.Circular, 500, int.MaxValue, 400);
                ECat = new Spell.Skillshot(SpellSlot.E, 300, SkillShotType.Cone, 250, int.MaxValue, (int)(15.00 * Math.PI / 180.00));

                // Smite
                if (HasSpell("smite"))
                {
                    Smite = new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonersmite"), 500);
                }
            }
            catch (Exception e)
            {
                Chat.Print("NidaleeBuddy: Exception while trying to set spells.(" + e.Message + ")");
            }

            NidaleeBuddy = MainMenu.AddMenu("Nidalee", "Nidalee");

            // Combo Menu
            ComboMenu = NidaleeBuddy.AddSubMenu("Combo", "Combo");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.Add("useQHuman", new CheckBox("Use Q in Human Form", true));
            ComboMenu.Add("useQCat", new CheckBox("Use Q in Cougar Form", true));
            ComboMenu.Add("useWCat", new CheckBox("Use W in Cougar Form", true));
            ComboMenu.Add("useECat", new CheckBox("Use E in Cougar Form", true));

            // Lane Clear Menu
            LaneClearMenu = NidaleeBuddy.AddSubMenu("Lane Clear", "LaneClear");
            LaneClearMenu.AddGroupLabel("LaneClear Settings");
            LaneClearMenu.Add("useQCat", new CheckBox("Use Q in Cougar Form", true));
            LaneClearMenu.Add("useWCat", new CheckBox("Use W in Cougar Form", true));
            LaneClearMenu.Add("useECat", new CheckBox("Use E in Cougar Form", true));

            // Jungle Clear Menu
            JungleClearMenu = NidaleeBuddy.AddSubMenu("Jungle Clear", "JungleClear");
            JungleClearMenu.AddGroupLabel("JungleClear Settings");
            JungleClearMenu.Add("useQHuman", new CheckBox("Use Q in Human Form", true));
            JungleClearMenu.Add("useQCat", new CheckBox("Use Q in Cougar Form", true));
            JungleClearMenu.Add("useWCat", new CheckBox("Use W in Cougar Form", true));
            JungleClearMenu.Add("useECat", new CheckBox("Use E in Cougar Form", true));

            // Harass Menu
            HarassMenu = NidaleeBuddy.AddSubMenu("Harass", "Harass");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("useQHuman", new CheckBox("Use Q in Human Form", true));

            // Kill Steal Menu
            KillStealMenu = NidaleeBuddy.AddSubMenu("Kill Steal", "KillSteal");
            KillStealMenu.AddGroupLabel("KillSteal Settings");
            KillStealMenu.Add("useQHuman", new CheckBox("Kill Steal using Q in Human Form", true));

            // Jungle Steal Menu
            JungleStealMenu = NidaleeBuddy.AddSubMenu("Jungle Steal", "JungleSteal");
            JungleStealMenu.AddGroupLabel("Jungle Steal Settings");
            JungleStealMenu.Add("useQHuman", new CheckBox("Jungle Steal using Q in Human Form", true));
            JungleStealMenu.Add("useSmite", new CheckBox("Jungle Steal using Smite", true));
            JungleStealMenu.AddSeparator();

            if (Game.MapId == GameMapId.SummonersRift)
            {
                JungleStealMenu.AddLabel("Epics");
                JungleStealMenu.Add("SRU_Baron", new CheckBox("Baron", true));
                JungleStealMenu.Add("SRU_Dragon", new CheckBox("Dragon", true));
                JungleStealMenu.AddLabel("Buffs");
                JungleStealMenu.Add("SRU_Blue", new CheckBox("Blue", true));
                JungleStealMenu.Add("SRU_Red", new CheckBox("Red", true));
                JungleStealMenu.AddLabel("Small Camps");
                JungleStealMenu.Add("SRU_Gromp", new CheckBox("Gromp", false));
                JungleStealMenu.Add("SRU_Murkwolf", new CheckBox("Murkwolf", false));
                JungleStealMenu.Add("SRU_Krug", new CheckBox("Krug", false));
                JungleStealMenu.Add("SRU_Razorbeak", new CheckBox("Razerbeak", false));
                JungleStealMenu.Add("Sru_Crab", new CheckBox("Skuttles", false));
            }

            if (Game.MapId == GameMapId.TwistedTreeline)
            {
                JungleStealMenu.AddLabel("Epics");
                JungleStealMenu.Add("TT_Spiderboss8.1", new CheckBox("Vilemaw", true));
                JungleStealMenu.AddLabel("Camps");
                JungleStealMenu.Add("TT_NWraith1.1", new CheckBox("Wraith", true));
                JungleStealMenu.Add("TT_NWraith4.1", new CheckBox("Wraith", true));
                JungleStealMenu.Add("TT_NGolem2.1", new CheckBox("Golem", true));
                JungleStealMenu.Add("TT_NGolem5.1", new CheckBox("Golem", true));
                JungleStealMenu.Add("TT_NWolf3.1", new CheckBox("Wolf", true));
                JungleStealMenu.Add("TT_NWolf6.1", new CheckBox("Wolf", true));
            }

            // Flee Menu
            FleeMenu = NidaleeBuddy.AddSubMenu("Flee", "Flee");
            FleeMenu.AddGroupLabel("Flee Settings");
            FleeMenu.Add("useWCat", new CheckBox("Use W in Cougar Form", true));

            // Drawing Menu
            DrawingMenu = NidaleeBuddy.AddSubMenu("Drawing", "Drawing");
            DrawingMenu.AddGroupLabel("Drawing Settings");
            DrawingMenu.Add("drawQHuman", new CheckBox("Draw Javelin Range", true));

            // Misc Menu
            var Allies = HeroManager.Allies.Where(a => !a.IsMe).OrderBy(a => a.BaseSkinName);

            MiscMenu = NidaleeBuddy.AddSubMenu("Misc", "Misc");
            MiscMenu.AddGroupLabel("Heal Settings");
            MiscMenu.Add("autoHeal", new CheckBox("Auto Heal Allies and Me", true));
            MiscMenu.Add("autoHealPercent", new Slider("Auto Heal Percent", 50, 0, 100));
            if (Allies != null)
            {
                foreach (var a in Allies)
                {
                    MiscMenu.Add("autoHeal" + a.BaseSkinName, new CheckBox("Auto Heal " + a.BaseSkinName, true));
                }
            }
            MiscMenu.AddGroupLabel("Misc Settings");
            MiscMenu.Add("useR", new CheckBox("Auto Switch R Form", true));

            Chat.Print("NidaleeBuddy | Loaded By KarmaPanda", System.Drawing.Color.LightGreen);

            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
            Game.OnTick += Game_OnTick;
            Game.OnTick += SpellsOnUpdate;
            AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        /// <summary>
        /// Updates the spell timers on update.
        /// </summary>
        /// <param name="args"></param>
        private static void SpellsOnUpdate(EventArgs args)
        {
            SpellTimer["Takedown"] = ((TimeStamp["Takedown"] - Game.Time) > 0)
                ? (TimeStamp["Takedown"] - Game.Time)
                : 0;

            SpellTimer["Pounce"] = ((TimeStamp["Pounce"] - Game.Time) > 0)
                ? (TimeStamp["Pounce"] - Game.Time)
                : 0;

            SpellTimer["Swipe"] = ((TimeStamp["Swipe"] - Game.Time) > 0)
                ? (TimeStamp["Swipe"] - Game.Time)
                : 0;

            SpellTimer["Javelin"] = ((TimeStamp["Javelin"] - Game.Time) > 0)
                ? (TimeStamp["Javelin"] - Game.Time)
                : 0;

            SpellTimer["Bushwhack"] = ((TimeStamp["Bushwhack"] - Game.Time) > 0)
                ? (TimeStamp["Bushwhack"] - Game.Time)
                : 0;

            SpellTimer["Primalsurge"] = ((TimeStamp["Primalsurge"] - Game.Time) > 0)
                ? (TimeStamp["Primalsurge"] - Game.Time)
                : 0;
        }

        /// <summary>
        /// Tracks the spells being casted.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="args">The Args</param>
        private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name.ToLower() == "pounce")
            {
                var unit = args.Target as AIHeroClient;

                if (unit.IsValidTarget() && IsHunted(unit))
                    TimeStamp["Pounce"] = Game.Time + 1.5f;
                else
                    TimeStamp["Pounce"] = Game.Time + (5 + (5 * PlayerInstance.PercentCooldownMod));
            }

            if (sender.IsMe && args.SData.Name.ToLower() == "swipe")
            {
                TimeStamp["Swipe"] = Game.Time + (5 + (5 * PlayerInstance.PercentCooldownMod));
            }

            if (sender.IsMe && args.SData.Name.ToLower() == "primalsurge")
            {
                TimeStamp["Primalsurge"] = Game.Time + (12 + (12 * PlayerInstance.PercentCooldownMod));
            }

            if (sender.IsMe && args.SData.Name.ToLower() == "bushwhack")
            {
                var wperlevel = new[] { 13, 12, 11, 10, 9 }[WCat.Level];
                TimeStamp["Bushwhack"] = Game.Time + (wperlevel + (wperlevel * PlayerInstance.PercentCooldownMod));
            }

            if (sender.IsMe && args.SData.Name.ToLower() == "javelintoss")
            {
                TimeStamp["Javelin"] = Game.Time + (6 + (6 * PlayerInstance.PercentCooldownMod));
            }

            if (sender.IsMe && args.SData.ConsideredAsAutoAttack && PlayerInstance.HasBuff("Takedown"))
            {
                TimeStamp["Takedown"] = Game.Time + (5 + (5 * PlayerInstance.PercentCooldownMod));
            }
        }

        /// <summary>
        /// Does Combo
        /// </summary>
        private static void Combo()
        {
            var useR = MiscMenu["useR"].Cast<CheckBox>().CurrentValue;

            if (CatForm())
            {
                var Q = QCat;
                var W = WCat;
                var E = ECat;
                var useWCat = ComboMenu["useWCat"].Cast<CheckBox>().CurrentValue;
                var useECat = ComboMenu["useECat"].Cast<CheckBox>().CurrentValue;
                var wBTarget = TargetSelector.GetTarget(WBOTH.Range, DamageType.Magical, PlayerInstance.ServerPosition);
                var eTarget = TargetSelector.GetTarget(E.Range, DamageType.Magical, PlayerInstance.ServerPosition); //HeroManager.Enemies.Where(t => t.IsValidTarget() && E.IsInRange(t)).OrderBy(t => t.Distance(PlayerInstance)).FirstOrDefault();

                if (useWCat && wBTarget != null)
                {
                    if (wBTarget.IsValidTarget() && IsHunted(wBTarget))
                    {
                        WBOTH.Cast(wBTarget.ServerPosition);
                    }
                }

                if (useECat && eTarget != null)
                {
                    var ePrediction = E.GetPrediction(eTarget);

                    if (ePrediction != null)
                    {
                        if (IsReady(SpellTimer["Swipe"]) && ePrediction.HitChance == HitChance.High)
                        {
                            E.Cast(ePrediction.CastPosition);
                        }
                    }
                }

                var autoHeal = MiscMenu["autoHeal"].Cast<CheckBox>().CurrentValue;
                var AATarget = HeroManager.Enemies.Where(t => t.IsValidTarget() && PlayerInstance.Distance(t) <= HumanRange).FirstOrDefault();

                if (useR && IsReady(SpellTimer["Javelin"]) && !Q.IsReady() && !W.IsReady() && !E.IsReady())
                {
                    R.Cast();
                }
                else if (AATarget != null && useR && PlayerInstance.Distance(AATarget) > CatRange && !Q.IsReady())
                {
                    R.Cast();
                }
                else if (useR && IsReady(SpellTimer["Javelin"]) && HeroManager.Enemies.Where(t => t.IsValidTarget() && QHuman.IsInRange(t)).Count() >= 1)
                {
                    R.Cast();
                }
                else if (autoHeal && useR && IsReady(SpellTimer["Primalsurge"])
                    && PlayerInstance.HealthPercent <= MiscMenu["autoHealPercent"].Cast<Slider>().CurrentValue
                    && !Q.IsReady() && HeroManager.Enemies.Where(t => t.IsValidTarget() && EHuman.IsInRange(t)).Count() < 1)
                {
                    R.Cast();
                }
            }
            else
            {
                var Q = QHuman;
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical, PlayerInstance.ServerPosition); //HeroManager.Enemies.Where(t => Q.IsInRange(t)).OrderBy(t => t.Health).FirstOrDefault();

                if (target != null)
                {
                    var pred = QHuman.GetPrediction(target);

                    if (pred != null)
                    {
                        if (IsReady(SpellTimer["Javelin"]) && pred.HitChance == HitChance.High)
                        {
                            Q.Cast(pred.CastPosition);
                        }
                    }
                }

                var wTarget = TargetSelector.GetTarget(WBOTH.Range, DamageType.Magical, PlayerInstance.ServerPosition);

                if (wTarget != null)
                {
                    if (useR && IsHunted(wTarget) && WBOTH.IsInRange(wTarget) && !Q.IsReady() && R.IsReady()
                        && IsReady(SpellTimer["Takedown"])
                        && IsReady(SpellTimer["Pounce"])
                        && IsReady(SpellTimer["ExPounce"])
                        && IsReady(SpellTimer["Swipe"]))
                    {
                        R.Cast();
                    }

                    else if (useR && !IsHunted(wTarget) && WCat.IsInRange(wTarget) && !Q.IsReady() && R.IsReady()
                        && IsReady(SpellTimer["Takedown"])
                        && IsReady(SpellTimer["Pounce"])
                        && IsReady(SpellTimer["ExPounce"])
                        && IsReady(SpellTimer["Swipe"]))
                    {
                        R.Cast();
                    }
                }
            }
        }

        /// <summary>
        /// Does Harass
        /// </summary>
        private static void Harass()
        {
            if (!CatForm() && HarassMenu["useQHuman"].Cast<CheckBox>().CurrentValue)
            {
                var target = TargetSelector.GetTarget(QHuman.Range, DamageType.Magical, PlayerInstance.ServerPosition); //HeroManager.Enemies.Where(t => QHuman.IsInRange(t)).OrderBy(t => t.Health);

                if (target != null)
                {
                    var pred = QHuman.GetPrediction(target);

                    if (QHuman.IsReady() && pred.HitChance == HitChance.High)
                    {
                        QHuman.Cast(pred.CastPosition);
                    }
                }
            }
            else if (CatForm())
            {
                if (R.IsReady() && MiscMenu["useR"].Cast<CheckBox>().CurrentValue)
                {
                    R.Cast();
                }
            }
        }

        /// <summary>
        /// Does LaneClear
        /// </summary>
        private static void LaneClear()
        {
            var useR = MiscMenu["useR"].Cast<CheckBox>().CurrentValue;

            if (CatForm())
            {
                var W = WCat;
                var E = ECat;
                var wTarget = EntityManager.GetLaneMinions(EntityManager.UnitTeam.Enemy, PlayerInstance.ServerPosition.To2D(), W.Radius, true).FirstOrDefault();
                var eTarget = EntityManager.GetLaneMinions(EntityManager.UnitTeam.Enemy, PlayerInstance.ServerPosition.To2D(), E.Radius, true);
                var useWCat = LaneClearMenu["useWCat"].Cast<CheckBox>().CurrentValue;
                var useECat = LaneClearMenu["useECat"].Cast<CheckBox>().CurrentValue;

                if (E.IsReady() && useECat && eTarget != null)
                {
                    var ePrediction = Prediction.Position.PredictConeSpellAoe(eTarget.ToArray(), E.Range, E.ConeAngleDegrees, E.CastDelay, E.Speed, PlayerInstance.ServerPosition);

                    if (ePrediction != null)
                    {
                        foreach (var pred in ePrediction)
                        {
                            if (pred.HitChance == HitChance.High)
                            {
                                E.Cast(pred.CastPosition);
                            }
                        }
                    }
                }

                if (useWCat && W.IsReady() && wTarget != null 
                    && HeroManager.Enemies.Where(t => t.IsValidTarget() && t.Distance(PlayerInstance) <= 1000 && t.Distance(wTarget.ServerPosition) <= t.AttackRange).Count() < 1)
                {
                    W.Cast(wTarget.ServerPosition);
                }

                var minion = ObjectManager.Get<Obj_AI_Base>().Where(t => t.IsMinion && t.IsValidTarget() && PlayerInstance.Distance(t) <= HumanRange).FirstOrDefault();
                
                if (minion != null)
                {
                    if (useR 
                        && PlayerInstance.Distance(minion) <= HumanRange
                        && IsReady(SpellTimer["Takedown"])
                        && IsReady(SpellTimer["Pounce"])
                        && IsReady(SpellTimer["Swipe"]))
                    {
                        R.Cast();
                    }
                }
            }
            else
            {
                if (useR && R.IsReady()
                    && !IsReady(SpellTimer["Javelin"])
                    && IsReady(SpellTimer["Takedown"])
                    && IsReady(SpellTimer["Pounce"])
                    && IsReady(SpellTimer["Swipe"]))
                {
                    R.Cast();
                }
            }
        }
        
        /// <summary>
        /// Does JungleClear
        /// </summary>
        private static void JungleClear()
        {
            if (CatForm())
            {
                var Q = QCat;
                var W = WCat;
                var E = ECat;
                var useWCat = JungleClearMenu["useWCat"].Cast<CheckBox>().CurrentValue;
                var useECat = JungleClearMenu["useECat"].Cast<CheckBox>().CurrentValue;
                var useR = MiscMenu["useR"].Cast<CheckBox>().CurrentValue;
                var wTarget = EntityManager.GetJungleMonsters(PlayerInstance.ServerPosition.To2D(), W.Radius, true).FirstOrDefault();
                var wBTarget = ObjectManager.Get<Obj_AI_Base>().Where(u => u.IsVisible && JungleMobsList.Contains(u.BaseSkinName)).FirstOrDefault();//EntityManager.GetJungleMonsters(PlayerInstance.ServerPosition.To2D(), WBOTH.Radius, true).FirstOrDefault();
                var eTarget = EntityManager.GetJungleMonsters(PlayerInstance.ServerPosition.To2D(), E.Radius, true);

                if (wBTarget != null)
                {
                    if (wBTarget.IsValidTarget() && IsHunted(wBTarget) && useWCat && WBOTH.IsInRange(wBTarget))
                    {
                        WBOTH.Cast(wBTarget.ServerPosition);
                    }
                }

                if (wTarget != null)
                {
                    if (wTarget.IsValidTarget() && useWCat && W.IsInRange(wTarget))
                    {
                        W.Cast(wTarget.ServerPosition);
                    }
                }
                
                if (eTarget != null)
                {
                    var ePrediction = Prediction.Position.PredictConeSpellAoe(eTarget.ToArray(), E.Range, E.ConeAngleDegrees, E.CastDelay, E.Speed, PlayerInstance.ServerPosition);

                    if (useECat && ePrediction != null)
                    {
                        foreach (var pred in ePrediction)
                        {
                            if (pred.HitChance == HitChance.High)
                            {
                                E.Cast(pred.CastPosition);
                            }
                        }
                    }
                }

                var JungleTarget = EntityManager.GetJungleMonsters(PlayerInstance.ServerPosition.To2D(), QHuman.Radius, true).FirstOrDefault();

                if (JungleTarget != null)
                {
                    if (useR && IsReady(SpellTimer["Javelin"]))
                    {
                        R.Cast();
                    }
                }
            }
            else
            {
                var useQHuman = JungleClearMenu["useQHuman"].Cast<CheckBox>().CurrentValue;
                var JungleMob = ObjectManager.Get<Obj_AI_Base>().Where(t => t.IsValidTarget() && QHuman.IsInRange(t) && JungleMobsList.Contains(t.BaseSkinName)).FirstOrDefault();//EntityManager.GetJungleMonsters(PlayerInstance.Position.To2D(), QHuman.Radius, true).Where(t => JungleMobsList.Contains(t.BaseSkinName)).FirstOrDefault();
                var useR = MiscMenu["useR"].Cast<CheckBox>().CurrentValue;

                if (JungleMob != null)
                {
                    if (useQHuman && QHuman.IsReady())
                    {
                        var qPrediction = Prediction.Position.PredictUnitPosition(JungleMob, QHuman.Speed);

                        if (qPrediction != null)
                        {
                            QHuman.Cast(qPrediction.To3D());
                        }
                    }

                    if (useR && PlayerInstance.Distance(JungleMob) <= CatRange && R.IsReady()
                        && !IsReady(SpellTimer["Javelin"])
                        && IsReady(SpellTimer["Takedown"])
                        && IsReady(SpellTimer["Pounce"])
                        && IsReady(SpellTimer["Swipe"]))
                    {
                        R.Cast();
                    }
                }
            }
        }

        /// <summary>
        /// Does Flee
        /// </summary>
        private static void Flee()
        {
            if (!CatForm() || !WCat.IsReady())
            {
                return;
            }

            if (FleeMenu["useWCat"].Cast<CheckBox>().CurrentValue)
            {
                WCat.Cast(Game.CursorPos);
            }
        }

        /// <summary>
        /// Does Kill Steal
        /// </summary>
        private static void KillSteal()
        {
            if (!QHuman.IsReady())
            {
                return;
            }

            var target = HeroManager.Enemies.Where(t => t.IsValidTarget() && QHuman.IsInRange(t));

            if (target != null)
            {
                foreach(var t in target)
                {
                    if (t.Health <= DamageLibrary.GetSpellDamage(PlayerInstance, t, SpellSlot.Q, DamageLibrary.SpellStages.Default)) //PlayerInstance.GetSpellDamage(t, SpellSlot.Q))
                    {
                        var pred = QHuman.GetPrediction(t);

                        if (pred.HitChance == HitChance.High)
                        {
                            QHuman.Cast(pred.CastPosition);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Does Jungle Steal
        /// </summary>
        private static void JungleSteal()
        {
            if (Game.MapId == GameMapId.SummonersRift)
            {
                var t = ObjectManager.Get<Obj_AI_Base>().Where(u => u.IsVisible && JungleMobsList.Contains(u.BaseSkinName)).FirstOrDefault();

                if (t != null)
                {
                    if (!JungleStealMenu[t.BaseSkinName].Cast<CheckBox>().CurrentValue)
                    {
                        return;
                    }

                    var useSmite = JungleStealMenu["useSmite"].Cast<CheckBox>().CurrentValue;

                    if (useSmite)
                    {
                        if (t.IsValidTarget()
                            && t.Health <= GetSmiteDamage())
                        {
                            Smite.Cast(t);
                        }
                    }

                    var useQ = JungleStealMenu["useQHuman"].Cast<CheckBox>().CurrentValue;

                    if (useQ && !CatForm())
                    {
                        if (t.IsValidTarget()
                            && QHuman.IsReady()
                            && QHuman.IsInRange(t)
                            && t.Health <= DamageLibrary.GetSpellDamage(PlayerInstance, t, SpellSlot.Q, DamageLibrary.SpellStages.Default))
                        {
                            var pred = Prediction.Position.PredictUnitPosition(t, QHuman.Speed);

                            if (pred != null)
                            {
                                QHuman.Cast(pred.To3D());
                            }
                        }
                    }
                }
            }
            
            if (Game.MapId == GameMapId.TwistedTreeline)
            {
                var t = ObjectManager.Get<Obj_AI_Base>().Where(u => u.IsVisible && JungleMobsList.Contains(u.BaseSkinName)).FirstOrDefault();

                if (t != null)
                {
                    if (!JungleStealMenu[t.BaseSkinName].Cast<CheckBox>().CurrentValue)
                    {
                        return;
                    }

                    var useSmite = JungleStealMenu["useSmite"].Cast<CheckBox>().CurrentValue;

                    if (useSmite)
                    {
                        if (t.IsValidTarget()
                            && t.Health <= GetSmiteDamage())
                        {
                            Smite.Cast(t);
                        }
                    }

                    var useQ = JungleStealMenu["useQHuman"].Cast<CheckBox>().CurrentValue;

                    if (useQ && !CatForm())
                    {
                        if (t.IsValidTarget()
                            && QHuman.IsReady()
                            && QHuman.IsInRange(t)
                            && t.Health <= DamageLibrary.GetSpellDamage(PlayerInstance, t, SpellSlot.Q, DamageLibrary.SpellStages.Default))
                        {
                            var pred = Prediction.Position.PredictUnitPosition(t, QHuman.Speed);

                            QHuman.Cast(pred.To3D());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Does AutoHeal
        /// </summary>
        private static void AutoHeal()
        {
            if (!EHuman.IsReady())
            {
                return;
            }

            var LowestHealthAlly = HeroManager.Allies.Where(a => EHuman.IsInRange(a) && !a.IsMe).OrderBy(a => a.Health).FirstOrDefault();

            if (PlayerInstance.HealthPercent <= MiscMenu["autoHealPercent"].Cast<Slider>().CurrentValue)
            {
                EHuman.Cast(PlayerInstance);
            }

            else if (LowestHealthAlly != null)
            {
                if (LowestHealthAlly.Health <= MiscMenu["autoHealPercent"].Cast<Slider>().CurrentValue)
                {
                    if (MiscMenu["autoHeal" + LowestHealthAlly.BaseSkinName].Cast<CheckBox>().CurrentValue)
                    {
                        EHuman.Cast(LowestHealthAlly);
                    }
                }
            }
        }

        /// <summary>
        /// Before Attack
        /// </summary>
        /// <param name="target">The Target being attacked</param>
        /// <param name="args">The Args</param>
        private static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            var t = target as AIHeroClient;
            var useWCat = ComboMenu["useWCat"].Cast<CheckBox>().CurrentValue;

            if (CatForm())
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    if (useWCat && IsReady(SpellTimer["Pounce"]) && t.IsValidTarget() && WCat.IsInRange(t) && !IsHunted(t))
                    {
                        var Prediction = WCat.GetPrediction(t);
                        
                        if (Prediction.HitChance == HitChance.High)
                        {
                            WCat.Cast(Prediction.CastPosition);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// After Attack
        /// </summary>
        /// <param name="target">The Target that got attacked</param>
        /// <param name="args">The Args</param>
        private static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            var t = target as AIHeroClient;
            var m = target as Obj_AI_Base;

            if (CatForm())
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && ComboMenu["useQCat"].Cast<CheckBox>().CurrentValue)
                {
                    if (t.IsValidTarget() && !t.IsZombie && QCat.IsInRange(t))
                    {
                        QCat.Cast(t);
                    }
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) && LaneClearMenu["useQCat"].Cast<CheckBox>().CurrentValue)
                {
                    if (m.IsValidTarget() && QCat.IsInRange(m))
                    {
                        QCat.Cast(m);
                    }
                }
                
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) && JungleClearMenu["useQCat"].Cast<CheckBox>().CurrentValue)
                {
                    if (m.IsValidTarget() && QCat.IsInRange(m))
                    {
                        QCat.Cast(m);
                    }
                }
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            try
            {
                SetSmiteSlot();
            }
            catch (Exception e)
            {
                Chat.Print("NidaleeBuddy | Failed to set Smite Data: " + e.Message);
            }

            if (!CatForm() && KillStealMenu["useQHuman"].Cast<CheckBox>().CurrentValue)
            {
                KillSteal();
            }

            if (JungleStealMenu["useQHuman"].Cast<CheckBox>().CurrentValue
                || JungleStealMenu["useSmite"].Cast<CheckBox>().CurrentValue)
            {
                JungleSteal();
            }

            if (!CatForm() && !PlayerInstance.IsRecalling && MiscMenu["autoHeal"].Cast<CheckBox>().CurrentValue)
            {
                AutoHeal();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (!CatForm() && DrawingMenu["drawQHuman"].Cast<CheckBox>().CurrentValue)
            {
                EloBuddy.SDK.Rendering.Circle.Draw(QHuman.IsReady() ? SharpDX.Color.Green : SharpDX.Color.Red, QHuman.Range, PlayerInstance.Position);
            }
        }
    }
}
