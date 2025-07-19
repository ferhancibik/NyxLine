namespace NyxLine.MAUI.Models
{
    public class ObservationSite
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }  // Rakım (metre)
        public int LightPollution { get; set; }  // Bortle ölçeği (1-9)
        public string City { get; set; }  // Şehir
        public string Region { get; set; }  // Bölge (Akdeniz, Ege vb.)
        public string BestTimeToVisit { get; set; }
        public List<string> AccessInfo { get; set; }  // Ulaşım bilgileri
        public List<string> Facilities { get; set; }  // Tesis ve imkanlar
        public List<string> RequiredEquipment { get; set; }  // Önerilen ekipmanlar
        public List<string> SafetyTips { get; set; }  // Güvenlik önerileri
        public List<string> BestObjects { get; set; }  // En iyi gözlemlenebilen gök cisimleri
        public string WeatherInfo { get; set; }  // Hava durumu özellikleri
        public bool HasParking { get; set; }
        public bool IsCampingAllowed { get; set; }
        public double DistanceFromCity { get; set; }  // Şehir merkezine uzaklık (km)
        public string ImageUrl { get; set; }
        public double DistanceFromUser { get; set; }  // Kullanıcıya olan uzaklık (hesaplanacak)
    }

    public class ObservationEquipment
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Purpose { get; set; }
        public bool IsEssential { get; set; }
        public string Category { get; set; }  // Optik, Fotoğraf, Konfor, Güvenlik vb.
        public string ImageUrl { get; set; }
        public List<string> Tips { get; set; }
    }

    public class ObservationTip
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }  // Güvenlik, Ekipman, Hava Durumu, Genel vb.
        public int Priority { get; set; }  // Önem derecesi
        public List<string> RelatedEquipment { get; set; }
        public string Icon { get; set; }
    }
} 