using System;
using System.Collections.Generic;
using System.Windows;
using ExceptionBreaker.Options;
using ExceptionBreaker.Options.Support;

namespace ExceptionBreaker.OptionsUITester {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            view.Model = new OptionsViewModel(
                new OptionsPageData(),
                new ObservableValue<IEnumerable<string>>(new string[] {
                    "System.Exception",
                    "System.ArgumentException",
                    "System.NullReferenceException",
                    "System.IO.IOException"
                })
            );
        }
    }
}
