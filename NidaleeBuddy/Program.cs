namespace NidaleeBuddy
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

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
        private const float HumanRange = 525;

        /// <summary>
        /// Attack range for Cougar Form
        /// </summary>
        private const float CatRange = 125;

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
            var level = ObjectManager.Player.Level;
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
        private static readonly string[] JungleMobsList = { "SRU_Red", "SRU_Blue", "SRU_Dragon", "SRU_Baron", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", "SRU_Krug", "Sru_Crab" };

        /// <summary>
        /// Jungle Mob List for Twisted Treeline
        /// </summary>
        private static readonly string[] JungleMobsListTT = { "TT_NWraith1.1", "TT_NWraith4.1", "TT_NGolem2.1", "TT_NGolem5.1", "TT_NWolf3.1", "TT_NWolf6.1", "TT_Spiderboss8.1" };

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
        private static void Main()
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
                /*if (HasSpell("smite"))
                {
                    Smite = new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonersmite"), 500);
                }*/
            }
            catch (Exception e)
            {
                Chat.Print("NidaleeBuddy: Exception while trying to set spells.(" + e.Message + ")");
            }

            Bootstrap.Init(null);

            NidaleeBuddy = MainMenu.AddMenu("Nidalee", "Nidalee");
            NidaleeBuddy.AddGroupLabel("This addon is made by KarmaPanda and should not be redistributed in any way.");
            NidaleeBuddy.AddGroupLabel("Any unauthorized redistribution without credits will result in severe consequences.");
            NidaleeBuddy.AddGroupLabel("Thank you for using this addon and have a fun time!");

            // Combo Menu
            ComboMenu = NidaleeBuddy.AddSubMenu("Combo", "Combo");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.Add("useQHuman", new CheckBox("Use Q in Human Form"));
            ComboMenu.Add("useWHuman", new CheckBox("Use W in Human Form"));
            ComboMenu.Add("useQCat", new CheckBox("Use Q in Cougar Form"));
            ComboMenu.Add("useWCat", new CheckBox("Use W in Cougar Form"));
            ComboMenu.Add("useECat", new CheckBox("Use E in Cougar Form"));
            ComboMenu.Add("useR", new CheckBox("Auto Switch R Form"));

            // Lane Clear Menu
            LaneClearMenu = NidaleeBuddy.AddSubMenu("Lane Clear", "LaneClear");
            LaneClearMenu.AddGroupLabel("LaneClear Settings");
            LaneClearMenu.Add("useQCat", new CheckBox("Use Q in Cougar Form"));
            LaneClearMenu.Add("useWCat", new CheckBox("Use W in Cougar Form"));
            LaneClearMenu.Add("useECat", new CheckBox("Use E in Cougar Form"));
            LaneClearMenu.Add("useR", new CheckBox("Auto Switch R Form"));

            // Jungle Clear Menu
            JungleClearMenu = NidaleeBuddy.AddSubMenu("Jungle Clear", "JungleClear");
            JungleClearMenu.AddGroupLabel("JungleClear Settings");
            JungleClearMenu.Add("useQHuman", new CheckBox("Use Q in Human Form"));
            JungleClearMenu.Add("useQCat", new CheckBox("Use Q in Cougar Form"));
            JungleClearMenu.Add("useWCat", new CheckBox("Use W in Cougar Form"));
            JungleClearMenu.Add("useECat", new CheckBox("Use E in Cougar Form"));
            JungleClearMenu.Add("useR", new CheckBox("Auto Switch R Form"));

            // Harass Menu
            HarassMenu = NidaleeBuddy.AddSubMenu("Harass", "Harass");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("useQHuman", new CheckBox("Use Q in Human Form"));
            HarassMenu.Add("useR", new CheckBox("Auto Switch R Form"));

            // Kill Steal Menu
            KillStealMenu = NidaleeBuddy.AddSubMenu("Kill Steal", "KillSteal");
            KillStealMenu.AddGroupLabel("KillSteal Settings");
            KillStealMenu.Add("useQHuman", new CheckBox("Kill Steal using Q in Human Form"));
            KillStealMenu.Add("useAll", new CheckBox("Use KillSteal all the time or not in any modes", false));

            // Jungle Steal Menu
            JungleStealMenu = NidaleeBuddy.AddSubMenu("Jungle Steal", "JungleSteal");
            JungleStealMenu.AddGroupLabel("Jungle Steal Settings");
            JungleStealMenu.Add("useQHuman", new CheckBox("Jungle Steal using Q in Human Form"));
            JungleStealMenu.Add("useSmite", new CheckBox("Jungle Steal using Smite"));
            JungleStealMenu.AddSeparator();

            if (Game.MapId == GameMapId.SummonersRift)
            {
                JungleStealMenu.AddLabel("Epics");
                JungleStealMenu.Add("SRU_Baron", new CheckBox("Baron"));
                JungleStealMenu.Add("SRU_Dragon", new CheckBox("Dragon"));
                JungleStealMenu.AddLabel("Buffs");
                JungleStealMenu.Add("SRU_Blue", new CheckBox("Blue"));
                JungleStealMenu.Add("SRU_Red", new CheckBox("Red"));
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
                JungleStealMenu.Add("TT_Spiderboss8.1", new CheckBox("Vilemaw"));
                JungleStealMenu.AddLabel("Camps");
                JungleStealMenu.Add("TT_NWraith1.1", new CheckBox("Wraith"));
                JungleStealMenu.Add("TT_NWraith4.1", new CheckBox("Wraith"));
                JungleStealMenu.Add("TT_NGolem2.1", new CheckBox("Golem"));
                JungleStealMenu.Add("TT_NGolem5.1", new CheckBox("Golem"));
                JungleStealMenu.Add("TT_NWolf3.1", new CheckBox("Wolf"));
                JungleStealMenu.Add("TT_NWolf6.1", new CheckBox("Wolf"));
            }

            // Flee Menu
            FleeMenu = NidaleeBuddy.AddSubMenu("Flee", "Flee");
            FleeMenu.AddGroupLabel("Flee Settings");
            FleeMenu.Add("useWCat", new CheckBox("Use W in Cougar Form"));

            // Drawing Menu
            DrawingMenu = NidaleeBuddy.AddSubMenu("Drawing", "Drawing");
            DrawingMenu.AddGroupLabel("Drawing Settings");
            DrawingMenu.Add("drawQHuman", new CheckBox("Draw Javelin Range"));

            // Misc Menu
            var allies = HeroManager.Allies.Where(a => !a.IsMe).OrderBy(a => a.BaseSkinName);

            MiscMenu = NidaleeBuddy.AddSubMenu("Misc", "Misc");
            MiscMenu.AddGroupLabel("Heal Settings");
            MiscMenu.Add("autoHeal", new CheckBox("Auto Heal Allies and Me"));
            MiscMenu.Add("autoHealPercent", new Slider("Auto Heal Percent", 50));
            foreach (var a in allies)
            {
                MiscMenu.Add("autoHeal" + a.BaseSkinName, new CheckBox("Auto Heal " + a.BaseSkinName));
            }
            //MiscMenu.AddGroupLabel("Misc Settings");
            //MiscMenu.Add("useR", new CheckBox("Auto Switch R Form"));

            Chat.Print("NidaleeBuddy | Loaded By KarmaPanda", Color.LightGreen);

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
            var useR = ComboMenu["useR"].Cast<CheckBox>().CurrentValue;

            if (CatForm())
            {
                var Q = QCat;
                var W = WCat;
                var E = ECat;
                var useWCat = ComboMenu["useWCat"].Cast<CheckBox>().CurrentValue;
                var useECat = ComboMenu["useECat"].Cast<CheckBox>().CurrentValue;
                var wBTarget = TargetSelector.GetTarget(WBOTH.Range, DamageType.Magical, PlayerInstance.ServerPosition);
                var eTarget = TargetSelector.GetTarget(E.Range, DamageType.Magical, PlayerInstance.ServerPosition);

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
                var aaTarget = TargetSelector.GetTarget(HumanRange, DamageType.Physical, PlayerInstance.ServerPosition);//HeroManager.Enemies.FirstOrDefault(t => t.IsValidTarget() && PlayerInstance.Distance(t) <= HumanRange);

                if (useR && IsReady(SpellTimer["Javelin"]) && !Q.IsReady() && !W.IsReady() && !E.IsReady())
                {
                    R.Cast();
                }
                else if (aaTarget != null && useR && PlayerInstance.Distance(aaTarget) > CatRange && !Q.IsReady())
                {
                    R.Cast();
                }
                else if (useR && IsReady(SpellTimer["Javelin"]) && HeroManager.Enemies.Any(t => t.IsValidTarget() && QHuman.IsInRange(t)))
                {
                    R.Cast();
                }
                else if (autoHeal && useR && IsReady(SpellTimer["Primalsurge"])
                    && PlayerInstance.HealthPercent <= MiscMenu["autoHealPercent"].Cast<Slider>().CurrentValue
                    && !Q.IsReady() && !HeroManager.Enemies.Any(t => t.IsValidTarget() && EHuman.IsInRange(t)))
                {
                    R.Cast();
                }
            }
            else
            {
                var Q = QHuman;
                var W = WHuman;
                var qHTarget = TargetSelector.GetTarget(Q.Range, DamageType.Magical, PlayerInstance.ServerPosition);
                var wHTarget = TargetSelector.GetTarget(W.Range, DamageType.Magical, PlayerInstance.ServerPosition);
                var useQHuman = ComboMenu["useQHuman"].Cast<CheckBox>().CurrentValue;
                var useWHuman = ComboMenu["useWHuman"].Cast<CheckBox>().CurrentValue;

                if (useQHuman && qHTarget != null)
                {
                    var pred = Q.GetPrediction(qHTarget);

                    if (pred != null)
                    {
                        if (IsReady(SpellTimer["Javelin"]) && pred.HitChance == HitChance.High)
                        {
                            Q.Cast(pred.CastPosition);
                        }
                    }
                }

                if (useWHuman && wHTarget != null)
                {
                    var pred = W.GetPrediction(wHTarget);

                    if (W.IsReady() && pred.HitChance == HitChance.Medium)
                    {
                        W.Cast(pred.CastPosition);
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

                    else if (useR && !IsHunted(wTarget) && WCat.IsInRange(wTarget) && R.IsReady()
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
        /// Does Harass
        /// </summary>
        private static void Harass()
        {
            if (!CatForm() && HarassMenu["useQHuman"].Cast<CheckBox>().CurrentValue)
            {
                var target = TargetSelector.GetTarget(QHuman.Range, DamageType.Magical, PlayerInstance.ServerPosition);

                if (target == null)
                {
                    return;
                }
                var pred = QHuman.GetPrediction(target);

                if (QHuman.IsReady() && pred.HitChance == HitChance.High)
                {
                    QHuman.Cast(pred.CastPosition);
                }
            }
            else if (CatForm())
            {
                if (R.IsReady() && HarassMenu["useR"].Cast<CheckBox>().CurrentValue)
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
            var useR = LaneClearMenu["useR"].Cast<CheckBox>().CurrentValue;

            if (CatForm())
            {
                var W = WCat;
                var E = ECat;
                var wTarget = EntityManager.GetLaneMinions(EntityManager.UnitTeam.Enemy, PlayerInstance.ServerPosition.To2D(), W.Radius).FirstOrDefault();
                var eTarget = EntityManager.GetLaneMinions(EntityManager.UnitTeam.Enemy, PlayerInstance.ServerPosition.To2D(), E.Radius);
                var useWCat = LaneClearMenu["useWCat"].Cast<CheckBox>().CurrentValue;
                var useECat = LaneClearMenu["useECat"].Cast<CheckBox>().CurrentValue;

                if (useECat && E.IsReady() && eTarget != null)
                {
                    var ePrediction = Prediction.Position.PredictConeSpellAoe(eTarget.ToArray(), E.Range, E.ConeAngleDegrees, E.CastDelay, E.Speed, PlayerInstance.ServerPosition);

                    if (ePrediction != null)
                    {
                        foreach (var pred in ePrediction.Where(pred => pred.HitChance == HitChance.High))
                        {
                            E.Cast(pred.CastPosition);
                        }
                    }
                }

                if (useWCat && W.IsReady() && wTarget != null 
                    && !HeroManager.Enemies.Any(t => t.IsValidTarget() && t.Distance(PlayerInstance) <= 1000 && t.Distance(wTarget.ServerPosition) <= t.AttackRange))
                {
                    W.Cast(wTarget.ServerPosition);
                }

                var minion = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(t => t.IsMinion && t.IsValidTarget() && PlayerInstance.Distance(t) <= HumanRange);

                if (minion == null)
                {
                    return;
                }
                if (useR
                    && PlayerInstance.Distance(minion) <= HumanRange
                    && !IsReady(SpellTimer["Takedown"])
                    && !IsReady(SpellTimer["Pounce"])
                    && !IsReady(SpellTimer["Swipe"]))
                {
                    R.Cast();
                }
            }
            else
            {
                var minion = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(t => t.IsMinion && t.IsValidTarget() && PlayerInstance.Distance(t) <= HumanRange);

                if (useR && R.IsReady()
                    && PlayerInstance.Distance(minion) <= CatRange
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
                var useR = JungleClearMenu["useR"].Cast<CheckBox>().CurrentValue;
                var wTarget = EntityManager.GetJungleMonsters(PlayerInstance.ServerPosition.To2D(), W.Radius).FirstOrDefault();
                var wBTarget = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(u => WBOTH.IsInRange(u) && JungleMobsList.Contains(u.BaseSkinName));
                var eTarget = EntityManager.GetJungleMonsters(PlayerInstance.ServerPosition.To2D(), E.Radius);

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
                        foreach (var pred in ePrediction.Where(pred => pred.HitChance == HitChance.High))
                        {
                            E.Cast(pred.CastPosition);
                        }
                    }
                }

                var JungleTarget = EntityManager.GetJungleMonsters(PlayerInstance.ServerPosition.To2D(), QHuman.Radius).FirstOrDefault();

                if (JungleTarget == null)
                {
                    return;
                }
                if (useR && IsReady(SpellTimer["Javelin"]))
                {
                    R.Cast();
                }
            }
            else
            {
                var useQHuman = JungleClearMenu["useQHuman"].Cast<CheckBox>().CurrentValue;
                var JungleMob = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(t => t.IsValidTarget() && QHuman.IsInRange(t) && JungleMobsList.Contains(t.BaseSkinName));//EntityManager.GetJungleMonsters(PlayerInstance.Position.To2D(), QHuman.Radius, true).Where(t => JungleMobsList.Contains(t.BaseSkinName)).FirstOrDefault();
                var useR = JungleClearMenu["useR"].Cast<CheckBox>().CurrentValue;

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
                    else if (IsHunted(JungleMob) && IsReady(SpellTimer["ExPounce"]))
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
            if (!CatForm() && IsReady(SpellTimer["Pounce"]) && R.IsReady())
            {
                R.Cast();
            }

            if (CatForm() && IsReady(SpellTimer["Pounce"]) && FleeMenu["useWCat"].Cast<CheckBox>().CurrentValue)
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

            foreach (var pred in target.Where(t => t.Health < PlayerInstance.GetSpellDamage(t, SpellSlot.Q)).Select(t => QHuman.GetPrediction(t)).Where(pred => pred.HitChance == HitChance.High))
            {
                QHuman.Cast(pred.CastPosition);
            }
        }

        /// <summary>
        /// Does Jungle Steal
        /// </summary>
        private static void JungleSteal()
        {
            if (Game.MapId == GameMapId.SummonersRift)
            {
                var t = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(u => QHuman.IsInRange(u) && u.IsVisible && JungleMobsList.Contains(u.BaseSkinName));

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
                            && t.Health <= PlayerInstance.GetSpellDamage(t, SpellSlot.Q))
                        {
                            var pred = Prediction.Position.PredictUnitPosition(t, QHuman.Speed);

                            QHuman.Cast(pred.To3D());
                        }
                    }
                }
            }
            
            if (Game.MapId == GameMapId.TwistedTreeline)
            {
                var t = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(u => QHuman.IsInRange(u) && u.IsVisible && JungleMobsListTT.Contains(u.BaseSkinName));

                if (t == null)
                {
                    return;
                }

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

                if (!useQ || CatForm())
                {
                    return;
                }
                if (!t.IsValidTarget() || !QHuman.IsReady() || !QHuman.IsInRange(t)
                    || !(t.Health
                         <= PlayerInstance.GetSpellDamage(t, SpellSlot.Q)))
                {
                    return;
                }
                var pred = Prediction.Position.PredictUnitPosition(t, QHuman.Speed);

                QHuman.Cast(pred.To3D());
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

            var lowestHealthAlly = HeroManager.Allies.Where(a => EHuman.IsInRange(a) && !a.IsMe).OrderBy(a => a.Health).FirstOrDefault();

            if (PlayerInstance.HealthPercent <= MiscMenu["autoHealPercent"].Cast<Slider>().CurrentValue)
            {
                EHuman.Cast(PlayerInstance);
            }

            else if (lowestHealthAlly != null)
            {
                if (!(lowestHealthAlly.Health <= MiscMenu["autoHealPercent"].Cast<Slider>().CurrentValue))
                {
                    return;
                }
                if (MiscMenu["autoHeal" + lowestHealthAlly.BaseSkinName].Cast<CheckBox>().CurrentValue)
                {
                    EHuman.Cast(lowestHealthAlly);
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

            if (!CatForm())
            {
                return;
            }
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                return;
            }
            if (!useWCat || !IsReady(SpellTimer["Pounce"]) || !t.IsValidTarget() || !WCat.IsInRange(t) || IsHunted(t))
            {
                return;
            }
            var pred = WCat.GetPrediction(t);
                        
            if (pred.HitChance == HitChance.High)
            {
                WCat.Cast(pred.CastPosition);
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

            if (!CatForm())
            {
                return;
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && ComboMenu["useQCat"].Cast<CheckBox>().CurrentValue)
            {
                if (t != null)
                {
                    if (t.IsValidTarget() && !t.IsZombie && QCat.IsInRange(t))
                    {
                        QCat.Cast(t);
                    }             
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

            if (!CatForm() 
                && KillStealMenu["useQHuman"].Cast<CheckBox>().CurrentValue)
            {
                if (KillStealMenu["useAll"].Cast<CheckBox>().CurrentValue
                    || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
                {
                    KillSteal();
                }
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

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!CatForm() && DrawingMenu["drawQHuman"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(QHuman.IsReady() ? SharpDX.Color.Green : SharpDX.Color.Red, QHuman.Range, PlayerInstance.Position);
            }
        }
    }
}
