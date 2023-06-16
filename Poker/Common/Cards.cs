namespace Poker.Common.Cards
{
    // TODO
    // Все enum и struct в отдельный namespace
    public enum Suit { Played = Card.PLAYED, Clubs, Diamonds, Hearts, Spades }
    public enum Value { Played = Card.PLAYED, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace }

    public struct Card
    {
        public const string FOLDER_PATH = "src";

        public const int DECK_SIZE = 52;
        public const int SUITS = 4;
        public const int VALUES = 13;

        public const int PLAYER_CARDS = 2;
        public const int TABLE_CARDS = 5;

        public const int FLOP_CARDS = 3;
        public const int TURN_CARDS = 4;
        public const int RIVER_CARDS = 5;

        public const int PLAYED = -1;

        public Suit Suit;
        public Value Value;

        public Card(Random random)
        {
            Suit = (Suit)random.Next(SUITS);
            Value = (Value)random.Next(VALUES);
        }

        public Card(int suit, int value)
        {
            Suit = (Suit)suit;
            Value = (Value)value;
        }

        public Card(Suit suit, Value value)
        {
            Suit = suit;
            Value = value;
        }

        public static void PaintCard(Card card, PictureBox pb)
        {
            if (pb.InvokeRequired)
            {
                pb.Invoke(() => PaintCard(card, pb));
            }
            else
            {
                string fileName = card.Value.ToString() + "_" + card.Suit.ToString() + ".bmp";
                string filePath = Path.Combine(FOLDER_PATH, fileName);
                if (File.Exists(filePath))
                {
                    pb.Image = Image.FromFile(filePath);
                }
                else
                {
                    throw new Exception($"File {filePath} not found while trying to SetCards");
                }
            }
        }
    }
}
