using Poker.Common.Cards;
using Poker.Logics;

namespace Poker.Actors
{
    public enum BetType { None, Call, Check, Fold, Bet};
    public struct PlayerStat
    {
        public const int START_BALANCE = 10000;
        public const int CARDS_NUM = 2;

        public bool isAllIn, isPlaying;
        public string Name = "NO_NAME_ERROR";
        public int Balance, Bet;
        public BetType BetType;
        public Card[] cards;
        public Value bestCardValue;
        public Combination combination;

        public PlayerStat(Card[] cards)
        {
            BetType = BetType.None;
            this.cards = cards;
            Balance = START_BALANCE;
            Bet = 0;
            bestCardValue = Combinations.GetBestCardValue(cards);
            isAllIn = false;
            isPlaying = true;
            combination = Combinations.DefineCombination(cards);
        }

        public PlayerStat(Card[] cards, string name)
        {
            BetType = BetType.None;
            this.cards = cards;
            Balance = START_BALANCE;
            Bet = 0;
            bestCardValue = Combinations.GetBestCardValue(cards);
            isAllIn = false;
            isPlaying = true;
            Name = name;
            combination = Combinations.DefineCombination(cards);
        }

        public PlayerStat(Card[] cards, string name, int balance)
        {
            BetType = BetType.None;
            this.cards = cards;
            Balance = balance;
            Bet = 0;
            bestCardValue = Combinations.GetBestCardValue(cards);
            isAllIn = false;
            isPlaying = true;
            Name = name;
            combination = Combinations.DefineCombination(cards);
        }

        public PlayerStat(string name)
        {
            BetType = BetType.None;
            cards = new Card[CARDS_NUM] { new Card(Suit.Played, Value.Played), new Card(Suit.Played, Value.Played) };
            Balance = START_BALANCE;
            Bet = 0;
            bestCardValue = Value.Played;
            isAllIn = false;
            isPlaying = true;
            Name = name;
            combination = Combination.None;
        }
    }

    public struct PlayerView
    {
        public PictureBox pbCard_0, pbCard_1;
        public Label lblBet, lblBalance, lblName;

        public PlayerView(PictureBox pbCard_0, PictureBox pbCard_1, Label lblBet, Label lblBalance, Label lblName)
        {
            this.pbCard_0 = pbCard_0;
            this.pbCard_1 = pbCard_1;
            this.lblBet = lblBet;
            this.lblBalance = lblBalance;
            this.lblName = lblName;
        }

        public PlayerView(PictureBox[] pbs, Label[] lbls)
        {
            pbCard_0 = pbs[0];
            pbCard_1 = pbs[1];
            lblBet = lbls[0];
            lblBalance = lbls[1];
            lblName = lbls[2];
        }
    }

    public class Player
    {
        public const int PLAYERS_NUM = 5;
        public const int CARDS_NUM = 2;

        public PlayerStat  PlayerStat;
        public int         Balance       { get => PlayerStat.Balance;       set => PlayerStat.Balance = value; }
        public int         Bet           { get => PlayerStat.Bet;           set => PlayerStat.Bet = value; }
        public bool        IsAllIn       { get => PlayerStat.isAllIn;       set => PlayerStat.isAllIn = value; }
        public bool        IsPlaying     { get => PlayerStat.isPlaying;     set => PlayerStat.isPlaying = value; }
        public string      Name          { get => PlayerStat.Name;          set => PlayerStat.Name = value; }
        public Combination Combination   { get => PlayerStat.combination;   set => PlayerStat.combination = value; }
        public Value       BestCardValue { get => PlayerStat.bestCardValue; set => PlayerStat.bestCardValue = value; }
        public Card[]      Cards         { get => PlayerStat.cards;         set => PlayerStat.cards = value; }
        public BetType     BetType       { get => PlayerStat.BetType;       set => PlayerStat.BetType = value; }

        public PlayerView PlayerView;
        public PictureBox PbCard_0   { get => PlayerView.pbCard_0; }
        public PictureBox PbCard_1   { get => PlayerView.pbCard_1; }
        public Label      LblBet     { get => PlayerView.lblBet; }
        public Label      LblBalance { get => PlayerView.lblBalance; }
        public Label      LblName    { get => PlayerView.lblName; }

        public Player(string name, PlayerView view)
        {
            PlayerStat = new(name);
            PlayerView = view;
            IsPlaying = false;
        }

        public virtual void Hide()
        {
            if (PbCard_0.InvokeRequired)
            {
                PbCard_0.Invoke(PbCard_0.Hide);
            }
            else
            {
                PbCard_0.Hide();
            }

            if (PbCard_1.InvokeRequired)
            {
                PbCard_1.Invoke(PbCard_1.Hide);
            }
            else
            {
                PbCard_1.Hide();
            }

            if (LblBalance.InvokeRequired)
            {
                LblBalance.Invoke(LblBalance.Hide);
            }
            else
            {
                LblBalance.Hide();
            }

            if (LblBet.InvokeRequired)
            {
                LblBet.Invoke(LblBet.Hide);
            }
            else
            {
                LblBet.Hide();
            }

            if (LblName.InvokeRequired)
            {
                LblName.Invoke(LblName.Hide);
            }
            else
            {
                LblName.Hide();
            }    
        }

        public virtual void Show()
        {
            Control[] controls = new Control[] { PbCard_0, PbCard_1, LblBalance, LblBet, LblName };

            for (int i = 0; i < controls.Length; i++)
            {
                if (controls[i].InvokeRequired)
                {
                    controls[i].Invoke(controls[i].Show);
                }
                else
                {
                    controls[i].Show();
                }
            }
        }

        public virtual void ShowInfo()
        {
            ShowName();
            ShowBet();
            ShowBalance();
            ShowBackCards();
        }

        public void ShowName()
        {
            if (LblName.InvokeRequired)
            {
                LblName.Invoke(ShowName);
            }
            else
            {
                LblName.Text = Name;
            }
        }

        public void ShowBet()
        {
            if (LblBet.InvokeRequired)
            {
                LblBet.Invoke(ShowBet);
            }
            else
            {
                if (IsAllIn)
                {
                    LblBet.Text = "All in";
                }
                else
                {
                    switch (BetType)
                    {
                        case BetType.Bet: 
                            LblBet.Text = $"Bet: {Bet}"; 
                            break;
                        case BetType.Call: 
                            LblBet.Text = $"Call"; 
                            break;
                        case BetType.Check: 
                            LblBet.Text = "Check"; 
                            break;
                        case BetType.Fold: 
                            LblBet.Text = "Fold"; 
                            break;
                        case BetType.None: 
                            LblBet.Text = string.Empty; 
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void ShowBalance()
        {
            if (LblBalance.InvokeRequired)
            {
                LblBalance.Invoke(ShowBalance);
            }
            else
            {
                LblBalance.Text = $"Balance: {Balance}";
            }
        }

        public void ShowRealCards()
        {
            if (Directory.Exists(Card.FOLDER_PATH))
            {
                if (Cards[0].Value != Value.Played)
                {
                    Card.PaintCard(Cards[0], PbCard_0);
                    Card.PaintCard(Cards[1], PbCard_1);
                }
            }
            else
            {
                throw new Exception($"File {Card.FOLDER_PATH} not found while trying to SetCards");
            }
        }

        public void ShowBackCards()
        {
            if (Directory.Exists(Card.FOLDER_PATH))
            {
                Card.PaintCard(new Card(Suit.Played, Value.Played), PbCard_0);
                Card.PaintCard(new Card(Suit.Played, Value.Played), PbCard_1);
            }
            else
            {
                throw new Exception($"File {Card.FOLDER_PATH} not found while trying to SetCards");
            }
        }
    }

    public struct UserControls
    {
        public const int Count = 4;

        public Button btnCall, btnBet, btnFold;
        public TrackBar tbBet;
        private readonly Control[] array = new Control[Count];

        public Control this[int index]
        {
            get { return array[index]; }
            set { array[index] = value; }
        }

        public UserControls(Button btnCall, Button btnBet, Button btnFold, TrackBar tbBet)
        {
            this[0] = this.btnCall = btnCall;
            this[1] = this.btnBet = btnBet;
            this[2] = this.btnFold = btnFold;
            this[3] = this.tbBet = tbBet;
        }
    }

    public class User : Player
    {
        public UserControls Controls;
        public TrackBar TbBet { get => Controls.tbBet; }
        public Label lblCombination, LblTbBet;

        public User(string name, PlayerView view, UserControls controls, Label lblCombination, Label lblTbBet) : base(name, view)
        {
            LblTbBet = lblTbBet;
            Controls = controls;
            this.lblCombination = lblCombination;
        }

        public void UpdateCombination(Card[] tableCards)
        {
            var res = Cards.Concat(tableCards).ToArray();
            Combination = Combinations.DefineCombination(res);
            ShowCombination();
        }

        public override void ShowInfo()
        {
            base.ShowInfo();
            ShowRealCards();
            ShowCombination();
        }

        public void ShowCombination()
        {

            if (lblCombination.InvokeRequired)
            {
                lblCombination.Invoke(ShowCombination);
            }
            else
            {
                if (Cards[0].Value != Value.Played)
                {
                    lblCombination.Text = $"Combination: {Combination}";
                }
                else
                {
                    lblCombination.Text = string.Empty;
                }
            }
        }

        public void LockControls()
        {
            for (int i = 0; i < UserControls.Count; i++)
            {
                if (Controls[i].InvokeRequired)
                {
                    Controls[i].Invoke(new MethodInvoker(() =>
                    {
                        Controls[i].Enabled = false;
                    }));
                }
                else
                {
                    Controls[i].Enabled = false;
                }
            }
        }

        public void UnlockControls()
        {
            for (int i = 0; i < UserControls.Count; i++)
            {
                if (Controls[i].InvokeRequired)
                {
                    Controls[i].Invoke(new MethodInvoker(() =>
                    {
                        Controls[i].Enabled = true;
                    }));
                }
                else
                {
                    Controls[i].Enabled = true;
                }
            }
        }

        public void UpdateTrackBarMax()
        {
            if (TbBet.InvokeRequired)
            {
                TbBet.Invoke(UpdateTrackBarMax);
            }
            else
            {
                TbBet.Maximum = Balance;
            }
        }

        public void UpdateTrackBarMin(TableStat stat)
        {
            if (TbBet.InvokeRequired)
            {
                TbBet.Invoke(() => UpdateTrackBarMin(stat));
            }
            else
            {
                if (stat.BetToKeepPlaying > Balance)
                {
                    LblTbBet.Text = Balance.ToString();
                    TbBet.Minimum = Balance;
                }
                else
                {
                    if (stat.BetToKeepPlaying != 0)
                    {
                        LblTbBet.Text = Math.Abs((stat.BetToKeepPlaying - Bet)).ToString();
                        TbBet.Minimum = stat.BetToKeepPlaying - Bet;
                    }
                    else
                    {
                        LblTbBet.Text = "0";
                        TbBet.Minimum = 0;
                        //Bet = 0;
                    }

                }
                TbBet.Value = TbBet.Minimum;
            }
        }
    }
}