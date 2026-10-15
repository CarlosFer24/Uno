using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace UNO
{
    public static class PartidaDAO
    {
        public static int CrearPartida(List<int> idsJugadores)
        {
            using (MySqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                conexion.Open();

                string queryPartida = "INSERT INTO partidas (fecha_inicio) VALUES (NOW()); SELECT LAST_INSERT_ID();";
                int idPartida;
                using (MySqlCommand cmd = new MySqlCommand(queryPartida, conexion))
                {
                    idPartida = Convert.ToInt32(cmd.ExecuteScalar());
                }

                foreach (int idJugador in idsJugadores)
                {
                    string queryPJ = "INSERT INTO partida_jugador (id_partida, id_jugador) VALUES (@idPartida, @idJugador)";
                    using (MySqlCommand cmd = new MySqlCommand(queryPJ, conexion))
                    {
                        cmd.Parameters.AddWithValue("@idPartida", idPartida);
                        cmd.Parameters.AddWithValue("@idJugador", idJugador);
                        cmd.ExecuteNonQuery();
                    }
                }

                return idPartida;
            }
        }

        public static void FinalizarPartida(int idPartida, int idGanador)
        {
            using (MySqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                conexion.Open();
                string query = "UPDATE partidas SET fecha_fin = NOW(), id_ganador = @idGanador WHERE id_partida = @idPartida";
                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@idGanador", idGanador);
                    cmd.Parameters.AddWithValue("@idPartida", idPartida);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
