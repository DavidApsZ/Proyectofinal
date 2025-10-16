using MySql.Data.MySqlClient;
using Proyecto_final_poo.Data;
using Proyecto_final_poo.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Proyecto_final_poo.UI
{
    public class FrmUsuarios : Form
    {
        // tabla para mostrar los usuarios
        private readonly DataGridView dgv = new();
        // caja para el nombre de usuario
        private readonly TextBox txtUser = new();
        // caja para el nombre real del usuario
        private readonly TextBox txtNombre = new();
        // checkbox para indicar si es admin
        private readonly CheckBox chkAdmin = new();
        // caja para la contrasena
        private readonly TextBox txtPass = new();
        // caja para confirmar la contrasena
        private readonly TextBox txtPass2 = new();

        // boton para agregar usuario
        private readonly Button btnAgregar = new();
        // boton para cambiar la contrasena de un usuario
        private readonly Button btnResetPass = new();
        // boton para eliminar usuario seleccionado
        private readonly Button btnEliminar = new();

        // lista de usuarios para la tabla
        private List<Usuario> listaUsuarios = new();

        public FrmUsuarios()
        {
            // solo permite acceso a admin
            if (!(Sesion.UsuarioActual?.IsAdmin ?? false))
            {
                MessageBox.Show("solo un administrador puede acceder aqui.");
                Close();
                return;
            }

            // configuracion basica del formulario
            Text = "Usuarios"; Width = 720; Height = 520;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;

            SuspendLayout();

            // tabla de usuarios
            dgv.Dock = DockStyle.Top; dgv.Height = 280; dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false; dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            var y = 300;
            // etiqueta y caja para usuario
            var lblU = new Label { Text = "Usuario", Left = 20, Top = y + 3, AutoSize = true };
            txtUser.Left = 80; txtUser.Top = y; txtUser.Width = 160;

            // etiqueta y caja para nombre
            var lblN = new Label { Text = "Nombre", Left = 250, Top = y + 3, AutoSize = true };
            txtNombre.Left = 310; txtNombre.Top = y; txtNombre.Width = 180;

            // checkbox admin
            chkAdmin.Text = "Admin"; chkAdmin.Left = 500; chkAdmin.Top = y; chkAdmin.AutoSize = true;

            // etiqueta y caja para contrasena
            var lblP = new Label { Text = "Contraseña", Left = 20, Top = y + 36 + 3, AutoSize = true };
            txtPass.Left = 90; txtPass.Top = y + 36; txtPass.Width = 150; txtPass.PasswordChar = '●';

            // etiqueta y caja para confirmar contrasena
            var lblP2 = new Label { Text = "Confirmar", Left = 250, Top = y + 36 + 3, AutoSize = true };
            txtPass2.Left = 310; txtPass2.Top = y + 36; txtPass2.Width = 150; txtPass2.PasswordChar = '●';

            // boton para agregar usuario
            btnAgregar.Text = "Agregar";
            btnAgregar.Left = 500; btnAgregar.Top = y + 32; btnAgregar.Width = 90;
            btnAgregar.Click += (_, __) => Agregar();

            // boton para resetear contrasena
            btnResetPass.Text = "Cambiar contraseña";
            btnResetPass.Left = 20; btnResetPass.Top = y + 80; btnResetPass.Width = 180;
            btnResetPass.Click += (_, __) => ResetPassword();

            // boton para eliminar usuario
            btnEliminar.Text = "Eliminar";
            btnEliminar.Left = 210; btnEliminar.Top = y + 80; btnEliminar.Width = 100;
            btnEliminar.Click += (_, __) => Eliminar();

            Controls.AddRange(new Control[] {
                dgv, lblU, txtUser, lblN, txtNombre, chkAdmin, lblP, txtPass, lblP2, txtPass2,
                btnAgregar, btnResetPass, btnEliminar
            });

            // cuando cambia la seleccion en la tabla, copia los datos
            dgv.SelectionChanged += (_, __) => UsuarioSeleccionado();
            // al cargar el formulario, carga los usuarios
            Load += (_, __) => Cargar();
            ResumeLayout(false);
        }

        // carga los usuarios desde la base de datos y los muestra en la tabla
        private void Cargar()
        {
            listaUsuarios = new List<Usuario>();
            using var c = Db.Con(); c.Open();
            using var cmd = new MySqlCommand("SELECT Id, Username, Nombre, IsAdmin FROM Usuarios ORDER BY Id DESC", c);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                listaUsuarios.Add(new Usuario
                {
                    Id = rd.GetInt32("Id"),
                    Username = rd.GetString("Username"),
                    Nombre = rd.IsDBNull("Nombre") ? "" : rd.GetString("Nombre"),
                    IsAdmin = rd.GetBoolean("IsAdmin")
                });
            }
            dgv.DataSource = null;
            dgv.DataSource = listaUsuarios;
            if (dgv.Columns.Contains("Id")) dgv.Columns["Id"].HeaderText = "ID";
            if (dgv.Columns.Contains("Username")) dgv.Columns["Username"].HeaderText = "Usuario";
            if (dgv.Columns.Contains("Nombre")) dgv.Columns["Nombre"].HeaderText = "Nombre";
            if (dgv.Columns.Contains("IsAdmin")) dgv.Columns["IsAdmin"].HeaderText = "Administrador";
        }

        // copia los datos del usuario seleccionado a las cajas de texto
        private void UsuarioSeleccionado()
        {
            if (dgv.CurrentRow?.DataBoundItem is Usuario u)
            {
                txtUser.Text = u.Username;
                txtNombre.Text = u.Nombre;
                chkAdmin.Checked = u.IsAdmin;
            }
        }

        // agrega un usuario nuevo
        private void Agregar()
        {
            var usuario = txtUser.Text.Trim();
            var nombre = txtNombre.Text.Trim();
            var pass = txtPass.Text;
            var pass2 = txtPass2.Text;
            var isAdmin = chkAdmin.Checked;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("faltan datos requeridos.");
                return;
            }
            if (pass != pass2)
            {
                MessageBox.Show("las contrasenas no coinciden.");
                return;
            }
            if (string.IsNullOrWhiteSpace(pass))
            {
                MessageBox.Show("contrasena requerida.");
                return;
            }

            using var c = Db.Con(); c.Open();
            using var cmd = new MySqlCommand(
                @"INSERT INTO Usuarios(Username, PasswordHash, Nombre, IsAdmin)
                  VALUES(@u, SHA2(@p,256), @n, @a);", c);
            cmd.Parameters.AddWithValue("@u", usuario);
            cmd.Parameters.AddWithValue("@p", pass);
            cmd.Parameters.AddWithValue("@n", nombre);
            cmd.Parameters.AddWithValue("@a", isAdmin);
            try
            {
                cmd.ExecuteNonQuery();
                txtUser.Clear(); txtNombre.Clear(); txtPass.Clear(); txtPass2.Clear(); chkAdmin.Checked = false;
                Cargar();
                MessageBox.Show("usuario agregado correctamente.");
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("error al agregar usuario: " + ex.Message);
            }
        }

        // cambia la contrasena de un usuario seleccionado
        private void ResetPassword()
        {
            if (dgv.CurrentRow?.DataBoundItem is not Usuario u)
            {
                MessageBox.Show("selecciona un usuario.");
                return;
            }
            var pass = Microsoft.VisualBasic.Interaction.InputBox("Nueva contraseña para " + u.Username + ":", "Cambiar contraseña");
            if (string.IsNullOrWhiteSpace(pass))
            {
                MessageBox.Show("contrasena no valida.");
                return;
            }
            using var c = Db.Con(); c.Open();
            using var cmd = new MySqlCommand("UPDATE Usuarios SET PasswordHash = SHA2(@p,256) WHERE Id = @id;", c);
            cmd.Parameters.AddWithValue("@p", pass);
            cmd.Parameters.AddWithValue("@id", u.Id);
            cmd.ExecuteNonQuery();
            MessageBox.Show("contrasena cambiada.");
        }

        // elimina el usuario seleccionado
        private void Eliminar()
        {
            if (dgv.CurrentRow?.DataBoundItem is not Usuario u)
            {
                MessageBox.Show("selecciona un usuario.");
                return;
            }
            if (MessageBox.Show("¿seguro que deseas eliminar este usuario?", "confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
            using var c = Db.Con(); c.Open();
            using var cmd = new MySqlCommand("DELETE FROM Usuarios WHERE Id = @id;", c);
            cmd.Parameters.AddWithValue("@id", u.Id);
            cmd.ExecuteNonQuery();
            Cargar();
            MessageBox.Show("usuario eliminado correctamente.");
        }
    }
}