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
    public partial class ProgressIndicator : Form {
        // Имя описываемого процесса
        string ProcName { get; set; }

        public ProgressIndicator() {
            InitializeComponent();
        }

        public ProgressIndicator(string procName) {
            ProcName = procName;
            Text = procName + " - 0%";
        }

        /// <summary>
        /// Задать значение прогресса
        /// </summary>
        /// <param name="percent">Процент выполнения</param>
        public void SetProgressPercent(int percent) {
            progressBar.Value = percent;
            Text = $"{ProcName} - {percent}%";
            if (percent == 100) {
                DialogResult = DialogResult.OK;
                Close();
            }
        }


        private void ProgressIndicator_FormClosing(object sender, FormClosingEventArgs e) {
            if (e.CloseReason == CloseReason.UserClosing) {
                var dRes = MessageBox.Show("Вы действительно хотите отменить выполнение действия?", "Отмена выполнения",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (dRes == DialogResult.Yes) {
                    DialogResult = DialogResult.Cancel;
                    e.Cancel = false;
                }
                else
                    e.Cancel = true;
            }
        }
    }
}
