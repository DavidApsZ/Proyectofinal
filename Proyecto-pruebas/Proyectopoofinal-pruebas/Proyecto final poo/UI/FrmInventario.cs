using MySql.Data.MySqlClient;
using Proyecto_final_poo.Data;
using System.Data;
using System.Windows.Forms;

namespace Proyecto_final_poo.UI
{
    public class FrmInventario : Form
    {
        // selector numerico para el umbral de stock bajo
        private readonly NumericUpDown numUmbral = new();
        // boton para buscar productos por debajo del umbral
        private readonly Button btnBuscar = new();
        // tabla para mostrar los productos de bajo stock
        private readonly DataGridView dgv = new();
        // caja de texto para filtrar por nombre del producto
        private readonly TextBox txtBusquedaInventario = new();
        // etiqueta para la caja de busqueda
        private readonly Label lblBusquedaInventario = new();

        public FrmInventario()
        {
            // configuracion basica del formulario
            Text = "Inventario - Bajo stock";
            Width = 700;
            Height = 540;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;

            // etiqueta y selector para el umbral
            var lblU = new Label { Text = "Umbral <", Left = 20, Top = 18, AutoSize = true };
            numUmbral.Left = 85; numUmbral.Top = 14; numUmbral.Width = 80; numUmbral.Value = 5; numUmbral.Maximum = 100000;
            // boton para buscar
            btnBuscar.Text = "Buscar"; btnBuscar.Left = 180; btnBuscar.Top = 12; btnBuscar.Width = 100;
            btnBuscar.Click += (_, __) => Buscar();

            // etiqueta para buscar por nombre
            lblBusquedaInventario.Text = "Buscar:";
            lblBusquedaInventario.Left = btnBuscar.Right + 20;
            lblBusquedaInventario.Top = 18;
            lblBusquedaInventario.AutoSize = true;
            Controls.Add(lblBusquedaInventario);

            // caja de texto para busqueda por nombre
            txtBusquedaInventario.Left = lblBusquedaInventario.Right + 8;
            txtBusquedaInventario.Top = 14;
            txtBusquedaInventario.Width = 220;
            txtBusquedaInventario.PlaceholderText = "Nombre del producto...";
            Controls.Add(txtBusquedaInventario);

            // evento para filtrar mientras se escribe
            txtBusquedaInventario.TextChanged += (_, __) => FiltrarInventario();

            // tabla de productos
            dgv.Left = 20;
            dgv.Top = txtBusquedaInventario.Bottom + 8;
            dgv.Width = 640;
            dgv.Height = 360;
            dgv.ReadOnly = true;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Controls.Add(dgv);

            Controls.AddRange(new Control[] { lblU, numUmbral, btnBuscar });

            // al cargar el form busca productos de bajo stock
            Load += (_, __) => Buscar();

            ResumeLayout(false);
        }

        // busca productos de bajo stock segun el umbral seleccionado
        private void Buscar()
        {
            try
            {
                using var c = Db.Con(); c.Open();
                using var da = new MySqlDataAdapter(
                    @"SELECT p.Id, p.Nombre, p.Stock, c.Nombre AS Categoria
                      FROM Productos p 
                      JOIN Categorias c ON c.Id = p.CategoriaId
                      WHERE p.Stock < @min AND p.Activo = 1
                      ORDER BY p.Stock ASC;", c);
                da.SelectCommand.Parameters.AddWithValue("@min", (int)numUmbral.Value);

                var t = new DataTable(); da.Fill(t);
                dgv.DataSource = t;
                FiltrarInventario();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"error al buscar productos con bajo stock: {ex.Message}");
            }
        }

        // filtra el inventario por nombre de producto
        private void FiltrarInventario()
        {
            var filtro = txtBusquedaInventario.Text.Trim().ToLower();
            var dt = dgv.DataSource as DataTable;
            if (dt != null)
            {
                if (string.IsNullOrWhiteSpace(filtro))
                    dt.DefaultView.RowFilter = "";
                else
                    dt.DefaultView.RowFilter = $"Nombre LIKE '%{filtro}%'";
            }
        }
    }
}