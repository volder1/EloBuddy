namespace WardBuddy
{
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;

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
        /// Initializes the Ward Location class.
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
            if (Game.MapId == GameMapId.SummonersRift)
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
                    this.Normal.Add(new Vector3(8180f, 5194f, 52.64763f));
                    this.Normal.Add(new Vector3(6496f, 4676f, 48.5272f));
                    this.Normal.Add(new Vector3(3416f, 7706f, 52.13334f));
                    this.Normal.Add(new Vector3(2340f, 9692f, 54.17554f));
                    this.Normal.Add(new Vector3(4710f, 10000f, -71.23711f));
                    this.Normal.Add(new Vector3(6850f, 9662f, 54.40515f));
                    this.Normal.Add(new Vector3(6868f, 11398f, 53.82961f));
                    this.Normal.Add(new Vector3(8286f, 10182f, 50.06982f));
                    this.Normal.Add(new Vector3(3074f, 10784f, -70.27567f));
                    this.Normal.Add(new Vector3(4484f, 11804f, 56.8484f));
                    this.Normal.Add(new Vector3(2364f, 13474f, 52.8381f));
                    this.Normal.Add(new Vector3(1136f, 12322f, 52.8381f));
                    this.Normal.Add(new Vector3(4460f, 11794f, 56.8484f));
                    this.Normal.Add(new Vector3(9982f, 7730f, 51.75227f));
                    this.Normal.Add(new Vector3(11450f, 7212f, 51.7251f));
                    this.Normal.Add(new Vector3(12546f, 5192f, 51.7294f));
                    this.Normal.Add(new Vector3(7800f, 3566f, 52.53794f));

                    // Summoner's Rift Pink Ward Locations
                    this.Pink.Add(new Vector3(9434f, 5614f, -71.2406f));
                    this.Pink.Add(new Vector3(10144f, 4796f, -71.2406f));
                    this.Pink.Add(new Vector3(10424f, 3086f, 50.59349f));
                    this.Pink.Add(new Vector3(3488f, 9084f, 49.50671f));
                    this.Pink.Add(new Vector3(5230f, 9074f, -71.2406f));
                }
            }
            else
            {
            }
        }
    }
}
