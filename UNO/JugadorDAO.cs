using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace UNO
{
    public static class JugadorDAO
    {
        public static int ObtenerOCrear(string nombre, out int partidasGanadas, out int partidasPerdidas)
        {
            partidasGanadas = 0;
            partidasPerdidas = 0;

            using (MySqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                conexion.Open();

                string queryBuscar = "SELECT id_jugador, partidas_ganadas, partidas_perdidas FROM jugadores WHERE nombre = @nombre";
                using (MySqlCommand cmd = new MySqlCommand(queryBuscar, conexion))
                {
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int id = reader.GetInt32("id_jugador");
                            partidasGanadas = reader.GetInt32("partidas_ganadas");
                            partidasPerdidas = reader.GetInt32("partidas_perdidas");
                            return id;
                        }
                    }
                }

                string queryInsert = "INSERT INTO jugadores (nombre, partidas_ganadas, partidas_perdidas) VALUES (@nombre, 0, 0); SELECT LAST_INSERT_ID();";
                using (MySqlCommand cmd = new MySqlCommand(queryInsert, conexion))
                {
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    int nuevoId = Convert.ToInt32(cmd.ExecuteScalar());
                    return nuevoId;
                }
            }
        }

        public static void IncrementarGanadas(int idJugador)
        {
            using (MySqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                conexion.Open();
                string query = "UPDATE jugadores SET partidas_ganadas = partidas_ganadas + 1 WHERE id_jugador = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", idJugador);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void IncrementarPerdidas(int idJugador)
        {
            using (MySqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                conexion.Open();
                string query = "UPDATE jugadores SET partidas_perdidas = partidas_perdidas + 1 WHERE id_jugador = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", idJugador);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static int ObtenerPartidasJugadas(int idJugador)
        {
            using (MySqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                conexion.Open();
                string query = "SELECT partidas_ganadas + partidas_perdidas FROM jugadores WHERE id_jugador = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", idJugador);
                    object resultado = cmd.ExecuteScalar();
                    if (resultado == null || resultado == DBNull.Value) return 0;
                    return Convert.ToInt32(resultado);
                }
            }
        }

        public static List<string> ListarTodos()
        {
            List<string> lista = new List<string>();

            using (MySqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                conexion.Open();
                string query = "SELECT nombre, partidas_ganadas, partidas_perdidas FROM jugadores ORDER BY nombre";
                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string nombre = reader.GetString("nombre");
                        int ganadas = reader.GetInt32("partidas_ganadas");
                        int perdidas = reader.GetInt32("partidas_perdidas");
                        lista.Add($"{nombre} — {ganadas} ganadas / {perdidas} perdidas");
                    }
                }
            }

            return lista;
        }
    }
}
