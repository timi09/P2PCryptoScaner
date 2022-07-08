## Краткое описание поекта
Данный проект представляет из себя агрегатор предложений по покупке/продаже криптовалюты на популярных p2p(person to person) криптобиржах: Binance, Huobi, Okx.

Программа сканирует предложения по криптовалютам Bitcoin(BTC), Ethereum(ETH), Tether(USDT), и по платежным методам Сбербанк, Тинькофф, Альфа банк, Qiwi.

Пользователь может зарегестрировать новый аккаунт на сайте и приобрести подписку для доступа к функционалу. От уровня подписки будет зависеть доступный функционал.

Так же на сайте имеется панель администратора. Администратор может выдавать роли пользователям, создавать/удалять/редактировать уровни подписки, или деактивировать их.

Используемые технологии: ASP.NET Core MVC, ASP.NET Core Identity, Entity Framework Core.

## Немного визуализации
Пока у меня нет возможности захостить данный проект для примера, поэтому решил показать основной функционал здесь
![image](https://github.com/timi09/gifs/blob/main/P2PCryptoScaner/register.gif)
<p align="center">Регистрация и вход</p>

<br/>
<br/>

![image](https://github.com/timi09/gifs/blob/main/P2PCryptoScaner/levelsilver.gif)
<p align="center">Получение уровня доступа</p>

<br/>
<br/>

![image](https://github.com/timi09/gifs/blob/main/P2PCryptoScaner/levelgold.gif)
<p align="center">Влияние уровня доступа на функционал</p>

<br/>
<br/>

![image](https://github.com/timi09/gifs/blob/main/P2PCryptoScaner/admin.gif)
<p align="center">Панель админа</p>

## Загрузка и использование

### Способ 1

Загрузите репозиторий с github
Загрузите его через свою Visual Studio с помощью файла решения P2PCryptoScaner.sln

> **Примечание:** Если вы будете запускать приложение не через docker compose. То измените строку подключения к базе данных в файле appsettings.json. Найдите строку "DefaultConnection": "Server=db;Initial Catalog=MssqlServerContainer;User Id=sa;Password=SwN12345678;" замените ее на строку подключения к вашей локальной базе данных, например "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=your-local-db;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"

### Способ 2
Понадобится git и Docker Desktop (docker и docker-compose для linux)

Клонируйте репозиторий: 
```
git clone https://github.com/timi09/P2PCryptoScaner.git
```
Перейдите в каталог репозитория /P2PCryptoScaner где хранится файл docker-compose.yml

Запустите PowerShell нажав Shift+Правой кнопкой мыши по пустому месту в папке. В выпавшем контекстном меню нажмите "открыть окно PowerShell здесь".

Выполните в PowerShell:
```
docker compose up
```
Подождите пока установятся необходимые пакеты.

Когда увидите строку типа 
```
Content root path: /app/ 
```
Значит все успешно запустилось

Откройте браузер и введите в строке запроса
```
https://localhost:44378/
```
