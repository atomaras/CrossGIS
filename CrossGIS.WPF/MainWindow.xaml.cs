using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CrossGIS.Core.ViewModels;
using CrossGIS.WPF.Core;

namespace CrossGIS.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowView
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void Connect(int connectionId, object target)
        {
            throw new NotImplementedException();
        }

        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainViewModel).ToggleGpsCommand.Execute(null);
        }
    }
}
