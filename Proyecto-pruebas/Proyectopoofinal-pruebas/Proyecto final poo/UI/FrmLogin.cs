using Proyecto_final_poo.Domain;
using Proyecto_final_poo.Security;
using System.Drawing;
using System.Windows.Forms;

namespace Proyecto_final_poo.UI
{
    public class FrmLogin : Form
    {
        // caja de texto para el usuario
        private readonly TextBox txtUser = new();
        // caja de texto para la contrasena
        private readonly TextBox txtPass = new();
        // boton para iniciar sesion
        private readonly Button btnOk = new();

        public FrmLogin()
        {
            // configuracion basica del formulario
            Text = "LOGIN";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;
            ClientSize = new Size(320, 160);

            // caja de usuario
            txtUser.Location = new Point(50, 25);
            txtUser.Width = 220;
#if NET6_0_OR_GREATER
            txtUser.PlaceholderText = "USUARIO";
#endif
            Controls.Add(txtUser);

            // caja de contrasena
            txtPass.Location = new Point(50, 60);
            txtPass.Width = 220;
            txtPass.UseSystemPasswordChar = true;
#if NET6_0_OR_GREATER
            txtPass.PlaceholderText = "CONTRASEÑA";
#endif
            Controls.Add(txtPass);

            // boton para entrar
            btnOk.Text = "ENTRAR";
            btnOk.Location = new Point(50, 100);
            btnOk.Width = 220;
            btnOk.Click += (_, __) => Entrar();
            Controls.Add(btnOk);

            // permite que enter haga click en el boton
            AcceptButton = btnOk;
        }

        // metodo para intentar loguear al usuario
        private void Entrar()
        {
            var u = txtUser.Text.Trim();
            var p = txtPass.Text;

            // valida que ambos campos no esten vacios
            if (string.IsNullOrWhiteSpace(u) || string.IsNullOrWhiteSpace(p))
            {
                MessageBox.Show("ingresa usuario y contrasena.");
                return;
            }

            // intenta iniciar sesion
            var usr = Auth.Login(u, p);
            if (usr == null)
            {
                MessageBox.Show("credenciales invalidas.");
                return;
            }

            // si inicia sesion, guarda el usuario y cierra el formulario
            Sesion.UsuarioActual = usr;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}