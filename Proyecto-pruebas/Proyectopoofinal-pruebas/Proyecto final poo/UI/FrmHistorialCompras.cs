using Proyecto_final_poo.Data;
using Proyecto_final_poo.Domain;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

// formulario para mostrar el historial de compras de un cliente
public class FrmHistorialCompras : Form
{
    // etiqueta para mostrar el nombre del cliente
    private readonly Label lblCliente = new();
    // tabla para mostrar las ventas del cliente
    private readonly DataGridView dgvVentas = new();
    // tabla para mostrar los productos de la venta seleccionada
    private readonly DataGridView dgvProductos = new();
    // cliente del cual se muestra el historial
    private readonly Cliente cliente;

    // constructor, recibe el cliente a mostrar
    public FrmHistorialCompras(Cliente cliente)
    {
        this.cliente = cliente;

        // configuracion basica del formulario
        Text = "Historial de compras";
        Width = 600; Height = 480;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        // configuracion de la etiqueta del cliente
        lblCliente.Text = $"Cliente: {cliente.Nombre}";
        lblCliente.Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold);
        lblCliente.Dock = DockStyle.Top;
        lblCliente.Height = 36;
        Controls.Add(lblCliente);

        // configuracion de la tabla de ventas
        dgvVentas.Top = lblCliente.Bottom;
        dgvVentas.Left = 0;
        dgvVentas.Width = 584;
        dgvVentas.Height = 170;
        dgvVentas.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        dgvVentas.ReadOnly = true;
        dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvVentas.MultiSelect = false;
        Controls.Add(dgvVentas);

        // configuracion de la tabla de productos
        dgvProductos.Top = dgvVentas.Bottom + 4;
        dgvProductos.Left = 0;
        dgvProductos.Width = 584;
        dgvProductos.Height = 200;
        dgvProductos.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        dgvProductos.ReadOnly = true;
        dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvProductos.MultiSelect = false;
        Controls.Add(dgvProductos);

        // al cargar el formulario, carga las ventas del cliente
        Load += (_, __) => CargarVentas();
        // al cambiar seleccion de venta, carga los productos de esa venta
        dgvVentas.SelectionChanged += DgvVentas_SelectionChanged;
    }

    // metodo para cargar las ventas del cliente y mostrarlas en la tabla
    private void CargarVentas()
    {
        var ventasTable = new DataTable();
        ventasTable.Columns.Add("IDVenta", typeof(int));
        ventasTable.Columns.Add("Fecha", typeof(string));
        ventasTable.Columns.Add("Total", typeof(decimal));
        ventasTable.Columns.Add("MetodoPago", typeof(string));

        using var c = Db.Con(); c.Open();
        using var da = new MySqlDataAdapter(
            @"SELECT v.Id AS IDVenta, v.Fecha, v.Total, v.MetodoPago
              FROM Ventas v
              WHERE v.ClienteId = @cliId
              ORDER BY v.Fecha DESC", c);
        da.SelectCommand.Parameters.AddWithValue("@cliId", cliente.Id);

        var t = new DataTable();
        da.Fill(t);

        foreach (DataRow row in t.Rows)
        {
            ventasTable.Rows.Add(
                row["IDVenta"],
                Convert.ToDateTime(row["Fecha"]).ToString("dd/MM/yyyy HH:mm"),
                row["Total"],
                row["MetodoPago"]
            );
        }

        dgvVentas.DataSource = ventasTable;

        // si hay al menos una venta, selecciona la primera y carga sus productos
        if (dgvVentas.Rows.Count > 0)
        {
            dgvVentas.Rows[0].Selected = true;
            var idVenta = Convert.ToInt32(dgvVentas.Rows[0].Cells["IDVenta"].Value);
            CargarProductosDeVenta(idVenta);
        }
        else
        {
            dgvProductos.DataSource = null;
        }
    }

    // evento al cambiar la venta seleccionada en la tabla
    private void DgvVentas_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvVentas.CurrentRow?.Cells["IDVenta"].Value != null)
        {
            int ventaId = Convert.ToInt32(dgvVentas.CurrentRow.Cells["IDVenta"].Value);
            CargarProductosDeVenta(ventaId);
        }
        else
        {
            dgvProductos.DataSource = null;
        }
    }

    // metodo para cargar los productos de una venta y mostrarlos en la tabla
    private void CargarProductosDeVenta(int ventaId)
    {
        var productosTable = new DataTable();
        productosTable.Columns.Add("Producto", typeof(string));
        productosTable.Columns.Add("PrecioUnitario", typeof(string));
        productosTable.Columns.Add("Cantidad", typeof(int));
        productosTable.Columns.Add("Subtotal", typeof(string));

        using var c = Db.Con(); c.Open();

        decimal totalVenta = 0, totalSinComision = 0, comisionPorc = 0;
        // obtiene el total de la venta
        using (var cmd = new MySqlCommand("SELECT Total FROM Ventas WHERE Id=@id;", c))
        {
            cmd.Parameters.AddWithValue("@id", ventaId);
            var val = cmd.ExecuteScalar();
            totalVenta = (val != null && val != DBNull.Value) ? Convert.ToDecimal(val) : 0;
        }
        // obtiene el total sin comision sumando los productos
        using (var cmd = new MySqlCommand("SELECT SUM(PrecioUnitario * Cantidad) FROM Detalles WHERE VentaId=@id;", c))
        {
            cmd.Parameters.AddWithValue("@id", ventaId);
            var val = cmd.ExecuteScalar();
            totalSinComision = (val != null && val != DBNull.Value) ? Convert.ToDecimal(val) : totalVenta;
        }
        // obtiene el porcentaje de comision configurado
        using (var cmdCfg = new MySqlCommand("SELECT ValorDecimal FROM Configuracion WHERE Clave='ComisionTarjeta';", c))
        {
            var val = cmdCfg.ExecuteScalar();
            comisionPorc = (val != null && val != DBNull.Value) ? Convert.ToDecimal(val) : 0.035m;
        }
        decimal comision = totalVenta - totalSinComision;

        // obtiene los productos de la venta
        using var da = new MySqlDataAdapter(
            @"SELECT NombreProducto AS Producto, PrecioUnitario, Cantidad, 
                     (PrecioUnitario * Cantidad) AS Subtotal
              FROM Detalles
              WHERE VentaId = @ventaId", c);
        da.SelectCommand.Parameters.AddWithValue("@ventaId", ventaId);

        var t = new DataTable();
        da.Fill(t);

        foreach (DataRow row in t.Rows)
        {
            var precio = Convert.ToDecimal(row["PrecioUnitario"]);
            var cantidad = Convert.ToInt32(row["Cantidad"]);
            var subtotal = Convert.ToDecimal(row["Subtotal"]);

            // si la venta tiene comision, suma la comision al precio y subtotal
            if (comision > 0.01m)
            {
                var precioComision = precio + Math.Round(precio * comisionPorc, 2);
                var subtotalComision = subtotal + Math.Round(subtotal * comisionPorc, 2);
                productosTable.Rows.Add(
                    row["Producto"],
                    precioComision.ToString("0.00"),
                    cantidad,
                    subtotalComision.ToString("0.00")
                );
            }
            else
            {
                productosTable.Rows.Add(
                    row["Producto"],
                    precio.ToString("0.00"),
                    cantidad,
                    subtotal.ToString("0.00")
                );
            }
        }

        dgvProductos.DataSource = productosTable;
    }
}