using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace saper
{
    public partial class MainForm : Form
    {
        //Стандартное значение параметров игрового поля
        public const int DEFAULT_SIZE = 20;
        public const int DEFAULT_BOMB_COUNT = 20;

        private int poleSize = 600;

        //Элементы управления формы, которые обновляются при запуске новой игры
        private PictureBox emojiImage;
        private Label flagCountInfo;
        private Label timer;
        private Panel pole;

        //Параметры игрового поля
        private int bombCout = DEFAULT_BOMB_COUNT;
        private int size = DEFAULT_SIZE;

        //Методы для получения и установки параметров игрового поля
        public int getBombCount() { return this.bombCout; }
        public void setBombCount(int bc) { this.bombCout = bc; }
        public int getSize() { return this.size; }
        public void setSize(int s) { this.size = s; }

        //Игровые значения, которые требуеться обновлять при запуске новой гры
        private Cell[,] cells;
        private DateTime startTime = DateTime.Now;
        private bool firstClick = true;
        private bool finishGame = false;
        private int clickCount = 0;
        private int flagCount;

        //Переменная котороя хранит в себе значение, которое обозначает открыта ли справка
        public bool faqIsOpen = false;

        //Служба управления очердеди рабочих элементов для потока, в котором работает форма
        private Dispatcher current = Dispatcher.CurrentDispatcher;

        //Переменные обознацающие потоки, в которых работает таймер и проверка разминирования
        private Thread timerThread = null;
        private Thread checkerThread = null;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainFormLoad(object sender, EventArgs e)
        {
            this.Text = "Saper";
            this.Size = new Size(poleSize+37, poleSize+170);
            this.MaximizeBox = false;
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
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
            ToolStripMenuItem settingsButton = new ToolStripMenuItem("Настройки игры");
            settingsButton.Click += new EventHandler(settingsButtonClick);
            menu.Items.Add(settingsButton);
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

        //Обработчик события клика по кнопке настройки
        private void settingsButtonClick(object sender, EventArgs args)
        {
            openSettings();
        }

        //Метод, открывающий окно настроек
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

        //Сброс всех игровых значений, создание нового поля, с указаными настройками
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
            this.Update();
            List<Button> listOfCreatedButton = new List<Button>();
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    cells[i, j] = new Cell(new int[] { i, j });
                    listOfCreatedButton.Add(genCell(i, j, cells[i, j]));
                }
            pole.Controls.AddRange(listOfCreatedButton.ToArray());
        }

        //Метод создания новой кнопки-клетки
        private Button genCell(int x, int y, Cell tag)
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
            return b;
        }

        //Обработчик клика по кнопке-клетки
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
                        startUtilThread();
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

        //Алгоритм генерации бомб на поле
        //Заготовка для работы с сидом
        private void generateBomb(int nX, int nY, bool needSeed = false, int seed = int.MinValue)
        {
            int count = 0;
            if (!needSeed) seed = (int)(DateTime.Now.Ticks & 0xFFFFFFF);
            Random rnd = new Random(seed);
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

        public static int seedReader(string seed, out int seedSize, out int cpx, out int cpy)
        {
            int rndSeed = (int)(DateTime.Now.Ticks & 0xFFFFFFF);
            seedSize = 33 - seed[0];
            cpx = 33 - seed[1];
            cpy = 33 - seed[2];
            if (Int32.TryParse(seed.Remove(0, 3), out int seedIntPart)) rndSeed = seedIntPart;
            return rndSeed;
        }

        public static string seedPreparer(int seedSize,  int cpx, int cpy, int seedIntPart)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append((char)(33+seedSize));
            sb.Append((char)(33+cpx));
            sb.Append((char)(33+cpy));
            sb.Append(seedIntPart);
            return sb.ToString();
        }

        //Обновление типа клетки, если она имеется
        private void setNear(int x, int y)
        {
            if (x >= size || x < 0) return;
            if (y >= size || y < 0) return;
            cells[x, y].updateTypeToNumOrNextNum();
        }

        //Проверка возможности поведения аккорд, для клетки
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

        //Открытие аккордом
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

        //Метод, реализующий алгоритм открытия соседних клеток
        private void openNear(Cell cell, int x = 0, int y = 0)
        {
            TaskFactory taskFactory = new TaskFactory();
            taskFactory.StartNew(() =>
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
                return;
            });
            
        }

        //Открытие всех бомб
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

        //Запуск утилитарных потоков
        private void startUtilThread()
        {
            checkerThread = new Thread(new ThreadStart(()=> {
                while (!finishGame)
                {
                    bool allOp = true;
                    foreach(Cell cell in cells)
                    {
                        if (!cell.isBomb()) allOp &= cell.isOpened();
                        //else allOp &= cell.getFlag();
                        if (!allOp) break;
                    }
                    if (allOp) {
                        finishGame = true;
                        TimeSpan dT = DateTime.Now - startTime;
                        MessageBox.Show(
                            String.Format("Вы смогли открыть поле за {0} клика." +
                            "\nНа это вам понадобилось {1}m:{2}s:{3}ms", clickCount, dT.Minutes, dT.Seconds, dT.Milliseconds),"Вы выиграли!");
                        Thread.CurrentThread.Abort();
                    }
                    Thread.Sleep(20);
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
                Thread.CurrentThread.Abort();
            }));
            timerThread.Start();
        }

        //Обновление текста лейблов
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
