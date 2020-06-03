using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab3Paint {
    public partial class CanvasSize : Form {
        // Свойства 
        /// <summary>
        /// Поле, задающее ширину холста
        /// </summary>
        public int CanvasWidth {
            get {
                bool isCorr = int.TryParse(tbWidth.Text, out int width);
                return isCorr ? width : -1;
            }
            set {
                tbWidth.Text = value.ToString();
            }
        }
        /// <summary>
        /// Поле, задающее высоту холста
        /// </summary>
        public int CanvasHeight {
            get {
                bool isCorr = int.TryParse(tbHeight.Text, out int height);
                return isCorr ? height : -1;
            }
            set {
                tbHeight.Text = value.ToString();
            }
        }

        public CanvasSize() {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e) {
            bool isWidthCorr = int.TryParse(tbWidth.Text, out int width);
            bool isHeightCorr = int.TryParse(tbHeight.Text, out int height);
            if (!isWidthCorr || !isHeightCorr) {
                MessageBox.Show("Введите положительное число!", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                DialogResult = DialogResult.None; // окно продолжает работу
                return;
            }
        }
    }
}
