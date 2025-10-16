using MySql.Data.MySqlClient;
using Proyecto_final_poo.Data;
using System.Data;

namespace Proyecto_final_poo.UI
{
    public class FrmProductos : Form
    {
        // tabla para mostrar los productos
        private readonly DataGridView dgv = new();
        // caja para nombre del producto
        private readonly TextBox txtNombre = new();
        // selector numerico para el precio
        private readonly NumericUpDown numPrecio = new();
        // selector numerico para el stock (no editable directo)
        private readonly NumericUpDown numStock = new();
        // combo para seleccionar la categoria
        private readonly ComboBox cboCategoria = new();
        // boton para agregar producto
        private readonly Button btnAgregar = new();
        // boton para editar producto seleccionado
        private readonly Button btnEditar = new();
        // boton para eliminar producto seleccionado
        private readonly Button btnEliminar = new();
        // caja para filtrar productos por nombre
        private readonly TextBox txtBusquedaProductos = new();
        // etiqueta para la busqueda
        private readonly Label lblBusqueda = new();
        // boton para añadir stock
        private readonly Button btnAñadirStock = new();
        // boton para remover stock
        private readonly Button btnRemoverStock = new();

        public FrmProductos()
        {
            // configuracion basica del formulario
            Text = "Productos";
            Width = 560; Height = 540;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            SuspendLayout();

            // etiqueta y caja para busqueda
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

            // evento para filtrar mientras se escribe
            txtBusquedaProductos.TextChanged += (_, __) => FiltrarProductos();

            // configuracion de la tabla de productos
            dgv.Left = 20;
            dgv.Top = txtBusquedaProductos.Bottom + 8;
            dgv.Width = 520;
            dgv.Height = 160;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Controls.Add(dgv);

            // campos para agregar/editar producto
            var lblNombre = new Label { Text = "Nombre", Left = 20, Top = dgv.Bottom + 15, AutoSize = true };
            txtNombre.Left = 90; txtNombre.Top = lblNombre.Top - 3; txtNombre.Width = 320;

            var lblPrecio = new Label { Text = "Precio", Left = 20, Top = lblNombre.Bottom + 12, AutoSize = true };
            numPrecio.Left = 90; numPrecio.Top = lblPrecio.Top - 3; numPrecio.Width = 100;
            numPrecio.DecimalPlaces = 2; numPrecio.Maximum = 1000000;

            var lblStock = new Label { Text = "Stock", Left = 20, Top = lblPrecio.Bottom + 12, AutoSize = true };
            numStock.Left = 90; numStock.Top = lblStock.Top - 3; numStock.Width = 100;
            numStock.Maximum = 100000;
            numStock.Enabled = false;

            // boton para añadir stock
            btnAñadirStock.Text = "Añadir stock";
            btnAñadirStock.Left = numStock.Right + 10;
            btnAñadirStock.Top = numStock.Top;
            btnAñadirStock.Width = 100;
            btnAñadirStock.Click += BtnAñadirStock_Click;

            // boton para remover stock
            btnRemoverStock.Text = "Remover stock";
            btnRemoverStock.Left = btnAñadirStock.Right + 10;
            btnRemoverStock.Top = numStock.Top;
            btnRemoverStock.Width = 110;
            btnRemoverStock.Click += BtnRemoverStock_Click;

            var lblCat = new Label { Text = "Categoría", Left = 20, Top = lblStock.Bottom + 12, AutoSize = true };
            cboCategoria.Left = 90; cboCategoria.Top = lblCat.Top - 3; cboCategoria.Width = 240;
            cboCategoria.DropDownStyle = ComboBoxStyle.DropDownList;

            // botones de accion
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

            // cuando cambia la seleccion en la tabla, copia los datos a los campos
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

            // al cargar el formulario, carga las categorias y productos
            Load += (_, __) => { CargarCategorias(); Cargar(); };

            ResumeLayout(false);
        }

        // carga las categorias en el combo
        private void CargarCategorias()
        {
            using var c = Db.Con(); c.Open();
            using var da = new MySqlDataAdapter("SELECT Id, Nombre FROM Categorias ORDER BY Nombre;", c);
            var t = new DataTable(); da.Fill(t);
            cboCategoria.DataSource = t;
            cboCategoria.ValueMember = "Id";
            cboCategoria.DisplayMember = "Nombre";
        }

        // carga los productos en la tabla
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

        // obtiene el id del producto seleccionado en la tabla
        private int? ProductoSeleccionadoId()
        {
            if (dgv.CurrentRow == null) return null;
            if (dgv.CurrentRow.DataBoundItem is not DataRowView row) return null;
            return Convert.ToInt32(row["Id"]);
        }
        // filtra productos por nombre
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
        // agrega un producto nuevo
        private void BtnAgregar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            { MessageBox.Show("nombre requerido"); return; }

            try
            {
                using var c = Db.Con(); c.Open();
                using var cmd = new MySqlCommand(
                    "INSERT INTO Productos(Nombre,Precio,Stock,CategoriaId,Activo) VALUES(@n,@p,@s,@cat,1);", c);
                cmd.Parameters.AddWithValue("@n", txtNombre.Text.Trim());
                cmd.Parameters.AddWithValue("@p", numPrecio.Value);
                cmd.Parameters.AddWithValue("@s", 1); // siempre 1 producto de stock al agregar
                cmd.Parameters.AddWithValue("@cat", Convert.ToInt32(cboCategoria.SelectedValue));
                cmd.ExecuteNonQuery();

                txtNombre.Clear(); numPrecio.Value = 0; numStock.Value = 0;
                Cargar();
            }
            catch (Exception ex) { MessageBox.Show("error al agregar: " + ex.Message); }
        }

        // edita el producto seleccionado
        private void BtnEditar_Click(object? sender, EventArgs e)
        {
            var id = ProductoSeleccionadoId();
            if (id == null) { MessageBox.Show("selecciona un producto."); return; }
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            { MessageBox.Show("nombre requerido"); return; }

            try
            {
                using var c = Db.Con(); c.Open();
                using var cmd = new MySqlCommand(
                    @"UPDATE Productos
                      SET Nombre=@n, Precio=@p, CategoriaId=@cat
                      WHERE Id=@id;", c); // stock no editable aqui
                cmd.Parameters.AddWithValue("@n", txtNombre.Text.Trim());
                cmd.Parameters.AddWithValue("@p", numPrecio.Value);
                cmd.Parameters.AddWithValue("@cat", Convert.ToInt32(cboCategoria.SelectedValue));
                cmd.Parameters.AddWithValue("@id", id.Value);
                cmd.ExecuteNonQuery();

                Cargar();
                MessageBox.Show("producto actualizado.");
            }
            catch (Exception ex) { MessageBox.Show("error al editar: " + ex.Message); }
        }

        // elimina (inactiva) el producto seleccionado
        private void BtnEliminar_Click(object? sender, EventArgs e)
        {
            var id = ProductoSeleccionadoId();
            if (id == null) { MessageBox.Show("selecciona un producto."); return; }

            try
            {
                using var c = Db.Con(); c.Open();
                using var cmd = new MySqlCommand("UPDATE Productos SET Activo = 0 WHERE Id=@id;", c);
                cmd.Parameters.AddWithValue("@id", id.Value);
                cmd.ExecuteNonQuery();

                Cargar();
                MessageBox.Show("producto eliminado.");
            }
            catch (Exception ex) { MessageBox.Show("error al inactivar: " + ex.Message); }
        }
        // añade stock al producto seleccionado
        private void BtnAñadirStock_Click(object? sender, EventArgs e)
        {
            var id = ProductoSeleccionadoId();
            if (id == null)
            {
                MessageBox.Show("selecciona un producto.");
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
                    MessageBox.Show("stock anadido correctamente.");
                    Cargar();
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    MessageBox.Show("error al anadir stock: " + ex.Message);
                }
            }
        }

        // remueve stock al producto seleccionado con motivo
        private void BtnRemoverStock_Click(object? sender, EventArgs e)
        {
            var id = ProductoSeleccionadoId();
            if (id == null)
            {
                MessageBox.Show("selecciona un producto.");
                return;
            }
            int stockActual = (int)numStock.Value;

            // formulario emergente para cantidad y motivo
            using var f = new Form() { Width = 400, Height = 180, Text = "Remover stock" };
            var lblCantidad = new Label { Text = "Cantidad a remover:", Left = 20, Top = 20, AutoSize = true };
            var numCantidad = new NumericUpDown { Left = 160, Top = 18, Width = 80, Minimum = 1, Maximum = stockActual, Value = 1 };
            var lblMotivo = new Label { Text = "Motivo:", Left = 20, Top = 60, AutoSize = true };
            var txtMotivo = new TextBox { Left = 80, Top = 58, Width = 260 };
            var btnOk = new Button { Text = "Aceptar", Left = 140, Top = 110, Width = 100, DialogResult = DialogResult.OK };
            f.Controls.AddRange(new Control[] { lblCantidad, numCantidad, lblMotivo, txtMotivo, btnOk });
            f.AcceptButton = btnOk;

            if (f.ShowDialog() == DialogResult.OK)
            {
                int cantidad = (int)numCantidad.Value;
                string motivo = txtMotivo.Text.Trim();

                if (string.IsNullOrWhiteSpace(motivo))
                {
                    MessageBox.Show("debes ingresar un motivo."); return;
                }

                using var c = Db.Con(); c.Open();
                using var tx = c.BeginTransaction();
                try
                {
                    using (var cmd = new MySqlCommand(
                        "UPDATE Productos SET Stock = Stock - @cant WHERE Id = @id;", c, tx))
                    {
                        cmd.Parameters.AddWithValue("@cant", cantidad);
                        cmd.Parameters.AddWithValue("@id", id.Value);
                        cmd.ExecuteNonQuery();
                    }
                    using (var cmd = new MySqlCommand(
                        @"INSERT INTO MovimientosInventario 
                            (ProductoId, Tipo, Cantidad, Motivo, Fecha) 
                          VALUES 
                            (@prod, 'salida', @cant, @motivo, @fecha);", c, tx))
                    {
                        cmd.Parameters.AddWithValue("@prod", id.Value);
                        cmd.Parameters.AddWithValue("@cant", cantidad);
                        cmd.Parameters.AddWithValue("@motivo", motivo);
                        cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                    tx.Commit();
                    MessageBox.Show("stock removido correctamente.");
                    Cargar();
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    MessageBox.Show("error al remover stock: " + ex.Message);
                }
            }
        }
    }
}