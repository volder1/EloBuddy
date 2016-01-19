namespace TwistedBuddy
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    internal class Essentials
    {
        /// <summary>
        /// Menu
        /// </summary>
        public static Menu MainMenu, CardSelectorMenu, ComboMenu, LaneClearMenu, JungleClearMenu, HarassMenu, KillStealMenu, DrawingMenu, MiscMenu;

        /// <summary>
        /// Returns the card that should be picked in that scenario
        /// </summary>
        /// <param name="t">The Target</param>
        /// <returns>The Card that should be used.</returns>
        public static Cards MinionCardSelection(Obj_AI_Base t)
        {
            var card = Cards.None;
            var minionsaroundTarget =
                EntityManager.MinionsAndMonsters.EnemyMinions
                    .Count(
                        target => target.Distance(t) <= 200 &&
                                  target.Health <=
                                  DamageLibrary.PredictWDamage(target, Cards.Red));
            var monstersAroundTarget =
                EntityManager.MinionsAndMonsters.Monsters.Count(
                    mob => mob.Distance(t) <= 200 &&
                           mob.Health <= DamageLibrary.PredictWDamage(mob, Cards.Red));
            var enemyW = MiscMenu["enemyW"].Cast<Slider>().CurrentValue;
            var manaW = MiscMenu["manaW"].Cast<Slider>().CurrentValue;

            if (Player.Instance.ManaPercent < manaW)
            {
                card = Cards.Blue;
                return card;
            }

            if (Player.Instance.ManaPercent >= manaW && t.IsMonster && monstersAroundTarget < enemyW)
            {
                card = Cards.Yellow;
                return card;
            }

            if (Player.Instance.ManaPercent >= manaW && (minionsaroundTarget >= enemyW || monstersAroundTarget >= enemyW))
            {
                card = Cards.Red;
                return card;
            }

            return card;
        }

        /// <summary>
        /// Returns the card that should be picked in that scenario
        /// </summary>
        /// <param name="t">The Target</param>
        /// <returns>The Card that should be used.</returns>
        public static Cards HeroCardSelection(AIHeroClient t)
        {
            var card = Cards.None; 
            var alliesaroundTarget = t.CountEnemiesInRange(200);
            var enemyW = MiscMenu["enemyW"].Cast<Slider>().CurrentValue;
            var manaW = MiscMenu["manaW"].Cast<Slider>().CurrentValue;

            if (Player.Instance.ManaPercent <= manaW)
            {
                card = Cards.Blue;
                return card;
            }

            if (Player.Instance.ManaPercent > manaW && alliesaroundTarget >= enemyW)
            {
                card = Cards.Red;
                return card;
            }

            if (Player.Instance.ManaPercent > manaW && alliesaroundTarget < enemyW)
            {
                card = Cards.Yellow;
                return card;
            }

            return card;
        }
    }
}
