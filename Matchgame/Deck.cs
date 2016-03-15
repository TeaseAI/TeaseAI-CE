using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Matchgame
{
    public class Deck : List<Card>
    {
        public Image CardBG = GameManager.DefaultCardBG;

        Random rng = new Random();

        public bool GameOver()
        {
            foreach(Card card in this)
            {
                if(card.FaceUp == false)
                {
                    return false;
                }               
            }

            return true;
        }

        public void FaceDownAllCrads()
        {
            foreach (Card card in this)
            {
                if (card.FaceUp == true)
                {
                    card.Flip();
                }
            }
        }

        public void Shuffle()
        {
            this.Shuffle(10);
        }

        private void Shuffle(int passes)
        {
            int n = this.Count;

            while(passes > 1)
            {
                passes--;

                while(n > 1)
                {
                    int swapIndex = rng.Next();
                    Card card = this[swapIndex];
                    this[swapIndex] = this[n];
                    this[n] = card;
                }
            }
        }
    }
}
