using MySql.Data.MySqlClient;
using Proyecto_final_poo.Data;
using System.Data;

namespace Proyecto_final_poo.UI
{
    public class FrmProductos : Form
    {
        private readonly DataGridView dgv = new();
        private readonly TextBox txtNombre = new();
        private readonly NumericUpDown numPrecio = new();
        private readonly NumericUpDown numStock = new();
        private readonly ComboBox cboCategoria = new();
        private readonly Button btnAgregar = new();
        private readonly Button btnEditar = new();
        private readonly Button btnEliminar = new();
        private readonly TextBox txtBusquedaProductos = new();
        private readonly Label lblBusqueda = new();
        private readonly Button btnAñadirStock = new();
        private readonly Button btnRemoverStock = new();

        public FrmProductos()
        {
            Text = "Productos";
            Width = 560; Height = 540;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            SuspendLayout();

            lblBusqueda.Text = "Buscar:";
            lblBusqueda.Left = 20;
            lblBusqueda.Top = 18;
            lblBusqueda.AutoSize = true;
            Controls.Add(lblBusqueda);

            txtBusquedaProductos.Left = lblBusqueda.Right + 8;
            txtBusquedaProductos.Top = 14;
            txtBusquedaProductos.Width = 320;
            txtBusquedaProductos.PlaceholderText = "Nombre del producto...";
            Controls.Add(txtBusquedaProductos);

            txtBusquedaProductos.TextChanged += (_, __) => FiltrarProductos();

            dgv.Left = 20;
            dgv.Top = txtBusquedaProductos.Bottom + 8;
            dgv.Width = 520;
            dgv.Height = 160;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Controls.Add(dgv);

            var lblNombre = new Label { Text = "Nombre", Left = 20, Top = dgv.Bottom + 15, AutoSize = true };
            txtNombre.Left = 90; txtNombre.Top = lblNombre.Top - 3; txtNombre.Width = 320;

            var lblPrecio = new Label { Text = "Precio", Left = 20, Top = lblNombre.Bottom + 12, AutoSize = true };
            numPrecio.Left = 90; numPrecio.Top = lblPrecio.Top - 3; numPrecio.Width = 100;
            numPrecio.DecimalPlaces = 2; numPrecio.Maximum = 1000000;

            var lblStock = new Label { Text = "Stock", Left = 20, Top = lblPrecio.Bottom + 12, AutoSize = true };
            numStock.Left = 90; numStock.Top = lblStock.Top - 3; numStock.Width = 100;
            numStock.Maximum = 100000;
            numStock.Enabled = false; 

            btnAñadirStock.Text = "Añadir stock";
            btnAñadirStock.Left = numStock.Right + 10;
            btnAñadirStock.Top = numStock.Top;
            btnAñadirStock.Width = 100;
            btnAñadirStock.Click += BtnAñadirStock_Click;

            btnRemoverStock.Text = "Remover stock";
            btnRemoverStock.Left = btnAñadirStock.Right + 10;
            btnRemoverStock.Top = numStock.Top;
            btnRemoverStock.Width = 110;
            btnRemoverStock.Click += BtnRemoverStock_Click;

            var lblCat = new Label { Text = "Categoría", Left = 20, Top = lblStock.Bottom + 12, AutoSize = true };
            cboCategoria.Left = 90; cboCategoria.Top = lblCat.Top - 3; cboCategoria.Width = 240;
            cboCategoria.DropDownStyle = ComboBoxStyle.DropDownList;

            btnAgregar.Text = "Agregar";
            btnAgregar.Left = 90; btnAgregar.Top = lblCat.Bottom + 18; btnAgregar.Width = 100;
            btnAgregar.Click += BtnAgregar_Click;

            btnEditar.Text = "Editar";
            btnEditar.Left = btnAgregar.Right + 10; btnEditar.Top = btnAgregar.Top; btnEditar.Width = 100;
            btnEditar.Click += BtnEditar_Click;

            btnEliminar.Text = "Eliminar";
            btnEliminar.Left = btnEditar.Right + 10; btnEliminar.Top = btnAgregar.Top; btnEliminar.Width = 100;
            btnEliminar.Click += BtnEliminar_Click;

            Controls.AddRange(new Control[] {
                lblNombre, txtNombre, lblPrecio, numPrecio, lblStock, numStock,
                btnAñadirStock, btnRemoverStock,
                lblCat, cboCategoria, btnAgregar, btnEditar, btnEliminar
            });

            dgv.SelectionChanged += (_, __) =>
            {
                if (dgv.CurrentRow?.DataBoundItem is DataRowView r)
                {
                    txtNombre.Text = r["Nombre"]?.ToString();
                    numPrecio.Value = Convert.ToDecimal(r["Precio"]);
                    numStock.Value = Convert.ToDecimal(r["Stock"]);
                    cboCategoria.SelectedValue = Convert.ToInt32(r["CategoriaId"]);
                }
            };

            Load += (_, __) => { CargarCategorias(); Cargar(); };

            ResumeLayout(false);
        }

        private void CargarCategorias()
        {
            using var c = Db.Con(); c.Open();
            using var da = new MySqlDataAdapter("SELECT Id, Nombre FROM Categorias ORDER BY Nombre;", c);
            var t = new DataTable(); da.Fill(t);
            cboCategoria.DataSource = t;
            cboCategoria.ValueMember = "Id";
            cboCategoria.DisplayMember = "Nombre";
        }

        private void Cargar()
        {
            using var c = Db.Con(); c.Open();
            using var da = new MySqlDataAdapter(
                @"SELECT p.Id, p.Nombre, p.Precio, p.Stock,
                         p.CategoriaId, c.Nombre AS Categoria
                  FROM Productos p
                  JOIN Categorias c ON c.Id = p.CategoriaId
                  WHERE p.Activo = 1  -- Solo productos activos
                  ORDER BY p.Id DESC;", c);
            var table = new DataTable(); da.Fill(table);
            dgv.DataSource = table;

            if (dgv.Columns["Precio"] != null)
                dgv.Columns["Precio"].DefaultCellStyle.Format = "0.00";
        }

        private int? ProductoSeleccionadoId()
        {
            if (dgv.CurrentRow == null) return null;
            if (dgv.CurrentRow.DataBoundItem is not DataRowView row) return null;
            return Convert.ToInt32(row["Id"]);
        }
        private void FiltrarProductos()
        {
            var filtro = txtBusquedaProductos.Text.Trim().ToLower();
            var dt = dgv.DataSource as DataTable;
            if (dt != null)
            {
                if (string.IsNullOrWhiteSpace(filtro))
                    dt.DefaultView.RowFilter = "";
                else
                    dt.DefaultView.RowFilter = $"Nombre LIKE '%{filtro}%'";
            }
        }
        private void BtnAgregar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            { MessageBox.Show("Nombre requerido"); return; }

            try
            {
                using var c = Db.Con(); c.Open();
                using var cmd = new MySqlCommand(
                    "INSERT INTO Productos(Nombre,Precio,Stock,CategoriaId,Activo) VALUES(@n,@p,@s,@cat,1);", c);
                cmd.Parameters.AddWithValue("@n", txtNombre.Text.Trim());
                cmd.Parameters.AddWithValue("@p", numPrecio.Value);
                cmd.Parameters.AddWithValue("@s", 1); // Siempre 1 producto de stock al agregar
                cmd.Parameters.AddWithValue("@cat", Convert.ToInt32(cboCategoria.SelectedValue));
                cmd.ExecuteNonQuery();

                txtNombre.Clear(); numPrecio.Value = 0; numStock.Value = 0;
                Cargar();
            }
            catch (Exception ex) { MessageBox.Show("Error al agregar: " + ex.Message); }
        }

        private void BtnEditar_Click(object? sender, EventArgs e)
        {
            var id = ProductoSeleccionadoId();
            if (id == null) { MessageBox.Show("Selecciona un producto."); return; }
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            { MessageBox.Show("Nombre requerido"); return; }

            try
            {
                using var c = Db.Con(); c.Open();
                using var cmd = new MySqlCommand(
                    @"UPDATE Productos
                      SET Nombre=@n, Precio=@p, CategoriaId=@cat
                      WHERE Id=@id;", c); // Stock NO editable aquí
                cmd.Parameters.AddWithValue("@n", txtNombre.Text.Trim());
                cmd.Parameters.AddWithValue("@p", numPrecio.Value);
                cmd.Parameters.AddWithValue("@cat", Convert.ToInt32(cboCategoria.SelectedValue));
                cmd.Parameters.AddWithValue("@id", id.Value);
                cmd.ExecuteNonQuery();

                Cargar();
                MessageBox.Show("Producto actualizado.");
            }
            catch (Exception ex) { MessageBox.Show("Error al editar: " + ex.Message); }
        }

        private void BtnEliminar_Click(object? sender, EventArgs e)
        {
            var id = ProductoSeleccionadoId();
            if (id == null) { MessageBox.Show("Selecciona un producto."); return; }

            try
            {
                using var c = Db.Con(); c.Open();
                using var cmd = new MySqlCommand("UPDATE Productos SET Activo = 0 WHERE Id=@id;", c);
                cmd.Parameters.AddWithValue("@id", id.Value);
                cmd.ExecuteNonQuery();

                Cargar();
                MessageBox.Show("Producto eliminado.");
            }
            catch (Exception ex) { MessageBox.Show("Error al inactivar: " + ex.Message); }
        }
        private void BtnAñadirStock_Click(object? sender, EventArgs e)
        {
            var id = ProductoSeleccionadoId();
            if (id == null)
            {
                MessageBox.Show("Selecciona un producto.");
                return;
            }

            var input = Microsoft.VisualBasic.Interaction.InputBox("Cantidad a añadir:", "Añadir Stock", "1");
            if (int.TryParse(input, out int cantidad) && cantidad > 0)
            {
                using var c = Db.Con(); c.Open();
                using var tx = c.BeginTransaction();
                try
                {
                    using var cmdUpdate = new MySqlCommand(
                        "UPDATE Productos SET Stock = Stock + @cant WHERE Id = @id;", c, tx);
                    cmdUpdate.Parameters.AddWithValue("@cant", cantidad);
                    cmdUpdate.Parameters.AddWithValue("@id", id.Value);
                    cmdUpdate.ExecuteNonQuery();

                    using var cmdMov = new MySqlCommand(
                        "INSERT INTO MovimientosInventario (ProductoId, Tipo, Cantidad) VALUES (@id, 'entrada', @cant);", c, tx);
                    cmdMov.Parameters.AddWithValue("@id", id.Value);
                    cmdMov.Parameters.AddWithValue("@cant", cantidad);
                    cmdMov.ExecuteNonQuery();

                    tx.Commit();
                    MessageBox.Show("Stock añadido correctamente.");
                    Cargar();
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    MessageBox.Show("Error al añadir stock: " + ex.Message);
                }
            }
        }

        private void BtnRemoverStock_Click(object? sender, EventArgs e)
        {
            var id = ProductoSeleccionadoId();
            if (id == null)
            {
                MessageBox.Show("Selecciona un producto.");
                return;
            }
            int stockActual = (int)numStock.Value;
            var frm = new FrmRemoverStock(stockActual);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                int cantidad = frm.CantidadRemover;
                string motivo = frm.Motivo;

                using var c = Db.Con(); c.Open();
                using var tx = c.BeginTransaction();
                try
                {
                    using var cmdUpdate = new MySqlCommand(
                        "UPDATE Productos SET Stock = Stock - @cant WHERE Id = @id;", c, tx);
                    cmdUpdate.Parameters.AddWithValue("@cant", cantidad);
                    cmdUpdate.Parameters.AddWithValue("@id", id.Value);
                    cmdUpdate.ExecuteNonQuery();

                    using var cmdMov = new MySqlCommand(
                        "INSERT INTO MovimientosInventario (ProductoId, Tipo, Cantidad, Motivo) VALUES (@id, 'salida', @cant, @motivo);", c, tx);
                    cmdMov.Parameters.AddWithValue("@id", id.Value);
                    cmdMov.Parameters.AddWithValue("@cant", cantidad);
                    cmdMov.Parameters.AddWithValue("@motivo", motivo);
                    cmdMov.ExecuteNonQuery();

                    tx.Commit();
                    MessageBox.Show("Stock removido correctamente.");
                    Cargar();
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    MessageBox.Show("Error al remover stock: " + ex.Message);
                }
            }
        }
    }
}