using MySql.Data.MySqlClient;
using Proyecto_final_poo.Data;
using Proyecto_final_poo.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Proyecto_final_poo.UI
{
    public class FrmVenta : Form
    {
        private readonly ComboBox cboCliente = new();
        private readonly ComboBox cboProducto = new();
        private readonly NumericUpDown numCant = new();
        private readonly Button btnAdd = new();
        private readonly DataGridView dgv = new();
        private readonly Label lblTotal = new();
        private readonly RadioButton rbEfectivo = new();
        private readonly RadioButton rbTarjeta = new();
        private readonly Button btnConfirmar = new();
        private readonly ComboBox cboEmpleado = new();

        private sealed class Item
        {
            public int ProductoId { get; set; }
            public string Nombre { get; set; } = "";
            public decimal Precio { get; set; }
            public int Cantidad { get; set; }
            public decimal Subtotal => Precio * Cantidad;
        }
        private readonly List<Item> carrito = new();

        public FrmVenta()
        {
            Text = "Venta";
            Width = 700; Height = 520;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            SuspendLayout();

            var lblCliente = new Label { Text = "Cliente", Left = 20, Top = 20, AutoSize = true };
            cboCliente.Left = 90; cboCliente.Top = 16; cboCliente.Width = 300;
            cboCliente.DropDownStyle = ComboBoxStyle.DropDownList;
            Controls.Add(lblCliente);
            Controls.Add(cboCliente);

            var lblEmpleado = new Label { Text = "Empleado", Left = 20, Top = 60, AutoSize = true };
            cboEmpleado.Left = 90; cboEmpleado.Top = 56; cboEmpleado.Width = 300;
            cboEmpleado.DropDownStyle = ComboBoxStyle.DropDownList;
            Controls.Add(lblEmpleado);
            Controls.Add(cboEmpleado);

            var lblP = new Label { Text = "Producto", Left = 20, Top = 100, AutoSize = true };
            cboProducto.Left = 90; cboProducto.Top = 96; cboProducto.Width = 300;
            cboProducto.DropDownStyle = ComboBoxStyle.DropDownList;
            Controls.Add(lblP);
            Controls.Add(cboProducto);

            var lblC = new Label { Text = "Cant.", Left = 400, Top = 100, AutoSize = true };
            numCant.Left = 440; numCant.Top = 96; numCant.Width = 80;
            numCant.Minimum = 1; numCant.Maximum = 1000; numCant.Value = 1;
            Controls.Add(lblC);
            Controls.Add(numCant);

            btnAdd.Text = "Añadir"; btnAdd.Left = 530; btnAdd.Top = 94; btnAdd.Width = 80;
            btnAdd.Click += (_, __) => AgregarAlCarrito();
            Controls.Add(btnAdd);

            dgv.Left = 20; dgv.Top = 140; dgv.Width = 640; dgv.Height = 200;
            dgv.ReadOnly = true; dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Controls.Add(dgv);

            lblTotal.Left = 20; lblTotal.Top = 360; lblTotal.Width = 300; lblTotal.Text = "Total: $0.00";
            Controls.Add(lblTotal);

            rbEfectivo.AutoSize = true; rbEfectivo.Text = "Efectivo";
            rbEfectivo.Left = 340; rbEfectivo.Top = 358; rbEfectivo.Checked = true;
            Controls.Add(rbEfectivo);

            rbTarjeta.AutoSize = true; rbTarjeta.Text = "Tarjeta";
            rbTarjeta.Left = 430; rbTarjeta.Top = 358;
            rbTarjeta.CheckedChanged += (_, __) => RefrescarCarrito(); 
            Controls.Add(rbTarjeta);

            btnConfirmar.Text = "Confirmar venta";
            btnConfirmar.Left = 20; btnConfirmar.Top = 400; btnConfirmar.Width = 160;
            btnConfirmar.Click += (_, __) => ConfirmarVenta();
            Controls.Add(btnConfirmar);

            Load += (_, __) => { CargarClientes(); CargarEmpleados(); CargarProductos(); };

            ResumeLayout(false);
        }

        private void CargarClientes()
        {
            using var c = Db.Con(); c.Open();
            using var da = new MySqlDataAdapter("SELECT Id, Nombre, Telefono, Email FROM Clientes ORDER BY Nombre;", c);
            var t = new DataTable(); da.Fill(t);
            cboCliente.DataSource = t;
            cboCliente.ValueMember = "Id";
            cboCliente.DisplayMember = "Nombre";
        }

        private void CargarEmpleados()
        {
            using var c = Db.Con(); c.Open();
            using var da = new MySqlDataAdapter("SELECT Id, Nombre FROM Empleados ORDER BY Nombre;", c);
            var t = new DataTable(); da.Fill(t);
            cboEmpleado.DataSource = t;
            cboEmpleado.ValueMember = "Id";
            cboEmpleado.DisplayMember = "Nombre";
        }

        private void CargarProductos()
        {
            using var c = Db.Con(); c.Open();
            using var da = new MySqlDataAdapter("SELECT Id, Nombre, Precio, Stock FROM Productos ORDER BY Nombre;", c);
            var t = new DataTable(); da.Fill(t);
            cboProducto.DataSource = t;
            cboProducto.ValueMember = "Id";
            cboProducto.DisplayMember = "Nombre";
        }

        private void AgregarAlCarrito()
        {
            if (cboProducto.SelectedItem is not DataRowView row) return;

            var id = Convert.ToInt32(row["Id"]);
            var nombre = Convert.ToString(row["Nombre"])!;
            var precio = Convert.ToDecimal(row["Precio"]);
            var stock = Convert.ToInt32(row["Stock"]);
            var cant = (int)numCant.Value;

            if (cant <= 0) { MessageBox.Show("Cantidad inválida."); return; }

            var yaEnCarrito = carrito.Where(x => x.ProductoId == id).Sum(x => x.Cantidad);

            if (cant + yaEnCarrito > stock)
            {
                MessageBox.Show($"Stock insuficiente. Disponible: {stock - yaEnCarrito}.");
                return;
            }

            carrito.Add(new Item { ProductoId = id, Nombre = nombre, Precio = precio, Cantidad = cant });
            RefrescarCarrito();
        }

        private void RefrescarCarrito()
        {
            decimal total = carrito.Sum(x => x.Subtotal);
            decimal comision = 0;
            decimal porcComision = 0;
            if (rbTarjeta.Checked)
            {
                using var c = Db.Con(); c.Open();
                using var cmdCfg = new MySqlCommand("SELECT ValorDecimal FROM Configuracion WHERE Clave='ComisionTarjeta';", c);
                var val = cmdCfg.ExecuteScalar();
                porcComision = (val != null && val != DBNull.Value) ? Convert.ToDecimal(val) : 0.035m;
                comision = Math.Round(total * porcComision, 2);
            }
            dgv.DataSource = null;
            dgv.DataSource = carrito.Select(x => new
            {
                x.ProductoId,
                x.Nombre,
                Precio = x.Precio.ToString("0.00"),
                x.Cantidad,
                Subtotal = rbTarjeta.Checked
                    ? (x.Subtotal + Math.Round(x.Subtotal * porcComision, 2)).ToString("0.00")
                    : x.Subtotal.ToString("0.00")
            }).ToList();

            lblTotal.Text = $"Total: ${(total + comision):0.00}";
        }

        private Cliente? ObtenerClienteSeleccionado()
        {
            if (cboCliente.SelectedItem is DataRowView row)
            {
                return new Cliente
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Nombre = Convert.ToString(row["Nombre"]) ?? "",
                    Telefono = Convert.ToString(row["Telefono"]) ?? "",
                    Email = Convert.ToString(row["Email"]) ?? ""
                };
            }
            return null;
        }

        private void ConfirmarVenta()
        {
            if (carrito.Count == 0) { MessageBox.Show("El carrito está vacío."); return; }

            var cliente = ObtenerClienteSeleccionado();
            if (cliente == null)
            {
                MessageBox.Show("Selecciona un cliente válido.");
                return;
            }

            if (cboEmpleado.SelectedItem == null)
            {
                MessageBox.Show("Selecciona un empleado.");
                return;
            }
            var empleadoId = Convert.ToInt32(cboEmpleado.SelectedValue);

            Pago pago = rbEfectivo.Checked ? new PagoEfectivo() : new PagoTarjeta();

            using var c = Db.Con(); c.Open();

            if (pago is PagoTarjeta pt)
            {
                using var cmdCfg = new MySqlCommand("SELECT ValorDecimal FROM Configuracion WHERE Clave='ComisionTarjeta';", c);
                var val = cmdCfg.ExecuteScalar();
                if (val != null && val != DBNull.Value)
                    pt.ComisionPorc = Convert.ToDecimal(val);
            }

            var total = carrito.Sum(x => x.Subtotal);
            var totalCobrado = pago.Procesar(total);

            decimal comision = (pago is PagoTarjeta pt2) ? Math.Round(total * pt2.ComisionPorc, 2) : 0;

            using var tx = c.BeginTransaction();
            try
            {
                int ventaId;
                using (var cmdV = new MySqlCommand(
                    "INSERT INTO Ventas(ClienteId, Fecha, Total, MetodoPago, EmpleadoId) VALUES(@cli, NOW(), @tot, @metodo, @emp); SELECT LAST_INSERT_ID();", c, tx))
                {
                    cmdV.Parameters.AddWithValue("@cli", cliente.Id);
                    cmdV.Parameters.AddWithValue("@tot", totalCobrado);
                    cmdV.Parameters.AddWithValue("@metodo", rbTarjeta.Checked ? "Tarjeta" : "Efectivo");
                    cmdV.Parameters.AddWithValue("@emp", empleadoId);
                    ventaId = Convert.ToInt32(cmdV.ExecuteScalar());
                }

                foreach (var it in carrito)
                {
                    using (var cmdD = new MySqlCommand(
                        @"INSERT INTO Detalles(VentaId, ProductoId, NombreProducto, PrecioUnitario, Cantidad)
                          VALUES(@v,@p,@n,@pr,@c);", c, tx))
                    {
                        cmdD.Parameters.AddWithValue("@v", ventaId);
                        cmdD.Parameters.AddWithValue("@p", it.ProductoId);
                        cmdD.Parameters.AddWithValue("@n", it.Nombre);
                        cmdD.Parameters.AddWithValue("@pr", it.Precio);
                        cmdD.Parameters.AddWithValue("@c", it.Cantidad);
                        cmdD.ExecuteNonQuery();
                    }
                }

                tx.Commit();

                MessageBox.Show($"{pago.GenerarRecibo(totalCobrado)}\nVenta registrada (Total: ${totalCobrado:0.00}).");
                carrito.Clear();
                CargarProductos();
                RefrescarCarrito();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                MessageBox.Show("Error al confirmar venta: " + ex.Message);
            }
        }
    }
}