using Poker.Common.Cards;
using Poker.Logics;

namespace Poker.Actors
{
    public struct TableStat
    {
        public int Bank, BetToKeepPlaying;
        public Card[] Cards = Array.Empty<Card>();
        public Phase Phase;

        public TableStat(Card[] cards, int bank, int betToKeepPlaying, Phase phase)
        {
            Cards = cards;
            Bank = bank;
            BetToKeepPlaying = betToKeepPlaying;
            Phase = phase;
        }

        public TableStat(int bank, int betToKeepPlaying)
        {
            Cards = new Card[Card.TABLE_CARDS];
            Bank = bank;
            BetToKeepPlaying = betToKeepPlaying;
            Phase = Phase.Preflop;
        }
    }

    public class Table
    {
        public TableStat TableStat;
        public Phase Phase          { get => TableStat.Phase;            set => TableStat.Phase = value; }
        public Card[] Cards         { get => TableStat.Cards;            set => TableStat.Cards = value; }
        public int Bank             { get => TableStat.Bank;             set => TableStat.Bank = value; }
        public int BetToKeepPlaying { get => TableStat.BetToKeepPlaying; set => TableStat.BetToKeepPlaying = value; }

        public PictureBox[] pbs;
        private readonly Label lblBank, lblGamePhase;

        public Table(PictureBox[] pbs, Label lblBank, Label lblGamePhase)
        {
            this.lblBank = lblBank;
            this.lblGamePhase = lblGamePhase;

            TableStat = new(0, 0);

            lblGamePhase.Text = Phase.ToString();
            lblBank.Text = $"Bank: {Bank}";

            this.pbs = pbs;
        }

        public void Update(TableStat stat)
        {
            Bank = stat.Bank;
            Phase = stat.Phase;
            BetToKeepPlaying = stat.BetToKeepPlaying;
            Cards = stat.Cards;

            ShowBank();
            ShowCards();
            ShowPhase();
        }

        public void ShowCards()
        {
            int countCards = Cards.Length;
            int countPbs = pbs.Length;
            for (int i = 0; i < countCards && i < countPbs; i++)
            {
                Card.PaintCard(Cards![i], pbs[i]);
                if (pbs[i].InvokeRequired)
                {
                    pbs[i].Invoke(pbs[i].Show);
                }
                else
                {
                    pbs[i].Show();
                }
            }
        }

        public void HideCards()
        {
            foreach(var pb in pbs)
            {
                if (pb.InvokeRequired)
                {
                    pb.Invoke(pb.Hide);
                }
                else
                {
                    pb.Hide();
                }
            }
        }

        public void ShowPhase()
        {
            if (lblGamePhase.InvokeRequired)
            {
                lblGamePhase.Invoke(ShowPhase);
            }
            else
            {
                lblGamePhase.Text = $"{Phase}";
            }
        }

        public void ShowBank()
        {
            if (lblBank.InvokeRequired)
            {
                lblBank.Invoke(ShowBank);
            }
            else
            {
                lblBank.Text = $"Bank: {Bank}";
            }
        }
    }
}