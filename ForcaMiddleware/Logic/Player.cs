using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Logic
{
    class Player
    {
        public Player(string name)
        {
            this.name = name;
            this.score = 0;
        }

        public string name { get; set;  }
        public int score { get; set; }
    }
}
