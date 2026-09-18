# 📱 CampusPulse  
A cross‑platform .NET MAUI application for managing campus activities, announcements, and student engagement — powered by an ASP.NET Core backend API.

![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)
![MAUI](https://img.shields.io/badge/MAUI-Cross--Platform-blue.svg)
![API](https://img.shields.io/badge/Backend-ASP.NET_Core-green.svg)
![License](https://img.shields.io/badge/License-MIT-orange.svg)

CampusPulse is designed to run seamlessly across Android, Windows, macOS, and iOS.  
It includes a robust API client, clean architecture, and platform‑aware networking logic for real‑device testing.

---

## ✨ Features

### 📡 API Client (MAUI)
- Automatic platform‑aware base URL selection  
- Android emulator support via `10.0.2.2`  
- LAN IP override for real device testing  
- Centralised error handling (`LastError`)  
- JSON‑based GET, POST, PUT, DELETE helpers  
- Timeout and network failure detection  

### 🖥️ Backend API (ASP.NET Core) https://github.com/Kyran4/CampusPulse.API
- REST endpoints for CampusPulse features  
- JSON serialization  
- CORS support for mobile apps  
- Local development via `http://localhost:5162`

### 📱 Mobile App (MAUI)
- Cross‑platform UI  
- MVVM architecture  
- Async data loading  
- Error messaging based on API client state  

---

## 🧱 Project Structure

```
CampusPulse/
│
├── CampusPulse/                # MAUI client app
│   ├── Services/
│   │   └── ApiClient.cs        # HTTP client wrapper
│   ├── ViewModels/             # MVVM logic
│   ├── Views/                  # XAML pages
│   └── App.xaml / App.xaml.cs
│
├── CampusPulse.Api/            # ASP.NET Core backend
│   ├── Controllers/
│   ├── Models/
│   ├── Program.cs
│   └── appsettings.json
│
└── README.md
```

---

## ⚙️ Configuration

### 🔌 API Base URL Logic

`ApiClient.cs` automatically chooses the correct base URL:

1. **If `ServerIp` is set**  
   ```
   http://<ServerIp>:5162
   ```
   Ideal for real devices on WiFi.

2. **Android Emulator**  
   ```
   [http://10.0.2.2:5162](http://10.0.2.2:5162)
   ```

3. **Windows / macOS / iOS Simulator**  
   ```
   http://localhost:5162
   ```

### 🛜 Finding Your LAN IP  
On the machine running the API:

- **Windows:** `ipconfig` → look for *IPv4 Address*  
- **Mac:** System Settings → Wi‑Fi → Details  
- **Terminal:** `ipconfig getifaddr en0`

Ensure all devices are on the same WiFi network and Windows Firewall allows inbound traffic on port **5162**.

---

## 🚀 Getting Started

### 1. Clone the repository
```bash
git clone https://github.com/<your-username>/<your-repo>.git
cd <your-repo>
```

### 2. Run the API
```bash
cd CampusPulse.Api
dotnet run
```

API will start at:
```
http://localhost:5162
```

### 3. Run the MAUI App
```bash
cd CampusPulse
dotnet build
dotnet maui run
```

Choose your target:
- `maui-android`
- `maui-windows`
- `maui-ios`
- `maui-maccatalyst`

---

## 🧪 Testing on Real Devices

1. Set `ServerIp` in `ApiClient.cs`:
   ```csharp
   private const string ServerIp = "192.168.x.x";
   ```
2. Ensure API machine and device are on the same WiFi  
3. Ensure port **5162** is open  
4. Run the API  
5. Launch the MAUI app on the device  

---

## 🐛 Troubleshooting

### ❌ Timeout / No Response  
Likely causes:
- Wrong `ServerIp`
- Device on different WiFi
- Firewall blocking port 5162

The app will show a friendly error via `LastError`.

### ❌ Android Emulator Cannot Reach API  
Use:
```
http://10.0.2.2:5162
```

### ❌ Build Errors in ApiClient.cs  
Delete the file → recreate → paste clean version.  
(Visual Studio sometimes inserts hidden characters.)

---

## 📜 License  
This project is licensed under the **MIT License**.

---
