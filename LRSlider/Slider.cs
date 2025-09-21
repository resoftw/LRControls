using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LightroomControls
{
    [DefaultEvent("ValueChanged")]
    [ToolboxItem(true)]
    public class LRSlider : UserControl
    {
        private Label lblName;
        private NumericUpDown numBox;

        private double min = -100.0;
        private double max = 100.0;
        private double value = 0.0;

        private bool isDragging = false;

        public event EventHandler? ValueChanged;

        private ColorBlend? gradientBlend;

        public LRSlider()
        {
            Height = 23;
            Width = 300;

            lblName = new Label
            {
                Text = "Exposure",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Left,
                Width = 80,
                Height = 23,
            };

            numBox = new NumericUpDown
            {
                DecimalPlaces = 2,
                Increment = 0.1M,
                Minimum = -1000,
                Maximum = 1000,
                Dock = DockStyle.Right,
                AutoSize = false,
                Width = 60,
                Height = 23,
            };
            numBox.ValueChanged += (s, e) =>
            {
                Value = (double)numBox.Value;
            };

            Controls.Add(numBox);
            Controls.Add(lblName);

            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string LabelText
        {
            get => lblName.Text;
            set => lblName.Text = value;
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ContentAlignment LabelAlign
        {
            get => lblName.TextAlign;
            set => lblName.TextAlign = value;
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int LabelWidth
        {
            get => lblName.Width;
            set { lblName.Width = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ColorBlend? GradientBlend
        {
            get => gradientBlend;
            set { gradientBlend = value; Invalidate(); }
        }

        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double Minimum
        {
            get => min;
            set { min = value; if (this.value < min) Value = min; Invalidate(); }
        }

        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double Maximum
        {
            get => max;
            set { max = value; if (this.value > max) Value = max; Invalidate(); }
        }

        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double Value
        {
            get => value;
            set
            {
                if (value < min) value = min;
                if (value > max) value = max;
                this.value = value;
                numBox.Value = (decimal)this.value;
                Invalidate();
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double Increment
        {
            get => (double)numBox.Increment;
            set => numBox.Increment = (decimal)value;
        }

        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int DecimalPlaces
        {
            get => numBox.DecimalPlaces;
            set => numBox.DecimalPlaces = value;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            int sliderLeft = lblName.Right + 8;
            int sliderRight = numBox.Left - 8;
            int sliderWidth = sliderRight - sliderLeft;
            int sliderY = Height / 2;

            // track
            Rectangle track = new Rectangle(sliderLeft, sliderY - 3, sliderWidth, 6);
            if (gradientBlend != null)
            {
                using var brush = new LinearGradientBrush(track, Color.Black, Color.Black, LinearGradientMode.Horizontal)
                {
                    InterpolationColors = gradientBlend
                };
                g.FillRectangle(brush, track);
            }
            else
                g.FillRectangle(Brushes.DarkGray, track);

            g.DrawRectangle(SystemPens.ActiveBorder,track);
            // filled
            float percent = (float)((value - min) / (max - min));
            int fillWidth = (int)(percent * sliderWidth);
            if (gradientBlend == null)
            {
                g.FillRectangle(Brushes.DodgerBlue, new Rectangle(sliderLeft, sliderY - 3, fillWidth, 6));
            }
            // thumb
            int thumbX = sliderLeft + fillWidth;
            Rectangle thumb = new Rectangle(thumbX - 5, sliderY - 8, 10, 16);
            g.FillRectangle(Brushes.White, thumb);
            g.DrawRectangle(Pens.Black, thumb);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                UpdateValueFromMouse(e.X);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isDragging)
                UpdateValueFromMouse(e.X);
            int sliderLeft = lblName.Right + 8;
            int sliderRight = numBox.Left - 8;
            int sliderWidth = sliderRight - sliderLeft;
            if (e.X >= sliderLeft && e.X <= sliderRight)
                Cursor = Cursors.Hand;
            else
                Cursor = Cursors.Default;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            isDragging = false;
        }

        private void UpdateValueFromMouse(int mouseX)
        {
            int sliderLeft = lblName.Right + 5;
            int sliderRight = numBox.Left - 5;
            int sliderWidth = sliderRight - sliderLeft;

            double percent = (double)(mouseX - sliderLeft) / sliderWidth;
            Value = min + percent * (max - min);
        }
    }
}
