using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elo_Simulation
{
    public class Player
    {
        public int id;
        public int skill;
        public int elo;

        public Player(int _id, int _skill, int _elo)
        {
            id = _id;
            skill = _skill;
            elo = _elo;
        }
    }
}
