using System;
using System.Windows.Forms;

namespace Proyecto_final_poo.UI
{
    public class FrmRemoverStock : Form
    {
        public int CantidadRemover => (int)numCantidad.Value;
        public string Motivo => txtMotivo.Text.Trim();

        private readonly NumericUpDown numCantidad = new();
        private readonly TextBox txtMotivo = new();
        private readonly Button btnAceptar = new();
        private readonly Button btnCancelar = new();

        public FrmRemoverStock(int stockActual)
        {
            Text = "Remover Stock";
            Width = 350; Height = 200;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            var lblCantidad = new Label { Text = "Cantidad a remover:", Left = 20, Top = 20, AutoSize = true };
            numCantidad.Left = 150; numCantidad.Top = 18; numCantidad.Width = 80;
            numCantidad.Maximum = stockActual;
            numCantidad.Minimum = 1;
            numCantidad.Value = 1;

            var lblMotivo = new Label { Text = "Motivo:", Left = 20, Top = 60, AutoSize = true };
            txtMotivo.Left = 80; txtMotivo.Top = 57; txtMotivo.Width = 200;

            btnAceptar.Text = "Aceptar";
            btnAceptar.Left = 60; btnAceptar.Top = 110; btnAceptar.Width = 80;
            btnAceptar.Click += (_, __) =>
            {
                if (string.IsNullOrWhiteSpace(txtMotivo.Text))
                {
                    MessageBox.Show("Ingresa un motivo.");
                    return;
                }
                DialogResult = DialogResult.OK;
                Close();
            };

            btnCancelar.Text = "Cancelar";
            btnCancelar.Left = 160; btnCancelar.Top = 110; btnCancelar.Width = 80;
            btnCancelar.Click += (_, __) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.AddRange(new Control[] { lblCantidad, numCantidad, lblMotivo, txtMotivo, btnAceptar, btnCancelar });
        }
    }
}