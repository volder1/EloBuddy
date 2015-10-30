namespace WardBuddy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.Sandbox;

    using SharpDX;

    /// <summary>
    /// FileHandler by KarmaPanda
    /// </summary>
    internal class FileHandler
    {
        #region Fields

        #region Normal Wards

        /// <summary>
        /// List of the Position of Normal Wards
        /// </summary>
        public static List<Vector3> NormalPosition = new List<Vector3>();

        /// <summary>
        /// The root of the folder.
        /// </summary>
        private static readonly string WardBuddy = SandboxConfig.DataDirectory + @"\WardBuddy\";

        /// <summary>
        /// File Location for Normal Wards for X
        /// </summary>
        private static string normalxFile = WardBuddy + Game.MapId + @"\" + "normalxFile" + ".txt";

        /// <summary>
        /// File Location for Normal Wards for Y
        /// </summary>
        private static string normalyFile = WardBuddy + Game.MapId + @"\" + "normalyFile" + ".txt";

        /// <summary>
        /// File Location for Normal Wards for Z
        /// </summary>
        private static string normalzFile = WardBuddy + Game.MapId + @"\" + "normalzFile" + ".txt";

        /// <summary>
        /// Array of X String
        /// </summary>
        private static string[] normalxString;

        /// <summary>
        /// Array of Z String
        /// </summary>
        private static string[] normalzString;

        /// <summary>
        /// Array of Y String
        /// </summary>
        private static string[] normalyString;

        /// <summary>
        /// Array of X Integer
        /// </summary>
        private static int[] normalxInt;

        /// <summary>
        /// Array of Z Integer
        /// </summary>
        private static int[] normalzInt;

        /// <summary>
        /// Array of Y Integer
        /// </summary>
        private static int[] normalyInt;

        #endregion

        #region Pink Wards

        /// <summary>
        /// List of the Position of Pink Wards
        /// </summary>
        public static List<Vector3> PinkPosition = new List<Vector3>();

        /// <summary>
        /// File Location for Pink Wards for X
        /// </summary>
        private static string pinkxFile = WardBuddy + Game.MapId + @"\" + "pinkxFile" + ".txt";

        /// <summary>
        /// File Location for Pink Wards for Y
        /// </summary>
        private static string pinkyFile = WardBuddy + Game.MapId + @"\" + "pinkyFile" + ".txt";

        /// <summary>
        /// File Location for Pink Wards for Z
        /// </summary>
        private static string pinkzFile = WardBuddy + Game.MapId + @"\" + "pinkzFile" + ".txt";

        /// <summary>
        /// Array of X String
        /// </summary>
        private static string[] pinkxString;

        /// <summary>
        /// Array of Z String
        /// </summary>
        private static string[] pinkzString;

        /// <summary>
        /// Array of Y String
        /// </summary>
        private static string[] pinkyString;

        /// <summary>
        /// Array of X Integer
        /// </summary>
        private static int[] pinkxInt;

        /// <summary>
        /// Array of Z Integer
        /// </summary>
        private static int[] pinkzInt;

        /// <summary>
        /// Array of Y Integer
        /// </summary>
        private static int[] pinkyInt;

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="FileHandler"/> class. 
        /// </summary>
        public FileHandler()
        {
            DoChecks();
        }

        /// <summary>
        /// Checks for missing files, Converts the values to integer, then adds them into a Vector3 List
        /// </summary>
        private static void DoChecks()
        {
            #region Check Missing Files

            if (!Directory.Exists(WardBuddy))
            {
                Directory.CreateDirectory(WardBuddy);
                Directory.CreateDirectory(WardBuddy + GameMapId.SummonersRift);
                CreateFile();
            }
            else if (!File.Exists(normalxFile) 
                || !File.Exists(normalzFile) 
                || !File.Exists(normalyFile)
                || !File.Exists(pinkxFile)
                || !File.Exists(pinkzFile)
                || !File.Exists(pinkyFile))
            {
                CreateFile();
            }
            else if (File.Exists(normalxFile) && File.Exists(normalzFile) && File.Exists(normalyFile)
                && File.Exists(pinkxFile) && File.Exists(pinkzFile) && File.Exists(pinkyFile))
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

            #region Normal Ward

            if (!File.Exists(normalxFile))
            {
                File.WriteAllText(normalxFile, "0");
            }

            if (!File.Exists(normalyFile))
            {
                File.WriteAllText(normalyFile, "0");
            }

            if (!File.Exists(normalzFile))
            {
                File.WriteAllText(normalzFile, "0");
            }

            #endregion

            #region Pink Ward

            if (!File.Exists(pinkxFile))
            {
                File.WriteAllText(pinkxFile, "0");
            }

            if (!File.Exists(pinkyFile))
            {
                File.WriteAllText(pinkyFile, "0");
            }

            if (!File.Exists(pinkzFile))
            {
                File.WriteAllText(pinkzFile, "0");
            }

            #endregion

            DoChecks();

            #endregion
        }

        /// <summary>
        /// Gets the location of the shroom and adds it to the list
        /// </summary>
        public static void GetWardLocation()
        {
            #region Get Location for Normal Wards

            for (var i = 0; i < normalxInt.Count() && i < normalyInt.Count() && i < normalzInt.Count(); i++)
            {
                NormalPosition.Add(new Vector3(normalxInt[i], normalyInt[i], normalzInt[i]));
            }

            #endregion

            #region Get Location for Pink Wards

            for (var i = 0; i < pinkxInt.Count() && i < pinkyInt.Count() && i < pinkzInt.Count(); i++)
            {
                PinkPosition.Add(new Vector3(pinkxInt[i], pinkyInt[i], pinkzInt[i]));
            }

            #endregion
        }

        /// <summary>
        /// Converts String to Integer
        /// </summary>
        private static void ConvertToInt()
        {
            #region Convert to Int

            #region Normal Wards

            normalxString = new string[File.ReadAllLines(normalxFile).Count()];
            normalyString = new string[File.ReadAllLines(normalyFile).Count()];
            normalzString = new string[File.ReadAllLines(normalzFile).Count()];

            normalxInt = new int[File.ReadAllLines(normalxFile).Count()];
            normalyInt = new int[File.ReadAllLines(normalyFile).Count()];
            normalzInt = new int[File.ReadAllLines(normalzFile).Count()];

            normalxString = File.ReadAllLines(normalxFile);
            normalyString = File.ReadAllLines(normalyFile);
            normalzString = File.ReadAllLines(normalzFile);

            for (var i = 0; i < normalxString.Count(); i++)
            {
                normalxInt[i] = Convert.ToInt32(normalxString[i]);
            }

            for (var i = 0; i < normalxString.Count(); i++)
            {
                normalzInt[i] = Convert.ToInt32(normalzString[i]);
            }

            for (var i = 0; i < normalxString.Count(); i++)
            {
                normalyInt[i] = Convert.ToInt32(normalyString[i]);
            }

            #endregion

            #region Pink Wards

            pinkxString = new string[File.ReadAllLines(pinkxFile).Count()];
            pinkyString = new string[File.ReadAllLines(pinkyFile).Count()];
            pinkzString = new string[File.ReadAllLines(pinkzFile).Count()];

            pinkxInt = new int[File.ReadAllLines(pinkxFile).Count()];
            pinkyInt = new int[File.ReadAllLines(pinkyFile).Count()];
            pinkzInt = new int[File.ReadAllLines(pinkzFile).Count()];

            pinkxString = File.ReadAllLines(pinkxFile);
            pinkyString = File.ReadAllLines(pinkyFile);
            pinkzString = File.ReadAllLines(pinkzFile);

            for (var i = 0; i < pinkxString.Count(); i++)
            {
                pinkxInt[i] = Convert.ToInt32(pinkxString[i]);
            }

            for (var i = 0; i < normalxString.Count(); i++)
            {
                pinkzInt[i] = Convert.ToInt32(pinkzString[i]);
            }

            for (var i = 0; i < normalxString.Count(); i++)
            {
                pinkyInt[i] = Convert.ToInt32(pinkyString[i]);
            }

            #endregion

            GetWardLocation();

            #endregion
        }

        #endregion
    }
}