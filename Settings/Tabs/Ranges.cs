using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeaseAI_CE.Settings
{
    [Serializable]
    public class Ranges
    {
        //Tease
        public int minTeaseLength { get; set; } = 15;
        public int maxTeaseLength { get; set; } = 60;
    }
}
