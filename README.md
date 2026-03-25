# Trade Monitor (WPF + Java Backend)

A desktop trade monitoring application built to simulate an enterprise trading system.  
The project demonstrates a full-stack architecture using **C# WPF (MVVM)** for the frontend and a **Java Spring Boot API** for backend trade analysis.

---

## 🚀 Features

### 📊 Trade Management
- Generate large datasets (1,000+ trades)
- Real-time filtering and sorting using LINQ
- Summary dashboard (total trades, notional, approvals, etc.)
- Detailed trade inspection panel

### ⚙️ UI Architecture
- MVVM pattern
- Data binding with `INotifyPropertyChanged`
- Commands (`RelayCommand`, `AsyncRelayCommand`)
- Reusable **UserControls**
- Custom **StatusBadge control**
- Centralised **styles and themes**

### ⚡ Performance & Responsiveness
- Async data loading with `Task.Run`
- Background processing for trade analysis
- UI thread-safe updates
- DataGrid virtualization
- Optimised large dataset handling (bulk collection replacement)

### 💾 Persistence
- SQLite database for trade storage
- Repository pattern for data access
- Async save/load operations

### 📄 Configuration
- XML-based config storage
- Saves user preferences:
  - search text
  - filters
  - sorting options

### 🔌 Backend Integration
- Java Spring Boot REST API
- DTO-based request/response model
- HTTP communication from WPF → Java backend
- Trade risk analysis handled server-side

---

## 🏗️ Architecture Overview

```
WPF (MVVM)
│
├── View (XAML, UserControls, Styles)
├── ViewModel (Commands, State, LINQ logic)
├── Services
│   ├── SQLite Repository
│   ├── XML Config Service
│   └── Java API Client
│
├── Core Models / DTOs
│
└── Java Spring Boot Backend
    └── Trade Analysis API
```

---

## 🔄 Data Flow Example

1. User selects a trade in the UI
2. ViewModel maps it → `TradeAnalysisRequestDto`
3. HTTP POST → Java backend (`/api/trade-analysis`)
4. Backend calculates risk score
5. Response returned → `TradeAnalysisResponseDto`
6. UI displays result

---

## 🧰 Tech Stack

### Frontend
- C# (.NET, WPF)
- MVVM architecture
- XAML (UserControls, Custom Controls, Styles)

### Backend
- Java
- Spring Boot
- REST API

### Data & Config
- SQLite
- XML Serialization

### Concepts Demonstrated
- Async / multithreading
- LINQ querying
- DTO mapping
- Separation of concerns
- UI virtualization

---

## ▶️ How to Run

### 1. Start Java Backend

In IntelliJ:

- Run `TradeMonitorBackendApplication`
- API will start at:

```
http://localhost:8080
```

---

### 2. Run WPF App

In Visual Studio:

- Set `TradeMonitor.App` as startup project
- Run the application

---

### 3. Use the App

- Click **Load Trades**
- Select a trade
- Click **Analyse Selected**

👉 This will call the Java backend and return a risk score

---

## 📁 Project Structure

```
TradeMonitor
│
├── TradeMonitor.App        (WPF UI)
├── TradeMonitor.Core       (Models, DTOs)
├── TradeMonitor.Data       (SQLite repository)
├── TradeMonitor.Services   (API + XML services)
│
└── TradeMonitor.JavaBackend (Spring Boot API)
```

---

## 🧠 Key Learnings

- Managing large datasets efficiently in WPF
- Avoiding UI freezes when updating ObservableCollections
- Structuring MVVM applications for scalability
- Integrating C# frontend with Java backend services
- Separating configuration, data, and UI state

---

## 📌 Notes

- The backend must be running for trade analysis to work
- SQLite database (`trades.db`) is created at runtime
- XML config (`TradeMonitorConfig.xml`) stores UI preferences

---

## 💬 Summary

This project was built to replicate a simplified enterprise trading system, demonstrating how desktop applications can integrate with backend services while maintaining clean architecture, performance, and scalability.
