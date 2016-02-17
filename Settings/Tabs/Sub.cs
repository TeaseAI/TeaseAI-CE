using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeaseAI_CE.Settings
{
    [Serializable]
    public class Sub
    {
        //Appearance
        public DateTime birthday { get; set; } = new DateTime(DateTime.Now.Year - 24, 1, 1);
        public int cockSize { get; set; } = 6; //Inches
        public bool circumsised { get; set; } = false;
        public bool pierced { get; set; } = false;
        public string hairColor { get; set; } = "brown";
        public string eyeColor { get; set; } = "green";

        //CBT
        public int CBTIntensity { get; set; } = 2; //0 to 5
        public bool cockTorture { get; set; } = true;
        public bool ballTorture { get; set; } = true;

        //Edging
        public int maxEdgeTime { get; set; } = 8; //0 for domme decide

        //Chastity
        public bool ownsChastity { get; set; } = false;

        //Performance
        //Consider the last orgasm/ruined to be the day this was generated
        public DateTime lastOrgasm { get; set; } = DateTime.Now;
        public DateTime lastRuined { get; set; } = DateTime.Now;

        //Phrases
        public string[] phraseGreetings { get; set; } = { "Greeting", "hello", "hi", "hey", "heya", "good morning", "good afternoon", "good evening" };
        public string[] phraseAffirmative { get; set; } = { "Yes", "yes", "yeah", "yep", "yup", "sure", "of course", "absolutely", "you know it", "definitely" };
        public string[] phraseNegative { get; set; } = { "No", "no", "nah", "nope", "not" };
        public string[] phraseHonorific { get; set; } = { "Mistress" };
    }
}
