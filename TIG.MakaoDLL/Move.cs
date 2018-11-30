using System.Collections.Generic;
using TIG.AV.Karte;

namespace TIG.MakaoDLL
{
    class Move : IMove
    {
        public TipPoteza Tip { get; set; }

        private List<Karta> karte;
        public List<Karta> Karte
        {
            get { return karte; } //da li je bezbedno direktno vratiti ili je bolje vratiti novu listu
            set
            {
                karte = value;
            }
        }
        public Boja NovaBoja { get; set; }

        public int BrojKaznenih { get; private set; }
        public Move(TipPoteza tip, List<Karta> karte, Boja novaBoja)
        {
            Tip = tip;
            Karte = karte;
            NovaBoja = novaBoja;
            BrojKaznenih = 0;
        }
        public Move(TipPoteza tip, List<Karta> karte, Boja novaBoja, int brojKaznenih) : this(tip, karte, novaBoja)
        {
            BrojKaznenih = brojKaznenih;
        }
        public override string ToString()
        {
            string temp = string.Empty;
            temp += Tip.ToString();
            if (karte == null || karte.Count == 0)
                return temp;
            else
            {
                temp += ": ";

                foreach (var karta in karte)
                {
                    temp += karta.Boja.ToString() + karta.Broj + " ";
                }

                if (NovaBoja != Boja.Unknown)
                {
                    temp += NovaBoja.ToString();
                }

                return temp;
            }
        }
    }
}
