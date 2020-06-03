using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace Lab3Paint {
    /// <summary>
    /// Форма для представления холста
    /// </summary>
    public partial class Canvas : Form {
        /// <summary>
        /// Путь к файлу картинки, изменяемой на данном холсте
        /// </summary>
        public string FileName { get; set; }

        // Картинка до последнего изменения
        private Bitmap prevBmp;

        // Предыдущие координаты мыши
        private int oldX, oldY;
        // Область рисования в виде битовой карты
        private Bitmap bmp;

        // Получение текущего изображения (для фильтра)
        public Bitmap GetCurImage() {
            return bmp;
        }

        // Замена текущего изображения изображением с наложенным фильтром
        public void SetFilteredImage(Bitmap fBmp) {
            prevBmp = bmp;
            pictureBox1.Width = fBmp.Width;
            pictureBox1.Height = fBmp.Height;
            bmp = fBmp;
            pictureBox1.Image = bmp;
            Invalidate();
        }


        // Свойства размеров формы
        /// <summary>
        /// Ширина холста
        /// </summary>
        public int CanvasWidth {
            get {
                return pictureBox1.Width;
            }
            set {
                pictureBox1.Width = value;
                // Создаём новую битовую карту заданного размера
                Bitmap tbmp = new Bitmap(value, pictureBox1.Height);
                Graphics g = Graphics.FromImage(tbmp);
                g.Clear(Color.White); // очищаем область и заливаем белым цветом
                g.DrawImage(bmp, new Point(0, 0)); // с начала координат рисуем старое изображение
                // Заменяем старую битовую карту новой
                bmp = tbmp;
                pictureBox1.Image = bmp;
            }
        }

        /// <summary>
        /// Высота холста
        /// </summary>
        public int CanvasHeight {
            get {
                return pictureBox1.Height;
            }
            set {
                pictureBox1.Height = value;
                // Создаём новую битовую карту заданного размера
                Bitmap tbmp = new Bitmap(pictureBox1.Width, value);
                Graphics g = Graphics.FromImage(tbmp);
                g.Clear(Color.White); // очищаем область и заливаем белым цветом
                g.DrawImage(bmp, new Point(0, 0)); // с начала координат рисуем старое изображение
                // Заменяем старую битовую карту новой
                bmp = tbmp;
                pictureBox1.Image = bmp;
            }
        }


        /// <summary>
        /// Конструктор для создания пустого холста
        /// </summary>
        public Canvas() {
            InitializeComponent();
            // Создание битовой карты
            bmp = new Bitmap(ClientSize.Width, ClientSize.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White); // очищаем область и заливаем белым цветом
            pictureBox1.Image = bmp;
            // Пустой путь к файлу
            FileName = null;
            // Предыдущего изображения нет
            prevBmp = null;
        }

        /// <summary>
        /// Констркутор для создания холста с заданным изображением
        /// </summary>
        /// <param name="fileName">Путь к файлу изображения</param>
        public Canvas(string fileName) {
            InitializeComponent();
            var tbmp = new Bitmap(fileName); // открываем из файла
            bmp = new Bitmap(tbmp); // открытое изображение переписываем в наше
            tbmp.Dispose(); // освобождаем открытое изображение (чтоб закрыть файл)

            Graphics g = Graphics.FromImage(bmp);
            pictureBox1.Width = bmp.Width;
            pictureBox1.Height = bmp.Height;
            pictureBox1.Image = bmp;
            // Сохраняем путь к файлу
            FileName = fileName;
            // Имя изображения выводим в заголовок формы
            Text = FileName.Split('\\').Last();
            // Предыдущего изображения нет
            prevBmp = null;
        }

        // Возникает при наведении указателя мыши на компонент (пишем координаты мыши в статус, рисуем)
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e) {
            // Пишем координаты мыши в статус
            var p = MdiParent as MainForm;
            p.toolStripStatusLabel1.Text = $"X:{e.X} Y:{e.Y}";

            if (e.Button == MouseButtons.Left) {
                if (MainForm.CurFigure == Figures.None) {
                    // Получаем поверхность рисования из картинки
                    Graphics g = Graphics.FromImage(bmp);
                    // Создаём перо и настраиваем так, чтоб линия не распадалась на пиксели
                    var pen = new Pen(MainForm.CurColor, MainForm.CurWidth);
                    pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                    pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

                    g.DrawLine(pen,
                        oldX, oldY, e.X, e.Y);
                    oldX = e.X;
                    oldY = e.Y;

                    // После рисования инвалидируем рисунок для отображения изменений
                    pictureBox1.Invalidate();
                }
                else {
                    pictureBox1.Refresh(); // затираем предыдущие несохранённые звёзды, если есть, принудительно перерисовывая всё
                    // Получаем поверхность рисования из картинки
                    Graphics g = pictureBox1.CreateGraphics();
                    
                    // Создаём перо и настраиваем так, чтоб линия не распадалась на пиксели
                    var pen = new Pen(MainForm.CurColor, MainForm.CurWidth);
                    pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                    pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

                    float R = Math.Max(e.X - oldX, e.Y - oldY) / 2f;
                    float r = R / 2;
                    int n = MainForm.StarTips;
                    bool isFilled = MainForm.CurFigure == Figures.FilledStar ? true : false;
                    if (R < 0 || r < 0 || n <= 0) {
                        return;
                    }
                    FiguresPainter.DrawStar(g, pen, oldX, oldY, R, r, n, isFilled: isFilled);
                }
            }
        }

        // Возникает в момент нажатия кнопки мыши, когда указатель - над компонентом (сохранение координат и рисунка до изменения)
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e) {
            oldX = e.X;
            oldY = e.Y;
            // Сохраняем текущую картинку перед изменениями
            prevBmp = new Bitmap(bmp);
        }

        // В момент отпускания кнопки мыши, когда указатель - над компонентом (сохранение звезды в картинку)
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e) {
            if (MainForm.CurFigure == Figures.Star || MainForm.CurFigure == Figures.FilledStar) {
                // Получаем поверхность рисования из картинки
                Graphics g = Graphics.FromImage(bmp);
                // Создаём перо и настраиваем так, чтоб линия не распадалась на пиксели
                var pen = new Pen(MainForm.CurColor, MainForm.CurWidth);
                pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

                float R = Math.Max(e.X - oldX, e.Y - oldY) / 2f;
                float r = R / 2;
                int n = MainForm.StarTips;
                bool isFilled = MainForm.CurFigure == Figures.FilledStar ? true : false;
                if (R < 0 || r < 0 || n <= 0) {
                    return;
                }
                FiguresPainter.DrawStar(g, pen, oldX, oldY, R, r, n, isFilled: isFilled);

                // После рисования инвалидируем рисунок для отображения изменений
                pictureBox1.Invalidate();
            }
        }

        /// <summary>
        /// Команда "Сохранить как" для холста
        /// </summary>
        public void SaveAs() {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.Filter = "Windows Bitmap (*.bmp)|*.bmp| Файлы JPEG (*.jpg)|*.jpg";
            dlg.FileName = Text; // предлагаем дефолтное имя
            ImageFormat[] ff = { ImageFormat.Bmp, ImageFormat.Jpeg };
            if (dlg.ShowDialog() == DialogResult.OK) {
                bmp.Save(dlg.FileName, ff[dlg.FilterIndex - 1]);
                // Сохраняем путь к файлу
                FileName = dlg.FileName;
                // Имя изображения выводим в заголовок формы
                Text = dlg.FileName.Split('\\').Last();
            }
        }

        /// <summary>
        /// Сохранение холста в текущий файл
        /// </summary>
        public void Save() {
            string fileExt = FileName.Split('.').Last();
            ImageFormat imageFormat = fileExt == "bmp" ? ImageFormat.Bmp : ImageFormat.Jpeg;
            bmp.Save(FileName, imageFormat);
        }

        // При закрытии формы (сохранение холста)
        private void Canvas_FormClosing(object sender, FormClosingEventArgs e) {
            var dr = MessageBox.Show($"Вы хотите сохранить холст \"{Text}\" перед закрытием?", "Закрытие холста", MessageBoxButtons.YesNoCancel);
            if (dr == DialogResult.Cancel) {
                e.Cancel = true;
                return;
            }
            if (dr == DialogResult.Yes) {
                if (FileName == null)
                    SaveAs();
                else
                    Save();
            }
        }

        // При уходе мыши с pictureBox (убираем координаты мыши из статуса)
        private void pictureBox1_MouseLeave(object sender, EventArgs e) {
            var p = MdiParent as MainForm;
            p.toolStripStatusLabel1.Text = String.Empty;
        }

        // При нажатии клавиш (обработка комбинации клавиш Ctrl + Z)
        private void Canvas_KeyDown(object sender, KeyEventArgs e) {
            // Если нажата комбинация Ctrl + Z
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Z) {
                if (prevBmp != null) {
                    // Создаём новую битовую карту размером с текущую картинку
                    Bitmap tbmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                    Graphics g = Graphics.FromImage(tbmp);
                    g.Clear(Color.White); // очищаем область и заливаем белым цветом
                    g.DrawImage(prevBmp, new Point(0, 0)); // с начала координат рисуем прошлое изображение
                    prevBmp = bmp; // Текущую картинку отправляем в прошлое
                    // Заменяем старую битовую карту новой
                    bmp = tbmp;
                    pictureBox1.Image = bmp;
                }
            }
        }
    }
}
