using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matchgame
{
    public class Card
    {
        bool faceUp = false;
        int id;

        public delegate void FlippedEventHandler(Card sender);
        public event FlippedEventHandler CardFlipped;

        public void Flip()
        {
            this.FaceUp = !this.FaceUp;
            this.CardFlipped(this);
        }

        public bool FaceUp
        {
            get
            {
                return faceUp;
            }

            private set
            {
                faceUp = value;
            }
        }

        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public Rectangle Rect
        {
            get
            {
                return rect;
            }

            set
            {
                rect = value;
            }
        } 
    }
}
