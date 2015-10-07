namespace EloBuddy_PandaTeemo
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.Sandbox;
    using EloBuddy.SDK.Menu.Values;

    using SharpDX;

    /// <summary>
    /// Ported to EloBuddy
    /// </summary>
    internal class FileHandler
    {
        #region Fields

        /// <summary>
        /// List of the Position of the Shroom
        /// </summary>
        public static List<Vector3> Position = new List<Vector3>();

        /// <summary>
        /// The root of the folder.
        /// </summary>
        private static readonly string ShroomLocation = SandboxConfig.DataDirectory + @"\PandaTeemo\";

        /// <summary>
        /// File Location for X
        /// </summary>
        private static string xFile = ShroomLocation + Game.MapId + @"\" + "xFile" + ".txt";

        /// <summary>
        /// File Location for Y
        /// </summary>
        private static string yFile = ShroomLocation + Game.MapId + @"\" + "yFile" + ".txt";

        /// <summary>
        /// File Location for Z
        /// </summary>
        private static string zFile = ShroomLocation + Game.MapId + @"\" + "zFile" + ".txt";

        /// <summary>
        /// Array of X String
        /// </summary>
        private static string[] xString;

        /// <summary>
        /// Array of Z String
        /// </summary>
        private static string[] zString;

        /// <summary>
        /// Array of Y String
        /// </summary>
        private static string[] yString;

        /// <summary>
        /// Array of X Integer
        /// </summary>
        private static int[] xInt;

        /// <summary>
        /// Array of Z Integer
        /// </summary>
        private static int[] zInt;

        /// <summary>
        /// Array of Y Integer
        /// </summary>
        private static int[] yInt;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="FileHandler"/> class. 
        /// </summary>
        public FileHandler()
        {
            #region Initialize
            DoChecks();
            #endregion
        }

        /// <summary>
        /// Checks for missing files, Converts the values to integer, then adds them into a Vector3 List
        /// </summary>
        private static void DoChecks()
        {
            #region Check Missing Files

            if (!Directory.Exists(ShroomLocation))
            {
                Directory.CreateDirectory(ShroomLocation);
                Directory.CreateDirectory(ShroomLocation + GameMapId.SummonersRift);
                Directory.CreateDirectory(ShroomLocation + GameMapId.HowlingAbyss);
                Directory.CreateDirectory(ShroomLocation + GameMapId.SummonersRift);
                Directory.CreateDirectory(ShroomLocation + GameMapId.TwistedTreeline);
                CreateFile();
            }
            else if (!File.Exists(xFile) || !File.Exists(zFile) || !File.Exists(yFile))
            {
                CreateFile();
            }
            else if (File.Exists(xFile) && File.Exists(zFile) && File.Exists(yFile))
            {
                ConvertToInt();
            }

            #endregion
        }

        /// <summary>
        /// Creates Files that are missing
        /// </summary>
        private static void CreateFile()
        {
            #region Create File

            if (!File.Exists(xFile))
            {
                File.WriteAllText(xFile, "5020");
            }
            else if (!File.Exists(yFile))
            {
                File.WriteAllText(yFile, "8430");
            }
            else if (!File.Exists(zFile))
            {
                File.WriteAllText(zFile, "2");
            }

            DoChecks();

            #endregion
        }

        /// <summary>
        /// Gets the location of the shroom and adds it to the list
        /// </summary>
        public static void GetShroomLocation()
        {
            #region Get Location

            for (var i = 0; i < xInt.Count() && i < yInt.Count() && i < zInt.Count(); i++)
            {
                Position.Add(new Vector3(xInt[i], zInt[i], yInt[i]));
                if (Program.Debug["debugpos"].Cast<CheckBox>().CurrentValue)
                {
                    Chat.Print(Position[i].ToString());
                }
            }

            #endregion
        }

        /// <summary>
        /// Converts String to Integer
        /// </summary>
        private static void ConvertToInt()
        {
            #region Convert to Int

            xString = new string[File.ReadAllLines(xFile).Count()];
            yString = new string[File.ReadAllLines(yFile).Count()];
            zString = new string[File.ReadAllLines(zFile).Count()];

            xInt = new int[File.ReadAllLines(xFile).Count()];
            yInt = new int[File.ReadAllLines(yFile).Count()];
            zInt = new int[File.ReadAllLines(zFile).Count()];

            xString = File.ReadAllLines(xFile);
            yString = File.ReadAllLines(yFile);
            zString = File.ReadAllLines(zFile);

            for (var i = 0; i < xString.Count(); i++)
            {
                xInt[i] = Convert.ToInt32(xString[i]);
            }

            for (var i = 0; i < xString.Count(); i++)
            {
                zInt[i] = Convert.ToInt32(zString[i]);
            }

            for (var i = 0; i < xString.Count(); i++)
            {
                yInt[i] = Convert.ToInt32(yString[i]);
            }

            GetShroomLocation();

            if (Program.Debug["debugpos"].Cast<CheckBox>().CurrentValue)
            {
                Chat.Print("Sucessfully Initialized FileHandler");
            }

            #endregion
        }

        #endregion
    }
}