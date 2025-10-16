using MySql.Data.MySqlClient;
using Proyecto_final_poo.Data;
using System.Data;

namespace Proyecto_final_poo.UI
{
    public class FrmEmpleados : Form
    {
        private readonly DataGridView dgv = new();
        private readonly TextBox txtNombre = new();
        private readonly TextBox txtRol = new();
        private readonly Button btnAgregar = new();
        private readonly Button btnEliminar = new();

        public FrmEmpleados()
        {
            Text = "Empleados"; Width = 580; Height = 420;
            StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedDialog;

            dgv.Dock = DockStyle.Top; dgv.Height = 240; dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect; dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            var lblN = new Label { Text = "Nombre", Left = 20, Top = 255, AutoSize = true };
            txtNombre.Left = 90; txtNombre.Top = 252; txtNombre.Width = 440;

            var lblR = new Label { Text = "Rol", Left = 20, Top = 285, AutoSize = true };
            txtRol.Left = 90; txtRol.Top = 282; txtRol.Width = 200;

            btnAgregar.Text = "Agregar"; btnAgregar.Left = 90; btnAgregar.Top = 320; btnAgregar.Width = 100;
            btnAgregar.Click += (_, __) => Agregar();

            btnEliminar.Text = "Eliminar"; btnEliminar.Left = 200; btnEliminar.Top = 320; btnEliminar.Width = 100;
            btnEliminar.Click += (_, __) => Eliminar();

            Controls.AddRange(new Control[] { dgv, lblN, txtNombre, lblR, txtRol, btnAgregar, btnEliminar });

            Load += (_, __) => Cargar();
        }

        private void Cargar()
        {
            try
            {
                using var c = Db.Con(); c.Open();
                using var da = new MySqlDataAdapter("SELECT Id, Nombre, Rol FROM Empleados ORDER BY Id DESC", c);
                var t = new DataTable(); da.Fill(t);
                dgv.DataSource = t;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar empleados: {ex.Message}");
            }
        }

        private void Agregar()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) { MessageBox.Show("Nombre requerido"); return; }
            try
            {
                using var c = Db.Con(); c.Open();
                using var cmd = new MySqlCommand("INSERT INTO Empleados(Nombre, Rol) VALUES(@n,@r);", c);
                cmd.Parameters.AddWithValue("@n", txtNombre.Text.Trim());
                cmd.Parameters.AddWithValue("@r", txtRol.Text.Trim());
                cmd.ExecuteNonQuery();
                txtNombre.Clear(); txtRol.Clear(); Cargar();
                MessageBox.Show("Empleado agregado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar empleado: {ex.Message}");
            }
        }

        private void Eliminar()
        {
            if (dgv.CurrentRow == null) { MessageBox.Show("Selecciona un empleado"); return; }
            var id = Convert.ToInt32(((DataRowView)dgv.CurrentRow.DataBoundItem)["Id"]);
            
            try
            {
                using var c = Db.Con(); c.Open();
                using var cmd = new MySqlCommand("DELETE FROM Empleados WHERE Id=@id;", c);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                Cargar();
                MessageBox.Show("Empleado eliminado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar empleado: {ex.Message}");
            }
        }
    }
}
