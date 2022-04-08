using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace saper
{
    public class Cell
    {
        /*
         * n - free cell
         * b - bomb
         * 0 .. N - near bomb type
         */
        char type;
        bool isOpen;
        int[] coords;
        bool isFlaged;

        public delegate void CellEvent();

        public event CellEvent openCell;

        public Cell(int[] coords, char type = 'n', bool isOpen = false, bool isFlaged = false)
        {
            this.type = type;
            this.isOpen = isOpen;
            this.coords = coords;
            this.isFlaged = isFlaged;
        }

        public void open()
        {
            if (this.isOpened()) return;
            this.isOpen = true;
            openCell();
        }

        public bool isOpened()
        {
            return this.isOpen;
        }

        public int[] getCoords()
        {
            return this.coords;
        }

        public int getX()
        {
            return this.coords[0];
        }

        public int getY()
        {
            return this.coords[1];
        }

        public bool eqlType(char _eq)
        {
            return this.type == _eq;
        }

        public void setType(char t)
        {
            this.type = t;
        }

        public void updateTypeToNumOrNextNum()
        {
            if (this.type == 'b') return;
            if (this.type == 'n') {
                this.type = '1';
                return;
            }
            if (Int32.TryParse(this.type.ToString(), out int _a))
            {
                this.type = (_a + 1).ToString()[0];
            }
        }

        public bool isFree()
        {
            return this.type == 'n';
        }

        public bool isNum()
        {
            return Int32.TryParse(this.type.ToString(), out int _a);
        }

        public bool isBomb()
        {
            return this.type == 'b';
        }

        public bool getFlag()
        {
            return this.isFlaged;
        }

        public void toggleFlag()
        {
            this.isFlaged = !isFlaged;
        }

        public char getType()
        {
            return this.type;
        }

        public Color getColor()
        {
            if (!isBomb() && getFlag()) return Color.PaleVioletRed;
            switch (this.type)
            {
                default:
                    return Color.Transparent;
                case 'b':
                    return this.isFlaged ? Color.DarkViolet : Color.Black;
                case '1':
                    return Color.Blue;
                case '2':
                    return Color.Green;
                case '3': 
                    return Color.Red;
                case '4':
                    return Color.BlueViolet;
                case '5':
                    return Color.CornflowerBlue;
                case '6':
                    return Color.Orange;
                case '7':
                    return Color.OrangeRed;
                case '8':
                    return Color.DarkRed;
            }
        }
    }
}
