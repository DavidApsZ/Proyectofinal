using MySql.Data.MySqlClient;
using Proyecto_final_poo.Data;
using Proyecto_final_poo.Domain;

namespace Proyecto_final_poo.UI
{
    public class FrmConfiguracion : Form
    {

        private readonly NumericUpDown numComision = new();
        private readonly Button btnGuardar = new();

        public FrmConfiguracion()
        {
            if (!(Sesion.UsuarioActual?.IsAdmin ?? false))
            {
                MessageBox.Show("Solo un administrador puede acceder aquí.");
                Close();
                return;
            }

            Text = "Configuración"; Width = 360; Height = 180;
            StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedDialog;

            var lbl = new Label { Text = "Comisión tarjeta (%)", Left = 20, Top = 30, AutoSize = true };
            numComision.Left = 160; numComision.Top = 26; numComision.Width = 80;
            numComision.DecimalPlaces = 2; numComision.Maximum = 100; numComision.Minimum = 0;

            btnGuardar.Text = "Guardar"; btnGuardar.Left = 160; btnGuardar.Top = 70; btnGuardar.Width = 100;
            btnGuardar.Click += (_, __) => Guardar();

            Controls.AddRange(new Control[] { lbl, numComision, btnGuardar });

            Load += (_, __) => Cargar();
        }

        private void Cargar()
        {
            using var c = Db.Con(); c.Open();
            using var cmd = new MySqlCommand("SELECT ValorDecimal FROM Configuracion WHERE Clave='ComisionTarjeta';", c);
            var val = cmd.ExecuteScalar();
            var frac = (val == null || val == DBNull.Value) ? 0.035m : Convert.ToDecimal(val);
            numComision.Value = frac * 100m; 
        }

        private void Guardar()
        {
            using var c = Db.Con(); c.Open();
            using var cmd = new MySqlCommand(
                @"INSERT INTO Configuracion(Clave, ValorDecimal)
                    VALUES('ComisionTarjeta', @v)
                  ON DUPLICATE KEY UPDATE ValorDecimal=@v;", c);
            cmd.Parameters.AddWithValue("@v", numComision.Value / 100m);
            cmd.ExecuteNonQuery();
            MessageBox.Show("Guardado.");
        }
    }
}
