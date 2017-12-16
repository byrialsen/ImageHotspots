using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace ImageHotspot.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class HotspotImageControl : Control
    {
        #region Fields

        private Canvas _canvas;
        private Image _image;
        
        #endregion Fields

        #region Constructors

        public HotspotImageControl()
        {
            this.DefaultStyleKey = typeof(HotspotImageControl);

            // design time data
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                var items = new List<Hotspot>();
                items.Add(new Hotspot() { Name = "SideInspection1", CenterX = 10.0, CenterY = 10.0 });
                items.Add(new Hotspot() { Name = "SideInspection2", CenterX = 50.0, CenterY = 50.0 });
                items.Add(new Hotspot() { Name = "SideInspection3", CenterX = 90.0, CenterY = 90.0 });
                ItemsSource = items;
            }

            // ensure we don't automatically tab to each hotspot
            this.IsTabStop = false;
            TabNavigation = KeyboardNavigationMode.Once;
            KeyDown += OnControlKeyDown;
        }

        private async Task PositionHotspotsAsync()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                PositionHotspots();
            });
        }

        /// <summary>
        /// Handle left/right key on control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        #endregion Constructors

        #region DP



        public double ItemDimension
        {
            get { return (double)GetValue(ItemDimensionProperty); }
            set { SetValue(ItemDimensionProperty, value); }
        }

        public static readonly DependencyProperty ItemDimensionProperty =
            DependencyProperty.Register("ItemDimension", typeof(double), typeof(HotspotImageControl), new PropertyMetadata(null));



        public Hotspot SelectedItem
        {
            get { return (Hotspot)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(Hotspot), typeof(HotspotImageControl), new PropertyMetadata(null, OnSelectedItemChanged));

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as HotspotImageControl;

            Hotspot newItem;
            int newIndex;

            if (e.NewValue != null)
            {
                newItem = e.NewValue as Hotspot;
                newIndex = control.ItemsSource.ToList().IndexOf(newItem);
                var newElement = control._canvas.Children.ElementAt(newIndex);

                // start animation

                // stop animation on all other
                foreach (var element in control._canvas.Children)
                {
                    // stop animation
                }
            }
            else
            {
                // stop animation on all other
                foreach (var element in control._canvas.Children)
                {
                    // stop animation
                }
            }

        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(HotspotImageControl), new PropertyMetadata(null));


        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(HotspotImageControl), new PropertyMetadata(null));

        public IEnumerable<Hotspot> ItemsSource
        {
            get { return (IEnumerable<Hotspot>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<Hotspot>), typeof(HotspotImageControl), new PropertyMetadata(null));

        #endregion DP

        #region Methods

        protected override void OnApplyTemplate()
        {
            _canvas = (Canvas)GetTemplateChild("ItemCanvas");
            if (_canvas == null)
            {
                throw new NullReferenceException();
            }

            _image = (Image)GetTemplateChild("Image");
            if (_image == null)
            {
                throw new NullReferenceException();
            }
            
            CreateHotspots();
            this.SizeChanged += (s, e) => PositionHotspots();
            _image.ImageOpened += (s, e) => PositionHotspots();
        }


        /// <summary>
        /// Create hotspots elements and add to canvas
        /// </summary>
        private void CreateHotspots()
        {
            // init / clear existing
            _canvas.Children.Clear();

            if (ItemsSource == null)
            {
                return;
            }

            // add hotspots
            foreach (var hotspot in ItemsSource)
            {
                FrameworkElement element = ItemTemplate?.LoadContent() as FrameworkElement;

                var control = new ContentControl();
                control.Content = element;
                control.DataContext = hotspot;
                element.Height = element.Width = ItemDimension;

                _canvas.Children.Add(control);

                control.IsTabStop = true;
                control.UseSystemFocusVisuals = true;
                control.GotFocus += OnHotspotGotFocus;
                control.LostFocus += OnHotspotLostFocus;
                control.PointerEntered += OnHotspotPointerEntered;
                control.PointerExited += OnHotspotPointerExited;
                control.PointerPressed += OnHotspotPointerPressed;
                control.PointerReleased += OnHotspotPointerReleased;
                control.KeyDown += OnHotspotKeyDown;
                control.KeyUp += OnHotspotKeyUp;

                control.SetValue(AutomationProperties.NameProperty, hotspot.Name);

            }

            // position hotspots
            PositionHotspots();
        }

        /// <summary>
        /// Position/draw hotspot correctly on canvas
        /// </summary>
        private void PositionHotspots()
        {
            var controlWidth = _image.ActualWidth;
            var controlHeight = _image.ActualHeight;

            if (controlWidth <= 0.0 || controlHeight <= 0.0)
            {
                return;
            }


            foreach (FrameworkElement element in _canvas.Children)
            {
                Hotspot hotspot = element.DataContext as Hotspot;

                double X = controlWidth * (hotspot.CenterX / 100) - (ItemDimension * 0.5);
                double Y = controlHeight * (hotspot.CenterY / 100) - (ItemDimension * 0.5);

                Point imagePos = new Point(X, Y);
                var posTransform = _image.TransformToVisual(_canvas);
                Point canvasPos = posTransform.TransformPoint(imagePos);

                Canvas.SetLeft(element, canvasPos.X);
                Canvas.SetTop(element, canvasPos.Y);
            }
        }

        #endregion Methods

        #region Events

        private void OnControlKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Left)
            {
                e.Handled = true;
                var currentElement = FocusManager.GetFocusedElement() as ContentControl;
                if (currentElement != null)
                {
                    var index = _canvas.Children.IndexOf(currentElement);
                    var nextIndex = (index + 1) % _canvas.Children.Count;
                    (_canvas.Children.ElementAt(nextIndex) as ContentControl).Focus(FocusState.Keyboard);
                }
            }
            else if (e.Key == Windows.System.VirtualKey.Right)
            {
                e.Handled = true;
                var currentElement = FocusManager.GetFocusedElement() as ContentControl;
                if (currentElement != null)
                {
                    var index = _canvas.Children.IndexOf(currentElement);
                    var nextIndex = index > 0 ? index - 1 : _canvas.Children.Count - 1;
                    (_canvas.Children.ElementAt(nextIndex) as ContentControl).Focus(FocusState.Keyboard);
                }
            }
        }

        private void OnHotspotKeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Space)
            {
                var item = sender as ContentControl;
                item.Opacity = 1.0;
                SelectedItem = item.DataContext != SelectedItem ? item.DataContext as Hotspot : null;
            }
        }

        private void OnHotspotKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Space)
            {
                var item = sender as ContentControl;
                item.Opacity = 0.5;
            }
        }

        private void OnHotspotPointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var item = sender as ContentControl;
            item.Opacity = 0.5;
            SelectedItem = item.DataContext != SelectedItem ? item.DataContext as Hotspot : null;
        }

        private void OnHotspotPointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var item = sender as ContentControl;
            item.Opacity = 1.0;
        }

        private void OnHotspotPointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var item = sender as ContentControl;
            item.Opacity = 0.5;
        }

        private void OnHotspotPointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var item = sender as ContentControl;
            item.Opacity = 1.0;
        }

        private void OnHotspotLostFocus(object sender, RoutedEventArgs e)
        {
            var item = sender as ContentControl;
            item.Opacity = 0.5;
        }

        private void OnHotspotGotFocus(object sender, RoutedEventArgs e)
        {
            var item = sender as ContentControl;
            item.Opacity = 1.0;
        }

        #endregion Events
    }
}
