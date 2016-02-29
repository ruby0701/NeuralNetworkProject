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





namespace TLU
{
    public partial class Form1 : Form
    {
        int xx, yy;
        double[,] A = new double[10, 3];
        double[,] B = new double[10, 3];
        int counterA = 0;
        int counterB = 0;
        double thx;//thresholdX
        double thy;//thresholdY
        double thdis;//threshold
        double w1 = 1, w2 = 1, constraint = 1;
        int bound = 0;
        double[] tr_X1 = new double[20];
        double[] tr_X2 = new double[20];
        double[] tr_output = new double[20];
        double learning_rate;
        int final = 0;//successed?

        public Form1()
        {
            InitializeComponent();

            foreach (Control c in this.Controls)
            {
                c.MouseDown += ShowMouseDown;
            }

            //this.MouseDown += (s, e) => { this.label1.Text = e.X + " " + e.Y; };
        }

        private void ShowMouseDown(object sender, MouseEventArgs e)
        {
            var x = e.X;// ((Control)sender).Left;
            var y = e.Y;// + ((Control)sender).Top;

            xx = x;
            yy = y;
            //this.label1.Text = x + "," + y;
            double X = (double)(xx - 290.00) / 40;
            double Y = (double)(290.00 - yy) / 40;
            if (clickcolor == 1)
            {

                A[counterA, 0] = X;
                A[counterA, 1] = Y;
                A[counterA, 2] = (X + 6) * (X + 6) + (Y + 6) * (Y + 6);
                //this.label3.Text = A[counterA, 2].ToString();

                counterA++;
            }
            else if (clickcolor == 2)
            {
                B[counterB, 0] = X;
                B[counterB, 1] = Y;
                B[counterB, 2] = (X + 6) * (X + 6) + (Y + 6) * (Y + 6);
                //this.label3.Text = B[counterB, 2].ToString();

                counterB++;
            }
            //this.label2.Text = X + "," + Y;
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

            chart1.Series.Add("LineResult");
            chart1.Series.Add("NodeA");
            chart1.Series.Add("NodeB");
            chart1.Series.Add("NodeC");
            button3.Enabled = false;
            textBox1.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            textBox5.Enabled = false;
            textBox6.Enabled = false;
        }
        int clickcolor = 0;
        private void button1_Click(object sender, EventArgs e)//red button
        {
            clickcolor = 1;
        }
        private void button2_Click_1(object sender, EventArgs e)//blue button
        {
            clickcolor = 2;
        }

        private void button5_Click(object sender, EventArgs e)
        //done,避免counterAB多算，所以設done>>點輸入完畢，會計算threshold
        {
            clickcolor = 0;

            button3.Enabled = true;
            textBox1.Enabled = true;
            textBox3.Enabled = true;
            textBox4.Enabled = true;
            textBox5.Enabled = true;
            textBox6.Enabled = true;

            counterA--;
            counterB--;
            
            double dmaxA = A[0, 2];
            int dmaxAi = 0;
            double dminA = A[0, 2];
            int dminAi = 0;
            tr_X1[0] = A[0, 0];//first node x
            tr_X2[0] = A[0, 1];//first node y

            //找出Threshold;
            for (int i = 1; i < counterA; i++)
            {
                tr_X1[i] = A[i, 0];//x
                tr_X2[i] = A[i, 1];//y

                if (A[i, 2] > dmaxA)
                {
                    dmaxA = A[i, 2];
                    dmaxAi = i;
                }

                if (A[i, 2] < dminA)
                {
                    dminA = A[i, 2];
                    dminAi = i;
                }

            }

            double dmaxB = B[0, 2];
            int dmaxBi = 0;
            double dminB = B[0, 2];
            int dminBi = 0;
            tr_X1[counterA] = B[0, 0];
            tr_X2[counterA] = B[0, 1];

            for (int i = 1; i < counterB; i++)
            {
                tr_X1[counterA + i] = B[i, 0];
                tr_X2[counterA + i] = B[i, 1];

                if (B[i, 2] > dmaxB)
                {
                    dmaxB = B[i, 2];
                    dmaxBi = i;
                }

                if (B[i, 2] < dminB)
                {
                    dminB = B[i, 2];
                    dminBi = i;
                }
            }

            //value of input
            textBox2.AppendText("Class Red : " + "\n");
            for (int i = 0; i < counterA;i++) 
            {
                textBox2.AppendText("("+ A[i, 0] + " , " + A[i, 1]+")\n");
            }

            textBox2.AppendText("Class Blue : " + "\n");
            for (int i = 0; i < counterB; i++)
            {
                textBox2.AppendText("(" + B[i, 0] + " , " + B[i, 1] + ")\n");
            }
               

            if ((dmaxA - dminB) * (dmaxA - dminB) > (dminA - dmaxB) * (dminA - dmaxB))//紅在上
            {
                thx = (A[dminAi, 0] + B[dmaxBi, 0]) / 2;
                thy = (A[dminAi, 1] + B[dmaxBi, 1]) / 2;
                
                //this.label1.Text = "紅在上";
                //red=1,blue=0
                for (int i = 0; i < counterA; i++)
                {
                    tr_output[i] = 1;
                }
                for (int i = 0; i < counterB; i++)
                {
                    tr_output[counterA+i] = 0;////////////////////////////////////////////
                }

            }
            else if ((dmaxA - dminB) * (dmaxA - dminB) < (dminA - dmaxB) * (dminA - dmaxB))//藍在上
            {
                thx = (B[dminBi, 0] + A[dmaxAi, 0]) / 2;
                thy = (B[dminBi, 1] + A[dmaxAi, 1]) / 2;
                thdis = Math.Sqrt(thx * thx + thy * thy);
                //this.label1.Text = "藍在上";
                //blue=1,red=0;
                for (int i = 0; i < counterA; i++)
                {
                    tr_output[i] = 0;
                }
                for (int i = 0; i < counterB; i++)
                {
                    tr_output[counterA+i] = 1;
                }

            }
            //************************************************************************
            //this.label4.Text = thx + "," + thy;
            textBox1.Text = thdis.ToString();
            //************************************************************************
            //draw point again

            chart1.Series["NodeA"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            chart1.Series["NodeB"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            chart1.Series["NodeC"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            for (int i = 0; i < counterA; i++)
            {
                chart1.Series["NodeA"].Points.Add(new DataPoint(A[i, 0], A[i, 1]));

            }
            for (int i = 0; i < counterB; i++)
            {
                chart1.Series["NodeB"].Points.Add(new DataPoint(B[i, 0], B[i, 1]));

            }
            chart1.Series["NodeC"].Points.Add(new DataPoint(thx, thy));

            //********************************************************************************
            for (int i = 0; i < 10; i++)
            {
                //textBox2.AppendText("\n" + "**" + i + ":" + tr_X1[i].ToString() + "+" + tr_X2[i].ToString());
            }
            //********************************************************************************
        }

        private void button4_Click(object sender, EventArgs e)//reset button
        {
           
            //clear all text
            textBoxact.Clear();
            textBoxw.Clear();
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();

            //clear chart
            chart1.Series.Clear();
            counterA = 0;
            counterB = 0;
            //new chart
            chart1.Series.Add("Line");
            chart1.Series["Line"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series["Line"].Points.AddXY(0, 0);

            chart1.Series.Add("LineResult");
            chart1.Series.Add("NodeA");
            chart1.Series.Add("NodeB");
            chart1.Series.Add("NodeC");

            //reset all variables
            button3.Enabled = false;
            textBox1.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            textBox5.Enabled = false;
            textBox6.Enabled = false;
            bound = 0;
            constraint = 0;
            thdis = 0;
            w1 = 0;
            w2 = 0;
            clickcolor = 0;
            for (int i = 0; i < 20; i++)
            {
                tr_X1[i] = 0;
                tr_X2[i] = 0;
            }


        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)//start button
        {
            //get all the values TLU need
            string temp;

            temp = textBox3.Text;
            w1 = float.Parse(temp);

            temp = textBox4.Text;
            w2 = float.Parse(temp);

            temp = textBox5.Text;
            constraint = Int32.Parse(temp);

            temp = textBox6.Text;
            learning_rate = float.Parse(temp);

            if (learning_rate > 1 && learning_rate <= 0)
            {
                MessageBox.Show("Learning rate must be a number between 0~1" + "\nPlease select a number in this range!");
                learning_rate = -1;
            }


            if (constraint > 2000)
            {
                MessageBox.Show("The maximum calculation times is 2000!" + "\nPlease select a number less than 2000!");
                constraint = -1;
            }

            //start calculating
            double[] targetTest = new double[20];
            double[] act = new double[20];
            bool cal = true;//remember to change this!!!!!!!!!!!!

            //calculate activation for the first time
            for (int i = 0; i < counterA+counterB; i++)
            {
                //label13.Text = "activation";
                textBoxact.AppendText("state " + i + " : " + "\n");

                act[i] = w1 * tr_X1[i] + w2 * tr_X2[i];

                textBoxact.AppendText("w1 : " + w1 + " , X1 : " + tr_X1[i] + "\n" + "w2 : " + w2 + " , X2 : " + tr_X2[i] + "\n");
                textBoxact.AppendText("Activation " + i + " : " + act[i].ToString() + "\n");
                textBoxact.AppendText("output : " + tr_output[i] + "\n");

                //set y value
                if (act[i] >= thdis)
                {
                    targetTest[i] = 1;
                }
            }

            //check if match output
            for (int i = 0; i < counterA+counterB; i++)
            {
                //label13.Text = "check match" + i.ToString();

                //set cal to true if any one is not a match
                if (targetTest[i] != tr_output[i])
                {
                    cal = true;
                }

                ////if y is in the tolernce of t, then stop calculation
                //if (act[i] <= thres + tol && act[i] >= thres - tol || targetTest[i] == tr_output[i])
                //{
                //    //cal = false;
                //}
            }

            while (cal == true)
            {
                textBoxw.AppendText("Bound : " + (bound + 1) + "\n");
                //label13.Text = "in cal function";
                bound++;

                //updating weights
                for (int i = 0; i < counterA + counterB; i++)
                {
                    //if t != y
                    if (targetTest[i] != tr_output[i])
                    {
                        double w1Temp = 0, w2Temp = 0;
                        w1Temp = w1 + learning_rate * (tr_output[i] - targetTest[i]) * tr_X1[i];
                        w2Temp = w2 + learning_rate * (tr_output[i] - targetTest[i]) * tr_X2[i];
                        w1 = w1Temp;
                        w2 = w2Temp;

                        textBoxw.AppendText("w1 : " + w1 + "\n" + " , w2 : " + w2 + "\n");
                    }
                }

                //updating activations
                for (int i = 0; i < counterA + counterB; i++)
                {
                    //label13.Text = "activation";

                    act[i] = w1 * tr_X1[i] + w2 * tr_X2[i];

                    textBoxact.AppendText("w1 : " + w1 + " , X1 : " + tr_X1[i] + "\n" + "w2 : " + w2 + " , X2 : " + tr_X2[i] + "\n");
                    textBoxact.AppendText("Activation " + i + " : " + act[i].ToString() + "\n");
                    textBoxact.AppendText("output : " + tr_output[i] + "\n");

                    //set y value
                    if (act[i] >= thdis)
                    {
                        targetTest[i] = 1;
                    }
                    textBoxact.AppendText("state " + i + " : " + targetTest[i] + "\n");
                }

                //check if match output
                int m = 0;
                while (m < counterA + counterB)
                {
                    //set cal to true if any one is not a match
                    if (targetTest[m] != tr_output[m])
                    {
                        cal = true;
                        break;
                    }
                    else
                    {
                        cal = false;
                    }

                    ////if y is in the tolernce of t, then stop calculation
                    //if (act[i] <= thres)
                    //{
                    //    cal = false;
                    //}

                    m++;
                }

                if (bound >= constraint)
                {
                    MessageBox.Show("Learning fails!\n" + "Please reset the test!\n");
                    final = 0;
                    break;
                }
            }

            if (cal == false)
            {
                MessageBox.Show("Learing completed!\n");
                final = 1;//successed
            }
            //draw line
            if (final == 1)
            {
                chart1.Series["LineResult"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                double K = (thdis / w1) * (thy / (thdis - thx));//1=w1
                double R = thy / (thx - thdis / w1);//1=w1
                chart1.Series["LineResult"].Points.AddXY(6, 6 * R + K);
                chart1.Series["LineResult"].Points.AddXY(-6, -6 * R + K); //thdis / w1, 0



            }
            //chart1.Series["Line2"].Points.AddXY(thx, thy);
            //chart1.Series["Line2"].Points.AddXY(0, (thdis/1)*(thy/thdis-thx)); //(thdis/w1)*(thy/(thdis-thx))
        }

        private void chart1_Click(object sender, EventArgs e)//point input
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
    }
}
