using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace UNO
{
    public class Uno : Form
    {
        private Juego juego;
        private List<Jugador> jugadoresIniciales;

        private readonly string carpetaCartas;
        private readonly string carpetaAvatares;
        private readonly string carpetaBotones;
        private readonly string carpetaFondos;

        private PictureBox picMazo;
        private PictureBox picMesa;

        private Panel[] panelesJugadores;
        private PictureBox[] avataresJugadores;
        private Label[] nombresJugadores;
        private Label[] contadorCartas;
        private FlowLayoutPanel[] flpCartasRival;

        private FlowLayoutPanel flpMano;

        private Button btnRobar;
        private Button btnUno;
        private Button btnSalir;

        private Label lblTurno;
        private Label lblColorActivo;
        private Label lblAvisoUno;

        private System.Windows.Forms.Timer timerUno;
        private Carta ultimaCartaJugada;

        private const int ANCHO_CARTA_GRANDE = 100;
        private const int ALTO_CARTA_GRANDE = 150;
        private const int ANCHO_CARTA_MANO = 90;
        private const int ALTO_CARTA_MANO = 135;
        private const int SEGUNDOS_PARA_UNO = 5;

        public Uno()
        {
            jugadoresIniciales = null;

            carpetaCartas = Path.Combine(Application.StartupPath, "Cartas");
            carpetaAvatares = Path.Combine(Application.StartupPath, "Avatares");
            carpetaBotones = Path.Combine(Application.StartupPath, "Botones");
            carpetaFondos = Path.Combine(Application.StartupPath, "Fondos");

            InicializarControles();
            this.Load += Uno_Load;
        }

        public Uno(List<Jugador> jugadores) : this()
        {
            jugadoresIniciales = jugadores;
        }

        private void InicializarControles()
        {
            this.Text = "UNO - Tablero";
            this.ClientSize = new Size(1280, 760);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 60, 30);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            CargarFondo("fondoTablero.jpg");

            picMazo = new PictureBox
            {
                Location = new Point(540, 300),
                Size = new Size(ANCHO_CARTA_GRANDE, ALTO_CARTA_GRANDE),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            CargarImagenDorso(picMazo);
            picMazo.Click += picMazo_Click;
            this.Controls.Add(picMazo);

            picMesa = new PictureBox
            {
                Location = new Point(660, 300),
                Size = new Size(ANCHO_CARTA_GRANDE, ALTO_CARTA_GRANDE),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            this.Controls.Add(picMesa);

            lblTurno = new Label
            {
                Location = new Point(0, 200),
                Size = new Size(1280, 40),
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Turno: ---"
            };
            this.Controls.Add(lblTurno);

            lblColorActivo = new Label
            {
                Location = new Point(0, 240),
                Size = new Size(1280, 30),
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.LightYellow,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Color activo: ---"
            };
            this.Controls.Add(lblColorActivo);

            lblAvisoUno = new Label
            {
                Location = new Point(0, 130),
                Size = new Size(1280, 60),
                Font = new Font("Arial", 26, FontStyle.Bold),
                ForeColor = Color.Yellow,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "",
                Visible = false
            };
            this.Controls.Add(lblAvisoUno);

            panelesJugadores = new Panel[4];
            avataresJugadores = new PictureBox[4];
            nombresJugadores = new Label[4];
            contadorCartas = new Label[4];
            flpCartasRival = new FlowLayoutPanel[4];

            Point[] posiciones = new Point[]
            {
                new Point(20, 580),
                new Point(20, 20),
                new Point(1020, 20),
                new Point(1020, 580)
            };

            for (int i = 0; i < 4; i++)
            {
                panelesJugadores[i] = CrearPanelJugador(posiciones[i], i);
                this.Controls.Add(panelesJugadores[i]);
            }

            flpMano = new FlowLayoutPanel
            {
                Location = new Point(280, 580),
                Size = new Size(700, 160),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.FromArgb(180, 20, 50, 20)
            };
            this.Controls.Add(flpMano);

            btnRobar = CrearBoton("Comer", 20, 490, 200, 70);
            btnRobar.Click += btnRobar_Click;
            this.Controls.Add(btnRobar);

            btnUno = CrearBoton("¡UNO!", 20, 380, 200, 70, backColor: Color.Gold);
            btnUno.Click += btnUno_Click;
            this.Controls.Add(btnUno);

            btnSalir = CrearBoton("Salir", 20, 670, 200, 45, backColor: Color.LightCoral);
            btnSalir.Click += btnSalir_Click;
            this.Controls.Add(btnSalir);

            timerUno = new System.Windows.Forms.Timer();
            timerUno.Interval = SEGUNDOS_PARA_UNO * 1000;
            timerUno.Tick += TimerUno_Tick;
        }

        private Panel CrearPanelJugador(Point posicion, int indice)
        {
            Panel panel = new Panel
            {
                Location = posicion,
                Size = new Size(240, 150),
                BackColor = Color.FromArgb(200, 100, 20, 30),
                BorderStyle = BorderStyle.FixedSingle
            };

            PictureBox pic = new PictureBox
            {
                Location = new Point(10, 15),
                Size = new Size(90, 90),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.LightGray,
                BorderStyle = BorderStyle.FixedSingle
            };
            panel.Controls.Add(pic);

            Label lblNombre = new Label
            {
                Text = "Jugador",
                Location = new Point(110, 15),
                Size = new Size(120, 25),
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };
            panel.Controls.Add(lblNombre);

            Label lblCartas = new Label
            {
                Text = "0 cartas",
                Location = new Point(110, 45),
                Size = new Size(120, 25),
                Font = new Font("Arial", 9),
                ForeColor = Color.LightYellow,
                BackColor = Color.Transparent
            };
            panel.Controls.Add(lblCartas);

            FlowLayoutPanel flp = new FlowLayoutPanel
            {
                Location = new Point(110, 75),
                Size = new Size(120, 40),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent
            };
            panel.Controls.Add(flp);

            avataresJugadores[indice] = pic;
            nombresJugadores[indice] = lblNombre;
            contadorCartas[indice] = lblCartas;
            flpCartasRival[indice] = flp;

            return panel;
        }

        private Button CrearBoton(string texto, int left, int top, int width, int height,
                                  string archivoImagen = null, Color? backColor = null)
        {
            Button btn = new Button
            {
                Text = texto,
                Location = new Point(left, top),
                Size = new Size(width, height),
                Font = new Font("Arial", 12, FontStyle.Bold),
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

        private void Uno_Load(object sender, EventArgs e)
        {
            if (jugadoresIniciales == null || jugadoresIniciales.Count < 2)
            {
                MessageBox.Show("Se necesitan al menos 2 jugadores.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            List<string> nombres = new List<string>();
            foreach (Jugador j in jugadoresIniciales) nombres.Add(j.Nombre);

            juego = new Juego(nombres);

            for (int i = 0; i < juego.Jugadores.Count; i++)
            {
                juego.Jugadores[i].NumeroAvatar = jugadoresIniciales[i].NumeroAvatar;
            }

            this.Text = $"UNO - Partida con {juego.Jugadores.Count} jugadores";

            RefrescarTodo();
        }

        private void RefrescarTodo()
        {
            RefrescarTablero();
            RefrescarMano();
            RefrescarIndicadores();

            if (juego.JuegoTerminado())
            {
                MostrarGanador();
            }
        }

        private void RefrescarTablero()
        {
            CargarImagenCarta(picMesa, juego.CartaMesa());

            for (int i = 0; i < panelesJugadores.Length; i++)
            {
                if (i < juego.Jugadores.Count)
                {
                    Jugador j = juego.Jugadores[i];

                    nombresJugadores[i].Text = j.Nombre;
                    contadorCartas[i].Text = $"{j.CantidadCartas()} cartas";

                    string rutaAvatar = Path.Combine(carpetaAvatares, $"avatar{j.NumeroAvatar}.jpg");
                    if (File.Exists(rutaAvatar))
                    {
                        using (var stream = new FileStream(rutaAvatar, FileMode.Open, FileAccess.Read))
                        {
                            avataresJugadores[i].Image = Image.FromStream(stream);
                        }
                    }

                    flpCartasRival[i].Controls.Clear();

                    if (i != juego.TurnoActual)
                    {
                        int cantidad = Math.Min(j.CantidadCartas(), 4);
                        for (int c = 0; c < cantidad; c++)
                        {
                            PictureBox miniCarta = new PictureBox
                            {
                                Size = new Size(20, 30),
                                SizeMode = PictureBoxSizeMode.Zoom,
                                Margin = new Padding(1)
                            };
                            CargarImagenDorso(miniCarta);
                            flpCartasRival[i].Controls.Add(miniCarta);
                        }
                    }

                    if (i == juego.TurnoActual)
                        panelesJugadores[i].BackColor = Color.FromArgb(230, 180, 40, 60);
                    else
                        panelesJugadores[i].BackColor = Color.FromArgb(200, 100, 20, 30);

                    panelesJugadores[i].Visible = true;
                }
                else
                {
                    panelesJugadores[i].Visible = false;
                }
            }
        }

        private void RefrescarMano()
        {
            flpMano.Controls.Clear();

            Jugador actual = juego.JugadorActual();

            foreach (Carta carta in actual.Mano)
            {
                PictureBox pic = new PictureBox
                {
                    Size = new Size(ANCHO_CARTA_MANO, ALTO_CARTA_MANO),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(4),
                    Cursor = Cursors.Hand,
                    Tag = carta
                };

                CargarImagenCarta(pic, carta);
                pic.Click += picCarta_Click;

                flpMano.Controls.Add(pic);
            }
        }

        private void RefrescarIndicadores()
        {
            lblTurno.Text = $"Turno: {juego.JugadorActual().Nombre}";
            lblColorActivo.Text = $"Color activo: {juego.ColorActivo}";

            if (juego.CartasPendientesRobar > 0)
            {
                lblTurno.Text += $"  DEBE COMER {juego.CartasPendientesRobar} CARTAS (o acumular +2/+4)";
                lblTurno.ForeColor = Color.OrangeRed;
            }
            else
            {
                lblTurno.ForeColor = Color.White;
            }

            btnRobar.Enabled = !juego.EsperandoUno;
            btnSalir.Enabled = !juego.EsperandoUno;
        }

        private void CargarImagenCarta(PictureBox pic, Carta carta)
        {
            if (carta == null) return;

            string ruta = Path.Combine(carpetaCartas, carta.NombreArchivoImagen());

            if (File.Exists(ruta))
            {
                using (var stream = new FileStream(ruta, FileMode.Open, FileAccess.Read))
                {
                    pic.Image = Image.FromStream(stream);
                }
            }
            else
            {
                pic.Image = GenerarPlaceholder(carta, pic.Width, pic.Height);
            }
        }

        private void CargarImagenDorso(PictureBox pic)
        {
            string ruta = Path.Combine(carpetaCartas, "parteatras.jpg");

            if (File.Exists(ruta))
            {
                using (var stream = new FileStream(ruta, FileMode.Open, FileAccess.Read))
                {
                    pic.Image = Image.FromStream(stream);
                }
            }
            else
            {
                Bitmap bmp = new Bitmap(pic.Width, pic.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.DarkBlue);
                    g.DrawRectangle(Pens.White, 3, 3, pic.Width - 7, pic.Height - 7);
                    g.DrawString("UNO", new Font("Arial", 10, FontStyle.Bold),
                        Brushes.White, new PointF(5, 40));
                }
                pic.Image = bmp;
            }
        }

        private Image GenerarPlaceholder(Carta carta, int ancho, int alto)
        {
            Bitmap bmp = new Bitmap(ancho, alto);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                Color fondo = ColorDeCarta(carta.Color);
                g.Clear(fondo);
                g.DrawRectangle(Pens.White, 2, 2, ancho - 5, alto - 5);

                string texto = TextoDeCarta(carta);
                Font fuente = new Font("Arial", 20, FontStyle.Bold);
                SizeF tam = g.MeasureString(texto, fuente);

                g.DrawString(texto, fuente, Brushes.White,
                    (ancho - tam.Width) / 2,
                    (alto - tam.Height) / 2);
            }
            return bmp;
        }

        private Color ColorDeCarta(ColorCarta c)
        {
            switch (c)
            {
                case ColorCarta.Rojo: return Color.FromArgb(200, 30, 30);
                case ColorCarta.Azul: return Color.FromArgb(30, 80, 200);
                case ColorCarta.Verde: return Color.FromArgb(30, 160, 60);
                case ColorCarta.Amarillo: return Color.FromArgb(220, 190, 30);
                default: return Color.FromArgb(60, 60, 60);
            }
        }

        private string TextoDeCarta(Carta carta)
        {
            switch (carta.Tipo)
            {
                case TipoCarta.Numero: return carta.Numero.ToString();
                case TipoCarta.Salta: return "SALTA";
                case TipoCarta.Reversa: return "REV";
                case TipoCarta.RobaDos: return "+2";
                case TipoCarta.Comodin: return "CAMB";
                case TipoCarta.ComodinRobaCuatro: return "+4";
                default: return "?";
            }
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

        private void picMazo_Click(object sender, EventArgs e)
        {
            btnRobar_Click(sender, e);
        }

        private void picCarta_Click(object sender, EventArgs e)
        {
            if (juego.JuegoTerminado()) return;
            if (juego.EsperandoUno) return;

            PictureBox pic = (PictureBox)sender;
            Carta carta = (Carta)pic.Tag;

            ColorCarta colorElegido = ColorCarta.Comodin;

            if (carta.Tipo == TipoCarta.Comodin || carta.Tipo == TipoCarta.ComodinRobaCuatro)
            {
                colorElegido = PedirColor();

                if (colorElegido == ColorCarta.Comodin)
                {
                    MessageBox.Show("Debes elegir un color para jugar el comodín.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            bool jugada = juego.JugarCarta(carta, colorElegido);

            if (!jugada)
            {
                MessageBox.Show("Esa carta no se puede jugar en este momento.", "Jugada inválida",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ultimaCartaJugada = carta;

            if (juego.EsperandoUno)
            {
                timerUno.Start();
                lblAvisoUno.Text = $"{juego.JugadorPendienteUno.Nombre} tiene 1 carta. ¡Presiona UNO!";
                lblAvisoUno.Visible = true;
                RefrescarIndicadores();
            }
            else
            {
                RefrescarTodo();
            }
        }

        private void TimerUno_Tick(object sender, EventArgs e)
        {
            timerUno.Stop();
            lblAvisoUno.Visible = false;

            if (juego.EsperandoUno)
            {
                juego.PenalizarPorNoDecirUno();
            }

            juego.ContinuarTurno(ultimaCartaJugada);
            RefrescarTodo();
        }

        private ColorCarta PedirColor()
        {
            using (Form dlg = new Form())
            {
                dlg.Text = "Elige un color";
                dlg.ClientSize = new Size(400, 180);
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;

                ColorCarta elegido = ColorCarta.Comodin;

                ColorCarta[] colores = { ColorCarta.Rojo, ColorCarta.Azul, ColorCarta.Verde, ColorCarta.Amarillo };

                for (int i = 0; i < 4; i++)
                {
                    ColorCarta color = colores[i];
                    Button boton = new Button
                    {
                        Text = color.ToString(),
                        Location = new Point(20 + i * 90, 50),
                        Size = new Size(80, 60),
                        BackColor = ColorDeCarta(color),
                        ForeColor = Color.White,
                        Font = new Font("Arial", 10, FontStyle.Bold),
                        FlatStyle = FlatStyle.Flat
                    };
                    boton.Click += (s, ev) =>
                    {
                        elegido = color;
                        dlg.Close();
                    };
                    dlg.Controls.Add(boton);
                }

                dlg.ShowDialog();
                return elegido;
            }
        }

        private void btnRobar_Click(object sender, EventArgs e)
        {
            if (juego.JuegoTerminado()) return;
            if (juego.EsperandoUno) return;

            if (juego.CartasPendientesRobar > 0)
            {
                int cantidad = juego.CartasPendientesRobar;
                juego.AplicarPendientes();

                MessageBox.Show($"Comiste {cantidad} cartas por efecto de +2/+4.",
                    "Efecto acumulado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                RefrescarTodo();
                return;
            }

            Carta robada = juego.RobarCartaActual();

            if (robada == null)
            {
                MessageBox.Show("El mazo está agotado. Se pasa turno.",
                    "Mazo vacío", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            RefrescarTodo();
        }

        private void btnUno_Click(object sender, EventArgs e)
        {
            if (juego.JuegoTerminado()) return;

            if (juego.EsperandoUno)
            {
                timerUno.Stop();
                juego.ConfirmarUno();
                lblAvisoUno.Visible = false;

                juego.ContinuarTurno(ultimaCartaJugada);
                RefrescarTodo();
                return;
            }

            if (juego.JugadorActual().TieneUnaCarta())
            {
                juego.DecirUno();
                MessageBox.Show($"{juego.JugadorActual().Nombre} dijo ¡UNO!",
                    "¡UNO!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            VolverAlMenu();
        }

        private void MostrarGanador()
        {
            btnRobar.Enabled = false;
            btnUno.Enabled = false;
            flpMano.Controls.Clear();

            lblTurno.Text = $"Ganador: {juego.Ganador.Nombre}";

            using (DialogoGanador dlg = new DialogoGanador(juego.Ganador.Nombre))
            {
                dlg.ShowDialog();

                if (dlg.QuiereVolverAJugar)
                {
                    VolverAJugar();
                }
                else
                {
                    VolverAlMenu();
                }
            }
        }

        private void VolverAJugar()
        {
            List<string> nombres = new List<string>();
            foreach (Jugador j in juego.Jugadores) nombres.Add(j.Nombre);

            juego = new Juego(nombres);

            for (int i = 0; i < juego.Jugadores.Count; i++)
            {
                juego.Jugadores[i].NumeroAvatar = jugadoresIniciales[i].NumeroAvatar;
            }

            btnRobar.Enabled = true;
            btnUno.Enabled = true;

            RefrescarTodo();
        }

        private void VolverAlMenu()
        {
            Menu menu = new Menu();
            menu.Show();
            this.Close();
        }
    }
}