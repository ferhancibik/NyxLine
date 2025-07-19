using Microsoft.Maui.Controls;
using NyxLine.MAUI.ViewModels;

namespace NyxLine.MAUI.Views
{
    public partial class StarsPage : ContentPage
    {
        public StarsPage()
        {
            InitializeComponent();
            BindingContext = new StarsViewModel();
        }
    }
} 