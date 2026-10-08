using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace UNO
{
    public class Menu : Form
    {
        private readonly string carpetaBotones;

        private Label lblTitulo;
        private Button btnCrearSala;
        private Button btnUnirseSala;
        private Button btnSalir;

        public Menu()
        {
            carpetaBotones = Path.Combine(Application.StartupPath, "Botones");
            InicializarControles();
        }

        private void InicializarControles()
        {
            this.Text = "UNO - Menú principal";
            this.ClientSize = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.DarkRed;

            lblTitulo = new Label
            {
                Text = "UNO",
                Location = new Point(0, 80),
                Size = new Size(800, 120),
                Font = new Font("Arial Black", 72, FontStyle.Bold),
                ForeColor = Color.Yellow,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblTitulo);

            btnCrearSala = CrearBoton("Crear Sala", 250);
            btnCrearSala.Click += (s, e) =>
            {
                Sala sala = new Sala(esLider: true);
                sala.Show();
                this.Hide();
            };
            this.Controls.Add(btnCrearSala);

            btnUnirseSala = CrearBoton("Unirse a Sala", 330);
            btnUnirseSala.Click += (s, e) =>
            {
                UnirseASala unirse = new UnirseASala();
                unirse.ShowDialog();

            };
            this.Controls.Add(btnUnirseSala);

            btnSalir = CrearBoton("Salir", 410);
            btnSalir.Click += (s, e) => Application.Exit();
            this.Controls.Add(btnSalir);
        }

        private Button CrearBoton(string texto, int top, string archivoImagen = null)
        {
            Button btn = new Button
            {
                Text = texto,
                Location = new Point(290, top),
                Size = new Size(220, 55),
                Font = new Font("Arial", 14, FontStyle.Bold),
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            // pa cargar mi fondo despues
            if (!string.IsNullOrEmpty(archivoImagen))
            {
                string ruta = Path.Combine(carpetaBotones, archivoImagen);
                if (File.Exists(ruta))
                {
                    btn.BackgroundImage = Image.FromFile(ruta);
                    btn.BackgroundImageLayout = ImageLayout.Stretch;
                    btn.Text = "";
                }
            }

            return btn;
        }
    }
}