using MySql.Data.MySqlClient;
using Proyecto_final_poo.Data;
using Proyecto_final_poo.Domain;
using System.Data;
using System.Windows.Forms;

namespace Proyecto_final_poo.UI
{
    public class FrmCategorias : Form
    {
        // tabla para mostrar categorias
        private readonly DataGridView dgv = new();
        // caja para escribir el nombre de la categoria
        private readonly TextBox txtNombre = new();
        // caja para escribir la descripcion de la categoria
        private readonly TextBox txtDescripcion = new();
        // boton para agregar categoria
        private readonly Button btnAgregar = new();
        // boton para editar categoria seleccionada
        private readonly Button btnEditar = new();
        // boton para eliminar categoria seleccionada
        private readonly Button btnEliminar = new();

        public FrmCategorias()
        {
            // configuracion basica del formulario
            Text = "Categorías";
            Width = 560; Height = 420;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            SuspendLayout();

            // configuracion de la tabla de datos
            dgv.Dock = DockStyle.Top; dgv.Height = 240; dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // etiqueta y caja para nombre de categoria
            var lblNombre = new Label { Text = "Nombre", Left = 20, Top = 255, AutoSize = true };
            txtNombre.Left = 90; txtNombre.Top = 252; txtNombre.Width = 420;

            // etiqueta y caja para descripcion de categoria
            var lblDesc = new Label { Text = "Descripción", Left = 20, Top = 285, AutoSize = true };
            txtDescripcion.Left = 90; txtDescripcion.Top = 282; txtDescripcion.Width = 420;

            // configuracion del boton agregar
            btnAgregar.Text = "Agregar";
            btnAgregar.Left = 90; btnAgregar.Top = 320; btnAgregar.Width = 100;
            btnAgregar.Click += BtnAgregar_Click;

            // configuracion del boton editar
            btnEditar.Text = "Editar";
            btnEditar.Left = 200; btnEditar.Top = 320; btnEditar.Width = 100;
            btnEditar.Click += BtnEditar_Click;

            // configuracion del boton eliminar
            btnEliminar.Text = "Eliminar";
            btnEliminar.Left = 310; btnEliminar.Top = 320; btnEliminar.Width = 100;
            btnEliminar.Click += BtnEliminar_Click;

            // agrega todos los controles al formulario
            Controls.AddRange(new Control[] {
                dgv, lblNombre, txtNombre, lblDesc, txtDescripcion,
                btnAgregar, btnEditar, btnEliminar
            });

            // al cambiar la seleccion en la tabla, actualiza los campos de texto
            dgv.SelectionChanged += (_, __) =>
            {
                if (dgv.CurrentRow?.DataBoundItem is DataRowView r)
                {
                    txtNombre.Text = r["Nombre"]?.ToString();
                    txtDescripcion.Text = r["Descripcion"]?.ToString();
                }
            };

            // al cargar el formulario, carga las categorias en la tabla
            Load += (_, __) => Cargar();
            ResumeLayout(false);
        }

        // carga las categorias desde la base de datos y las muestra en la tabla
        private void Cargar()
        {
            using var c = Db.Con(); c.Open();
            using var da = new MySqlDataAdapter(
                "SELECT Id, Nombre, Descripcion FROM Categorias ORDER BY Id DESC;", c);
            var t = new DataTable(); da.Fill(t);
            dgv.DataSource = t;
        }

        // obtiene la categoria seleccionada en la tabla
        private Categoria? CategoriaSeleccionada()
        {
            if (dgv.CurrentRow?.DataBoundItem is not DataRowView row) return null;
            return new Categoria
            {
                Id = Convert.ToInt32(row["Id"]),
                Nombre = row["Nombre"]?.ToString() ?? "",
                Descripcion = row["Descripcion"]?.ToString()
            };
        }

        // evento para agregar una nueva categoria
        private void BtnAgregar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            { MessageBox.Show("Nombre requerido."); return; }

            var cat = new Categoria
            {
                Nombre = txtNombre.Text.Trim(),
                Descripcion = string.IsNullOrWhiteSpace(txtDescripcion.Text) ? null : txtDescripcion.Text.Trim()
            };

            try
            {
                using var c = Db.Con(); c.Open();
                using var cmd = new MySqlCommand(
                    "INSERT INTO Categorias(Nombre, Descripcion) VALUES(@n, @d);", c);
                cmd.Parameters.AddWithValue("@n", cat.Nombre);
                cmd.Parameters.AddWithValue("@d", (object?)cat.Descripcion ?? DBNull.Value);
                cmd.ExecuteNonQuery();
                Cargar();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error al agregar: " + ex.Message);
            }
        }

        // evento para editar la categoria seleccionada
        private void BtnEditar_Click(object? sender, EventArgs e)
        {
            var cat = CategoriaSeleccionada();
            if (cat == null)
            { MessageBox.Show("Selecciona una categoría."); return; }
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            { MessageBox.Show("Nombre requerido."); return; }

            cat.Nombre = txtNombre.Text.Trim();
            cat.Descripcion = string.IsNullOrWhiteSpace(txtDescripcion.Text) ? null : txtDescripcion.Text.Trim();

            try
            {
                using var c = Db.Con(); c.Open();
                using var cmd = new MySqlCommand(
                    "UPDATE Categorias SET Nombre=@n, Descripcion=@d WHERE Id=@id;", c);
                cmd.Parameters.AddWithValue("@n", cat.Nombre);
                cmd.Parameters.AddWithValue("@d", (object?)cat.Descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@id", cat.Id);
                cmd.ExecuteNonQuery();
                Cargar();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error al editar: " + ex.Message);
            }
        }

        // evento para eliminar la categoria seleccionada
        private void BtnEliminar_Click(object? sender, EventArgs e)
        {
            var cat = CategoriaSeleccionada();
            if (cat == null)
            { MessageBox.Show("Selecciona una categoría."); return; }

            if (MessageBox.Show("¿Seguro que deseas eliminar esta categoría?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using var c = Db.Con(); c.Open();
                    using var cmd = new MySqlCommand("DELETE FROM Categorias WHERE Id=@id;", c);
                    cmd.Parameters.AddWithValue("@id", cat.Id);
                    cmd.ExecuteNonQuery();
                    Cargar();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error al eliminar: " + ex.Message);
                }
            }
        }
    }
}