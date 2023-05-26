using System.Runtime.InteropServices;
using Rary.GL.Rasterizer;
using Rary.GL.Fonts;
using System.Numerics;
using Cosmos.Core;
using Rary.GL;
using Cosmos.System;

namespace Rary.GL
{
    /// <summary>
    /// Implementation of high-res console
    /// </summary>
    public class Terminal
    {
        /// <summary>
        /// The canvas that this terminal should draw on.
        /// </summary>
        public SVGAIICanvas canvas;

        /// <summary>
        /// The font that this font should draw with.
        /// </summary>
        public Font font;

        /// <summary>
        /// Cursor position, in pixels. THIS IS OFFSETTED BY THE CARRIDGE RETURN MARGINS
        /// </summary>
        public int CursorX = 0, CursorY = 0;

        /// <summary>
        /// Max cursor position, in pixels. THIS IS OFFSETTED BY THE CARRIDGE RETURN MARGINS
        /// </summary>
        public ushort maxX, maxY;

        /// <summary>
        /// Carridge return left margin, basically where should lines start horizontally.
        /// </summary>
        public int CRLeftMargin = 0;

        /// <summary>
        /// Carridge return top margin, basically where should lines start vertically.
        /// </summary>
        public int CRTopMargin = 0;

        /// <summary>
        /// Foreground color of text.
        /// </summary>
        public Rary.GL.Color ForegroundC = Rary.GL.Color.White;

        /// <summary>
        /// Background color of text.
        /// </summary>
        public Rary.GL.Color BackgroundC = Rary.GL.Color.Black;


        /// <summary>
        /// Set this to true for backspaces to work properly, set to false when you are not using inputs, as it controls the spaces between letters which makes them look better.
        /// </summary>
        // TODO: do a better backspace fix than this stupid fucking hack
        public bool UImode = false;


        public Terminal(SVGAIICanvas canvas, Font font)
        {
            this.canvas = canvas;
            this.font = font;
            this.maxX = this.canvas.Width;
            this.maxY = this.canvas.Height;
        }

        /// <summary>
        /// Writes a string to the virtual terminal.
        /// </summary>
        /// <param name="s">The string that you want to write.</param>
        public void Write(string s)
        {
            foreach (char c in s)
            {
                if (c == '\n') { if ( CRTopMargin + CursorY + 16 > maxY) { Clear(); } CursorY += 16; CursorX = CRLeftMargin; } // NEWLINE
                else if (c == '\t') { CursorX += 16; } // TAB
                else if (c == '\b') // BACKSPACE
                {
                    if (CursorX == CRLeftMargin)
                    {
                        break;
                    }
                    CursorX -= 8; // add exceptions to this rule
                    this.canvas.DrawFilledRectangle(CRLeftMargin + CursorX, CRTopMargin + CursorY, 8, 16, 0, BackgroundC);
                }
                else
                {
                    this.canvas.DrawFilledRectangle(CRLeftMargin + CursorX, CRTopMargin + CursorY, 8, 16, 0, BackgroundC);
                    this.canvas.DrawString(CRLeftMargin + CursorX, CRTopMargin + CursorY, c.ToString(), font, ForegroundC);
                    if (CursorX + 16 > maxX) { if (CRTopMargin + CursorY + 16 > maxY) { Clear(); } CursorY += 16; CursorX = CRLeftMargin; }
                    if (UImode)
                    {
                        CursorX += 8;
                    }
                    else
                    {
                        CursorX += font.MeasureString(c.ToString());
                    }
                }
            }
            this.canvas.Update();
        }

        /// <summary>
        /// Writes a string to the virtual terminal with specified colors. Used for quick colors.
        /// </summary>
        /// <param name="s">The string that you want to write.</param>
        /// <param name="fcolor">Foreground color</param>
        /// <param name="bcolor">Background color</param>
        public void Write(string s, Color fcolor, Color bcolor)
        {
            Color oldf = ForegroundC;
            Color oldb = BackgroundC;

            ForegroundC = fcolor;
            BackgroundC = bcolor;

            Write(s);

            ForegroundC = oldf;
            BackgroundC = oldb;
        }

        /// <summary>
        /// Write(), but with \n already added.
        /// </summary>
        /// <param name="s">The string that you want to write.</param>
        public void WriteLine(string s)
        {
            Write(s + '\n');
        }
        /// <summary>
        /// Writes a newline then stops.
        /// </summary>
        public void WriteLine()
        {
            Write("\n");
        }

        /// <summary>
        /// Clears the area that is allocated to the virtual terminal with the BackgroundC(olor). offset by carridge return margins.
        /// </summary>
        public void Clear()
        {
            this.canvas.DrawFilledRectangle(CRLeftMargin,CRTopMargin,maxX,maxY,0,BackgroundC);
        }
    }
}
