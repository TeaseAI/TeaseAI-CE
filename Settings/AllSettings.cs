using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeaseAI_CE.Settings
{
    [Serializable]
    public class AllSettings
    {
        public static General general { get; set; } = new General();
        public static Domme domme { get; set; } = new Domme();
        public static Sub sub { get; set; } = new Sub();
        public static Images images { get; set; } = new Images();
        public static Videos videos { get; set; } = new Videos();
        public static Ranges ranges { get; set; } = new Ranges();
    }
}
