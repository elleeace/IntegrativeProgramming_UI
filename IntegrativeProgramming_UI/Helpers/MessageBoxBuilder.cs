using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IntegrativeProgramming_UI.Helpers
{
    internal static class MessageBoxBuilder
    {
        public static void ShowError(string message, string title = "Error")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowWarning(string message, string title = "Warning")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void ShowInfo(string message, string title = "Information")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowSuccess(string message, string title = "Success")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Asterisk); 
        }

        public static void ShowIncompleteInput(string field = "Some fields")
        {
            MessageBox.Show($"{field} are required or incomplete.", "Incomplete Input", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void ShowNotFound(string entity = "Record")
        {
            MessageBox.Show($"{entity} not found.", "Not Found", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowConfirmation(string message, string title = "Confirm Action")
        {
            MessageBox.Show(message, title, MessageBoxButton.OKCancel, MessageBoxImage.Question);
        }

        public static MessageBoxResult ShowConfirm(string message, string title = "Confirm")
        {
            return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
        }

    }
}
