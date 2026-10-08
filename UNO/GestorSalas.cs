using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNO
{
    /// <summary>
    /// Guarda las salas activas en memoria.
    /// En el futuro, con red, esto se llenará desde el servidor.
    /// </summary>
    public static class GestorSalas
    {
        // Lista de salas activas
        private static readonly List<Sala> salasActivas = new List<Sala>();

        // Generador de números aleatorios
        private static readonly Random rnd = new Random();

        /// <summary>
        /// Genera un código de 4 dígitos único (1000-9999).
        /// </summary>
        public static int GenerarCodigoUnico()
        {
            int codigo;
            int intentos = 0;

            do
            {
                codigo = rnd.Next(1000, 10000);   // 1000 a 9999
                intentos++;
                if (intentos > 1000) break;       // seguridad
            }
            while (ExisteCodigo(codigo));

            return codigo;
        }

        /// <summary>
        /// ¿Ya existe una sala con ese código?
        /// </summary>
        public static bool ExisteCodigo(int codigo)
        {
            return salasActivas.Exists(s => s.CodigoSala == codigo);
        }

        /// <summary>
        /// Registra una sala en la lista de activas.
        /// </summary>
        public static void RegistrarSala(Sala sala)
        {
            if (!salasActivas.Contains(sala))
                salasActivas.Add(sala);
        }

        /// <summary>
        /// Quita una sala de la lista (al cerrarse).
        /// </summary>
        public static void QuitarSala(Sala sala)
        {
            salasActivas.Remove(sala);
        }

        /// <summary>
        /// Busca una sala por código. Devuelve null si no existe.
        /// </summary>
        public static Sala BuscarPorCodigo(int codigo)
        {
            return salasActivas.Find(s => s.CodigoSala == codigo);
        }
    }
}