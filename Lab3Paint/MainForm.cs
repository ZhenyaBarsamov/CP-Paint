using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab3Paint {
    public partial class MainForm : Form {
        // Менеджер плагинов
        PluginsManager pManager;

        // Параметры рисования
        public static Color CurColor = Color.Black;
        public static int CurWidth = 3; // толщина кисти
        public static Figures CurFigure; // фигура, которая будет нарисована
        public static int StarTips = 0; // количество концов звезды

        // Количество созданных холстов
        private int newCanvasCount = 0;

        // Метод для настройки активности кнопок, зависящих от существования дочерних окон
        // toEnable = true - включить, false - выключить
        void SetButtonsEnabled(bool toEnable) {
            // Кнопка изменения размера холста доступна только тогда, когда есть активное окно
            размерХолстаToolStripMenuItem.Enabled = toEnable;
            // Кнопки сохранения доступны только тогда, когда есть активное окно
            сохранитьToolStripMenuItem.Enabled = сохранитьКакToolStripMenuItem.Enabled = toEnable;
            // Кнопки фильтров доступны только тогда, когда есть активное окно
            foreach (var mi in toolStripDDBPlugins.DropDownItems)
                ((ToolStripMenuItem)mi).Enabled = toEnable;
            // Кнопки упрвления окнами доступны только тогда, когда есть окна
            каскадомToolStripMenuItem.Enabled = toEnable;
            слеваНаправоToolStripMenuItem.Enabled = toEnable;
            сверхуВнизToolStripMenuItem.Enabled = toEnable;
            упорядочитьЗначкиToolStripMenuItem.Enabled = toEnable;
        }

        // Обработчик на событие активации (закрытия) MDI-форм для управления активностью кнопок
        void OnMdiActivate(object sender, EventArgs e) {
            bool isActiveMdiChild = !(ActiveMdiChild == null); // есть ли активное окно (а оно есть всегда, когда есть хотя бы одно окно)
            SetButtonsEnabled(isActiveMdiChild);
        }

        public MainForm() {
            InitializeComponent();
            // Обнуляем координаты мыши в статусе
            toolStripStatusLabel1.Text = String.Empty;
            // Нет фигуры
            CurFigure = Figures.None;
            // Создаём менеджера плагинов
            pManager = new PluginsManager(this, toolStrip1);
            // Загружаем плагины
            pManager.LoadFilterPlugins();
            // Подписываемся на событие активации (закрытия) MDI-формы
            MdiChildActivate += OnMdiActivate;
            // Выключаем кнопки, зависящие от существования активного окна (окон ещё нет)
            SetButtonsEnabled(false);
        }

        // Выход из приложения
        private void выходToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        // Вызов окна "О программе"
        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e) {
            AboutPaint frmAbout = new AboutPaint();
            frmAbout.ShowDialog();
        }

        // Создание нового рисунка
        private void новыйToolStripMenuItem_Click(object sender, EventArgs e) {
            Canvas frmChild = new Canvas();
            
            // Задаём окну имя
            frmChild.Text = "Холст" + (newCanvasCount > 0 ? newCanvasCount.ToString() : "");
            newCanvasCount++;

            frmChild.MdiParent = this;
            frmChild.Show();
        }

        // Изменение размеров активного холста
        private void размерХолстаToolStripMenuItem_Click(object sender, EventArgs e) {
            CanvasSize cs = new CanvasSize();
            cs.CanvasWidth = ((Canvas)ActiveMdiChild).CanvasWidth;
            cs.CanvasHeight = ((Canvas)ActiveMdiChild).CanvasHeight;
            if (cs.ShowDialog() == DialogResult.OK) {
                ((Canvas)ActiveMdiChild).CanvasWidth = cs.CanvasWidth;
                ((Canvas)ActiveMdiChild).CanvasHeight = cs.CanvasHeight;
            }
        }

        // Выбор красного цвета
        private void красныйToolStripMenuItem_Click(object sender, EventArgs e) {
            CurColor = Color.Red;
        }

        // Выбор синего цвета
        private void синийToolStripMenuItem_Click(object sender, EventArgs e) {
            CurColor = Color.Blue;
        }

        // Выбор зелёного цвета
        private void зелёныйToolStripMenuItem_Click(object sender, EventArgs e) {
            CurColor = Color.Green;
        }

        // Выбор цвета из списка цветов
        private void другойToolStripMenuItem_Click(object sender, EventArgs e) {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
                CurColor = cd.Color;
        }

        // Выбор размера кисти
        private void txtBrushSize_TextChanged(object sender, EventArgs e) {
            //if (txtBrushSize.Text == string.Empty)
            //    txtBrushSize.Text = "0";
            bool isValCorr = int.TryParse(txtBrushSize.Text, out int val);
            if (isValCorr && val >= 0)
                CurWidth = val;
            else {
                MessageBox.Show("Введите целое число!", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtBrushSize.Text = string.Empty;
            }
        }

        // Открытие файла
        private void открытьToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Windows Bitmap (*.bmp)|*.bmp| Файлы JPEG (*.jpeg, *.jpg)|*.jpeg;*.jpg|Все файлы ()*.*|*.*";
            if (dlg.ShowDialog() == DialogResult.OK) {
                Canvas frmChild = new Canvas(dlg.FileName);
                frmChild.MdiParent = this;
                frmChild.Show();
            }
        }

        // Команда "Сохранить как"
        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e) {
            ((Canvas)ActiveMdiChild).SaveAs();
        }

        // Команда "Сохранить"
        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e) {
            var canvas = (Canvas)ActiveMdiChild;
            if (canvas.FileName == null)
                canvas.SaveAs();
            else
                canvas.Save();
        }

        // Расположение окон каскадом
        private void каскадомToolStripMenuItem_Click(object sender, EventArgs e) {
            LayoutMdi(MdiLayout.Cascade);
        }

        // Расположение окон слева направо
        private void слеваНаправоToolStripMenuItem_Click(object sender, EventArgs e) {
            LayoutMdi(MdiLayout.TileVertical);
        }

        // Расположение окон сверху вниз
        private void сверхуВнизToolStripMenuItem_Click(object sender, EventArgs e) {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        // Расположение окон по порядку значков
        private void упорядочитьЗначкиToolStripMenuItem_Click(object sender, EventArgs e) {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        // Выбор количества концов звезды
        private void txtStarTips_TextChanged(object sender, EventArgs e) {
            //if (txtStarTips.Text == string.Empty)
            //    txtStarTips.Text = "0";
            bool isValCorr = int.TryParse(txtStarTips.Text, out int val);
            if (isValCorr && val >= 0)
                StarTips = val;
            else {
                MessageBox.Show("Введите целое число!", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtStarTips.Text = string.Empty;
            }
        }

        // Выбор закрашенной звезды в качестве фигуры
        private void закрашеннаяToolStripMenuItem_Click(object sender, EventArgs e) {
            CurFigure = Figures.FilledStar;
        }

        // Выбор звезды в качестве фигуры
        private void незакрашеннаяToolStripMenuItem_Click(object sender, EventArgs e) {
            CurFigure = Figures.Star;
        }

        // Выбор отсутствия фигур
        private void нетToolStripMenuItem_Click(object sender, EventArgs e) {
            CurFigure = Figures.None;
        }
    }
}
