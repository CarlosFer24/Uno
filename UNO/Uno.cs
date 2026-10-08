using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace UNO
{
    public class Uno : Form
    {
        private List<Jugador> jugadoresIniciales;

        public Uno()
        {
            jugadoresIniciales = null;
            InicializarControles();
        }

        public Uno(List<Jugador> jugadores) : this()
        {
            jugadoresIniciales = jugadores;
        }

        private void InicializarControles()
        {
            this.Text = "UNO - Tablero";
            this.ClientSize = new Size(900, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.DarkGreen;

            //tablero que luego hare
            Label lblPlaceholder = new Label
            {
                Text = "Tablero (en construcción)",
                Location = new Point(0, 0),
                Size = new Size(900, 650),
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            this.Controls.Add(lblPlaceholder);

            this.Load += Uno_Load;
        }

        private void Uno_Load(object sender, EventArgs e)
        {
            if (jugadoresIniciales != null)
            {
                this.Text = $"UNO - Partida con {jugadoresIniciales.Count} jugadores";
            }
        }
    }
}