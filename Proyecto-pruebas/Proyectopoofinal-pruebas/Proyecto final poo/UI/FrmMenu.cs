using Proyecto_final_poo.Domain;
using System;
using System.Windows.Forms;

namespace Proyecto_final_poo.UI
{
    public class FrmMenu : Form
    {
        public FrmMenu()
        {
            Text = "ABARROTES ALCANTARILLA";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Width = 560; Height = 330;
            MaximizeBox = false;

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(16),
                AutoScroll = true
            };
            Controls.Add(panel);

            Button NewBtn(string text, Action onClick)
            {
                var b = new Button { Width = 150, Height = 42, Margin = new Padding(10) };
                b.Text = (text ?? string.Empty).ToUpper();
                b.Click += (_, __) => onClick();
                return b;
            }

            var botones = new[]
            {
                ("Productos", (Action)(() => new FrmProductos().ShowDialog())),
                ("Categorías", () => new FrmCategorias().ShowDialog()),
                ("Clientes", () => new FrmClientes().ShowDialog()),
                ("Venta", () => new FrmVenta().ShowDialog()),
                ("Reporte Ventas", () => new FrmReporteVentas().ShowDialog()),
                ("Inventario", () => new FrmInventario().ShowDialog()),
                ("Proveedores", () => new FrmProveedores().ShowDialog()),
                ("Mi contraseña", () => { using var f = new FrmCambiarMiPassword(); f.ShowDialog(); })
            };

            if (Sesion.UsuarioActual?.IsAdmin == true)
            {
                panel.Controls.Add(NewBtn("Configuración", () => new FrmConfiguracion().ShowDialog()));
                panel.Controls.Add(NewBtn("Empleados", () => new FrmEmpleados().ShowDialog()));
                panel.Controls.Add(NewBtn("Usuarios", () => new FrmUsuarios().ShowDialog()));
            }

            foreach (var (txt, action) in botones)
                panel.Controls.Add(NewBtn(txt, action));

            panel.Controls.Add(NewBtn("Salir", Close));
        }
    }
}