using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeaseAI_CE.Settings
{
    [Serializable]
    public class General
    {
        public enum Slideshow
        {
            Manual, Time, Tease
        }

        //Slideshow
        public Slideshow slideshowType { get; set; } = Slideshow.Tease;
        public int slideshowSeconds { get; set; } = 30;
        public bool slideshowRandomOrder { get; set; } = false;

        //Chat
        public bool showTimestamps { get; set; } = true;
        public bool showNames { get; set; } = true;
        public bool typeInstantly { get; set; } = false;
        public bool saveChatlog { get; set; } = true;

        //Scripts
        public bool auditScripts { get; set; } = true;

        //Media
        public bool allowMediaDelete { get; set; } = false;

        //Safeword
        public string safeword { get; set; } = "red";

        //Media
        public string dommeImageDir { get; set; } = string.Empty;
    }
}
