using MySql.Data.MySqlClient;
using Proyecto_final_poo.Data;
using System.Data;

namespace Proyecto_final_poo.UI
{
    public class FrmCategorias : Form
    {
        private readonly DataGridView dgv = new();
        private readonly TextBox txtNombre = new();
        private readonly TextBox txtDescripcion = new();
        private readonly Button btnAgregar = new();
        private readonly Button btnEditar = new();
        private readonly Button btnEliminar = new();

        public FrmCategorias()
        {
            Text = "Categorías";
            Width = 560; Height = 420;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            SuspendLayout();

            dgv.Dock = DockStyle.Top; dgv.Height = 240; dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            var lblNombre = new Label { Text = "Nombre", Left = 20, Top = 255, AutoSize = true };
            txtNombre.Left = 90; txtNombre.Top = 252; txtNombre.Width = 420;

            var lblDesc = new Label { Text = "Descripción", Left = 20, Top = 285, AutoSize = true };
            txtDescripcion.Left = 90; txtDescripcion.Top = 282; txtDescripcion.Width = 420;

            btnAgregar.Text = "Agregar";
            btnAgregar.Left = 90; btnAgregar.Top = 320; btnAgregar.Width = 100;
            btnAgregar.Click += BtnAgregar_Click;

            btnEditar.Text = "Editar";
            btnEditar.Left = 200; btnEditar.Top = 320; btnEditar.Width = 100;
            btnEditar.Click += BtnEditar_Click;

            btnEliminar.Text = "Eliminar";
            btnEliminar.Left = 310; btnEliminar.Top = 320; btnEliminar.Width = 100;
            btnEliminar.Click += BtnEliminar_Click;

            Controls.AddRange(new Control[] {
                dgv, lblNombre, txtNombre, lblDesc, txtDescripcion,
                btnAgregar, btnEditar, btnEliminar
            });

            dgv.SelectionChanged += (_, __) =>
            {
                if (dgv.CurrentRow?.DataBoundItem is DataRowView r)
                {
                    txtNombre.Text = r["Nombre"]?.ToString();
                    txtDescripcion.Text = r["Descripcion"]?.ToString();
                }
            };

            Load += (_, __) => Cargar();
            ResumeLayout(false);
        }

        private void Cargar()
        {
            using var c = Db.Con(); c.Open();
            using var da = new MySqlDataAdapter(
                "SELECT Id, Nombre, Descripcion FROM Categorias ORDER BY Id DESC;", c);
            var t = new DataTable(); da.Fill(t);
            dgv.DataSource = t;
        }

        private int? CategoriaSeleccionadaId()
        {
            if (dgv.CurrentRow?.DataBoundItem is not DataRowView row) return null;
            return Convert.ToInt32(row["Id"]);
        }

        private void BtnAgregar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            { MessageBox.Show("Nombre requerido."); return; }

            try
            {
                using var c = Db.Con(); c.Open();
                using var cmd = new MySqlCommand(
                    "INSERT INTO Categorias(Nombre, Descripcion) VALUES(@n, @d);", c);
                cmd.Parameters.AddWithValue("@n", txtNombre.Text.Trim());
                cmd.Parameters.AddWithValue("@d", string.IsNullOrWhiteSpace(txtDescripcion.Text)
                                                ? (object)DBNull.Value : txtDescripcion.Text.Trim());
                cmd.ExecuteNonQuery();

                txtNombre.Clear(); txtDescripcion.Clear();
                Cargar();
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                MessageBox.Show("Ya existe una categoría con ese nombre.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar: " + ex.Message);
            }
        }

        private void BtnEditar_Click(object? sender, EventArgs e)
        {
            var id = CategoriaSeleccionadaId();
            if (id == null) { MessageBox.Show("Selecciona una categoría."); return; }
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            { MessageBox.Show("Nombre requerido."); return; }

            try
            {
                using var c = Db.Con(); c.Open();
                using var cmd = new MySqlCommand(
                    @"UPDATE Categorias
                      SET Nombre = @n, Descripcion = @d
                      WHERE Id = @id;", c);
                cmd.Parameters.AddWithValue("@n", txtNombre.Text.Trim());
                cmd.Parameters.AddWithValue("@d", string.IsNullOrWhiteSpace(txtDescripcion.Text)
                                                ? (object)DBNull.Value : txtDescripcion.Text.Trim());
                cmd.Parameters.AddWithValue("@id", id.Value);
                cmd.ExecuteNonQuery();

                Cargar();
                MessageBox.Show("Categoría actualizada.");
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                MessageBox.Show("Ya existe una categoría con ese nombre.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al editar: " + ex.Message);
            }
        }

        private void BtnEliminar_Click(object? sender, EventArgs e)
        {
            var id = CategoriaSeleccionadaId();
            if (id == null) { MessageBox.Show("Selecciona una categoría."); return; }

            if (MessageBox.Show("¿Eliminar la categoría seleccionada?",
                "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                using var c = Db.Con(); c.Open();

                using (var chk = new MySqlCommand("SELECT COUNT(*) FROM Productos WHERE CategoriaId=@id;", c))
                {
                    chk.Parameters.AddWithValue("@id", id.Value);
                    var usados = Convert.ToInt32(chk.ExecuteScalar());
                    if (usados > 0)
                    {
                        MessageBox.Show("No se puede eliminar: hay productos con esta categoría.");
                        return;
                    }
                }

                using var cmd = new MySqlCommand("DELETE FROM Categorias WHERE Id=@id;", c);
                cmd.Parameters.AddWithValue("@id", id.Value);
                cmd.ExecuteNonQuery();

                Cargar();
                MessageBox.Show("Categoría eliminada.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message);
            }
        }
    }
}
