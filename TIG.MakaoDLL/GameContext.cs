using System;
using System.Collections.Generic;
using System.Linq;
using TIG.AV.Karte;

//To do - decembar
//1. Sta raditi sa sirenjem stabla nakon kupovine karte?
//2. Da li je algoritam za nalazenje mogucih poteza protivnika korektan
//3. Razdojiti evaluaciju na dva dela
//4. Implementirati yield u GeneratorPoteza i GeneratorKonteksta
//5. Isprobati pokretanje taskova za svaki kontekst pri racunanju prvog poteza, ili pokretanje nekoliko alphabeta razlicitih dubina u isto vreme
//6. Izbaci keceve i osmice iz obicnih nizova preostalih karata OBAVEZNO Done
//7. Ubaci sedmice u obicne nizove?

//To do - novembar
//1. Zavrsiti klasu Makao - Done
//2. Testirati pozivanje kroz klasu Makao sve sem Begin i End move - Done?
//3. Napisati generator poteza za protivnickog igraca - Done
//4. Napisati generator konteksta - Done
//5. Napisati bazicnu evaluaciju (materijalnu?) - Ne valja ali Done
//6. Testiraj alfabeta - Funkcionise

namespace TIG.MakaoDLL
{
    public class GameContext
    {
        internal struct ChainAndHandPair
        {
            public List<Karta> chain { get; }
            public List<Karta> hand { get; }
            public int noOfCardsLeft { get; }

            public ChainAndHandPair(List<Karta> chain, List<Karta> hand)
            {
                this.chain = chain;
                this.hand = hand;
                this.noOfCardsLeft = -1;
            }

            public ChainAndHandPair(List<Karta> chain, List<Karta> hand, int noOfCardsLeft)
            {
                this.chain = chain;
                this.hand = hand;
                this.noOfCardsLeft = noOfCardsLeft;
            }
        }

        private static List<String> preferences = new List<string> { "A", "8" };
        public readonly static String[] Broj = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

        private List<Karta> mojeKarte; //ubaciti broj nepoznatih karata? da se ne prebrojava svaki put
        private int brojProtKarata { get; set; }
        public Move izvorniPotez { get; private set; }

        private Boja trenutnaBoja;
        private Karta talon;
        private List<Karta> preostaleHerz;
        private List<Karta> preostaleKaro;
        private List<Karta> preostalePik;
        private List<Karta> preostaleTref;

        private List<Karta> preostaleKec;
        private List<Karta> preostaleZandar;
        private List<Karta> preostaleSedmice;
        private List<Karta> preostaleOsmice;

        private bool kupioKartu;
        private bool kupioKartuProtivnik;
        private int brojKaznenih;

        private int brojKupljenihKarti;   //FEJK
        private int brojProtKupljenihKarti; //isto fejk, skloni kad promenis nacin generisanja novih stanja

        public GameContext()
        {
            mojeKarte = new List<Karta>(6);
            brojProtKarata = 0;
            trenutnaBoja = Boja.Unknown;
            talon = new Karta()
            {
                Boja = Boja.Unknown
            };

            preostaleHerz = new List<Karta>(9);
            preostalePik = new List<Karta>(9);
            preostaleTref = new List<Karta>(9);
            preostaleKaro = new List<Karta>(9);

            preostaleKec = new List<Karta>(4);
            preostaleZandar = new List<Karta>(4);
            preostaleSedmice = new List<Karta>(4);
            preostaleOsmice = new List<Karta>(4);

            kupioKartu = false;
            kupioKartuProtivnik = false;

            brojKaznenih = 0;

            brojKupljenihKarti = 0;
            brojProtKupljenihKarti = 0;

            for (int i = 0; i < Broj.Length; i++)
            {

                if (i == 0)
                {
                    Karta referenca = new Karta() { Boja = Boja.Herz, Broj = Broj[i] };
                    preostaleKec.Add(referenca);

                    referenca = new Karta() { Boja = Boja.Karo, Broj = Broj[i] };
                    preostaleKec.Add(referenca);

                    referenca = new Karta() { Boja = Boja.Pik, Broj = Broj[i] };
                    preostaleKec.Add(referenca);

                    referenca = new Karta() { Boja = Boja.Tref, Broj = Broj[i] };
                    preostaleKec.Add(referenca);
                }
                else if (i == 6)
                {
                    Karta referenca = new Karta() { Boja = Boja.Herz, Broj = Broj[i] };
                    preostaleSedmice.Add(referenca);

                    referenca = new Karta() { Boja = Boja.Karo, Broj = Broj[i] };
                    preostaleSedmice.Add(referenca);

                    referenca = new Karta() { Boja = Boja.Pik, Broj = Broj[i] };
                    preostaleSedmice.Add(referenca);

                    referenca = new Karta() { Boja = Boja.Tref, Broj = Broj[i] };
                    preostaleSedmice.Add(referenca);
                }
                else if (i == 7)
                {
                    Karta referenca = new Karta() { Boja = Boja.Herz, Broj = Broj[i] };
                    preostaleOsmice.Add(referenca);

                    referenca = new Karta() { Boja = Boja.Karo, Broj = Broj[i] };
                    preostaleOsmice.Add(referenca);

                    referenca = new Karta() { Boja = Boja.Pik, Broj = Broj[i] };
                    preostaleOsmice.Add(referenca);

                    referenca = new Karta() { Boja = Boja.Tref, Broj = Broj[i] };
                    preostaleOsmice.Add(referenca);
                }
                else if (i == 10)
                {
                    Karta referenca = new Karta() { Boja = Boja.Herz, Broj = Broj[i] };
                    preostaleZandar.Add(referenca);

                    referenca = new Karta() { Boja = Boja.Karo, Broj = Broj[i] };
                    preostaleZandar.Add(referenca);

                    referenca = new Karta() { Boja = Boja.Pik, Broj = Broj[i] };
                    preostaleZandar.Add(referenca);

                    referenca = new Karta() { Boja = Boja.Tref, Broj = Broj[i] };
                    preostaleZandar.Add(referenca);
                }
                else //ovo ostaje uvek
                {
                    preostaleHerz.Add(new Karta() { Boja = Boja.Herz, Broj = Broj[i] });
                    preostaleKaro.Add(new Karta() { Boja = Boja.Karo, Broj = Broj[i] });
                    preostalePik.Add(new Karta() { Boja = Boja.Pik, Broj = Broj[i] });
                    preostaleTref.Add(new Karta() { Boja = Boja.Tref, Broj = Broj[i] });
                }
            }

        }
        private GameContext(GameContext kontekst, Move genPotez, bool maxPlayer)
        {
            izvorniPotez = genPotez;

            mojeKarte = new List<Karta>(kontekst.mojeKarte);
            preostaleHerz = new List<Karta>(kontekst.preostaleHerz);
            preostalePik = new List<Karta>(kontekst.preostalePik);
            preostaleKaro = new List<Karta>(kontekst.preostaleKaro);
            preostaleTref = new List<Karta>(kontekst.preostaleTref);

            preostaleKec = new List<Karta>(kontekst.preostaleKec);
            preostaleZandar = new List<Karta>(kontekst.preostaleZandar);
            preostaleSedmice = new List<Karta>(kontekst.preostaleSedmice);
            preostaleOsmice = new List<Karta>(kontekst.preostaleOsmice);


            brojKaznenih = genPotez.BrojKaznenih;
            brojKupljenihKarti = kontekst.brojKupljenihKarti;
            kupioKartu = false;
            kupioKartuProtivnik = false;

            if (genPotez.Karte != null && genPotez.Karte.Count != 0)
            {
                if (genPotez.NovaBoja != Boja.Unknown)
                    trenutnaBoja = genPotez.NovaBoja;
                else
                    trenutnaBoja = genPotez.Karte.Last().Boja;

                brojProtKarata = kontekst.brojProtKarata;

                talon = new Karta();
                talon.Boja = genPotez.Karte.Last().Boja;
                talon.Broj = genPotez.Karte.Last().Broj;
            }
            else
            {
                talon = new Karta();
                talon.Boja = kontekst.talon.Boja;
                talon.Broj = kontekst.talon.Broj;
                trenutnaBoja = kontekst.trenutnaBoja;
                brojProtKarata = kontekst.brojProtKarata;
            }

            if (maxPlayer)
            {
                //Novostavljene karte
                if (genPotez.Karte != null && genPotez.Karte.Count != 0)
                {
                    foreach (var karta in genPotez.Karte)
                    {
                        foreach (var mojaKarta in mojeKarte.Reverse<Karta>())
                        {
                            if (mojaKarta.Boja == karta.Boja && mojaKarta.Broj.Equals(karta.Broj))
                            {
                                mojeKarte.Remove(mojaKarta);
                            }
                        }
                    }
                }

                if (brojKaznenih > 0 && genPotez.Tip.HasFlag(TipPoteza.KupiKazneneKarte))
                {
                    brojKupljenihKarti += brojKaznenih;
                    brojKaznenih = 0;
                }
                else
                {
                    if (genPotez.Tip.HasFlag(TipPoteza.KupiKartu))
                    {
                        brojKupljenihKarti++;
                        kupioKartu = true;
                    }
                }

            }
            else
            {
                //Novostavljene karte
                if (genPotez.Karte != null && genPotez.Karte.Count != 0)
                {
                    brojProtKarata = kontekst.brojProtKarata - genPotez.Karte.Count;
                    IzbaciIzIzbora(genPotez.Karte);
                }

                if (brojKaznenih > 0 && genPotez.Tip.HasFlag(TipPoteza.KupiKazneneKarte))
                {
                    brojProtKarata += brojKaznenih;
                    brojProtKupljenihKarti += brojKaznenih; //To Be Removed
                    brojKaznenih = 0;
                }
                else
                {
                    if (genPotez.Tip.HasFlag(TipPoteza.KupiKartu))
                    {
                        brojProtKarata++;
                        brojProtKupljenihKarti++; //To Be Removed
                        kupioKartuProtivnik = true;
                    }
                }
            }
        }

        private void OrderHandByImportance()
        {
            //Seems to not increase performance
            mojeKarte = mojeKarte.OrderByDescending(item => preferences.IndexOf(item.Broj)).ToList();
        }
        public void PostaviRuku(List<Karta> karte)
        {
            IzbaciIzIzbora(karte);

            mojeKarte = new List<Karta>(karte.Count);
            foreach (var karta in karte)
            {
                Karta temp = new Karta();
                temp.Boja = karta.Boja;
                temp.Broj = karta.Broj;
                mojeKarte.Add(temp);
            }

            OrderHandByImportance();
        }
        public void KupiKarte(List<Karta> karte)
        {
            if (karte.Count == 1)
            {
                //if (kupioKartu == false)      //Workaround, resiti kada se ubaci generator kupovine
                //{
                    Karta temp = new Karta();
                    temp.Boja = karte.Last().Boja;
                    temp.Broj = karte.Last().Broj;

                    mojeKarte.Add(temp);
                    IzbaciIzIzbora(temp);
                    kupioKartu = true;
                //
                //OrderHandByImportance();
            }
            else if (karte.Count > 1)
            {
                List<Karta> tempList = new List<Karta>(karte.Count);
                foreach (var kar in karte)
                {
                    Karta temp = new Karta();
                    temp.Boja = kar.Boja;
                    temp.Broj = kar.Broj;
                    tempList.Add(temp);
                }
                mojeKarte.AddRange(tempList);
                IzbaciIzIzbora(karte);
                brojKaznenih = 0;

                //OrderHandByImportance();
            }
        }
        public void PostaviTalon(List<Karta> karte, Boja boja, int brojProtivnikovihKarata)
        {
            if (karte != null && karte.Count > 0)
            {
                if (boja != Boja.Unknown)
                    trenutnaBoja = boja;
                else
                    trenutnaBoja = karte.Last().Boja;

                brojProtKarata = brojProtivnikovihKarata;
                IzbaciIzIzbora(karte);

                if (karte.Last().Broj.Equals(Broj[6]))
                {
                    brojKaznenih += 2;
                }
                else if (karte.Last().Broj.Equals(Broj[1]) && karte.Last().Boja == Boja.Tref)
                {
                    brojKaznenih += 4;
                }

                Karta temp = new Karta();
                temp.Boja = karte.Last().Boja;
                temp.Broj = karte.Last().Broj;
                talon = temp;
            }
            else
            {
                brojProtKarata = brojProtivnikovihKarata;
            }

            kupioKartu = false;
        }
        public void OdigrajPotez(Move bestMove)
        {
            if (bestMove.Karte != null && bestMove.Karte.Count > 0)
            {
                foreach (var karta in bestMove.Karte)
                {
                    foreach (var mojaKarta in mojeKarte.Reverse<Karta>())
                    {
                        if (mojaKarta.Boja == karta.Boja && mojaKarta.Broj.Equals(karta.Broj))
                        {
                            mojeKarte.Remove(mojaKarta);
                        }
                    }
                }

                if (bestMove.NovaBoja != Boja.Unknown)
                    trenutnaBoja = bestMove.NovaBoja;
                else
                    trenutnaBoja = bestMove.Karte.Last().Boja;

                brojKaznenih = bestMove.BrojKaznenih;

                talon = new Karta();
                talon.Boja = bestMove.Karte.Last().Boja;
                talon.Broj = bestMove.Karte.Last().Broj;
            }

            //if (bestMove.Tip.HasFlag(TipPoteza.KupiKazneneKarte))
            //{//Regulise slucaj kada se generise kontekst nakon Kupovine kaznenih karti (koristan pri igranju sam sa sobom)
            //    brojKaznenih = 0;
            //}

            if (bestMove.Tip.HasFlag(TipPoteza.KrajPoteza))
            {
                kupioKartu = false;
            }

            //else if (bestMove.Tip.HasFlag(TipPoteza.KupiKartu))
            //{//Regulise slucaj kupovine karte (koristan pri igranju sam sa sobom)
            //    kupioKartu = true;
            //}
        }

        private void IzbaciIzIzbora(Karta karta)
        {
            if (karta.Broj == Broj[0])
            {
                foreach (var value in preostaleKec.Reverse<Karta>())
                {
                    if (value.Boja == karta.Boja)
                    {
                        preostaleKec.Remove(value);
                        break;
                    }
                }
            }
            else if (karta.Broj == Broj[6])
            {
                foreach (var value in preostaleSedmice.Reverse<Karta>())
                {
                    if (value.Boja == karta.Boja)
                    {
                        preostaleSedmice.Remove(value);
                        break;
                    }
                }
            }
            else if (karta.Broj == Broj[7])      
            {
                foreach (var value in preostaleOsmice.Reverse<Karta>())
                {
                    if (value.Boja == karta.Boja)
                    {
                        preostaleOsmice.Remove(value);
                        break;
                    }
                }
            }
            else if (karta.Broj == Broj[10])
            {
                foreach (var value in preostaleZandar.Reverse<Karta>())
                {
                    if (value.Boja == karta.Boja)
                    {
                        preostaleZandar.Remove(value);
                        break;
                    }
                }
            }
            else
            {
                switch (karta.Boja)             //Reverse omogucava da izbacim element iz liste dok iteriram kroz nju, moglo bi se ovo uraditi i sa for i RemoveAt ali time gubim mogucnost remove po referenci za ostale dodatne liste
                {
                    case Boja.Herz:
                        foreach (var value in preostaleHerz.Reverse<Karta>())
                        {
                            if (value.Broj == karta.Broj)
                            {
                                preostaleHerz.Remove(value);
                                break;
                            }
                        }
                        break;
                    case Boja.Karo:
                        foreach (var value in preostaleKaro.Reverse<Karta>())
                        {
                            if (value.Broj == karta.Broj)
                            {
                                preostaleKaro.Remove(value);
                                break;
                            }
                        }
                        break;
                    case Boja.Pik:
                        foreach (var value in preostalePik.Reverse<Karta>())
                        {
                            if (value.Broj == karta.Broj)
                            {
                                preostalePik.Remove(value);
                                break;
                            }
                        }
                        break;
                    case Boja.Tref:
                        foreach (var value in preostaleTref.Reverse<Karta>())
                        {
                            if (value.Broj == karta.Broj)
                            {
                                preostaleTref.Remove(value);
                                break;
                            }
                        }
                        break;
                }
            }

        }
        private void IzbaciIzIzbora(List<Karta> karte)
        {
            foreach (var karta in karte)
                IzbaciIzIzbora(karta);
        }

        private void DodajPotezeZandar(Karta karta, ref List<Move> lista, int count)
        {
            List<Karta> output = new List<Karta>(1);
            output.Add(karta);

                if (count == 2)
                {
                    lista.Add(new Move(TipPoteza.KrajPotezaBacikartu | TipPoteza.PromeniBoju | TipPoteza.Poslednja, output, Boja.Herz));
                    lista.Add(new Move(TipPoteza.KrajPotezaBacikartu | TipPoteza.PromeniBoju | TipPoteza.Poslednja, output, Boja.Pik));
                    lista.Add(new Move(TipPoteza.KrajPotezaBacikartu | TipPoteza.PromeniBoju | TipPoteza.Poslednja, output, Boja.Tref));
                    lista.Add(new Move(TipPoteza.KrajPotezaBacikartu | TipPoteza.PromeniBoju | TipPoteza.Poslednja, output, Boja.Karo));
                }
                else if (count > 2 || count == 1)
                {
                    lista.Add(new Move(TipPoteza.KrajPotezaBacikartu | TipPoteza.PromeniBoju, output, Boja.Herz));
                    lista.Add(new Move(TipPoteza.KrajPotezaBacikartu | TipPoteza.PromeniBoju, output, Boja.Pik));
                    lista.Add(new Move(TipPoteza.KrajPotezaBacikartu | TipPoteza.PromeniBoju, output, Boja.Tref));
                    lista.Add(new Move(TipPoteza.KrajPotezaBacikartu | TipPoteza.PromeniBoju, output, Boja.Karo));
                }
        }
        private void DodajPotezeZandar(List<Karta> karte, ref List<Move> lista, int count)
        {
                if ((count - (karte.Count - 1)) > 2 || (count - (karte.Count - 1)) == 1)
                {
                    lista.Add(new Move(TipPoteza.KrajPotezaBacikartu | TipPoteza.PromeniBoju, karte, Boja.Herz));
                    lista.Add(new Move(TipPoteza.KrajPotezaBacikartu | TipPoteza.PromeniBoju, karte, Boja.Pik));
                    lista.Add(new Move(TipPoteza.KrajPotezaBacikartu | TipPoteza.PromeniBoju, karte, Boja.Tref));
                    lista.Add(new Move(TipPoteza.KrajPotezaBacikartu | TipPoteza.PromeniBoju, karte, Boja.Karo));
                }
                else if ((count - (karte.Count - 1)) == 2)
                {
                    lista.Add(new Move(TipPoteza.KrajPotezaBacikartu | TipPoteza.PromeniBoju | TipPoteza.Poslednja, karte, Boja.Herz));
                    lista.Add(new Move(TipPoteza.KrajPotezaBacikartu | TipPoteza.PromeniBoju | TipPoteza.Poslednja, karte, Boja.Pik));
                    lista.Add(new Move(TipPoteza.KrajPotezaBacikartu | TipPoteza.PromeniBoju | TipPoteza.Poslednja, karte, Boja.Tref));
                    lista.Add(new Move(TipPoteza.KrajPotezaBacikartu | TipPoteza.PromeniBoju | TipPoteza.Poslednja, karte, Boja.Karo));
                }
        }
        private void DodajPotezSlabaKarta(Karta karta, ref List<Move> lista, int count)
        {
            List<Karta> output = new List<Karta>(1);
            output.Add(karta);

                if (count == 2)
                {
                    lista.Add(new Move(TipPoteza.KrajPotezaBacikartu | TipPoteza.Poslednja, output, Boja.Unknown));
                }
                else if (count > 2 || count == 1)
                {
                    lista.Add(new Move(TipPoteza.KrajPotezaBacikartu, output, Boja.Unknown));
                }
        }
        private void DodajPotezSlabaKarta(List<Karta> karte, ref List<Move> lista, int count)
        {
            if ((count - (karte.Count - 1)) > 2 || (count - (karte.Count - 1)) == 1)
            {
                lista.Add(new Move(TipPoteza.KrajPotezaBacikartu, karte, Boja.Unknown));
            }
            else if ((count - (karte.Count - 1)) == 2)
            {
                lista.Add(new Move(TipPoteza.KrajPotezaBacikartu | TipPoteza.Poslednja, karte, Boja.Unknown));
            }
        }

        public List<GameContext> GenerisiKontekste(bool maxPlayer) 
        {
            List<Move> potezi = GenerisiMogucePoteze(maxPlayer).OrderByDescending(move => move.Karte != null ? move.Karte.Count : 0).ToList();
            List<GameContext> konteksti = new List<GameContext>(potezi.Count);

            foreach (var potez in potezi)
            {
                konteksti.Add(new GameContext(this, potez, maxPlayer));
            }
            return konteksti;
        }     
        public List<Move> GenerisiMogucePoteze(bool maxPlayer)
        {
            List<Move> potezi = new List<Move>();

            if (!IsTerminal(maxPlayer))  //vrvtno ne treba
            {
                if (maxPlayer)
                {
                    if (trenutnaBoja == Boja.Tref && talon.Broj.Equals(Broj[1]))            //na talonu mala dvojka
                    {
                        if (brojKaznenih != 0)      //bacio je malu dvojku u poslednjem potezu
                        {
                            potezi.Add(new Move(TipPoteza.KupiKazneneKarte, null, Boja.Unknown, brojKaznenih));
                            return potezi;
                        }
                    }
                    else if (talon.Broj.Equals(Broj[6]))        //na talonu je sedmica
                    {
                        if (brojKaznenih != 0)      //bacio je sedmicu u poslednjem potezu
                        {
                            List<Karta> sedmica = new List<Karta>(1);
                            foreach (var karta in mojeKarte)
                            {
                                if (karta.Broj.Equals(Broj[6]))
                                {
                                    sedmica.Add(karta);
                                    if (mojeKarte.Count == 2)
                                        potezi.Add(new Move(TipPoteza.KrajPotezaBacikartu | TipPoteza.Poslednja, new List<Karta>(sedmica), Boja.Unknown, brojKaznenih + 2));
                                    else if (mojeKarte.Count > 2 || mojeKarte.Count == 1)
                                        potezi.Add(new Move(TipPoteza.KrajPotezaBacikartu, new List<Karta>(sedmica), Boja.Unknown, brojKaznenih + 2));

                                    sedmica.Clear();
                                }
                            }
                            potezi.Add(new Move(TipPoteza.KupiKazneneKarte, null, Boja.Unknown, brojKaznenih));
                            return potezi;
                        }
                    }

                    foreach (var karta in mojeKarte)
                    {
                        if (karta.Broj.Equals(Broj[0]) || karta.Broj.Equals(Broj[7]))
                        {
                            if (karta.Boja == trenutnaBoja || talon.Broj.Equals(karta.Broj))
                            {
                                DodajPotezeA8Kombinacije(karta, mojeKarte, ref potezi);
                            }
                        }
                        else 
                        {
                            if (karta.Broj.Equals(Broj[10]))
                            {
                                DodajPotezeZandar(karta, ref potezi, mojeKarte.Count);
                            }
                            else
                            {
                                if (karta.Boja == trenutnaBoja || talon.Broj.Equals(karta.Broj))
                                {
                                    DodajPotezSlabaKarta(karta, ref potezi, mojeKarte.Count);
                                }
                            }

                        }
                    }

                    if (kupioKartu == false)
                    {
                        potezi.Add(new Move(TipPoteza.KupiKartu, null, Boja.Unknown));
                        //kupioKartu = true;    Nema svrhu
                    }
                    else
                    {
                        potezi.Add(new Move(TipPoteza.KrajPoteza, null, Boja.Unknown));
                    }
                }
                else
                {
                    //Odgovor na kaznene
                    if (brojKaznenih != 0)
                    {
                        if (talon.Boja == Boja.Tref && talon.Broj.Equals(Broj[1]))
                        {
                            potezi.Add(new Move(TipPoteza.KupiKazneneKarte, null, Boja.Unknown, 4));    //Jel treba ovde da naznacimo koliko karta ili moze samo da se pogleda u brojKaznenih?
                            brojKaznenih = 0;
                            return potezi;
                        }
                        else if (talon.Broj.Equals(Broj[6]))
                        {
                            List<Karta> sedmica = new List<Karta>(1);
                            foreach (var karta in preostaleSedmice)
                            {
                                sedmica.Add(karta);
                                if (brojProtKarata == 2)
                                    potezi.Add(new Move(TipPoteza.KrajPotezaBacikartu | TipPoteza.Poslednja, new List<Karta>(sedmica), Boja.Unknown, brojKaznenih + 2));
                                else if (brojProtKarata > 2 || brojProtKarata == 1)
                                    potezi.Add(new Move(TipPoteza.KrajPotezaBacikartu, new List<Karta>(sedmica), Boja.Unknown, brojKaznenih + 2));

                                sedmica.Clear();
                            }

                            potezi.Add(new Move(TipPoteza.KupiKazneneKarte, null, Boja.Unknown, brojKaznenih));   //Jel treba ovde da naznacimo koliko karta ili moze samo da se pogleda u brojKaznenih?
                            brojKaznenih = 0;
                            return potezi;
                        }
                    }

                    //Obicne karte bez A,7,8,J
                    switch (trenutnaBoja)
                    {
                        case Boja.Herz:
                            foreach (var karta in preostaleHerz)
                            {
                                    DodajPotezSlabaKarta(karta, ref potezi, brojProtKarata);
                            }
                            break;
                        case Boja.Karo:
                            foreach (var karta in preostaleKaro)
                            {
                                DodajPotezSlabaKarta(karta, ref potezi, brojProtKarata);
                            }
                            break;
                        case Boja.Pik:
                            foreach (var karta in preostalePik)
                            {
                                DodajPotezSlabaKarta(karta, ref potezi, brojProtKarata);
                            }
                            break;
                        case Boja.Tref:
                            foreach (var karta in preostaleTref)
                            {
                                DodajPotezSlabaKarta(karta, ref potezi, brojProtKarata);
                            }
                            break;
                    }
                    //Sve J
                    foreach (var karta in preostaleZandar)
                    {
                        DodajPotezeZandar(karta, ref potezi, brojProtKarata);
                    }
                    //Sve 7
                    List<Karta> output = new List<Karta>(1);
                    foreach (var karta in preostaleSedmice)
                    {
                        if (karta.Boja == trenutnaBoja)
                        {
                            output.Add(karta);
                            if (brojProtKarata == 2)
                                potezi.Add(new Move(TipPoteza.KrajPotezaBacikartu | TipPoteza.Poslednja, new List<Karta>(output), Boja.Unknown, brojKaznenih + 2));
                            else if (brojProtKarata > 2 || brojProtKarata == 1)
                                potezi.Add(new Move(TipPoteza.KrajPotezaBacikartu, new List<Karta>(output), Boja.Unknown, brojKaznenih + 2));

                            output.Clear();
                        }
                    }

                    List<Karta> container = new List<Karta>(preostaleHerz.Count + preostaleKaro.Count + preostaleKec.Count + preostaleOsmice.Count + preostalePik.Count + preostaleTref.Count + preostaleSedmice.Count + preostaleZandar.Count);
                    container.AddRange(preostaleKec);
                    container.AddRange(preostaleOsmice);
                    container.AddRange(preostaleHerz);
                    container.AddRange(preostalePik);
                    container.AddRange(preostaleKaro);
                    container.AddRange(preostaleTref);
                    container.AddRange(preostaleZandar);
                    container.AddRange(preostaleSedmice);

                    for (int i = 0; i < preostaleKec.Count + preostaleOsmice.Count; i++)
                    {
                        if (container[i].Boja == trenutnaBoja)
                        {
                            DodajPotezeA8KombinacijeProtivnik(container[i], container, ref potezi);
                        }
                    }

                    //Default odgovor kupovine
                    if (kupioKartuProtivnik == false)
                    {
                        potezi.Add(new Move(TipPoteza.KupiKartu, null, Boja.Unknown));
                    }
                }
            }

            return potezi;
        }
        private List<Move> SortMovesDescending(List<Move> source)
        {
            return source.OrderByDescending(move => move.Karte != null ? move.Karte.Count : 0).ToList();
        }
        private void DodajPotezeA8KombinacijeProtivnik(Karta start, List<Karta> moguceKarte, ref List<Move> potezi)
        {
            List<Karta> lastHand = new List<Karta>(moguceKarte);
            lastHand.Remove(start);
            List<Karta> zaSlanja;       //Pretpostavljamo da se ostali iz izvora ne pojavljuju u mojeKarte jer su ranije izvuceni
            List<Karta> lastChain;
            ChainAndHandPair lastPair;
            int preostaliBrojKarata = brojProtKarata - 1;

            Stack<ChainAndHandPair> unfinishedMoves = new Stack<ChainAndHandPair>();

            unfinishedMoves.Push(new ChainAndHandPair(new List<Karta>() { start }, new List<Karta>(lastHand), preostaliBrojKarata));
            //Setup za prvu iteraciju

            while (unfinishedMoves.Count > 0)
            {
                lastPair = unfinishedMoves.Pop();
                lastChain = lastPair.chain;
                lastHand = lastPair.hand;
                preostaliBrojKarata = lastPair.noOfCardsLeft;

                if (lastChain.Last().Broj.Equals(Broj[7]))          //Ako je trenutna prva karta osmica legalan potez je zavrsiti potez
                {
                    if (preostaliBrojKarata > 1 || preostaliBrojKarata == 0)
                        potezi.Add(new Move(TipPoteza.KrajPotezaBacikartu, new List<Karta>(lastChain), Boja.Unknown));
                    else if (preostaliBrojKarata == 1)
                        potezi.Add(new Move(TipPoteza.KrajPotezaBacikartu | TipPoteza.Poslednja, new List<Karta>(lastChain), Boja.Unknown));
                }

                if (lastChain.Last().Broj.Equals(Broj[0]))       //Ako je trenutna prva karta kec legalan potez je kupiti kartu
                {
                    if (preostaliBrojKarata > 1 || preostaliBrojKarata == 0)
                        potezi.Add(new Move(TipPoteza.BacaKartu | TipPoteza.KupiKartu, new List<Karta>(lastChain), Boja.Unknown));
                    else if (preostaliBrojKarata == 1)
                        potezi.Add(new Move(TipPoteza.BacaKartu | TipPoteza.Poslednja | TipPoteza.KupiKartu, new List<Karta>(lastChain), Boja.Unknown));
                }

                if (preostaliBrojKarata > 0)
                {
                    lastHand = lastHand.OrderBy(item => preferences.IndexOf(item.Broj)).ToList();
                    foreach (var karta in lastHand)
                    {
                        zaSlanja = new List<Karta>(lastChain);

                        if (!karta.Broj.Equals(Broj[0]) && !karta.Broj.Equals(Broj[7]))
                        {
                            if (karta.Broj.Equals(Broj[10]))
                            {
                                zaSlanja.Add(karta);
                                DodajPotezeZandar(zaSlanja, ref potezi, preostaliBrojKarata + zaSlanja.Count - 1);
                            }
                            else if (zaSlanja.Last().Boja == karta.Boja)
                            {
                                zaSlanja.Add(karta);
                                DodajPotezSlabaKarta(zaSlanja, ref potezi, preostaliBrojKarata + zaSlanja.Count - 1);
                            }
                        }
                        else if (karta.Boja == zaSlanja.Last().Boja || karta.Broj.Equals(zaSlanja.Last().Broj))
                        {
                            zaSlanja.Add(karta);
                            List<Karta> temp = new List<Karta>(lastHand);
                            temp.Remove(karta);
                            unfinishedMoves.Push(new ChainAndHandPair(zaSlanja, temp, preostaliBrojKarata - 1));
                        }
                    }
                }
            }
        }
        private void DodajPotezeA8Kombinacije(Karta start, List<Karta> mojeKarte, ref List<Move> potezi)
        {
            List<Karta> lastHand = new List<Karta>(mojeKarte);
            lastHand.Remove(start);     //Pretpostavljamo da se ostali iz izvora ne pojavljuju u mojeKarte jer su ranije izvuceni
            List<Karta> zaSlanja;
            List<Karta> lastChain;
            ChainAndHandPair lastPair;

            Stack<ChainAndHandPair> unfinishedMoves = new Stack<ChainAndHandPair>();

            unfinishedMoves.Push(new ChainAndHandPair(new List<Karta>() { start }, new List<Karta>(lastHand)));
            //Setup za prvu iteraciju
            while (unfinishedMoves.Count > 0)
            {
                lastPair = unfinishedMoves.Pop();
                lastChain = lastPair.chain;
                lastHand = lastPair.hand;

                if (lastChain.Last().Broj.Equals(Broj[7]))          //Ako je trenutna prva karta osmica legalan potez je zavrsiti potez
                {
                    if (lastHand.Count > 1 || lastHand.Count == 0)
                        potezi.Add(new Move(TipPoteza.KrajPotezaBacikartu, new List<Karta>(lastChain), Boja.Unknown));
                    else if (lastHand.Count == 1)
                        potezi.Add(new Move(TipPoteza.KrajPotezaBacikartu | TipPoteza.Makao, new List<Karta>(lastChain), Boja.Unknown));
                }
                else if (lastChain.Last().Broj.Equals(Broj[0]))       //Ako je trenutna prva karta kec legalan potez je kupiti kartu
                {
                    if (lastHand.Count > 1 || lastHand.Count == 0)
                        potezi.Add(new Move(TipPoteza.BacaKartu | TipPoteza.KupiKartu, new List<Karta>(lastChain), Boja.Unknown));
                    else if (lastHand.Count == 1)
                        potezi.Add(new Move(TipPoteza.BacaKartu | TipPoteza.Makao | TipPoteza.KupiKartu, new List<Karta>(lastChain), Boja.Unknown));
                }

                lastHand = lastHand.OrderBy(item => preferences.IndexOf(item.Broj)).ToList();
                foreach (var karta in lastHand)
                {
                    zaSlanja = new List<Karta>(lastChain);

                    if (!karta.Broj.Equals(Broj[0]) && !karta.Broj.Equals(Broj[7]))
                    {
                        if (karta.Broj.Equals(Broj[10]))
                        {
                            zaSlanja.Add(karta);
                            DodajPotezeZandar(zaSlanja, ref potezi, lastHand.Count + zaSlanja.Count - 1);
                        }
                        else if (zaSlanja.Last().Boja == karta.Boja)
                        {
                            zaSlanja.Add(karta);
                            DodajPotezSlabaKarta(zaSlanja, ref potezi, lastHand.Count + zaSlanja.Count - 1);
                        }
                    }
                    else
                    {
                        if (karta.Boja == zaSlanja.Last().Boja || karta.Broj.Equals(zaSlanja.Last().Broj))
                        {
                            zaSlanja.Add(karta);
                            List<Karta> temp = new List<Karta>(lastHand);
                            temp.Remove(karta);
                            unfinishedMoves.Push(new ChainAndHandPair(zaSlanja, temp));
                        }
                    }
                }
            }
        }

        //NE VALJA EVALUACIJA
        public int RacunajVrednost(bool player)
        {
            int result = 0;
            if ((mojeKarte.Count + brojKupljenihKarti) == 0)
            {
                return 100;
            }
            else if ((brojProtKarata + brojProtKupljenihKarti) == 0)
            {
                return -100;
            }
            else if (!kupioKartu)
            {
                result = (brojProtKarata - mojeKarte.Count) * 30;
                result -= (brojKupljenihKarti - brojProtKupljenihKarti) * 10;

                for (int i = 0; i < mojeKarte.Count; i++)
                {
                    if (trenutnaBoja == mojeKarte[i].Boja)
                        result += 20;
                }
            }
            else if (izvorniPotez.Tip == TipPoteza.KrajPoteza)
            {
                result = -1000;
            }
            else
            {
                result = -2000;
            }
            return result;
        }
        public bool IsTerminal(bool player)
        {
            if (mojeKarte.Count == 0 || brojKupljenihKarti != 0) //drugi uslov da se skloni
                return true;
            else if (brojProtKarata == 0 || brojProtKupljenihKarti != 0) //drugi uslov da se skloni
                return true;
            else return false;
        }
    }
}
