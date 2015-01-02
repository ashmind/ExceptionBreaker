using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.PlatformUI;

namespace ExceptionBreaker.Breakpoints {
    /// <summary>
    /// Interaction logic for BreakpointExceptionsDialog.xaml
    /// </summary>
    public partial class BreakpointExceptionsDialog : DialogWindow {
        public BreakpointExceptionsDialog() {
            InitializeComponent();
        }

        public BreakpointExceptionSettings ViewModel {
            get { return (BreakpointExceptionSettings)DataContext; }
            set { DataContext = value; }
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }

        private void comboChange_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
        }
    }
}
