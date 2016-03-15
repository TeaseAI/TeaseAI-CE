using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Matchgame
{
    public class Card
    {
        Image image;
        RectangleF rect;
        PointF position;

        bool faceUp = false;

        public delegate void FlippedEventHandler(Card sender);
        public event FlippedEventHandler CardFlipped;

        public Card(Image image, RectangleF rect, PointF position)
        {
            this.Image = image;
            this.Rect = rect;
            this.Position = position;
        }

        public void Draw(Graphics g)
        {

        }

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

        public Image Image
        {
            get
            {
                return image;
            }

            set
            {
                image = value;
            }
        }

        public RectangleF Rect
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

        public PointF Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
            }
        }
    }
}
