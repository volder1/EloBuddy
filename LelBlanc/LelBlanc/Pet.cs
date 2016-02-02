using System;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace LelBlanc
{
    class Pet
    {
        /// <summary>
        /// The Pet Instance
        /// </summary>
        public static GameObject LeBlancPet { get; set; }

        /// <summary>
        /// The Path of the Player
        /// </summary>
        public static Vector3 NewPath { get; set; }

        /// <summary>
        /// Delay for Movement
        /// </summary>
        public static int HumanizedDelay { get; set; }

        /// <summary>
        /// Method to Move Pet
        /// </summary>
        public static void MovePet()
        {
            if (LeBlancPet == null)
            {
                return;
            }

            Core.DelayAction(() =>
            {
                Player.IssueOrder(GameObjectOrder.MovePet, CalculatePosition(Player.Instance, NewPath));
            }, HumanizedDelay);
        }

        /// <summary>
        /// Calculates Reflected Position. Returns no Vector if it is a wall, building, or prop
        /// </summary>
        /// <param name="source">The Player or Position Being Reflected</param>
        /// <param name="newPath">The Path</param>
        /// <returns></returns>
        private static Vector3 CalculatePosition(Obj_AI_Base source, Vector3 path)
        {
            var playerPosition2D = source.Position.To2D();
            var pathPosition2D = path.To2D();
            var reflectedPos = Vector2.Reflect(pathPosition2D, playerPosition2D).To3D();

            return reflectedPos;
        }
    }
}
