using Microsoft.Maui.Controls;
using NyxLine.MAUI.ViewModels;

namespace NyxLine.MAUI.Views
{
    public partial class ObservationPage : ContentPage
    {
        public ObservationPage()
        {
            InitializeComponent();
            BindingContext = new ObservationViewModel();
        }
    }
} 