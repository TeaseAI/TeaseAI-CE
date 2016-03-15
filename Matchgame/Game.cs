using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace Matchgame
{
    public class Game
    {
        Thread GameRunner;

        Graphics g;

        int xCardCount = 0;
        int yCardCount = 0;

        decimal cardHeight = 0;
        decimal cardWidth = 0;

        const int padding = 5;

        public delegate void GameOverEventHandler(object sender);
        event GameOverEventHandler GameOver;

        Deck Cards = new Deck();

        public Game(PictureBox drawingArea, int pairCount)
        {
            int uniqueCardCount = GameManager.Cards.Count;

            if(pairCount > uniqueCardCount)
            {
                throw new Exception("Paircount exceeds cards present in the deck!");
            }

            GameManager.Cards.Shuffle();

            for (int i = 0; i <= pairCount; i++)
            {
                Cards.Add(GameManager.Cards[i]);
                Cards.Add(GameManager.Cards[i]);
            }

            Cards.Shuffle();

            g = drawingArea.CreateGraphics();

            int cardcount = Cards.Count;

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

            xCardCount = x;
            yCardCount = y;

            cardHeight = (drawingArea.Height - ((y + 1) * padding)) / y; //strip complete height from all padding needed
            cardWidth = (drawingArea.Width - ((x + 1) * padding)) / x;
        }

        public void Start()
        {
            foreach(Card card in Cards)
            {
                card.CardFlipped += Card_CardFlipped;
            }
        }

        private void Card_CardFlipped(Card sender)
        {
            
        }
    }
}
