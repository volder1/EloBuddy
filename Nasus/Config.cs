using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Nasus
{
    internal class Config
    {
        /// <summary>
        /// Initializes the Menu
        /// </summary>
        public static Menu ConfigMenu, FarmMenu, ComboMenu, DrawingMenu;

        /// <summary>
        /// Creates the Menu
        /// </summary>
        public static void Initialize()
        {
            // Main Menu
            ConfigMenu = MainMenu.AddMenu("KA Nasus", "ConfigMenu");
            ConfigMenu.AddGroupLabel("This addon is made by KarmaPanda and should not be redistributed in any way.");
            ConfigMenu.AddGroupLabel(
                "Any unauthorized redistribution without credits will result in severe consequences.");
            ConfigMenu.AddGroupLabel("Thank you for using this addon and have a fun time!");

            // Farm Menu
            FarmMenu = ConfigMenu.AddSubMenu("Farm", "Farm");
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
            ComboMenu = ConfigMenu.AddSubMenu("Combo", "Combo");
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

            // Drawing Menu
            DrawingMenu = ConfigMenu.AddSubMenu("Drawing", "Drawing");
            DrawingMenu.AddGroupLabel("Spell Drawing Settings");
            DrawingMenu.Add("drawW", new CheckBox("Draw W Range", false));
            DrawingMenu.Add("drawE", new CheckBox("Draw E Range", false));
            DrawingMenu.AddLabel("DamageIndicator");
            DrawingMenu.Add("draw.Damage", new CheckBox("Draw Damage"));
            DrawingMenu.Add("draw.Q", new CheckBox("Calculate Q Damage"));
            DrawingMenu.Add("draw.E", new CheckBox("Calculate E Damage"));
            DrawingMenu.AddLabel("Color Settings for Damage Indicator");
            DrawingMenu.Add("draw_Alpha", new Slider("Alpha: ", 255, 0, 255));
            DrawingMenu.Add("draw_Red", new Slider("Red: ", 255, 0, 255));
            DrawingMenu.Add("draw_Green", new Slider("Green: ", 0, 0, 255));
            DrawingMenu.Add("draw_Blue", new Slider("Blue: ", 0, 0, 255));
        }
    }
}