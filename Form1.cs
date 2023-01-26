using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GOLStartUp
{
    public partial class Form1 : Form
    {
        // The universe array
        bool[,] universe = new bool[15, 15];
        bool[,] scatchPad = new bool[15, 15];

        // mode finite or toroidal
        bool isToroidal = false;

        // heads up display
        bool displayHeadsUp = false;

        // Neighbor count display
        bool displayNeighbors = false;

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.DarkBlue;
        Brush TextBrush = new SolidBrush(Color.Red);

        // The Timer class
        Timer timer = new Timer();

        // The Random class
        Random random = new Random();

        // Generation count
        int generations = 0;

        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }

        // Calculate the next generation of cells
        private void NextGeneration()

        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int count = CountNeighbors(x, y);
                    scatchPad[x, y] = checkGameLifeRules(x, y, count);
                }
            }

            universe = scatchPad.Clone() as bool[,];

            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();

            graphicsPanel1.Invalidate();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                        if (displayNeighbors)
                        {
                            string ftext = CountNeighbors(x, y).ToString();
                            Console.WriteLine(cellRect.Width + "," + cellRect.Height);
                            PointF locc = new PointF(cellRect.X, cellRect.Y);
                            Font aral = new Font("Arial", 10);
                            e.Graphics.DrawString(ftext, aral, TextBrush, locc);
                        }
                    }



                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);

                }
            }
            if (displayHeadsUp)
            {
                PointF loc = new PointF(10, graphicsPanel1.ClientSize.Height - 100);
                Font aaral = new Font("Arial", 15);
                e.Graphics.DrawString(generateHeadsUpDisplay(), aaral, TextBrush, loc);
            }


            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private string generateHeadsUpDisplay()
        {
            string results =
                "Generation Number: " + generations + '\n' +
                "universe size: " + universe.GetLength(0) + ", " + universe.GetLength(1) + '\n' +
                "cell count: " + universe.GetLength(0) * universe.GetLength(1) + '\n' +
                "boundary type: ";

            if (isToroidal)
            {
                return results + "Toroidal";
            }
            return results + "Finite";
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        private int CountNeighbors(int x, int y)
        {
            int count = 0;

            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);


            if (isToroidal)
            {
                for (int yOffset = -1; yOffset <= 1; yOffset++)
                {
                    for (int xOffset = -1; xOffset <= 1; xOffset++)
                    {
                        int xCheck = x + xOffset;
                        int yCheck = y + yOffset;



                        // if xOffset and yOffset are both equal to 0 then continue
                        if (xOffset == 0 && yOffset == 0)
                        {
                            continue;
                        }


                        // if xCheck is less than 0 then continue
                        // if yCheck is less than 0 then continue
                        // if xCheck or yCheck is less than 0 then continue
                        if (xCheck < 0)
                        {
                            xCheck += xLen;
                        }
                        if (yCheck < 0)
                        {
                            yCheck += yLen;
                        }


                        // if xCheck is greater than or equal too xLen then continue
                        // if yCheck is greater than or equal too yLen then continue
                        if (xCheck >= xLen)
                        {
                            xCheck -= xLen;
                        }
                        if (yCheck >= yLen)
                        {
                            yCheck -= yLen;
                        }



                        if (universe[xCheck, yCheck] == true) count++;
                    }
                }
                return count;
            }

            else
            {
                for (int yOffset = -1; yOffset <= 1; yOffset++)
                {
                    for (int xOffset = -1; xOffset <= 1; xOffset++)
                    {
                        int xCheck = x + xOffset;
                        int yCheck = y + yOffset;



                        // if xOffset and yOffset are both equal to 0 then continue
                        if (xOffset == 0 && yOffset == 0)
                        {
                            continue;
                        }


                        // if xCheck is less than 0 then continue
                        // if yCheck is less than 0 then continue
                        // if xCheck or yCheck is less than 0 then continue
                        if (xCheck < 0 || yCheck < 0)
                        {
                            continue;
                        }

                        // if xCheck is greater than or equal too xLen then continue
                        // if yCheck is greater than or equal too yLen then continue
                        if (xCheck >= xLen || yCheck >= yLen)
                        {
                            continue;
                        }

                        if (universe[xCheck, yCheck] == true) count++;
                    }
                }
                return count;
            }



        }

        private bool checkGameLifeRules(int x, int y, int neighbors)
        {

            bool value = false;
            //Living cells with less than 2 living neighbors die in the next generation.
            //Living cells with 2 or 3 living neighbors live in the next generation.
            //Living cells with more than 3 living neighbors die in the next generation.
            if (universe[x, y] == true)
            {
                if (neighbors == 2 || neighbors == 3)
                {
                    value = true;
                }
            }
            //Dead cells with exactly 3 living neighbors live in the next generation.
            else
            {
                if (neighbors == 3)
                {
                    value = true;
                }
            }
            return value;

        }

        private void setSeed(int seed)
        {
            random = new Random(seed);
        }

        public void randomizeUniverse()
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (random.Next(0, 2) == 0)
                    {
                        universe[x, y] = false;
                    }
                    else
                    {
                        universe[x, y] = true;
                    }
                }
            }
            graphicsPanel1.Invalidate();

        }

        public void resetUniverse()
        // resets board to all be off/false
        {
            generations = 0;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (universe[x, y] == true)
                    {
                        universe[x, y] = !universe[x, y];
                    }


                }
            }
            graphicsPanel1.Invalidate();
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resetUniverse();

        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            //generations = 0;
            resetUniverse();
        }

        private void playToolStriButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void pauseToolStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        private void nextToolStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            NextGeneration();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            ModelDialog dlg = new ModelDialog();

            dlg.PromptText.Text = "what file name do you want to save";

            if (DialogResult.OK == dlg.ShowDialog())
            {
                string fileName = dlg.textBox1.Text + ".txt";
                StreamWriter writer = new StreamWriter(fileName);

                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    String currentRow = string.Empty;
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        if (universe[x, y] == true)
                        {
                            currentRow = currentRow + "O";
                        }
                        else if (universe[x, y] == false)
                        {
                            currentRow = currentRow + ".";
                        }
                    }
                    writer.WriteLine(currentRow);

                }
                writer.Close();

                toolStripStatusLabelGenerations.Text = "Successfully saved " + dlg.textBox1.Text;
            }
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            ModelDialog dlg = new ModelDialog();

            dlg.PromptText.Text = "what file name do you want to load";

            if (DialogResult.OK == dlg.ShowDialog())
            {
                int maxWidth = 0;
                int maxHeight = 0;
                string fileName = dlg.textBox1.Text + ".txt";
                StreamReader reader = new StreamReader(fileName);

                while (!reader.EndOfStream)
                {
                    string row = reader.ReadLine();

                    if (row[0] == '!')
                    {
                        continue;
                    }
                    if (row.Length > maxWidth)
                    {
                        maxWidth = row.Length;
                    }
                    maxHeight += 1;
                }

                universe = new bool[maxWidth, maxHeight];
                scatchPad = new bool[maxWidth, maxHeight];



                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                int yPos = 0;
                while (!reader.EndOfStream)
                {
                    string row = reader.ReadLine();

                    if (row[0] == '!')
                    {
                        continue;
                    }

                    for (int xPos = 0; xPos < row.Length; xPos++)
                    {
                        if (row[xPos] == 'O')
                        {
                            scatchPad[yPos, xPos] = true;
                        }

                    }
                    yPos++;
                }

                reader.Close();
                universe = scatchPad.Clone() as bool[,];
                toolStripStatusLabelGenerations.Text = "Successfully loaded " + dlg.textBox1.Text;
                graphicsPanel1.Invalidate();

            }
        }

        private void gridSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModelDialog dlg = new ModelDialog();

            dlg.PromptText.Text = "changing grid dimensions (x,y)";

            if (DialogResult.OK == dlg.ShowDialog())
            {
                int x = Int16.Parse(dlg.textBox1.Text.Split(',')[0]);
                int y = Int16.Parse(dlg.textBox1.Text.Split(',')[1]);

                universe = new bool[x, y];
                scatchPad = new bool[x, y];

                toolStripStatusLabelGenerations.Text = "Successfully resized to " + dlg.textBox1.Text;
                graphicsPanel1.Invalidate();
            }
        }

        private void timeIntervalsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModelDialog dlg = new ModelDialog();

            dlg.PromptText.Text = "changing time interval, currently at " + timer.Interval;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                timer.Interval = Int16.Parse(dlg.textBox1.Text);

                toolStripStatusLabelGenerations.Text = "Successfully changed interval to " + timer.Interval;
                graphicsPanel1.Invalidate();
            }
        }

        private void neighborCountDisplayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            displayNeighbors = !displayNeighbors;
            toolStripStatusLabelGenerations.Text = "Successfully Toggled Neighbors Display to : " + displayNeighbors;
            graphicsPanel1.Invalidate();
        }

        private void headsUpDisplayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            displayHeadsUp = !displayHeadsUp;
            toolStripStatusLabelGenerations.Text = "Successfully Toggled Heads Up Display to : " + displayHeadsUp;
            graphicsPanel1.Invalidate();
        }

        private void boundaryTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            isToroidal = !isToroidal;
            if (isToroidal)
            {
                toolStripStatusLabelGenerations.Text = "Successfully Toggled Boundary Type to: Toroidal";
            }
            else
            {
                toolStripStatusLabelGenerations.Text = "Successfully Toggled Boundary Type to: Finite";
            }
        }

        private void setSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModelDialog dlg = new ModelDialog();

            dlg.PromptText.Text = "changing seed";

            if (DialogResult.OK == dlg.ShowDialog())
            {
                random = new Random(Int16.Parse(dlg.textBox1.Text));


                toolStripStatusLabelGenerations.Text = "Successfully changed seed to " + dlg.textBox1.Text;
                graphicsPanel1.Invalidate();
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            randomizeUniverse();
        }

        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cdlg = new ColorDialog();

            if (cdlg.ShowDialog() == DialogResult.OK)
            {
                gridColor = cdlg.Color;
                toolStripStatusLabelGenerations.Text = "Successfully changed grid color to " + cdlg.Color.Name;
                graphicsPanel1.Invalidate();
            }
        }

        private void tilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cdlg = new ColorDialog();

            if (cdlg.ShowDialog() == DialogResult.OK)
            {
                cellColor = cdlg.Color;
                toolStripStatusLabelGenerations.Text = "Successfully changed cell color to " + cdlg.Color.Name;
                graphicsPanel1.Invalidate();
            }
        }

        private void textToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cdlg = new ColorDialog();

            if (cdlg.ShowDialog() == DialogResult.OK)
            {
                TextBrush = new SolidBrush(cdlg.Color);
                toolStripStatusLabelGenerations.Text = "Successfully changed text color to " + cdlg.Color.Name;
                graphicsPanel1.Invalidate();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveToolStripButton_Click(sender, e);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openToolStripButton_Click(sender, e);
        }
    }
}
