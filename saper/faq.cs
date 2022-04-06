using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace saper
{
    public partial class FAQ : Form
    {
        public Dispatcher current = Dispatcher.CurrentDispatcher;
        public FAQ()
        {
            InitializeComponent();
            this.Text = "Справка";
            this.MaximumSize = new Size(450, 460);
            this.MaximizeBox = false;
            this.MinimumSize = this.MinimumSize;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void faq_Load(object sender, EventArgs e)
        {
            for(int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Color color = i > 0 && j > 0 ? Color.Blue : Color.Gray;
                    color = i == 2 && j == 2 ? Color.White : color;
                    string text = i == 2 && j == 2 ? "?" : i > 0 && j > 0 ? "1" : "";
                    if (i == 1 && j == 2) { color = Color.Red; text = "3"; }
                    if (i == 0 && j == 2) { color = Color.Blue; text = "1"; }
                    FAQPanel.Controls.Add(getSapCell(13+31*i, 13+31*j, color, text));
                }
            }

            Cell cell = new Cell(new int[1]);
            for(int i=1; i < 9; i++)
            {
                cell.setType(i.ToString()[0]);
                FAQPanel.Controls.Add(getSapCell(45*i,215,cell.getColor(),i.ToString()));
            }

            Button flagedButton = getSapCell(350,263,Color.White);
            flagedButton.Tag = true;
            flagedButton.MouseDown += (obj, meargs) =>
            {
                if (meargs.Button != MouseButtons.Right) return;
                flagedButton.Text = (bool)flagedButton.Tag ? "F" : "";
                flagedButton.BackColor = (bool)flagedButton.Tag ? Color.MediumVioletRed : Color.White;
                flagedButton.Tag = !(bool)flagedButton.Tag;
            };
            FAQPanel.Controls.Add(flagedButton);
        }

        private Button getSapCell(int x, int y, Color backColor, string text="")
        {
            Button b = new Button()
            {
                BackColor = backColor,
                Size = new Size(30, 30),
                Location = new Point(x, y),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Gray,
                Text = text,
                Font = new Font(FontFamily.GenericSansSerif, 10),
                TextAlign = ContentAlignment.MiddleCenter
            };
            return b;
        }
    }
}
