using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Proyecto_final_poo.Data;
using Proyecto_final_poo.Domain;
using System.Data;

namespace Proyecto_final_poo.UI
{
    public class FrmClientes : Form
    {
        // tabla para mostrar los clientes
        private readonly DataGridView dgv = new();
        // caja de texto para el nombre del cliente
        private readonly TextBox txtNombre = new();
        // caja de texto para el telefono del cliente
        private readonly TextBox txtTelefono = new();
        // caja de texto para el email del cliente
        private readonly TextBox txtEmail = new();
        // boton para agregar cliente
        private readonly Button btnAgregar = new();
        // boton para editar cliente seleccionado
        private readonly Button btnEditar = new();
        // boton para eliminar cliente seleccionado
        private readonly Button btnEliminar = new();
        // boton para ver el historial de compras del cliente
        private readonly Button btnHistorial = new();

        public FrmClientes()
        {
            // configuracion basica del formulario
            Text = "Clientes";
            Width = 600; Height = 400;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;

            InicializarControles();
            // al cargar el formulario se cargan los clientes en la tabla
            Load += (_, __) => CargarClientes();
        }

        // metodo para inicializar los controles del formulario
        private void InicializarControles()
        {
            // configuracion de la tabla de clientes
            dgv.Dock = DockStyle.Top;
            dgv.Height = 160;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.MultiSelect = false;
            Controls.Add(dgv);

            // etiqueta y caja para nombre
            var lblNombre = new Label { Text = "Nombre", Left = 10, Top = dgv.Bottom + 10, Width = 60 };
            txtNombre.Left = 80; txtNombre.Top = dgv.Bottom + 8; txtNombre.Width = 140;

            // etiqueta y caja para telefono
            var lblTelefono = new Label { Text = "Teléfono", Left = 230, Top = dgv.Bottom + 10, Width = 60 };
            txtTelefono.Left = 300; txtTelefono.Top = dgv.Bottom + 8; txtTelefono.Width = 120;

            // etiqueta y caja para email
            var lblEmail = new Label { Text = "Email", Left = 430, Top = dgv.Bottom + 10, Width = 40 };
            txtEmail.Left = 480; txtEmail.Top = dgv.Bottom + 8; txtEmail.Width = 100;

            Controls.Add(lblNombre);
            Controls.Add(txtNombre);
            Controls.Add(lblTelefono);
            Controls.Add(txtTelefono);
            Controls.Add(lblEmail);
            Controls.Add(txtEmail);

            // boton para agregar cliente
            btnAgregar.Text = "Agregar";
            btnAgregar.Left = 80; btnAgregar.Top = txtNombre.Bottom + 10; btnAgregar.Width = 80;
            btnAgregar.Click += BtnAgregar_Click;
            Controls.Add(btnAgregar);

            // boton para editar cliente
            btnEditar.Text = "Editar";
            btnEditar.Left = 200; btnEditar.Top = txtNombre.Bottom + 10; btnEditar.Width = 80;
            btnEditar.Click += BtnEditar_Click;
            Controls.Add(btnEditar);

            // boton para eliminar cliente
            btnEliminar.Text = "Eliminar";
            btnEliminar.Left = 320; btnEliminar.Top = txtNombre.Bottom + 10; btnEliminar.Width = 80;
            btnEliminar.Click += BtnEliminar_Click;
            Controls.Add(btnEliminar);

            // boton para ver historial de compras
            btnHistorial.Text = "Ver historial de compras";
            btnHistorial.Dock = DockStyle.Bottom;
            btnHistorial.Height = 35;
            btnHistorial.Click += BtnHistorial_Click;
            Controls.Add(btnHistorial);

            // cuando cambia la seleccion en la tabla, copia los datos a las cajas de texto
            dgv.SelectionChanged += (_, __) => CopiarDatosFilaSeleccionada();
        }

        // metodo para cargar los clientes desde la base de datos y mostrarlos en la tabla
        private void CargarClientes()
        {
            using var c = Db.Con();
            c.Open();
            using var da = new MySqlDataAdapter("SELECT Id, Nombre, Telefono, Email FROM Clientes ORDER BY Id;", c);
            var t = new DataTable();
            da.Fill(t);
            dgv.DataSource = t;
        }

        // metodo para obtener el cliente seleccionado en la tabla
        private Cliente? ClienteSeleccionado()
        {
            if (dgv.CurrentRow?.DataBoundItem is not DataRowView row) return null;
            return new Cliente
            {
                Id = Convert.ToInt32(row["Id"]),
                Nombre = row["Nombre"]?.ToString() ?? "",
                Telefono = row["Telefono"]?.ToString(),
                Email = row["Email"]?.ToString()
            };
        }

        // metodo para copiar los datos del cliente seleccionado a las cajas de texto
        private void CopiarDatosFilaSeleccionada()
        {
            var cliente = ClienteSeleccionado();
            if (cliente == null)
            {
                txtNombre.Text = "";
                txtTelefono.Text = "";
                txtEmail.Text = "";
                return;
            }
            txtNombre.Text = cliente.Nombre;
            txtTelefono.Text = cliente.Telefono ?? "";
            txtEmail.Text = cliente.Email ?? "";
        }

        // evento para agregar un cliente nuevo
        private void BtnAgregar_Click(object? sender, System.EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.");
                return;
            }

            var cliente = new Cliente
            {
                Nombre = txtNombre.Text.Trim(),
                Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim()
            };

            if (!string.IsNullOrWhiteSpace(cliente.Email) && !cliente.EmailValido())
            {
                MessageBox.Show("El email no es válido.");
                return;
            }

            try
            {
                using var c = Db.Con(); c.Open();
                using var cmd = new MySqlCommand(
                    "INSERT INTO Clientes(Nombre, Telefono, Email) VALUES(@n, @t, @e);", c);
                cmd.Parameters.AddWithValue("@n", cliente.Nombre);
                cmd.Parameters.AddWithValue("@t", (object?)cliente.Telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@e", (object?)cliente.Email ?? DBNull.Value);
                cmd.ExecuteNonQuery();
                CargarClientes();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error al agregar: " + ex.Message);
            }
        }

        // evento para editar el cliente seleccionado
        private void BtnEditar_Click(object? sender, System.EventArgs e)
        {
            var cliente = ClienteSeleccionado();
            if (cliente == null)
            {
                MessageBox.Show("Selecciona un cliente.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.");
                return;
            }

            cliente.Nombre = txtNombre.Text.Trim();
            cliente.Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim();
            cliente.Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim();

            if (!string.IsNullOrWhiteSpace(cliente.Email) && !cliente.EmailValido())
            {
                MessageBox.Show("El email no es válido.");
                return;
            }

            try
            {
                using var c = Db.Con(); c.Open();
                using var cmd = new MySqlCommand(
                    "UPDATE Clientes SET Nombre=@n, Telefono=@t, Email=@e WHERE Id=@id;", c);
                cmd.Parameters.AddWithValue("@n", cliente.Nombre);
                cmd.Parameters.AddWithValue("@t", (object?)cliente.Telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@e", (object?)cliente.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@id", cliente.Id);
                cmd.ExecuteNonQuery();
                CargarClientes();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error al editar: " + ex.Message);
            }
        }

        // evento para eliminar el cliente seleccionado
        private void BtnEliminar_Click(object? sender, System.EventArgs e)
        {
            var cliente = ClienteSeleccionado();
            if (cliente == null)
            {
                MessageBox.Show("Selecciona un cliente.");
                return;
            }

            if (MessageBox.Show("¿Seguro que deseas eliminar este cliente?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using var c = Db.Con(); c.Open();
                    using var cmd = new MySqlCommand("DELETE FROM Clientes WHERE Id=@id;", c);
                    cmd.Parameters.AddWithValue("@id", cliente.Id);
                    cmd.ExecuteNonQuery();
                    CargarClientes();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error al eliminar: " + ex.Message);
                }
            }
        }

        // evento para mostrar el historial de compras del cliente seleccionado
        private void BtnHistorial_Click(object? sender, System.EventArgs e)
        {
            var cliente = ClienteSeleccionado();
            if (cliente == null)
            {
                MessageBox.Show("Selecciona un cliente.");
                return;
            }
            using var frm = new FrmHistorialCompras(cliente);
            frm.ShowDialog();
        }
    }
}