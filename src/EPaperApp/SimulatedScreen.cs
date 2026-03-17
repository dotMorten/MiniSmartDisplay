#if WINDOWS
using SkiaSharp;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace EPaperApp
{
    internal class SimulatedScreen : IScreen
    {
        public int Width { get; } 
        public int Height { get; }
        public event EventHandler Touched;

        private readonly SimulatedScreenForm _form;
        private bool _disposed;

        public SimulatedScreen(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            _form = new SimulatedScreenForm(Width, Height, OnTouched);
            var thread = new Thread(() => Application.Run(_form));
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void DisplayImage(SKBitmap bitmap, bool partial)
        {
            if (_disposed) return;
            _form?.UpdateImage(bitmap);
        }

        public void Clear(bool white)
        {
            if (_disposed) return;
            _form?.Clear(white);
        }

        private void OnTouched()
        {
            Touched?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _form?.Invoke(new Action(() => _form.Close()));
        }

        private class SimulatedScreenForm : Form
        {
            private Bitmap _bitmap;
            private readonly int _width;
            private readonly int _height;
            private readonly Action _touchCallback;

            public SimulatedScreenForm(int width, int height, Action touchCallback)
            {
                _width = width;
                _height = height;
                _touchCallback = touchCallback;
                this.Text = "Simulated E-Paper Screen";
                this.ClientSize = new Size(_width, _height);
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.DoubleBuffered = true;
                this._bitmap = new Bitmap(_width, _height);
                this.MouseDown += (s, e) => _touchCallback?.Invoke();
            }
            public void Clear(bool white)
            {
                Color c = white ? Color.White : Color.Black;
                lock (this)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        for (int x = 0; x < _width; x++)
                        {
                            _bitmap.SetPixel(x, y, c);
                        }
                    }
                }
                this.Invoke(new Action(() => this.Invalidate()));
            }
            public void UpdateImage(SKBitmap bitmap)
            {
                lock (this)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        for (int x = 0; x < _width; x++)
                        {
                            var skColor = bitmap.GetPixel(x, y);
                            Color c = Color.FromArgb(skColor.Red, skColor.Green, skColor.Blue);
                            _bitmap.SetPixel(x, y, c);
                        }
                    }
                }
                this.Invoke(new Action(() => this.Invalidate()));
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                lock (this)
                {
                    e.Graphics.DrawImage(_bitmap, 0, 0, _width, _height);
                }
            }
        }
    }
}
#endif