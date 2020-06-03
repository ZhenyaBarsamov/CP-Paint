using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintInterfaces
{
    /// <summary>
    /// Общий интерфейс для плагинов
    /// </summary>
    public interface IPlugin {
        /// <summary>
        /// Название плагина
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Версия плагина
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Имя автора плагина
        /// </summary>
        string Author { get; }
    }

    /// <summary>
    /// Интерфейс для фильтров-плагинов
    /// </summary>
    public interface IFilterPlugin : IPlugin {
        /// <summary>
        /// Применяет фильтр к изображению
        /// </summary>
        /// <param name="image">Изображение, подлежащее изменению</param>
        Bitmap TransformImage(Bitmap image);
    }

    /// <summary>
    /// Интерфейс для фигур-плагинов
    /// </summary>
    public interface IFigurePlugin : IPlugin {
        // TODO: Как программист узнает, какие параметры передавать в fFarams?

        /// <summary>
        /// Рисует фигуру по заданным параметрам
        /// </summary>
        /// <param name="gCanvas">Поверхность, на которой будет нарисована фигура</param>
        /// <param name="pen">Перо для рисования</param>
        /// <param name="xLeft">Координата X левого верхнего угла рисунка</param>
        /// <param name="yTop">Координата Y левого верхнего угла рисунка</param>
        /// <param name="xRight">Координата X правого нижнего угла рисунка</param>
        /// <param name="yBottom">Координата Y правого нижнего угла рисунка</param>
        /// <param name="isFilled">Необходимо ли закрасить нарисованную фигуру</param>
        /// <param name="fParams">Список параметров фигуры</param>
        void DrawFigure(Graphics gCanvas, Pen pen, float xLeft, float yTop, float xRight, float yBottom, bool isFilled, params float[] fParams);
    }
}
