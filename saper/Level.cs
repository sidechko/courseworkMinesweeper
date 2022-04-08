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
        MainForm owner;
        public Level(MainForm form1)
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
            this.Size = new Size(250, 200);
            this.MinimizeBox = false;
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            this.Text = "Настройки поля";

            Label sizeLabel = new Label()
            {
                Text = "Размеры поля:",
                Location = new Point(15,35),
                AutoSize = true
            };
            size = new TextBox()
            {
                Text = owner.getSize().ToString(),
                Location = new Point(110,31),
                Size = new Size(100,20)
            };
            this.Controls.Add(sizeLabel);
            this.Controls.Add(size);

            Label bombCountLabel = new Label()
            {
                Text = "Кол-во бомб:",
                Location = new Point(15,70),
                AutoSize = true
            };
            bombCount = new TextBox()
            {
                Text = owner.getBombCount().ToString(),
                Location = new Point(110,66),
                Size = new Size(100,20)
            };
            this.Controls.Add(bombCountLabel);
            this.Controls.Add(bombCount);

            Button save = new Button()
            {
                Location = new Point(15, 95),
                Size = new Size(200, 25),
                Text = "Сохранить изменения"
            };
            this.Controls.Add(save);
            save.Click += new EventHandler(updateData);

            Button restart = new Button()
            {
                Location = new Point(15, 125),
                Size = new Size(200, 25),
                Text = "Перезапуск"
            };
            this.Controls.Add(restart);
            restart.Click += new EventHandler(restartData);

            MenuStrip menu = new MenuStrip();
            ToolStripMenuItem levelsDefault = new ToolStripMenuItem("Стандартная сложность");
            levelsDefault.DropDownItems.Add(createNewLevel("Простой", 9, 10));
            levelsDefault.DropDownItems.Add(createNewLevel("Средний", 16, 40));
            levelsDefault.DropDownItems.Add(createNewLevel("Эксперт", 22, 99));
            menu.Items.Add(levelsDefault);
            this.Controls.Add(menu);
        }

        private ToolStripMenuItem createNewLevel(string name, int size ,int bombCount)
        {
            ToolStripMenuItem level = new ToolStripMenuItem(name, null, new EventHandler((e,eargs)=> { this.size.Text=size.ToString(); this.bombCount.Text = bombCount.ToString(); }));
            return level;
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
            if (!Int32.TryParse(size.Text, out sizeOut)) sizeOut = MainForm.DEFAULT_SIZE;
            if (!Int32.TryParse(bombCount.Text, out bombCountOut)) bombCountOut = MainForm.DEFAULT_BOMB_COUNT;
            if (sizeOut < 9) sizeOut = 9;
            if (bombCountOut < 0 || bombCountOut > sizeOut * sizeOut) bombCountOut = rnd.Next(MainForm.DEFAULT_BOMB_COUNT, sizeOut * sizeOut / 5);
            owner.setSize(sizeOut);
            owner.setBombCount(bombCountOut);
            changeSizeAndBombCount();
        }
    }
}
