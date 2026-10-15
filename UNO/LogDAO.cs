using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace UNO
{
    public static class LogDAO
    {
        public static void Registrar(int idPartida, int idJugador, string accion)
        {
            using (MySqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                conexion.Open();
                string query = "INSERT INTO log_movimientos (id_partida, id_jugador, accion) VALUES (@idPartida, @idJugador, @accion)";
                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@idPartida", idPartida);
                    cmd.Parameters.AddWithValue("@idJugador", idJugador);
                    cmd.Parameters.AddWithValue("@accion", accion);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}