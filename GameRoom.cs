using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Formats.Asn1.AsnWriter;

namespace WinFormsApp1
{
    public partial class GameRoom : Form
    {
        string Random_Word;
        string pressed_key;
        Label dynamicLabel;
        int score = 0;
        List<Label> dynamicLabels = new List<Label>();
        client clientInstance;

        public GameRoom(client clientinst)
        {
            InitializeComponent();


            clientInstance = clientinst;
            Random_Word = clientinst.RandomWord;

            clientInstance = clientinst;
            int ROOM = clientinst.RoomId;


            Word_dashed();
            if (clientinst.ClientState == "Host")
            {
                Player1.Text = clientInstance.ClientName;
                Player2.Text = clientInstance.Player2;
                Keyboard.Enabled = false;
                clientInstance.FlagToYourTurnOrNot = false;
                PlayerName.Text = clientInstance.ClientName;

            }
            else if (clientinst.ClientState == "Player")
            {

                Player2.Text = clientinst.ClientName;
                Player1.Text = clientinst.Player1;
                Keyboard.Enabled = true;
                clientInstance.FlagToYourTurnOrNot = false;
                PlayerName.Text = clientInstance.ClientName;


            }
            else
            {
                Player1.Text = clientinst.Player1;
                Player2.Text = clientinst.Player2;
                Keyboard.Enabled = false;
                WatchersNum.Text = "";
                label1.Text = "";
                PlayerName.Text = clientInstance.ClientName;

            }
        }

        public void Word_dashed()
        {
            int x_size = 50;

            for (int i = 0; i < Random_Word.Length; i++)
            {

                dynamicLabel = new Label();

                dynamicLabel.Text = "___";
                dynamicLabel.Location = new Point(100 + x_size, 150);
                dynamicLabel.Size = new Size(30, 50);
                dynamicLabel.Font = new Font("Arial", 20);
                Controls.Add(dynamicLabel);


                dynamicLabels.Add(dynamicLabel);
                x_size += 50;


                if (Random_Word.ToUpper()[i].ToString() == " ")
                {
                    dynamicLabels[i].Text = "";
                }

            }
        }
        public void DrawCharTheAnotherPlayerPressed(string charPressed)
        {
            string pressed_key = charPressed;


            if (Random_Word.ToUpper().Contains(pressed_key))
            {


                for (int i = 0; i < Random_Word.Length; i++)
                {
                    if (Random_Word.ToUpper()[i].ToString() == pressed_key)
                    {
                        dynamicLabels[i].Text = pressed_key;
                    }
                    foreach (Control cntrl in Keyboard.Controls)
                    {
                        if ( cntrl.Text == pressed_key)
                        {
                            cntrl.Enabled = false;
                        }
                    }

                }
            }
        }




        public void Word_index(string charPressed)
        { 

            string pressed_key = charPressed;
            score += 1;

            if (Random_Word.ToUpper().Contains(pressed_key))
            {


                for (int i = 0; i < Random_Word.Length; i++)
                {
                    if (Random_Word.ToUpper()[i].ToString() == pressed_key)
                    {
                        dynamicLabels[i].Text = pressed_key;

                    }
                    foreach (Control cntrl in Keyboard.Controls)
                    {
                        if (cntrl.Focused && cntrl.Text == pressed_key)
                        {
                            cntrl.Enabled = false;
                        }
                    }

                }
                clientInstance.SendMessageToServer($"10,{clientInstance.ClientId},{clientInstance.RoomId},{clientInstance.ClientState},{pressed_key}");
                clientInstance.FlagToYourTurnOrNot=true;
                Player_turn.Text = "Your Turn";

            }
            else
            {
                clientInstance.SendMessageToServer($"11,{clientInstance.ClientId},{clientInstance.RoomId},{clientInstance.ClientState}");
                
                clientInstance.FlagToYourTurnOrNot=false;
                Player_turn.Text = "Another Player Turn";
            }

        }
        public void StartPlay()
        {

            Player2.Text = clientInstance.Player2;
            clientInstance.FlagToStartPlay = true;
            clientInstance.FlagToYourTurnOrNot = true;
            Keyboard.Enabled = true;
        }
        public void ChangeThePlayerTurn(string PlayerTurnName , string c)
        {
            Player_turn.Text= PlayerTurnName;
        }
        public void ChangeTheWatchersNum(string c)
        {
            WatchersNum.Text = c;

        }
        public void ChangeTheScores(string Score1 ,string Score2)
        {
            Player1Score.Text = Score1.ToString();
            Player2Score.Text = Score2.ToString();

        }
        public void closeTheGameRoom()
        {
            this.Hide();
            Form2 form = new Form2(clientInstance);
            form.Show();
        }
        #region buttons
        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void e_Click_1(object sender, EventArgs e)
        {
            if(clientInstance.FlagToYourTurnOrNot==false)
            { 
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("E");

            }
        }
      
        private void t_Click_1(object sender, EventArgs e)
        {
            if (clientInstance.FlagToYourTurnOrNot == false)
            {
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("T");

            }
        }

        private void q_Click(object sender, EventArgs e)
        {
            if (clientInstance.FlagToYourTurnOrNot == false)
            {
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("Q");

            }
        }

        private void w_Click(object sender, EventArgs e)
        {
            if (clientInstance.FlagToYourTurnOrNot == false)
            {
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("W");

            }
        }

        private void r_Click(object sender, EventArgs e)
        {
            if (clientInstance.FlagToYourTurnOrNot == false)
            {
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("R");

            }
        }

        private void y_Click(object sender, EventArgs e)
        {
            if (clientInstance.FlagToYourTurnOrNot == false)
            {
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("Y");

            }
        }

        private void u_Click(object sender, EventArgs e)
        {
            if (clientInstance.FlagToYourTurnOrNot == false)
            {
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("U");

            }
        }

        private void i_Click(object sender, EventArgs e)
        {
            if (clientInstance.FlagToYourTurnOrNot == false)
            {
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("I");

            }
        }

        private void o_Click(object sender, EventArgs e)
        {
            if (clientInstance.FlagToYourTurnOrNot == false)
            {
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("O");

            }
        }

        private void p_Click(object sender, EventArgs e)
        {
            if (clientInstance.FlagToYourTurnOrNot == false)
            {
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("P");

            }
        }

        private void a_Click(object sender, EventArgs e)
        {
            if (clientInstance.FlagToYourTurnOrNot == false)
            {
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("A");

            }
        }

        private void s_Click(object sender, EventArgs e)
        {
            if (clientInstance.FlagToYourTurnOrNot == false)
            {
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("S");

            }
        }

        private void d_Click(object sender, EventArgs e)
        {
            if (clientInstance.FlagToYourTurnOrNot == false)
            {
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("D");

            }
        }

        private void f_Click(object sender, EventArgs e)
        {
            if (clientInstance.FlagToYourTurnOrNot == false)
            {
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("F");

            }
        }

        private void g_Click(object sender, EventArgs e)
        {
            if (clientInstance.FlagToYourTurnOrNot == false)
            {
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("G");

            }
        }

        private void h_Click(object sender, EventArgs e)
        {
            if (clientInstance.FlagToYourTurnOrNot == false)
            {
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("H");

            }
        }

        private void j_Click(object sender, EventArgs e)
        {
            if (clientInstance.FlagToYourTurnOrNot == false)
            {
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("J");

            }
        }

        private void k_Click(object sender, EventArgs e)
        {
            if (clientInstance.FlagToYourTurnOrNot == false)
            {
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("K");

            }
        }

        private void l_Click(object sender, EventArgs e)
        {
            if (clientInstance.FlagToYourTurnOrNot == false)
            {
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("L");

            }
        }

        private void z_Click(object sender, EventArgs e)
        {
            if (clientInstance.FlagToYourTurnOrNot == false)
            {
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("Z");

            }
        }

        private void x_Click(object sender, EventArgs e)
        {
            if (clientInstance.FlagToYourTurnOrNot == false)
            {
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("X");

            }
        }

        private void c_Click(object sender, EventArgs e)
        {
            if (clientInstance.FlagToYourTurnOrNot == false)
            {
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("C");

            }
        }

        private void v_Click(object sender, EventArgs e)
        {
            if (clientInstance.FlagToYourTurnOrNot == false)
            {
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("V");

            }
        }

        private void b_Click(object sender, EventArgs e)
        {
            if (clientInstance.FlagToYourTurnOrNot == false)
            {
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("B");

            }
        }

        private void n_Click(object sender, EventArgs e)
        {
            if (clientInstance.FlagToYourTurnOrNot == false)
            {
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("N");

            }

        }

        private void m_Click(object sender, EventArgs e)
        {
            if (clientInstance.FlagToYourTurnOrNot == false)
            {
                MessageBox.Show("it is the another player Turn");
            }
            else
            {
                Word_index("M");

            }
        }
        #endregion
    }
}
