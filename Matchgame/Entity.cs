using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matchgame
{
    public abstract class Entity
    {
        public Coordinate Position;

        public abstract void Update();
    }
}
