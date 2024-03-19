using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    internal class clientClass
    {
        TcpClient Client;
        byte[] bt;
        IPAddress serverIPadress;
        NetworkStream nstream;
        BinaryReader br;
        BinaryWriter bw;
        string Message;
        Task receiveMessagesTask;
        string namefromForm;
        public int ClientId { get; set; } 
        public string ClientName { get; private set; } 
        public string ClientState { get; set; }
        public string RandomWord { get; set; } 
        public void ConnectToServer(string str)
        {
            try
            {
                bt = new byte[] { 127, 0, 0, 1 };
                serverIPadress = new IPAddress(bt);
                Client = new TcpClient();
                Client.Connect(serverIPadress, 2000);

                if (Client.Connected)
                {
                    receiveMessagesTask = Task.Run(async () => await ReceiveMessagesAsync());
                    ClientName = str;
                }
                else
                {
                    MessageBox.Show("Connected unsuccessfully");
                }

                nstream = Client.GetStream();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        
        private bool SendMessageToServer(string message)
        {
            try
            {
                bw = new BinaryWriter(nstream);
                bw.Write(message);
                return true; // Successfully sent the message
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message to server: {ex.Message}");
                return false; // Failed to send the message
            }
        }


        private async Task ReceiveMessagesAsync()
        {
            try
            {
                BinaryReader reader = new BinaryReader(nstream);

                while (true)
                {
                    string receivedMessage = reader.ReadString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in ReceiveMessagesAsync: {ex.Message}");
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
        public void getNameFromTextBox(string n)
        {
            namefromForm = n;

        }
        public void getid(int i)
        {
            ClientId = i;

        }
       

    }
}