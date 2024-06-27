using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sokoban;

namespace YongKoBan
{
    class Goal
    {
        public Vector2Int pos;
        public bool isGoalUsed = false;

        public Goal(Vector2Int pos)
        {
            this.pos = pos;
            isGoalUsed = false;
        }
    }
}
