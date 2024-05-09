using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace EPaperApp
{
    internal interface IStatusScreen
    {
        void Initialize();
        bool IsReady { get; }
        void GetScreen(SkiaSharp.SKCanvas canvas, SkiaSharp.SKImageInfo info);

        event EventHandler? HasChanged;
    }

    internal abstract class ScreenBase : IStatusScreen
    {
        public abstract bool IsReady { get; }

        public event EventHandler? HasChanged;

        protected void RaiseHasChanged()
        {
            IsDirty = true;
            HasChanged?.Invoke(this, EventArgs.Empty);
        }


        public virtual void GetScreen(SKCanvas canvas, SKImageInfo info)
        {
            IsDirty = false;
        }
        public bool IsDirty { get; private set; }

        public abstract void Initialize();

        protected static void DrawTitle(string title, SKCanvas canvas, SKImageInfo info)
        {
            canvas.DrawRect(new SKRect(0,0,info.Width, 12), new SKPaint() { Color = SKColors.Black });
            DrawText(canvas, title, 10, info.Width / 2, 10, centerHorizontal: SKTextAlign.Center, color: SKColors.White);
            var time = DateTime.Now.ToShortTimeString();
            DrawText(canvas, time, 10, info.Width - 1, 10, centerHorizontal: SKTextAlign.Right, color: SKColors.White);
        }
        protected static void DrawTime(SKCanvas canvas, SKImageInfo info)
        {

            var time = DateTime.Now.ToShortTimeString();
            //paint.MeasureText()
            //font10.GetGlyphs(time, out Span<ushort> glyphs);
            //var size = font10.MeasureText(time, out var bounds, paint);
            //canvas.DrawText(time, canvas.Width - 30, 12, font10, paint);
            DrawText(canvas, time, 10, info.Width - 1, 11, centerHorizontal: SKTextAlign.Right);
        }
        private static float WrapLines(string longLine, float lineLengthLimit, float x, float y, SKCanvas canvas, SKPaint defPaint)
        {
            var wrappedLines = new List<string>();
            var lineLength = 0f;
            var line = "";
            foreach (var word in longLine.Split(' '))
            {
                var wordWithSpace = word + " ";
                var wordWithSpaceLength = defPaint.MeasureText(wordWithSpace);
                if (lineLength + wordWithSpaceLength > lineLengthLimit)
                {
                    wrappedLines.Add(line);
                    line = "" + wordWithSpace;
                    lineLength = wordWithSpaceLength;
                }
                else
                {
                    line += wordWithSpace;
                    lineLength += wordWithSpaceLength;
                }
            }
            if(line.Trim().Length > 0)
                wrappedLines.Add(line);
            foreach (var wrappedLine in wrappedLines)
            {
                canvas.DrawText(wrappedLine, x, y, defPaint);
                y += (float)Math.Round(defPaint.FontSpacing);
            }
            return y;
        }
        protected static float DrawText(SKCanvas canvas, string text, float size, float x, float y, SKColor? color = null, SKTextAlign centerHorizontal = SKTextAlign.Left, bool centerVertical = false, bool bold = false, float maxwidth = -1)
        {
            using var paint = new SKPaint
            {
                Color = color ?? SKColors.Black,
                IsAntialias = false,
                Style = SKPaintStyle.Fill,
                TextAlign = centerHorizontal,
                TextSize = size,
                HintingLevel = SKPaintHinting.NoHinting,
                IsAutohinted = false,
                SubpixelText = false,
                LcdRenderText = false
            };
            if (bold)
                paint.Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
            using var font = new SKFont() { Size = size, Subpixel = false, Edging = SKFontEdging.Alias };
            var coord = new SKPoint(x, y + (centerVertical ? size / 2f : 0f));
            if (maxwidth > 0)
            {
                return WrapLines(text, maxwidth, x, coord.Y, canvas, paint);
            }
            else
            {
                canvas.DrawText(text, coord.X, coord.Y, font, paint);
                return paint.FontSpacing;
            }
        }

        protected void DrawTitle(SKCanvas canvas, SKImageInfo info, string text)
        {
            DrawText(canvas, text, 16, info.Width / 2, 18, color: SKColors.Gray, centerHorizontal: SKTextAlign.Center, centerVertical: false);
        }

        public struct Point
        {
            public Point(double x, double y) { X = x; Y = y; }
            public double X { get; set; }
            public double Y { get; set; }
        }

        public struct DatePoint
        {
            public DatePoint(DateTimeOffset time, double y) { Time = time; Y = y; }
            public DateTimeOffset Time { get; set; }
            public double X => Time.ToUnixTimeSeconds();
            public double Y { get; set; }
        }

        protected void DrawGraph(SKCanvas canvas, SKRect rect, double[] values, bool smooth = true)
        {
            var data = new Point[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                data[i] = new Point(i, values[i]);
            }
            DrawGraph(canvas, rect, data, color: null, smooth: smooth);
        }

        protected void DrawGraph(SKCanvas canvas, SKRect rect, IEnumerable<DatePoint> values, SKColor? color = null, bool smooth = true)
        {
            var data = new List<Point>();
            foreach (var d in values)
            {
                data.Add(new Point(d.X, d.Y));
            }
            DrawGraph(canvas, rect, data.ToArray(), color, smooth);
        }

        protected void DrawGraph(SKCanvas canvas, SKRect rect, Point[] values, SKColor? color = null, bool smooth = true)
        {
            var minX = values.Select(s => s.X).Min();
            var maxX = values.Select(s => s.X).Max();
            var minY = values.Select(s => s.Y).Min();
            var maxY = values.Select(s => s.Y).Max();
            using var paint = new SKPaint();
            paint.Style = SKPaintStyle.Stroke;
            paint.IsAntialias = false;
            paint.Color = color ?? SKColors.Black;
            paint.StrokeWidth = 2;

            double x = rect.Left;
            SKPoint[] points = new SKPoint[values.Length];
            points[0] = new SKPoint((float)((values[0].X - minX) / (maxX - minX) * rect.Width + rect.Left), (float)((maxY - values[0].Y) / (maxY - minY) * rect.Height + rect.Top));
            for (int i = 1; i < values.Length; i++)
            {
                points[i] = new SKPoint((float)((values[i].X - minX) / (maxX - minX) * rect.Width + rect.Left), (float)((maxY - values[i].Y) / (maxY - minY) * rect.Height + rect.Top));
            }
            if (smooth)
            {
                using var path = CreateSpline(points);
                canvas.DrawPath(path, paint);
            }
            else
            {
                using var path = new SKPath();
                path.MoveTo(points[0]);
                for (int i = 1; i < points.Length; i++)
                {
                    path.LineTo(points[i]);
                }
                canvas.DrawPath(path, paint);
            }
        }


        /// <summary>
        /// Creates a Spline path through a given set of points.
        /// </summary>
        /// <param name="Points">Points between which the spline will be created.</param>
        /// <returns>Spline path.</returns>
        public static SKPath CreateSpline(params SKPoint[] Points)
        {
            return CreateSpline(null, Points);
        }

        /// <summary>
        /// Creates a Spline path through a given set of points.
        /// </summary>
        /// <param name="AppendTo">Spline should be appended to this path. If null, a new path will be created.</param>
        /// <param name="Points">Points between which the spline will be created.</param>
        /// <returns>Spline path.</returns>
        private static SKPath CreateSpline(SKPath AppendTo, params SKPoint[] Points)
        {
            int i, c = Points.Length;
            if (c == 0)
                throw new ArgumentException("No points provided.", nameof(Points));

            if (AppendTo is null)
            {
                AppendTo = new SKPath();
                AppendTo.MoveTo(Points[0]);
            }
            else
                AppendTo.LineTo(Points[0]);

            if (c == 1)
                return AppendTo;

            if (c == 2)
            {
                AppendTo.LineTo(Points[1]);
                return AppendTo;
            }

            double[] V = new double[c];

            for (i = 0; i < c; i++)
                V[i] = Points[i].X;

            GetCubicBezierCoefficients(V, out double[] Ax, out double[] Bx);

            for (i = 0; i < c; i++)
                V[i] = Points[i].Y;

            GetCubicBezierCoefficients(V, out double[] Ay, out double[] By);

            for (i = 0; i < c - 1; i++)
            {
                AppendTo.CubicTo((float)Ax[i], (float)Ay[i], (float)Bx[i], (float)By[i],
                    Points[i + 1].X, Points[i + 1].Y);
            }

            return AppendTo;
        }

        /// <summary>
        /// Gets a set of coefficients for cubic Bezier curves, forming a spline, one coordinate at a time.
        /// </summary>
        /// <param name="V">One set of coordinates.</param>
        /// <param name="A">Corresponding coefficients for first control points.</param>
        /// <param name="B">Corresponding coefficients for second control points.</param>
        private static void GetCubicBezierCoefficients(double[] V, out double[] A, out double[] B)
        {
            // Calculate Spline between points P[0], ..., P[N].
            // Divide into segments, B[0], ...., B[N-1] of cubic Bezier curves:
            //
            // B[i](t) = (1-t)³P[i] + 3t(1-t)²A[i] + 3t²(1-t)B[i] + t³P[i+1]
            //
            // B'[i](t) = (-3+6t-3t²)P[i]+(3-12t+9t²)A[i]+(6t-9t²)B[i]+3t²P[i+1]
            // B"[i](t) = (6-6t)P[i]+(-12+18t)A[i]+(6-18t)B[i]+6tP[i+1]
            //
            // Choose control points A[i] and B[i] such that:
            //
            // B'[i](1) = B'[i+1](0) => A[i+1]+B[i]=2P[i+1], i<N		(eq 1)
            // B"[i](1) = B"[i+1](0) => A[i]-2B[i]+2A[i+1]-B[i+1]=0		(eq 2)
            //
            // Also add the boundary conditions:
            //
            // B"[0](0)=0 => 2A[0]-B[0]=P[0]			(eq 3)
            // B"[N-1](1)=0 => -A[N-1]+2B[N-1]=P[N]		(eq 4)
            //
            // Method solves this linear equation for one coordinate of A[i] and B[i] at a time.
            //
            // First, the linear equation, is reduced downwards. Only coefficients close to
            // the diagonal, and in the right-most column need to be processed. Furthermore,
            // we don't have to store values we know are zero or one. Since number of operations
            // depend linearly on number of vertices, algorithm is O(N).

            int N = V.Length - 1;
            int N2 = N << 1;
            int i = 0;
            int j = 0;
            double r11, r12, r15;               // r13 & r14 always 0.
            double r22, r23, r25;               // r21 & r24 always 0 for all except last equation, where r21 is -1.
            double /*r31,*/ r32, r33, r34, r35;
            double[,] Rows = new double[N2, 3];
            double a;

            A = new double[N];
            B = new double[N];

            r11 = 2;        // eq 3
            r12 = -1;
            r15 = V[j++];

            r22 = 1;        // eq 1
            r23 = 1;
            r25 = 2 * V[j++];

            // r31 = 1;        // eq 2
            r32 = -2;
            r33 = 2;
            r34 = -1;
            r35 = 0;

            while (true)
            {
                a = 1 / r11;
                // r11 = 1;
                r12 *= a;
                r15 *= a;

                // r21 is always 0. No need to eliminate column.
                // r22 is always 1. No need to scale row.

                // r31 is always 1 at this point.
                // r31 -= r11;
                r32 -= r12;
                r35 -= r15;

                if (r32 != 0)
                {
                    r33 -= r32 * r23;
                    r35 -= r32 * r25;
                    // r32 = 0;
                }

                // r33 is always 0.

                // r11 always 1.
                Rows[i, 0] = r12;
                Rows[i, 1] = 0;
                Rows[i, 2] = r15;
                i++;

                // r21, r24 always 0.
                Rows[i, 0] = r22;
                Rows[i, 1] = r23;
                Rows[i, 2] = r25;
                i++;

                if (i >= N2 - 2)
                    break;

                r11 = r33;
                r12 = r34;
                r15 = r35;

                r22 = 1;        // eq 1
                r23 = 1;
                r25 = 2 * V[j++];

                // r31 = 1;        // eq 2
                r32 = -2;
                r33 = 2;
                r34 = -1;
                r35 = 0;
            }

            r11 = r33;
            r12 = r34;
            r15 = r35;

            //r21 = -1;		// eq 4
            r22 = 2;
            r23 = 0;
            r25 = V[j++];

            a = 1 / r11;
            //r11 = 1;
            r12 *= a;
            r15 *= a;

            //r21 += r11;
            r22 += r12;
            r25 += r15;

            r25 /= r22;
            r22 = 1;

            // r11 always 1.
            Rows[i, 0] = r12;
            Rows[i, 1] = 0;
            Rows[i, 2] = r15;
            i++;

            // r21 and r24 always 0.
            Rows[i, 0] = r22;
            Rows[i, 1] = r23;
            Rows[i, 2] = r25;
            i++;

            // Then eliminate back up:

            j--;
            while (i > 0)
            {
                i--;
                if (i < N2 - 1)
                {
                    a = Rows[i, 1];
                    if (a != 0)
                    {
                        Rows[i, 1] = 0;
                        Rows[i, 2] -= a * Rows[i + 1, 2];
                    }
                }

                B[--j] = Rows[i, 2];

                i--;
                a = Rows[i, 0];
                if (a != 0)
                {
                    Rows[i, 0] = 0;
                    Rows[i, 2] -= a * Rows[i + 1, 2];
                }

                A[j] = Rows[i, 2];
            }
        }

    }
}
