using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace saper
{
    public partial class Level : Form
    {
        Form1 owner;
        public Level(Form1 form1)
        {
            InitializeComponent();
            this.owner = form1;
        }

        public delegate void UpdateDate();

        public event UpdateDate changeSizeAndBombCount;

        private TextBox size;
        private TextBox bombCount;

        private void Level_Load(object sender, EventArgs e)
        {
            this.Size = new Size(250, 180);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            Label sizeLabel = new Label()
            {
                Text = "Board Size N:",
                Location = new Point(15,15),
                AutoSize = true
            };
            size = new TextBox()
            {
                Text = Form1.DEFAULT_SIZE.ToString(),
                Location = new Point(110,11),
                Size = new Size(100,20)
            };
            this.Controls.Add(sizeLabel);
            this.Controls.Add(size);

            Label bombCountLabel = new Label()
            {
                Text = "Bomb Count N:",
                Location = new Point(15,50),
                AutoSize = true
            };
            bombCount = new TextBox()
            {
                Text = Form1.DEFAULT_BOMB_COUNT.ToString(),
                Location = new Point(110,46),
                Size = new Size(100,20)
            };
            this.Controls.Add(bombCountLabel);
            this.Controls.Add(bombCount);

            Button save = new Button()
            {
                Location = new Point(15, 70),
                Size = new Size(200, 20),
                Text = "Save Cheange"
            };
            this.Controls.Add(save);
            save.Click += new EventHandler(updateData);

            Button restart = new Button()
            {
                Location = new Point(15, 95),
                Size = new Size(200, 20),
                Text = "Restart"
            };
            this.Controls.Add(restart);
            restart.Click += new EventHandler(restartData);
        }

        private void restartData(object sender, EventArgs args)
        {
            changeSizeAndBombCount();
        }

        private void updateData(object sender, EventArgs args)
        {
            int sizeOut;
            int bombCountOut;
            Random rnd = new Random();
            if (!Int32.TryParse(size.Text, out sizeOut)) sizeOut = Form1.DEFAULT_SIZE;
            if (!Int32.TryParse(bombCount.Text, out bombCountOut)) bombCountOut = Form1.DEFAULT_BOMB_COUNT;
            if (sizeOut< 10) sizeOut = 10;
            if (bombCountOut < 0 || bombCountOut > sizeOut * sizeOut) bombCountOut = rnd.Next(Form1.DEFAULT_BOMB_COUNT, sizeOut * sizeOut / 5);
            owner.size = sizeOut;
            owner.bombCout = bombCountOut;
            changeSizeAndBombCount();
        }
    }
}
