using POSUNO.Components;
using POSUNO.Dialogs;
using POSUNO.Helpers;
using POSUNO.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace POSUNO.Pages
{
    public sealed partial class CustomersPage : Page
    {
        public CustomersPage()
        {
            InitializeComponent();
        }

        public ObservableCollection<Customer> Customers { get; set; }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadCustomersAsync();
        }

        private async void LoadCustomersAsync()
        {
            Loader loader = new Loader("Por favor espere...");
            loader.Show();
            Response response = await ApiService.GetListAsync<Customer>("customers");
            loader.Close();
            if (!response.isSuccess)
            {
                MessageDialog messageDialog = new MessageDialog(response.Message, "Error");
                await messageDialog.ShowAsync();
                return;
            }
            List<Customer> customers = (List<Customer>)response.Result;
            Customers = new ObservableCollection<Customer>(customers);
            RefreshList();
        }

        private void RefreshList()
        {
            CustomersListView.ItemsSource = null;
            CustomersListView.Items.Clear();
            CustomersListView.ItemsSource = Customers;
        }

        private async void AddCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = new Customer();
            CustomerDialog dialog = new CustomerDialog(customer);
            await dialog.ShowAsync();
            if (!customer.WasSaved)
            {
                return;
            }
            customer.User = MainPage.GetInstance().user;
            Loader loader = new Loader("Por favor espere...");
            loader.Show();
            Response response = await ApiService.PostAsync("Customers", customer);
            loader.Close();
            if (!response.isSuccess)
            {
                MessageDialog messageDialog = new MessageDialog(response.Message, "Error");
                await messageDialog.ShowAsync();
                return;
            }

            Customer newCustomer = (Customer)response.Result;
            Customers.Add(newCustomer);
            RefreshList();
        }

        private async void EditImage_Tapped(object sender, RoutedEventArgs e)
        {
            Customer customer = Customers[CustomersListView.SelectedIndex];
            customer.IsEdit = true;
            CustomerDialog dialog = new CustomerDialog(customer);
            await dialog.ShowAsync();
            if (!customer.WasSaved)
            {
                return;
            }
            customer.User = MainPage.GetInstance().user;
            Loader loader = new Loader("Por favor espere...");
            loader.Show();
            Response response = await ApiService.PutAsync("Customers", customer, customer.Id);
            loader.Close();
            if (!response.isSuccess)
            {
                MessageDialog messageDialog = new MessageDialog(response.Message, "Error");
                await messageDialog.ShowAsync();
                return;
            }

            Customer newCustomer = (Customer)response.Result;
            Customer oldCustomer = Customers.FirstOrDefault(c => c.Id == newCustomer.Id);
            oldCustomer = newCustomer;
            RefreshList();
        }

        private async void DeleteImage_Tapped(object sender, RoutedEventArgs e)
        {
            ContentDialogResult result = await ConfirmDeleteAsync();
            if (result != ContentDialogResult.Primary)
            {
                return;
            }

            Loader loader = new Loader("Por favor espere...");
            loader.Show();
            Customer customer = Customers[CustomersListView.SelectedIndex];
            Response response = await ApiService.DeleteAsync("Customers", customer.Id);
            loader.Close();

            if (!response.isSuccess)
            {
                MessageDialog messageDialog = new MessageDialog(response.Message, "Error");
                await messageDialog.ShowAsync();
                return;
            }

            List<Customer> customers = Customers.Where(c => c.Id != customer.Id).ToList();
            Customers = new ObservableCollection<Customer>(customers);
            RefreshList();
        }

        private async Task<ContentDialogResult> ConfirmDeleteAsync()
        {
            ContentDialog confirmDialog = new ContentDialog
            {
                Title = "Confirmación",
                Content = "¿Estás seguro de querer borrar el registro?",
                PrimaryButtonText = "Sí",
                CloseButtonText = "No"
            };

            return await confirmDialog.ShowAsync();
        }

    }

}
