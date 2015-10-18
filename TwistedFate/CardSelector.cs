namespace TwistedBuddy
{
    using System;
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;

    public enum Cards
    {
        Red,
        Yellow,
        Blue,
        None,
    }

    public enum SelectStatus
    {
        Ready,
        Selecting,
        Selected,
        Cooldown,
    }

    internal class CardSelector
    {
        public static Cards LastCard;
        public static int LastW;
        public static SelectStatus Status { get; set; }

        public static int Delay
        {
            get
            {
                return Essentials.MiscMenu["delay"].Cast<Slider>().CurrentValue;
            }
        }

        static CardSelector()
        {
            Game.OnTick += Game_OnTick;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (args.SData.Name == "PickACard")
            {
                Status = SelectStatus.Selecting;
            }

            if (args.SData.Name == "goldcardlock" || args.SData.Name == "bluecardlock" || args.SData.Name == "redcardlock")
            {
                Status = SelectStatus.Selected;
            }
        }

        public static void StartSelecting(Cards card)
        {
            if (Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name == "PickACard" && Status == SelectStatus.Ready
                && Environment.TickCount - LastW > 170 + Game.Ping / 2)
            {
                Player.CastSpell(SpellSlot.W, Player.Instance.ServerPosition);
                LastW = Environment.TickCount;
                LastCard = card;
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            var wName = Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name;
            var wState = Player.Instance.Spellbook.CanUseSpell(SpellSlot.W);

            if (wState == SpellState.Ready && wName == "PickACard" && Status != SelectStatus.Selecting && !Player.Instance.IsDead)
            {
                Status = SelectStatus.Ready;
            }
            else
            {
                if (wState == SpellState.Cooldown && wName == "PickACard")
                {
                    LastCard = Cards.None;
                    Status = SelectStatus.Cooldown;
                }
                
                if (wState == SpellState.Surpressed && !Player.Instance.IsDead)
                {
                    Status = SelectStatus.Selected;
                }

                if (Status != SelectStatus.Selecting)
                {
                    return;
                }

                if (LastCard == Cards.Blue && wName == "bluecardlock"
                    && Environment.TickCount - Delay > LastW)
                {
                    Player.CastSpell(SpellSlot.W, false);
                }

                if (LastCard == Cards.Yellow && wName == "goldcardlock"
                    && Environment.TickCount - Delay > LastW)
                {
                    Player.CastSpell(SpellSlot.W, false);
                }

                if (LastCard == Cards.Red && wName == "redcardlock"
                    && Environment.TickCount - Delay > LastW)
                {
                    Player.CastSpell(SpellSlot.W, false);
                }
            }
        }
    }
}