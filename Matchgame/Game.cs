using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Matchgame
{
    public class Game
    {
        Graphics g;
        public Game(PictureBox drawingArea, int pairCount)
        {
            g = drawingArea.CreateGraphics();

            int uniqueCardCount = GameManager.Cards.Count;

            if(pairCount > uniqueCardCount)
            {
                throw new Exception("Paircount exceeds cards present in the deck!");
            }

            int cardcount = pairCount * 2;

            int x = 3;
            int y = 0;

            for(int i = 2; Math.Abs(x - y) <= 2; i++)
            {
                if (cardcount % i == 0)
                {
                    x = cardcount / i;
                    y = i;
                }
            }

            GameManager.Cards.Shuffle();
            
        }
    }
}
