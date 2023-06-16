using Poker.Network;

namespace Poker.Interface
{
    public partial class MainMenuForm : Form
    {
        public Thread? serverThread;

        public MainMenuForm()
        {
            InitializeComponent();

            Server.CreationErrorEvent += OnCreationError;
        }

        private void OnCreationError()
        {
            MessageBox.Show("Your previous game is not finished");
            Server.Disconnect();
            Client.Disconnect();
            Close();
        }

        private void Create_Click(object sender, EventArgs e)
        {
            //serverThread?.Join();

            Server.IsOn = true;
            serverThread = new(Server.Start);
            serverThread.Start();

            var gameForm = new GameForm(tbName.Text, Server.IP);
            Server.StartButtonEvent += gameForm.ShowStart;
            Hide();
            gameForm.Show();
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            serverThread?.Join();

            var gameForm = new GameForm(tbName.Text, tbIP.Text);
            gameForm.HideStart();
            Hide();
            gameForm.Show();
        }

        private void Combinations_Click(object sender, EventArgs e)
        {
            var combinations = new CombinationsForm();
            combinations.Show();
        }

        private void Help_Click(object sender, EventArgs e)
        {
            var help = new HelpForm();
            help.Show();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
