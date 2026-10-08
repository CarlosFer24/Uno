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
    public class SeleccionarAvatar : Form
    {

        public string NombreElegido { get; private set; }
        public int NumeroAvatar { get; private set; }   // 1..5
        public bool Aceptado { get; private set; }


        private readonly string carpetaAvatares;
        private readonly string carpetaBotones;


        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblAvatar;
        private PictureBox[] pics;
        private Button btnAceptar;
        private Button btnCancelar;

        private const int NUM_AVATARES = 5;

        public SeleccionarAvatar()
        {
            carpetaAvatares = Path.Combine(Application.StartupPath, "Avatares");
            carpetaBotones = Path.Combine(Application.StartupPath, "Botones");

            NumeroAvatar = 0;
            Aceptado = false;

            InicializarControles();
            this.Load += SeleccionarAvatar_Load;
        }

        private void InicializarControles()
        {
            this.Text = "Nuevo jugador";
            this.ClientSize = new Size(540, 320);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.WhiteSmoke;

            // ── Etiqueta "Nombre" ──
            lblNombre = new Label
            {
                Text = "Nombre:",
                Location = new Point(20, 25),
                Size = new Size(80, 25),
                Font = new Font("Arial", 11, FontStyle.Bold)
            };
            this.Controls.Add(lblNombre);

            // ── Caja de texto ──
            txtNombre = new TextBox
            {
                Location = new Point(110, 25),
                Size = new Size(400, 25),
                Font = new Font("Arial", 11)
            };
            this.Controls.Add(txtNombre);

            // ── Etiqueta "Elige tu avatar" ──
            lblAvatar = new Label
            {
                Text = "Elige tu avatar:",
                Location = new Point(20, 70),
                Size = new Size(200, 25),
                Font = new Font("Arial", 11, FontStyle.Bold)
            };
            this.Controls.Add(lblAvatar);

            // ── 5 PictureBox para avatares ──
            pics = new PictureBox[NUM_AVATARES];

            for (int i = 0; i < NUM_AVATARES; i++)
            {
                pics[i] = new PictureBox
                {
                    Location = new Point(20 + i * 100, 105),
                    Size = new Size(85, 85),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.LightGray,
                    Cursor = Cursors.Hand,
                    Tag = i + 1
                };

                int numero = i + 1;
                pics[i].Click += (s, ev) => MarcarSeleccionado(numero);

                this.Controls.Add(pics[i]);
            }

            // ── Botón Aceptar ──
            btnAceptar = CrearBoton("Aceptar", 260, 240, 130, 45);
            btnAceptar.Click += btnAceptar_Click;
            this.Controls.Add(btnAceptar);

            // ── Botón Cancelar ──
            btnCancelar = CrearBoton("Cancelar", 400, 240, 130, 45, backColor: Color.LightCoral);
            btnCancelar.Click += btnCancelar_Click;
            this.Controls.Add(btnCancelar);
        }


        private Button CrearBoton(string texto, int left, int top, int width, int height,
                                  string archivoImagen = null, Color? backColor = null)
        {
            Button btn = new Button
            {
                Text = texto,
                Location = new Point(left, top),
                Size = new Size(width, height),
                Font = new Font("Arial", 11, FontStyle.Bold),
                BackColor = backColor ?? Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

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

        private void SeleccionarAvatar_Load(object sender, EventArgs e)
        {
            CargarAvatares();
            MarcarSeleccionado(1);   // por defecto el avatar 1
        }

        private void CargarAvatares()
        {
            for (int i = 0; i < NUM_AVATARES; i++)
            {
                string archivo = Path.Combine(carpetaAvatares, $"avatar{i + 1}.jpg");

                if (File.Exists(archivo))
                {
                    using (var stream = new FileStream(archivo, FileMode.Open, FileAccess.Read))
                    {
                        pics[i].Image = Image.FromStream(stream);
                    }
                }
            }
        }

        private void MarcarSeleccionado(int numero)
        {
            NumeroAvatar = numero;

            for (int i = 0; i < NUM_AVATARES; i++)
            {
                pics[i].BorderStyle = (i + 1 == numero)
                    ? BorderStyle.Fixed3D
                    : BorderStyle.FixedSingle;
            }
        }


        private void btnAceptar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();

            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Escribe un nombre.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }

            if (NumeroAvatar == 0)
            {
                MessageBox.Show("Selecciona un avatar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            NombreElegido = nombre;
            Aceptado = true;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Aceptado = false;
            this.Close();
        }
    }
}