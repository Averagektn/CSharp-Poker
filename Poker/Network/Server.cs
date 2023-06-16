using Newtonsoft.Json;
using Poker.Actors;
using Poker.Common.Cards;
using Poker.Logics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Poker.Network
{
    public enum MessageType { Player, Table, StartTrades, Players, Calls, Winner, Error, NewGame }
    public enum ErrorType { NameAlreadyExists, Disconnect, OnePlayer, OutOfBalance }

    public delegate void StartButtonEvent();

    public static class Server
    {
        public const string IP = "127.0.0.1";
        public const int PORT = 12345;
        private const int BUFFER_SIZE = 4096;

        private static Dictionary<TcpClient, PlayerStat> playerStatsDict = new();
        private static List<TcpClient> clients { get => playerStatsDict.Keys.ToList(); }
        private static List<PlayerStat> playerStats = new();
        public static List<Card> Cards = new();
        private static TableStat table;


        public static bool IsOn = false;
        private static bool isAccepting = false;
        private static bool IsPlaying = true;
        private static bool IsGameSession = true;

        public static event ErrorEvent? CreationErrorEvent;
        public static event StartButtonEvent? StartButtonEvent;
        public static int ActivePlayers { get => playerStatsDict.Values.Count; }

        public static void Disconnect()
        {
            SendMessage(MessageType.Error, ErrorType.Disconnect, clients);

            CreationErrorEvent = null;
            StartButtonEvent = null;

            IsPlaying = false;
            isAccepting = false;
            IsGameSession = false;

            IsOn = false;
        }

        public static void Start()
        {
            IsOn = true;
            isAccepting = true;
            IsPlaying = true;
            IsGameSession = true;

            playerStats.Clear();
            playerStatsDict.Clear();

            var listener = new TcpListener(IPAddress.Parse(IP), PORT);
            try
            {
                listener.Start();
            }
            catch
            {
                CreationErrorEvent?.Invoke();
            }

            while (IsGameSession)
            {
                isAccepting = true;
                IsPlaying = true;
                while (isAccepting && IsGameSession)
                {
                    try
                    {
                        if (listener.Pending())
                        {
                            TcpClient client = listener.AcceptTcpClient();
                            ProcessConnections(client);

                            if (playerStatsDict.Count == Player.PLAYERS_NUM)
                            {
                                isAccepting = false;
                            }
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        isAccepting = false;
                        IsPlaying = false;
                        IsGameSession = false;
                    }

                }

                playerStats.Clear();
                foreach (var player in playerStatsDict)
                {
                    Card[] cards = new Card[PlayerStat.CARDS_NUM] { DealCard(), DealCard() };
                    PlayerStat playerStat = new(cards, player.Value.Name, player.Value.Balance);
                    playerStatsDict[player.Key] = playerStat;
                    playerStats.Add(playerStat);
                }
                SendMessage(MessageType.Players, playerStats, clients);
                table.Cards = Array.Empty<Card>();

                Trades();
                Calls();
                ZeroBets();

                //FLOP
                Card[] crds = new Card[Card.FLOP_CARDS] { DealCard(), DealCard(), DealCard() };
                table = new(crds, table.Bank, 0, Phase.Flop);
                SendMessage(MessageType.Table, table, clients);

                // Trades
                Trades();
                Calls();
                ZeroBets();

                // TURN
                crds = new Card[Card.TURN_CARDS] { crds[0], crds[1], crds[2], DealCard() };
                table = new(crds, table.Bank, 0, Phase.Turn);
                SendMessage(MessageType.Table, table, clients);

                // TRADES
                Trades();
                Calls();
                ZeroBets();

                // RIVER
                crds = new Card[Card.RIVER_CARDS] { crds[0], crds[1], crds[2], crds[3], DealCard() };
                table = new(crds, table.Bank, 0, Phase.River);
                SendMessage(MessageType.Table, table, clients);

                // TRADES
                Trades();
                Calls();
                ZeroBets();

                FindWinner();

                if (IsGameSession)
                {
                    StartButtonEvent?.Invoke();

                    List<KeyValuePair<TcpClient, PlayerStat>> losers = new();
                    foreach (var player in playerStatsDict)
                    {
                        if (player.Value.Balance == 0)
                        {
                            losers.Add(player);
                        }
                    }

                    foreach (var loser in losers)
                    {
                        SendMessage(MessageType.Error, ErrorType.Disconnect, new List<TcpClient> { loser.Key });
                        playerStatsDict.Remove(loser.Key);
                    }

                    playerStats = playerStatsDict.Values.ToList();
                    for (int i = 0; i < playerStats.Count; i++)
                    {
                        var tmp = playerStats[i];
                        tmp.isPlaying = true;
                        tmp.Bet = 0;
                        tmp.BetType = BetType.None;
                        tmp.isAllIn = false;
                        playerStats[i] = tmp;
                    }
                    table.Bank = 0;
                    table.BetToKeepPlaying = 0;
                    table.Cards = Array.Empty<Card>();
                    table.Phase = Phase.Preflop;
                    SendMessage(MessageType.NewGame, table, clients);
                    SendMessage(MessageType.Players, playerStats, clients);
                    SendMessage(MessageType.Table, table, clients);
                }
                Cards.Clear();
            }

            listener.Stop();
            foreach (var client in clients)
            {
                client.Close();
            }
        }

        private static void CheckActivePlayers()
        {
            if (IsGameSession && ActivePlayers == 1)
            {
                SendMessage(MessageType.Error, ErrorType.OnePlayer, clients);
                IsGameSession = false;
            }
            if (IsPlaying && playerStatsDict.Values.Where(player => player.isPlaying).Count() == 1)
            {
                SendMessage(MessageType.Winner, playerStatsDict.Values.Where(player => player.isPlaying).First(), clients);
                IsPlaying = false;
            }
        }

        private static void ZeroBets()
        {
            for (int i = 0; i < playerStats.Count; i++)
            {
                var tmp = playerStats[i];
                tmp.Bet = 0;
                playerStats[i] = tmp;
            }
            SendMessage(MessageType.Players, playerStats, clients);
        }

        public static void StartGame()
        {
            isAccepting = false;
        }

        public static void ProcessConnections(TcpClient client)
        {
            PlayerStat playerStat = GetJSON(client);
            var tmp = new List<TcpClient>() { client };
            if (playerStatsDict.Any(x => x.Value.Name == playerStat.Name))
            {
                SendMessage(MessageType.Error, ErrorType.NameAlreadyExists, tmp);
            }
            else
            {
                Card[] crds = new Card[PlayerStat.CARDS_NUM] { new Card(Suit.Played, Value.Played), new Card(Suit.Played, Value.Played) };
                playerStat = new(crds, playerStat.Name);
                playerStatsDict.Add(client, playerStat);
                playerStats.Add(playerStat);

                SendMessage(MessageType.Players, playerStats, clients);
            }
        }

        public static PlayerStat GetJSON(TcpClient client)
        {
            byte[] buffer = new byte[BUFFER_SIZE];
            var ms = new MemoryStream();
            int bytesRead;

            var stream = client.GetStream();
            do
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                ms.Write(buffer, 0, bytesRead);
            } while (stream.DataAvailable);

            byte[] data = ms.ToArray();
            string json = Encoding.UTF8.GetString(data);

            PlayerStat playerStat = JsonConvert.DeserializeObject<PlayerStat>(json);
            return playerStat;
        }

        public static void SendMessage(MessageType messageType, object message, List<TcpClient> clients)
        {
            if (IsPlaying)
            {
                string json = JsonConvert.SerializeObject(message);
                byte[] messageTypeBytes = BitConverter.GetBytes((int)messageType);
                byte[] messageLengthBytes = BitConverter.GetBytes(json.Length);

                byte[] header = messageLengthBytes.Concat(messageTypeBytes).ToArray();
                byte[] body = Encoding.UTF8.GetBytes(json);
                byte[] messageBytes = header.Concat(body).ToArray();

                for (int i = clients.Count - 1; i >= 0; i--)
                {

                    try
                    {
                        var stream = clients[i].GetStream();
                        stream.Write(messageBytes, 0, messageBytes.Length);
                    }
                    catch
                    {
                        if (playerStatsDict.ContainsKey(clients[i]))
                        {
                            int index = playerStats.FindIndex(player => player.Name == playerStatsDict[clients[i]].Name);
                            if (index != -1 && playerStats.Count > index)
                            {
                                PlayerStat newPlayer = playerStats[index];
                                newPlayer.BetType = BetType.Fold;

                                playerStats.RemoveAt(index);
                                playerStats.Insert(index, newPlayer);
                            }

                            // Replace with clients[i].Remove?
                            playerStatsDict.Remove(clients[i]);
                        }

                    }
                }
            }
        }

        public static Card DealCard()
        {
            Card result;
            var random = new Random();

            do
            {
                result = new(random);
            } while (Cards.Contains(result));
            Cards.Add(result);

            return result;
        }

        public static void DeckReset()
        {
            Cards.Clear();
        }

        public static void FindWinner()
        {
            if (IsGameSession)
            {
                var playerStats = playerStatsDict.Values.ToList();
                int countClients = playerStats.Count;
                Combination[] combinations = new Combination[countClients];
                int highest = 0;
                int anotherWinner = -1;
                List<PlayerStat> winners = new();

                for (int i = 0; i < countClients; i++)
                {
                    combinations[i] = Combination.None;
                    if (playerStats[i].isPlaying)
                    {
                        Card[] crds = playerStats[i].cards.Concat(table.Cards).ToArray();
                        combinations[i] = Combinations.DefineCombination(crds);
                    }
                }

                for (int i = 1; i < countClients; i++)
                {
                    if (combinations[highest] < combinations[i])
                    {
                        highest = i;
                    }
                    if (combinations[highest] == combinations[i] && playerStats[highest].bestCardValue < playerStats[i].bestCardValue)
                    {
                        highest = i;
                    }
                    if (combinations[highest] == combinations[i] && playerStats[highest].bestCardValue == playerStats[i].bestCardValue)
                    {
                        anotherWinner = i;
                    }

                }

                if (anotherWinner == -1)
                {
                    PlayerStat first = playerStats[highest];
                    first.Balance += table.Bank;
                    playerStats[highest] = first;

                    var tmp = playerStatsDict.FirstOrDefault(p => p.Value.Name == first.Name);
                    playerStatsDict[tmp.Key] = first;

                    winners.Add(first);
                }
                else
                {
                    PlayerStat first = playerStats[highest];
                    first.Balance += table.Bank / 2;
                    playerStats[highest] = first;

                    var tmp = playerStatsDict.FirstOrDefault(p => p.Value.Name == first.Name);
                    playerStatsDict[tmp.Key] = first;

                    winners.Add(first);

                    first = playerStats[anotherWinner];
                    first.Balance += table.Bank / 2;
                    playerStats[highest] = first;

                    tmp = playerStatsDict.FirstOrDefault(p => p.Value.Name == first.Name);
                    playerStatsDict[tmp.Key] = first;

                    winners.Add(first);
                }

                SendMessage(MessageType.Winner, winners, clients);
                SendMessage(MessageType.Players, playerStatsDict.Values.ToList(), clients);
            }
        }


        private static void Trades()
        {
            CheckActivePlayers();
            if (IsPlaying)
            {
                foreach (var player in playerStatsDict)
                {
                    if (player.Value.isPlaying && !player.Value.isAllIn)
                    {
                        string tmpName = player.Value.Name;
                        var tmpList = new List<TcpClient>() { player.Key };

                        SendMessage(MessageType.StartTrades, tmpName, tmpList);

                        //NewTurn(player.Key);
                        PlayerStat playerStat = GetJSON(player.Key);
                        playerStatsDict[player.Key] = playerStat;
                        playerStats[playerStats.FindIndex(player => player.Name == playerStat.Name)] = playerStat;

                        table.Bank += playerStat.Bet;
                        table.BetToKeepPlaying = playerStat.Bet;

                        // Ход текущего игрока у него вычисляется без обращения к серверу
                        var clientsWithoutUser = clients.Where(item => item != player.Key).ToList();
                        SendMessage(MessageType.Players, playerStats, clientsWithoutUser);
                        SendMessage(MessageType.Table, table, clients);
                    }
                }
            }
        }
        private static void Calls()
        {
            CheckActivePlayers();
            if (IsPlaying)
            {
                foreach (var player in playerStatsDict)
                {
                    if (player.Value.isPlaying && player.Value.Bet != table.BetToKeepPlaying && !player.Value.isAllIn)
                    //if (player.Value.isPlaying)
                    {
                        string tmpName = player.Value.Name;
                        var tmpList = new List<TcpClient>() { player.Key };
                        SendMessage(MessageType.Calls, tmpName, tmpList);

                        //NewTurn(player.Key);
                        PlayerStat playerStat = GetJSON(player.Key);
                        playerStatsDict[player.Key] = playerStat;
                        playerStats[playerStats.FindIndex(player => player.Name == playerStat.Name)] = playerStat;

                        table.Bank += playerStat.Bet;
                        //table.BetToKeepPlaying = playerStat.Bet;
                        //playerStat.Bet = 0;

                        // Ход текущего игрока у него вычисляется без обращения к серверу
                        var clientsWithoutUser = clients.Where(item => item != player.Key).ToList();
                        SendMessage(MessageType.Players, playerStats, clientsWithoutUser);
                    }
                }
                table.BetToKeepPlaying = 0;
            }

        }
    }
}