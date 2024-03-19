using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    public class Rooms
    {
        public int Id { get; set; }
        public string RoomType { get; set; }
        public string RandomWord { get; set; }
        public int Player1ID { get; set; }    
        public int Player2ID { get; set; }  
        public List<int> WatcherIds { get; set; } = new List<int>();
    }
}
