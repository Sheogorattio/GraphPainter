using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Data;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using NFun;

namespace GraphPainter
{
    public partial class Form1 : Form
    {
        private Graphics g;
        private Pen gridPen;
        private Pen graphPen;
        private Bitmap bmp;
        private Matrix transformationMatrix;

        private int cellSize = 20;
        private Point origin;

        public Form1()
        {
            InitializeComponent();
            InitializeGraphics();
            button1.Click += Button1_Click;
            MessageBox.Show("+\t\tAdds two operands. A+B=10\r\n−\t\tSubtracts the second operand from the first. B−A=-2\r\n*\t\tMultiplies both operands. A*B=24\r\n%\t\tModulo operator - remainder of the division. A%B=2\r\n**\t\tRaises A to the power of B. A**B=1296\r\n− (unary)\t\tMultiply the expression by -1. −(A)=-6");
        }

        private void InitializeGraphics()
        {
            pictureBox1.Height = 400;
            pictureBox1.Width = 480;

            origin = new Point(pictureBox1.Width / 2, pictureBox1.Height / 2);
            transformationMatrix = new Matrix();
            transformationMatrix.Translate(origin.X, origin.Y);
            transformationMatrix.Scale(1, -1);

            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bmp;

            g = Graphics.FromImage(bmp);
            g.Transform = transformationMatrix;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            gridPen = new Pen(Brushes.Black, 1);
            graphPen = new Pen(Brushes.Blue, 1);

            RedrawGraph();
            pictureBox1.Refresh();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            RedrawGraph();
            DrawGraph(textBox1.Text);
            pictureBox1.Refresh();
        }

        private void RedrawGraph()
        {
            g.Clear(BackColor);
            DrawGrid();
            g.FillEllipse(Brushes.Red, -3, -3, 6, 6);

        }

        private void DrawGrid()
        {
            for (int x = (int)-origin.X; x <= origin.X; x += cellSize)
            {
                g.DrawLine(gridPen, x, -origin.Y, x, origin.Y);
            }

            for (int y = (int)-origin.Y; y <= origin.Y; y += cellSize)
            {
                g.DrawLine(gridPen, -origin.X, y, origin.X, y);
            }
        }

        private void DrawGraph(string function)
        {
            StringBuilder sb = new StringBuilder(function);
            sb.Remove(0, 2);
            StringBuilder var_sb = new StringBuilder(sb.ToString());
            // Парсинг функции и отрисовка графика здесь
            DataTable table = new DataTable();
            DataColumn x = new DataColumn("x", typeof(double));
            DataColumn y = new DataColumn("y", typeof(double));
            table.Columns.Add(x);
            table.Columns.Add(y);

            double xValue, yValue;
            for (xValue = -origin.X; xValue <= origin.X; xValue += 1)
            {
               
                DataRow row = table.NewRow();
                row["x"] = xValue;
                try
                {
                    // Парсим функцию и вычисляем значение для текущего x
                    var_sb.Replace("x", xValue.ToString());
                    //yValue = Convert.ToDouble(table.Compute(var_sb.ToString(),""));
                    yValue = Convert.ToDouble(Funny.Calc(var_sb.ToString()));
                    row["y"] = Math.Round(yValue,4);
                    var_sb.Clear();
                    var_sb.Append(sb.ToString());
                }
                catch
                {
                    row["y"] = DBNull.Value;
                }
                table.Rows.Add(row);
            }

            // Рисуем график
            double x1, x2, y1, y2;
            try
            {
                
                for (int i = 0; i < table.Rows.Count - 1; i++)
                {

                    x1 = Convert.ToDouble(table.Rows[i]["x"]);


                    if (table.Rows[i]["y"].ToString()[0] == 45 || table.Rows[i]["y"].ToString()[0] <= 57 && table.Rows[i]["y"].ToString()[0] >= 48)
                    {
                        y1 = Convert.ToDouble(table.Rows[i]["y"]);
                    }
                    else continue;


                    x2 = Convert.ToDouble(table.Rows[i + 1]["x"]);

                    if (table.Rows[i]["y"].ToString()[0] == 45 || table.Rows[i + 1]["y"].ToString()[0] <= 57 && table.Rows[i + 1]["y"].ToString()[0] >= 48)
                    {
                        y2 = Convert.ToDouble(table.Rows[i + 1]["y"]);
                    }
                    else continue;


                    g.DrawLine(graphPen, (float)x1, (float)y1, (float)x2, (float)y2);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
