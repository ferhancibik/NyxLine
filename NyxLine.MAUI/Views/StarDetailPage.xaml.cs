using Microsoft.Maui.Controls;
using NyxLine.MAUI.ViewModels;

namespace NyxLine.MAUI.Views
{
    public partial class StarDetailPage : ContentPage
    {
        public StarDetailPage()
        {
            InitializeComponent();
            BindingContext = new StarDetailViewModel();
        }
    }
} 