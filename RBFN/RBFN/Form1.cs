using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace RBFN
{
    public partial class Form1 : Form
    {
        int xx, yy;
        int counterA = 0;
        int counterB = 0;
        int count = 1;

        double[,] A = new double[50, 3];
        double[,] B = new double[50, 3];
        double[,] mean = new double[6, 5];
        double sigma;
        double[,] newA = new double[50, 3];
        double[,] newB = new double[50, 3];
        double[] targetTest = new double[50];
        double w1 = 0.5, w2 = 0.5;
        double learning_rate = 1;
        double thdis = 0.7;

        public Form1()
        {
            InitializeComponent();

            foreach (Control c in this.Controls)
            {
                c.MouseDown += ShowMouseDown;
            }

          
            //this.MouseDown += (s, e) => { this.label1.Text = e.X + " " + e.Y; };
        }

        //record all points in the graph
        private void ShowMouseDown(object sender, MouseEventArgs e)
        {
            var x = e.X;// ((Control)sender).Left;
            var y = e.Y;// + ((Control)sender).Top;

            xx = x;
            yy = y;
            //this.label1.Text = x + "," + y;
            double X = (double)(xx - 290.00) / 40;
            double Y = (double)(290.00 - yy) / 40;

            //don't record points that are out of range
            //while ((X < 6 && X > -6) && (Y < 6 && Y > -6))// && counterA + counterB <= 50
            {
                //prevent recording too many points
                if (counterA + counterB >= 50)
                {
                    MessageBox.Show("超過可以記錄的點的個數了!" + Environment.NewLine + "請停止輸入!");
                    //break;
                }

                if (clickcolor == 1)
                {
                    //textBox3.AppendText("(" + X + "," + Y + ")");
                    A[counterA, 0] = X;
                    A[counterA, 1] = Y;
                    //A[counterA, 2] = (X + 6) * (X + 6) + (Y + 6) * (Y + 6);
                    //this.label3.Text = A[counterA, 2].ToString();
                    textBox1.AppendText(count + ": ( " + X + " , " + Y + " )" + Environment.NewLine);//show points 
                    count++;
                    counterA++;
                    //break;
                }
                else if (clickcolor == 2)
                {
                    B[counterB, 0] = X;
                    B[counterB, 1] = Y;
                    //B[counterB, 2] = (X + 6) * (X + 6) + (Y + 6) * (Y + 6);
                    //this.label3.Text = B[counterB, 2].ToString();
                    textBox1.AppendText(count+ ": ( " + X + " , " + Y + " )" + Environment.NewLine);//show points
                    count++;
                    counterB++;
                    //break;
                }

            }

           
            //this.label2.Text = X + "," + Y;
        }

        //draw points where you click
        private void chart1_Click(object sender, EventArgs e)
        {
            Graphics gra = chart1.CreateGraphics();

            if (clickcolor == 1)
            {
                Pen mypen = new Pen(Color.Red, 2);
                gra.DrawRectangle(mypen, xx, yy, 6, 6);
            }
            if (clickcolor == 2)
            {
                Pen mypen = new Pen(Color.Blue, 2);
                gra.DrawRectangle(mypen, xx, yy, 6, 6);
            }
        }

        //clear color 1
        int clickcolor = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("請先在圖表上畫出要運算的點，" + Environment.NewLine + "然後計算出center，" + Environment.NewLine + "再來請輸入sigma值，" + Environment.NewLine + "就可以算出在新的空間中的點(Transfer)，" + Environment.NewLine + "最後請按下 START 即會畫出分割線!");
            clickcolor = 1;
            if (counterA == 0)
            {
                textBox1.AppendText("choosing RED" + Environment.NewLine);
            }
            //textBox1.AppendText("counterA=" + counterA);
        }

        //clear color 2
        private void button2_Click(object sender, EventArgs e)
        {
            clickcolor = 2;
            if (counterA != 0)
            {
                textBox1.AppendText("choosing BLUE" + Environment.NewLine);
            }
        }

        //calculate centers
        private void button3_Click(object sender, EventArgs e)
        {
            counterA--;
            counterB--;
            clickcolor = 0;

            //for (int i = 0; i < counterA; i++)
            //{
            //    textBox3.AppendText("(" + A[i, 0] + "," + A[i, 1] + ")");
            //}


            int count1A = 0;
            int count2A = 0;
            int count3A = 0;
            int count4A = 0;
            clickcolor = 0;
            for (int i = 0; i < counterA; i++)
            {
                if (A[i, 0] >= 0 && A[i, 1] >= 0)//第一象限
                {
                    //class1A[count1A, 0] = A[i, 0];
                    //class1A[count1A, 1] = A[i, 1];
                    count1A++;
                    mean[0, 1] += A[i, 0];
                    mean[1, 1] += A[i, 1];

                }
                else if (A[i, 0] < 0 && A[i, 1] > 0)
                {
                    //class2A[count2A, 0] = A[i, 0];
                    //class2A[count2A, 1] = A[i, 1];
                    count2A++;
                    mean[0, 2] += A[i, 0];
                    mean[1, 2] += A[i, 1];
                }
                else if (A[i, 0] <= 0 && A[i, 1] <= 0)
                {
                    //class3A[count3A, 0] = A[i, 0];
                    //class3A[count3A, 1] = A[i, 1];
                    count3A++;
                    mean[0, 3] += A[i, 0];
                    mean[1, 3] += A[i, 1];
                }
                else if (A[i, 0] > 0 && A[i, 1] < 0)
                {
                    //class4A[count4A, 0] = A[i, 0];
                    //class4A[count4A, 1] = A[i, 1];
                    count4A++;
                    mean[0, 4] += A[i, 0];
                    mean[1, 4] += A[i, 1];
                }
            }
            int count1B = 0;
            int count2B = 0;
            int count3B = 0;
            int count4B = 0;

            for (int i = 0; i < counterB; i++)
            {
                if (B[i, 0] >= 0 && B[i, 1] >= 0)
                {
                    //class1B[count1B, 0] = B[i, 0];
                    //class1B[count1B, 1] = B[i, 1];
                    count1B++;
                    mean[2, 1] += B[i, 0];
                    mean[3, 1] += B[i, 1];

                }
                else if (B[i, 0] < 0 && B[i, 1] > 0)
                {
                    //class2B[count2B, 0] = B[i, 0];
                    //class2B[count2B, 1] = B[i, 1];
                    count2B++;
                    mean[2, 2] += B[i, 0];
                    mean[3, 2] += B[i, 1];
                }
                else if (B[i, 0] <= 0 && B[i, 1] <= 0)
                {
                    //class3B[count3B, 0] = B[i, 0];
                    //class3B[count3B, 1] = B[i, 1];
                    count3B++;
                    mean[2, 3] += B[i, 0];
                    mean[3, 3] += B[i, 1];
                }
                else if (B[i, 0] > 0 && B[i, 1] < 0)
                {
                    //class4B[count4B, 0] = B[i, 0];
                    //class4B[count4B, 1] = B[i, 1];
                    count4B++;
                    mean[2, 4] += B[i, 0];
                    mean[3, 4] += B[i, 1];
                }

            }
            mean[4, 1] = count1A;
            mean[4, 2] = count2A;
            mean[4, 3] = count3A;
            mean[4, 4] = count4A;
            mean[5, 1] = count1B;
            mean[5, 2] = count2B;
            mean[5, 3] = count3B;
            mean[5, 4] = count4B;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 1; j < 5; j++)
                {
                    if (i < 2 && mean[4, j] != 0)//A                  
                        mean[i, j] = mean[i, j] / mean[4, j];

                    else if (i >= 2 && mean[5, j] != 0)//B
                        mean[i, j] = mean[i, j] / mean[5, j];
                }
            }

            chart1.Series["NodeA"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            chart1.Series["NodeB"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            chart1.Series["NodeC"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;

            for (int i = 0; i < counterA; i++)
            {
                chart1.Series["NodeB"].Points.Add(new DataPoint(A[i, 0], A[i, 1]));

            }
            for (int i = 0; i < counterB; i++)
            {
                chart1.Series["NodeC"].Points.Add(new DataPoint(B[i, 0], B[i, 1]));

            }

            int countxor = 0;
            for (int i = 1; i <= 4; i++)
            {
                //textboxact.AppendText("A(" + mean[0, i] + "," + mean[1, i] + ")" + "B(" + mean[2, i] + "," + mean[3, i] + ")");

                if (mean[0, i] == 0 && mean[1, i] == 0)
                {
                    if (mean[2, i] == 0 && mean[3, i] == 0)
                        countxor++;
                }


            }

            //textboxact.AppendText("counterxor=" + countxor + Environment.NewLine);
            if (countxor == 3)
            {
                //textboxact.AppendText("xor" + Environment.NewLine);
                for (int i = 1; i <= 4; i++)
                {
                    if (mean[0, i] != 0 && mean[1, i] != 0)
                    {
                        mean[0, i] = B[0, 0];
                        mean[1, i] = B[0, 1];
                        mean[2, i] = B[counterB - 1, 0];
                        mean[3, i] = B[counterB - 1, 1];
                        //textboxact.AppendText("counterA=" + counterA + "counterB" + counterB + Environment.NewLine);
                    }

                }
            }

            for (int i = 1; i <= 4; i++)
            {
                chart1.Series["NodeA"].Points.Add(new DataPoint(mean[0, i], mean[1, i]));
                //textboxact.AppendText("(" + mean[0, i] + " , " + mean[1, i] + ")" + Environment.NewLine);
                chart1.Series["NodeA"].Points.Add(new DataPoint(mean[2, i], mean[3, i]));
                //textboxact.AppendText("(" + mean[2, i] + " , " + mean[3, i] + ")" + Environment.NewLine);
            }

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        //transfer point to new space with the calculated center
        private void button4_Click(object sender, EventArgs e)
        {
            string temp;
            double Q;
            temp = textBox2.Text;
            sigma = double.Parse(temp);

            if (sigma != 0)
            {
                Q = -(1 / sigma);//參數


                for (int i = 0; i < counterA; i++)
                {
                    for (int j = 1; j <= 4; j++)
                    {
                        if (mean[0, j] != 0 || mean[1, j] != 0)//centerA不存在
                        {
                            newA[i, 0] = newA[i, 0] + 2 * ((double)Math.Exp((Math.Sqrt(Math.Pow((A[i, 0] - mean[0, j]), 2) + Math.Pow((A[i, 1] - mean[1, j]), 2)) * Q)));
                        }
                        if (mean[2, j] != 0 || mean[3, j] != 0)//centerB不存在
                        {
                            newA[i, 1] = newA[i, 1] + 2 * ((double)Math.Exp((Math.Sqrt(Math.Pow((A[i, 0] - mean[2, j]), 2) + Math.Pow((A[i, 1] - mean[3, j]), 2)) * Q)));
                        }
                    }
                    newA[i, 2] = newA[i, 0] * newA[i, 0] + newA[i, 1] * newA[i, 1];
                    textboxact.AppendText("RED-"+ (i+1) + " : ( " + newA[i, 0] + " , " + newA[i, 1] + ")" + Environment.NewLine);
                }
                for (int i = 0; i < counterB; i++)
                {
                    for (int j = 1; j <= 4; j++)
                    {
                        if (mean[0, j] != 0 || mean[1, j] != 0)
                        {
                            newB[i, 0] = newB[i, 0] + 2 * ((double)Math.Exp((Math.Sqrt(Math.Pow((B[i, 0] - mean[0, j]), 2) + Math.Pow((B[i, 1] - mean[1, j]), 2)) * Q)));
                        }
                        if (mean[2, j] != 0 || mean[3, j] != 0)
                        {
                            newB[i, 1] = newB[i, 1] + 2 * ((double)Math.Exp((Math.Sqrt(Math.Pow((B[i, 0] - mean[2, j]), 2) + Math.Pow((B[i, 1] - mean[3, j]), 2)) * Q)));
                        }
                    }
                    newB[i, 2] = newB[i, 0] * newB[i, 0] + newB[i, 1] * newB[i, 1];
                    textboxact.AppendText("BLUE-" + (i + 1) + " : ( " + newB[i, 0] + " , " + newB[i, 1] + ")" + Environment.NewLine);
                }

                foreach (var series in chart1.Series)
                {
                    series.Points.Clear();
                }

                for (int i = 0; i < counterA; i++)
                {
                    chart1.Series["NodeB"].Points.Add(new DataPoint(newA[i, 0], newA[i, 1]));


                }

                for (int i = 0; i < counterB; i++)
                {
                    chart1.Series["NodeC"].Points.Add(new DataPoint(newB[i, 0], newB[i, 1]));

                }

            }
        }

        //clear all inputs
        private void button5_Click(object sender, EventArgs e)
        {

            chart1.Series.Clear();
            textboxact.Clear();
            textBox2.Clear();
            textBox1.Clear();
            counterA = 0;
            counterB = 0;
            count = 1;
            clickcolor = 0;
            chart1.Series.Add("Line");
            chart1.Series["Line"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series["Line"].Points.AddXY(0, 0);


            chart1.Series.Add("NodeA");
            chart1.Series.Add("NodeB");
            chart1.Series.Add("NodeC");
            chart1.Series.Add("NodeD");
            chart1.Series.Add("LineResult");
            chart1.Series.Add("NodeE");
       
            for (int i = 0; i < 50; i++)
            {
                A[i, 0] = 0;
                A[i, 1] = 0;
                A[i, 2] = 0;
                B[i, 0] = 0;
                B[i, 1] = 0;
                B[i, 2] = 0;
                newA[i, 0] = 0;
                newA[i, 1] = 0;
                newB[i, 0] = 0;
                newB[i, 1] = 0;
            }
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    mean[i, j] = 0;
                }
            }
        }

        Random rnd = new Random();

        protected int GetRandomInt(int min, int max)
        {
            return rnd.Next(min, max);
        }

        //draw line here
        private void button6_Click(object sender, EventArgs e)
        {
            double maxB = 0, maxA = 0;
            int maxnumB = 0, maxnumA = 0;
            double minB = 10, minA = 10;
            int minnumA = 0, minnumB = 0;
            for (int i = 0; i < counterA; i++)
            {
                if (newA[i, 1] > maxA)//A_ymax
                {
                    maxA = newA[i, 1];
                    maxnumA = i;
                }
                if (newA[i, 2] < minA)
                {
                    minA = newA[i, 2];
                    minnumA = i;
                }
            }

            for (int i = 0; i < counterB; i++)//B_xmax
            {
                if (newB[i, 0] > maxB)
                {
                    maxB = newB[i, 0];
                    maxnumB = i;

                }

            }
            for (int i = 0; i < counterB; i++)
            {
                if (newB[i, 2] < minB && i != maxnumB)
                {
                    minB = newB[i, 2];
                    minnumB = i;
                }
            }

            double linemean1x = (newA[maxnumA, 0] + newB[maxnumB, 0]) / 2;
            double linemean1y = (newA[maxnumA, 1] + newB[maxnumB, 1]) / 2;
            double linemean2x = (newA[minnumA, 0] + newB[minnumB, 0]) / 2;
            double linemean2y = (newA[minnumA, 1] + newB[minnumB, 1]) / 2;

            chart1.Series["NodeD"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            chart1.Series["NodeE"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            //chart1.Series["NodeD"].Points.Add(new DataPoint(linemean1x, linemean1y));
            //chart1.Series["NodeE"].Points.Add(new DataPoint(linemean2x, linemean2y));

            chart1.Series["LineResult"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            double shelui = (linemean2y - linemean1y) / (linemean2x - linemean1x);//斜率唷
            double k = linemean1y - shelui * linemean1x;
            chart1.Series["LineResult"].Points.AddXY(6, 6 * shelui + k);
            chart1.Series["LineResult"].Points.AddXY(-6, -6 * shelui + k);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chart1.ChartAreas.Add("area");
            chart1.ChartAreas["area"].AxisX.Minimum = -6;
            chart1.ChartAreas["area"].AxisX.Maximum = 6;
            chart1.ChartAreas["area"].AxisX.Interval = 2;

            chart1.ChartAreas["area"].AxisY.Minimum = -6;
            chart1.ChartAreas["area"].AxisY.Maximum = 6;
            chart1.ChartAreas["area"].AxisY.Interval = 2;

            chart1.Series.Add("Line");
            chart1.Series["Line"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series["Line"].Points.AddXY(0, 0);

            chart1.Series.Add("NodeA");
            chart1.Series.Add("NodeB");
            chart1.Series.Add("NodeC");

            chart1.Series.Add("NodeD");

            chart1.Series.Add("LineResult");
            chart1.Series.Add("NodeE");

            

        }
    }
}
