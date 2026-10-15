using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNO
{
    public class Juego
    {
        public List<Jugador> Jugadores { get; private set; }
        public Mazo Mazo { get; private set; }
        public int TurnoActual { get; private set; }
        public int Direccion { get; private set; }
        public ColorCarta ColorActivo { get; private set; }
        public Jugador Ganador { get; private set; }
        public List<string> Log { get; private set; }
        public int CartasPendientesRobar { get; private set; }
        public TipoCarta? TipoPendiente { get; private set; }

        private bool _esperandoUno = false;
        private Jugador _jugadorPendienteUno = null;

        public bool EsperandoUno => _esperandoUno;
        public Jugador JugadorPendienteUno => _jugadorPendienteUno;

        public Juego(List<string> nombresJugadores)
        {
            Jugadores = new List<Jugador>();
            Mazo = new Mazo();
            Log = new List<string>();
            TurnoActual = 0;
            Direccion = 1;
            Ganador = null;
            CartasPendientesRobar = 0;
            TipoPendiente = null;

            foreach (string nombre in nombresJugadores)
            {
                Jugadores.Add(new Jugador(nombre));
            }

            foreach (Jugador j in Jugadores)
            {
                j.AgregarCartas(Mazo.Repartir(7));
            }

            Carta primera = Mazo.RobarCarta();
            while (primera != null &&
                   (primera.Tipo == TipoCarta.Comodin || primera.Tipo == TipoCarta.ComodinRobaCuatro))
            {
                Mazo.Cartas.Add(primera);
                Mazo.Barajar();
                primera = Mazo.RobarCarta();
            }

            Mazo.PonerEnDescarte(primera);
            ColorActivo = primera.Color;

            Log.Add("── Inicia la partida ──");
            Log.Add($"Carta inicial en la mesa: {primera}");
            Log.Add($"Turno de: {JugadorActual().Nombre}");

            AplicarEfectoCartaInicial(primera);
        }

        public Jugador JugadorActual()
        {
            return Jugadores[TurnoActual];
        }

        public Carta CartaMesa()
        {
            return Mazo.CartaSuperior();
        }

        public bool JuegoTerminado()
        {
            return Ganador != null;
        }

        public bool SePuedeJugar(Carta carta)
        {
            Carta mesa = CartaMesa();

            if (mesa == null) return true;

            if (carta.Tipo == TipoCarta.Comodin || carta.Tipo == TipoCarta.ComodinRobaCuatro)
                return true;

            if (CartasPendientesRobar > 0)
            {
                if (TipoPendiente == TipoCarta.RobaDos)
                {
                    if (carta.Tipo != TipoCarta.RobaDos && carta.Tipo != TipoCarta.ComodinRobaCuatro)
                        return false;
                    return true;
                }
                else if (TipoPendiente == TipoCarta.ComodinRobaCuatro)
                {
                    if (carta.Tipo != TipoCarta.ComodinRobaCuatro)
                        return false;
                    return true;
                }
            }

            if (mesa.Tipo == TipoCarta.Comodin || mesa.Tipo == TipoCarta.ComodinRobaCuatro)
            {
                if (carta.Color == ColorActivo)
                    return true;

                if (carta.Tipo == mesa.Tipo && carta.Tipo != TipoCarta.Numero)
                    return true;

                return false;
            }

            return carta.SePuedeJugarSobre(mesa);
        }

        private void AvanzarTurno()
        {
            int total = Jugadores.Count;
            TurnoActual = (TurnoActual + Direccion + total) % total;
        }

        public void PasarTurno()
        {
            AvanzarTurno();
            Log.Add($"Turno de: {JugadorActual().Nombre}");
        }

        public bool JugarCarta(Carta carta, ColorCarta colorElegido = ColorCarta.Comodin)
        {
            Jugador actual = JugadorActual();

            if (!actual.Mano.Contains(carta))
                return false;

            if (!SePuedeJugar(carta))
                return false;

            actual.QuitarCarta(carta);
            Mazo.PonerEnDescarte(carta);

            if (carta.Tipo == TipoCarta.Comodin || carta.Tipo == TipoCarta.ComodinRobaCuatro)
            {
                ColorActivo = (colorElegido == ColorCarta.Comodin) ? ColorCarta.Rojo : colorElegido;
                Log.Add($"{actual.Nombre} juega {carta} y elige color {ColorActivo}");
            }
            else
            {
                ColorActivo = carta.Color;
                Log.Add($"{actual.Nombre} juega {carta}");
            }

            if (actual.Gano())
            {
                Ganador = actual;
                Log.Add($"¡{actual.Nombre} GANÓ la partida!");
                return true;
            }

            // ✅ Si le queda 1 carta y no dijo UNO → activar espera de 5 seg
            if (actual.TieneUnaCarta() && !actual.DijoUno)
            {
                Log.Add($"{actual.Nombre} tiene 1 carta. Tiene 5 segundos para decir ¡UNO!");
                _esperandoUno = true;
                _jugadorPendienteUno = actual;
                return true;   // ✅ NO pasar turno todavía
            }

            // Si no hay espera de UNO, pasar turno normal
            AplicarEfecto(carta);
            return true;
        }

        public void AplicarEfecto(Carta carta)
        {
            switch (carta.Tipo)
            {
                case TipoCarta.Salta:
                    Log.Add($"{JugadorActual().Nombre} jugó Salta.");
                    AvanzarTurno();
                    PasarTurno();
                    break;

                case TipoCarta.Reversa:
                    Direccion *= -1;
                    Log.Add($"Reversa. Nueva dirección: {(Direccion == 1 ? "horario" : "antihorario")}");
                    if (Jugadores.Count == 2)
                    {
                        Log.Add("(Con 2 jugadores, Reversa actúa como Salta)");
                        AvanzarTurno();
                    }
                    PasarTurno();
                    break;

                case TipoCarta.RobaDos:
                    CartasPendientesRobar += 2;
                    TipoPendiente = TipoCarta.RobaDos;
                    Log.Add($"+2 acumulado. Pendientes: {CartasPendientesRobar}");
                    PasarTurno();
                    break;

                case TipoCarta.ComodinRobaCuatro:
                    CartasPendientesRobar += 4;
                    TipoPendiente = TipoCarta.ComodinRobaCuatro;
                    Log.Add($"+4 acumulado. Pendientes: {CartasPendientesRobar}");
                    PasarTurno();
                    break;

                default:
                    PasarTurno();
                    break;
            }
        }

        private void AplicarEfectoCartaInicial(Carta carta)
        {
            switch (carta.Tipo)
            {
                case TipoCarta.Salta:
                    Log.Add("La primera carta es Salta. El primer jugador pierde turno.");
                    AvanzarTurno();
                    Log.Add($"Turno de: {JugadorActual().Nombre}");
                    break;

                case TipoCarta.Reversa:
                    Direccion *= -1;
                    Log.Add("La primera carta es Reversa. Se invierte el sentido.");
                    break;

                case TipoCarta.RobaDos:
                    CartasPendientesRobar += 2;
                    TipoPendiente = TipoCarta.RobaDos;
                    Log.Add("La primera carta es +2. El primer jugador deberá robar 2 o acumular.");
                    break;
            }
        }

        public Carta RobarCartaActual()
        {
            Jugador actual = JugadorActual();

            if (!Mazo.HayCartas())
            {
                Log.Add($"{actual.Nombre} intentó robar, pero el mazo está agotado. Pasa turno.");
                PasarTurno();
                return null;
            }

            Carta robada = Mazo.RobarCarta();
            actual.AgregarCarta(robada);
            Log.Add($"{actual.Nombre} roba 1 carta. ({actual.CantidadCartas()} en mano)");
            return robada;
        }

        public void RobarParaJugador(Jugador jugador, int cantidad)
        {
            for (int i = 0; i < cantidad; i++)
            {
                if (!Mazo.HayCartas())
                {
                    Log.Add($"El mazo se agotó. {jugador.Nombre} solo pudo robar {i} cartas.");
                    break;
                }
                jugador.AgregarCarta(Mazo.RobarCarta());
            }
        }

        public void AplicarPendientes()
        {
            if (CartasPendientesRobar <= 0) return;

            Jugador actual = JugadorActual();
            Log.Add($"{actual.Nombre} roba {CartasPendientesRobar} cartas por efecto acumulado.");
            RobarParaJugador(actual, CartasPendientesRobar);

            CartasPendientesRobar = 0;
            TipoPendiente = null;

            PasarTurno();
        }

        public void DecirUno()
        {
            Jugador actual = JugadorActual();
            actual.DijoUno = true;
            Log.Add($"{actual.Nombre} dice: ¡UNO!");
        }

        public void ConfirmarUno()
        {
            if (_jugadorPendienteUno != null)
            {
                _jugadorPendienteUno.DijoUno = true;
                Log.Add($"{_jugadorPendienteUno.Nombre} dice: ¡UNO!");
            }

            _esperandoUno = false;
            _jugadorPendienteUno = null;
        }

        public void PenalizarPorNoDecirUno()
        {
            if (_jugadorPendienteUno != null)
            {
                Log.Add($"{_jugadorPendienteUno.Nombre} no dijo ¡UNO! Roba 2 cartas.");
                RobarParaJugador(_jugadorPendienteUno, 2);
            }

            _esperandoUno = false;
            _jugadorPendienteUno = null;
        }

        public void ContinuarTurno(Carta carta)
        {
            if (carta != null)
            {
                AplicarEfecto(carta);
            }
        }

        public string ColorActivoTexto()
        {
            return ColorActivo.ToString();
        }

        public string LogComoTexto()
        {
            return string.Join(Environment.NewLine, Log);
        }
    }
}