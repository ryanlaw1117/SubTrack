using System.Windows;

namespace Subscription_Manager
{
    public partial class DeleteConfirmationWindow : Window
    {
        public DeleteConfirmationWindow(int count, decimal yearlySavings)
        {
            InitializeComponent();

            TitleText.Text = (count == 1)
                ? "Are you sure you want to delete this subscription?"
                : $"Delete {count} subscription(s)?";

            SavingsText.Text = $"You will save {yearlySavings:C} per year.";
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}