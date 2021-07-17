using POSUNO.Helpers;
using POSUNO.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace POSUNO.Dialogs
{
	public sealed partial class CustomerDialog : ContentDialog
	{
		public CustomerDialog(Customer customer)
		{
			InitializeComponent();
			Customer = customer;
            if (Customer.IsEdit)
            {
				TitleTextBlock.Text = $"Editar el cliente: {Customer.FullName}";
            }
            else
            {
				TitleTextBlock.Text = "Nuevo Cliente";
			}
		}

        public Customer Customer { get; set; }

		private async void CloseImage_Tapped(object sender, RoutedEventArgs e)
		{
            Customer.WasSaved = false;
            Hide();
		}

		private async void CancelClick_Button(object sender, RoutedEventArgs e)
		{
            Customer.WasSaved = false;
            Hide();
		}

		private async void SaveClick_Button(object sender, RoutedEventArgs e)
		{
			bool isValid = await ValidateFormAsync();
            if (!isValid)
            {
                return;
            }
            Customer.WasSaved = true;
			Hide();
		}

        private async Task<bool> ValidateFormAsync()
        {
            MessageDialog messageDialog;

            if (string.IsNullOrEmpty(Customer.FirstName))
            {
                messageDialog = new MessageDialog("Debes ingresar nombres del cliente.", "Error");
                await messageDialog.ShowAsync();
                return false;
            }

            if (string.IsNullOrEmpty(Customer.LastName))
            {
                messageDialog = new MessageDialog("Debes ingresar apellidos del cliente.", "Error");
                await messageDialog.ShowAsync();
                return false;
            }

            if (string.IsNullOrEmpty(Customer.Phonenumber))
            {
                messageDialog = new MessageDialog("Debes ingresar teléfono del cliente.", "Error");
                await messageDialog.ShowAsync();
                return false;
            }

            if (string.IsNullOrEmpty(Customer.Address))
            {
                messageDialog = new MessageDialog("Debes ingresar dirección del cliente.", "Error");
                await messageDialog.ShowAsync();
                return false;
            }

            if (string.IsNullOrEmpty(Customer.Email))
            {
                messageDialog = new MessageDialog("Debes ingresar email del cliente.", "Error");
                await messageDialog.ShowAsync();
                return false;
            }

            if (!RegexUtilities.IsValidEmail(Customer.Email))
            {
                messageDialog = new MessageDialog("Debes ingresar un email válido.", "Error");
                await messageDialog.ShowAsync();
                return false;
            }

            return true;
        }
    }
}
