using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace LelBlanc
{
    class Config
    {
        /// <summary>
        /// Contains all the Menu's
        /// </summary>
        public static Menu ConfigMenu, ComboMenu, HarassMenu, LaneClearMenu, JungleClearMenu, KillStealMenu, DrawingMenu, MiscMenu;

        /// <summary>
        /// Creates the Menu
        /// </summary>
        public static void Initialize()
        {
            ConfigMenu = MainMenu.AddMenu("LelBlanc", "LelBlanc");
            ConfigMenu.AddGroupLabel("This addon is made by KarmaPanda and should not be redistributed in any way.");
            ConfigMenu.AddGroupLabel("Any unauthorized redistribution without credits will result in severe consequences.");
            ConfigMenu.AddGroupLabel("Thank you for using this addon and have a fun time!");

            ComboMenu = ConfigMenu.AddSubMenu("Combo Menu", "cMenu");
            ComboMenu.AddLabel("Spell Settings");
            ComboMenu.Add("useQ", new CheckBox("Use Q"));
            ComboMenu.Add("useW", new CheckBox("Use W"));
            ComboMenu.Add("useE", new CheckBox("Use E"));
            ComboMenu.AddLabel("R Settings");
            ComboMenu.Add("useQR", new CheckBox("Use QR"));
            ComboMenu.Add("useWR", new CheckBox("Use WR"));
            ComboMenu.Add("useER", new CheckBox("Use ER"));

            HarassMenu = ConfigMenu.AddSubMenu("Harass Menu", "hMenu");
            HarassMenu.AddLabel("Spell Settings");
            HarassMenu.Add("useQ", new CheckBox("Use Q"));
            HarassMenu.Add("useW", new CheckBox("Use W"));
            HarassMenu.Add("useE", new CheckBox("Use E"));
            HarassMenu.AddLabel("R Settings");
            HarassMenu.Add("useQR", new CheckBox("Use QR"));
            HarassMenu.Add("useWR", new CheckBox("Use WR"));
            HarassMenu.Add("useER", new CheckBox("Use ER"));

            LaneClearMenu = ConfigMenu.AddSubMenu("Laneclear Menu", "lcMenu");
            LaneClearMenu.AddLabel("Spell Settings");
            LaneClearMenu.Add("useQ", new CheckBox("Use Q", false));
            LaneClearMenu.Add("useW", new CheckBox("Use W"));
            LaneClearMenu.Add("sliderW", new Slider("Use W if Kill x Minions", 3, 1, 5));
            LaneClearMenu.AddLabel("R Settings");
            LaneClearMenu.Add("useQR", new CheckBox("Use QR", false));
            LaneClearMenu.Add("useWR", new CheckBox("Use WR"));
            LaneClearMenu.Add("sliderWR", new Slider("Use WR if Kill x Minions", 5, 1, 5));

            JungleClearMenu = ConfigMenu.AddSubMenu("Jungleclear Menu", "jcMenu");
            JungleClearMenu.AddLabel("Spell Settings");
            JungleClearMenu.Add("useQ", new CheckBox("Use Q"));
            JungleClearMenu.Add("useW", new CheckBox("Use W"));
            JungleClearMenu.Add("useE", new CheckBox("Use E"));
            JungleClearMenu.Add("sliderW", new Slider("Use W if Kill x Minions", 3, 1, 5));
            JungleClearMenu.AddLabel("R Settings");
            JungleClearMenu.Add("useQR", new CheckBox("Use QR"));
            JungleClearMenu.Add("useWR", new CheckBox("Use WR"));
            JungleClearMenu.Add("useER", new CheckBox("Use ER"));
            JungleClearMenu.Add("sliderWR", new Slider("Use WR if Kill x Minions", 5, 1, 5));

            KillStealMenu = ConfigMenu.AddSubMenu("Killsteal Menu", "ksMenu");
            KillStealMenu.AddLabel("Spell Settings");
            KillStealMenu.Add("useQ", new CheckBox("Use Q"));
            KillStealMenu.Add("useW", new CheckBox("Use W"));
            KillStealMenu.Add("useE", new CheckBox("Use E"));
            KillStealMenu.AddLabel("R Settings");
            KillStealMenu.Add("useQR", new CheckBox("Use QR"));
            KillStealMenu.Add("useWR", new CheckBox("Use WR"));
            KillStealMenu.Add("useER", new CheckBox("Use ER"));
            KillStealMenu.AddLabel("Misc Settings");
            KillStealMenu.Add("toggle", new CheckBox("Enable Kill Steal"));

            DrawingMenu = ConfigMenu.AddSubMenu("Drawing", "dMenu");
            DrawingMenu.AddLabel("Range Drawings");
            DrawingMenu.Add("drawQ", new CheckBox("Draw Q Range", false));
            DrawingMenu.Add("drawW", new CheckBox("Draw W Range", false));
            DrawingMenu.Add("drawE", new CheckBox("Draw E Range", false));
            DrawingMenu.AddLabel("DamageIndicator");
            DrawingMenu.Add("draw.Damage", new CheckBox("Draw Damage"));
            DrawingMenu.Add("draw.Q", new CheckBox("Calculate Q Damage"));
            DrawingMenu.Add("draw.W", new CheckBox("Calculate W Damage"));
            DrawingMenu.Add("draw.E", new CheckBox("Calculate E Damage"));
            DrawingMenu.Add("draw.R", new CheckBox("Calculate R Damage"));
            DrawingMenu.AddLabel("Color Settings for Damage Indicator");
            DrawingMenu.Add("draw_Alpha", new Slider("Alpha: ", 255, 0, 255));
            DrawingMenu.Add("draw_Red", new Slider("Red: ", 255, 0, 255));
            DrawingMenu.Add("draw_Green", new Slider("Green: ", 0, 0, 255));
            DrawingMenu.Add("draw_Blue", new Slider("Blue: ", 0, 0, 255));

            MiscMenu = ConfigMenu.AddSubMenu("Misc", "mMenu");
            MiscMenu.AddLabel("Miscellaneous");
            MiscMenu.Add("pet", new CheckBox("Automatic Clone Movement -- BROKEN", false));
            MiscMenu.AddLabel("Gapcloser Settings");
            MiscMenu.Add("gapCloser", new CheckBox("Use Gapcloser Combo (W -> Q -> R -> E)"));
            MiscMenu.Add("useQ", new CheckBox("Use Q"));
            MiscMenu.Add("useW", new CheckBox("Use W"));
            MiscMenu.Add("useE", new CheckBox("Use E"));
            MiscMenu.AddLabel("R Settings");
            MiscMenu.Add("useQR", new CheckBox("Use QR"));
            MiscMenu.Add("useWR", new CheckBox("Use WR"));
            MiscMenu.Add("useER", new CheckBox("Use ER"));
        }
    }
}
