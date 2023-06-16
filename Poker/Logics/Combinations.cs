using Poker.Actors;
using Poker.Common.Cards;

namespace Poker.Logics
{
    // TODO
    // ТЕСТЫ
    // Добавить bool методы определения комбинации
    public enum Combination
    {
        None, HighCard, Pair, TwoPairs, ThreeOfAKind, Straight, Flush, FullHouse, FourOfAKind, StaightFlush, RoyalFlush
    };

    public static class Combinations
    {
        private const int FULL_COMBINATION_CARDS_NUM = 5; 

        public static Value GetBestCardValue(Card[] cards)
        {
            Value result = Value.Played;

            int countCards = cards.Length - 1; 
            for (int i = 0; i < countCards; i++)
            {
                result = cards[i].Value > cards[i + 1].Value ? cards[i].Value : cards[i + 1].Value;
            }

            return result;
        }
        public static Combination DefineCombination(Card[] cards)
        {
            Combination result = Combination.None;

            // try to remove sorts in methods
            int countCards = cards.Length;
            if (countCards == (int)Phase.Preflop + Player.CARDS_NUM)
            {
                result = IsCombinationPair(cards) ? Combination.Pair : Combination.HighCard;
            }
            else if (countCards >= FULL_COMBINATION_CARDS_NUM)
            {
                if (IsCombinationRoyalFlush(cards))
                {
                    result = Combination.RoyalFlush;
                }
                else if (IsCombinationStraightFlush(cards))
                {
                    result = Combination.StaightFlush;
                }
                else if (IsCombinationFourOfAKind(cards))
                {
                    result = Combination.FourOfAKind;
                }
                else if (IsCombinationFullHouse(cards))
                {
                    result = Combination.FullHouse;
                }
                else if (IsCombinationFlush(cards))
                {
                    result = Combination.Flush;
                }
                else if (IsCombinationStraight(cards))
                {
                    result = Combination.Straight;
                }
                else if (IsCombinationThreeOfAKind(cards))
                {
                    result = Combination.ThreeOfAKind;
                }
                else if (IsCombinationTwoPairs(cards))
                {
                    result = Combination.TwoPairs;
                }
                else if (IsCombinationPair(cards))
                {
                    result = Combination.Pair;
                }
                else
                {
                    result = Combination.HighCard;
                }
            }

            return result;
        }

        private static bool IsCombinationRoyalFlush(Card[] cards)
        {
            bool result = false;

            Array.Sort(cards, CompareByValueAscend);
            int countCards = cards.Length - 4;
            for (int i = 0; i < countCards && !result; i++)
            {
                if (cards[i].Value == Value.Ten && IsFlush(cards[i..(i + 4)]) && IsStraight(cards[i..(i + 4)]))
                {
                    result = true;
                }
            }

            return result;
        }

        private static bool IsCombinationStraightFlush(Card[] cards)
        {
            bool result = false;

            Array.Sort(cards, CompareByValueAscend);
            int countCards = cards.Length - 4;
            for (int i = 0; i < countCards && !result; i++)
            {
                if (IsStraight(cards[i..(i + 4)]) && IsFlush(cards[i..(i + 4)]))
                {
                    result = true;
                }
            }

            return result;
        }

        private static bool IsCombinationFourOfAKind(Card[] cards)
        {
            bool result = false;

            int countCards = cards.Length - 4;
            for (int i = 0; i < countCards && !result; i++)
            {
                if (IsFourOfAKind(cards[i], cards[i + 1], cards[i + 2], cards[i + 3]))
                {
                    result = true;
                }
            }

            return result;
        }

        private static bool IsCombinationFullHouse(Card[] cards)
        {
            bool result, isThreeOfAKind, isPair;
            result = isThreeOfAKind = isPair = false;

            Array.Sort(cards, CompareByValueAscend);
            int countCards = cards.Length - 1;
            for (int i = 0; i < countCards && !(isThreeOfAKind && isPair); i++)
            {
                //CHECK
                if (i < countCards - 1 && IsThreeOfAKind(cards[i], cards[i + 1], cards[i + 2]))
                {
                    isThreeOfAKind = true;
                    i += 3;
                }
                if (IsPair(cards[i], cards[i + 1]))
                {
                    isPair = true;
                    i += 2;
                }
            }

            result = isThreeOfAKind && isPair;
            return result;
        }

        private static bool IsCombinationFlush(Card[] cards)
        {
            bool result = false;

            Array.Sort(cards, CompareBySuitAscend);
            int countCards = cards.Length - 4;
            for (int i = 0; i < countCards && !result; i++)
            {
                // CHECK IF IT IS TRUE 5 CARDS
                if (IsFlush(cards[i..(i + 4)]))
                {
                    result = true;
                }
            }

            return result;
        }

        private static bool IsCombinationStraight(Card[] cards)
        {
            bool result = false;

            Array.Sort(cards, CompareByValueAscend);
            int countCards = cards.Length - 4;
            for (int i = 0; i < countCards && !result; i++)
            {
                // CHECK IF It IS TRUE 5 CARDS
                if (IsStraight(cards[i..(i + 4)]))
                {
                    result = true;
                }
            }

            return result;
        }

        private static bool IsCombinationThreeOfAKind(Card[] cards)
        {
            bool result = false;

            Array.Sort(cards, CompareByValueAscend);
            int countCards = cards.Length - 2;
            for (int i = 0; i < countCards && !result; i++)
            {
                if (IsThreeOfAKind(cards[i], cards[i + 1], cards[i + 2]))
                {
                    result = true;
                }
            }

            return result;
        }

        private static bool IsCombinationTwoPairs(Card[] cards)
        {
            bool result, isOnePair;
            result = isOnePair = false;

            Array.Sort(cards, CompareByValueAscend);
            int countCards = cards.Length - 3;
            // - 3 (all cards = cards.Length - 1 - 2 cards for the second pair) this also allows to use cards[(i + 2)..]
            for (int i = 0; i < countCards && !isOnePair; i++)
            {
                if (IsPair(cards[i], cards[i + 1]))
                {
                    isOnePair = true;
                    result = IsCombinationPair(cards[(i + 2)..]);
                }
            }

            return result;
        }

        private static bool IsCombinationPair(Card[] cards)
        {
            bool result = false;

            Array.Sort(cards, CompareByValueAscend);
            int countCards = cards.Length - 1;
            for (int i = 0; i < countCards && !result; i++)
            {
                if (IsPair(cards[i], cards[i + 1]))
                {
                    result = true;
                }
            }

            return result;
        }

        private static bool IsStraight(Card[] cards)
        {
            bool result = true;

            int countCards = cards.Length;
            if (countCards != FULL_COMBINATION_CARDS_NUM)
            {
                result = false;
            }
            else
            {
                countCards--;
                for (int i = 0; i < countCards; i++)
                {
                    // CHECK FOR A 2 3 4 5 
                    if ((cards[i + 1].Value - cards[i].Value) % ((int)Value.Ace - 1) != 1)
                    {
                        result = false;
                    }
                }
            }

            return result;
        }
        private static bool IsFlush(Card[] cards)
        {
            bool result = false;

            int countCards = cards.Length;
            if (countCards == FULL_COMBINATION_CARDS_NUM)
            {
                result = true;
                countCards--;
                for (int i = 0; i < countCards && result; i++)
                {
                    if (cards[i].Suit != cards[i + 1].Suit)
                    {
                        result = false;
                    }
                }
            }

            return result;
        }
        private static bool IsFourOfAKind(Card card_1, Card card_2, Card card_3, Card card_4) 
        { 
            return card_1.Value == card_2.Value && card_2.Value == card_3.Value && card_3.Value == card_4.Value;
        } 
        private static bool IsThreeOfAKind(Card card_1, Card card_2, Card card_3)
        {
            return IsPair(card_1, card_2) && IsPair(card_2, card_3);
        }
        private static bool IsPair(Card card_1, Card cards_2) => card_1.Value == cards_2.Value;

        private static int CompareByValueAscend(Card card_1, Card card_2) => card_1.Value.CompareTo(card_2.Value);
        private static int CompareBySuitAscend(Card card_1, Card card_2) => card_1.Suit.CompareTo(card_2.Suit);
    }
}
