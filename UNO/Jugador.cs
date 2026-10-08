using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNO
{
    public class Jugador
    {
        public int NumeroAvatar { get; set; }
        public string Nombre { get; set; }
        public List<Carta> Mano { get; private set; }
        public bool DijoUno { get; set; }
        public int IdJugador { get; set; }

        //mi constructor de jugador
        public Jugador(string nombre, int numeroAvatar = 1)
        {
            Nombre = nombre;
            NumeroAvatar = numeroAvatar;
            Mano = new List<Carta>();
            DijoUno = false;
            IdJugador = -1;
        }

        public void AgregarCarta(Carta carta)
        {
            if (carta != null)
                Mano.Add(carta);
        }

        public void AgregarCartas(List<Carta> cartas)
        {
            if (cartas != null)
                Mano.AddRange(cartas);
        }
        public bool QuitarCarta(Carta carta)
        {
            return Mano.Remove(carta);
        }
        public int CantidadCartas()
        {
            return Mano.Count;
        }
        public bool Gano()
        {
            return Mano.Count == 0;
        }
        public bool TieneUnaCarta()
        {
            return Mano.Count == 1;
        }
        public List<Carta> CartasJugables(Carta cartaMesa)
        {
            List<Carta> jugables = new List<Carta>();

            foreach (Carta c in Mano)
            {
                if (c.SePuedeJugarSobre(cartaMesa))
                    jugables.Add(c);
            }

            return jugables;
        }
        public bool TieneJugada(Carta cartaMesa)
        {
            foreach (Carta c in Mano)
            {
                if (c.SePuedeJugarSobre(cartaMesa))
                    return true;
            }
            return false;
        }

        public override string ToString()
        {
            return $"{Nombre} ({Mano.Count} cartas)";
        }
    }
}