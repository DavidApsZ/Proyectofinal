using MySql.Data.MySqlClient;
using Proyecto_final_poo.Data;
using Proyecto_final_poo.Domain;

namespace Proyecto_final_poo.UI
{
    public class FrmConfiguracion : Form
    {
        // control para ingresar el porcentaje de comision
        private readonly NumericUpDown numComision = new();
        // boton para guardar la configuracion
        private readonly Button btnGuardar = new();

        public FrmConfiguracion()
        {
            // valida que solo un usuario admin pueda acceder
            if (!(Sesion.UsuarioActual?.IsAdmin ?? false))
            {
                MessageBox.Show("solo un administrador puede acceder aqui.");
                Close();
                return;
            }

            // configuracion basica del formulario
            Text = "Configuración"; Width = 360; Height = 180;
            StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedDialog;

            // etiqueta para el campo de comision
            var lbl = new Label { Text = "Comisión tarjeta (%)", Left = 20, Top = 30, AutoSize = true };
            // numericupdown para la comision, porcentaje
            numComision.Left = 160; numComision.Top = 26; numComision.Width = 80;
            numComision.DecimalPlaces = 2; numComision.Maximum = 100; numComision.Minimum = 0;

            // boton guardar
            btnGuardar.Text = "Guardar"; btnGuardar.Left = 160; btnGuardar.Top = 70; btnGuardar.Width = 100;
            btnGuardar.Click += (_, __) => Guardar();

            // agrega los controles al formulario
            Controls.AddRange(new Control[] { lbl, numComision, btnGuardar });

            // al cargar el formulario, carga la configuracion actual
            Load += (_, __) => Cargar();
        }

        // metodo para cargar el valor actual de la comision desde la base de datos
        private void Cargar()
        {
            using var c = Db.Con(); c.Open();
            using var cmd = new MySqlCommand("SELECT ValorDecimal FROM Configuracion WHERE Clave='ComisionTarjeta';", c);
            var val = cmd.ExecuteScalar();
            var frac = (val == null || val == DBNull.Value) ? 0.035m : Convert.ToDecimal(val);
            numComision.Value = frac * 100m;
        }

        // metodo para guardar el valor de la comision en la base de datos
        private void Guardar()
        {
            using var c = Db.Con(); c.Open();
            using var cmd = new MySqlCommand(
                @"INSERT INTO Configuracion(Clave, ValorDecimal)
                    VALUES('ComisionTarjeta', @v)
                  ON DUPLICATE KEY UPDATE ValorDecimal=@v;", c);
            cmd.Parameters.AddWithValue("@v", numComision.Value / 100m);
            cmd.ExecuteNonQuery();
            MessageBox.Show("guardado.");
        }
    }
}