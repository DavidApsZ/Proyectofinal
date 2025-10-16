using MySql.Data.MySqlClient;
using Proyecto_final_poo.Data;
using System.Data;

namespace Proyecto_final_poo.UI
{
    public class FrmInventario : Form
    {
        private readonly NumericUpDown numUmbral = new();
        private readonly Button btnBuscar = new();
        private readonly DataGridView dgv = new();
        private readonly TextBox txtBusquedaInventario = new();
        private readonly Label lblBusquedaInventario = new();

        public FrmInventario()
        {
            Text = "Inventario - Bajo stock";
            Width = 700;
            Height = 540; 
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;

            var lblU = new Label { Text = "Umbral <", Left = 20, Top = 18, AutoSize = true };
            numUmbral.Left = 85; numUmbral.Top = 14; numUmbral.Width = 80; numUmbral.Value = 5; numUmbral.Maximum = 100000;
            btnBuscar.Text = "Buscar"; btnBuscar.Left = 180; btnBuscar.Top = 12; btnBuscar.Width = 100;
            btnBuscar.Click += (_, __) => Buscar();

            lblBusquedaInventario.Text = "Buscar:";
            lblBusquedaInventario.Left = btnBuscar.Right + 20;
            lblBusquedaInventario.Top = 18;
            lblBusquedaInventario.AutoSize = true;
            Controls.Add(lblBusquedaInventario);

            txtBusquedaInventario.Left = lblBusquedaInventario.Right + 8;
            txtBusquedaInventario.Top = 14;
            txtBusquedaInventario.Width = 220;
            txtBusquedaInventario.PlaceholderText = "Nombre del producto...";
            Controls.Add(txtBusquedaInventario);

            txtBusquedaInventario.TextChanged += (_, __) => FiltrarInventario();

            dgv.Left = 20;
            dgv.Top = txtBusquedaInventario.Bottom + 8; 
            dgv.Width = 640;
            dgv.Height = 360; 
            dgv.ReadOnly = true;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Controls.Add(dgv);

            Controls.AddRange(new Control[] { lblU, numUmbral, btnBuscar });

            Load += (_, __) => Buscar();

            ResumeLayout(false);
        }

        private void Buscar()
        {
            try
            {
                using var c = Db.Con(); c.Open();
                using var da = new MySqlDataAdapter(
                    @"SELECT p.Id, p.Nombre, p.Stock, c.Nombre AS Categoria
                      FROM Productos p JOIN Categorias c ON c.Id = p.CategoriaId
                      WHERE p.Stock < @min ORDER BY p.Stock ASC;", c);
                da.SelectCommand.Parameters.AddWithValue("@min", (int)numUmbral.Value);

                var t = new DataTable(); da.Fill(t);
                dgv.DataSource = t;
                FiltrarInventario(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar productos con bajo stock: {ex.Message}");
            }
        }
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