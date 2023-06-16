using Newtonsoft.Json;
using Poker.Actors;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Poker.Network
{
    public delegate void HostDisconnectedEvent();
    public delegate void WinnerEvent(List<PlayerStat> stat);
    // Используется у сервера
    public delegate void NewGameEvent();
    public delegate void ErrorEvent();
    public delegate void UserEvent();
    public delegate void EnemiesEvent(List<PlayerStat> message);
    public delegate void PlayerEvent(PlayerStat stat);
    public delegate void CallsEvent();
    public delegate void TableEvent(TableStat table);

    public static class Client
    {
        //public const string IP = "127.0.0.1";
        public const int PORT = 12345;

        public static event HostDisconnectedEvent? HostDisconnectedEvent;
        public static event WinnerEvent? WinnerEvent;
        public static event ErrorEvent? Error_OnePlayer;
        public static event ErrorEvent? Error_Disconnect;
        public static event ErrorEvent? Error_NameAlreadyExists;
        public static event CallsEvent? CallsEvent;
        public static event UserEvent? UserEvent;
        public static event EnemiesEvent? EnemiesEvent;
        public static event PlayerEvent? PlayerEvent;
        public static event TableEvent? TableEvent;
        public static event NewGameEvent? NewGameEvent;

        public static PlayerStat thisPlayer;
        public static bool IsGameContinues;
        private static TcpClient? client;

        public static void Connect(string name, string IP)
        {
            IsGameContinues = true;
            client = new TcpClient();

            try
            {
                client.Connect(IPAddress.Parse(IP), PORT);

                thisPlayer = new PlayerStat(name);

                Send(thisPlayer);

                var stream = client.GetStream();

                while (IsGameContinues)
                {
                    if (stream.DataAvailable)
                    {
                        Receive();
                    }
                }
            }
            catch
            {
                HostDisconnectedEvent?.Invoke();
            }
        }

        public static void Disconnect()
        {
            IsGameContinues = false;

            UserEvent = null;
            PlayerEvent = null;
            EnemiesEvent = null;
            WinnerEvent = null;
            TableEvent = null;
            CallsEvent = null;
            Error_NameAlreadyExists = null;
            HostDisconnectedEvent = null;
            Error_Disconnect = null;
            Error_OnePlayer = null;
            NewGameEvent = null;

            thisPlayer.isPlaying = false;
            Send(thisPlayer);

            client?.Close();
        }

        private static void Receive()
        {
            byte[] lengthBuffer = new byte[4];
            int bytesRead = 0;
            var stream = client!.GetStream();
            do
            {
                bytesRead += stream.Read(lengthBuffer, bytesRead, 4 - bytesRead);
            }
            while (bytesRead < 4);

            int length = BitConverter.ToInt32(lengthBuffer, 0);

            byte[] typeBuffer = new byte[4];
            bytesRead = 0;
            do
            {
                bytesRead += stream.Read(typeBuffer, bytesRead, 4 - bytesRead);
            }
            while (bytesRead < 4);

            int type = BitConverter.ToInt32(typeBuffer, 0);

            byte[] dataBuffer = new byte[length];
            bytesRead = 0;
            do
            {
                bytesRead += stream.Read(dataBuffer, bytesRead, length - bytesRead);
            }
            while (bytesRead < length);

            string json = Encoding.UTF8.GetString(dataBuffer);

            switch (type)
            {
                case (int)MessageType.Player:
                    PlayerStat playerStat = JsonConvert.DeserializeObject<PlayerStat>(json);
                    PlayerEvent?.Invoke(playerStat);
                    break;
                case (int)MessageType.Players:
                    List<PlayerStat> playersStat = JsonConvert.DeserializeObject<List<PlayerStat>>(json) ?? new();
                    EnemiesEvent?.Invoke(playersStat);
                    break;
                case (int)MessageType.Table:
                    TableStat table = JsonConvert.DeserializeObject<TableStat>(json);
                    TableEvent?.Invoke(table);
                    break;
                case (int)MessageType.StartTrades:
                    UserEvent?.Invoke();
                    break;
                case (int)MessageType.Error:
                    ErrorType error = JsonConvert.DeserializeObject<ErrorType>(json);
                    switch (error)
                    {
                        case ErrorType.OnePlayer:
                            Error_OnePlayer?.Invoke();
                            break;
                        case ErrorType.Disconnect: 
                            Error_Disconnect?.Invoke();
                            break;
                        case ErrorType.NameAlreadyExists: 
                            Error_NameAlreadyExists?.Invoke();
                            break;
                        default:
                            break;
                    }
                    break;
                case (int)MessageType.Winner:
                    List<PlayerStat> winners = JsonConvert.DeserializeObject<List<PlayerStat>>(json) ?? new();
                    WinnerEvent?.Invoke(winners);
                    NewGameEvent?.Invoke();
                    break;
                case (int)MessageType.Calls:
                    CallsEvent?.Invoke();
                    break;
                default:
                    break;
            }
        }

        public static void Send(PlayerStat message)
        {
            try
            {
                string json = JsonConvert.SerializeObject(message);
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                var stream = client!.GetStream();
                stream.Write(bytes, 0, bytes.Length);
            }
            catch
            {
                HostDisconnectedEvent?.Invoke();
            }
            
        }
    }
}
