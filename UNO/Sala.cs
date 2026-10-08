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
    public class Sala : Form
    {
        private readonly bool esLider;
        private readonly List<Jugador> jugadores = new List<Jugador>();
        private readonly Dictionary<Jugador, bool> listos = new Dictionary<Jugador, bool>();
        private readonly string carpetaAvatares;
        private readonly string carpetaBotones;

        private const int MAX_JUGADORES = 4;

        public int CodigoSala { get; private set; }

        private Label lblTituloSala;
        private Label lblCodigoSala;
        private Label lblLider;
        private FlowLayoutPanel flpJugadores;
        private Button btnAgregar;
        private Button btnIniciar;
        private Button btnSalir;

        public Sala(bool esLider)
        {
            this.esLider = esLider;
            carpetaAvatares = Path.Combine(Application.StartupPath, "Avatares");
            carpetaBotones = Path.Combine(Application.StartupPath, "Botones");

            if (esLider)
            {
                CodigoSala = GestorSalas.GenerarCodigoUnico();
                GestorSalas.RegistrarSala(this);
            }
            else
            {
                CodigoSala = 0;
            }

            InicializarControles();
            this.Load += Sala_Load;
            this.FormClosed += Sala_FormClosed;
        }

        private void InicializarControles()
        {
            this.Text = "Sala de Espera";
            this.ClientSize = new Size(720, 560);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            lblTituloSala = new Label
            {
                Text = "Sala de Espera",
                Location = new Point(20, 15),
                Size = new Size(680, 35),
                Font = new Font("Arial", 18, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblTituloSala);

            lblCodigoSala = new Label
            {
                Text = "",
                Location = new Point(20, 55),
                Size = new Size(680, 45),
                Font = new Font("Consolas", 24, FontStyle.Bold),
                ForeColor = Color.DarkRed,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblCodigoSala);

            lblLider = new Label
            {
                Text = "Jugadores: 0 / 4",
                Location = new Point(20, 105),
                Size = new Size(680, 25),
                Font = new Font("Arial", 11),
                ForeColor = Color.DimGray,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblLider);

            flpJugadores = new FlowLayoutPanel
            {
                Location = new Point(20, 140),
                Size = new Size(680, 310),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            this.Controls.Add(flpJugadores);

            btnAgregar = CrearBoton("Agregar jugador", 20, 470, 170, 45);
            btnAgregar.Click += btnAgregar_Click;
            this.Controls.Add(btnAgregar);

            btnIniciar = CrearBoton("Iniciar partida", 200, 470, 170, 45);
            btnIniciar.Click += btnIniciar_Click;
            this.Controls.Add(btnIniciar);

            btnSalir = CrearBoton("Salir", 530, 470, 170, 45, backColor: Color.LightCoral);
            btnSalir.Click += btnSalir_Click;
            this.Controls.Add(btnSalir);
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

        private void Sala_Load(object sender, EventArgs e)
        {
            this.Text = esLider ? "Sala (Líder)" : "Sala (Invitado)";
            lblTituloSala.Text = esLider ? "Sala de Espera — Líder" : "Sala de Espera";

            if (esLider)
            {
                lblCodigoSala.Text = $"ID Sala: {CodigoSala}";
            }
            else
            {
                lblCodigoSala.Text = "";
            }

            btnAgregar.Visible = esLider;
            btnIniciar.Visible = esLider;
            ActualizarUI();

            if (esLider)
            {
                AgregarJugadorLocal();
            }
        }

        private void Sala_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (esLider && CodigoSala != 0)
            {
                GestorSalas.QuitarSala(this);
            }
        }
        public bool AgregarInvitado(Jugador j)
        {
            if (jugadores.Count >= MAX_JUGADORES) return false;
            if (jugadores.Exists(x => x.Nombre.Equals(j.Nombre, StringComparison.OrdinalIgnoreCase)))
                return false;

            jugadores.Add(j);
            listos[j] = false;
            ActualizarUI();
            return true;
        }
        public void AbrirParaInvitado()
        {
            // Oculta botones de líder (por seguridad)
            btnAgregar.Visible = false;
            btnIniciar.Visible = false;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            AgregarJugadorLocal();
        }

        private void AgregarJugadorLocal()
        {
            if (jugadores.Count >= MAX_JUGADORES)
            {
                MessageBox.Show("La sala está llena (máx. 4 jugadores).", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var dlg = new SeleccionarAvatar())
            {
                dlg.ShowDialog();

                if (!dlg.Aceptado) return;

                if (jugadores.Exists(j => j.Nombre.Equals(dlg.NombreElegido, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("Ese nombre ya está en la sala.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Jugador nuevo = new Jugador(dlg.NombreElegido, dlg.NumeroAvatar);
                jugadores.Add(nuevo);
                listos[nuevo] = false;
                ActualizarUI();
            }
        }

        private void AlternarListo(Jugador j)
        {
            listos[j] = !listos[j];
            ActualizarUI();

            if (jugadores.Count >= 2 && TodosListos())
            {
                IniciarPartida();
            }
        }

        private bool TodosListos()
        {
            foreach (var j in jugadores)
                if (!listos[j]) return false;
            return true;
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            if (jugadores.Count < 2)
            {
                MessageBox.Show("Se necesitan al menos 2 jugadores.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            IniciarPartida();
        }

        private void IniciarPartida()
        {
            Uno tablero = new Uno(jugadores);
            tablero.Show();
            this.Close();
        }

        private void QuitarJugador(Jugador j)
        {
            if (!esLider) return;

            jugadores.Remove(j);
            listos.Remove(j);
            ActualizarUI();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Menu menu = new Menu();
            menu.Show();
            this.Close();
        }

        private void ActualizarUI()
        {
            flpJugadores.Controls.Clear();

            foreach (Jugador j in jugadores)
            {
                Panel fila = new Panel
                {
                    Width = flpJugadores.Width - 30,
                    Height = 90,
                    BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(5)
                };

                PictureBox pic = new PictureBox
                {
                    Width = 70,
                    Height = 70,
                    Left = 10,
                    Top = 10,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.LightGray
                };

                string rutaAvatar = Path.Combine(carpetaAvatares, $"avatar{j.NumeroAvatar}.jpg");
                if (File.Exists(rutaAvatar))
                {
                    using (var stream = new FileStream(rutaAvatar, FileMode.Open, FileAccess.Read))
                    {
                        pic.Image = Image.FromStream(stream);
                    }
                }
                fila.Controls.Add(pic);

                Label lblNombre = new Label
                {
                    Text = j.Nombre,
                    Left = 90,
                    Top = 15,
                    Width = 200,
                    Font = new Font("Arial", 14, FontStyle.Bold)
                };
                fila.Controls.Add(lblNombre);

                Label lblEstado = new Label
                {
                    Text = listos[j] ? "Listo" : "Esperando",
                    Left = 90,
                    Top = 45,
                    Width = 200,
                    Font = new Font("Arial", 10),
                    ForeColor = listos[j] ? Color.Green : Color.Orange
                };
                fila.Controls.Add(lblEstado);

                Button btnListo = new Button
                {
                    Text = listos[j] ? "No listo" : "Listo",
                    Left = fila.Width - 220,
                    Top = 25,
                    Width = 100,
                    Height = 40,
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnListo.Click += (s, ev) => AlternarListo(j);
                fila.Controls.Add(btnListo);

                if (esLider)
                {
                    Button btnQuitar = new Button
                    {
                        Text = "Quitar",
                        Left = fila.Width - 110,
                        Top = 25,
                        Width = 80,
                        Height = 40,
                        Font = new Font("Arial", 10, FontStyle.Bold),
                        FlatStyle = FlatStyle.Flat,
                        BackColor = Color.LightCoral,
                        Cursor = Cursors.Hand
                    };
                    btnQuitar.Click += (s, ev) => QuitarJugador(j);
                    fila.Controls.Add(btnQuitar);
                }

                flpJugadores.Controls.Add(fila);
            }

            lblLider.Text = $"Jugadores: {jugadores.Count} / {MAX_JUGADORES}";
        }
    }
}