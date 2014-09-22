using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KMeansAlg;

namespace lab1
{
    public partial class Form1 : Form
    {
        private BackgroundWorker bw = new BackgroundWorker();
        private BufferedGraphicsContext current = BufferedGraphicsManager.Current;
        private BufferedGraphics graphicsBuffer;
        private object _Sync = new object();

        public Form1()
        {
            InitializeComponent();
            graphicsBuffer = current.Allocate(CreateGraphics(), DisplayRectangle);
            bw.WorkerReportsProgress = false;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (bw.IsBusy != true)
            {
                button1.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                checkBox1.Enabled = false;
                button1.Text = "Выполнение...";
                button1.Update();
                bw.RunWorkerAsync();
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || e.KeyChar == '\b')
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || e.KeyChar == '\b')
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            bool drawLine = checkBox1.Checked;
            bool notChanged;
            uint currentIteration = 0;
            Random random = new Random();
            List<KMPoint> points = new List<KMPoint>();
            BackgroundWorker worker = sender as BackgroundWorker;
            uint classCount = Convert.ToUInt32(textBox1.Text, 10);
            uint dotCount = Convert.ToUInt32(textBox2.Text, 10);

            button2.Enabled = true;

            for (int i = 0; i < dotCount; i++)
                points.Add(new KMPoint(random.Next(10, 700), random.Next(30, 350)));

            KMeans kmeans = new KMeans(points, classCount);

            do
            {
                currentIteration++;
                label5.Text = currentIteration.ToString();
                label5.Update();

                lock (_Sync)
                {
                    graphicsBuffer.Graphics.Clear(ActiveForm.BackColor);
                    kmeans.DrawClusters(graphicsBuffer.Graphics, drawLine);
                    graphicsBuffer.Render();
                }

                if (bw.CancellationPending == true)
                {
                    button2.Text = "Стоп";
                    button2.Update();
                    break;
                }

                notChanged = kmeans.Calculate();
            } while (!notChanged);
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button1.Text = "Запуск";
            button1.Update();
            button1.Enabled = true;
            button2.Enabled = false;
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            checkBox1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (bw.WorkerSupportsCancellation == true)
            {
                button2.Enabled = false;
                button2.Text = "Остановка...";
                button2.Update();
                bw.CancelAsync();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            lock (_Sync)
            {
                graphicsBuffer.Render();
            }
        }

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            graphicsBuffer.Dispose();

            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            lock (_Sync)
            {
                graphicsBuffer.Graphics.Clear(ActiveForm.BackColor);
            }
        }

    }
}
