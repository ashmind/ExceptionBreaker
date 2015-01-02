using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.VisualStudio.PlatformUI;

namespace ExceptionBreaker.Options {
    // This is not 'proper' WPF -- but I find it hard to spend any more 
    // time remembering WPF when I am planning to avoid using it wherever
    // possible.
    public partial class OptionsPageView : UserControl {
        public OptionsPageView() {
            InitializeComponent();
        }
        
        public OptionsViewModel ViewModel {
            get { return (OptionsViewModel)DataContext; }
            set { DataContext = value; }
        }
        
        private void textPattern_OnGotFocus(object sender, RoutedEventArgs e) {
            var item = ((UIElement)sender).FindAncestor<ListBoxItem>();
            item.IsSelected = true;
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e) {
            ViewModel.IgnoredPatterns.AddNew();
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e) {
            ViewModel.IgnoredPatterns.DeleteSelected();
        }
    }
}
