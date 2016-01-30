using System;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace LelBlanc
{
    class Pet
    {
        private static Vector3 _randomPosition;
        
        /// <summary>
        /// Delay for Movement and Attacking
        /// </summary>
        private static int _humanizedDelay;

        /// <summary>
        /// Method to Move Pet
        /// </summary>
        /// <param name="args"></param>
        public static void MovePet()
        {
            if (Player.Instance.Pet == null) return;

            _humanizedDelay = new Random().Next(500);
            var enemiesAroundPet = Player.Instance.Pet.CountEnemiesInRange(Player.Instance.AttackRange);
            var randX = new Random().Next(0, 200);
            var randY = new Random().Next(0, 200);

            if (enemiesAroundPet > 1)
            {
                var target = TargetSelector.GetTarget(Player.Instance.AttackRange, DamageType.Physical, Player.Instance.Pet.Position);

                if (target != null && Player.Instance.Pet.IsInRange(target.Position.To2D(), Player.Instance.AttackRange))
                    Core.DelayAction(() => Player.IssueOrder(GameObjectOrder.AutoAttackPet, target), _humanizedDelay);
            }
            else
            {
                _randomPosition = new Vector3(Player.Instance.Position.X + randX, Player.Instance.Position.Y + randY, Player.Instance.Position.Z);
                Core.DelayAction(() => Player.IssueOrder(GameObjectOrder.MovePet, _randomPosition), _humanizedDelay);
            }
        }
    }
}
