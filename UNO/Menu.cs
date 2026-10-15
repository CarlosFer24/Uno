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
        private readonly string carpetaFondos;

        private PictureBox picPortada;
        private Button btnCrearSala;
        private Button btnUnirseSala;
        private Button btnSalir;

        public Menu()
        {
            carpetaBotones = Path.Combine(Application.StartupPath, "Botones");
            carpetaFondos = Path.Combine(Application.StartupPath, "Fondos");
            InicializarControles();
        }

        private void InicializarControles()
        {
            this.Text = "UNO - Menú principal";
            this.ClientSize = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.DarkRed;

            CargarFondo("fondoMenu.jpg");

            picPortada = new PictureBox
            {
                Location = new Point(-40, 60),
                Size = new Size(925, 185),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };

            string rutaPortada = Path.Combine(carpetaFondos, "portada.png");
            if (File.Exists(rutaPortada))
            {
                using (var stream = new FileStream(rutaPortada, FileMode.Open, FileAccess.Read))
                {
                    picPortada.Image = Image.FromStream(stream);
                }
            }
            this.Controls.Add(picPortada);

            btnCrearSala = CrearBoton("Crear Sala", 250, "crearsala1.png");
            btnCrearSala.Click += (s, e) =>
            {
                Sala sala = new Sala(esLider: true);
                sala.Show();
                this.Hide();
            };
            this.Controls.Add(btnCrearSala);

            btnUnirseSala = CrearBoton("Unirse a Sala", 340, "unirsesala1.png");
            btnUnirseSala.Click += (s, e) =>
            {
                UnirseASala unirse = new UnirseASala();
                unirse.ShowDialog();
            };
            this.Controls.Add(btnUnirseSala);

            btnSalir = CrearBoton("Salir", 430, "salir.png");
            btnSalir.Click += (s, e) => Application.Exit();
            this.Controls.Add(btnSalir);
        }

        private void CargarFondo(string nombreArchivo)
        {
            string ruta = Path.Combine(carpetaFondos, nombreArchivo);

            if (File.Exists(ruta))
            {
                using (var stream = new FileStream(ruta, FileMode.Open, FileAccess.Read))
                {
                    this.BackgroundImage = Image.FromStream(stream);
                    this.BackgroundImageLayout = ImageLayout.Stretch;
                }
            }
        }

        private Button CrearBoton(string texto, int top, string archivoImagen = null)
        {
            Button btn = new Button
            {
                Text = texto,
                Location = new Point(290, top),
                Size = new Size(220, 60),
                Font = new Font("Arial", 14, FontStyle.Bold),
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btn.FlatAppearance.BorderSize = 0;

            if (!string.IsNullOrEmpty(archivoImagen))
            {
                string ruta = Path.Combine(carpetaBotones, archivoImagen);
                if (File.Exists(ruta))
                {
                    using (var stream = new FileStream(ruta, FileMode.Open, FileAccess.Read))
                    {
                        btn.BackgroundImage = Image.FromStream(stream);
                    }
                    btn.BackgroundImageLayout = ImageLayout.Stretch;
                    btn.Text = "";
                }
            }

            return btn;
        }
    }
}