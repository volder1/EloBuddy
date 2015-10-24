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
        /// Returns the Player ManaPercent
        /// </summary>
        /// <returns>Player's ManaPercent</returns>
        public static float ManaPercent()
        {
            return (Player.Instance.Mana / Player.Instance.MaxMana) * 100;
        }

        /// <summary>
        /// Returns the card that should be picked in that scenario
        /// </summary>
        /// <param name="t">The Target</param>
        /// <returns>The Card that should be used.</returns>
        public static string MinionCardSelection(Obj_AI_Base t)
        {
            string card;
            var minionsaroundTarget = ObjectManager.Get<Obj_AI_Minion>().Count(target => !target.IsAlly && target.IsMinion && target.Distance(t) <= 200);
            var enemyW = MiscMenu["enemyW"].Cast<Slider>().CurrentValue;
            var manaW = MiscMenu["manaW"].Cast<Slider>().CurrentValue;

            if (ManaPercent() <= manaW)
            {
                card = "Blue";
                return card;
            }

            if (ManaPercent() > manaW
                && t.Team == GameObjectTeam.Neutral
                && (t.Name == "SRU_Blue"
                    || t.Name == "SRU_Gromp"
                    || t.Name == "SRU_Murkwolf"
                    || t.Name == "SRU_Razorbeak"
                    || t.Name == "SRU_Red")
                && minionsaroundTarget < enemyW)
            {
                card = "Yellow";
                return card;
            }

            if (ManaPercent() > manaW && minionsaroundTarget >= enemyW)
            {
                card = "Red";
                return card;
            }
            return null;
        }

        /// <summary>
        /// Returns the card that should be picked in that scenario
        /// </summary>
        /// <param name="t">The Target</param>
        /// <returns>The Card that should be used.</returns>
        public static string HeroCardSelection(AIHeroClient t)
        {
            string card;
            var alliesaroundTarget = EntityManager.Heroes.Enemies.Count(target => target.Distance(t) <= 200);
            var enemyW = MiscMenu["enemyW"].Cast<Slider>().CurrentValue;
            var manaW = MiscMenu["manaW"].Cast<Slider>().CurrentValue;

            if (ManaPercent() <= manaW)
            {
                card = "Blue";
                return card;
            }

            if (ManaPercent() > manaW && alliesaroundTarget >= enemyW)
            {
                card = "Red";
                return card;
            }

            if (ManaPercent() > manaW && alliesaroundTarget < enemyW)
            {
                card = "Yellow";
                return card;
            }
            return null;
        }
    }
}
