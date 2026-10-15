using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace UNO
{
    public class DialogoGanador : Form
    {
        public bool QuiereVolverAJugar { get; private set; }

        public DialogoGanador(string nombreGanador)
        {
            QuiereVolverAJugar = false;

            this.Text = "Fin de la partida";
            this.ClientSize = new Size(450, 220);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(40, 20, 30);

            Label lblTitulo = new Label
            {
                Text = "¡Ganó la partida!",
                Location = new Point(0, 20),
                Size = new Size(450, 40),
                Font = new Font("Arial", 18, FontStyle.Bold),
                ForeColor = Color.Gold,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblTitulo);

            Label lblNombre = new Label
            {
                Text = nombreGanador,
                Location = new Point(0, 65),
                Size = new Size(450, 35),
                Font = new Font("Arial", 22, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblNombre);

            Label lblPregunta = new Label
            {
                Text = "¿Quieres volver a jugar?",
                Location = new Point(0, 110),
                Size = new Size(450, 30),
                Font = new Font("Arial", 12),
                ForeColor = Color.LightYellow,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblPregunta);

            Button btnSi = new Button
            {
                Text = "Sí",
                Location = new Point(80, 155),
                Size = new Size(130, 45),
                Font = new Font("Arial", 12, FontStyle.Bold),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSi.Click += (s, e) =>
            {
                QuiereVolverAJugar = true;
                this.Close();
            };
            this.Controls.Add(btnSi);

            Button btnSalir = new Button
            {
                Text = "Salir",
                Location = new Point(240, 155),
                Size = new Size(130, 45),
                Font = new Font("Arial", 12, FontStyle.Bold),
                BackColor = Color.LightCoral,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSalir.Click += (s, e) =>
            {
                QuiereVolverAJugar = false;
                this.Close();
            };
            this.Controls.Add(btnSalir);
        }
    }
}