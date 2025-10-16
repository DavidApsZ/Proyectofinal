using MySql.Data.MySqlClient;
using Proyecto_final_poo.Data;
using Proyecto_final_poo.Domain;

namespace Proyecto_final_poo.UI
{
    public class FrmCambiarMiPassword : Form
    {
        private readonly TextBox txtActual = new();
        private readonly TextBox txtNueva = new();
        private readonly TextBox txtNueva2 = new();
        private readonly Button btnGuardar = new();

        public FrmCambiarMiPassword()
        {
            Text = "Cambiar mi contraseña"; Width = 360; Height = 220;
            StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedDialog;

            var lbl1 = new Label { Text = "Actual", Left = 20, Top = 25, AutoSize = true };
            txtActual.Left = 100; txtActual.Top = 22; txtActual.Width = 200; txtActual.PasswordChar = '●';

            var lbl2 = new Label { Text = "Nueva", Left = 20, Top = 65, AutoSize = true };
            txtNueva.Left = 100; txtNueva.Top = 62; txtNueva.Width = 200; txtNueva.PasswordChar = '●';

            var lbl3 = new Label { Text = "Confirmar", Left = 20, Top = 105, AutoSize = true };
            txtNueva2.Left = 100; txtNueva2.Top = 102; txtNueva2.Width = 200; txtNueva2.PasswordChar = '●';

            btnGuardar.Text = "Guardar"; btnGuardar.Left = 100; btnGuardar.Top = 140; btnGuardar.Width = 100;
            btnGuardar.Click += (_, __) => Guardar();

            Controls.AddRange(new Control[] { lbl1, txtActual, lbl2, txtNueva, lbl3, txtNueva2, btnGuardar });
        }

        private void Guardar()
        {
            if (!Sesion.EstaLogueado) { MessageBox.Show("No hay sesión activa."); return; }
            if (string.IsNullOrWhiteSpace(txtNueva.Text) || txtNueva.Text != txtNueva2.Text)
            { MessageBox.Show("Verifica la nueva contraseña."); return; }

            using var c = Db.Con(); c.Open();

            using (var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM Usuarios WHERE Id=@id AND PasswordHash=SHA2(@p,256);", c))
            {
                cmd.Parameters.AddWithValue("@id", Sesion.UsuarioActual!.Id);
                cmd.Parameters.AddWithValue("@p", txtActual.Text);
                var ok = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                if (!ok) { MessageBox.Show("Contraseña actual incorrecta."); return; }
            }

            using (var cmd2 = new MySqlCommand(
                "UPDATE Usuarios SET PasswordHash=SHA2(@n,256) WHERE Id=@id;", c))
            {
                cmd2.Parameters.AddWithValue("@n", txtNueva.Text);
                cmd2.Parameters.AddWithValue("@id", Sesion.UsuarioActual!.Id);
                cmd2.ExecuteNonQuery();
            }

            MessageBox.Show("Contraseña actualizada.");
            Close();
        }
    }
}
