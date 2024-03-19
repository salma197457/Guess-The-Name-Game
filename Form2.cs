using System;
using System.Windows.Forms;
using System.Drawing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace WinFormsApp1
{
    public partial class Form2 : Form
    {
        private client ClientInstance;
        string randomWord;

        Random rand = new Random();
        int intRoomNumber;
        Rooms rm = new Rooms();
        public Form2(client clientInstance)
        {
            InitializeComponent();

            ClientInstance = clientInstance;
            ClientInstance.OnMessageReceive += OnResponseForm2;

            Options.Visible = false;
            label1.Text = clientInstance.ClientName;
            intRoomNumber = clientInstance.RoomNumber;
            DrawRooms();
        }

        private void OnResponseForm2(string s)
        { 
        }

        public void DrawRooms()
        {
            for (int i = 0; i < intRoomNumber; i++)
            {
                ListBox listBox = new ListBox();
                listBox.Name = "listBox" + i.ToString();
                listBox.Location = new Point(0, 70 * i);
                listBox.Size = new Size(300, 50);
                listBox.Items.Add($"Room ID:{intRoomNumber}");

                Button joinButton = new Button();
                Button watchButton = new Button();

                joinButton.Text = "Join";
                watchButton.Text = "Watch";

                joinButton.Size = new Size(50, 20);
                watchButton.Size = new Size(50, 20);

                joinButton.Location = new Point(250, 25);
                watchButton.Location = new Point(200, 25);

                joinButton.Name = $"joinButton{i}";
                watchButton.Name = $"watchButton{i}";

                joinButton.Click += JoinButtonClick;
                watchButton.Click += WatchButtonClick;

                listBox.Controls.Add(joinButton);
                listBox.Controls.Add(watchButton);

                groupBox1.Controls.Add(listBox);
            }
        }

        private void JoinButtonClick(object sender, EventArgs e)
        {
            ClientInstance.Flag2 = false;
            ClientInstance.MessageFromserver = "";
            Button joinButton = (Button)sender;
            int roomId = int.Parse(joinButton.Name.Replace("joinButton", ""));
            ClientInstance.RoomId = roomId;
            ClientInstance.ClientState = "Player";
            ClientInstance.SendMessageToServer($"2,{ClientInstance.ClientId},{roomId},{ClientInstance.ClientName}");
            MessageBox.Show("Please Wait a minute");

            while(ClientInstance.Flag2==false&& ClientInstance.MessageFromserver=="")
            {
                
            }
            if (ClientInstance.MessageFromserver == "Accepted")
            {
                GameRoom gm = new GameRoom(ClientInstance);
                ClientInstance.gameRoomInClientThread = gm;

                gm.Show();
            }
            this.Hide();

        }

        private void WatchButtonClick(object sender, EventArgs e)
        {
            Button watchButton = (Button)sender;
            int roomId = int.Parse(watchButton.Name.Replace("watchButton", ""));
            ClientInstance.RoomId = roomId;
            ClientInstance.ClientState = "Watcher";
            ClientInstance.SendMessageToServer($"3,{ClientInstance.ClientId},{roomId}");
            MessageBox.Show("Please Wait a minute");
            GameRoom gm = new GameRoom(ClientInstance);
            ClientInstance.gameRoomInClientThread = gm;
            gm.Show();
            this.Hide();

        }


        private void CreateRoom_Click(object sender, EventArgs e)
        {
            Options.Visible = true;
        }

        private void Animals_CheckedChanged(object sender, EventArgs e)
        {
            ClientInstance.FlagToStartPlay = false;
            string filePath = @"G:\ITI\ITI-9 months\C#\Final Project\Animals.txt";
            string[] AnimalWords = File.ReadAllLines(filePath);
            randomWord = AnimalWords[rand.Next(AnimalWords.Length)];

            int RoomId = ClientInstance.RoomNumber;
            ClientInstance.SendMessageToServer($"1,{ClientInstance.ClientId},{RoomId},Animals,{randomWord},{ClientInstance.ClientName}");
            ClientInstance.ClientState = "Host";
            ClientInstance.RandomWord = randomWord;
            ClientInstance.RoomId = intRoomNumber;

            rm.Id = intRoomNumber;
            rm.RandomWord = randomWord;
            rm.Player1ID = ClientInstance.ClientId;

           GameRoom GameForm = new GameRoom(ClientInstance);
            ClientInstance.gameRoomInClientThread = GameForm;
            ClientInstance.gameRoomInClientThread2 = GameForm;
           GameForm.Show();
            this.Hide();
        }
        private void Food_CheckedChanged(object sender, EventArgs e)
        {
            ClientInstance.FlagToStartPlay = false;
            string filePath = @"G:\ITI\ITI-9 months\C#\Final Project\Food.txt";
            string[] AnimalWords = File.ReadAllLines(filePath);
            randomWord = AnimalWords[rand.Next(AnimalWords.Length)];

            int RoomId = ClientInstance.RoomNumber;
            ClientInstance.SendMessageToServer($"1,{ClientInstance.ClientId},{RoomId},Food,{randomWord},{ClientInstance.ClientName}");
            ClientInstance.ClientState = "Host";
            ClientInstance.RandomWord = randomWord;
            ClientInstance.RoomId = intRoomNumber;

            rm.Id = intRoomNumber;
            rm.RandomWord = randomWord;
            rm.Player1ID = ClientInstance.ClientId;

            GameRoom GameForm = new GameRoom(ClientInstance);
            ClientInstance.gameRoomInClientThread = GameForm;
            ClientInstance.gameRoomInClientThread2 = GameForm;
            GameForm.Show();
            this.Hide();
        }

        private void Vehicle_CheckedChanged(object sender, EventArgs e)
        {
            ClientInstance.FlagToStartPlay = false;
            string filePath = @"G:\ITI\ITI-9 months\C#\Final Project\Vehicles.txt";
            string[] AnimalWords = File.ReadAllLines(filePath);
            randomWord = AnimalWords[rand.Next(AnimalWords.Length)];

            int RoomId = ClientInstance.RoomNumber;
            ClientInstance.SendMessageToServer($"1,{ClientInstance.ClientId},{RoomId},Vehicle,{randomWord},{ClientInstance.ClientName}");
            ClientInstance.ClientState = "Host";
            ClientInstance.RandomWord = randomWord;
            ClientInstance.RoomId = intRoomNumber;

            rm.Id = intRoomNumber;
            rm.RandomWord = randomWord;
            rm.Player1ID = ClientInstance.ClientId;

            GameRoom GameForm = new GameRoom(ClientInstance);
            ClientInstance.gameRoomInClientThread = GameForm;
            ClientInstance.gameRoomInClientThread2 = GameForm;
            GameForm.Show();
            this.Hide();
        }

    }
}
