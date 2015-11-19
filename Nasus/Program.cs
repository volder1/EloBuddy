namespace Nasus
{
    using System;
    using System.Drawing;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    /// <summary>
    /// Main Class
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Nasus's Name
        /// </summary>
        private const string ChampionName = "Nasus";

        /// <summary>
        /// Spell Q
        /// </summary>
        public static Spell.Active Q;

        /// <summary>
        /// Spell W
        /// </summary>
        public static Spell.Targeted W;

        /// <summary>
        /// Spell E
        /// </summary>
        public static Spell.Skillshot E;

        /// <summary>
        /// Spell R
        /// </summary>
        public static Spell.Active R;

        /// <summary>
        /// Initializes the Menu
        /// </summary>
        public static Menu Nasus, FarmMenu, ComboMenu, KillStealMenu, DrawingMenu;

        /// <summary>
        /// Gets the player.
        /// </summary>
        private static AIHeroClient PlayerInstance
        {
            get { return Player.Instance; }
        }

        /// <summary>
        /// Called when the Program Starts
        /// </summary>
        private static void Main()
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        /// <summary>
        /// Called when the Game finished Loading
        /// </summary>
        /// <param name="args">The Args</param>
        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            try
            {
                if (PlayerInstance.BaseSkinName != ChampionName)
                {
                    return;
                }

                Bootstrap.Init(null);

                Q = new Spell.Active(SpellSlot.Q, 150);
                W = new Spell.Targeted(SpellSlot.W, 600);
                E = new Spell.Skillshot(SpellSlot.E, 650, SkillShotType.Circular, 250, 190, int.MaxValue);
                R = new Spell.Active(SpellSlot.R);

                Nasus = MainMenu.AddMenu("Nasus", "Nasus");
                Nasus.AddGroupLabel("Made by KarmaPanda");
                Nasus.AddLabel("Idea by Ban :P");

                // Farm Menu
                FarmMenu = Nasus.AddSubMenu("Farm", "Farm");
                FarmMenu.AddGroupLabel("Spell Usage Settings");
                FarmMenu.AddLabel("Q Settings");
                FarmMenu.Add("useQ", new CheckBox("Last Hit Minion with Q"));
                FarmMenu.Add("disableAA", new CheckBox("Don't LastHit Minion without Q", false));
                FarmMenu.AddLabel("Harass Settings");
                FarmMenu.Add("useQH", new CheckBox("Use Q on Champion", false));
                FarmMenu.Add("useEH", new CheckBox("Use E on Champion", false));
                FarmMenu.Add("manaEH", new Slider("Mana % before E (Harass)", 30));
                FarmMenu.AddLabel("Lane Clear Settings");
                FarmMenu.Add("useELC", new CheckBox("Use E in LaneClear"));
                FarmMenu.Add("useELCS", new Slider("Minions before Casting E", 2, 1, 6));
                FarmMenu.Add("manaELC", new Slider("Mana % before E (Lane Clear)", 30));

                // Combo Menu
                ComboMenu = Nasus.AddSubMenu("Combo", "Combo");
                ComboMenu.AddGroupLabel("Spell Usage Settings");
                ComboMenu.Add("useQ", new CheckBox("Use Q in Combo"));
                ComboMenu.Add("useW", new CheckBox("Use W in Combo"));
                ComboMenu.Add("useE", new CheckBox("Use E in Combo"));
                ComboMenu.Add("useR", new CheckBox("Use R in Combo"));
                ComboMenu.AddGroupLabel("ManaManager");
                ComboMenu.Add("manaW", new Slider("Mana % before W", 25));
                ComboMenu.Add("manaE", new Slider("Mana % before E", 30));
                ComboMenu.AddGroupLabel("R Settings");
                ComboMenu.Add("hpR", new Slider("Use R at % HP", 25));
                ComboMenu.Add("intR", new Slider("Use R when x Enemies are Around", 1, 0, 5));
                ComboMenu.Add("rangeR", new Slider("Use R when Enemies are in x Range", 1200, 0, 2000));

                // Kill Steal Menu
                KillStealMenu = Nasus.AddSubMenu("Kill Steal", "KillSteal");
                KillStealMenu.AddGroupLabel("Spell Usage Settings");
                KillStealMenu.Add("useE", new CheckBox("Use E in Kill Steal"));

                // Drawing Menu
                DrawingMenu = Nasus.AddSubMenu("Drawing", "Drawing");
                DrawingMenu.AddGroupLabel("Spell Drawing Settings");
                DrawingMenu.Add("drawQ", new CheckBox("Draw Killable Minions with Q", false));
                DrawingMenu.Add("drawW", new CheckBox("Draw W Range", false));
                DrawingMenu.Add("drawE", new CheckBox("Draw E Range", false));

                Chat.Print("Nasus | Loaded by KarmaPanda", Color.Green);

                Game.OnTick += Game_OnTick;
                Drawing.OnDraw += Drawing_OnDraw;
                Orbwalker.OnAttack += Orbwalker_OnAttack;
                Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
                Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            }
            catch (Exception e)
            {
                Chat.Print("Nasus | Encountered Exception while Initializing: " + e.Message);
            }
        }

        /// <summary>
        /// Called whenever a target is going to be attacked by Orbwalker.
        /// </summary>
        /// <param name="target">The Target</param>
        /// <param name="args">The Args</param>
        private static void Orbwalker_OnAttack(AttackableUnit target, EventArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit)
                && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                return;
            }
            var t = target as Obj_AI_Base;
            var useQ = FarmMenu["useQ"].Cast<CheckBox>().CurrentValue;

            if (!useQ || t == null)
            {
                return;
            }
            if (t.IsValidTarget() && Q.IsReady())
            {
                Q.Cast();
            }
        }

        /// <summary>
        /// Called whenever a target is about to be attacked by Orbwalker.
        /// </summary>
        /// <param name="target">The Target</param>
        /// <param name="args">The Args</param>
        private static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit)
                && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)
                && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                return;
            }
            var t = target as Obj_AI_Base;
            var useQ = FarmMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var aaDisable = FarmMenu["disableAA"].Cast<CheckBox>().CurrentValue;

            if (aaDisable && !PlayerInstance.HasBuff("SiphoningStrike"))
            {
                args.Process = false;
            }

            if (!useQ || t == null)
            {
                return;
            }
            if (!t.IsValidTarget() || !Q.IsReady())
            {
                return;
            }
            if (GetDamage(t) >= t.Health)
            {
                Q.Cast();
            }
        }

        /// <summary>
        /// Called whenever a target has been attacked by Orbwalker.
        /// </summary>
        /// <param name="target">The Target</param>
        /// <param name="args">The Args</param>
        private static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)
                && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                return;
            }

            var t = target as AIHeroClient;
            var useQ = ComboMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var useQH = FarmMenu["useQH"].Cast<CheckBox>().CurrentValue;

            if (t == null || (!useQ && !useQH))
            {
                return;
            }
            if (t.IsValidTarget() && Q.IsReady())
            {
                Q.Cast();
            }
        }

        /// <summary>
        /// Called whenever the Game Draws Itself
        /// </summary>
        /// <param name="args">The Args</param>
        private static void Drawing_OnDraw(EventArgs args)
        {
            var drawQ = DrawingMenu["drawQ"].Cast<CheckBox>().CurrentValue;
            var drawW = DrawingMenu["drawW"].Cast<CheckBox>().CurrentValue;
            var drawE = DrawingMenu["drawE"].Cast<CheckBox>().CurrentValue;

            if (drawQ)
            {
                var minion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.Distance(PlayerInstance) <= E.Range
                    && t.IsValidTarget()
                    && t.Health <= GetDamage(t));

                foreach (var m in minion)
                {
                    Circle.Draw(SharpDX.Color.Red, m.BoundingRadius, m.Position);
                }
            }

            if (drawW)
            {
                Circle.Draw(W.IsReady() ? SharpDX.Color.Green : SharpDX.Color.Red, W.Range, PlayerInstance.Position);
            }

            if (drawE)
            {
                Circle.Draw(E.IsReady() ? SharpDX.Color.Green : SharpDX.Color.Red, E.Range, PlayerInstance.Position);
            }
        }

        /// <summary>
        /// Called whenever the Game Updates Itself
        /// </summary>
        /// <param name="args">The Args</param>
        private static void Game_OnTick(EventArgs args)
        {
            try
            {
                StateHandler.KillSteal();

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    StateHandler.Combo();
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    StateHandler.Harass();
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                {
                    StateHandler.LaneClear();
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    StateHandler.LastHit();
                }
            }
            catch (Exception e)
            {
                Chat.Print("Nasus | Encountered a Exception in Game_OnTick" + e.Message);
            }
        }

        /// <summary>
        /// Gets Q + AA Damage Totally Done to Target
        /// </summary>
        /// <param name="target">The Target</param>
        /// <returns>Damage Totally Done to Target</returns>
        public static double GetDamage(Obj_AI_Base target)
        {
            double dmgItem = 0;

            if (Item.HasItem(3057) && (Item.CanUseItem(3057) || Player.HasBuff("sheen"))
                && PlayerInstance.BaseAttackDamage > dmgItem)
            {
                dmgItem = PlayerInstance.GetAutoAttackDamage(target);
            }

            if (Item.HasItem(3025) && (Item.CanUseItem(3025) || Player.HasBuff("itemfrozenfist"))
                && PlayerInstance.BaseAttackDamage * 1.25 > dmgItem)
            {
                dmgItem = PlayerInstance.GetAutoAttackDamage(target) * 1.25;
            }

            return target.CalculateDamageOnUnit(target, DamageType.Mixed, new float[] { 0, 30, 50, 70, 90, 110 }[Q.Level] + PlayerInstance.FlatPhysicalDamageMod + PlayerInstance.GetBuffCount("NasusQStacks")) + PlayerInstance.GetAutoAttackDamage(target) + dmgItem;
        }
    }
}
