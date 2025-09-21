using System.Drawing.Drawing2D;

namespace TestBed
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void lr_ValueChanged(object sender, EventArgs e)
        {
            label1.Text = lr.Value.ToString("F2");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lr.GradientBlend = new ColorBlend(3)
            {
                Colors = new Color[] { Color.Red, Color.Yellow, Color.Lime },
                Positions = new float[] { 0f, 0.5f, 1f }
            };
        }
    }
}
