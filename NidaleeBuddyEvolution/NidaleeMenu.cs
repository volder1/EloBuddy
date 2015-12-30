using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace NidaleeBuddyEvolution
{
    internal class NidaleeMenu
    {
        /// <summary>
        /// Stores Menus
        /// </summary>
        public static Menu DefaultMenu,
            ComboMenu,
            LastHitMenu,
            HarassMenu,
            LaneClearMenu,
            JungleClearMenu,
            KillStealMenu,
            JungleStealMenu,
            DrawingMenu,
            MiscMenu;

        /// <summary>
        /// Creates the Menu.
        /// </summary>
        public static void Create()
        {
            DefaultMenu = MainMenu.AddMenu("NidaleeBuddyEvolution", "NidaleeBuddyEvolution");
            DefaultMenu.AddGroupLabel("This addon is made by KarmaPanda and should not be redistributed in any way.");
            DefaultMenu.AddGroupLabel(
                "Any unauthorized redistribution without credits will result in severe consequences.");
            DefaultMenu.AddGroupLabel("Thank you for using this addon and have a fun time!");

            #region Combo

            ComboMenu = DefaultMenu.AddSubMenu("Combo", "Combo");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.Add("useQH", new CheckBox("Cast Q in Human Form"));
            ComboMenu.Add("useWH", new CheckBox("Cast W in Human Form"));
            ComboMenu.Add("useQC", new CheckBox("Cast Q in Cougar Form"));
            ComboMenu.Add("useWC", new CheckBox("Cast W in Cougar Form"));
            ComboMenu.Add("useEC", new CheckBox("Cast E in Cougar Form"));
            ComboMenu.Add("useR", new CheckBox("Cast R during Combo"));
            ComboMenu.AddLabel("Prediction Settings - Human Form");
            ComboMenu.Add("predQH", new Slider("Cast Q if HitChance % is x", 75));
            ComboMenu.Add("predWH", new Slider("Cast W if HitChance % is x", 75));
            ComboMenu.AddLabel("Prediction Settings - Cougar Form");
            ComboMenu.Add("predWC", new Slider("Cast W if HitChance % is x", 75));
            ComboMenu.Add("predEC", new Slider("Cast E if HitChance % is x", 75));

            #endregion

            #region Last Hit

            LastHitMenu = DefaultMenu.AddSubMenu("Last Hit", "Last Hit");
            LastHitMenu.AddGroupLabel("Last Hit Settings");
            LastHitMenu.Add("useQC", new CheckBox("Cast Q in Cougar Form on Unkillable Minion"));
            LastHitMenu.Add("useEC", new CheckBox("Cast E in Cougar Form on Unkillable Minion", false));
            LastHitMenu.Add("useR", new CheckBox("Cast R in Cougar Form if Out of Range"));

            #endregion

            #region Harass

            HarassMenu = DefaultMenu.AddSubMenu("Harass", "Harass");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("useQH", new CheckBox("Cast Q in Human Form"));
            HarassMenu.Add("useR", new CheckBox("Cast R to force Human Form"));
            HarassMenu.AddLabel("Prediction Settings - Human Form");
            HarassMenu.Add("predQH", new Slider("Cast Q if HitChance % is x", 75));

            #endregion

            #region Kill Steal

            KillStealMenu = DefaultMenu.AddSubMenu("Kill Steal", "Kill Steal");
            KillStealMenu.AddGroupLabel("KillSteal Settings");
            KillStealMenu.Add("useQH", new CheckBox("Cast Q to Kill Steal"));
            KillStealMenu.Add("predQH", new Slider("Cast Q if HitChance % is x", 75));
            KillStealMenu.Add("useIgnite", new CheckBox("Use Ignite", false));

            #endregion

            #region Lane Clear

            LaneClearMenu = DefaultMenu.AddSubMenu("Lane Clear", "Lane Clear");
            LaneClearMenu.AddGroupLabel("Lane Clear Settings");
            LaneClearMenu.Add("useQC", new CheckBox("Cast Q in Cougar Form"));
            LaneClearMenu.Add("useWC", new CheckBox("Cast W in Cougar Form"));
            LaneClearMenu.Add("useEC", new CheckBox("Cast E in Cougar Form"));
            LaneClearMenu.Add("useR", new CheckBox("Cast R during Lane Clear", false));
            LaneClearMenu.AddLabel("Farm Settings - Cougar Form");
            LaneClearMenu.Add("predWC", new Slider("Cast W if it hits x minions", 1, 1, 7));
            LaneClearMenu.Add("predEC", new Slider("Cast E if HitChance % is x", 75));

            #endregion

            #region Jungle Clear

            JungleClearMenu = DefaultMenu.AddSubMenu("Jungle Clear", "Jungle Clear");
            JungleClearMenu.AddGroupLabel("Jungle Clear Settings");
            JungleClearMenu.Add("useQH", new CheckBox("Cast Q in Human Form"));
            JungleClearMenu.Add("useQC", new CheckBox("Cast Q in Cougar Form"));
            JungleClearMenu.Add("useWC", new CheckBox("Cast W in Cougar Form"));
            JungleClearMenu.Add("useEC", new CheckBox("Cast E in Cougar Form"));
            JungleClearMenu.Add("useR", new CheckBox("Cast R during Jungle Clear"));
            JungleClearMenu.AddLabel("Prediction Settings");
            JungleClearMenu.Add("predQH", new Slider("Cast Q in Human Form if HitChance % is x", 75));
            JungleClearMenu.Add("predWC", new Slider("Cast W in Cougar Form if HitChance % is x", 75));
            JungleClearMenu.Add("predEC", new Slider("Cast E in Cougar Form if HitChance % is x", 75));

            #endregion

            #region Jungle Steal

            JungleStealMenu = DefaultMenu.AddSubMenu("Jungle Steal", "Jungle Steal");
            JungleStealMenu.AddGroupLabel("Jungle Steal Settings");
            JungleStealMenu.Add("useQH", new CheckBox("Cast Q to Steal Jungle"));
            JungleStealMenu.Add("predQH", new Slider("Cast Q if HitChance % is x", 75));
            JungleStealMenu.Add("useSmite", new CheckBox("Cast Smite to Steal Jungle"));
            JungleStealMenu.Add("toggleK", new KeyBind("Toggle Smite", true, KeyBind.BindTypes.PressToggle, 'M'));
            JungleStealMenu.AddGroupLabel("Jungle Camp Toggle");
            switch (Game.MapId)
            {
                case GameMapId.SummonersRift:
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
                    break;
                case GameMapId.TwistedTreeline:
                    JungleStealMenu.AddLabel("Epics");
                    JungleStealMenu.Add("TT_Spiderboss8.1", new CheckBox("Vilemaw"));
                    JungleStealMenu.AddLabel("Camps");
                    JungleStealMenu.Add("TT_NWraith1.1", new CheckBox("Wraith"));
                    JungleStealMenu.Add("TT_NWraith4.1", new CheckBox("Wraith"));
                    JungleStealMenu.Add("TT_NGolem2.1", new CheckBox("Golem"));
                    JungleStealMenu.Add("TT_NGolem5.1", new CheckBox("Golem"));
                    JungleStealMenu.Add("TT_NWolf3.1", new CheckBox("Wolf"));
                    JungleStealMenu.Add("TT_NWolf6.1", new CheckBox("Wolf"));
                    break;
            }

            #endregion

            #region Drawing

            DrawingMenu = DefaultMenu.AddSubMenu("Drawing", "Drawing");
            DrawingMenu.AddGroupLabel("Drawing Settings");
            DrawingMenu.Add("drawQH", new CheckBox("Draw Javelin Range"));
            DrawingMenu.Add("drawPred", new CheckBox("Draw Javelin Prediction"));
            DrawingMenu.AddLabel("DamageIndicator");
            DrawingMenu.Add("draw.Damage", new CheckBox("Draw Damage"));
            DrawingMenu.Add("draw.Q", new CheckBox("Calculate Q Damage"));
            DrawingMenu.Add("draw.W", new CheckBox("Calculate W Damage"));
            DrawingMenu.Add("draw.E", new CheckBox("Calculate E Damage"));
            DrawingMenu.Add("draw.R", new CheckBox("Calculate R Damage", false));
            DrawingMenu.AddLabel("Color Settings for Damage Indicator");
            DrawingMenu.Add("draw_Alpha", new Slider("Alpha: ", 255, 0, 255));
            DrawingMenu.Add("draw_Red", new Slider("Red: ", 255, 0, 255));
            DrawingMenu.Add("draw_Green", new Slider("Green: ", 0, 0, 255));
            DrawingMenu.Add("draw_Blue", new Slider("Blue: ", 0, 0, 255));

            #endregion

            #region Misc

            MiscMenu = DefaultMenu.AddSubMenu("Misc Menu", "Misc Menu");
            MiscMenu.AddGroupLabel("Auto Heal Settings");
            MiscMenu.Add("autoHeal", new CheckBox("Auto Heal Allies and Me"));
            MiscMenu.Add("autoHealPercent", new Slider("Auto Heal Percent", 50));

            foreach (var a in EntityManager.Heroes.Allies.OrderBy(a => a.BaseSkinName))
            {
                MiscMenu.Add("autoHeal_" + a.BaseSkinName, new CheckBox("Auto Heal " + a.BaseSkinName));
            }

            MiscMenu.AddGroupLabel("Spell Settings");
            MiscMenu.AddLabel("Only choose one of them below.");
            MiscMenu.Add("useQC_AfterAttack", new CheckBox("Cast Q in Cougar Form After Attack"));
            MiscMenu.Add("useQC_BeforeAttack", new CheckBox("Cast Q in Cougar Form Before Attack", false));
            MiscMenu.Add("useQC_OnUpdate", new CheckBox("Cast Q in Cougar Form on Update", false));
            MiscMenu.AddGroupLabel("ManaManager");
            MiscMenu.Add("manaQ", new Slider("Use Q in Human Form only if Mana Percent is >= x", 25));
            MiscMenu.Add("manaW", new Slider("Use W in Human Form only if Mana Percent is >= x", 25));
            MiscMenu.Add("manaE", new Slider("Use E in Human Form only if Mana Percent is >= x", 25));
            MiscMenu.Add("disableMM", new CheckBox("Disable ManaManager in Combo Mode"));

            #endregion
        }
    }
}
