using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNO
{
    public class Carta
    {
        public const string Extension = ".jpg";

        public ColorCarta Color { get; set; }
        public TipoCarta Tipo { get; set; }
        public int Numero { get; set; }

        //constructor para cartas numeradas (0-9)
        public Carta(ColorCarta color, int numero)
        {
            Color = color;
            Tipo = TipoCarta.Numero;
            Numero = numero;
        }

        //constructor para cartas especiales (Salta, Reversa, +2, Comodín, +4)
        public Carta(ColorCarta color, TipoCarta tipo)
        {
            Color = color;
            Tipo = tipo;
            Numero = -1;   // No aplica
        }

        //Método para determinar si una carta se puede jugar
        public bool SePuedeJugarSobre(Carta cartaMesa)
        {
            if (Tipo == TipoCarta.Comodin || Tipo == TipoCarta.ComodinRobaCuatro)
                return true;

            if (Color == cartaMesa.Color)
                return true;

            if (Tipo == cartaMesa.Tipo && Tipo != TipoCarta.Numero)
                return true;

            if (Tipo == TipoCarta.Numero &&
                cartaMesa.Tipo == TipoCarta.Numero &&
                Numero == cartaMesa.Numero)
                return true;

            return false;
        }
        public string NombreArchivoImagen()
        {
            switch (Tipo)
            {
                case TipoCarta.Numero:
                    return $"ceizy{Numero}{Color}{Extension}";

                case TipoCarta.Salta:
                    return $"ceizysalta{Color}{Extension}";

                case TipoCarta.Reversa:
                    return $"ceizyreverse{Color}{Extension}";

                case TipoCarta.RobaDos:
                    return $"ceizy+2{Color}{Extension}";

                case TipoCarta.Comodin:
                    return $"ceizyCambiaColor{Extension}";

                case TipoCarta.ComodinRobaCuatro:
                    return $"ceizy+4{Extension}";

                default:
                    return $"ceizyReverso{Extension}";
            }
        }
        public static string NombreArchivoReverso()
        {
            return "parteatras.jpg";
        }

        public override string ToString()
        {
            switch (Tipo)
            {
                case TipoCarta.Numero: return $"{Color} {Numero}";
                case TipoCarta.Salta: return $"{Color} Salta";
                case TipoCarta.Reversa: return $"{Color} Reversa";
                case TipoCarta.RobaDos: return $"{Color} +2";
                case TipoCarta.Comodin: return "Comodín";
                case TipoCarta.ComodinRobaCuatro: return "Comodín +4";
                default: return "Carta desconocida";
            }
        }
    }
}