using MySql.Data.MySqlClient;
using Proyecto_final_poo.Data;
using System.Data;
using System.Windows.Forms;

namespace Proyecto_final_poo.UI
{
    public class FrmProveedores : Form
    {
        private readonly DataGridView dgv = new();
        private readonly TextBox txtNombre = new();
        private readonly TextBox txtTelefono = new();
        private readonly Button btnAgregar = new();
        private readonly Button btnEliminar = new();
        private readonly Button btnVerCambiosStock = new();

        public FrmProveedores()
        {
            Text = "Proveedores"; Width = 580; Height = 420;
            StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedDialog;

            dgv.Dock = DockStyle.Top; dgv.Height = 240; dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect; dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            var lblN = new Label { Text = "Nombre", Left = 20, Top = 255, AutoSize = true };
            txtNombre.Left = 90; txtNombre.Top = 252; txtNombre.Width = 440;

            var lblT = new Label { Text = "Teléfono", Left = 20, Top = 285, AutoSize = true };
            txtTelefono.Left = 90; txtTelefono.Top = 282; txtTelefono.Width = 200;

            btnAgregar.Text = "Agregar"; btnAgregar.Left = 90; btnAgregar.Top = 320; btnAgregar.Width = 100;
            btnAgregar.Click += (_, __) => Agregar();

            btnEliminar.Text = "Eliminar"; btnEliminar.Left = 200; btnEliminar.Top = 320; btnEliminar.Width = 100;
            btnEliminar.Click += (_, __) => Eliminar();

            btnVerCambiosStock.Text = "Ver cambios de stock";
            btnVerCambiosStock.Left = 310; btnVerCambiosStock.Top = 320; btnVerCambiosStock.Width = 150;
            btnVerCambiosStock.Click += (_, __) => VerCambiosStock();

            Controls.AddRange(new Control[] {
                dgv, lblN, txtNombre, lblT, txtTelefono,
                btnAgregar, btnEliminar, btnVerCambiosStock
            });

            Load += (_, __) => Cargar();
        }

        private void Cargar()
        {
            try
            {
                using var c = Db.Con(); c.Open();
                using var da = new MySqlDataAdapter("SELECT Id, Nombre, Telefono FROM Proveedores ORDER BY Id DESC", c);
                var t = new DataTable(); da.Fill(t);
                dgv.DataSource = t;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar proveedores: {ex.Message}");
            }
        }

        private void Agregar()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) { MessageBox.Show("Nombre requerido"); return; }
            try
            {
                using var c = Db.Con(); c.Open();
                using var cmd = new MySqlCommand("INSERT INTO Proveedores(Nombre, Telefono) VALUES(@n,@t);", c);
                cmd.Parameters.AddWithValue("@n", txtNombre.Text.Trim());
                cmd.Parameters.AddWithValue("@t", txtTelefono.Text.Trim());
                cmd.ExecuteNonQuery();
                txtNombre.Clear(); txtTelefono.Clear(); Cargar();
                MessageBox.Show("Proveedor agregado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar proveedor: {ex.Message}");
            }
        }

        private void Eliminar()
        {
            if (dgv.CurrentRow == null) { MessageBox.Show("Selecciona un proveedor"); return; }
            var id = Convert.ToInt32(((DataRowView)dgv.CurrentRow.DataBoundItem)["Id"]);

            try
            {
                using var c = Db.Con(); c.Open();
                using var cmd = new MySqlCommand("DELETE FROM Proveedores WHERE Id=@id;", c);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                Cargar();
                MessageBox.Show("Proveedor eliminado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar proveedor: {ex.Message}");
            }
        }

        private void VerCambiosStock()
        {
            try
            {
                using var c = Db.Con(); c.Open();
                using var cmd = new MySqlCommand(
                    @"SELECT m.Fecha, p.Nombre AS Producto, m.Cantidad, m.Motivo
                      FROM MovimientosInventario m
                      JOIN Productos p ON m.ProductoId = p.Id
                      WHERE m.Tipo = 'salida' AND m.Motivo IS NOT NULL AND m.Motivo <> ''
                      ORDER BY m.Fecha DESC
                      LIMIT 10;", c);

                using var reader = cmd.ExecuteReader();
                string mensaje = "Últimos cambios/remociones de stock:\n\n";
                while (reader.Read())
                {
                    var fecha = Convert.ToDateTime(reader["Fecha"]).ToString("yyyy-MM-dd HH:mm");
                    var producto = reader["Producto"].ToString();
                    var cantidad = reader["Cantidad"].ToString();
                    var motivo = reader["Motivo"].ToString();
                    mensaje += $"{fecha}: {producto}, -{cantidad} unidades. Motivo: {motivo}\n";
                }
                MessageBox.Show(mensaje, "Historial de motivos al remover stock");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar el historial de motivos: " + ex.Message);
            }
        }
    }
}