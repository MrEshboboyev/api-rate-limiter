# 🚀 API Rate Limiter - .NET 9

![.NET 9](https://img.shields.io/badge/.NET%209-%F0%9F%92%AA-blue)
![Redis](https://img.shields.io/badge/Redis-%F0%9F%93%9A-red)
![Clean Architecture](https://img.shields.io/badge/Clean%20Architecture-%F0%9F%9A%80-green)
![Rate Limiting](https://img.shields.io/badge/Rate%20Limiting-%F0%9F%94%92-yellow)

A powerful and scalable **API Rate Limiter** built with **.NET 9**, supporting multiple rate-limiting algorithms (**Fixed Window, Sliding Window, Token Bucket, Concurrency**) with **IP-based rate limiting**. Designed using **Clean Architecture** and integrated with **Redis** for distributed environments.

---

## 🌟 Features

✅ **Multiple Rate-Limiting Algorithms**:
   - **Fixed Window** 🕒 – Limits requests per fixed time window.
   - **Sliding Window** 🔄 – Smooths request spikes with a rolling window.
   - **Token Bucket** 🪙 – Allows burstable traffic via token-based management.
   - **Concurrency** 🔗 – Restricts simultaneous client connections.

✅ **IP-Based Rate Limiting** 📡 – Ensures fair request distribution across users.
✅ **Redis Integration** 🛢️ – Enables scalable, distributed rate limiting.
✅ **Clean Architecture** 🏗️ – Layered design following **best practices**.
✅ **Fully Configurable** 🔧 – Manage rate limits in `appsettings.json`.

---

## 🛠️ Technologies Used

| Technology  | Purpose  |
|-------------|----------|
| **.NET 9** | Core framework |
| **Redis** | Distributed caching & rate limiting |
| **StackExchange.Redis** | Redis client for .NET |
| **MediatR** | CQRS pattern implementation |
| **Serilog** | Logging |
| **xUnit** | Unit testing framework |
| **Moq** | Mocking for unit tests |

---

## 🏗️ Clean Architecture Overview

📌 **This project follows Clean Architecture principles:**

```
📂 src/
 ├── 📁 App             # Runnable and installers located web api
 ├── 📁 Domain          # Business logic & entities
 ├── 📁 Application     # Use cases, handlers, services
 ├── 📁 Infrastructure  # Redis integration & rate-limiting logic
 ├── 📁 Persistence     # EF Core integration & implementations
 ├── 📁 Presentation    # API controllers & contracts
```

### ✨ Benefits of Clean Architecture
- **Separation of Concerns** – Organized and maintainable code.
- **Scalability** – Easily extendable for new features.
- **Testability** – Isolated business logic for robust unit tests.

---

## ⚡ How It Works

### **1️⃣ Fixed Window Algorithm**
- Restricts requests within a fixed time period.
- Example: `100 requests per minute`.

### **2️⃣ Sliding Window Algorithm**
- Maintains a rolling window to smooth traffic bursts.
- Example: **Consider last `60 seconds` instead of resetting counters**.

### **3️⃣ Token Bucket Algorithm**
- Allows controlled bursts of traffic.
- Example: **Users get `100 tokens`, refilling at `10/sec`**.

### **4️⃣ Concurrency Algorithm**
- Limits the number of simultaneous API requests.
- Example: `Max 10 concurrent users`.

### **5️⃣ IP-Based Rate Limiting**
- Identifies and restricts request flow per IP address.
- Ensures fair resource allocation across users.

---

## 🚀 Getting Started

### 📝 Prerequisites

✅ [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)  
✅ [Redis](https://redis.io/download)  
✅ [Visual Studio Code](https://code.visualstudio.com/)  
✅ [Postman](https://www.postman.com/) (for API testing)

### 🔧 Installation & Setup

#### 1️⃣ Clone the Repository
```sh
 git clone https://github.com/MrEshboboyev/api-rate-limiter.git
 cd src
```

#### 2️⃣ Install Dependencies
```sh
 dotnet restore
```

#### 3️⃣ Run Redis in Docker
```sh
 docker run -d -p 6379:6379 redis
```

#### 4️⃣ Run the Application
```sh
 dotnet run --project src/App
```

---

## 🔧 Configuration

Modify `appsettings.json` to configure rate limits:

```json
{
  "RateLimitSettings": {
    "FixedWindow": {
      "PermitLimit": 100,
      "WindowInSeconds": 60
    },
    "SlidingWindow": {
      "PermitLimit": 100,
      "WindowInSeconds": 60
    },
    "TokenBucket": {
      "BucketSize": 100,
      "RefillRate": 10
    },
    "Concurrency": {
      "MaxConcurrentRequests": 10
    }
  }
}
```

---

## 📡 API Endpoints

| Method | Endpoint | Description |
|--------|---------|-------------|
| **GET** | `/api/rate-limit/get-random` | Enabled endpoint for rate limiting |
| **POST** | `/api/rate-limit/get-random-two` | Disabled endpoint for rate limiting |

---

## 🧪 Testing

### 🔍 Unit Tests
Run tests to validate the functionality:
```sh
 dotnet test
```

### 🌐 API Testing (Postman/cURL)
Send multiple requests to trigger rate limiting:
```sh
 curl -X GET http://localhost:5000/api/rate-limit/test
```
✅ If rate limit is exceeded, you’ll get **429 Too Many Requests**.

---

## 🤝 Contributing

Contributions are **welcome!** 🎉 If you find any issues or have suggestions, feel free to **open an issue** or **submit a pull request**.

---

## 📜 License

This project is **MIT Licensed**. See the [LICENSE](LICENSE) file for details.

---

## 🎉 Acknowledgments

Special thanks to:
- The **.NET** and **Redis** communities for excellent tools.
- Clean Architecture & Domain-Driven Design inspirations.

---

## ✉️ Contact

📬 **GitHub**: [MrEshboboyev](https://github.com/MrEshboboyev)  
📬 **Email**: mreshboboyev@gmail.com

---

⭐ **Star this repository** if you found it helpful! 🚀
