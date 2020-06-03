using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3Paint {
    public enum Figures { None, Star, FilledStar};

    static class FiguresPainter {
        /// <summary>
        /// Метод рисования n-конечной звезды
        /// </summary>
        /// <param name="gCanvas">Холст, на котором должна быть отрисована звезда</param>
        /// <param name="pen">Перо, которым будет нарисована звезда</param>
        /// <param name="xLeft">Координата x верхнего левого угла рисунка</param>
        /// <param name="yUpper">Координата y вехнего левого угла рисунка</param>
        /// <param name="R">Внешний радиус звезды</param>
        /// <param name="r">Внутренний радиус звезды</param>
        /// <param name="n">Количество концов звезды</param>
        /// <param name="rotateAngle">Угол поворота звезды</param>
        public static void DrawStar(Graphics gCanvas, Pen pen, float xLeft, float yUpper, float R, float r, int n, double rotateAngle = Math.PI / 2, bool isFilled = false) {
            /* Для определения координат k-ой точки на окружности в порядке обхода против часовой стрелки (положительный обход)
             * используются следующие формулы:
             * x = x0 + r*cos(alpha + 2*pi/n * k),
             * y = y0 + r*sin(alpha + 2*pi/n * k),
             * где: x0, y0 - координаты центра, r - радиус окружности, alpha - угол поворота звезды (обычно pi/2 - т.е. первым будет рассмотрена верхняя точка)
             * За основу берутся формулы для единичной окружности, а затем значения увеличиваются путём домножения
             * на соответствующие радиусы */

            // Координаты середины звезды - это точка с отступом R от верхнего левого угла рисунка
            float x0 = xLeft + R,
                y0 = yUpper + R;
            rotateAngle = -rotateAngle; // т.к. ось Y направлена вниз, положительный ход по окружности (против часовой) для нас - это отрицательный ход

            double a = rotateAngle; // первая точка на окружности имеет угол rotateAngle

            PointF[] points = new PointF[2 * n + 1]; // точек 2n + 1, т.к. мы должны вернуться в исходную точку
            for (int k = 0; k < 2 * n + 1; k++) {
                float curRadius = k % 2 == 0 ? R : r; // попеременно рассматриваем точки на внешнем и внутреннем радиусах
                points[k] = new PointF(x0 + curRadius * (float)Math.Cos(a), y0 + curRadius * (float)Math.Sin(a));
                a += Math.PI / n; // шагаем по окружности с шагом pi / n, чтоб попеременно попадать в угол то внешней, то внутренней точки
            }

            if (isFilled)
                gCanvas.FillPolygon(new SolidBrush(pen.Color), points);
            else
                gCanvas.DrawLines(pen, points);
        }
    }
}
