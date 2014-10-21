using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using KMeansAlg;
using MaxMinAlg;

namespace lab1
{
    public partial class Form1 : Form
    {
        private BackgroundWorker _bw = new BackgroundWorker();
        private BackgroundWorker _bw2 = new BackgroundWorker();
        private BufferedGraphicsContext _current = BufferedGraphicsManager.Current;
        private BufferedGraphics _graphicsBuffer;
        private object _sync = new object();
        private List<KMPoint> _points = new List<KMPoint>();
        private int _dotCount;
        private int _classCount;
        private MaxMin _maxmin;

        public Form1()
        {
            InitializeComponent();
            _graphicsBuffer = _current.Allocate(CreateGraphics(), DisplayRectangle);
            _bw.WorkerReportsProgress = false;
            _bw.WorkerSupportsCancellation = true;
            _bw.DoWork += bw_DoWork;
            _bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            _bw2.WorkerReportsProgress = false;
            _bw2.DoWork += bw_DoWork2;
            _bw2.RunWorkerCompleted += bw_RunWorkerCompleted;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_bw.IsBusy != true)
            {
                button1.Enabled = false;
                button2.Enabled = true;
                button3.Enabled = false;
                textBox2.Enabled = false;
                checkBox1.Enabled = false;
                button1.Text = "Выполнение...";
                button1.Update();
                _bw.RunWorkerAsync();
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
            bool changed;
            bool drawLine = checkBox1.Checked;
            uint currentIteration = 0;
            BackgroundWorker worker = sender as BackgroundWorker;
            KMeans kmeans = new KMeans(_maxmin._points, _maxmin._clusters);

            do
            {
                currentIteration++;
                label5.Invoke(new Action(delegate()
                {
                    label5.Text = currentIteration.ToString();
                    label5.Update();
                }));
                

                lock (_sync)
                {
                    _graphicsBuffer.Graphics.Clear(BackColor);
                    kmeans.DrawClusters(_graphicsBuffer.Graphics, drawLine);
                    _graphicsBuffer.Render();
                }

                if (_bw.CancellationPending == true)
                {
                    button2.Invoke(new Action(delegate()
                    {
                        button2.Text = "Стоп";
                        button2.Update();
                    }));
                    
                    break;
                }

                changed = kmeans.Calculate();
            } while (changed);
        }

        private void bw_DoWork2(object sender, DoWorkEventArgs e)
        {
            bool newCore;
            bool drawLine = checkBox1.Checked;
            Random random = new Random();

            button2.Invoke(new Action(delegate() { button2.Enabled = false; }));
            _dotCount = Convert.ToInt32(textBox2.Text, 10);
            _points.Clear();

            for (int i = 0; i < _dotCount; i++)
                _points.Add(new KMPoint(random.Next(10, 700), random.Next(30, 350)));

            _maxmin = new MaxMin(_points);

            do
            {
                newCore = _maxmin.Calculate();
            } while (newCore);

            lock (_sync)
            {
                _graphicsBuffer.Graphics.Clear(BackColor);
                _maxmin.DrawClusters(_graphicsBuffer.Graphics, drawLine);
                _graphicsBuffer.Render();
            }
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button1.Text = "K-means";
            button1.Update();
            button1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = true;
            textBox2.Enabled = true;
            checkBox1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (_bw.WorkerSupportsCancellation == true)
            {
                button2.Enabled = false;
                button2.Text = "Остановка...";
                button2.Update();
                _bw.CancelAsync();
            }

            if (_bw2.WorkerSupportsCancellation == true)
            {
                button2.Enabled = false;
                button2.Text = "Остановка...";
                button2.Update();
                _bw2.CancelAsync();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            lock (_sync)
            {
                _graphicsBuffer.Render();
            }
        }

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            _graphicsBuffer.Dispose();

            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            lock (_sync)
            {
                _graphicsBuffer.Graphics.Clear(ActiveForm.BackColor);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (_bw2.IsBusy != true)
            {
                button1.Enabled = false;
                button3.Enabled = false;
                textBox2.Enabled = false;
                checkBox1.Enabled = false;
                _bw2.RunWorkerAsync();
            }
        }
    }
}
