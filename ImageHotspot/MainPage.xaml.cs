using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ImageHotspot
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Hotspot> Hotspots { get; set; } = new ObservableCollection<Hotspot>();

        public MainPage()
        {
            this.InitializeComponent();

            (this.Content as FrameworkElement).DataContext = this;

            Hotspots.Add(new Hotspot() { Name = "SideInspection1", CenterX = 10.0, CenterY = 10.0 });
            Hotspots.Add(new Hotspot() { Name = "SideInspection2", CenterX = 50.0, CenterY = 50.0 });
            Hotspots.Add(new Hotspot() { Name = "SideInspection3", CenterX = 90.0, CenterY = 90.0 });
        }
    }
}
