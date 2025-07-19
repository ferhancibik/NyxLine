using System.Collections.ObjectModel;
using NyxLine.MAUI.Models;

namespace NyxLine.MAUI.Views;

public partial class AstrologyPage : ContentPage
{
    private ObservableCollection<Models.Star> _visibleStars;
    private readonly Random _random = new Random();

    public AstrologyPage()
    {
        InitializeComponent();
        _visibleStars = new ObservableCollection<Models.Star>();
        VisibleStars.ItemsSource = _visibleStars;
        
        // Varsayılan değerleri ayarla
        ObservationDate.Date = DateTime.Today;
        ObservationTime.Time = DateTime.Now.TimeOfDay;
        
        UpdateAstrologyInfo();
    }

    private void OnDateSelected(object sender, DateChangedEventArgs e)
    {
        UpdateAstrologyInfo();
    }

    private void OnTimeChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "Time")
        {
            UpdateAstrologyInfo();
        }
    }

    private void UpdateAstrologyInfo()
    {
        var selectedDateTime = ObservationDate.Date + ObservationTime.Time;
        
        // Burç bilgilerini güncelle
        UpdateZodiacInfo(selectedDateTime);
        
        // Görünür yıldızları güncelle
        UpdateVisibleStars(selectedDateTime);
        
        // Gözlem ipuçlarını güncelle
        UpdateObservationTips(selectedDateTime);
    }

    private void UpdateZodiacInfo(DateTime dateTime)
    {
        var zodiacSigns = new[]
        {
            "♈ Koç", "♉ Boğa", "♊ İkizler", "♋ Yengeç",
            "♌ Aslan", "♍ Başak", "♎ Terazi", "♏ Akrep",
            "♐ Yay", "♑ Oğlak", "♒ Kova", "♓ Balık"
        };

        var visibleSigns = zodiacSigns
            .OrderBy(x => _random.Next())
            .Take(4)
            .ToList();

        ZodiacInfo.Text = $"Görünür Burçlar:\n{string.Join("\n", visibleSigns)}";
    }

    private void UpdateVisibleStars(DateTime dateTime)
    {
        var stars = new[]
        {
            new Models.Star { Icon = "⭐", Name = "Sirius", Description = "Gökyüzünün en parlak yıldızı", Visibility = "Mükemmel" },
            new Models.Star { Icon = "⭐", Name = "Vega", Description = "Lir takımyıldızının en parlak yıldızı", Visibility = "Çok İyi" },
            new Models.Star { Icon = "⭐", Name = "Antares", Description = "Akrep takımyıldızının kırmızı süper devi", Visibility = "İyi" },
            new Models.Star { Icon = "⭐", Name = "Aldebaran", Description = "Boğa takımyıldızının turuncu devi", Visibility = "Mükemmel" },
            new Models.Star { Icon = "⭐", Name = "Betelgeuse", Description = "Avcı takımyıldızının kırmızı süper devi", Visibility = "Çok İyi" },
            new Models.Star { Icon = "⭐", Name = "Rigel", Description = "Avcı takımyıldızının mavi süper devi", Visibility = "İyi" },
            new Models.Star { Icon = "⭐", Name = "Polaris", Description = "Kutup Yıldızı", Visibility = "Mükemmel" },
            new Models.Star { Icon = "⭐", Name = "Arcturus", Description = "Çoban takımyıldızının turuncu devi", Visibility = "Çok İyi" }
        };

        _visibleStars.Clear();
        foreach (var star in stars.OrderBy(x => _random.Next()).Take(5))
        {
            _visibleStars.Add(star);
        }
    }

    private void UpdateObservationTips(DateTime dateTime)
    {
        var tips = new[]
        {
            "🌙 Yeni ay dönemi - gözlem için ideal zaman!",
            "☁️ Açık gökyüzü bekleniyor - teleskop kullanımı için uygun",
            "🌡️ Sıcaklık uygun - uzun süreli gözlem yapılabilir",
            "💨 Rüzgar hafif - görüş netliği yüksek",
            "🌫️ Nem düşük - lens buğulanması riski az"
        };

        ObservationTips.Text = tips[_random.Next(tips.Length)];
    }
}

public class Star
{
    public string Icon { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Visibility { get; set; }
} 