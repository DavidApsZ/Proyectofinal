using Proyecto_final_poo.Domain;
using System;
using System.Windows.Forms;

namespace Proyecto_final_poo.UI
{
    public class FrmMenu : Form
    {
        // formulario principal de menu de la aplicacion
        public FrmMenu()
        {
            // configuracion basica del formulario
            Text = "abarrotes alcantarilla";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Width = 560; Height = 330;
            MaximizeBox = false;

            // panel para acomodar los botones
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(16),
                AutoScroll = true
            };
            Controls.Add(panel);

            // funcion auxiliar para crear un boton con texto y accion
            Button NewBtn(string text, Action onClick)
            {
                var b = new Button { Width = 150, Height = 42, Margin = new Padding(10) };
                b.Text = (text ?? string.Empty).ToUpper();
                b.Click += (_, __) => onClick();
                return b;
            }

            // array con los botones principales del menu
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

            // si el usuario es admin, agrega botones extra
            if (Sesion.UsuarioActual?.IsAdmin == true)
            {
                panel.Controls.Add(NewBtn("Configuración", () => new FrmConfiguracion().ShowDialog()));
                panel.Controls.Add(NewBtn("Empleados", () => new FrmEmpleados().ShowDialog()));
                panel.Controls.Add(NewBtn("Usuarios", () => new FrmUsuarios().ShowDialog()));
            }

            // agrega los botones principales al panel
            foreach (var (txt, action) in botones)
                panel.Controls.Add(NewBtn(txt, action));

            // boton para salir de la aplicacion
            panel.Controls.Add(NewBtn("Salir", Close));
        }
    }
}