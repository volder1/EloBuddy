namespace WardBuddy
{
    using System;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    using SharpDX;

    using Color = System.Drawing.Color;

    internal class Program
    {
        /// <summary>
        /// Checks if the position is warded
        /// </summary>
        /// <param name="position">The Position</param>
        /// <returns>If the position is warded.</returns>
        private static bool IsWarded(Vector3 position)
        {
            return ObjectManager.Get<Obj_AI_Base>().Any(obj => obj.IsWard() && obj.Distance(position) <= 200);
        }

        /// <summary>
        /// Gets the Value of a CheckBox or KeyBind
        /// </summary>
        /// <param name="menu">The menu you want to fetch</param>
        /// <param name="item">The item you want to fetch</param>
        /// <param name="type">Is it a CheckBox or KeyBind</param>
        /// <returns>The value of the CheckBox or KeyBind.</returns>
        public static bool GetMenuValue(Menu menu, string item, string type)
        {
            try
            {
                switch (type)
                {
                    case "CheckBox":
                        return menu[item].Cast<CheckBox>().CurrentValue;
                    case "KeyBind":
                        return menu[item].Cast<KeyBind>().CurrentValue;
                    default:
                        return false;
                }
            }
            catch(Exception e)
            {
                Chat.Print("WardBuddy|Exception Occured while trying to get menu value: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets the Value of a Slider Menu
        /// </summary>
        /// <param name="menu">The menu you want to fetch</param>
        /// <param name="item">The item you want to fetch</param>
        /// <returns>The value of the Slider.</returns>
        public static int GetMenuValue(Menu menu, string item)
        {
            try
            {
                return menu[item].Cast<Slider>().CurrentValue;
            }
            catch (Exception e)
            {
                Chat.Print("WardBuddy|Exception Occured while trying to get menu value: " + e.Message);
                return 0;
            }
        }

        /// <summary>
        /// Initializes the Ward Locations
        /// </summary>
        private static WardLocation wardLocation;

        /// <summary>
        /// Initializes the FileHandler
        /// </summary>
        public static FileHandler Handler { get; private set; }

        /// <summary>
        /// Initializes the Menu
        /// </summary>
        public static Menu WardBuddy, FileHandlerMenu, WardMenu, DrawingMenu;

        /// <summary>
        /// Initializes the Items
        /// </summary>
        private static Item sWard, vWard, sightStone, rSightStone, trinket, gsT, gvT;

        /// <summary>
        /// Gets the Ward Range
        /// </summary>
        private const float WardRange = 1000f;

        /// <summary>
        /// Gets the Last Time a Ward was Placed.
        /// </summary>
        private static int time;

        /// <summary>
        /// Gets the player.
        /// </summary>
        private static AIHeroClient PlayerInstance
        {
            get { return Player.Instance; }
        }

        /// <summary>
        /// Called when Program Initializes
        /// </summary>
        private static void Main()
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        /// <summary>
        /// Called when Game finishes loading.
        /// </summary>
        /// <param name="args">The Loading Args</param>
        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Game.MapId != GameMapId.SummonersRift)
            {
                return;
            }

            try
            {
                sWard = new Item(2044, WardRange);
                vWard = new Item(2043, WardRange);
                sightStone = new Item(2049, WardRange);
                rSightStone = new Item(2045, WardRange);
                trinket = new Item(3340, WardRange);
                gsT = new Item(3361, WardRange);
                gvT = new Item(3362, WardRange);

                WardBuddy = MainMenu.AddMenu("WardBuddy", "WardBuddy");

                FileHandlerMenu = WardBuddy.AddSubMenu("FileHandler", "FileHandler");
                FileHandlerMenu.AddGroupLabel("FileHandler Settings");
                FileHandlerMenu.AddSeparator();
                FileHandlerMenu.Add("toggleC", new CheckBox("Use Custom Locations"));
                FileHandlerMenu.Add("toggleD", new CheckBox("Use Default Locations"));
                
                WardMenu = WardBuddy.AddSubMenu("Ward", "Ward");
                WardMenu.AddGroupLabel("Ward Settings");
                WardMenu.AddSeparator();
                WardMenu.Add("normal", new CheckBox("Use Normal Ward"));
                WardMenu.Add("pink", new CheckBox("Use Pink Ward"));
                WardMenu.AddSeparator();
                WardMenu.AddGroupLabel("How should the Ward be placed?");
                WardMenu.Add("always", new CheckBox("Always ward any position", false));
                WardMenu.Add("usekey", new CheckBox("Use keybind to ward nearest ward."));
                WardMenu.Add("key", new KeyBind("Place ward with keybind", false, KeyBind.BindTypes.HoldActive, "Z".ToCharArray()[0]));

                DrawingMenu = WardBuddy.AddSubMenu("Drawing", "Drawing");
                DrawingMenu.AddGroupLabel("Drawing Settings");
                DrawingMenu.AddSeparator();
                DrawingMenu.Add("normal", new CheckBox("Draw Normal Ward Positions"));
                DrawingMenu.Add("pink", new CheckBox("Draw Pink Ward Positions"));
                DrawingMenu.AddSeparator();
                DrawingMenu.AddGroupLabel("Debug Settings");
                DrawingMenu.Add("text", new CheckBox("Draw Player Coordinates"));
                DrawingMenu.Add("x", new Slider("X", 500, 0, 1920));
                DrawingMenu.Add("y", new Slider("Y", 500, 0, 1080));

                Chat.Print("WardBuddy Initialized by KarmaPanda");

                wardLocation = new WardLocation();
                Handler = new FileHandler();

                Game.OnTick += Game_OnTick;
                Drawing.OnDraw += Drawing_OnDraw;
            }
            catch (Exception e)
            {
                Chat.Print("Failed to Initialize WardBuddy. Exception: " + e.Message);
            }
        }

        /// <summary>
        /// Called when Game Updates
        /// </summary>
        /// <param name="args">The Args</param>
        private static void Game_OnTick(EventArgs args)
        {
            try
            {
                if (PlayerInstance.IsDead || PlayerInstance.HasBuff("Recall"))
                {
                    return;
                }

                if (GetMenuValue(WardMenu, "normal", "CheckBox"))
                {
                    if (GetMenuValue(WardMenu, "always", "CheckBox")
                        || GetMenuValue(WardMenu, "usekey", "CheckBox") && GetMenuValue(WardMenu, "key", "KeyBind"))
                    {
                        foreach (var place in wardLocation.Normal.Where(pos => pos.Distance(ObjectManager.Player.Position) <= 1000))
                        {
                            if (trinket.IsOwned(PlayerInstance) && trinket.IsReady() && !IsWarded(place))
                            {
                                trinket.Cast(place);
                                time = Environment.TickCount + 5000;
                            }
                            if (sightStone.IsOwned(PlayerInstance) && sightStone.IsReady() && !IsWarded(place) && (Environment.TickCount > time))
                            {
                                sightStone.Cast(place);
                                time = Environment.TickCount + 5000;
                            }
                            if (rSightStone.IsOwned(PlayerInstance) && rSightStone.IsReady() && !IsWarded(place) && (Environment.TickCount > time))
                            {
                                rSightStone.Cast(place);
                                time = Environment.TickCount + 5000;
                            }
                            if (gsT.IsOwned(PlayerInstance) && gsT.IsReady() && !IsWarded(place) && (Environment.TickCount > time)) 
                            {
                                gsT.Cast(place);
                                time = Environment.TickCount + 5000;
                            }
                            if (!sWard.IsOwned(PlayerInstance) || !sWard.IsReady() || IsWarded(place)
                                || (Environment.TickCount <= time))
                            {
                                continue;
                            }
                            sWard.Cast(place);
                            time = Environment.TickCount + 5000;
                        }
                    }
                }

                if (!GetMenuValue(WardMenu, "pink", "CheckBox"))
                {
                    return;
                }
                if (!GetMenuValue(WardMenu, "always", "CheckBox")
                    && (!GetMenuValue(WardMenu, "usekey", "CheckBox") || !GetMenuValue(WardMenu, "key", "KeyBind")))
                {
                    return;
                }
                foreach (var place in wardLocation.Pink.Where(pos => pos.Distance(ObjectManager.Player.Position) <= 1000))
                {
                    if (vWard.IsOwned(PlayerInstance) && vWard.IsReady() && !IsWarded(place))
                    {
                        vWard.Cast(place);
                        time = Environment.TickCount + 5000;
                    }
                    if (!gvT.IsOwned(PlayerInstance) || !gvT.IsReady() || IsWarded(place)
                        || (Environment.TickCount <= time))
                    {
                        continue;
                    }
                    gvT.Cast(place);
                    time = Environment.TickCount + 5000;
                }
            }
            catch (Exception e)
            {
                Chat.Print("WardBuddy|Exception Occured while trying to update: " + e.Message);
            }
        }

        /// <summary>
        /// Called whenever the Game Draws.
        /// </summary>
        /// <param name="args">The Args</param>
        private static void Drawing_OnDraw(EventArgs args)
        {
            try
            {
                if (Game.MapId != GameMapId.SummonersRift)
                {
                    return;
                }
                if (GetMenuValue(DrawingMenu, "text", "CheckBox"))
                {
                    Drawing.DrawText(new Vector2(
                        GetMenuValue(DrawingMenu, "x"),
                        GetMenuValue(DrawingMenu, "y")),
                        Color.Red,
                        PlayerInstance.Position.ToString(), 25);
                }

                if (GetMenuValue(DrawingMenu, "normal", "CheckBox") && wardLocation.Normal.Any())
                {
                    foreach (var place in wardLocation.Normal.Where(pos => pos.Distance(ObjectManager.Player.Position) <= 1500))
                    {
                        Drawing.DrawCircle(place, 100, IsWarded(place) ? Color.Red : Color.Green);
                    }
                }

                if (!GetMenuValue(DrawingMenu, "pink", "CheckBox") || !wardLocation.Pink.Any())
                {
                    return;
                }
                foreach (var place in wardLocation.Pink.Where(pos => pos.Distance(ObjectManager.Player.Position) <= 1500))
                {
                    Drawing.DrawCircle(place, 100, IsWarded(place) ? Color.Red : Color.Pink);
                }
            }
            catch (Exception e)
            {
                Chat.Print("WardBuddy|Exception Occured while trying to draw: " + e.Message);
            }
        }
    }
}
