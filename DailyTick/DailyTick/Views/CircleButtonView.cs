using System;
using System.Windows.Input;
using NControl.Abstractions;
using NGraphics;
using Xamarin.Forms;

namespace DailyTick.Views
{
    /// <summary>
    /// https://github.com/chrfalch/NControl/blob/develop/NControlDemo/NControlDemo.FormsApp/Controls/CircularButtonControl.cs
    /// </summary>
    public class CircleButtonView : NControlView
    {
        #region BindableProperty
        /// <summary>
        /// The Command property.
        /// </summary>
        public static BindableProperty CommandProperty =
            BindableProperty.Create(
                propertyName: nameof(Command),
                returnType: typeof(ICommand),
                declaringType: typeof(CircleButtonView),
                defaultBindingMode: BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) => {
                    var ctrl = (CircleButtonView)bindable;
                    ctrl.Command = (ICommand)newValue;
                });

        /// <summary>
        /// The CommandParameter property.
        /// </summary>
        public static BindableProperty CommandParameterProperty =
            BindableProperty.Create(
                propertyName: nameof(CommandParameter),
                returnType: typeof(object),
                declaringType: typeof(CircleButtonView),
                defaultBindingMode: BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) => {
                    var ctrl = (CircleButtonView)bindable;
                    ctrl.CommandParameter = newValue;
                });
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Command of the CircleButtonView instance.
        /// </summary>
        /// <value>The color of the buton.</value>
        public ICommand Command {
            get { return (ICommand)GetValue(CommandProperty); }
            set {
                SetValue(CommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the CommandParameter of the CircleButtonView instance.
        /// </summary>
        /// <value>The color of the buton.</value>
        public object CommandParameter {
            get { return (object)GetValue(CommandParameterProperty); }
            set {
                SetValue(CommandParameterProperty, value);
            }
        }

        public DateTimeOffset DurationStart { get; set; }
        #endregion

        public event EventHandler Clicked;

        private readonly Label _labelDuration;
        private readonly Label _labelTimeLeft;
        private readonly NControlView _circle;

        public CircleButtonView()
        {
            _labelDuration = new Label {
                Text = DurationText(),
                TextColor = Xamarin.Forms.Color.Blue,
                FontSize = 35,
                BackgroundColor = Xamarin.Forms.Color.Transparent,
                HorizontalTextAlignment = Xamarin.Forms.TextAlignment.Center,
                VerticalTextAlignment = Xamarin.Forms.TextAlignment.Center,
            };
            _labelTimeLeft = new Label {
                Text = TimeLeftText(),
                FontSize = 15,
                TextColor = Xamarin.Forms.Color.Green,
                BackgroundColor = Xamarin.Forms.Color.Transparent,
                HorizontalTextAlignment = Xamarin.Forms.TextAlignment.Center,
            };
            _circle = new NControlView {
                DrawingFunction = DrawCircle
            };

            var layout = new RelativeLayout();
            layout.Children.Add(_circle, Constraint.RelativeToParent((parent) => {
                return 0;
            }), Constraint.RelativeToParent((parent) => {
                return 0;
            }), Constraint.RelativeToParent((parent) => {
                return parent.Width;
            }), Constraint.RelativeToParent((parent) => {
                return parent.Height;
            }));

            layout.Children.Add(_labelDuration, Constraint.RelativeToParent((parent) => {
                return 0;
            }), Constraint.RelativeToParent((parent) => {
                return 60;
            }), Constraint.RelativeToParent((parent) => {
                return parent.Width;
            }));

            layout.Children.Add(_labelTimeLeft, Constraint.RelativeToParent((parent) => {
                return 0;
            }), Constraint.RelativeToParent((parent) => {
                return 120;
            }), Constraint.RelativeToParent((parent) => {
                return parent.Width;
            }));
            Content = layout;
        }

        private string DurationText()
        {
            var timeSpan = DateTimeOffset.Now - DurationStart;
            return timeSpan.ToString(@"hh\:mm");
        }

        private double TimeLeft()
        {
            var now = DateTimeOffset.Now;
            var nextDayStart = (new DateTimeOffset(now.Date, now.Offset)).AddDays(1);
            return (nextDayStart - now).TotalSeconds / 60.0;
        }

        private string TimeLeftText()
        {
            return string.Format("今天还剩 {0:F1} 分钟", TimeLeft());
        }

        private void DrawCircle(ICanvas canvas, Rect rect)
        {
            double cX = rect.Left + rect.Width / 2.0;
            double cY = rect.Top  + rect.Height / 2.0;
            var center = new NGraphics.Point(cX, cY);

            var angle = 2.0 * Math.PI * (1440.0 - TimeLeft()) / 1440.0;

            // Leave to check the Arc
            //canvas.DrawEllipse(new NGraphics.Point(cX - rect.Height / 2.0, 0), new NGraphics.Size(rect.Height), Pens.Blue.WithWidth(15.0));

            canvas.DrawPath((p) => {
                double s90 = Math.Sin(angle);
                double r = rect.Height / 2.0;
                var targetPoint = new NGraphics.Point(cX + r * Math.Sin(angle), cY - r * Math.Cos(angle));

                p.MoveTo(new NGraphics.Point(cX, cY - r));
                p.ArcTo(new NGraphics.Size(r), angle < Math.PI, false, targetPoint);
                p.MoveTo(new NGraphics.Point(cX, cY - r));
                p.Close();
            }, Pens.Gray.WithWidth(10.0));
        }

        public void Update()
        {
            _labelDuration.Text = DurationText();
            _labelTimeLeft.Text = TimeLeftText();
            Invalidate();
        }

        public override bool TouchesBegan(System.Collections.Generic.IEnumerable<NGraphics.Point> points)
        {
            base.TouchesBegan(points);
            this.ScaleTo(0.8, 65, Easing.CubicInOut);
            return true;
        }

        public override bool TouchesCancelled(System.Collections.Generic.IEnumerable<NGraphics.Point> points)
        {
            base.TouchesCancelled(points);
            this.ScaleTo(1.0, 65, Easing.CubicInOut);
            return true;
        }

        public override bool TouchesEnded(System.Collections.Generic.IEnumerable<NGraphics.Point> points)
        {
            base.TouchesEnded(points);
            this.ScaleTo(1.0, 65, Easing.CubicInOut);
            if (Command != null && Command.CanExecute(CommandParameter)) {
                Command.Execute(CommandParameter);
            }

            EventHandler handler = Clicked;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }

            return true;
        }
    }
}
