using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using NyxLine.MAUI.Models;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace NyxLine.MAUI.ViewModels
{
    public class ObservationViewModel : BindableObject
    {
        private List<ObservationSite> allSites;
        private double _userLatitude;
        public double UserLatitude
        {
            get => _userLatitude;
            set
            {
                _userLatitude = value;
                OnPropertyChanged();
                SearchNearbySites();
            }
        }

        private double _userLongitude;
        public double UserLongitude
        {
            get => _userLongitude;
            set
            {
                _userLongitude = value;
                OnPropertyChanged();
                SearchNearbySites();
            }
        }

        private string _userCity;
        public string UserCity
        {
            get => _userCity;
            set
            {
                _userCity = value;
                OnPropertyChanged();
                SearchNearbySites();
            }
        }

        private ObservableCollection<ObservationSite> _nearbySites;
        public ObservableCollection<ObservationSite> NearbySites
        {
            get => _nearbySites;
            set
            {
                _nearbySites = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ObservationEquipment> _recommendedEquipment;
        public ObservableCollection<ObservationEquipment> RecommendedEquipment
        {
            get => _recommendedEquipment;
            set
            {
                _recommendedEquipment = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ObservationTip> _observationTips;
        public ObservableCollection<ObservationTip> ObservationTips
        {
            get => _observationTips;
            set
            {
                _observationTips = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _observationTimes;
        public ObservableCollection<string> ObservationTimes
        {
            get => _observationTimes;
            set
            {
                _observationTimes = value;
                OnPropertyChanged();
            }
        }

        private string _selectedTime;
        public string SelectedTime
        {
            get => _selectedTime;
            set
            {
                _selectedTime = value;
                OnPropertyChanged();
            }
        }

        public ICommand GetLocationCommand { get; }
        public ICommand OpenSiteDetailsCommand { get; }
        public ICommand OpenMapCommand { get; }

        public ObservationViewModel()
        {
            GetLocationCommand = new Command(GetCurrentLocation);
            OpenSiteDetailsCommand = new Command<ObservationSite>(OpenSiteDetails);
            OpenMapCommand = new Command<ObservationSite>(OpenMap);
            
            ObservationTimes = new ObservableCollection<string>
            {
                "Gün Batımı",
                "Gece Yarısı",
                "Şafak Öncesi",
                "Tüm Gece"
            };
            
            LoadInitialData();
        }

        private async void GetCurrentLocation()
        {
            try
            {
                var location = await Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Medium,
                    Timeout = TimeSpan.FromSeconds(5)
                });

                if (location != null)
                {
                    UserLatitude = location.Latitude;
                    UserLongitude = location.Longitude;
                    
                    // Geocoding ile şehir bilgisini al
                    var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                    var placemark = placemarks?.FirstOrDefault();
                    if (placemark != null)
                    {
                        UserCity = placemark.AdminArea; // Şehir/İl bilgisi
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", "Konum alınamadı. Lütfen konum servislerini kontrol edin.", "Tamam");
            }
        }

        private void LoadInitialData()
        {
            // Tüm gözlem noktaları listesi
            allSites = new List<ObservationSite>
            {
                // Akdeniz Bölgesi
                new ObservationSite
                {
                    Id = "saklıkent",
                    Name = "Saklıkent",
                    Description = "Antalya'nın yüksek rakımlı bölgesi, ışık kirliliğinden uzak mükemmel bir gözlem noktası.",
                    Latitude = 36.8372,
                    Longitude = 30.3367,
                    Altitude = 2400,
                    LightPollution = 2,
                    City = "Antalya",
                    Region = "Akdeniz",
                    BestTimeToVisit = "Aralık-Mart arası",
                    AccessInfo = new List<string>
                    {
                        "Antalya merkezden 1.5 saat sürüş mesafesi",
                        "Özel araç veya tur ile ulaşım mümkün",
                        "Son kısım stabilize yol"
                    },
                    Facilities = new List<string>
                    {
                        "Kamp alanı mevcut",
                        "Elektrik yok - powerbank önerilir",
                        "Temel ihtiyaçlar için yakın tesisler"
                    },
                    RequiredEquipment = new List<string>
                    {
                        "Orta seviye teleskop (Celestron 70AZ veya Dobsonian 6\")",
                        "Kırmızı ışık fener",
                        "Termal mont",
                        "Tripodlu dürbün"
                    },
                    SafetyTips = new List<string>
                    {
                        "Yeni ay zamanları tercih edilmeli",
                        "Kuzeydoğu yönüne doğru gözlem yapın (Samanyolu için)",
                        "Gece görüşünü korumak için kırmızı ışık kullanın"
                    },
                    BestObjects = new List<string>
                    {
                        "Samanyolu",
                        "Derin uzay objeleri",
                        "Gezegenler"
                    },
                    WeatherInfo = "Kış aylarında yıldızlar çok net görünür",
                    HasParking = true,
                    IsCampingAllowed = true,
                    DistanceFromCity = 50
                },
                new ObservationSite
                {
                    Id = "bakırlıtepe",
                    Name = "Bakırlıtepe",
                    Description = "TÜBİTAK Ulusal Gözlemevi'nin bulunduğu, Türkiye'nin en önemli astronomi merkezlerinden biri.",
                    Latitude = 36.8249,
                    Longitude = 30.3354,
                    Altitude = 2500,
                    LightPollution = 1,
                    City = "Antalya",
                    Region = "Akdeniz",
                    BestTimeToVisit = "Tüm yıl (özellikle yaz ayları)",
                    AccessInfo = new List<string>
                    {
                        "Özel araç veya organize turlar",
                        "Antalya'dan yaklaşık 2.5 saat",
                        "Son bölüm stabilize yol"
                    },
                    Facilities = new List<string>
                    {
                        "Profesyonel gözlemevi",
                        "Sınırlı elektrik imkanı",
                        "Ziyaretçi merkezi"
                    },
                    RequiredEquipment = new List<string>
                    {
                        "Dürbün veya teleskop",
                        "Kırmızı ışık fener",
                        "Mat/sandalye",
                        "Yedek pil ve su"
                    },
                    SafetyTips = new List<string>
                    {
                        "Önceden randevu gerekebilir",
                        "Yeni ay dönemlerini tercih edin",
                        "Gece soğuk olabilir"
                    },
                    HasParking = true,
                    IsCampingAllowed = false,
                    DistanceFromCity = 60
                },
                new ObservationSite
                {
                    Id = "beydaglari",
                    Name = "Beydağları Milli Parkı",
                    Description = "Işık kirliliğinden uzak, yüksek rakımlı mükemmel bir gözlem noktası.",
                    Latitude = 36.7000,
                    Longitude = 30.4000,
                    Altitude = 2000,
                    LightPollution = 2,
                    City = "Antalya",
                    Region = "Akdeniz",
                    BestTimeToVisit = "Nisan-Ekim arası",
                    AccessInfo = new List<string>
                    {
                        "Özel araç veya toplu taşıma + yürüyüş",
                        "Antalya'dan yaklaşık 2 saat"
                    },
                    RequiredEquipment = new List<string>
                    {
                        "Dürbün veya teleskop",
                        "Kırmızı ışık fener",
                        "Mat/sandalye",
                        "Yedek pil ve su"
                    },
                    SafetyTips = new List<string>
                    {
                        "Yeni ay dönemlerini tercih edin",
                        "Yanınızda rehber bulundurun",
                        "Hava durumunu kontrol edin"
                    },
                    HasParking = true,
                    IsCampingAllowed = true,
                    DistanceFromCity = 45
                },
                // Ege Bölgesi
                new ObservationSite
                {
                    Id = "spil-dagi",
                    Name = "Spil Dağı",
                    Description = "Manisa'nın doğal güzelliği, gece gökyüzü gözlemi için ideal koşullar sunar.",
                    Latitude = 38.5667,
                    Longitude = 27.4333,
                    Altitude = 1517,
                    LightPollution = 3,
                    City = "Manisa",
                    Region = "Ege",
                    BestTimeToVisit = "Mayıs-Eylül arası",
                    AccessInfo = new List<string>
                    {
                        "Manisa merkezden 45 dakika",
                        "Özel araç önerilir"
                    },
                    RequiredEquipment = new List<string>
                    {
                        "Dürbün veya teleskop",
                        "Kırmızı ışık fener",
                        "Mat/sandalye",
                        "Yedek pil ve su"
                    },
                    HasParking = true,
                    IsCampingAllowed = true,
                    DistanceFromCity = 25
                },
                // Karadeniz Bölgesi
                new ObservationSite
                {
                    Id = "kackar-daglari",
                    Name = "Kaçkar Dağları",
                    Description = "Yüksek rakımı ve temiz havası ile mükemmel gözlem koşulları sunar.",
                    Latitude = 40.8333,
                    Longitude = 41.1667,
                    Altitude = 3000,
                    LightPollution = 1,
                    City = "Rize",
                    Region = "Karadeniz",
                    BestTimeToVisit = "Temmuz-Eylül arası",
                    AccessInfo = new List<string>
                    {
                        "Rize'den özel araç ile ulaşım",
                        "Son kısım yürüyüş gerektirir"
                    },
                    RequiredEquipment = new List<string>
                    {
                        "Dürbün veya teleskop",
                        "Kırmızı ışık fener",
                        "Mat/sandalye",
                        "Yedek pil ve su"
                    },
                    HasParking = true,
                    IsCampingAllowed = true,
                    DistanceFromCity = 70
                }
                // Diğer bölgeler için gözlem noktaları eklenebilir
            };

            // Önerilen ekipmanlar
            RecommendedEquipment = new ObservableCollection<ObservationEquipment>
            {
                new ObservationEquipment
                {
                    Name = "Teleskop",
                    Description = "Orta seviye teleskop (Celestron 70AZ veya Dobsonian 6\")",
                    Purpose = "Detaylı gök cismi gözlemi için",
                    IsEssential = true,
                    Category = "Optik",
                    Tips = new List<string>
                    {
                        "Kullanmadan önce ayarları kontrol edin",
                        "Lensleri temiz tutun",
                        "Tripod kullanın"
                    }
                },
                new ObservationEquipment
                {
                    Name = "Kırmızı Işıklı El Feneri",
                    Description = "Gece görüşünü bozmayan kırmızı ışıklı el feneri",
                    Purpose = "Karanlıkta güvenli hareket ve gece görüşünü korumak için",
                    IsEssential = true,
                    Category = "Güvenlik",
                    Tips = new List<string>
                    {
                        "Pil seviyesini kontrol edin",
                        "Yedek pil bulundurun",
                        "Beyaz ışık kullanmaktan kaçının"
                    }
                },
                new ObservationEquipment
                {
                    Name = "Termal Kıyafetler",
                    Description = "Soğuk hava koşullarına uygun termal mont ve kıyafetler",
                    Purpose = "Uzun süreli gözlemler için konfor",
                    IsEssential = true,
                    Category = "Konfor",
                    Tips = new List<string>
                    {
                        "Katmanlı giyinin",
                        "Su geçirmez olmasına dikkat edin",
                        "Eldiven ve bere unutmayın"
                    }
                },
                new ObservationEquipment
                {
                    Name = "Tripodlu Dürbün",
                    Description = "10x50 veya benzer güçte dürbün ve tripod",
                    Purpose = "Geniş alan gözlemi ve yıldız bulma",
                    IsEssential = false,
                    Category = "Optik",
                    Tips = new List<string>
                    {
                        "Tripod kullanımı titreşimi önler",
                        "Lensleri temiz tutun",
                        "Taşıma çantası kullanın"
                    }
                },
                new ObservationEquipment
                {
                    Name = "Powerbank",
                    Description = "Yüksek kapasiteli taşınabilir şarj cihazı",
                    Purpose = "Elektronik cihazlar için güç kaynağı",
                    IsEssential = true,
                    Category = "Elektronik",
                    Tips = new List<string>
                    {
                        "Tam şarjlı olduğundan emin olun",
                        "Uygun kablolarınızı yanınızda bulundurun",
                        "Soğuktan koruyun"
                    }
                },
                new ObservationEquipment
                {
                    Name = "Kamp Sandalyesi/Mat",
                    Description = "Rahat gözlem için katlanabilir sandalye veya mat",
                    Purpose = "Konforlu gözlem deneyimi",
                    IsEssential = true,
                    Category = "Konfor",
                    Tips = new List<string>
                    {
                        "Su geçirmez olmasına dikkat edin",
                        "Hafif ve taşınabilir olsun",
                        "Yedek battaniye bulundurun"
                    }
                }
            };

            // Gözlem ipuçları
            ObservationTips = new ObservableCollection<ObservationTip>
            {
                new ObservationTip
                {
                    Title = "Yeni Ay Takibi",
                    Description = "En iyi gözlem zamanı yeni ay dönemleridir. Ay ışığı olmadığında gökyüzü daha net görünür.",
                    Category = "Zamanlama",
                    Priority = 1,
                    Icon = "moon.png",
                    RelatedEquipment = new List<string> { "Teleskop", "Dürbün" }
                },
                new ObservationTip
                {
                    Title = "Işık Kirliliği",
                    Description = "Şehir ışıklarından uzak durun ve ışık kaynaklarından uzaklaşın. Bortle sınıfı düşük olan yerleri tercih edin.",
                    Category = "Konum",
                    Priority = 1,
                    Icon = "light.png",
                    RelatedEquipment = new List<string> { "Kırmızı Işıklı El Feneri" }
                },
                new ObservationTip
                {
                    Title = "Göz Adaptasyonu",
                    Description = "Gözlerinizin karanlığa alışması için en az 30 dakika bekleyin. Bu sürede beyaz ışık kullanmayın.",
                    Category = "Gözlem",
                    Priority = 1,
                    Icon = "eye.png",
                    RelatedEquipment = new List<string> { "Kırmızı Işıklı El Feneri" }
                },
                new ObservationTip
                {
                    Title = "Hava Durumu",
                    Description = "Açık ve bulutsuz geceleri tercih edin. Nem oranı düşük olmalı. Rüzgar hızını kontrol edin.",
                    Category = "Hava",
                    Priority = 1,
                    Icon = "weather.png",
                    RelatedEquipment = new List<string> { "Termal Kıyafetler" }
                },
                new ObservationTip
                {
                    Title = "Güvenlik",
                    Description = "Gece gözlemi için yanınızda mutlaka bir arkadaşınız olsun. Acil durum kiti bulundurun.",
                    Category = "Güvenlik",
                    Priority = 1,
                    Icon = "safety.png",
                    RelatedEquipment = new List<string> { "Powerbank", "Kırmızı Işıklı El Feneri" }
                },
                new ObservationTip
                {
                    Title = "Ekipman Hazırlığı",
                    Description = "Tüm ekipmanları önceden test edin. Pilleri kontrol edin. Lensleri temizleyin.",
                    Category = "Ekipman",
                    Priority = 2,
                    Icon = "equipment.png",
                    RelatedEquipment = new List<string> { "Teleskop", "Dürbün", "Powerbank" }
                },
                new ObservationTip
                {
                    Title = "Konfor",
                    Description = "Uzun süreli gözlemler için rahat kıyafetler ve oturma düzeni önemlidir.",
                    Category = "Konfor",
                    Priority = 2,
                    Icon = "comfort.png",
                    RelatedEquipment = new List<string> { "Kamp Sandalyesi/Mat", "Termal Kıyafetler" }
                },
                new ObservationTip
                {
                    Title = "Yön Bulma",
                    Description = "Kutup Yıldızı'nı referans alın. Gözlem yapacağınız yönü önceden belirleyin.",
                    Category = "Gözlem",
                    Priority = 2,
                    Icon = "compass.png",
                    RelatedEquipment = new List<string> { "Dürbün" }
                }
            };
        }

        private void SearchNearbySites()
        {
            // Kullanıcının konumuna göre yakındaki gözlem noktalarını filtrele
            if (UserLatitude != 0 && UserLongitude != 0 && allSites != null)
            {
                foreach (var site in allSites)
                {
                    site.DistanceFromUser = CalculateDistance(
                        UserLatitude, UserLongitude,
                        site.Latitude, site.Longitude
                    );
                }

                // En yakın 5 noktayı seç
                NearbySites = new ObservableCollection<ObservationSite>(
                    allSites.OrderBy(s => s.DistanceFromUser).Take(5)
                );
            }
            else
            {
                // Konum bilgisi yoksa tüm noktaları göster
                NearbySites = new ObservableCollection<ObservationSite>(allSites ?? new List<ObservationSite>());
            }
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // Dünya'nın yarıçapı (km)
            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRad(double degree)
        {
            return degree * Math.PI / 180;
        }

        private async void OpenSiteDetails(ObservationSite site)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Site", site }
            };
            await Shell.Current.GoToAsync("observationdetail", parameters);
        }

        private async void OpenMap(ObservationSite site)
        {
            try
            {
                var location = new Location(site.Latitude, site.Longitude);
                var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };
                await Map.OpenAsync(location, options);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", "Harita açılamadı.", "Tamam");
            }
        }
    }
} 