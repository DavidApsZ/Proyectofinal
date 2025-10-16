using MySql.Data.MySqlClient;
using Proyecto_final_poo.Data;
using System.Data;

namespace Proyecto_final_poo.UI
{
    public class FrmReporteVentas : Form
    {
        // selector de fecha desde
        private readonly DateTimePicker dpDesde = new();
        // selector de fecha hasta
        private readonly DateTimePicker dpHasta = new();
        // boton para buscar ventas en el rango
        private readonly Button btnBuscar = new();
        // tabla para mostrar las ventas encontradas
        private readonly DataGridView dgvVentas = new();
        // tabla para mostrar el detalle de la venta seleccionada
        private readonly DataGridView dgvDetalle = new();
        // etiqueta para mostrar el total vendido y total de productos
        private readonly Label lblResumen = new();

        public FrmReporteVentas()
        {
            // configuracion basica del formulario
            Text = "Reporte de Ventas";
            Width = 950; Height = 600;
            StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedDialog;

            // campos para elegir el rango de fechas
            var lblD = new Label { Text = "Desde:", Left = 20, Top = 18, AutoSize = true };
            dpDesde.Left = 70; dpDesde.Top = 14; dpDesde.Width = 200; dpDesde.Value = DateTime.Today.AddDays(-7);

            var lblH = new Label { Text = "Hasta:", Left = 290, Top = 18, AutoSize = true };
            dpHasta.Left = 340; dpHasta.Top = 14; dpHasta.Width = 200; dpHasta.Value = DateTime.Today;

            // boton para buscar
            btnBuscar.Text = "Buscar"; btnBuscar.Left = 560; btnBuscar.Top = 12; btnBuscar.Width = 100;
            btnBuscar.Click += (_, __) => Buscar();

            // tabla de ventas
            dgvVentas.Left = 20; dgvVentas.Top = 50; dgvVentas.Width = 900; dgvVentas.Height = 200;
            dgvVentas.ReadOnly = true; dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVentas.MultiSelect = false;

            // tabla de detalle de venta
            dgvDetalle.Left = 20; dgvDetalle.Top = 270; dgvDetalle.Width = 900; dgvDetalle.Height = 200;
            dgvDetalle.ReadOnly = true; dgvDetalle.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // etiqueta resumen de totales
            lblResumen.Left = 20; lblResumen.Top = 480; lblResumen.Width = 900; lblResumen.Text = "total vendido: $0.00 | total productos vendidos: 0";

            Controls.AddRange(new Control[] { lblD, dpDesde, lblH, dpHasta, btnBuscar, dgvVentas, dgvDetalle, lblResumen });

            // al cambiar la seleccion de venta, muestra el detalle
            dgvVentas.SelectionChanged += (_, __) => MostrarDetalle();
        }

        // busca las ventas entre las fechas seleccionadas y muestra totales
        private void Buscar()
        {
            using var c = Db.Con(); c.Open();

            string sqlVentas = @"
                SELECT v.Id AS IDVenta, v.Fecha, v.Total AS TotalVenta, 
                       c.Nombre AS Cliente, v.MetodoPago, e.Nombre AS Empleado
                FROM Ventas v
                LEFT JOIN Clientes c ON c.Id = v.ClienteId
                LEFT JOIN Empleados e ON e.Id = v.EmpleadoId
                WHERE v.Fecha >= @d1 AND v.Fecha < DATE_ADD(@d2, INTERVAL 1 DAY)
                ORDER BY v.Fecha DESC, v.Id DESC;";

            using var daVentas = new MySqlDataAdapter(sqlVentas, c);
            daVentas.SelectCommand.Parameters.AddWithValue("@d1", dpDesde.Value.Date);
            daVentas.SelectCommand.Parameters.AddWithValue("@d2", dpHasta.Value.Date);

            var tVentas = new DataTable();
            daVentas.Fill(tVentas);
            dgvVentas.DataSource = tVentas;

            if (dgvVentas.Columns.Contains("IDVenta")) dgvVentas.Columns["IDVenta"].HeaderText = "IDVenta";
            if (dgvVentas.Columns.Contains("Fecha")) dgvVentas.Columns["Fecha"].HeaderText = "Fecha";
            if (dgvVentas.Columns.Contains("TotalVenta")) dgvVentas.Columns["TotalVenta"].HeaderText = "TotalVenta";
            if (dgvVentas.Columns.Contains("Cliente")) dgvVentas.Columns["Cliente"].HeaderText = "Cliente";
            if (dgvVentas.Columns.Contains("MetodoPago")) dgvVentas.Columns["MetodoPago"].HeaderText = "Método de pago";
            if (dgvVentas.Columns.Contains("Empleado")) dgvVentas.Columns["Empleado"].HeaderText = "Empleado";

            decimal totalDinero = 0;
            int totalProductos = 0;

            string sqlTotales = @"
                SELECT SUM(v.Total) AS TotalDinero, 
                       (SELECT SUM(d.Cantidad) FROM Detalles d JOIN Ventas v2 ON d.VentaId = v2.Id WHERE v2.Fecha >= @d1 AND v2.Fecha < DATE_ADD(@d2, INTERVAL 1 DAY)) AS TotalProductos
                FROM Ventas v
                WHERE v.Fecha >= @d1 AND v.Fecha < DATE_ADD(@d2, INTERVAL 1 DAY);";

            using var cmdTotales = new MySqlCommand(sqlTotales, c);
            cmdTotales.Parameters.AddWithValue("@d1", dpDesde.Value.Date);
            cmdTotales.Parameters.AddWithValue("@d2", dpHasta.Value.Date);

            using var rd = cmdTotales.ExecuteReader();
            if (rd.Read())
            {
                totalDinero = rd.IsDBNull(0) ? 0 : rd.GetDecimal(0);
                totalProductos = rd.IsDBNull(1) ? 0 : rd.GetInt32(1);
            }
            rd.Close();

            lblResumen.Text = $"total vendido: ${totalDinero:0.00} | total productos vendidos: {totalProductos}";
            MostrarDetalle();
        }

        // muestra el detalle de la venta seleccionada
        private void MostrarDetalle()
        {
            if (dgvVentas.CurrentRow == null)
            {
                dgvDetalle.DataSource = null;
                return;
            }

            var ventaId = dgvVentas.CurrentRow.Cells["IDVenta"].Value;
            using var c = Db.Con(); c.Open();

            decimal totalVenta = 0, totalSinComision = 0, comisionPorc = 0;
            // obtiene el total de la venta
            using (var cmd = new MySqlCommand("SELECT Total FROM Ventas WHERE Id=@id;", c))
            {
                cmd.Parameters.AddWithValue("@id", ventaId);
                var val = cmd.ExecuteScalar();
                totalVenta = (val != null && val != DBNull.Value) ? Convert.ToDecimal(val) : 0;
            }
            // obtiene el total sin comision
            using (var cmd = new MySqlCommand("SELECT SUM(PrecioUnitario * Cantidad) FROM Detalles WHERE VentaId=@id;", c))
            {
                cmd.Parameters.AddWithValue("@id", ventaId);
                var val = cmd.ExecuteScalar();
                totalSinComision = (val != null && val != DBNull.Value) ? Convert.ToDecimal(val) : totalVenta;
            }
            // obtiene el porcentaje de comision
            using (var cmdCfg = new MySqlCommand("SELECT ValorDecimal FROM Configuracion WHERE Clave='ComisionTarjeta';", c))
            {
                var val = cmdCfg.ExecuteScalar();
                comisionPorc = (val != null && val != DBNull.Value) ? Convert.ToDecimal(val) : 0.035m;
            }
            decimal comision = totalVenta - totalSinComision;

            string sqlDetalle = @"
                SELECT d.NombreProducto AS Producto, d.Cantidad, d.PrecioUnitario, (d.Cantidad * d.PrecioUnitario) AS Importe
                FROM Detalles d
                WHERE d.VentaId = @ventaId
                ORDER BY Producto;";

            using var daDetalle = new MySqlDataAdapter(sqlDetalle, c);
            daDetalle.SelectCommand.Parameters.AddWithValue("@ventaId", ventaId);

            var tDetalle = new DataTable();
            daDetalle.Fill(tDetalle);

            // ajusta los precios y subtotales si la venta tuvo comision
            if (comision > 0.01m)
            {
                foreach (DataRow row in tDetalle.Rows)
                {
                    var precio = Convert.ToDecimal(row["PrecioUnitario"]);
                    var importe = Convert.ToDecimal(row["Importe"]);
                    var precioComision = precio + Math.Round(precio * comisionPorc, 2);
                    var importeComision = importe + Math.Round(importe * comisionPorc, 2);
                    row["PrecioUnitario"] = precioComision.ToString("0.00");
                    row["Importe"] = importeComision.ToString("0.00");
                }
            }
            else
            {
                foreach (DataRow row in tDetalle.Rows)
                {
                    row["PrecioUnitario"] = Convert.ToDecimal(row["PrecioUnitario"]).ToString("0.00");
                    row["Importe"] = Convert.ToDecimal(row["Importe"]).ToString("0.00");
                }
            }

            dgvDetalle.DataSource = tDetalle;

            if (dgvDetalle.Columns.Contains("Producto")) dgvDetalle.Columns["Producto"].HeaderText = "Producto vendido";
            if (dgvDetalle.Columns.Contains("Cantidad")) dgvDetalle.Columns["Cantidad"].HeaderText = "Cantidad";
            if (dgvDetalle.Columns.Contains("PrecioUnitario")) dgvDetalle.Columns["PrecioUnitario"].HeaderText = "Precio unitario";
            if (dgvDetalle.Columns.Contains("Importe")) dgvDetalle.Columns["Importe"].HeaderText = "Importe";
        }
    }
}