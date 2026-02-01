using Subscription_Manager.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace Subscription_Manager
{
    public partial class DisabledSubscriptionsWindow : Window
    {
        public ObservableCollection<Subscription> DisabledSubscriptions { get; }

        public DisabledSubscriptionsWindow(ObservableCollection<Subscription> allSubscriptions)
        {
            InitializeComponent();

            DisabledSubscriptions = new ObservableCollection<Subscription>(
                allSubscriptions.Where(s => !s.IsActive)
            );

            DataContext = this;
        }
    }
}