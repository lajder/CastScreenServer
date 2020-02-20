# CastScreenServer
Aplikacja do przechwytywania zawartości ekranu i udostępnia jej przez utworzony przez siebie serwer HTTP do wielu klientów znajdujących się w tej samem sieci. Możliwość konfiguracji aplikacji do automatycznego startu z wykorzystaniem parametrów konsolowych (np. w przypadku jednostki bez monitora).

# Wymagania
* Windows 10
* przeglądarka HTTP z obsługą HTML5 i JavaScript (rekomendowana: FireFox 73.0.1 lub Microsoft Edge 44.18362.449.0)
* .NET Core 3.1
* zasada firewall pozwalająca na komunikację na odpowiednim porcie
* dodany wpis do znanych adresów HTTP dla danego użytkownika lub uprawnienia administratora do uruchomienia aplikacji (ograniczenia Windows dla HTTPListener)
