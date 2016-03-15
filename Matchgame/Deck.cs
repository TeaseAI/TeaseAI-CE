using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matchgame
{
    public class Deck : List<Card>
    {
        Random rng = new Random();
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
