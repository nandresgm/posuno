using POSUNO.Components;
using POSUNO.Helpers;
using POSUNO.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Uno.Extensions;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace POSUNO.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = await ValidForm();
            if (!isValid)
            {
                return;
            }

            Loader loader = new Loader("Por favor espere...");
            loader.Show();
            Response response = await ApiService.LoginAsync(new LoginRequest
            {
                Email = EmailTextBox.Text,
                Password = PasswordPasswordBox.Password
            });
            loader.Close();
            MessageDialog messageDialog;
            if (!response.isSuccess)
            {
                messageDialog = new MessageDialog(response.Message, "Error");
                await messageDialog.ShowAsync();
                return;
            }
            User user = (User)response.Result;
            if (user == null)
            {
                messageDialog = new MessageDialog("usuario o contraseñas incorrectos", "Error");
                await messageDialog.ShowAsync();
                return;
            }

            Frame.Navigate(typeof(MainPage), user);

        }

        private async Task<bool> ValidForm()
        {
            MessageDialog messageDialog;

            if (string.IsNullOrEmpty(EmailTextBox.Text))
            {
                messageDialog = new MessageDialog("Debes ingresar tu email", "Error");
                await messageDialog.ShowAsync();
                return false;
            }

            if (!RegexUtilities.IsValidEmail(EmailTextBox.Text))
            {
                messageDialog = new MessageDialog("Debes ingresar un email válido", "Error");
                await messageDialog.ShowAsync();
                return false;
            }

            if (string.IsNullOrEmpty(PasswordPasswordBox.Password))
            {
                messageDialog = new MessageDialog("Debes ingresar tu contraseña", "Error");
                await messageDialog.ShowAsync();
                return false;
            }

            return true;
        }

    }
}
