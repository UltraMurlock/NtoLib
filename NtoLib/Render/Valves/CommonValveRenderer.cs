﻿using NtoLib.Valves;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace NtoLib.Render.Valves
{
    internal class CommonValveRenderer : ValveBaseRenderer
    {
        public CommonValveRenderer(ValveControl valveControl) : base(valveControl)
        {
            LineWidth = 2f;

            ErrorLineWidth = 2f;
            ErrorOffset = 5f;
        }



        public override void Draw(Graphics graphics, RectangleF boundsRect, Orientation orientation, bool isLight)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Bounds graphicsBounds = BoundsFromRect(boundsRect);

            if(orientation == Orientation.Vertical)
            {
                Matrix transform = graphics.Transform;
                transform.RotateAt(90f, graphicsBounds.Center);
                graphics.Transform = transform;
                transform.Dispose();

                (graphicsBounds.Width, graphicsBounds.Height) = (graphicsBounds.Height, graphicsBounds.Width);
            }

            Status status = Control.Status;
            Bounds valveBounds = GetValveBounds(graphicsBounds);
            DrawValve(graphics, valveBounds, status, isLight);

            if(status.Error)
                DrawErrorRectangle(graphics, graphicsBounds);
        }



        /// <summary>
        /// Отрисовывает клапан, состоящий из двух треугольников в заданной области
        /// </summary>
        protected void DrawValve(Graphics graphics, Bounds valveBounds, Status status, bool isLight)
        {
            Color[] colors = GetValveColors(status, isLight);
            PointF[][] valvePoints = GetValvePoints(valveBounds);
            for(int i = 0; i < valvePoints.Length; i++)
            {
                using(SolidBrush brush = new SolidBrush(colors[i]))
                    graphics.FillClosedCurve(brush, valvePoints[i], FillMode.Alternate, 0);

            }

            using(Pen pen = new Pen(GetLineColor(Control.Status), LineWidth))
            {
                graphics.DrawClosedCurve(pen, valvePoints[0], 0, FillMode.Alternate);
                graphics.DrawClosedCurve(pen, valvePoints[1], 0, FillMode.Alternate);
            }
        }

        /// <summary>
        /// Возвращает массивы точек для двух треугольников клапана соответственно
        /// </summary>
        protected PointF[][] GetValvePoints(Bounds valveBounds)
        {
            valveBounds.Width -= LineWidth;
            valveBounds.Height -= LineWidth * 1.154f;

            PointF[][] points = new PointF[2][];
            points[0] = new[] {
                    valveBounds.LeftTop,
                    valveBounds.Center,
                    valveBounds.LeftBottom
            };
            points[1] = new[] {
                    valveBounds.RightTop,
                    valveBounds.Center,
                    valveBounds.RightBottom
            };

            return points;
        }
    }
}