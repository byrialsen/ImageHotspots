using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace ImageHotspot.Controls
{
    [TemplatePart(Name = BorderPartName, Type = typeof(Ellipse))]
    public class HotspotControl : ContentControl
    {
        #region Fields

        private const string BorderPartName = "PART_Border";

        private Ellipse _extend;

        #endregion Fields

        #region Properties

        public Image ParentImage
        {
            get { return (Image)GetValue(ParentImageProperty); }
            set { SetValue(ParentImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParentImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParentImageProperty =
            DependencyProperty.Register("ParentImage", typeof(Image), typeof(HotspotControl), new PropertyMetadata(null));

        /// <summary>
        /// Diameter of circle/ellipse
        /// </summary>
        public double Diameter
        {
            get { return (double)GetValue(DiameterProperty); }
            set { SetValue(DiameterProperty, value); }
        }

        public static readonly DependencyProperty DiameterProperty =
            DependencyProperty.Register(
                "Diameter",
                typeof(double),
                typeof(HotspotControl),
                new PropertyMetadata(0.0, Invalidate));

        private static void Invalidate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //(d as HotspotControl)?.AdjustCanvasPosition();
        }

        /// <summary>
        /// Horizonal Center in percent
        /// </summary>
        public double CenterRelativeX
        {
            get { return (double)GetValue(CenterRelativeXProperty); }
            set { SetValue(CenterRelativeXProperty, value); }
        }

        public static readonly DependencyProperty CenterRelativeXProperty =
            DependencyProperty.Register(
                "CenterRelativeX",
                typeof(double),
                typeof(HotspotControl),
                new PropertyMetadata(0.0, Invalidate));

        /// <summary>
        /// Vertical Center in percent
        /// </summary>
        public double CenterRelativeY
        {
            get { return (double)GetValue(CenterRelativeYProperty); }
            set { SetValue(CenterRelativeYProperty, value); }
        }


        // Using a DependencyProperty as the backing store for CenterRelativeY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CenterRelativeYProperty =
            DependencyProperty.Register(
                "CenterRelativeY",
                typeof(double),
                typeof(HotspotControl),
                new PropertyMetadata(0.0, Invalidate));

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="HotspotControl"/> class.
        /// </summary>
        public HotspotControl()
        {
            this.DefaultStyleKey = typeof(HotspotControl);
        }

        #endregion Constructor

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call ApplyTemplate.
        /// In simplest terms, this means the method is called just before a UI element displays in your app.
        /// Override this method to influence the default post-template logic of a class.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            // Border
            this._extend = this.GetTemplateChild(BorderPartName) as Ellipse;
            if (this._extend != null)
            {
                this._extend.Tapped += OnBorderTapped;
                this._extend.PointerEntered += OnBorderPointerEntered;

                // Move Canvas properties from control to border.
                Canvas.SetLeft(_extend, Canvas.GetLeft(this));
                Canvas.SetLeft(this, 0);
                Canvas.SetTop(_extend, Canvas.GetTop(this));
                Canvas.SetTop(this, 0);

                // Move Margin to border.
                //this.border.Padding = this.Margin;
                //this.Margin = new Thickness(0);
            }
            else
            {
                // Exception
                throw new Exception($"{nameof(HotspotControl)} Style has no border.");
            }

            // Overlay
            //this.overlay = GetTemplateChild(OverlayPartName) as UIElement;

            (Parent as FrameworkElement).SizeChanged += OnSizeChanged;

            // 1
            //var realParent = GetRealParent(this);
            //realParent.SizeChanged += OnSizeChanged;

            AdjustCanvasPosition();
        }

        private FrameworkElement GetRealParent(FrameworkElement element)
        {
            while (!(element is Viewbox))
            {
                // Crawl up the Visual Tree.
                element = element.Parent as FrameworkElement;
            }

            return element;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Parent size changed");

            AdjustCanvasPosition();
        }

        private void OnBorderPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            //FlashBorder();
        }

        private void OnBorderTapped(object sender, TappedRoutedEventArgs e)
        {
            //FlashBorder();
        }

        private void AdjustCanvasPosition()
        {
            FrameworkElement parent = this.Parent as FrameworkElement; // hotspot canvas

            if (parent != null)
            {
                System.Diagnostics.Debug.WriteLine("Left: " + Canvas.GetLeft(parent));
                System.Diagnostics.Debug.WriteLine("Top:" + Canvas.GetTop(parent));

                var res = this.TransformToVisual(parent);

                Canvas.SetLeft(_extend, parent.ActualWidth * (CenterRelativeX / 100) - Diameter / 2);
                Canvas.SetTop(_extend, parent.ActualHeight * (CenterRelativeY / 100) - Diameter / 2); ;
            }
        }

        private void FlashBorder()
        {
            if (this._extend != null)
            {
                var ani = new DoubleAnimation()
                {
                    From = 0.0,
                    To = 1.0,
                    Duration = new Duration(TimeSpan.FromSeconds(1.5))
                };
                var storyBoard = new Storyboard();
                storyBoard.Children.Add(ani);
                Storyboard.SetTarget(ani, _extend);
                ani.SetValue(Storyboard.TargetPropertyProperty, "Opacity");
                storyBoard.Begin();
            }
        }

        /// <summary>
        /// Gets the closest parent with a real size.
        /// </summary>
        private FrameworkElement GetClosestParentWithSize(FrameworkElement element)
        {
            while (element != null && (element.ActualHeight == 0 || element.ActualWidth == 0))
            {
                // Crawl up the Visual Tree.
                element = element.Parent as FrameworkElement;
            }

            return element;
        }
    }
}
