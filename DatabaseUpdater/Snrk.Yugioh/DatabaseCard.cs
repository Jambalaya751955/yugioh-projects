using System;
using System.Collections.Generic;

namespace Snrk.Yugioh
{
    public class DatabaseCard
    {
        public string Name { get; set; }
        public string NameKana { get; set; }
        public string CardText { get; set; }
        public string PendulumEffect { get; set; }
        public string Id { get; set; }
        public string Attribute { get; set; }
        public string PendulumScale { get; set; }
        public string Level { get; set; }
        public string Atk { get; set; }
        public string Def { get; set; }
        public string Race { get; set; }
        public string Language { get; set; }
        public int Appearance { get; set; }
        public List<string> Types { get; set; }
        public List<Pack> Packs { get; set; }

        public DatabaseCard()
        {
            Types = new List<string>();
            Packs = new List<Pack>();
        }

        ~DatabaseCard() { Clear(); }
        public void Clear()
        {
            Types = null;
            for (int i = 0; i < Packs.Count; ++i)
            {
                Packs[i] = null;
            }
        }

        public DatabaseCard Clone()
        {
            return new DatabaseCard()
            {
                Name = this.Name,
                NameKana = this.NameKana,
                CardText = this.CardText,
                PendulumEffect = this.PendulumEffect,
                Id = this.Id,
                Attribute = this.Attribute,
                PendulumScale = this.PendulumScale,
                Level = this.Level,
                Atk = this.Atk,
                Def = this.Def,
                Race = this.Race,
                Language = this.Language,
                Appearance = this.Appearance,
                Types = new List<string>(this.Types),
                Packs = new List<Pack>(this.Packs)
            };
        }

        public class Pack
        {
            public string PackName { get; set; }
            public string EnglishPackName { get; set; }
            public string CardID { get; set; }
            public string Rarity { get; set; }
            public string ReleaseDate { get; set; }

            public Pack()
            {
            }

            public Pack(Pack Pack)
            {
                PackName = Pack.PackName;
                EnglishPackName = Pack.EnglishPackName;
                CardID = Pack.CardID;
                Rarity = Pack.Rarity;
                ReleaseDate = Pack.ReleaseDate;
            }
        }

        public class Effect
        {
            public string CardEffect { get; set; }
            public string PendulumEffect { get; set; }
            public string DeckMasterEffect { get; set; }

            public Effect(string cardEffect = null, string pendulumEffect = null, string deckMasterEffect = null)
            {
                this.CardEffect = cardEffect;
                this.PendulumEffect = pendulumEffect;
                this.DeckMasterEffect = deckMasterEffect;
            }
        }
    }
}
