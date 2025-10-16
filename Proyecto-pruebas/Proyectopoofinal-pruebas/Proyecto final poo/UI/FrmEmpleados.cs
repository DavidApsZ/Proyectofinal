using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Proyecto_final_poo.Data;
using Proyecto_final_poo.Domain;
using System.Data;

namespace Proyecto_final_poo.UI
{
    public class FrmEmpleados : Form
    {
        // tabla para mostrar los empleados
        private readonly DataGridView dgv = new();
        // caja para escribir el nombre del empleado
        private readonly TextBox txtNombre = new();
        // caja para escribir el rol del empleado
        private readonly TextBox txtRol = new();
        // boton para agregar empleado
        private readonly Button btnAgregar = new();
        // boton para editar empleado seleccionado
        private readonly Button btnEditar = new();
        // boton para eliminar empleado seleccionado
        private readonly Button btnEliminar = new();

        public FrmEmpleados()
        {
            // configuracion basica del formulario
            Text = "Empleados";
            Width = 600; Height = 400;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;

            InicializarControles();
            // al cargar el formulario se cargan los empleados en la tabla
            Load += (_, __) => CargarEmpleados();
        }

        // metodo para inicializar los controles del formulario
        private void InicializarControles()
        {
            // configuracion de la tabla de empleados
            dgv.Dock = DockStyle.Top;
            dgv.Height = 160;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.MultiSelect = false;
            Controls.Add(dgv);

            // etiqueta y caja para nombre
            var lblNombre = new Label { Text = "Nombre", Left = 10, Top = dgv.Bottom + 10, Width = 60 };
            txtNombre.Left = 80; txtNombre.Top = dgv.Bottom + 8; txtNombre.Width = 180;

            // etiqueta y caja para rol
            var lblRol = new Label { Text = "Rol", Left = 270, Top = dgv.Bottom + 10, Width = 40 };
            txtRol.Left = 320; txtRol.Top = dgv.Bottom + 8; txtRol.Width = 140;

            Controls.Add(lblNombre);
            Controls.Add(txtNombre);
            Controls.Add(lblRol);
            Controls.Add(txtRol);

            // boton para agregar empleado
            btnAgregar.Text = "Agregar";
            btnAgregar.Left = 80; btnAgregar.Top = txtNombre.Bottom + 10; btnAgregar.Width = 80;
            btnAgregar.Click += BtnAgregar_Click;
            Controls.Add(btnAgregar);

            // boton para editar empleado
            btnEditar.Text = "Editar";
            btnEditar.Left = 200; btnEditar.Top = txtNombre.Bottom + 10; btnEditar.Width = 80;
            btnEditar.Click += BtnEditar_Click;
            Controls.Add(btnEditar);

            // boton para eliminar empleado
            btnEliminar.Text = "Eliminar";
            btnEliminar.Left = 320; btnEliminar.Top = txtNombre.Bottom + 10; btnEliminar.Width = 80;
            btnEliminar.Click += BtnEliminar_Click;
            Controls.Add(btnEliminar);

            // cuando cambia la seleccion en la tabla, copia los datos a las cajas de texto
            dgv.SelectionChanged += (_, __) => CopiarDatosFilaSeleccionada();
        }

        // metodo para cargar los empleados desde la base de datos y mostrarlos en la tabla
        private void CargarEmpleados()
        {
            using var c = Db.Con();
            c.Open();
            using var da = new MySqlDataAdapter("SELECT Id, Nombre, Rol FROM Empleados ORDER BY Id;", c);
            var t = new DataTable();
            da.Fill(t);
            dgv.DataSource = t;
        }

        // metodo para obtener el empleado seleccionado en la tabla
        private Empleado? EmpleadoSeleccionado()
        {
            if (dgv.CurrentRow?.DataBoundItem is not DataRowView row) return null;
            return new Empleado
            {
                Id = Convert.ToInt32(row["Id"]),
                Nombre = row["Nombre"]?.ToString() ?? "",
                Rol = row["Rol"]?.ToString()
            };
        }

        // metodo para copiar los datos del empleado seleccionado a las cajas de texto
        private void CopiarDatosFilaSeleccionada()
        {
            var empleado = EmpleadoSeleccionado();
            if (empleado == null)
            {
                txtNombre.Text = "";
                txtRol.Text = "";
                return;
            }
            txtNombre.Text = empleado.Nombre;
            txtRol.Text = empleado.Rol ?? "";
        }

        // evento para agregar un empleado nuevo
        private void BtnAgregar_Click(object? sender, System.EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("el nombre es obligatorio.");
                return;
            }

            var empleado = new Empleado
            {
                Nombre = txtNombre.Text.Trim(),
                Rol = string.IsNullOrWhiteSpace(txtRol.Text) ? null : txtRol.Text.Trim()
            };

            try
            {
                using var c = Db.Con(); c.Open();
                using var cmd = new MySqlCommand(
                    "INSERT INTO Empleados(Nombre, Rol) VALUES(@n, @r);", c);
                cmd.Parameters.AddWithValue("@n", empleado.Nombre);
                cmd.Parameters.AddWithValue("@r", (object?)empleado.Rol ?? DBNull.Value);
                cmd.ExecuteNonQuery();
                CargarEmpleados();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("error al agregar: " + ex.Message);
            }
        }

        // evento para editar el empleado seleccionado
        private void BtnEditar_Click(object? sender, System.EventArgs e)
        {
            var empleado = EmpleadoSeleccionado();
            if (empleado == null)
            {
                MessageBox.Show("selecciona un empleado.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("el nombre es obligatorio.");
                return;
            }

            empleado.Nombre = txtNombre.Text.Trim();
            empleado.Rol = string.IsNullOrWhiteSpace(txtRol.Text) ? null : txtRol.Text.Trim();

            try
            {
                using var c = Db.Con(); c.Open();
                using var cmd = new MySqlCommand(
                    "UPDATE Empleados SET Nombre=@n, Rol=@r WHERE Id=@id;", c);
                cmd.Parameters.AddWithValue("@n", empleado.Nombre);
                cmd.Parameters.AddWithValue("@r", (object?)empleado.Rol ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@id", empleado.Id);
                cmd.ExecuteNonQuery();
                CargarEmpleados();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("error al editar: " + ex.Message);
            }
        }

        // evento para eliminar el empleado seleccionado
        private void BtnEliminar_Click(object? sender, System.EventArgs e)
        {
            var empleado = EmpleadoSeleccionado();
            if (empleado == null)
            {
                MessageBox.Show("selecciona un empleado.");
                return;
            }

            if (MessageBox.Show("¿seguro que deseas eliminar este empleado?", "confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using var c = Db.Con(); c.Open();
                    using var cmd = new MySqlCommand("DELETE FROM Empleados WHERE Id=@id;", c);
                    cmd.Parameters.AddWithValue("@id", empleado.Id);
                    cmd.ExecuteNonQuery();
                    CargarEmpleados();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("error al eliminar: " + ex.Message);
                }
            }
        }
    }
}