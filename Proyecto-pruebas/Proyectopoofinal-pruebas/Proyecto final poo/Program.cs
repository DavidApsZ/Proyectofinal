using Proyecto_final_poo.Data;
using Proyecto_final_poo.Domain;
using Proyecto_final_poo.UI;

namespace Proyecto_final_poo
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            try { Db.Init(); }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error inicializando BD: " + ex.Message);
                return;
            }

            ApplicationConfiguration.Initialize();

            using var login = new FrmLogin();
            var res = login.ShowDialog();
            if (res != DialogResult.OK || !Sesion.EstaLogueado)
                return;

            Application.Run(new FrmMenu());
        }
    }
}
