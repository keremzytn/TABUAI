using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TabuAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedDataWithNewCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Words",
                columns: new[] { "Id", "Category", "CreatedAt", "Difficulty", "IsActive", "TabuWords", "TargetWord", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("c1000001-0001-0001-0001-000000000001"), "Yemek", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"et\",\"\\u015Fi\\u015F\",\"\\u0131zgara\",\"Adana\",\"mangal\"]", "Kebap", null },
                    { new Guid("c1000001-0001-0001-0001-000000000002"), "Yemek", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"Japon\",\"pirin\\u00E7\",\"bal\\u0131k\",\"soya sosu\",\"\\u00E7i\\u011F\"]", "Sushi", null },
                    { new Guid("c1000001-0001-0001-0001-000000000003"), "Yemek", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"so\\u011Fuk\",\"tatl\\u0131\",\"k\\u00FClah\",\"s\\u00FCt\",\"vanilya\"]", "Dondurma", null },
                    { new Guid("c1000001-0001-0001-0001-000000000004"), "Yemek", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"demlik\",\"bardak\",\"s\\u0131cak\",\"\\u015Feker\",\"\\u00E7aydanl\\u0131k\"]", "Çay", null },
                    { new Guid("c1000001-0001-0001-0001-000000000005"), "Yemek", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"fincan\",\"kafein\",\"T\\u00FCrk\",\"espresso\",\"\\u00E7ekirdek\"]", "Kahve", null },
                    { new Guid("c1000001-0001-0001-0001-000000000006"), "Yemek", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"ince\",\"hamur\",\"k\\u0131yma\",\"limon\",\"rulo\"]", "Lahmacun", null },
                    { new Guid("c1000001-0001-0001-0001-000000000007"), "Yemek", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"hamur\",\"k\\u0131yma\",\"yo\\u011Furt\",\"Kayseri\",\"k\\u00FC\\u00E7\\u00FCk\"]", "Mantı", null },
                    { new Guid("c2000001-0001-0001-0001-000000000001"), "Teknoloji", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"makine\",\"otomat\",\"metal\",\"programlama\",\"android\"]", "Robot", null },
                    { new Guid("c2000001-0001-0001-0001-000000000002"), "Teknoloji", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"u\\u00E7mak\",\"kumanda\",\"kamera\",\"pervane\",\"insans\\u0131z\"]", "Drone", null },
                    { new Guid("c2000001-0001-0001-0001-000000000003"), "Teknoloji", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"kablosuz\",\"ba\\u011Flant\\u0131\",\"kulakl\\u0131k\",\"e\\u015Fle\\u015Ftirme\",\"sinyal\"]", "Bluetooth", null },
                    { new Guid("c2000001-0001-0001-0001-000000000004"), "Teknoloji", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"g\\u00FCvenlik\",\"giri\\u015F\",\"parola\",\"karakter\",\"gizli\"]", "Şifre", null },
                    { new Guid("c2000001-0001-0001-0001-000000000005"), "Teknoloji", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"payla\\u015F\\u0131m\",\"be\\u011Feni\",\"takip\",\"Instagram\",\"g\\u00F6nderi\"]", "Sosyal Medya", null },
                    { new Guid("c3000001-0001-0001-0001-000000000001"), "Spor", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"pota\",\"top\",\"sma\\u00E7\",\"NBA\",\"say\\u0131\"]", "Basketbol", null },
                    { new Guid("c3000001-0001-0001-0001-000000000002"), "Spor", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"raket\",\"top\",\"kort\",\"servis\",\"set\"]", "Tenis", null },
                    { new Guid("c3000001-0001-0001-0001-000000000003"), "Spor", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"havuz\",\"su\",\"kula\\u00E7\",\"mayo\",\"kulvar\"]", "Yüzme", null },
                    { new Guid("c3000001-0001-0001-0001-000000000004"), "Spor", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"ko\\u015Fu\",\"42 km\",\"dayan\\u0131kl\\u0131l\\u0131k\",\"yar\\u0131\\u015F\",\"parkur\"]", "Maraton", null },
                    { new Guid("c3000001-0001-0001-0001-000000000005"), "Spor", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"file\",\"sma\\u00E7\",\"pas\",\"top\",\"set\"]", "Voleybol", null },
                    { new Guid("c4000001-0001-0001-0001-000000000001"), "Bilim", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"\\u00E7ekirdek\",\"elektron\",\"proton\",\"k\\u00FC\\u00E7\\u00FCk\",\"element\"]", "Atom", null },
                    { new Guid("c4000001-0001-0001-0001-000000000002"), "Bilim", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"g\\u00F6zlem\",\"uzay\",\"mercek\",\"y\\u0131ld\\u0131z\",\"b\\u00FCy\\u00FCtmek\"]", "Teleskop", null },
                    { new Guid("c4000001-0001-0001-0001-000000000003"), "Bilim", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"fay\",\"sars\\u0131nt\\u0131\",\"b\\u00FCy\\u00FCkl\\u00FCk\",\"Richter\",\"y\\u0131k\\u0131m\"]", "Deprem", null },
                    { new Guid("c4000001-0001-0001-0001-000000000004"), "Bilim", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, "[\"uzay\",\"\\u00E7ekim\",\"\\u0131\\u015F\\u0131k\",\"y\\u0131ld\\u0131z\",\"galaksi\"]", "Karadelik", null },
                    { new Guid("c4000001-0001-0001-0001-000000000005"), "Bilim", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, "[\"Darwin\",\"do\\u011Fal se\\u00E7ilim\",\"t\\u00FCr\",\"adaptasyon\",\"mutasyon\"]", "Evrim", null },
                    { new Guid("c5000001-0001-0001-0001-000000000001"), "Doğa", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"renk\",\"ya\\u011Fmur\",\"g\\u00FCne\\u015F\",\"yedi\",\"g\\u00F6ky\\u00FCz\\u00FC\"]", "Gökkuşağı", null },
                    { new Guid("c5000001-0001-0001-0001-000000000002"), "Doğa", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"deniz\",\"su\",\"derin\",\"dalga\",\"Pasifik\"]", "Okyanus", null },
                    { new Guid("c5000001-0001-0001-0001-000000000003"), "Doğa", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"a\\u011Fa\\u00E7\",\"ye\\u015Fil\",\"do\\u011Fa\",\"hayvan\",\"yaprak\"]", "Orman", null },
                    { new Guid("c5000001-0001-0001-0001-000000000004"), "Doğa", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"su\",\"d\\u00FC\\u015Fmek\",\"kayal\\u0131k\",\"do\\u011Fa\",\"akmak\"]", "Şelale", null },
                    { new Guid("c5000001-0001-0001-0001-000000000005"), "Doğa", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"buz\",\"so\\u011Fuk\",\"penguen\",\"kuzey\",\"g\\u00FCney\"]", "Kutup", null },
                    { new Guid("c6000001-0001-0001-0001-000000000001"), "Tarih", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"padi\\u015Fah\",\"imparatorluk\",\"\\u0130stanbul\",\"fetih\",\"sultan\"]", "Osmanlı", null },
                    { new Guid("c6000001-0001-0001-0001-000000000002"), "Tarih", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"Cumhuriyet\",\"kurucu\",\"lider\",\"Ankara\",\"devrim\"]", "Atatürk", null },
                    { new Guid("c6000001-0001-0001-0001-000000000003"), "Tarih", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"nesli t\\u00FCkenmi\\u015F\",\"dev\",\"s\\u00FCr\\u00FCngen\",\"fosil\",\"Jura\"]", "Dinozor", null },
                    { new Guid("c6000001-0001-0001-0001-000000000004"), "Tarih", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"Roma\",\"arena\",\"d\\u00F6v\\u00FC\\u015F\",\"k\\u0131l\\u0131\\u00E7\",\"k\\u00F6le\"]", "Gladyatör", null },
                    { new Guid("c6000001-0001-0001-0001-000000000005"), "Tarih", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, "[\"at\",\"sava\\u015F\",\"Yunan\",\"hile\",\"antik\"]", "Truva", null },
                    { new Guid("c7000001-0001-0001-0001-000000000001"), "Sanat", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"mermer\",\"yontmak\",\"sanat\\u00E7\\u0131\",\"m\\u00FCze\",\"\\u00FC\\u00E7 boyutlu\"]", "Heykel", null },
                    { new Guid("c7000001-0001-0001-0001-000000000002"), "Sanat", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"dans\",\"parmak ucu\",\"sahne\",\"ku\\u011Fu\",\"tutu\"]", "Bale", null },
                    { new Guid("c7000001-0001-0001-0001-000000000003"), "Sanat", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, "[\"su\",\"boya\",\"desen\",\"geleneksel\",\"ka\\u011F\\u0131t\"]", "Ebru", null },
                    { new Guid("c8000001-0001-0001-0001-000000000001"), "Müzik", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"tu\\u015F\",\"siyah beyaz\",\"\\u00E7almak\",\"kuyruklu\",\"nota\"]", "Piyano", null },
                    { new Guid("c8000001-0001-0001-0001-000000000002"), "Müzik", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"vurmak\",\"ritim\",\"baget\",\"ses\",\"tempo\"]", "Davul", null },
                    { new Guid("c8000001-0001-0001-0001-000000000003"), "Müzik", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"s\\u00F6z\",\"ritim\",\"hip-hop\",\"beat\",\"kafiye\"]", "Rap", null },
                    { new Guid("c9000001-0001-0001-0001-000000000001"), "Ulaşım", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"yeralt\\u0131\",\"istasyon\",\"ray\",\"\\u015Fehir\",\"ula\\u015F\\u0131m\"]", "Metro", null },
                    { new Guid("c9000001-0001-0001-0001-000000000002"), "Ulaşım", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"deniz\",\"liman\",\"kaptan\",\"yolcu\",\"g\\u00FCverte\"]", "Gemi", null },
                    { new Guid("c9000001-0001-0001-0001-000000000003"), "Ulaşım", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"uzay\",\"f\\u0131rlatmak\",\"NASA\",\"yak\\u0131t\",\"h\\u0131z\"]", "Roket", null },
                    { new Guid("d1000001-0001-0001-0001-000000000001"), "Coğrafya", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"bo\\u011Faz\",\"k\\u00F6pr\\u00FC\",\"iki k\\u0131ta\",\"cami\",\"b\\u00FCy\\u00FCk\\u015Fehir\"]", "İstanbul", null },
                    { new Guid("d1000001-0001-0001-0001-000000000002"), "Coğrafya", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"peri bacas\\u0131\",\"balon\",\"Nev\\u015Fehir\",\"yeralt\\u0131\",\"kayal\\u0131k\"]", "Kapadokya", null },
                    { new Guid("d1000001-0001-0001-0001-000000000003"), "Coğrafya", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"buz\",\"kutup\",\"so\\u011Fuk\",\"penguen\",\"k\\u0131ta\"]", "Antarktika", null },
                    { new Guid("d1000001-0001-0001-0001-000000000004"), "Coğrafya", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"travertenler\",\"beyaz\",\"termal\",\"Denizli\",\"UNESCO\"]", "Pamukkale", null },
                    { new Guid("d2000001-0001-0001-0001-000000000001"), "Meslekler", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"hastane\",\"tedavi\",\"ila\\u00E7\",\"muayene\",\"stetoskop\"]", "Doktor", null },
                    { new Guid("d2000001-0001-0001-0001-000000000002"), "Meslekler", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"uzay\",\"roket\",\"NASA\",\"a\\u011F\\u0131rl\\u0131ks\\u0131z\",\"kask\"]", "Astronot", null },
                    { new Guid("d2000001-0001-0001-0001-000000000003"), "Meslekler", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"bina\",\"\\u00E7izim\",\"proje\",\"tasar\\u0131m\",\"yap\\u0131\"]", "Mimar", null },
                    { new Guid("d2000001-0001-0001-0001-000000000004"), "Meslekler", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"mutfak\",\"yemek\",\"pi\\u015Firmek\",\"restoran\",\"tarif\"]", "Aşçı", null },
                    { new Guid("d2000001-0001-0001-0001-000000000005"), "Meslekler", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"ipucu\",\"soru\\u015Fturma\",\"cinayet\",\"b\\u00FCy\\u00FCte\\u00E7\",\"su\\u00E7\"]", "Dedektif", null },
                    { new Guid("d3000001-0001-0001-0001-000000000001"), "Hayvanlar", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"deniz\",\"zeki\",\"atlama\",\"memeli\",\"gri\"]", "Yunus", null },
                    { new Guid("d3000001-0001-0001-0001-000000000002"), "Hayvanlar", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"ku\\u015F\",\"y\\u0131rt\\u0131c\\u0131\",\"kanat\",\"g\\u00F6ky\\u00FCz\\u00FC\",\"pen\\u00E7e\"]", "Kartal", null },
                    { new Guid("d3000001-0001-0001-0001-000000000003"), "Hayvanlar", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"renk de\\u011Fi\\u015Ftirmek\",\"s\\u00FCr\\u00FCngen\",\"dil\",\"kamufle\",\"g\\u00F6z\"]", "Bukalemun", null },
                    { new Guid("d3000001-0001-0001-0001-000000000004"), "Hayvanlar", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"hortum\",\"b\\u00FCy\\u00FCk\",\"fildi\\u015Fi\",\"Afrika\",\"haf\\u0131za\"]", "Fil", null },
                    { new Guid("d3000001-0001-0001-0001-000000000005"), "Hayvanlar", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"sekiz\",\"kol\",\"deniz\",\"m\\u00FCrekkep\",\"y\\u00FCzmek\"]", "Ahtapot", null },
                    { new Guid("d4000001-0001-0001-0001-000000000001"), "Eğlence", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"palya\\u00E7o\",\"akrobat\",\"g\\u00F6steri\",\"\\u00E7ad\\u0131r\",\"hayvan\"]", "Sirk", null },
                    { new Guid("d4000001-0001-0001-0001-000000000002"), "Eğlence", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"\\u015Fark\\u0131\",\"mikrofon\",\"ekran\",\"s\\u00F6z\",\"e\\u011Flence\"]", "Karaoke", null },
                    { new Guid("d4000001-0001-0001-0001-000000000003"), "Eğlence", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"d\\u00F6nme dolap\",\"h\\u0131z treni\",\"e\\u011Flence\",\"bilet\",\"atl\\u0131kar\\u0131nca\"]", "Lunapark", null },
                    { new Guid("d5000001-0001-0001-0001-000000000001"), "Kavramlar", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, "[\"ge\\u00E7mi\\u015F\",\"\\u00F6zlem\",\"an\\u0131\",\"eski\",\"hat\\u0131ra\"]", "Nostalji", null },
                    { new Guid("d5000001-0001-0001-0001-000000000002"), "Kavramlar", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"oy\",\"halk\",\"se\\u00E7im\",\"\\u00F6zg\\u00FCrl\\u00FCk\",\"y\\u00F6netim\"]", "Demokrasi", null },
                    { new Guid("d5000001-0001-0001-0001-000000000003"), "Kavramlar", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, "[\"anlama\",\"duygu\",\"kar\\u015F\\u0131 taraf\",\"hissetmek\",\"ba\\u015Fkas\\u0131\"]", "Empati", null },
                    { new Guid("d5000001-0001-0001-0001-000000000004"), "Kavramlar", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, true, "[\"tersine\",\"alay\",\"anlam\",\"s\\u00F6z\",\"beklenti\"]", "Ironi", null },
                    { new Guid("d6000001-0001-0001-0001-000000000001"), "Sinema", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"\\u00F6d\\u00FCl\",\"Hollywood\",\"film\",\"heykelcik\",\"t\\u00F6ren\"]", "Oscar", null },
                    { new Guid("d6000001-0001-0001-0001-000000000002"), "Sinema", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"b\\u00F6l\\u00FCm\",\"sezon\",\"televizyon\",\"izlemek\",\"oyuncu\"]", "Dizi", null },
                    { new Guid("d6000001-0001-0001-0001-000000000003"), "Sinema", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"\\u00E7izgi film\",\"karakter\",\"Pixar\",\"canland\\u0131rma\",\"Disney\"]", "Animasyon", null },
                    { new Guid("d7000001-0001-0001-0001-000000000001"), "Edebiyat", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"kitap\",\"yazar\",\"sayfa\",\"okumak\",\"hikaye\"]", "Roman", null },
                    { new Guid("d7000001-0001-0001-0001-000000000002"), "Edebiyat", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"m\\u0131sra\",\"kafiye\",\"dize\",\"\\u015Fair\",\"\\u00F6l\\u00E7\\u00FC\"]", "Şiir", null },
                    { new Guid("d7000001-0001-0001-0001-000000000003"), "Edebiyat", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"\\u00E7ocuk\",\"prenses\",\"bir varm\\u0131\\u015F\",\"hayal\",\"ejderha\"]", "Masal", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c1000001-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c1000001-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c1000001-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c1000001-0001-0001-0001-000000000004"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c1000001-0001-0001-0001-000000000005"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c1000001-0001-0001-0001-000000000006"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c1000001-0001-0001-0001-000000000007"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c2000001-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c2000001-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c2000001-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c2000001-0001-0001-0001-000000000004"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c2000001-0001-0001-0001-000000000005"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c3000001-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c3000001-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c3000001-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c3000001-0001-0001-0001-000000000004"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c3000001-0001-0001-0001-000000000005"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c4000001-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c4000001-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c4000001-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c4000001-0001-0001-0001-000000000004"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c4000001-0001-0001-0001-000000000005"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c5000001-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c5000001-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c5000001-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c5000001-0001-0001-0001-000000000004"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c5000001-0001-0001-0001-000000000005"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c6000001-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c6000001-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c6000001-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c6000001-0001-0001-0001-000000000004"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c6000001-0001-0001-0001-000000000005"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c7000001-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c7000001-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c7000001-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c8000001-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c8000001-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c8000001-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c9000001-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c9000001-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c9000001-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d1000001-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d1000001-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d1000001-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d1000001-0001-0001-0001-000000000004"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d2000001-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d2000001-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d2000001-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d2000001-0001-0001-0001-000000000004"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d2000001-0001-0001-0001-000000000005"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d3000001-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d3000001-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d3000001-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d3000001-0001-0001-0001-000000000004"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d3000001-0001-0001-0001-000000000005"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d4000001-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d4000001-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d4000001-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d5000001-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d5000001-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d5000001-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d5000001-0001-0001-0001-000000000004"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d6000001-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d6000001-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d6000001-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d7000001-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d7000001-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d7000001-0001-0001-0001-000000000003"));
        }
    }
}
