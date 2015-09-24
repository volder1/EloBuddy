namespace WardBuddy
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
        /// Checks if there is ward in location.
        /// </summary>
        /// <param name="position">The location to check.</param>
        /// <returns>If that location has a ward.</returns>
        private static bool IsWarded(Vector3 position)
        {
            return ObjectManager.Get<Obj_Ward>().Any(obj => position.Distance(obj.Position) <= 250);
        }

        /// <summary>
        /// Gets the Value of a CheckBox or KeyBind
        /// </summary>
        /// <param name="Menu">The menu you want to fetch</param>
        /// <param name="Item">The item you want to fetch</param>
        /// <param name="Type">Is it a CheckBox or KeyBind</param>
        /// <returns>The value of the CheckBox or KeyBind.</returns>
        public static bool GetMenuValue(Menu Menu, string Item, string Type)
        {
            try
            {
                switch (Type)
                {
                    case "CheckBox":
                        return Menu[Item].Cast<CheckBox>().CurrentValue;
                    case "KeyBind":
                        return Menu[Item].Cast<KeyBind>().CurrentValue;
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
        /// <param name="Menu">The menu you want to fetch</param>
        /// <param name="Item">The item you want to fetch</param>
        /// <returns>The value of the Slider.</returns>
        public static int GetMenuValue(Menu Menu, string Item)
        {
            try
            {
                return Menu[Item].Cast<Slider>().CurrentValue;
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
        private static FileHandler fileHandler;

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
        private static float wardRange = 1000f;

        /// <summary>
        /// Gets the Last Time a Ward was Placed.
        /// </summary>
        private static int Time;

        /// <summary>
        /// Gets the player.
        /// </summary>
        private static AIHeroClient PlayerInstance
        {
            get { return EloBuddy.Player.Instance; }
        }

        /// <summary>
        /// Called when Program Initializes
        /// </summary>
        /// <param name="args">The Program Args</param>
        private static void Main(string[] args)
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
                sWard = new Item(2044, wardRange);
                vWard = new Item(2043, wardRange);
                sightStone = new Item(2049, wardRange);
                rSightStone = new Item(2045, wardRange);
                trinket = new Item(3340, wardRange);
                gsT = new Item(3361, wardRange);
                gvT = new Item(3362, wardRange);

                WardBuddy = MainMenu.AddMenu("WardBuddy", "WardBuddy");

                FileHandlerMenu = WardBuddy.AddSubMenu("FileHandler", "FileHandler");
                FileHandlerMenu.AddGroupLabel("FileHandler Settings");
                FileHandlerMenu.AddSeparator();
                FileHandlerMenu.Add("toggleC", new CheckBox("Use Custom Locations", true));
                FileHandlerMenu.Add("toggleD", new CheckBox("Use Default Locations", true));
                
                WardMenu = WardBuddy.AddSubMenu("Ward", "Ward");
                WardMenu.AddGroupLabel("Ward Settings");
                WardMenu.AddSeparator();
                WardMenu.Add("normal", new CheckBox("Use Normal Ward", true));
                WardMenu.Add("pink", new CheckBox("Use Pink Ward", true));
                WardMenu.AddSeparator();
                WardMenu.AddGroupLabel("How should the Ward be placed?");
                WardMenu.Add("always", new CheckBox("Always ward any position", false));
                WardMenu.Add("usekey", new CheckBox("Use keybind to ward nearest ward.", true));
                WardMenu.Add("key", new KeyBind("Place ward with keybind", false, KeyBind.BindTypes.HoldActive, "Z".ToCharArray()[0]));

                DrawingMenu = WardBuddy.AddSubMenu("Drawing", "Drawing");
                DrawingMenu.AddGroupLabel("Drawing Settings");
                DrawingMenu.AddSeparator();
                DrawingMenu.Add("normal", new CheckBox("Draw Normal Ward Positions", true));
                DrawingMenu.Add("pink", new CheckBox("Draw Pink Ward Positions", true));
                DrawingMenu.AddSeparator();
                DrawingMenu.AddGroupLabel("Debug Settings");
                DrawingMenu.Add("text", new CheckBox("Draw Player Coordinates", true));
                DrawingMenu.Add("x", new Slider("X", 500, 0, 1920));
                DrawingMenu.Add("y", new Slider("Y", 500, 0, 1080));

                Chat.Print("WardBuddy Initialized");

                wardLocation = new WardLocation();
                fileHandler = new FileHandler();

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
                if (PlayerInstance.IsDead || PlayerInstance.IsRecalling)
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
                                Time = Environment.TickCount + 5000;
                            }
                            if (sightStone.IsOwned(PlayerInstance) && sightStone.IsReady() && !IsWarded(place) && (Environment.TickCount > Time))
                            {
                                sightStone.Cast(place);
                                Time = Environment.TickCount + 5000;
                            }
                            if (rSightStone.IsOwned(PlayerInstance) && rSightStone.IsReady() && !IsWarded(place) && (Environment.TickCount > Time))
                            {
                                rSightStone.Cast(place);
                                Time = Environment.TickCount + 5000;
                            }
                            if (gsT.IsOwned(PlayerInstance) && gsT.IsReady() && !IsWarded(place) && (Environment.TickCount > Time)) 
                            {
                                gsT.Cast(place);
                                Time = Environment.TickCount + 5000;
                            }
                            if (sWard.IsOwned(PlayerInstance) && sWard.IsReady() && !IsWarded(place) && (Environment.TickCount > Time))
                            {
                                sWard.Cast(place);
                                Time = Environment.TickCount + 5000;
                            }
                        }
                    }
                }

                if (GetMenuValue(WardMenu, "pink", "CheckBox"))
                {
                    if (GetMenuValue(WardMenu, "always", "CheckBox")
                        || GetMenuValue(WardMenu, "usekey", "CheckBox") && GetMenuValue(WardMenu, "key", "KeyBind"))
                    {
                        foreach (var place in wardLocation.Pink.Where(pos => pos.Distance(ObjectManager.Player.Position) <= 1000))
                        {
                            if (vWard.IsOwned(PlayerInstance) && vWard.IsReady() && !IsWarded(place))
                            {
                                vWard.Cast(place);
                                Time = Environment.TickCount + 5000;
                            }
                            if (gvT.IsOwned(PlayerInstance) && gvT.IsReady() && !IsWarded(place) && (Environment.TickCount > Time))
                            {
                                gvT.Cast(place);
                                Time = Environment.TickCount + 5000;
                            }
                        }
                    }
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
                if (Game.MapId == EloBuddy.GameMapId.SummonersRift)
                {
                    if (GetMenuValue(DrawingMenu, "text", "CheckBox"))
                    {
                        Drawing.DrawText(new Vector2(
                            (float)GetMenuValue(DrawingMenu, "x"),
                            (float)GetMenuValue(DrawingMenu, "y")),
                            System.Drawing.Color.Red,
                            PlayerInstance.Position.ToString(), 25);
                    }

                    if (GetMenuValue(DrawingMenu, "normal", "CheckBox") && wardLocation.Normal.Any())
                    {
                        foreach (var place in wardLocation.Normal.Where(pos => pos.Distance(ObjectManager.Player.Position) <= 1500))
                        {
                            EloBuddy.SDK.Rendering.Circle.Draw(SharpDX.Color.Green, 100, place);
                        }
                    }

                    if (GetMenuValue(DrawingMenu, "pink", "CheckBox") && wardLocation.Pink.Any())
                    {
                        foreach (var place in wardLocation.Pink.Where(pos => pos.Distance(ObjectManager.Player.Position) <= 1500))
                        {
                            EloBuddy.SDK.Rendering.Circle.Draw(SharpDX.Color.Pink, 100, place);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Chat.Print("WardBuddy|Exception Occured while trying to draw: " + e.Message);
            }
        }
    }
}
