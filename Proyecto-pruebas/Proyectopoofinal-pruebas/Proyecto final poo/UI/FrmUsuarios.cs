using MySql.Data.MySqlClient;
using Proyecto_final_poo.Data;
using Proyecto_final_poo.Domain;
using System.Data;

namespace Proyecto_final_poo.UI
{
    public class FrmUsuarios : Form
    {
        private readonly DataGridView dgv = new();
        private readonly TextBox txtUser = new();
        private readonly TextBox txtNombre = new();
        private readonly CheckBox chkAdmin = new();
        private readonly TextBox txtPass = new();
        private readonly TextBox txtPass2 = new();

        private readonly Button btnAgregar = new();
        private readonly Button btnResetPass = new();
        private readonly Button btnEliminar = new();

        public FrmUsuarios()
        {
            if (!(Sesion.UsuarioActual?.IsAdmin ?? false))
            {
                MessageBox.Show("Solo un administrador puede acceder aquí.");
                Close();
                return;
            }

            Text = "Usuarios"; Width = 720; Height = 520;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;

            SuspendLayout();

            dgv.Dock = DockStyle.Top; dgv.Height = 280; dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false; dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            var y = 300;
            var lblU = new Label { Text = "Usuario", Left = 20, Top = y + 3, AutoSize = true };
            txtUser.Left = 80; txtUser.Top = y; txtUser.Width = 160;

            var lblN = new Label { Text = "Nombre", Left = 250, Top = y + 3, AutoSize = true };
            txtNombre.Left = 310; txtNombre.Top = y; txtNombre.Width = 180;

            chkAdmin.Text = "Admin"; chkAdmin.Left = 500; chkAdmin.Top = y; chkAdmin.AutoSize = true;

            var lblP = new Label { Text = "Contraseña", Left = 20, Top = y + 36 + 3, AutoSize = true };
            txtPass.Left = 90; txtPass.Top = y + 36; txtPass.Width = 150; txtPass.PasswordChar = '●';

            var lblP2 = new Label { Text = "Confirmar", Left = 250, Top = y + 36 + 3, AutoSize = true };
            txtPass2.Left = 310; txtPass2.Top = y + 36; txtPass2.Width = 150; txtPass2.PasswordChar = '●';

            btnAgregar.Text = "Agregar";
            btnAgregar.Left = 500; btnAgregar.Top = y + 32; btnAgregar.Width = 90;
            btnAgregar.Click += (_, __) => Agregar();

            btnResetPass.Text = "Cambiar contraseña";
            btnResetPass.Left = 20; btnResetPass.Top = y + 80; btnResetPass.Width = 180;
            btnResetPass.Click += (_, __) => ResetPassword();

            btnEliminar.Text = "Eliminar";
            btnEliminar.Left = 210; btnEliminar.Top = y + 80; btnEliminar.Width = 100;
            btnEliminar.Click += (_, __) => Eliminar();

            Controls.AddRange(new Control[] {
                dgv, lblU, txtUser, lblN, txtNombre, chkAdmin, lblP, txtPass, lblP2, txtPass2,
                btnAgregar, btnResetPass, btnEliminar
            });

            Load += (_, __) => Cargar();
            ResumeLayout(false);
        }

        private void Cargar()
        {
            using var c = Db.Con(); c.Open();
            using var da = new MySqlDataAdapter(
                "SELECT Id, Username, Nombre, IsAdmin FROM Usuarios ORDER BY Id DESC", c);
            var t = new DataTable(); da.Fill(t);
            dgv.DataSource = t;
        }

        private void Agregar()
        {
            var u = txtUser.Text.Trim();
            var n = txtNombre.Text.Trim();
            var p1 = txtPass.Text;
            var p2 = txtPass2.Text;

            if (string.IsNullOrWhiteSpace(u)) { MessageBox.Show("Usuario requerido"); return; }
            if (string.IsNullOrWhiteSpace(p1) || string.IsNullOrWhiteSpace(p2)) { MessageBox.Show("Contraseña requerida"); return; }
            if (p1 != p2) { MessageBox.Show("Las contraseñas no coinciden"); return; }

            try
            {
                using var c = Db.Con(); c.Open();
                using var cmd = new MySqlCommand(
                    @"INSERT INTO Usuarios(Username, PasswordHash, Nombre, IsAdmin)
                      VALUES(@u, SHA2(@p,256), @n, @a);", c);
                cmd.Parameters.AddWithValue("@u", u);
                cmd.Parameters.AddWithValue("@p", p1);
                cmd.Parameters.AddWithValue("@n", string.IsNullOrWhiteSpace(n) ? (object)DBNull.Value : n);
                cmd.Parameters.AddWithValue("@a", chkAdmin.Checked ? 1 : 0);
                cmd.ExecuteNonQuery();

                txtUser.Clear(); txtNombre.Clear(); txtPass.Clear(); txtPass2.Clear(); chkAdmin.Checked = false;
                Cargar();
            }
            catch (MySqlException ex) when (ex.Number == 1062) // duplicate key
            {
                MessageBox.Show("Ese nombre de usuario ya existe.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar: " + ex.Message);
            }
        }

        private void ResetPassword()
        {
            if (dgv.CurrentRow == null) { MessageBox.Show("Selecciona un usuario"); return; }

            var id = Convert.ToInt32(((DataRowView)dgv.CurrentRow.DataBoundItem)["Id"]);
            var p1 = txtPass.Text;
            var p2 = txtPass2.Text;

            if (string.IsNullOrWhiteSpace(p1) || string.IsNullOrWhiteSpace(p2)) { MessageBox.Show("Escribe la nueva contraseña (dos veces)"); return; }
            if (p1 != p2) { MessageBox.Show("Las contraseñas no coinciden"); return; }

            try
            {
                using var c = Db.Con(); c.Open();
                using var cmd = new MySqlCommand(
                    "UPDATE Usuarios SET PasswordHash = SHA2(@p,256) WHERE Id=@id;", c);
                cmd.Parameters.AddWithValue("@p", p1);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();

                txtPass.Clear(); txtPass2.Clear();
                MessageBox.Show("Contraseña actualizada.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cambiar contraseña: " + ex.Message);
            }
        }

        private void Eliminar()
        {
            if (dgv.CurrentRow == null) { MessageBox.Show("Selecciona un usuario"); return; }

            var id = Convert.ToInt32(((DataRowView)dgv.CurrentRow.DataBoundItem)["Id"]);
            var username = ((DataRowView)dgv.CurrentRow.DataBoundItem)["Username"].ToString();

            if (Sesion.UsuarioActual != null && Sesion.UsuarioActual.Id == id)
            {
                MessageBox.Show("No puedes eliminar tu propio usuario.");
                return;
            }

            if (MessageBox.Show($"¿Eliminar usuario '{username}'?", "Confirmación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                using var c = Db.Con(); c.Open();
                using var cmd = new MySqlCommand("DELETE FROM Usuarios WHERE Id=@id;", c);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                Cargar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message);
            }
        }
    }
}
