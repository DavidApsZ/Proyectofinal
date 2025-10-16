using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Proyecto_final_poo.Data;
using Proyecto_final_poo.Domain;

namespace Proyecto_final_poo.UI
{
    public class FrmClientes : Form
    {
        private readonly DataGridView dgv = new();
        private readonly TextBox txtNombre = new();
        private readonly TextBox txtTelefono = new();
        private readonly TextBox txtEmail = new();
        private readonly Button btnAgregar = new();
        private readonly Button btnEditar = new();
        private readonly Button btnEliminar = new();
        private readonly Button btnHistorial = new();

        public FrmClientes()
        {
            Text = "Clientes";
            Width = 600; Height = 400;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;

            InicializarControles();
            Load += (_, __) => CargarClientes();
        }

        private void InicializarControles()
        {
            dgv.Dock = DockStyle.Top;
            dgv.Height = 160;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.MultiSelect = false;
            Controls.Add(dgv);

            var lblNombre = new Label { Text = "Nombre", Left = 10, Top = dgv.Bottom + 10, Width = 60 };
            txtNombre.Left = 80; txtNombre.Top = dgv.Bottom + 8; txtNombre.Width = 140;

            var lblTelefono = new Label { Text = "Teléfono", Left = 230, Top = dgv.Bottom + 10, Width = 60 };
            txtTelefono.Left = 300; txtTelefono.Top = dgv.Bottom + 8; txtTelefono.Width = 120;

            var lblEmail = new Label { Text = "Email", Left = 430, Top = dgv.Bottom + 10, Width = 40 };
            txtEmail.Left = 480; txtEmail.Top = dgv.Bottom + 8; txtEmail.Width = 100;

            Controls.Add(lblNombre);
            Controls.Add(txtNombre);
            Controls.Add(lblTelefono);
            Controls.Add(txtTelefono);
            Controls.Add(lblEmail);
            Controls.Add(txtEmail);

            btnAgregar.Text = "Agregar";
            btnAgregar.Left = 80; btnAgregar.Top = txtNombre.Bottom + 10; btnAgregar.Width = 80;
            btnAgregar.Click += BtnAgregar_Click;
            Controls.Add(btnAgregar);

            btnEditar.Text = "Editar";
            btnEditar.Left = 200; btnEditar.Top = txtNombre.Bottom + 10; btnEditar.Width = 80;
            btnEditar.Click += BtnEditar_Click;
            Controls.Add(btnEditar);

            btnEliminar.Text = "Eliminar";
            btnEliminar.Left = 320; btnEliminar.Top = txtNombre.Bottom + 10; btnEliminar.Width = 80;
            btnEliminar.Click += BtnEliminar_Click;
            Controls.Add(btnEliminar);

            btnHistorial.Text = "Ver historial de compras";
            btnHistorial.Dock = DockStyle.Bottom;
            btnHistorial.Height = 35;
            btnHistorial.Click += BtnHistorial_Click;
            Controls.Add(btnHistorial);

            dgv.SelectionChanged += (_, __) => CopiarDatosFilaSeleccionada();
        }

        private void CargarClientes()
        {
            using var c = Db.Con();
            c.Open();
            using var da = new MySqlDataAdapter("SELECT Id, Nombre, Telefono, Email FROM Clientes ORDER BY Id;", c);
            var t = new DataTable();
            da.Fill(t);
            dgv.DataSource = t;
        }

        private void CopiarDatosFilaSeleccionada()
        {
            if (dgv.SelectedRows.Count == 0) return;
            var row = dgv.SelectedRows[0];
            txtNombre.Text = row.Cells["Nombre"].Value?.ToString() ?? "";
            txtTelefono.Text = row.Cells["Telefono"].Value?.ToString() ?? "";
            txtEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";
        }

        private void BtnAgregar_Click(object? sender, EventArgs e)
        {
            var nombre = txtNombre.Text.Trim();
            var telefono = txtTelefono.Text.Trim();
            var email = txtEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("El nombre es obligatorio.");
                return;
            }
            if (!string.IsNullOrWhiteSpace(email) && !new Cliente { Email = email }.EmailValido())
            {
                MessageBox.Show("El email es inválido.");
                return;
            }
            try
            {
                using var c = Db.Con();
                c.Open();
                using var cmd = new MySqlCommand("INSERT INTO Clientes (Nombre, Telefono, Email) VALUES (@n, @t, @e);", c);
                cmd.Parameters.AddWithValue("@n", nombre);
                cmd.Parameters.AddWithValue("@t", string.IsNullOrWhiteSpace(telefono) ? DBNull.Value : telefono);
                cmd.Parameters.AddWithValue("@e", string.IsNullOrWhiteSpace(email) ? DBNull.Value : email);
                cmd.ExecuteNonQuery();
                CargarClientes();
                MessageBox.Show("Cliente agregado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar cliente: {ex.Message}");
            }
        }

        private void BtnEditar_Click(object? sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona un cliente para editar.");
                return;
            }
            var id = Convert.ToInt32(dgv.SelectedRows[0].Cells["Id"].Value);
            var nombre = txtNombre.Text.Trim();
            var telefono = txtTelefono.Text.Trim();
            var email = txtEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("El nombre es obligatorio.");
                return;
            }
            if (!string.IsNullOrWhiteSpace(email) && !new Cliente { Email = email }.EmailValido())
            {
                MessageBox.Show("El email es inválido.");
                return;
            }
            try
            {
                using var c = Db.Con();
                c.Open();
                using var cmd = new MySqlCommand("UPDATE Clientes SET Nombre=@n, Telefono=@t, Email=@e WHERE Id=@id;", c);
                cmd.Parameters.AddWithValue("@n", nombre);
                cmd.Parameters.AddWithValue("@t", string.IsNullOrWhiteSpace(telefono) ? DBNull.Value : telefono);
                cmd.Parameters.AddWithValue("@e", string.IsNullOrWhiteSpace(email) ? DBNull.Value : email);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                CargarClientes();
                MessageBox.Show("Cliente editado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al editar cliente: {ex.Message}");
            }
        }

        private void BtnEliminar_Click(object? sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona un cliente para eliminar.");
                return;
            }
            var id = Convert.ToInt32(dgv.SelectedRows[0].Cells["Id"].Value);
            var resultado = MessageBox.Show("¿Estás seguro de eliminar el cliente seleccionado?", "Confirmar", MessageBoxButtons.YesNo);
            if (resultado != DialogResult.Yes)
                return;
            try
            {
                using var c = Db.Con();
                c.Open();
                using var cmd = new MySqlCommand("DELETE FROM Clientes WHERE Id=@id;", c);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                CargarClientes();
                MessageBox.Show("Cliente eliminado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar cliente: {ex.Message}");
            }
        }

        private void BtnHistorial_Click(object? sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona un cliente para ver el historial.");
                return;
            }
            var row = dgv.SelectedRows[0];
            var cliente = new Cliente
            {
                Id = Convert.ToInt32(row.Cells["Id"].Value),
                Nombre = row.Cells["Nombre"].Value?.ToString() ?? "",
                Telefono = row.Cells["Telefono"].Value?.ToString() ?? "",
                Email = row.Cells["Email"].Value?.ToString() ?? ""
            };

            var frm = new FrmHistorialCompras(cliente);
            frm.ShowDialog();
        }
    }
}