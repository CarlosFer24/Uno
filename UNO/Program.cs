using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UNO
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            if (!ConexionDB.ProbarConexion())
            {
                System.Windows.Forms.MessageBox.Show(
                    "No se pudo conectar a la base de datos.\n",
                    "Error de BD",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Menu());
        }
    }
}