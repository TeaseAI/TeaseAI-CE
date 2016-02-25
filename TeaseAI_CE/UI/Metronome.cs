using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace TeaseAI_CE.UI
{
    public partial class Metronome : Form
    {
        List<PictureBox> boxes = new List<PictureBox>();
        Color center = Color.AliceBlue;
        Color offCenter = Color.LightSteelBlue;
        Color original = Color.SteelBlue;
        Timer timer = new Timer();
        int boxIndex = 0;
        bool direction = true;

        public Metronome()
        {
            InitializeComponent();
        }

        private void Metronome_Load(object sender, EventArgs e)
        {
            boxes.Add(pictureBox10);
            boxes.Add(pictureBox9);
            boxes.Add(pictureBox8);
            boxes.Add(pictureBox7);
            boxes.Add(pictureBox6);
            boxes.Add(pictureBox5);
            boxes.Add(pictureBox4);
            boxes.Add(pictureBox3);
            boxes.Add(pictureBox2);
            boxes.Add(pictureBox1);
        }

        /// <summary>
        /// Begins the metronome
        /// </summary>
        /// <param name="speed">Seconds</param>
        public void Start(double speed)
        {
            timer.Tick += new EventHandler(Tick);
            timer.Interval = (int)((speed / 20.0) * 1000.0);
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        void Tick(Object myObject, EventArgs myEventArgs)
        {
            if(checkBox3.Checked)
            {
                SetBoxColor(boxIndex, center);

                if(direction)
                {
                    SetBoxColor(boxIndex - 1, original);

                    ++boxIndex;
                    if(boxIndex == boxes.Count)
                    {
                        boxIndex -= 2;
                        direction = !direction;

                        if(checkBox2.Checked)
                        {
                            //Play sound
                        }
                    }
                }
                else
                {
                    SetBoxColor(boxIndex + 1, original);

                    --boxIndex;
                    if(boxIndex == -1)
                    {
                        boxIndex += 2;
                        direction = !direction;
                    }
                }
            }
        }

        void SetBoxColor(int index, Color color)
        {
            if(index < 0 || index >= boxes.Count)
            {
                return;
            }

            boxes[index].BackColor = color;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if(!checkBox3.Checked)
            {
                for(int i = 0 ; i < boxes.Count ; ++i)
                {
                    boxes[i].BackColor = original;
                }
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox2.Checked)
            {
                timer.Interval /= 2;
            }
            else
            {
                timer.Interval *= 2;
            }
        }
    }
}
