using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;

namespace WinFormsApp1
{
    public class client
    {
        public delegate void MessageReceiveEventHandler(string message);


        TcpClient Client;
        byte[] bt;
        IPAddress serverIPadress;
        NetworkStream nstream;
        BinaryReader br;
        BinaryWriter bw;

        //string Message;
        //Task receiveMessagesTask;
        //string namefromForm;

        BackgroundWorker worker;
        DialogResult joinResult;
        public Form2 form { get; set; }
        public event MessageReceiveEventHandler OnClientConnected;
        public event MessageReceiveEventHandler OnMessageReceive;
        public client()
        {
            worker = new BackgroundWorker();
            worker.DoWork += DoBackgroundWork;
            
        }

        public int ClientId { get; set; } 
        public string ClientName { get; set; } 
        public string ClientState { get; set; } 
        public string RandomWord { get; set; } 
        public TcpClient TcpClient { get; set; }
        public int RoomId { get; set; }
        public int RoomNumber { get; set; }
        public bool Flag { get; set; }
        public string Player1 { get; set; } 
        public string Player2 { get; set; } 
        public bool Flag2 { get; set; }
        public bool FlagToStartPlay { get; set; }
        public GameRoom gameRoomInClientThread { get; set; }
        public GameRoom gameRoomInClientThread2 { get; set; }

        public string MessageFromserver { get; set; }
        public bool FlagToYourTurnOrNot { get; set; }
        public string PlayerTurn { get; set; }



        public void StartServer()
        {
            worker.RunWorkerAsync();
        }

        private void DoBackgroundWork(object? sender, DoWorkEventArgs e)
        {
            ConnectToServer();
        }

        public void ConnectToServer()
        {
            try
            {
                bt = new byte[] { 127, 0, 0, 1 };
                serverIPadress = new IPAddress(bt);
                Client = new TcpClient();
                Client.Connect(serverIPadress, 2000);

                if (Client.Connected)
                {
                    nstream = Client.GetStream();
                    bw = new BinaryWriter(nstream);
                    br = new BinaryReader(nstream);
                    OnClientConnected("Success");
                    ReceiveMessages();
                }
                else
                {
                    MessageBox.Show("Connection failed");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        public void ReceiveMessages()
        {
            try
            {
                byte[] buffer = new byte[1024];

                while (true)
                {
                    string msg = br.ReadString();
                    string[] clientMsg = msg.Split(',');
                        OnMessageReceive(msg);

                        switch (clientMsg[0])
                        {
                            case "0":
                                if (clientMsg.Length > 2)
                                {
                                    RoomNumber = int.Parse(clientMsg[1]);
                                    ClientId = int.Parse(clientMsg[2]);
                                }
                                break;

                            case "1": //This is asking for someone to join
                            Player2 = clientMsg[1];
                            joinResult = MessageBox.Show($"{Player2} wants to join. Accept?", "Join Request", MessageBoxButtons.OKCancel);

                            if (joinResult == DialogResult.OK)
                            {
                                bw.Write("4,Accept");
                                FlagToStartPlay= true;

                                gameRoomInClientThread.Invoke(() => { gameRoomInClientThread.StartPlay(); });
                            }
                            else { bw.Write("4,Reject"); }
                            break;

                        case "2":
                            if (clientMsg[1] == "Accepted")
                            {
                                Flag2 = true;
                                MessageFromserver = "Accepted";

                            }
                            else if(clientMsg[1] == "notAccepted") 
                            {
                                Flag2 = false;
                                MessageFromserver = "notAccepted";
                                MessageBox.Show($"Host denied your request to join");
                            }
                            break;

                        case "3":
                            RandomWord = clientMsg[1];
                            Player1= clientMsg[2];
                            Player2 = ClientName;
                            break;

                        case "4":
                            RandomWord= clientMsg[1];
                            Player1 = clientMsg[2];
                            Player2 = clientMsg[3];
                            break;

                        case "10": 
                            string charPressedFromAnotherPlayer = clientMsg[1];
                            string c = clientMsg[2];
                            
                            gameRoomInClientThread.Invoke(() =>
                            {
                                gameRoomInClientThread.DrawCharTheAnotherPlayerPressed(charPressedFromAnotherPlayer);
                                gameRoomInClientThread.ChangeThePlayerTurn("Another Player Turn",c);

                            });
                            FlagToYourTurnOrNot = false;
                            break;

                        case "11":
                            string c2 = clientMsg[2];
                            gameRoomInClientThread.Invoke(() =>
                            {
                                gameRoomInClientThread.ChangeThePlayerTurn("Your Turn",c2);
                            });
                            FlagToYourTurnOrNot = true;
                            break;

                        case "12":
                            PlayerTurn = clientMsg[3];
                            gameRoomInClientThread.Invoke(() =>
                            {
                                gameRoomInClientThread.Word_index(clientMsg[1]);
                                gameRoomInClientThread.ChangeThePlayerTurn(PlayerTurn,"");

                            });
                            FlagToYourTurnOrNot = false;
                            break;

                        case "13":
                            PlayerTurn = clientMsg[1];
                            gameRoomInClientThread.Invoke(() =>
                            {
                                gameRoomInClientThread.ChangeThePlayerTurn(PlayerTurn,"");

                            });
                            FlagToYourTurnOrNot = false;
                            break;

                        case "14":

                            if(clientMsg[1] =="1"&& clientMsg[2]==ClientName)
                            {
                                MessageBox.Show("You Won!","Congratulations");
                            }
                            else if (clientMsg[1] == "1" && clientMsg[2] != ClientName)
                            {
                                MessageBox.Show("Better luck next time","Loser");
                            }
                            else
                            {
                                MessageBox.Show("It's a Tie","Draw");
                            }

                            gameRoomInClientThread.Invoke(() =>
                            {
                                gameRoomInClientThread.closeTheGameRoom();

                            });
                            break;

                        case "15":
                            string Player1Score = clientMsg[1];
                            string Player2score = clientMsg[2];

                            gameRoomInClientThread.Invoke(() =>
                            {
                                gameRoomInClientThread.ChangeTheScores(Player1Score, Player2score);

                            });
                            break;

                        case "16":
                            gameRoomInClientThread.Invoke(() =>
                            {
                                gameRoomInClientThread.ChangeTheWatchersNum(clientMsg[1]);
                            });
                            break;

                        default:
                                break;
                        }
                    
                }
        }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in ReceiveMessages: {ex.Message}");
            }
}


        public bool SendMessageToServer(string message)
        {
            try
            {
                bw.Write(message);
                return true; 
            }
            catch (Exception e)
            {
                return false; 
            }
        }

        public void DisconnectFromServer()
        {
            try
            {
                if (Client != null && Client.Connected) 
                {
                    nstream.Close();
                    Client.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in disconnecting");
            }
        }

        public string reciveRandomWordFromServer()
        {
            string[] clientMsg = (br.ReadString()).Split(',');

            if (clientMsg[0] == "3")
            {
                return clientMsg[1];

            }
            return "not found";
        }
    }
}