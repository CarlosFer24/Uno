using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;

namespace UNO
{
    public class Mazo
    {

        //cartas volteadas
        public List<Carta> Cartas { get; private set; }
        public List<Carta> PilaDescarte { get; private set; }

        public Mazo()
        {
            Cartas = new List<Carta>();
            PilaDescarte = new List<Carta>();
            CrearMazoCompleto();
            Barajar();
        }
        private void CrearMazoCompleto()
        {
            ColorCarta[] colores = { ColorCarta.Rojo, ColorCarta.Azul, ColorCarta.Verde, ColorCarta.Amarillo };

            foreach (ColorCarta color in colores)
            {
                Cartas.Add(new Carta(color, 0));

                for (int n = 1; n <= 9; n++)
                {
                    Cartas.Add(new Carta(color, n));
                    Cartas.Add(new Carta(color, n));
                }

                // Dos de cada carta especial de color
                for (int i = 0; i < 2; i++)
                {
                    Cartas.Add(new Carta(color, TipoCarta.Salta));
                    Cartas.Add(new Carta(color, TipoCarta.Reversa));
                    Cartas.Add(new Carta(color, TipoCarta.RobaDos));
                }
            }

            // 4 comodines de cada tipo
            for (int i = 0; i < 4; i++)
            {
                Cartas.Add(new Carta(ColorCarta.Comodin, TipoCarta.Comodin));
                Cartas.Add(new Carta(ColorCarta.Comodin, TipoCarta.ComodinRobaCuatro));
            }
        }
        public void Barajar()
        {
            Random rnd = new Random();
            int n = Cartas.Count;

            for (int i = n - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);

                Carta temp = Cartas[i];
                Cartas[i] = Cartas[j];
                Cartas[j] = temp;
            }
        }
        public Carta RobarCarta()
        {
            if (Cartas.Count == 0)
                return null;   //sin cartas en el mazo

            Carta carta = Cartas[0];
            Cartas.RemoveAt(0);
            return carta;
        }
        public bool HayCartas()
        {
            return Cartas.Count > 0;
        }
        public int CartasRestantes()
        {
            return Cartas.Count;
        }
        public void PonerEnDescarte(Carta carta)
        {
            PilaDescarte.Add(carta);
        }
        public Carta CartaSuperior()
        {
            if (PilaDescarte.Count == 0) return null;
            return PilaDescarte[PilaDescarte.Count - 1];
        }
        public List<Carta> Repartir(int cantidad)
        {
            List<Carta> mano = new List<Carta>();
            for (int i = 0; i < cantidad; i++)
            {
                Carta c = RobarCarta();
                if (c == null) break; //ya no hay cartas
                mano.Add(c);
            }
            return mano;
        }
    }
}