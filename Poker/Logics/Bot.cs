using Poker.Actors;
using Poker.Common.Cards;

namespace Poker.Logics
{
    // НЕ НУЖЕН
    public enum Strategy { Fold, Bet, Call, Check, AllIn }
    // strategy decision
    // Extends or decorator?
    //public class Bot : Player
    //{
        // REMOVE??
        /*        private Player player;
                private static Combination _combination;
                private static int _balance, _bet;*/
/*        private int _currBet;
        private Card[] _tableCards;
        public Bot(string name, PlayerView view, Label? lblCombination = null) : base(name, view, lblCombination)
        {
            _tableCards = Array.Empty<Card>();
        }

        public void NextTurn(Card[] tableCards, int currentBet)
        {
            
            Strategy strategy = ChooseStrategy(tableCards, currentBet);
            switch (strategy)
            {
                case Strategy.Fold: Fold(); break;
                case Strategy.Bet: NewBet(); break;
                case Strategy.Check: Check(); break;
                case Strategy.AllIn: NewBet(); break;
                    //JUST IN CASE _currBet - Bet
                case Strategy.Call: Call(_currBet - Bet); break;
                default: break;
            }
            ShowInfo();
        }

        // Comb, phase, balance, bank - previous bets??
        public Strategy ChooseStrategy(Card[] tableCards, int currentBet)
        {
            Strategy result = Strategy.Fold;

            _tableCards = tableCards;

            _currBet = currentBet;

            // CHANGE LOGICS
            if (IsCheckStrategy())
            {
                // CHANGE
                result = Strategy.Check;
            }
            else if (IsCallStrategy())
            {
                result = Strategy.Call;
            }
            else if (IsAllInStrategy())
            {
                result = Strategy.AllIn;
            }
            else if (IsBetStrategy())
            {
                result = Strategy.Bet;
            }

            return result;
        }

        private bool IsAllInStrategy()
        {
            bool result = false;

            if (_currBet <= Balance && FindCombination(cards.Concat(_tableCards).ToArray()) == Combination.RoyalFlush)
            {
                result = true;
            }

            return result;
        }

        private bool IsBetStrategy()
        {
            bool result = true;

            if (_currBet <= Balance && FindCombination(cards.Concat(_tableCards).ToArray()) >= Combination.Pair)
            {
                result = true;
            } 

            return result;
        }

        private bool IsCallStrategy()
        {
            bool result = false;

            if (_currBet <= Balance && Bet * 2 >= Balance && FindCombination(cards.Concat(_tableCards).ToArray()) >= Combination.Pair)
            {
                result = true;
            }

            return result;
        }

        private bool IsCheckStrategy()
        {
            bool result = false;

            if (_currBet == 0)
            {
                result = true;
            }

            return result;
        }*/
    //}
}
