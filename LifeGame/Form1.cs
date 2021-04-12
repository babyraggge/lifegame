using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LifeGame
{
    public partial class Form1 : Form
    {
        private Graphics graphics;
        private int resolution;
        private bool[,] field;
        private int rows;
        private int cols;
        private int currentGeneration;
        private Random rnd;

        public Form1()
        {
            InitializeComponent();
        }

        private void GameStart()
        {
            if (timer1.Enabled) return;

            currentGeneration = 0;
            Text = $"Current generation: {currentGeneration}";

            numericUpDown_Resolution.Enabled = false;
            numericUpDown_Density.Enabled = false;

            resolution = (int)numericUpDown_Resolution.Value;
            rows = pictureBox1.Height / resolution;
            cols = pictureBox1.Width / resolution;
            field = new bool[cols,rows];

            rnd = new Random();

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    field[x, y] = rnd.Next((int)numericUpDown_Density.Value) == 0;
                }
            }

            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(pictureBox1.Image);
            timer1.Start();
        }

        private void StopGame()
        {
            if (!timer1.Enabled) return;

            timer1.Stop();

            numericUpDown_Resolution.Enabled = true;
            numericUpDown_Density.Enabled = true;
        }

        private void NextGeneration()
        {
            graphics.Clear(Color.Black);
            var NewField = new bool[cols, rows];

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    var neighboursCount = CountNeighbours(x, y);
                    var hasLife = field[x, y];

                    if (!hasLife && neighboursCount == 3)                    
                        NewField[x, y] = true;
                    
                    else if (hasLife && (neighboursCount < 2 || neighboursCount > 3))                    
                        NewField[x, y] = false;
                    
                    else                    
                        NewField[x, y] = field[x, y];
                    
                    if (hasLife) 
                        graphics.FillRectangle(Brushes.Crimson, x*resolution, y*resolution, resolution, resolution);                                    
                }
            }
            field = NewField;
            Text = $"Current generation: {currentGeneration++}";
            pictureBox1.Refresh();
        }

        private int CountNeighbours(int x, int y)
        {
            int count = 0;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int col = (x + i + cols) % cols;
                    int row = (y + j + rows) % rows;

                    bool isSelfCecking = col == x && row == y;
                    bool hasLife = field[col, row];

                    if (!isSelfCecking && hasLife) 
                        count++;
                        
                }
            }

            return count;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            GameStart();
        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            StopGame();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!timer1.Enabled)
                return;

            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    var x = e.Location.X / resolution;
                    var y = e.Location.Y / resolution;
                    field[x, y] = false;
                }

                if (e.Button == MouseButtons.Right)
                {
                    var x = e.Location.X / resolution;
                    var y = e.Location.Y / resolution;
                    field[x, y] = true;
                }
            }
            catch { }

        }
    }
}
