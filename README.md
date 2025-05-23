# Cloudot CRM - Multi-Tenant Modular Monolith

> Modern .NET 9 ile geliştirilmiş enterprise seviye multi-tenant CRM sistemi

## 📋 İçindekiler

- [Proje Genel Bakış](#-proje-genel-bakış)
- [Mimari Yaklaşım](#-mimari-yaklaşım)
- [Teknoloji Stack](#-teknoloji-stack)
- [Modül Yapısı](#-modül-yapısı)
- [Multi-Tenant Mimarisi](#-multi-tenant-mimarisi)
- [Event-Driven Architecture](#-event-driven-architecture)
- [Güvenlik ve Yetkilendirme](#-güvenlik-ve-yetkilendirme)
- [Background Services](#-background-services)
- [Geliştirme Planı](#-geliştirme-planı)
- [Kurulum ve Çalıştırma](#-kurulum-ve-çalıştırma)

## 🎯 Proje Genel Bakış

Cloudot, farklı organizasyonların kendi izole ortamlarında çalışabileceği, modüler yapıda tasarlanmış bir CRM sistemidir. Her tenant kendi veritabanında tamamen izole şekilde çalışır ve sadece ihtiyaç duyduğu modüllere erişim sağlar.

### Temel Özellikler
- **Multi-Tenant Architecture**: Her müşteri kendi izole ortamı
- **Modular Monolith**: Gevşek bağlı, bağımsız modüller
- **Domain-Driven Design**: Temiz mimari yaklaşımı
- **Event-Driven Communication**: Modüller arası asenkron iletişim
- **Enterprise Security**: JWT + OTP, role-based authorization
- **Real-time Operations**: SignalR ile canlı updates

## 🏗️ Mimari Yaklaşım

### Layered Architecture
```
┌─ Presentation Layer ────────────────────────────┐
│  ├─ Admin API (Management)                      │
│  ├─ Tenant API (Business Modules)               │
│  └─ Shared Controllers                          │
├─ Application Layer ────────────────────────────┤
│  ├─ Services                                    │
│  ├─ DTOs                                        │
│  ├─ Commands/Queries                            │
│  └─ Event Handlers                              │
├─ Domain Layer ─────────────────────────────────┤
│  ├─ Entities                                    │
│  ├─ Value Objects                               │
│  ├─ Domain Events                               │
│  └─ Business Rules                              │
├─ Infrastructure Layer ─────────────────────────┤
│  ├─ Entity Framework DbContexts                 │
│  ├─ Repositories                                │
│  ├─ External Services                           │
│  └─ Message Bus                                 │
└─ Shared/Core Layer ────────────────────────────┘
   ├─ Common Interfaces
   ├─ Base Classes
   ├─ Utilities
   └─ Cross-Cutting Concerns
```

### Modular Monolith Yapısı
Her modül kendi domain'i, application logic'i ve infrastructure'ını içerir:
```
src/
├── Cloudot.Core/                    # Core utilities
├── Cloudot.Shared/                  # Shared components
├── Cloudot.WebAPI/                  # Main API project
├── Infrastructure/
│   ├── Cloudot.Infrastructure.Auth/
│   ├── Cloudot.Infrastructure.Redis/
│   ├── Cloudot.Infrastructure.RabbitMQ/
│   └── Cloudot.Infrastructure.Messaging/
└── Modules/
    ├── Management/                   # ✅ Tamamlandı
    │   ├── Domain/
    │   ├── Application/
    │   ├── Infrastructure/
    │   └── Shared/
    └── Procurement/                  # 🚧 Geliştirilecek
        ├── Domain/
        ├── Application/
        ├── Infrastructure/
        └── Shared/
```

## 🛠️ Teknoloji Stack

### Backend
- **.NET 9**: Ana framework
- **PostgreSQL**: Primary database
- **Entity Framework Core**: ORM
- **Redis**: Caching & session management
- **RabbitMQ**: Message queue (gelecek)
- **AutoMapper**: Object mapping
- **FluentValidation**: Model validation
- **JWT**: Authentication

### Infrastructure
- **Docker**: Containerization
- **SignalR**: Real-time communication (planlı)
- **MailKit**: Email services
- **Polly**: Retry policies (planlı)

## 📦 Modül Yapısı

### Tamamlanan Modüller

#### Management Modülü
**Amaç**: Sistem yönetimi ve tenant management
**Özellikler**:
- User management (OTP-based authentication)
- Tenant management
- Localization (database-driven)
- System administration

**Entities**:
- `User`: Kullanıcı yönetimi
- `Tenant`: Organizasyon yönetimi
- `LocalizationRecord`: Çoklu dil desteği

### Geliştirilecek Modüller

#### Procurement Modülü (Öncelikli)
**Amaç**: Satın alma süreç yönetimi
**Planlanan Özellikler**:
- Tedarikçi yönetimi
- Satın alma talepleri
- Onay süreçleri (workflow engine ile)
- Sipariş takibi
- Fatura eşleştirme

**Planlanan Entities**:
```csharp
public class Supplier : BaseAuditEntity
{
    public string Name { get; set; }
    public string TaxNumber { get; set; }
    public Guid? AccountId { get; set; } // Finance modülü ile entegrasyon
}

public class PurchaseRequest : BaseAuditEntity
{
    public string RequestNumber { get; set; }
    public Guid ProjectId { get; set; } // Proje bazlı çalışma
    public PurchaseRequestStatus Status { get; set; }
    public List<PurchaseRequestItem> Items { get; set; }
}
```

#### Finance Modülü (Gelecek)
**Amaç**: Mali işlemler ve cari hesap yönetimi
**Planlanan Özellikler**:
- Cari hesap yönetimi
- Fatura yönetimi
- Mali raporlama
- Procurement modülü ile entegrasyon

## 🏢 Multi-Tenant Mimarisi

### Tenant Isolation
Her tenant kendi PostgreSQL veritabanında çalışır:
```
Master DB (Management)
├── Tenant A → cloudot_tenant_companya_db
├── Tenant B → cloudot_tenant_companyb_db
└── Tenant C → cloudot_tenant_companyc_db
```

### Connection String Management
```csharp
public class TenantConnectionInfo
{
    public Guid Id { get; set; }
    public string DbName { get; set; }
}

// Runtime'da tenant context'e göre connection string resolve edilir
```

### Database Per Tenant Benefits
- **Complete Isolation**: Her tenant tamamen izole
- **Scalability**: Tenant bazlı scaling mümkün
- **Security**: Data leakage riski minimum
- **Compliance**: GDPR, SOX compliance kolay

## ⚡ Event-Driven Architecture

### İki Seviyeli Event Sistemi

#### 1. InMemory Events (Domain Events)
```csharp
public class SupplierCreatedEvent : IDomainEvent
{
    // Aynı transaction içinde, immediate business rules
}
```
**Kullanım**: Validation, audit logging, immediate business rules

#### 2. Distributed Events (Cross-Module)
```csharp
public class SupplierCreatedDistributedEvent : IDistributedEvent
{
    // Modüller arası, eventual consistency
}
```
**Kullanım**: Module-to-module communication, external integrations

### Event Flow
```
Procurement Module → InMemory Event → Business Rules
                  ↓
                  Distributed Event → Finance Module
                                   ↓
                                   Account Creation
```

### Tenant-Aware Messaging
RabbitMQ ile tenant bazlı event routing:
```
Exchange: domain_events
Routing: tenant.{tenantId}.{eventType}
Queues: tenant-specific queues
```

## 🔒 Güvenlik ve Yetkilendirme

### Authentication Flow
1. **OTP-based Authentication**: Email ile OTP gönderimi
2. **JWT Token**: Access + Refresh token
3. **Session Management**: Redis'te session tracking
4. **Multi-Factor**: Email OTP (SMS gelecek)

### Authorization Layers
```csharp
[Area("Admin")]
[Authorize(Roles = "SystemAdmin")]
public class AdminController : ControllerBase
{
    // Admin-only endpoints
}

[Area("Tenant")]
[RequireModule("Procurement")]
public class ProcurementController : ControllerBase
{
    // Tenant + module specific endpoints
}
```

### Route-Based Separation
```
/admin/*          → Admin operations (system management)
/tenant/*         → Tenant operations (business modules)
```

### Module Access Control
```csharp
public class TenantModuleAccess : BaseAuditEntity
{
    public Guid TenantId { get; set; }
    public string ModuleName { get; set; } // "Procurement", "Finance"
    public bool IsEnabled { get; set; }
}
```

## 🔄 Background Services

### Multi-Tenant Background Processing
```csharp
public class BusinessPartnerSyncBackgroundService : BackgroundService
{
    // Her tenant için paralel processing
    // Tenant-specific connection strings
    // Error handling per tenant
}
```

### Job Tracking System
```csharp
public class BackgroundJob : BaseAuditEntity
{
    public string JobType { get; set; }
    public JobStatus Status { get; set; }
    public string Progress { get; set; } // "3/10 processed"
}
```

### Real-time Status Updates
SignalR ile kullanıcılara job progress bildirimi

## 🚀 Geliştirme Planı

### ✅ Tamamlanan (Phase 1)
- [x] Core infrastructure
- [x] Multi-tenant foundation
- [x] Management module
- [x] Authentication system
- [x] Database per tenant setup
- [x] Basic event system

### 🚧 Devam Eden (Phase 2)
- [ ] Project Management (Management modülüne ekleme)
- [ ] Enhanced Authorization (project-based permissions)
- [ ] Module access control implementation
- [ ] Admin panel route separation

### 📋 Planlanan (Phase 3)
- [ ] Procurement modülü
  - [ ] Supplier CRUD
  - [ ] Purchase Request workflow
  - [ ] Basic approval system
- [ ] Cross-module communication
- [ ] Background job system

### 🔮 Gelecek (Phase 4)
- [ ] Workflow Engine (approval automation)
- [ ] Finance modülü
- [ ] RabbitMQ integration
- [ ] Advanced reporting
- [ ] Mobile API optimization

### 🎯 Gelecek Özellikler
- [ ] Workflow Engine (otomatik onay sistemi)
- [ ] Multi-language UI
- [ ] Advanced analytics
- [ ] Mobile app support
- [ ] External API integrations

## 💭 Mimari Kararlar ve Prensipler

### Cross-Module Communication
❌ **Navigation Properties**: Farklı DbContext'lerde çalışmaz
✅ **Foreign Key Only**: Sadece Guid referanslar
✅ **Service Communication**: Interface'ler üzerinden

### Entity Relationships
```csharp
// DOĞRU yaklaşım
public class Supplier : BaseAuditEntity
{
    public Guid? AccountId { get; set; }  // Finance modülündeki Account ID
    // Navigation property YOK
}

// Service layer'da join
public async Task<SupplierWithAccountDto> GetSupplierDetailsAsync(Guid supplierId)
{
    var supplier = await _supplierRepo.GetByIdAsync(supplierId);
    var account = supplier.AccountId.HasValue 
        ? await _financeService.GetAccountAsync(supplier.AccountId.Value)
        : null;
    return new SupplierWithAccountDto(supplier, account);
}
```

### Database Strategy
- **Database Per Tenant**: Complete isolation
- **Schema Per Module**: Modules içinde schema separation
- **Migration Strategy**: Automatic tenant DB creation

### Caching Strategy
```csharp
// Tenant-aware caching
string cacheKey = $"tenant:{tenantId}:suppliers";
await _cacheManager.SetAsync(cacheKey, data, TimeSpan.FromHours(1));
```

## 🚀 Kurulum ve Çalıştırma

### Gereksinimler
- .NET 9 SDK
- PostgreSQL 15+
- Redis 6+
- Docker (opsiyonel)

### Geliştirme Ortamı Kurulumu

1. **Repository Clone**
```bash
git clone https://github.com/yourusername/cloudot.git
cd cloudot
```

2. **Veritabanı Kurulumu**
```bash
# PostgreSQL ve Redis başlatın
docker-compose up -d postgres redis
```

3. **Configuration**
```bash
# appsettings.Development.json'ı düzenleyin
cp src/Cloudot.WebAPI/appsettings.json src/Cloudot.WebAPI/appsettings.Development.json
```

4. **Migration ve Seed**
```bash
cd src/Cloudot.Migrator
dotnet run
```

5. **API Başlatma**
```bash
cd src/Cloudot.WebAPI
dotnet run
```

### Test Kullanıcıları
- **Admin**: admin@mail.com
- **Demo Tenant**: Migrator ile otomatik oluşturulur

### API Documentation
- Swagger UI: `https://localhost:7299/swagger`
- Admin endpoints: `/admin/*`
- Tenant endpoints: `/tenant/*`

## 📞 İletişim ve Katkı

Bu proje aktif geliştirme aşamasındadır. Katkılarınızı bekliyoruz!

### Geliştirme Yaklaşımı
- **Domain-First**: Business logic öncelikli
- **Test-Driven**: Unit/Integration testler
- **Clean Code**: SOLID principles
- **Documentation**: Kod ve mimari dokümantasyonu

---

*Bu README sürekli güncellenmektedir. Proje geliştikçe yeni bölümler eklenecektir.*