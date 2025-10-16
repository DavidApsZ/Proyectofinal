using System;
using System.Windows.Forms;

namespace Proyecto_final_poo.UI
{
    public class FrmRemoverStock : Form
    {
        // propiedad para obtener la cantidad a remover
        public int CantidadRemover => (int)numCantidad.Value;
        // propiedad para obtener el motivo ingresado
        public string Motivo => txtMotivo.Text.Trim();

        // selector numerico para la cantidad
        private readonly NumericUpDown numCantidad = new();
        // caja de texto para el motivo
        private readonly TextBox txtMotivo = new();
        // boton para aceptar el cambio
        private readonly Button btnAceptar = new();
        // boton para cancelar la operacion
        private readonly Button btnCancelar = new();

        public FrmRemoverStock(int stockActual, string nombre)
        {
            // configuracion basica del formulario
            Text = "Remover Stock";
            Width = 350; Height = 200;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            // etiqueta y selector para cantidad
            var lblCantidad = new Label { Text = "Cantidad a remover:", Left = 20, Top = 20, AutoSize = true };
            numCantidad.Left = 150; numCantidad.Top = 18; numCantidad.Width = 80;
            numCantidad.Maximum = stockActual;
            numCantidad.Minimum = 1;
            numCantidad.Value = 1;

            // etiqueta y caja de texto para motivo
            var lblMotivo = new Label { Text = "Motivo:", Left = 20, Top = 60, AutoSize = true };
            txtMotivo.Left = 80; txtMotivo.Top = 57; txtMotivo.Width = 200;

            // boton aceptar
            btnAceptar.Text = "Aceptar";
            btnAceptar.Left = 60; btnAceptar.Top = 110; btnAceptar.Width = 80;
            btnAceptar.Click += (_, __) =>
            {
                if (string.IsNullOrWhiteSpace(txtMotivo.Text))
                {
                    MessageBox.Show("ingresa un motivo.");
                    return;
                }
                DialogResult = DialogResult.OK;
                Close();
            };

            // boton cancelar
            btnCancelar.Text = "Cancelar";
            btnCancelar.Left = 160; btnCancelar.Top = 110; btnCancelar.Width = 80;
            btnCancelar.Click += (_, __) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.AddRange(new Control[] { lblCantidad, numCantidad, lblMotivo, txtMotivo, btnAceptar, btnCancelar });
        }
    }
}