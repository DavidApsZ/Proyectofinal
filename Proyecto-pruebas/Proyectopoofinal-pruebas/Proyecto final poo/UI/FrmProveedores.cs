using MySql.Data.MySqlClient;
using Proyecto_final_poo.Data;
using Proyecto_final_poo.Domain;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Proyecto_final_poo.UI
{
    public class FrmProveedores : Form
    {
        // tabla para mostrar los proveedores
        private readonly DataGridView dgv = new();
        // caja de texto para el nombre del proveedor
        private readonly TextBox txtNombre = new();
        // caja de texto para el telefono del proveedor
        private readonly TextBox txtTelefono = new();
        // boton para agregar proveedor
        private readonly Button btnAgregar = new();
        // boton para eliminar proveedor seleccionado
        private readonly Button btnEliminar = new();
        // boton para ver historial de cambios de stock
        private readonly Button btnVerCambiosStock = new();

        // lista de proveedores para la tabla
        private List<Proveedor> proveedores = new();

        public FrmProveedores()
        {
            // configuracion basica del formulario
            Text = "Proveedores"; Width = 580; Height = 420;
            StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedDialog;

            // configuracion de la tabla
            dgv.Dock = DockStyle.Top; dgv.Height = 240; dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect; dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.MultiSelect = false;

            // etiqueta y caja para nombre
            var lblN = new Label { Text = "Nombre", Left = 20, Top = 255, AutoSize = true };
            txtNombre.Left = 90; txtNombre.Top = 252; txtNombre.Width = 440;

            // etiqueta y caja para telefono
            var lblT = new Label { Text = "Teléfono", Left = 20, Top = 285, AutoSize = true };
            txtTelefono.Left = 90; txtTelefono.Top = 282; txtTelefono.Width = 200;

            // boton para agregar proveedor
            btnAgregar.Text = "Agregar"; btnAgregar.Left = 90; btnAgregar.Top = 320; btnAgregar.Width = 100;
            btnAgregar.Click += (_, __) => Agregar();

            // boton para eliminar proveedor
            btnEliminar.Text = "Eliminar"; btnEliminar.Left = 200; btnEliminar.Top = 320; btnEliminar.Width = 100;
            btnEliminar.Click += (_, __) => Eliminar();

            // boton para ver los ultimos cambios de stock
            btnVerCambiosStock.Text = "Ver cambios de stock";
            btnVerCambiosStock.Left = 310; btnVerCambiosStock.Top = 320; btnVerCambiosStock.Width = 150;
            btnVerCambiosStock.Click += (_, __) => VerCambiosStock();

            Controls.AddRange(new Control[] {
                dgv, lblN, txtNombre, lblT, txtTelefono,
                btnAgregar, btnEliminar, btnVerCambiosStock
            });

            // cuando cambia la seleccion en la tabla, copia los datos
            dgv.SelectionChanged += (_, __) => CargarProveedorSeleccionado();
            // al cargar el formulario, carga los proveedores
            Load += (_, __) => Cargar();
        }

        // carga los proveedores desde la base de datos y los muestra en la tabla
        private void Cargar()
        {
            proveedores.Clear();
            try
            {
                using var c = Db.Con(); c.Open();
                using var cmd = new MySqlCommand("SELECT Id, Nombre, Telefono FROM Proveedores ORDER BY Id DESC", c);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    proveedores.Add(new Proveedor
                    {
                        Id = reader.GetInt32("Id"),
                        Nombre = reader.GetString("Nombre"),
                        Telefono = reader["Telefono"] as string ?? ""
                    });
                }
                dgv.DataSource = null;
                dgv.DataSource = proveedores;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"error al cargar proveedores: {ex.Message}");
            }
        }

        // copia los datos del proveedor seleccionado a las cajas de texto
        private void CargarProveedorSeleccionado()
        {
            if (dgv.CurrentRow?.DataBoundItem is Proveedor p)
            {
                txtNombre.Text = p.Nombre;
                txtTelefono.Text = p.Telefono;
            }
        }

        // agrega un proveedor nuevo
        private void Agregar()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) { MessageBox.Show("nombre requerido"); return; }
            try
            {
                using var c = Db.Con(); c.Open();
                using var cmd = new MySqlCommand("INSERT INTO Proveedores(Nombre, Telefono) VALUES(@n,@t);", c);
                cmd.Parameters.AddWithValue("@n", txtNombre.Text.Trim());
                cmd.Parameters.AddWithValue("@t", txtTelefono.Text.Trim());
                cmd.ExecuteNonQuery();
                txtNombre.Clear(); txtTelefono.Clear(); Cargar();
                MessageBox.Show("proveedor agregado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"error al agregar proveedor: {ex.Message}");
            }
        }

        // elimina el proveedor seleccionado
        private void Eliminar()
        {
            if (dgv.CurrentRow?.DataBoundItem is not Proveedor p) { MessageBox.Show("selecciona un proveedor"); return; }
            var id = p.Id;

            try
            {
                using var c = Db.Con(); c.Open();
                using var cmd = new MySqlCommand("DELETE FROM Proveedores WHERE Id=@id;", c);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                Cargar();
                MessageBox.Show("proveedor eliminado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"error al eliminar proveedor: {ex.Message}");
            }
        }

        // muestra los ultimos cambios/remociones de stock con motivo
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
                string mensaje = "ultimos cambios/remociones de stock:\n\n";
                while (reader.Read())
                {
                    var fecha = Convert.ToDateTime(reader["Fecha"]).ToString("yyyy-MM-dd HH:mm");
                    var producto = reader["Producto"].ToString();
                    var cantidad = reader["Cantidad"].ToString();
                    var motivo = reader["Motivo"].ToString();
                    mensaje += $"{fecha}: {producto}, -{cantidad} unidades. motivo: {motivo}\n";
                }
                MessageBox.Show(mensaje, "historial de motivos al remover stock");
            }
            catch (Exception ex)
            {
                MessageBox.Show("error al consultar el historial de motivos: " + ex.Message);
            }
        }
    }
}