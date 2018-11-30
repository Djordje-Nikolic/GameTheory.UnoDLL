using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TIG.AV.Karte;

namespace TIG.MakaoDLL
{
    public class Makao : IIgra
    {
        private GameContext trenutniKontekst = null;      
        private bool rukaSpremna = false;
        private bool talonSpreman = false;
        private CancellationTokenSource tokenSource;
        private int maxDubina;

        private Move bestMove;
        public IMove BestMove
        {
            get
            {
                return bestMove;
            }
            set
            {
                bestMove = (Move) value;
            }
        }

        private int nodeCounter = 0;

        /// <summary>
        /// Opcioni parametar predstavlja maksimalnu dubinu Iterative Deepening-a, u slucaju da je uneta vrednost negativna ili jednaka nuli, 
        /// podrazumeva se da se Iterative Deepening vrsi do rucnog prekida.
        /// </summary>
        /// <param name="maxDubina"></param>
        public Makao(int maxDubina = -1)
        {
            rukaSpremna = false;
            talonSpreman = false;
            trenutniKontekst = new GameContext();
            this.maxDubina = maxDubina;
        }
        public void Bacenekarte(List<Karta> karte, Boja boja, int BrojKarataProtivnika)
        {
            talonSpreman = true;

            trenutniKontekst.PostaviTalon(karte, boja, BrojKarataProtivnika);
        }
        public void BeginBestMove()
        {
            nodeCounter = 0;
            if (tokenSource != null)
            {
                tokenSource.Dispose();
            }
            tokenSource = new CancellationTokenSource();

            if (rukaSpremna && talonSpreman)
            {
                Task.Factory.StartNew(() => AlphaBetaID(tokenSource.Token));
            }
            else
                throw new Exception("Igra nije postavljena te igra ne moze poceti.");
        }
        private async void AlphaBetaID(CancellationToken token)
        {
            Tuple<int, Move> rezultat = null;

            if (maxDubina > 0)
            {
                for (int i = 1; i <= maxDubina; i++)
                {
                    rezultat = await IterateAB(trenutniKontekst, i, int.MinValue, int.MaxValue, true, token);

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                    else if (rezultat != null && rezultat.Item2 != null)
                    {
                        bestMove = rezultat.Item2;
                    }
                }
            }
            else
            {
                for (int i = 1; i >= 1; i++)
                {
                    rezultat = await IterateAB(trenutniKontekst, i, int.MinValue, int.MaxValue, true, token);

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                    else if (rezultat != null && rezultat.Item2 != null)
                    {
                        bestMove = rezultat.Item2;
                    }
                }
            }

            trenutniKontekst.OdigrajPotez(bestMove);        //Da li treba da izvrsimo nadjeni najbolji potez na trenutnom kontekstu ili je to posao engina?
        }
        private async Task<Tuple<int, Move>> IterateAB(GameContext kontekst, int dubina, int alfa, int beta, bool maxPlayer, CancellationToken token)
        {
            if (!token.IsCancellationRequested)
            {
                nodeCounter++;
                if (dubina == 0 || kontekst.IsTerminal(maxPlayer))
                {
                    return new Tuple<int, Move>(kontekst.RacunajVrednost(maxPlayer), kontekst.izvorniPotez);    //mozda treba novi potez da generisemo
                }

                int v;
                Tuple<int, Move> temp = null;   //da li ce se ikada vratiti null?
                Move bestMove = null;

                if (maxPlayer)
                {
                    v = int.MinValue;
                    foreach (var node in kontekst.GenerisiKontekste(maxPlayer))
                    {
                        temp = await IterateAB(node, dubina - 1, alfa, beta, !maxPlayer, token);

                        if (token.IsCancellationRequested)
                        {
                            break;
                        }

                        if (temp.Item1 > v)
                        {
                            v = temp.Item1;
                            bestMove = node.izvorniPotez;
                        }

                        if (v > alfa)
                        {
                            alfa = v;
                        }
                      
                        if (beta < alfa)
                        {
                            return new Tuple<int, Move>(alfa, bestMove);
                        }
                    }

                    return new Tuple<int, Move>(v, bestMove);
                }
                else
                {
                    v = int.MaxValue;
                    foreach (var node in kontekst.GenerisiKontekste(maxPlayer))
                    {
                        temp = await IterateAB(node, dubina - 1, alfa, beta, !maxPlayer, token);

                        if (token.IsCancellationRequested)
                        {
                            break;
                        }

                        if (temp.Item1 < v)
                        {
                            v = temp.Item1;
                            bestMove = node.izvorniPotez;
                        }

                        if (v < beta)
                        {
                            beta = v;
                        }

                        if (beta < alfa)
                        {
                            return new Tuple<int, Move>(beta, bestMove);
                        }
                    }

                    return new Tuple<int, Move>(v, bestMove);
                }
            }
            else
                return null;
        }
        public void EndBestMove()
        {
            tokenSource.Cancel();
        }
        public void KupioKarte(List<Karta> karte)
        {
            trenutniKontekst.KupiKarte(karte);
        }
        public void Reset()
        {
            rukaSpremna = false;
            talonSpreman = false;
            trenutniKontekst = new GameContext();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);  //Hopefully ovo nece previse da zakoci sistem
        }
        public void SetRuka(List<Karta> karte)
        {
            rukaSpremna = true;

            trenutniKontekst.PostaviRuku(karte);
        }
    }
}
