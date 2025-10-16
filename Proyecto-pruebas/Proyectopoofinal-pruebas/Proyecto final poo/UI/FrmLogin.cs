using Proyecto_final_poo.Domain;
using Proyecto_final_poo.Security;
using System.Drawing;
using System.Windows.Forms;

namespace Proyecto_final_poo.UI
{
    public class FrmLogin : Form
    {
        private readonly TextBox txtUser = new();
        private readonly TextBox txtPass = new();
        private readonly Button btnOk = new();

        public FrmLogin()
        {
            Text = "LOGIN";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;
            ClientSize = new Size(320, 160);

            txtUser.Location = new Point(50, 25);
            txtUser.Width = 220;
#if NET6_0_OR_GREATER
            txtUser.PlaceholderText = "USUARIO";
#endif
            Controls.Add(txtUser);

            txtPass.Location = new Point(50, 60);
            txtPass.Width = 220;
            txtPass.UseSystemPasswordChar = true;
#if NET6_0_OR_GREATER
            txtPass.PlaceholderText = "CONTRASEÑA";
#endif
            Controls.Add(txtPass);

            btnOk.Text = "ENTRAR";
            btnOk.Location = new Point(50, 100);
            btnOk.Width = 220;
            btnOk.Click += (_, __) => Entrar();
            Controls.Add(btnOk);

            AcceptButton = btnOk;
        }

        private void Entrar()
        {
            var u = txtUser.Text.Trim();
            var p = txtPass.Text;

            if (string.IsNullOrWhiteSpace(u) || string.IsNullOrWhiteSpace(p))
            {
                MessageBox.Show("Ingresa usuario y contraseña.");
                return;
            }

            var usr = Auth.Login(u, p);
            if (usr == null)
            {
                MessageBox.Show("Credenciales inválidas.");
                return;
            }

            Sesion.UsuarioActual = usr;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}