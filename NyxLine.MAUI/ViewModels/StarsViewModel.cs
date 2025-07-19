using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using NyxLine.MAUI.Models;
using NyxLine.MAUI.Views;

namespace NyxLine.MAUI.ViewModels
{
    public class StarsViewModel : BindableObject
    {
        private ObservableCollection<Models.Star> _constellations;
        public ObservableCollection<Models.Star> Constellations
        {
            get => _constellations;
            set
            {
                _constellations = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Models.Star> _importantStars;
        public ObservableCollection<Models.Star> ImportantStars
        {
            get => _importantStars;
            set
            {
                _importantStars = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenConstellationCommand { get; }
        public ICommand OpenStarCommand { get; }

        public StarsViewModel()
        {
            OpenConstellationCommand = new Command<Models.Star>(async (star) => await OpenStarDetail(star));
            OpenStarCommand = new Command<Models.Star>(async (star) => await OpenStarDetail(star));
            LoadData();
        }

        private async Task OpenStarDetail(Models.Star star)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Star", star }
            };
            await Shell.Current.GoToAsync("stardetail", parameters);
        }

        private void LoadData()
        {
            // Takımyıldızları
            Constellations = new ObservableCollection<Models.Star>
            {
                new Models.Star
                {
                    Id = "orion",
                    Name = "Avcı (Orion)",
                    ShortDescription = "\"The scorpion aimed its deadly stingers at Leto. Orion saw this, and threw himself in front of her, shielding her with his body. … Artemis and Leto… put him among the stars.\"",
                    Description = "Orion takımyıldızı, gökyüzünün en belirgin ve tanınabilir takımyıldızlarından biridir. Betelgeuse ve Rigel gibi parlak yıldızları ve ünlü Orion Kuşağı'nı içerir. 88 modern takımyıldızdan biri olup, 594 derece karelik alanıyla 26. en büyük takımyıldızdır.",
                    MythologicalStory = "Orion, Yunan mitolojisinde Poseidon ve Euryale'nin oğlu olan devasa ve yakışıklı bir avcıdır. Deniz tanrısının oğlu olması sebebiyle su üzerinde yürüyebilirdi. Chios Adası'nda Merope'yi kaçırınca Kral Oenopion onu kör ederek adadan kovar. Lemnos Adası'na gidip Hephaestus'un kölesi Cedalion'un rehberliğinde doğuya varır; burada Helios onu iyileştirir. Artemis ve Leto ile avlanırken tüm hayvanları yok edeceğini söylemesi üzerine Gaia veya Apollo, dev bir akrebi göndererek Orion'u öldürtür. Zeus, Olympos'taki Avcı'yı gökyüzüne yerleştirir; öldüren akrep de Scorpius olarak karşı yanda göze çarpar, böylece ikisi aynı anda görünmez. Alternatif bir efsaneye göre Artemis'in kendi aşkını hedef alarak onu yanlışlıkla öldürmesiyle de ölüm gerçekleşir.",
                    HowToFind = "Kasım–Şubat aylarında, özellikle Ocak akşamları Kuzey Yarımküre'de güney-batı yönünde yüksek olarak görülür. Ekvator bölgelerinde yıl boyunca gözlemlenebilir. En kolay tanıma yöntemi üç yıldızdan oluşan 'Orion Kuşağı'dır (Alnitak, Alnilam, Mintaka). Kuşak üzerindeki iki parlak yıldız Betelgeuse (omuz) ve Rigel (diz) bulunur. Kuşağın altındaki 'kılıç'ta Orion Bulutsusu (M42) gibi ilginç yapılar bulunur. Navigasyon için kuşak çizgisi uzatıldığında Sirius (köpek yıldızı) ve Aldebaran gibi yıldızlara ulaşılır.",
                    InterestingFacts = new List<string>
                    {
                        "Rigel (Beta) ve Betelgeuse (Alpha) yıldızları gökyüzünün en parlak ilk 10 yıldızı arasındadır",
                        "Orion Bulutsusu M42, kılıcın ortasında yer alan, çıplak gözle görülebilen parlak bir yıldız doğum bölgesidir",
                        "Her yıl Ekim sonunda Orionids meteor yağmuru gerçekleşir, Halley Kuyrukluyıldızı kalıntılarından kaynaklanır",
                        "MS 14.000 yılında Dünya'nın eksen kayması sebebiyle yüksek enlemlerde görünmeyecek",
                        "Antik Mısır'da Osiris, Babil'de Gilgamesh, Asya halklarında avcı figürünü temsil eder",
                        "Rigel, mavi süperdev olup 848 ışık yılı uzaklıkta ve 120.000 güneş parlaklığındadır",
                        "Orion Kuşağı'ndaki yıldızlar (Alnitak, Alnilam, Mintaka) yaklaşık 2.300°C sıcaklığında mavi dev yıldızlardır",
                        "Takımyıldız içinde M42, M43, Alev Bulutsusu, Running Man Bulutsusu, Barnard's Loop, Atbaşı Bulutsusu gibi önemli derin uzay nesneleri bulunur"
                    },
                    IsConstellation = true,
                    ImageUrl = "orion.png",
                    AnimatedImageUrl = "orion_animated.gif",
                    BestViewingSeason = "Kasım-Şubat arası, özellikle Ocak ayı"
                },
                new Models.Star
                {
                    Id = "ursa-major",
                    Name = "Büyük Ayı (Ursa Major)",
                    ShortDescription = "\"One version of the myth, it is Zeus who transforms Callisto into a bear … He then throws their bear form into the sky … Ursa Major represents Callisto, while Ursa Minor represents Arcas.\"",
                    Description = "Büyük Ayı takımyıldızı, gökyüzünün üçüncü en büyük takımyıldızıdır ve gökyüzünün %3'ünü (yaklaşık 1.280 derece²) kaplar. En tanınabilir özelliği Büyük Kepçe (Big Dipper) asterizmidir ve Kuzey Yarımküre'de yıl boyunca görülebilir.",
                    MythologicalStory = "Callisto, Artemis'in avcı yoldaşlarından güzel ve sadık bir nimf olarak doğar. Artemis ile bakire kalmayı vaat eden Callisto'yu Zeus görür, bazılarının dediğine göre Artemis'e bürünerek onu kandırır ve hamile bırakır. Artemis, bu durumu öğrenince öfkeyle Callisto'yu ayıya dönüştürür. Kızıyla ormanda gezen Callisto, oğlu Arcas tarafından avlanmak üzereyken Zeus onları göğe alarak Ursa Major (Callisto) ve Ursa Minor (Arcas) takımyıldızları olarak yerleştirir. Hera ise onların bir daha su içmemesi için Tethys'e yön verir.",
                    HowToFind = "Büyük Kepçe (Big Dipper) asterizmi Ursa Major'un en tanıdık şeklidir: dörtlü bir kova ve üç yıldızlı sap kısmından oluşur. Kutup Yıldızı'na (Polaris) erişim için Dubhe ve Merak yıldızlarını birleştirip çizgiyi uzattığınızda kolaylıkla bulunabilir. Kuzey yarımkürede yıl boyunca, özellikle ilkbaharda (Nisan) yüksek konumda görülür. Sağ açıklık ~11 saat, deklinasyon +50° civarındadır ve yaklaşık 55°'lik genişlikte yer kaplar.",
                    InterestingFacts = new List<string>
                    {
                        "Mizar yıldızı, çıplak gözle görülebilen ilk ikili yıldız (Alcor) sistemine sahiptir ve teleskopla dört bileşeni olduğu bulunmuştur",
                        "Takımyıldız içinde 7 Messier derin gökyüzü nesnesi bulunur: M40, M81, M82, M97 (Baykuş Bulutsusu), M101 (Pinwheel), M108 ve M109",
                        "M81 ve M82 galaksileri yaklaşık 12 milyon ışık yılı uzaklıkta ve teleskopla kolayca gözlemlenebilir",
                        "Kuzey Yarımküre'de circumpolardır; yani hiç batmaz ve gökyüzünde sürekli görülebilir",
                        "7 parlak yıldız (Alpha Dubhe, Beta Merak, Gamma Phecda, Delta Megrez, Epsilon Alioth, Zeta Mizar, Eta Alkaid) ve 20'den fazla zayıf yıldız içerir",
                        "Sistemlerinde toplam 21 gezegen barındırır",
                        "Ursa Major Moving Group, asterizmin ortasındaki beş yıldızı içerir; Dubhe ve Alkaid ise gruptan bağımsızdır"
                    },
                    IsConstellation = true,
                    ImageUrl = "ursa_major.png",
                    AnimatedImageUrl = "ursa_major_animated.gif",
                    BestViewingSeason = "Tüm yıl, özellikle ilkbahar (Nisan)"
                },
                new Models.Star
                {
                    Id = "cassiopeia",
                    Name = "Cassiopeia (Kraliçe)",
                    ShortDescription = "\"Cassiopeia was condemned to circle the celestial pole forever, and spends half of the year upside down in the sky as further punishment for her vanity.\"",
                    Description = "Cassiopeia takımyıldızı, gökyüzünde belirgin bir W veya M harfi şeklinde görünen beş parlak yıldızdan oluşur. 598 derece karelik alanıyla 88 takımyıldız arasında 25. sırada yer alır ve Kuzey Yarımküre'den yıl boyunca görülebilir.",
                    MythologicalStory = "Cassiopeia, Etiyopya Kralı Cepheus'la evli ve Andromeda'nın annesiydi. Kendisi ve kızı Andromeda'nın deniz perilerinden (Nereid'lerden) daha güzel olduğunu iddia edince, deniz tanrısı Poseidon bunu cezasız bırakmaz. Karaya büyük bir canavar, Cetus gönderilir ve Andromeda kayalıklara zincirlenir. Son anda Perseus canavarı öldürüp Andromeda'yı kurtarır. Cassiopeia ise gururuyla cezalandırılır — gökyüzünde tahttayken zincire vurulur ve yılda yarım yıl 'baş aşağı' görünerek alay konusu olur.",
                    HowToFind = "Cassiopeia açık ve tanınabilir 'W' veya ters 'M' formundadır; beş parlak yıldız oluşturur: Segin, Ruchbah, Gamma, Schedar ve Caph. Kutupsal (Circumpolar) yapı sayesinde Kuzey Yarımküre'den yıl boyunca görülebilir. Polaris'e referansla: Büyük Ayı'nın kepçesi ile Kastiopeia arasında kalan W şekli Kuzey Yıldızı ile hizalanınca hemen fark edilir.",
                    InterestingFacts = new List<string>
                    {
                        "En parlak yıldızı Schedar (α Cas), turuncu bir dev yıldızdır ve 229 ışık yılı uzaklıktadır. Adı Arapça'da 'göğüs' anlamına gelir",
                        "Ruchbah (δ Cas) ikili yıldız sistemidir; Gamma Cassiopeiae ise parlaklığı değişen bir yıldızdır",
                        "M52 açık yıldız kümesi yaklaşık 100 yıldız içerir",
                        "M103 kümesi yaklaşık 25 yıldız barındırır",
                        "Cassiopeia A, Samanyolu dışındaki en parlak radyo kaynaklarından biridir ve bir süpernova kalıntısıdır",
                        "Kalp ve Ruh Bulutsuları, HFG1 gezegenimsi bulutsu, Kabarcık Bulutsusu gibi ilginç yapılar içerir",
                        "5 ana parlak yıldız ve toplam 53 orta parlaklıkta yıldız içerir, bunların 14'ü gezegen barındıran sistemlerdir"
                    },
                    IsConstellation = true,
                    ImageUrl = "cassiopeia.png",
                    AnimatedImageUrl = "cassiopeia_animated.gif",
                    BestViewingSeason = "Tüm yıl, özellikle Eylül-Kasım arası"
                },
                new Models.Star
                {
                    Id = "scorpius",
                    Name = "Akrep (Scorpius)",
                    ShortDescription = "\"Scorpius is a zodiac constellation particularly well-known … its patterns resemble the scorpion which it represents.\"",
                    Description = "Scorpius takımyıldızı, gökyüzünde gerçekten bir akrebe benzeyen nadir takımyıldızlarından biridir. 497 derece karelik alanıyla 33. en büyük takımyıldızdır. Kalbinde parlak kırmızı yıldız Antares bulunur ve Zodyak'ın sekizinci burcudur.",
                    MythologicalStory = "Scorpius, Yunan mitolojisinde kibirli avcı Orion'u öldüren akreptir. Orion, 'Dünyadaki hiçbir hayvanı öldüremeyeceğini' iddia edince ya Gaia ya da Artemis/Apollo tarafından gönderildi. Ölümünün ardından Zeus, her iki figürü gökyüzüne yerleştirir ve onları birbirinden uzak tutar: Scorpius yükseldiğinde Orion batar, böylece düşmanlar asla karşılaşmaz. Diğer kültürlerde farklı anlamlar taşır: Hawai'de Maui'nin sihirli balık kancası, Babil'de ise yeraltı tanrısını bekleyen dev bir akrep olarak görülürdü.",
                    HowToFind = "Kuzey yarımkürede Haziran–Ağustos aylarında güney ufkunun üzerinde görülür. Güney yarımkürede bu dönemde yükselir, kuzeyde ise ufka yakındır. Çevresinde Libra (batı) ve Sagittarius (doğu) takımyıldızları bulunur. Kanca gibi kıvrılan gövdesiyle akrep figürü hissi verir; kalbinde parlak kırmızı yıldız Antares, kuyruğunda Shaula (kıvrım ucunda) göze çarpar. Scorpius yükseldiğinde Orion ufkun altındadır—bu, mitolojik düşmanlığın göksel temsilidir.",
                    InterestingFacts = new List<string>
                    {
                        "Antares (Alpha Scorpii) kırmızı süperdev bir yıldızdır, 550 ışık yılı uzaklıkta ve Güneş'ten 10.000 kat daha parlaktır",
                        "Antares'in çapı Güneş'in 883 katıdır ve adı 'Mars'ın rakibi' anlamına gelir",
                        "Shaula (Lambda Scorpii) kuyruk ucundaki parlak yıldız olup üçlü bir sistemdir",
                        "Dschubba (Delta Scorpii) kafa bölgesinde yer alan çoklu yıldız sistemidir",
                        "M4, en yakın küresel yıldız kümesidir ve Antares'in yanında teleskopla görülebilir",
                        "M6 (Kelebek Kümesi) ve M7 (Ptolemy Kümesi) çıplak gözle görülebilen açık yıldız kümeleridir",
                        "NGC 6302 (Bug Nebula), NGC 6334 (Cat's Paw Nebula), M80 gibi ilginç derin uzay nesneleri içerir",
                        "Alpha Scorpiids meteor yağmuru 16 Nisan–9 Mayıs arası, June Scorpiids ise Haziran başında gerçekleşir"
                    },
                    IsConstellation = true,
                    ImageUrl = "scorpius.png",
                    AnimatedImageUrl = "scorpius_animated.gif",
                    BestViewingSeason = "Haziran-Ağustos arası"
                }
            };

            // Önemli Yıldızlar
            ImportantStars = new ObservableCollection<Models.Star>
            {
                new Models.Star
                {
                    Id = "sirius",
                    Name = "Sirius (Akyıldız)",
                    ShortDescription = "\"It is curious that many of them also considered Sirius to be a dog or associated with dogs.\"",
                    Description = "Sirius, Dünya'dan görülen en parlak yıldızdır (görünür büyüklük −1,46) ve Büyük Köpek (Canis Major) takımyıldızının alfa yıldızıdır. Samanyolu'na en yakın yıldızlardan biri olup, sadece 8,6 ışık yılı uzaklıktadır.",
                    MythologicalStory = "Yunan ve Roma mitolojisinde 'Seirios' (parlayan/yanan) adıyla bilinir ve Orion'un av köpeğidir. Homer, onu 'Orion'un köpeği... en parlak yıldız' olarak tanımlar ve uğursuzlukla ilişkilendirir. Antik Mısır'da 'Sopdet' ya da 'Sothis' olarak tapınıldı ve bu yıldızın heliakal doğuşu Nil'in taşmasını müjdelerdi, takvim düzenlemesinde büyük rol oynadı. Diğer medeniyetlerde de önemli bir yere sahiptir: Çin'de 'Göksel kurt' (Tiānláng), Polinezya'da 'Manu' kuş takımyıldızı içinde, Yerli Amerikan halklarında 'Ay köpeği' veya 'Kurt yıldızı' olarak anılır. Kuran'da ise 'Şi'râ' olarak anılır ve 'o, Şi'râ'nın Rabbidir' ifadesiyle yüceliği dile getirilir.",
                    HowToFind = "Sirius, Canis Major (Büyük Köpek) takımyıldızının en parlak yıldızıdır. En kolay bulunma yöntemi Orion Kuşağı'nı (Alnitak-Alnilam-Mintaka) takip etmektir - kuşaktan uzanan kurgusal çizgi doğrudan Sirius'u gösterir. Kuzey Yarımküre'de Temmuz–Ağustos aylarında sabahları doğudan yükselir, Güney Yarımküre'de ise akşam gökyüzünün yükseklerinde görünür. Betelgeuse ve Procyon ile birlikte 'Kış Üçgeni' asterizmini oluşturur.",
                    InterestingFacts = new List<string>
                    {
                        "Sirius aslında bir çift yıldız sistemidir: Sirius A ve Sirius B",
                        "Sirius A, mavi-beyaz bir ana seri yıldızdır ve Güneş'ten yaklaşık 25 kat daha parlaktır",
                        "Sirius A'nın yüzey sıcaklığı yaklaşık 9.940 Kelvin, çapı Güneş'in 1.7 katıdır",
                        "Sirius B, 1862'de keşfedilen ilk beyaz cücedir, kütlesi Güneş kadar ancak çapı Dünya büyüklüğündedir",
                        "İki yıldız arası ortalama mesafe 20 astronomik birimdir ve 50 yılda bir birbirlerinin etrafında dönerler",
                        "'Dog Days' (köpek günleri) terimi, Sirius'un sıcak yaz günleriyle örtüşmesinden gelir",
                        "İsmi Yunanca'da 'parıldayan/yanıcı', Arapça'da 'Al Shira' (önder, dominant) anlamına gelir",
                        "Antik Mısır'da Nil'in taşmasını haber veren kutsal bir yıldız olarak kabul edilirdi"
                    },
                    IsConstellation = false,
                    ImageUrl = "sirius.png",
                    AnimatedImageUrl = "sirius_animated.gif",
                    Magnitude = -1.46,
                    DistanceFromEarth = 8.6,
                    Constellation = "Canis Major",
                    BestViewingSeason = "Kış ayları, özellikle Aralık-Mart arası"
                },
                new Models.Star
                {
                    Id = "betelgeuse",
                    Name = "Betelgeuse (Orion'un Omzu)",
                    ShortDescription = "\"Betelgeuse has long been regarded as a sign of change in the heavens due to its variable brightness.\"",
                    Description = "Betelgeuse, Orion takımyıldızının sol omzunu oluşturan devasa bir kırmızı süperdev yıldızdır. Adını Arapça 'yad al-jauzā' (Orion'un eli) ifadesinden alır. Çapı Güneş'in yaklaşık 700 katı olan bu yıldız, yaklaşık 700 ışık yılı uzaklıkta bulunur ve parlaklığı düzensiz olarak değişir.",
                    MythologicalStory = "Betelgeuse, Orion takımyıldızının sol omzunu simgeler ve mitolojide dev avcı Orion'un gücünün ve kudretinin simgesi olarak kabul edilir. Bazı kültürlerde savaşçılığın, cesaretin ve koruyuculuğun sembolü olmuştur. Ayrıca, bazı eski uygarlıklar Betelgeuse'un parlaklığındaki değişimleri gökyüzünden gelecek önemli olayların işareti olarak yorumlamıştır.",
                    HowToFind = "Orion takımyıldızının en parlak ikinci yıldızı olan Betelgeuse, parlak turuncu-kırmızı rengi ve omuz konumuyla kolayca tanınır. Gece gökyüzünde Orion'un avcı figürünü bulduğunuzda, Betelgeuse solda, sağda ise parlak mavi Rigel vardır. Kuzey yarımkürede kış aylarında akşam gökyüzünde kolayca görünür.",
                    InterestingFacts = new List<string>
                    {
                        "Çapı Güneş'ten yaklaşık 700 kat büyüktür - Güneş yerine Betelgeuse olsaydı, yörüngesi Mars'ın ötesine kadar uzanırdı",
                        "Parlaklığı düzensiz olarak değişir ve bazen gözle fark edilir şekilde solup parlar",
                        "Yakın gelecekte süpernova olarak patlayacağı öngörülüyor, bu olay insanlık tarihinin en büyük gök olaylarından biri olabilir",
                        "2019-2020 yıllarında parlaklığında aniden büyük bir düşüş yaşanması bilim insanlarının dikkatini çekmiştir",
                        "Yüzey sıcaklığı yaklaşık 3.500 Kelvin olup, Güneş'ten çok daha soğuktur",
                        "Spektral tipi M1-2 Ia-Iab olan bir kırmızı süperdevdir",
                        "Görünür parlaklığı 0.0 ile 1.3 arasında değişir",
                        "Çapı yaklaşık 950-1.200 milyon kilometre arasındadır"
                    },
                    IsConstellation = false,
                    ImageUrl = "betelgeuse.png",
                    AnimatedImageUrl = "betelgeuse_animated.gif",
                    Magnitude = 0.42,  // Ortalama parlaklık
                    DistanceFromEarth = 700,
                    Constellation = "Orion",
                    BestViewingSeason = "Kış ayları"
                },
                new Models.Star
                {
                    Id = "vega",
                    Name = "Vega (Düşen Kartal)",
                    ShortDescription = "\"Vega is a bright beacon in the northern sky, inspiring stories of love and music throughout cultures.\"",
                    Description = "Vega, Lyra (Çalgı) takımyıldızının en parlak yıldızıdır. Adı Arapça 'Saq al-Waqi' yani 'düşen kartal' anlamından türemiştir. Gökyüzünün en parlak yıldızlarından biri olup, Güneş'ten yaklaşık 25 ışık yılı uzaklıktadır ve yaklaşık 450 milyon yaşında genç bir yıldızdır.",
                    MythologicalStory = "Mitolojide Vega, Yıldız Çalgısı'nı temsil eder; bu çalgı Orpheus'un efsanevi lirinden esinlenmiştir ve gökyüzündeki müzik ve sanatın simgesi olarak kabul edilir. Çin mitolojisinde 'Zhinu' olarak bilinir ve 'Tekrar Kavuşan Aşıklar' efsanesinin bir parçasıdır; bu efsaneye göre, Vega ve Altair yıldızları, yalnızca yılda bir kez, Kuğu Nehri üzerinden buluşur.",
                    HowToFind = "Vega, yaz aylarında kuzey yarımkürede çok parlak bir yıldız olarak görülür. Lyra takımyıldızında bulunur ve özellikle yazın akşam saatlerinde gökyüzünde kolayca fark edilir. Kuzey Yarımküre'de Polaris'in ardından en parlak ikinci yıldızdır. Yaz Üçgeni'nin (Summer Triangle) köşelerinden biridir; diğer köşeleri Deneb ve Altair'dir.",
                    InterestingFacts = new List<string>
                    {
                        "Çevresinde gezegen oluşumuna işaret eden bir toz diski bulunur",
                        "Uzun zaman boyunca kuzey gökyüzünün kutup yıldızı olmuştur ve yaklaşık 12.000 yıl sonra tekrar kutup yıldızı olacaktır",
                        "UV ve mavi ışıkta çok parlak olan A0V sınıfı bir yıldızdır",
                        "Çok hızlı döner, rotasyon hızı yaklaşık 274 km/s'dir (Güneş'ten çok daha hızlı)",
                        "Yüzey sıcaklığı yaklaşık 9.600 Kelvin'dir",
                        "Çapı Güneş'in yaklaşık 2.3 katıdır",
                        "Görünür parlaklığı 0.03'tür ve çıplak gözle kolayca görülebilir",
                        "Yaz Üçgeni asterizminin en parlak köşesini oluşturur"
                    },
                    IsConstellation = false,
                    ImageUrl = "vega.png",
                    AnimatedImageUrl = "vega_animated.gif",
                    Magnitude = 0.03,
                    DistanceFromEarth = 25,
                    Constellation = "Lyra",
                    BestViewingSeason = "Yaz ayları"
                },
                new Models.Star
                {
                    Id = "antares",
                    Name = "Antares (Akrep'in Kalbi)",
                    ShortDescription = "\"Antares shines fiercely as the fiery heart of the Scorpion, a celestial warrior in the night sky.\"",
                    Description = "Antares, Akrep (Scorpius) takımyıldızının kalbinde yer alan devasa bir kırmızı süperdev yıldızdır. Adını 'Mars'a karşı' anlamına gelen 'Anti-Ares' kelimelerinden alır. Çapı Güneş'in yaklaşık 700 katı olan bu yıldız, yaklaşık 550 ışık yılı uzaklıkta bulunur ve parlaklığı Güneş'in 10.000 katıdır.",
                    MythologicalStory = "Yunan mitolojisinde, Akrep Orion'u öldürmek için gönderilen kutsal yaratık olarak bilinir; Antares, bu savaşın kalbindeki ateşli yıldızdır. Birçok kültürde savaş, güç ve yıkım sembolü olarak görülmüştür; ancak aynı zamanda yeniden doğuş ve dönüşümle de ilişkilendirilir. Mars gezegenine benzer parlak kırmızı rengi nedeniyle 'Mars'ın rakibi' olarak adlandırılmıştır.",
                    HowToFind = "Antares, Scorpius takımyıldızının en parlak yıldızıdır ve parlak kırmızı rengiyle kolayca ayırt edilir. Güney ufkunda Haziran'dan Ağustos'a kadar gözlemlenebilir, özellikle gece yarısına doğru gökyüzünün doğu tarafında yükselir. Çevresinde, M4 adlı parlak küresel yıldız kümesi ve diğer açık kümelerle birlikte bulunur. Orion takımyıldızının solundaki parlak Betelgeuse'a benzer kırmızı tonuyla dikkat çeker.",
                    InterestingFacts = new List<string>
                    {
                        "Çapı Güneş'in yaklaşık 700-880 katı büyüklüğündedir (700-880 milyon km)",
                        "Aslında bir çift yıldız sistemidir; ana yıldızın yanında daha küçük ve mavi-beyaz bir yıldız (Antares B) bulunur",
                        "Yüzey sıcaklığı yaklaşık 3.400 Kelvin'dir",
                        "Spektral tipi M1.5Iab olan bir kırmızı süperdevdir",
                        "Görünür parlaklığı ortalama +1.0'dir ve zaman zaman değişkenlik gösterir",
                        "M4 küresel yıldız kümesi yakınında bulunur ve bu küme teleskopla gözlemlenebilir",
                        "Mars gezegenine benzer kırmızı rengi nedeniyle antik çağlardan beri dikkat çekmiştir",
                        "Samanyolu'nun merkez bölgesine yakın konumdadır"
                    },
                    IsConstellation = false,
                    ImageUrl = "antares.png",
                    AnimatedImageUrl = "antares_animated.gif",
                    Magnitude = 1.0,
                    DistanceFromEarth = 550,
                    Constellation = "Scorpius",
                    BestViewingSeason = "Haziran-Ağustos arası"
                }
            };
        }
    }
} 