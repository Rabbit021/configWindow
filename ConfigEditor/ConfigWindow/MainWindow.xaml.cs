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
using System.Xml;
using System.IO;
using System.Collections.ObjectModel;
using ConfigFileAlter;
using System.Data;
using Microsoft.Expression.Interactivity.Core;
using StringOperation;
using System.ComponentModel;
using ConfigWindow.VM;

namespace ConfigWindow
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded -= MainWindow_Loaded;
            this.Loaded += MainWindow_Loaded;
        }

        public void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = MainViewModel.Instance;
            MainViewModel.Instance.Load();
            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
        }

        void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
           
        }
    }



}
