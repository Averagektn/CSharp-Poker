using Poker.Actors;
using Poker.Common.Cards;
using Poker.Interface;
using Poker.Network;

namespace Poker
{
    // Когда остается 1 не сбросивший карты игрок, хост отключается(ОШИБКА)

    public partial class GameForm : Form
    {
        public string UserName;
        public List<Player> players = new();
        public Table table;
        private readonly Thread clientThread;
        private static int PlayersNUM;
        private User User
        {
            get { return players[0] as User ?? throw new Exception("Error while trying to get User from players array"); }
            set { players[0] = value; }
        }

        public GameForm(string name, string ServerIP)
        {
            InitializeComponent();

            UserName = name;
            PlayersNUM = 0;

            InitializePlayers();

            if (Server.IsOn)
            {
                ShowStart();
            }

            table = new(
                new PictureBox[Card.TABLE_CARDS] { pbTableCard_0, pbTableCard_1, pbTableCard_2, pbTableCard_3, pbTableCard_4 },
                lblBank, lblGamePhase
                );
            table.HideCards();

            User.LockControls();

            // Заменить на tableEvent?
            Client.UserEvent += User.UnlockControls;
            Client.UserEvent += User.UpdateTrackBarMax;

            Client.PlayerEvent += UpdatePlayerControls;
            Client.EnemiesEvent += UpdateEnemyControls;

            Client.TableEvent += UpdateTable;
            Client.TableEvent += User.UpdateTrackBarMin;

            Client.CallsEvent += UnlockCall;

            Client.Error_NameAlreadyExists += OnNameAlreadyExists;
            Client.Error_OnePlayer += OnOnePlayer;
            Client.Error_Disconnect += OnHostDisconnect;
            Client.HostDisconnectedEvent += OnHostDisconnect;

            Client.NewGameEvent += NewGame;
            Client.WinnerEvent += OnVictory;

            clientThread = new(() => Client.Connect(UserName, ServerIP));
            clientThread.Start();
        }

        private void OnVictory(List<PlayerStat> stats)
        {
            int index = stats.FindIndex(player => player.Name == User.Name);

            foreach (var player in players)
            {
                player.ShowRealCards();
            }

            if (index != -1)
            {
                MessageBox.Show($"You won {stats[index].Balance - User.Balance}");
                User.Balance = stats[index].Balance;
            }
            else
            {
                foreach (var stat in stats)
                {
                    MessageBox.Show($"{stat.Name} won");
                }
            }
        }

        private void NewGame()
        {
            table.HideCards();
            // обнулить стол, обнулить ставки игроков, переслать игроков заново, переслать стол заново, 
        }

        private void OnOnePlayer()
        {
            if (InvokeRequired)
            {
                Invoke(Close);
            }
            else
            {
                Close();
            }
            MessageBox.Show("You are the only player");
            PlayersNUM = 0;
        }

        private void OnHostDisconnect()
        {
            if (InvokeRequired)
            {
                Invoke(Close);
            }
            else
            {
                Close();
            }
            MessageBox.Show("Host disconnected");
            PlayersNUM = 0;
        }

        private void OnNameAlreadyExists()
        {
            if (InvokeRequired)
            {
                Invoke(Close);
            }
            else
            {
                Close();
            }
            MessageBox.Show("Change name");
            PlayersNUM = 0;
        }

        private void UnlockCall()
        {
            if (btnCall.InvokeRequired)
            {
                btnCall.Invoke(UnlockCall);
            }
            else
            {
                btnCall.Enabled = true;
            }
        }

        private void UpdateTable(TableStat stat)
        {
            table.Update(stat);
            User.UpdateCombination(stat.Cards);
        }

        private void UpdatePlayerControls(PlayerStat player)
        {
            players[PlayersNUM].Show();
            players[PlayersNUM].PlayerStat = player;
            players[PlayersNUM].ShowInfo();

            PlayersNUM++;
        }

        private void UpdateEnemyControls(List<PlayerStat> stats)
        {
            int index = stats.FindIndex(elem => elem.Name == UserName);

            int countStats = stats.Count;
            int countPlayers = players.Count;

            foreach (var player in players)
            {
                player.Hide();
            }

            for (int i = 0; i < countStats; i++)
            {
                int ind = (countPlayers * 2 - index + i) % countPlayers;
                players[ind].PlayerStat = stats[i];
                players[ind].ShowInfo();
                players[ind].Show();
            }
        }

        private void ShowCombinations_Click(object sender, EventArgs e)
        {
            CombinationsForm combinationsForm = new();
            combinationsForm.Show();
        }

        private void Call_Click(object sender, EventArgs e)
        {
            User.Balance -= tbBet.Minimum;
            User.Bet = tbBet.Minimum;

            if (User.Bet == 0)
            {
                User.BetType = BetType.Check;
            }
            else
            {
                User.BetType = BetType.Call;
            }

            // update bet to avoid negative values
            if (User.Balance <= 0)
            {
                User.Bet += User.Balance;
                User.Balance = 0;
                User.IsAllIn = true;
            }

            User.ShowInfo();
            User.LockControls();

            Client.Send(User.PlayerStat);
        }

        private void Bet_Click(object sender, EventArgs e)
        {
            // Стол обновляется на сервере
            User.Bet = tbBet.Value;
            User.Balance -= tbBet.Value;
            if (User.Bet == 0)
            {
                User.BetType = BetType.Check;
            }
            else
            {
                User.BetType = BetType.Bet;
            }

            if (User.Balance == 0)
            {
                User.IsAllIn = true;
            }

            User.ShowInfo();
            User.LockControls();

            Client.Send(User.PlayerStat);
        }

        private void Fold_Click(object sender, EventArgs e)
        {
            User.BetType = BetType.Fold;
            User.IsPlaying = false;

            User.LockControls();
            User.ShowInfo();
            Client.Send(User.PlayerStat);
        }

        private void Bet_Scroll(object sender, EventArgs e)
        {
            lblBet.Text = tbBet.Value.ToString();
        }

        private void GiveUp_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void StartGame_Click(object sender, EventArgs e)
        {
            Server.StartGame();
            btnStartGame.Hide();
        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Client.Disconnect();
            Server.Disconnect();
            MainMenuForm? menu = Application.OpenForms[0] as MainMenuForm;
            menu?.Show();
        }

        public void ShowStart()
        {
            if (btnStartGame.InvokeRequired)
            {
                btnStartGame?.Invoke(btnStartGame.Show);
            }
            else
            {
                btnStartGame.Show();
            }
        }

        public void HideStart()
        {
            btnStartGame.Hide();
        }

        private void InitializePlayers()
        {
            players.Add(new User("NAME_ERROR",
                new PlayerView(pbPlayer_0_Card_1, pbPlayer_0_Card_2, lblPlayer_0_Bet, lblPlayer_0_Balance, lblPlayer_0_Name),
                new UserControls(btnCall, btnBet, btnFold, tbBet),
                lblCombination, lblBet
                ));
            players.Add(new("",
                new PlayerView(pbPlayer_1_Card_1, pbPlayer_1_Card_2, lblPlayer_1_Bet, lblPlayer_1_Balance, lblPlayer_1_Name)
                ));
            players.Add(new("",
                new PlayerView(pbPlayer_2_Card_1, pbPlayer_2_Card_2, lblPlayer_2_Bet, lblPlayer_2_Balance, lblPlayer_2_Name)
                ));
            players.Add(new("",
                new PlayerView(pbPlayer_3_Card_1, pbPlayer_3_Card_2, lblPlayer_3_Bet, lblPlayer_3_Balance, lblPlayer_3_Name)
                ));
            players.Add(new("",
                new PlayerView(pbPlayer_4_Card_1, pbPlayer_4_Card_2, lblPlayer_4_Bet, lblPlayer_4_Balance, lblPlayer_4_Name)
                ));
            for (int i = 1; i < players.Count; i++)
            {
                players[i].Hide();
            }
        }
    }
}