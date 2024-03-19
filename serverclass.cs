using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Windows.Forms;
using static System.Formats.Asn1.AsnWriter;

namespace server
{
    internal class serverclass
    {
        TcpListener Server;
        byte[] bt;
        IPAddress MyipAddress;
        int port_number;
        object clientsLock = new object();
        Dictionary<int, ClientInfo> clients = new Dictionary<int, ClientInfo>();
        Dictionary<int, RoomClass> rooms = new Dictionary<int, RoomClass>(); 
        public int clientCounter = 0;
        public int RoomIdCounter = 0;
        string messageFromPlayer1 = "";
        string namefromclient;
        clientClass client1;
        int c = 1;


        public int CounterForRoom { get; private set; } 
        public int ClientIdFromServer { get; private set; } 
        public bool FlagToMakePlayer2join { get; set; }



        public serverclass()
        {
            bt = new byte[] { 127, 0, 0, 1 };
            MyipAddress = new IPAddress(bt);
            port_number = 2000;
            Server = new TcpListener(MyipAddress, port_number);
        }

        public void StartServer()
        {
            try
            {
                Server.Start();
                Task.Run(() => AcceptClientsAsync());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }


        private async Task AcceptClientsAsync()
        {
            while (true)
            {
                TcpClient tcpClient = await Server.AcceptTcpClientAsync();
                // open stream
                clients.Add(clientCounter, new ClientInfo { Id = clientCounter, TcpClient = tcpClient });
                Task.Run(() => HandleClientAsync(tcpClient));
            }
        }

        private async Task HandleClientAsync(TcpClient tcpClient)
        {
            int clientId;
            lock (clientsLock)
            {
                clientId = clientCounter;
                var clientInfo = new ClientInfo
                {
                    Id = clientId,
                    TcpClient = tcpClient,
                };
            }


            NetworkStream stream = tcpClient.GetStream();
            BinaryReader reader = new BinaryReader(stream);
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write($"{0},{RoomIdCounter},{clientCounter}");
            clientCounter++;
            string msg;
            while ((msg = reader.ReadString()).Length > 0)
            {
                string[] ClientMsg = msg.Split(',');
              

                switch (ClientMsg[0])
                {
                    
                    case "1":                              
                        int clientId2 = int.Parse(ClientMsg[1]);
                        int roomId = int.Parse(ClientMsg[2]);
                        clients[clientId].Name = ClientMsg[5];

                        CreateRoom(roomId, ClientMsg[3], clientId2, ClientMsg[4], ClientMsg[5]);
                        break;

                    case "2":
                        
                        int clientId1 = int.Parse(ClientMsg[1]);
                        int roomid = int.Parse(ClientMsg[2]);
                        clients[clientId].Name = ClientMsg[3];
                        int P1Id = rooms[roomid].Player1ID;

                        TcpClient tcpClientForPlayer1 = clients[P1Id].TcpClient;
                        NetworkStream streamForPlayer1 = tcpClientForPlayer1.GetStream();
                        BinaryWriter writerForPlayer1 = new BinaryWriter(streamForPlayer1);

                        FlagToMakePlayer2join = false;
                        writerForPlayer1.Write($"1,{ClientMsg[3]}");


                        while (FlagToMakePlayer2join == false && messageFromPlayer1 == "") { }

                        if (messageFromPlayer1 == "Accepted")
                        {
                            JoinRoom(roomid, clientId1, ClientMsg[3]);
                            writer.Write($"3,{rooms[roomid].RandomWord},{rooms[roomid].Player1Name},{rooms[roomid].Player2Name}");
                            writer.Write($"2,Accepted");
                        }

                        else if (messageFromPlayer1 == "notAccepted")
                        {
                            writer.Write($"2,notAccepted");
                        }
                        break;

                    case "3": //watch message from client
                        int WatcherId = int.Parse(ClientMsg[1]);
                        int Roomid = int.Parse(ClientMsg[2]);

                        WatchRoom(WatcherId, Roomid);
                        SendNumberOFWatchers(Roomid);
                        writer.Write($"4,{rooms[Roomid].RandomWord},{rooms[Roomid].Player1Name},{rooms[Roomid].Player2Name}");
                        break;

                    case "4":

                        if (ClientMsg[1] == "Accept")
                        {
                            messageFromPlayer1 = "Accepted";
                            FlagToMakePlayer2join =true;
                        }
                        else
                        {
                            messageFromPlayer1 = "notAccepted";
                            FlagToMakePlayer2join = false;
                        }
                        break;

                    case "10":
                        int roomNum = int.Parse(ClientMsg[2]);
                        string SenderState = ClientMsg[3];
                        string CharPressed= ClientMsg[4];

                        sendTheScores(roomNum , SenderState );
                        BroadCastForRooms(roomNum,SenderState, CharPressed);
                        rooms[roomNum].counterForRandomWord++;

                        if ( (rooms[roomNum].Player1Score + rooms[roomNum].Player2Score) == rooms[roomNum].RandomWord.Distinct().Count() )
                        { SayTheResult(roomNum); }
                        break;

                    case "11"://Switching between players      
;                       int roomNumber = int.Parse(ClientMsg[2]);
                        SwitchTurn(roomNumber, ClientMsg[3]);
                        break;

                    default:
                        break;          

                }

            }

            // Clean up resources for the disconnected client
            lock (clientsLock)
            {
                clients.Remove(clientId);
            }
            tcpClient.Close();
        }



        public void CreateRoom(int roomId, string roomType, int clientId, string Random , string Player1Name)
        {
            RoomIdCounter++;

            lock (clientsLock)
            {
                if (!rooms.ContainsKey(roomId))
                {
                    rooms[roomId] = new RoomClass { Id = roomId, RoomType = roomType };

                    clients[clientId].RoomId = roomId;
                    rooms[roomId].Player1ID = clientId;
                    rooms[roomId].Player1Name = Player1Name;
                    rooms[roomId].RandomWord = Random;

                    clients[clientId].RandomWordInClientInfo = Random;
                    clients[clientId].ClientState = "Host";
                }

            }
        }


        public void JoinRoom(int roomId, int clientId , string Player2Name)
        {           
            try
            {
                lock (clientsLock)
                {     
                    if (rooms.ContainsKey(roomId) && clients.ContainsKey(clientId))
                    {
                        clients[clientId].RoomId = roomId;

                        rooms[roomId].Player2ID = clientId;
                        rooms[roomId].Player2Name = Player2Name;
                        clients[clientId].RandomWordInClientInfo = rooms[roomId].RandomWord;
                        clients[clientId].ClientState = "Player";
                        NetworkStream stream = clients[clientId].TcpClient.GetStream();
                        BinaryWriter writer = new BinaryWriter(stream);

                        writer.Write($"3,{rooms[roomId].RandomWord},{rooms[roomId].Player1ID},{rooms[roomId].Player2ID}");
                    }
                }
            }
            catch
            {
                MessageBox.Show("Error from JoinRoom method");
            }
        }
        public void WatchRoom(int clientId, int roomId)
        {
            try
            {
                lock (clientsLock)
                {


                    if (rooms.ContainsKey(roomId) && clients.ContainsKey(clientId))
                    {
                        clients[clientId].RoomId = roomId;
                        rooms[roomId].WatcherIds.Add(clientId);
                        clients[clientId].RandomWordInClientInfo = rooms[roomId].RandomWord;
                        clients[clientId].ClientState = "Watcher";
                        NetworkStream stream = clients[clientId].TcpClient.GetStream();
                        BinaryWriter writer = new BinaryWriter(stream);


                        writer.Write($"3,{rooms[roomId].RandomWord},{rooms[roomId].Player1Name},{rooms[roomId].Player2Name}");
                    }
                }
            }
            catch
            {
                MessageBox.Show("Error from WatchRoom method");
            }
        }

        public void BroadCastForRooms(int roomId,string StateOfTheSender ,String charPressed )
        {
            if (StateOfTheSender == "Host")
            {
                int ReceivePlayerId = rooms[roomId].Player2ID;
                NetworkStream stream = clients[ReceivePlayerId].TcpClient.GetStream();
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write($"10,{charPressed},{rooms[roomId].Player1Name},{rooms[roomId].WatcherIds.Count()},{rooms[roomId].Player1Score},{rooms[roomId].Player2Score}");

                foreach (var WatcherId in rooms[roomId].WatcherIds)
                {
                    NetworkStream streamToWatcher = clients[WatcherId].TcpClient.GetStream();
                    BinaryWriter writerToWacher = new BinaryWriter(streamToWatcher);
                    writerToWacher.Write($"12,{charPressed},{StateOfTheSender},{rooms[roomId].Player1Name}");
                }
            }

            if (StateOfTheSender == "Player")
            {
                int ReceivePlayerId = rooms[roomId].Player1ID;
                NetworkStream stream = clients[ReceivePlayerId].TcpClient.GetStream();
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write($"10,{charPressed},{rooms[roomId].Player2Name},{rooms[roomId].WatcherIds.Count()},{rooms[roomId].Player1Score},{rooms[roomId].Player2Score}");

                foreach (var WatcherId in rooms[roomId].WatcherIds)
                {
                    NetworkStream streamToWatcher = clients[WatcherId].TcpClient.GetStream();
                    BinaryWriter writerToWacher = new BinaryWriter(streamToWatcher);
                    writerToWacher.Write($"12,{charPressed},{StateOfTheSender},{rooms[roomId].Player2Name}");
                }
            }

        }

        public void SwitchTurn(int roomId, string SenderState)
        {
            if (SenderState == "Host")//HostSendCorrectCharToPLyer2
            {

                int ReceivePlayerId = rooms[roomId].Player2ID;
                NetworkStream stream = clients[ReceivePlayerId].TcpClient.GetStream();
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write($"11,{rooms[roomId].Player2Name},{rooms[roomId].WatcherIds.Count()}");
                foreach (var WatcherId in rooms[roomId].WatcherIds)
                {
                    NetworkStream streamToWatcher = clients[WatcherId].TcpClient.GetStream();
                    BinaryWriter writerToWacher = new BinaryWriter(streamToWatcher);
                    writerToWacher.Write($"13,{rooms[roomId].Player2Name}");
                }
            }
            else if (SenderState == "Player")//Player2SendCorrectCharToHost
            {
                int ReceivePlayerId = rooms[roomId].Player1ID;
                NetworkStream stream = clients[ReceivePlayerId].TcpClient.GetStream();
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write($"11,{rooms[roomId].Player1Name},{rooms[roomId].WatcherIds.Count()}");
                foreach (var WatcherId in rooms[roomId].WatcherIds)
                {
                    NetworkStream streamToWatcher = clients[WatcherId].TcpClient.GetStream();
                    BinaryWriter writerToWacher = new BinaryWriter(streamToWatcher);
                    writerToWacher.Write($"13,{rooms[roomId].Player2Name}");
                }
            }
        }

        public void SayTheResult(int roomid )
        {
          
            if (rooms[roomid].Player1Score > rooms[roomid].Player2Score)
            {
                int ReceivePlayerId = rooms[roomid].Player1ID;
                NetworkStream stream = clients[ReceivePlayerId].TcpClient.GetStream();
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write($"14,1,{rooms[roomid].Player1Name}");


                int ReceivePlayerId2 = rooms[roomid].Player2ID;
                NetworkStream stream2 = clients[ReceivePlayerId2].TcpClient.GetStream();
                BinaryWriter writer2 = new BinaryWriter(stream2);
                writer2.Write($"14,1,{rooms[roomid].Player1Name}");


               

            }
            else if (rooms[roomid].Player1Score < rooms[roomid].Player2Score)
            {

                int ReceivePlayerId = rooms[roomid].Player1ID;
                NetworkStream stream = clients[ReceivePlayerId].TcpClient.GetStream();
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write($"14,1,{rooms[roomid].Player2Name}");


                int ReceivePlayerId2 = rooms[roomid].Player2ID;
                NetworkStream stream2 = clients[ReceivePlayerId2].TcpClient.GetStream();
                BinaryWriter writer2 = new BinaryWriter(stream2);
                writer2.Write($"14,1,{rooms[roomid].Player2Name}");

            }
            else
            {
                int ReceivePlayerId = rooms[roomid].Player1ID;
                NetworkStream stream = clients[ReceivePlayerId].TcpClient.GetStream();
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write($"14,2");


                int ReceivePlayerId2 = rooms[roomid].Player2ID;
                NetworkStream stream2 = clients[ReceivePlayerId2].TcpClient.GetStream();
                BinaryWriter writer2 = new BinaryWriter(stream2);
                writer2.Write($"14,2");
            }


            string filePath = @"G:\ITI\ITI-9 months\C#\Final Project\Scores.txt"; //Adjust your file path here to save scores

            using (StreamWriter writer3 = new StreamWriter(filePath))
            {
                writer3.WriteLine($"Player 1: {rooms[roomid].Player1Name} || Score: {rooms[roomid].Player1Score}");
                writer3.WriteLine($"Player 2: {rooms[roomid].Player2Name} || Score: {rooms[roomid].Player2Score}");
            }


        }

        public void sendTheScores(int roomid , string senderState)
        {
            if (senderState == "Host")
            {
                rooms[roomid].Player1Score++;
            }
            else if(senderState == "Player")
            {
                rooms[roomid].Player2Score++;
            }

            int ReceivePlayerId = rooms[roomid].Player1ID;
            NetworkStream stream = clients[ReceivePlayerId].TcpClient.GetStream();
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write($"15,{rooms[roomid].Player1Score},{rooms[roomid].Player2Score}");


            int ReceivePlayerId2 = rooms[roomid].Player2ID;
            NetworkStream stream2 = clients[ReceivePlayerId2].TcpClient.GetStream();
            BinaryWriter writer2 = new BinaryWriter(stream2);
            writer2.Write($"15,{rooms[roomid].Player1Score},{rooms[roomid].Player2Score}");

            foreach (var WatcherId in rooms[roomid].WatcherIds)
            {
                NetworkStream streamToWatcher = clients[WatcherId].TcpClient.GetStream();
                BinaryWriter writerToWacher = new BinaryWriter(streamToWatcher);
                writerToWacher.Write($"15,{rooms[roomid].Player1Score},{rooms[roomid].Player2Score}");
            }
        }
        public void SendNumberOFWatchers(int roomid)
        {
            int ReceivePlayerId = rooms[roomid].Player1ID;
            NetworkStream stream = clients[ReceivePlayerId].TcpClient.GetStream();
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write($"16,{rooms[roomid].WatcherIds.Count()}");


            int ReceivePlayerId2 = rooms[roomid].Player2ID;
            NetworkStream stream2 = clients[ReceivePlayerId2].TcpClient.GetStream();
            BinaryWriter writer2 = new BinaryWriter(stream2);
            writer2.Write($"16,{rooms[roomid].WatcherIds.Count()}");
        }

    }

}
