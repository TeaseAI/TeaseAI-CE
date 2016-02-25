using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeaseAI_CE.Settings
{
    [Serializable]
    public class Domme
    {
        public enum Level
        {
            Gentle, Lenient, Tease, Rough, Sadistic
        }

        public enum Apathy
        {
            Cautious, Caring, Moderate, Cruel, Merciless
        }

        public enum Rarity
        {
            Never, Rarely, Sometimes, Often, Always
        }

        public enum CupSize
        {
            A, B, C, D, DD, DDD, DDDP
        }

        public enum PubicHair
        {
            Shaved, Sparse, Trimmed, Natural, Hairy
        }
        
        //Appearance
        public DateTime birthday { get; set; } = new DateTime(DateTime.Now.Year - 24, 1, 1);
        public bool tattoos { get; set; } = false;
        public bool freckles { get; set; } = false;
        public string hairColor { get; set; } = "blonde";
        public string hairLength { get; set; } = "long";
        public string eyeColor { get; set; } = "green";
        public CupSize cupSize { get; set; } = CupSize.C;
        public PubicHair pubicHair { get; set; } = PubicHair.Shaved;

        //Orgasms
        public Rarity orgasmAllow { get; set; } = Rarity.Sometimes;
        public Rarity orgasmRuin { get; set; } = Rarity.Sometimes;
        public bool orgasmLimit { get; set; } = false;
        public int orgasmLimitCount { get; set; } = 3;
        public DateTime orgasmLimitDate { get; set; } = DateTime.Now;
        public bool denialEnds { get; set; } = true;
        public bool orgasmEnds { get; set; } = true;

        //Difficulty
        public Level difficultyLevel { get; set; } = Domme.Level.Tease;
        public Apathy difficultyApathy { get; set; } = Apathy.Moderate;
        public int lowerMoodRange { get; set; } = 5;
        public int upperMoodRange { get; set; } = 8;

        //Personality
        public bool crazy { get; set; } = false;
        public bool vulgar { get; set; } = false;
        public bool supremacist { get; set; } = false;
        public string teasePersonality { get; set; } = string.Empty;

        //Sub names
        public string[] moodNamesGreat { get; set; } = { "stroker", "stroker" };
        public string[] moodNamesNeutral { get; set; } = { "stroker", "stroker", "stroker", "stroker" };
        public string[] moodNamesBad { get; set; } = { "stroker", "stroker" };
    }
}
