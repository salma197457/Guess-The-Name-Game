using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    internal class RoomClass
    {
        public int Id { get; set; }
        public string RoomType { get; set; }
        public string RandomWord { get; set; }
        public int Player1ID { get; set; }
        public string Player1Name { get; set; }

        public int Player2ID { get; set; }
        public string Player2Name { get; set; }
        public int counterForRandomWord { get; set; }
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }


        public List<int> WatcherIds { get; set; } = new List<int>();
    }
}
