# WeatherService
.Net Core Web API for weather service and Console app with SignalR integration.

=== Turkish Release  ===
PROJE AÇIKLAMASI : Locationiq API (latitude longitude bilgisi) ve darksky API (lokasyon bazlı sıcaklık verisi) kullanılarak; şehir bazlı meteorolojik veri elde eden servis ve monitörleme ekranı. WebAPI a gelen sorgulamalar şehir bazlı cache üzerinde tutulmuştur. Ayrıca; WebAPI ye gelen her bir yeni sorgu Monitör ekranlarına push edilmektedir (SignalR ile).

DEPLOYMENT Notları :

Deployment icin oncelikle DB ve tablo create edilir : "Docs" altinda "Script.sql" scripti çalıştırılarak tablo yaratılır. Veritabanı olarak "MS SQL Server 2016" kullanıldı ve SSMS uzerinde testler yapıldı.
Proje VS2019 ile derlendi: "WeatherAPI\WebApplication1\BigDataTeknolojiWeather.sln" ile proje açılabilir. F5 ile kod çalıştırılırsa öncelikle API ardından Monitoring uygulaması run olur.
Monitorlerme uygulaması için aşağıdaki exe ayrıca kullanılabilir (multiple monitoring yapmak için) "BigDataTeknoloji.UI\bin\Debug\netcoreapp3.1\Weather.UI.exe"
DB Notları :

DAL katmanı icin EF kullanildi.
appsettings.json icinde "BigDataTeknolojiTESTDatabase" connection stringi ilgili DB erişimini gösterir. BigDataTeknolojiTEST yerine gerekirse farkli bir DB uzerinde de script calistirilabilir.
Web API - servis sorgulama notları :

Aşağıdaki URL deki gibi şehir bazlı API bilgileri alınabilir, İlk etapta DB boş iken kayıtlar cache lenmeden direk API ler üzerinden gelir; Aynı şehre ait daha sonra gelen sorgular ise Cache den okunur (süresi expire olana kadar)

http://localhost:50217/api/WeatherApi/GetWeatherData/Ordu http://localhost:50217/api/WeatherApi/GetWeatherData/Antalya

Bilinen bug lar (Known Issues) :

locationiq API sorgularında bir adet sorun dikkatimi çekti: Örneğin Hakkari, Şırnak gibi bazı şehirlerde "locationiq" API içinde Dakika/Saat bazlı sıcaklık verilerinde hata olduğu görüldü. Bu nednele JSON objesi içinde bu alan çıkartıldı. Diğer tüm JSON objeleri modellendi.
Sade bir tasarım olması açısından; Veritabanı tasarımında; "weatherRecords" tablosunda sadece günlük/haftalık sıcaklık verileri tutuldu. Basit bir kullanım planlandıği için tüm JSON verileri tabloya yansıtılmadı.
