namespace WardBuddy
{
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    using SharpDX;

    internal class WardLocation
    {
        /// <summary>
        /// List of Locations in Summoner's Rift
        /// </summary>
        public List<Vector3> Normal = new List<Vector3>();

        /// <summary>
        /// List of Locations in Howling Abyss
        /// </summary>
        public List<Vector3> Pink = new List<Vector3>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WardTables"/> class.
        /// </summary>
        public WardLocation()
        {
            this.CreateTables();

            var list = (from pos in this.Normal
                        let x = pos.X
                        let y = pos.Y
                        let z = pos.Z
                        select new Vector3(x, y, z)).ToList();
            this.Normal = list;

            list = (from pos in this.Pink
                        let x = pos.X
                        let y = pos.Y
                        let z = pos.Z
                        select new Vector3(x, y, z)).ToList();
            this.Pink = list;
        }

        /// <summary>
        /// Creates a List of Shroom Locations around the map.
        /// </summary>
        private void CreateTables()
        {
            if (Game.MapId == EloBuddy.GameMapId.SummonersRift)
            {
                // Custom List
                if (Program.GetMenuValue(Program.FileHandlerMenu, "toggleC", "CheckBox"))
                {
                    if (FileHandler.NormalPosition.Any())
                    {
                        foreach (var pos in FileHandler.NormalPosition)
                        {
                            this.Normal.Add(pos);
                        }
                    }
                    if (FileHandler.PinkPosition.Any())
                    {
                        foreach (var pos in FileHandler.PinkPosition)
                        {
                            this.Pink.Add(pos);
                        }
                    }
                }
                 
                // Default List
                if (Program.GetMenuValue(Program.FileHandlerMenu, "toggleD", "CheckBox"))
                {
                    // Summoner's Rift Normal Ward Locations
                    this.Normal.Add(new Vector3(9918f, 6538f, 33.13258f));
                    this.Normal.Add(new Vector3(12504f, 1490f, 53.74172f));
                    this.Normal.Add(new Vector3(13312f, 2448f, 51.3669f));
                    this.Normal.Add(new Vector3(11852f, 3940f, -68.11531f));

                    // Summoner's Rift Pink Ward Locations
                    this.Pink.Add(new Vector3(9434f, 5614f, -71.2406f));
                    this.Pink.Add(new Vector3(10144f, 4796f, -71.2406f));
                    this.Pink.Add(new Vector3(10424f, 3086f, 50.59349f));
                }
            }
            else
            {
                return;
            }
        }
    }
}
