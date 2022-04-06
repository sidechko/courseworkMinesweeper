using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace saper
{
    public partial class MainForm : Form
    {
        public const int DEFAULT_SIZE = 20;
        public const int DEFAULT_BOMB_COUNT = 20;
        private int clickCount = 0;

        private Panel pole;
        private int poleSize = 600;
        private int flagCount;

        private PictureBox emojiImage;
        private Label flagCountInfo;
        private Label timer;

        private DateTime startTime = DateTime.Now;

        public Cell[,] cells;
        private bool firstClick = true;
        private bool finishGame = false;

        public int bombCout = DEFAULT_BOMB_COUNT;
        public int size = DEFAULT_SIZE;

        public bool faqIsOpen = false;

        public Dispatcher current = Dispatcher.CurrentDispatcher;
        Thread timerThread = null;
        Thread checkerThread = null;
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Saper";
            this.Size = new Size(poleSize+37, poleSize+170);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;


            pole = new Panel
            {
                BackColor = Color.Gray,
                Location = new Point(10, 120),
                Size = new Size(poleSize, poleSize),
                AutoScroll = true,
                AutoScrollMargin = new Size(0, 5)
            };
            this.Controls.Add(pole);


            emojiImage = new PictureBox()
            {
                Size = new Size(50, 50),
                Location = new Point(280, 35)
            };
            this.Controls.Add(emojiImage);
            flagCountInfo = new Label() {
                AutoSize = true,
                Location = new Point(55, 40),
                Font = new Font(FontFamily.GenericMonospace,28,FontStyle.Bold)
            };
            this.Controls.Add(flagCountInfo);
            timer = new Label()
            {
                AutoSize = true,
                Location = new Point(450,40),
                Font = new Font(FontFamily.GenericMonospace, 28, FontStyle.Bold),
                Text = "00:00"
            };
            this.Controls.Add(timer);

            MenuStrip menu = new MenuStrip();
            ToolStripMenuItem restartButton = new ToolStripMenuItem("Настройки игры");
            restartButton.Click += new EventHandler(restart);
            menu.Items.Add(restartButton);
            ToolStripMenuItem faq = new ToolStripMenuItem("Справка");
            faq.Click += (obj, eargs) => {
                if (faqIsOpen) return;
                FAQ fAQ = new FAQ();
                (fAQ).Show();
                faqIsOpen = true;
                fAQ.FormClosed += (obje, earg) => { this.faqIsOpen = false; };
            };
            menu.Items.Add(faq);
            this.Controls.Add(menu);


            init();
        }

        private void restart(object sender, EventArgs args)
        {
            openSettings();
        }

        private void openSettings()
        {
            Level level = new Level(this);
            level.Show();
            level.changeSizeAndBombCount += () =>
            {
                level.Close();
                init();
            };
        }

        public void init()
        {
            clickCount = 0;
            if(timerThread!=null)timerThread.Abort();
            if(checkerThread != null)checkerThread.Abort();
            timer.Text = "00:00";
            updateThisText();
            finishGame = true;
            emojiImage.Image = imgs.getAlive();
            firstClick = true;
            finishGame = false;
            flagCount = bombCout;
            updateFlagCountInfo(flagCount);
            pole.Controls.Clear();
            cells = new Cell[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    cells[i, j] = new Cell(new int[] { i, j });
                    genCell(i, j, cells[i, j]);
                }
        }

        private void genCell(int x, int y, Cell tag)
        {
            int allSize = poleSize / size;
            int ost = Math.Min(poleSize % size, 2);
            Button b = new Button()
            {
                BackColor = Color.White,
                Size = new Size(allSize - 1 - ost, allSize - 1 - ost),
                Location = new Point(x * allSize + 1 + ost, y * allSize + 1 + ost),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Gray,
                Tag = tag,
                TextAlign = ContentAlignment.TopCenter,
                Font = new Font(FontFamily.GenericSansSerif, Math.Max(0, allSize - 11), GraphicsUnit.Pixel)
            };
            b.MouseDown += new MouseEventHandler(click);
            ((Cell)b.Tag).openCell += () => current.Invoke(()=> {
                b.BackColor = ((Cell)b.Tag).getColor();
                if (((Cell)b.Tag).isNum()) b.Text = ((Cell)b.Tag).getType().ToString();
            });
            pole.Controls.Add(b);
        }

        private void click(object sender, MouseEventArgs args)
        {
            if (finishGame) return;
            if (sender is Control control)
                if (control.Tag is Cell cell)
                {
                    if (firstClick)
                    {
                        generateBomb(cell.getX(),cell.getY());
                        firstClick = false;
                        checker();
                    }
                    if (args.Button.Equals(MouseButtons.Left))
                    {
                        if (cell.getFlag()) return;
                        clickCount++;
                        if (!cell.isOpened()) openNear(cell);
                    }
                    else if (args.Button.Equals(MouseButtons.Middle)) {
                        if (!canDoAcrod(cell)) return;
                        openAcord(cell);
                        clickCount++;
                    }
                    else{
                        if (cell.isOpened()) return;
                        cell.toggleFlag();
                        if (sender is Button b) { 
                            b.Text = cell.getFlag() ? "F" : "";
                            flagCount += cell.getFlag() ? -1 : 1;
                            b.BackColor = cell.getFlag() ? Color.MediumVioletRed : Color.White;
                            updateFlagCountInfo(flagCount);
                        }
                    }

                }
        }

        private void generateBomb(int nX, int nY)
        {
            int count = 0;
            Random rnd = new Random();
            while (count < bombCout)
            {
                int x = rnd.Next(size);
                int y = rnd.Next(size);
                if (x == nX && y == nY) continue;
                if (cells[x, y].isBomb()) continue;
                cells[x, y].setType('b');
                for (int i = -1; i < 2; i++)
                    for (int j = -1; j < 2; j++)
                        setNear(x + i, y + j);
                count++;
            }
        }

        private void setNear(int x, int y)
        {
            try { Cell c = cells[x, y]; }
            catch { return; }
            cells[x, y].updateType();
        }

        private bool canDoAcrod(Cell cell)
        {
            if (!cell.isNum() || !cell.isOpened()) return false;
            int flagCountNear = Int32.Parse(cell.getType().ToString());
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    try
                    {
                        Cell cellToCheck = cells[cell.getX() + i, cell.getY() + j];
                        if (cellToCheck.getFlag()) flagCountNear--;
                    }
                    catch { continue; }
                    if (flagCountNear == 0) return true;
                }
            }
            return flagCountNear == 0;
        }

        private void openAcord(Cell cell)
        {
            for(int i = -1; i < 2; i++)
            {
                for(int j = -1; j< 2; j++)
                {
                    try
                    {
                        Cell cellToOpen = cells[cell.getX() + i, cell.getY() + j];
                        if (cellToOpen.getFlag()) continue;
                        if (cellToOpen.isOpened()) continue;
                        if (cellToOpen.isBomb())
                        {
                            openAllBomb();
                            return;
                        }
                        if (cellToOpen.isFree()) { 
                            openNear(cellToOpen);
                            continue;
                        }
                        cellToOpen.open();
                    }
                    catch { continue; }
                }
            }
        }

        private void openNear(Cell cell, int x = 0, int y = 0)
        {
            Task task = new Task(() =>
            {
                if (cell.getX() + x >= size || cell.getY() + y >= size || cell.getX() + x < 0 || cell.getY() + y < 0) return;
                if (cells[cell.getX() + x, cell.getY() + y].isOpened()) return;
                if (cells[cell.getX() + x, cell.getY() + y].getFlag()) return;
                cells[cell.getX() + x, cell.getY() + y].open();
                if (cells[cell.getX(), cell.getY()].isBomb())
                {
                    openAllBomb();
                    return;
                }
                if (cells[cell.getX() + x, cell.getY() + y].isNum()) return;

                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        openNear(cells[cell.getX() + x, cell.getY() + y], i, j);
                    }
                }
            });
            task.Start();
        }

        private void openAllBomb()
        {
            current.Invoke(() =>
            {
                finishGame = true;
                emojiImage.Image = imgs.getDie();
                foreach (Cell cell in cells)
                {
                    if (cell.isBomb()) cell.open();
                    else if (cell.getFlag()) cell.open();
                }
                DialogResult dialogResult = MessageBox.Show("Увы, вы проиграли.\nХотите начать с начала?", "Взрыв", MessageBoxButtons.YesNo);
                Console.WriteLine(dialogResult);
                if (dialogResult == DialogResult.Yes) init();
            });
        }

        private void checker()
        {
            checkerThread = new Thread(new ThreadStart(()=> {
                while (!finishGame)
                {
                    bool allOp = true;
                    foreach(Cell cell in cells)
                    {
                        if (!cell.isBomb()) allOp &= cell.isOpened();
                        else allOp &= cell.getFlag();
                        if (!allOp) break;
                    }
                    if (allOp) {
                        finishGame = true;
                        TimeSpan dT = DateTime.Now - startTime;
                        MessageBox.Show(String.Format("Вы смогли открыть поле за {0} клика.\nНа это вам понадобилось {1}m:{2}s:{3}ms", clickCount, dT.Minutes, dT.Seconds, dT.Milliseconds),"Вы выиграли!");
                        Thread.CurrentThread.Abort();
                    }
                    Thread.Sleep(10);
                }
                Thread.CurrentThread.Abort();
            }));
            checkerThread.Start();

            
            timerThread = new Thread(new ThreadStart(() => {
                startTime = DateTime.Now;
                while (!finishGame)
                {
                    Thread.Sleep(1000);
                    TimeSpan dT = DateTime.Now - startTime;
                    current.Invoke(()=> { timer.Text = String.Format("{0:00}:{1:00}",dT.Minutes,dT.Seconds); });
                }
                Console.WriteLine("Finish");
                current.Invoke(() =>{timer.Text = "00:00"; });
                Thread.CurrentThread.Abort();
            }));
            timerThread.Start();
        }

        public void updateFlagCountInfo(int count)
        {
            flagCountInfo.Text = count.ToString();
        }

        public void updateThisText()
        {
            this.Text = "Сапер "+ String.Format("(Количество бомб: {0}; Размеры поля: {1})", this.bombCout, this.size);
        }
    }
}
