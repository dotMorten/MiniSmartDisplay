using System.Buffers;
using System.Diagnostics;

namespace EPaperApp
{
    public class ImageBuffer : IDisposable
    {
        private byte[] buffer;
        private readonly int width;
        private readonly int height;
        private bool _isDisposed;
        private int bufferSize;

        public int Width => width;
        public int Height => height;

        public ImageBuffer(int width, int height)
        {
            this.width = width;
            this.height = height;
            bufferSize = (int)(width / 8 + (width % 8 == 0 ? 0 : 1)) * height;
            buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
        }

        ~ImageBuffer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (_isDisposed) return;
            _isDisposed = true;
            ArrayPool<byte>.Shared.Return(buffer);
            buffer = null!;
        }

        public void Clear()
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = 0xFF;
            }
        }

        public void SetPixel(int x, int y, bool enabled)
        {
            if (x >= width || y >= height) return;
            int index = (x + y * width) >> 3;
            byte bitMask = (byte)(0x80 >> (x % 8));

            buffer[index] = enabled ? (byte)(buffer[index] | bitMask) : (byte)(buffer[index] & ~bitMask);
        }

        public bool GetPixel(int x, int y)
        {
            int index = (x + y * width) >> 3;
            return (buffer[index] & (byte)(0x80 >> (x % 8))) != 0;
        }

        public ImageBuffer Rotate()
        {
            var rotatedBuffer = new ImageBuffer(Height, Width);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var px = GetPixel(x, y);
                    rotatedBuffer.SetPixel(Height - y - 1, x, px);
                }
            }
            return rotatedBuffer;
        }

        public ReadOnlySpan<byte> Buffer => new ReadOnlySpan<byte>(buffer, 0, bufferSize);

        /******************************************************************************
        function: Show English characters
        parameter:
            Xpoint           ：X coordinate
            Ypoint           ：Y coordinate
            Acsii_Char       ：To display the English characters
            Font             ：A structure pointer that displays a character size
            Color_Foreground : Select the foreground color
            Color_Background : Select the background color
        ******************************************************************************/
        public void Paint_DrawChar(int Xpoint, int Ypoint, char Acsii_Char,
                            Fonts Font, bool color)
        {
            int Page, Column;

            if (Xpoint > Width || Ypoint > Height)
            {
                Trace.WriteLine("Paint_DrawChar Input exceeds the normal display range\r\n");
                return;
            }

            int Char_Offset = (Acsii_Char - ' ') * Font.Height * (Font.Width / 8 + ((Font.Width % 8 != 0) ? 1 : 0));
            //const unsigned char* ptr = &Font->table[Char_Offset];
            var ptr = Char_Offset;
            for (Page = 0; Page < Font.Height; Page++)
            {
                for (Column = 0; Column < Font.Width; Column++)
                {

                    //To determine whether the font background color and screen background color is consistent

                    if ((Font.FontTable[ptr] & (0x80 >> (Column % 8))) != 0)
                    {
                        SetPixel(Xpoint + Column, Ypoint + Page, color);
                        // Paint_DrawPoint(Xpoint + Column, Ypoint + Page, Color_Foreground, DOT_PIXEL_DFT, DOT_STYLE_DFT);
                    }
                    else
                    {
                        if ((Font.FontTable[ptr] & (0x80 >> (Column % 8))) != 0)
                        {
                            SetPixel(Xpoint + Column, Ypoint + Page, color);
                            // Paint_DrawPoint(Xpoint + Column, Ypoint + Page, Color_Foreground, DOT_PIXEL_DFT, DOT_STYLE_DFT);
                        }
                        else
                        {
                            SetPixel(Xpoint + Column, Ypoint + Page, !color);
                            // Paint_DrawPoint(Xpoint + Column, Ypoint + Page, Color_Background, DOT_PIXEL_DFT, DOT_STYLE_DFT);
                        }
                    }
                    //One pixel is 8 bits
                    if (Column % 8 == 7)
                        ptr++;
                }// Write a line
                if (Font.Width % 8 != 0)
                    ptr++;
            }// Write all
        }

        /******************************************************************************
        function:	Display the string
        parameter:
            Xstart           ：X coordinate
            Ystart           ：Y coordinate
            pString          ：The first address of the English string to be displayed
            Font             ：A structure pointer that displays a character size
            Color_Foreground : Select the foreground color
            Color_Background : Select the background color
        ******************************************************************************/
        public void Paint_DrawString(int Xstart, int Ystart, string pString, Fonts Font, bool color, bool centered = false)
        {
            if (centered)
            {
                if (pString.Contains("\n"))
                {

                    foreach (var s in pString.Split('\n'))
                    {
                        Paint_DrawString(Xstart, Ystart, s, Font, color, centered: true);
                        Ystart += Font.Height;
                    }
                    return;
                }
                Xstart = Xstart - (pString.Length * Font.Width) / 2;
            }
            int Xpoint = Xstart;
            int Ypoint = Ystart;
            if (Xstart > Width || Ystart > Height)
            {
                Trace.WriteLine("Paint_DrawString_EN Input exceeds the normal display range\r\n");
                return;
            }
            int index = 0;
            while (index < pString.Length)
            {
                if (pString[index] == '\n')
                {
                    Xpoint = Xstart;
                    Ypoint += Font.Height;
                    index++;
                    continue;
                }
                //if X direction filled , reposition to(Xstart,Ypoint),Ypoint is Y direction plus the Height of the character
                if ((Xpoint + Font.Width) > Width)
                {
                    Xpoint = Xstart;
                    Ypoint += Font.Height;
                }

                // If the Y direction is full, reposition to(Xstart, Ystart)
                if ((Ypoint + Font.Height) > Height)
                {
                    Xpoint = Xstart;
                    Ypoint = Ystart;
                }
                Paint_DrawChar(Xpoint, Ypoint, pString[index], Font, color);

                //The next character of the address
                index++;

                //The next word of the abscissa increases the font of the broadband
                Xpoint += Font.Width;
            }
        }
    }
}