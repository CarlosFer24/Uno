using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace UNO
{
    public class UnirseASala : Form
    {
        private readonly string carpetaBotones;

        private Label lblInstruccion;
        private TextBox txtCodigo;
        private Button btnUnirse;
        private Button btnCancelar;

        public UnirseASala()
        {
            carpetaBotones = Path.Combine(Application.StartupPath, "Botones");
            InicializarControles();
        }

        private void InicializarControles()
        {
            this.Text = "Unirse a Sala";
            this.ClientSize = new Size(420, 260);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.WhiteSmoke;

            lblInstruccion = new Label
            {
                Text = "Ingresa el código de la sala:",
                Location = new Point(20, 25),
                Size = new Size(380, 30),
                Font = new Font("Arial", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblInstruccion);

            txtCodigo = new TextBox
            {
                Location = new Point(110, 75),
                Size = new Size(200, 40),
                Font = new Font("Consolas", 22, FontStyle.Bold),
                TextAlign = HorizontalAlignment.Center,
                MaxLength = 4
            };
            this.Controls.Add(txtCodigo);

            txtCodigo.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };

            btnUnirse = CrearBoton("Unirse", 60, 160, 130, 45);
            btnUnirse.Click += btnUnirse_Click;
            this.Controls.Add(btnUnirse);

            btnCancelar = CrearBoton("Cancelar", 230, 160, 130, 45, backColor: Color.LightCoral);
            btnCancelar.Click += (s, e) => this.Close();
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

        private void btnUnirse_Click(object sender, EventArgs e)
        {
            string texto = txtCodigo.Text.Trim();

            if (texto.Length != 4 || !int.TryParse(texto, out int codigo))
            {
                MessageBox.Show("Ingresa un código válido de 4 dígitos.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Sala sala = GestorSalas.BuscarPorCodigo(codigo);

            if (sala == null)
            {
                MessageBox.Show($"No existe ninguna sala con el código {codigo}.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Pedir nombre+avatar al invitado
            using (var dlg = new SeleccionarAvatar())
            {
                dlg.ShowDialog();
                if (!dlg.Aceptado) return;

                Jugador invitado = new Jugador(dlg.NombreElegido, dlg.NumeroAvatar);

                if (!sala.AgregarInvitado(invitado))
                {
                    MessageBox.Show("No se pudo unir (sala llena o nombre repetido).", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show($"¡Te uniste a la sala {codigo}!", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // luego implementare la ventana de la sala en modo invitado
                this.Close();
            }
        }
    }
}