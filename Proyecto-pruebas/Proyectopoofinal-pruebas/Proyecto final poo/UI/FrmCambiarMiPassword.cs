using Proyecto_final_poo.Security;
using Proyecto_final_poo.Domain;

namespace Proyecto_final_poo.UI
{
    public class FrmCambiarMiPassword : Form
    {
        // caja para escribir la contrasena actual
        private readonly TextBox txtActual = new();
        // caja para escribir la nueva contrasena
        private readonly TextBox txtNueva = new();
        // caja para confirmar la nueva contrasena
        private readonly TextBox txtNueva2 = new();
        // boton para guardar los cambios de contrasena
        private readonly Button btnGuardar = new();

        public FrmCambiarMiPassword()
        {
            // configuracion basica del formulario
            Text = "Cambiar mi contraseña"; Width = 360; Height = 220;
            StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedDialog;

            // etiqueta y caja para contrasena actual
            var lbl1 = new Label { Text = "Actual", Left = 20, Top = 25, AutoSize = true };
            txtActual.Left = 100; txtActual.Top = 22; txtActual.Width = 200; txtActual.PasswordChar = '●';

            // etiqueta y caja para nueva contrasena
            var lbl2 = new Label { Text = "Nueva", Left = 20, Top = 65, AutoSize = true };
            txtNueva.Left = 100; txtNueva.Top = 62; txtNueva.Width = 200; txtNueva.PasswordChar = '●';

            // etiqueta y caja para confirmar nueva contrasena
            var lbl3 = new Label { Text = "Confirmar", Left = 20, Top = 105, AutoSize = true };
            txtNueva2.Left = 100; txtNueva2.Top = 102; txtNueva2.Width = 200; txtNueva2.PasswordChar = '●';

            // configuracion del boton guardar
            btnGuardar.Text = "Guardar"; btnGuardar.Left = 100; btnGuardar.Top = 140; btnGuardar.Width = 100;
            btnGuardar.Click += (_, __) => Guardar();

            // agrega todos los controles al formulario
            Controls.AddRange(new Control[] { lbl1, txtActual, lbl2, txtNueva, lbl3, txtNueva2, btnGuardar });
        }

        // metodo para intentar guardar el cambio de contrasena
        private void Guardar()
        {
            // valida si hay un usuario logueado
            if (!Sesion.EstaLogueado) { MessageBox.Show("No hay sesión activa."); return; }
            // valida que la nueva contrasena no este vacia y coincida con la confirmacion
            if (string.IsNullOrWhiteSpace(txtNueva.Text) || txtNueva.Text != txtNueva2.Text)
            { MessageBox.Show("Verifica la nueva contraseña."); return; }

            // llama al metodo para cambiar la contrasena, resultado en mensaje y ok
            string mensaje;
            bool ok = Auth.CambiarPassword(Sesion.UsuarioActual!.Id, txtActual.Text, txtNueva.Text, out mensaje);
            // muestra el mensaje al usuario
            MessageBox.Show(mensaje);
            // si el cambio fue exitoso cierra el formulario
            if (ok) Close();
        }
    }
}