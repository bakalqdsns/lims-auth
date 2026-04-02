# LIMS 实验室信息化管理系统

高校实验室信息化管理系统 (Laboratory Information Management System) - 登录认证模块

## 项目结构

```
lims-auth/
├── backend/          # .NET 8.0 Web API 后端
│   └── LimsAuth.Api/
├── frontend/         # Vue 3 + Element Plus 前端
├── mobile/           # Flutter 移动端 (跨平台)
│   └── NETWORK_CONFIG.md  # 移动端网络配置指南
├── mobile-android/   # Android 原生移动端 (Kotlin)
│   └── README.md     # Android 项目说明
└── tests/            # 测试用例
```

## 技术栈

### 后端
- **.NET 8.0** - Web API 框架
- **Entity Framework Core** - ORM
- **PostgreSQL** - 数据库
- **JWT** - 身份认证
- **Swagger** - API 文档

### Web 前端
- **Vue 3** - 前端框架
- **Element Plus** - UI 组件库
- **Pinia** - 状态管理
- **Axios** - HTTP 客户端
- **Vite** - 构建工具

### 移动端

#### Flutter 版本 (mobile/)
- **Flutter** - 跨平台框架
- **http** - 网络请求
- **shared_preferences** - 本地存储

#### Android 原生版本 (mobile-android/)
- **Kotlin** - 编程语言
- **Retrofit2** - 网络请求
- **OkHttp3** - HTTP 客户端
- **DataStore** - 本地存储
- **Material Design 3** - UI 组件
- **MVVM** - 架构模式

## 快速开始

### 环境要求
- .NET 8.0 SDK
- Node.js 18+
- PostgreSQL 14+
- Flutter 3.0+ (Flutter 移动端)
- Android Studio Hedgehog+ + JDK 17 (Android 原生移动端)

### 1. 启动后端

```bash
cd backend/LimsAuth.Api

# 配置数据库连接字符串 (appsettings.json)
# 默认: Host=127.0.0.1;Port=5432;Database=lims_db;Username=lims_user;Password=lims_password

dotnet run
```

后端服务将启动在 `http://localhost:5047`

### 2. 启动 Web 前端

```bash
cd frontend
npm install
npm run dev
```

前端开发服务器将启动在 `http://localhost:5173`

### 3. 启动 Flutter 移动端

```bash
cd mobile
flutter pub get
flutter run
```

**注意**: 真机测试时需要配置服务器地址，详见 [mobile/NETWORK_CONFIG.md](mobile/NETWORK_CONFIG.md)

### 4. 启动 Android 原生移动端

```bash
cd mobile-android
# 在 Android Studio 中打开项目
# 或使用命令行构建
./gradlew assembleDebug
./gradlew installDebug
```

**注意**: Android 原生版本使用 Kotlin + Retrofit + MVVM 架构，详见 [mobile-android/README.md](mobile-android/README.md)

## 测试账号

| 角色 | 用户名 | 密码 |
|------|--------|------|
| 管理员 | admin | admin123 |
| 教师 | teacher | teacher123 |
| 学生 | student | student123 |

## API 接口

### 认证接口

| 方法 | 路径 | 描述 |
|------|------|------|
| POST | /api/v1/auth/login | 用户登录 |
| GET | /api/v1/auth/me | 获取当前用户 |

### 登录请求示例

```bash
curl -X POST http://localhost:5047/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}'
```

### 响应格式

```json
{
  "code": 200,
  "message": "登录成功",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "tokenType": "Bearer",
    "expiresIn": 3600,
    "user": {
      "id": "...",
      "username": "admin",
      "role": "Admin",
      "fullName": "系统管理员"
    }
  }
}
```

## 端口配置

| 服务 | 端口 | 配置文件 |
|------|------|----------|
| 后端 API | 5047 | `launchSettings.json` |
| Web 前端 | 5173 | `vite.config.ts` |
| 前端代理 | 5047 | `vite.config.ts` |

## 移动端技术选型对比

| 特性 | Flutter (mobile/) | Android 原生 (mobile-android/) |
|------|-------------------|-------------------------------|
| 开发语言 | Dart | Kotlin |
| 架构模式 | StatefulWidget | MVVM + Repository |
| 网络库 | http | Retrofit2 + OkHttp3 |
| 本地存储 | shared_preferences | DataStore |
| UI 框架 | Flutter Widgets | Material Design 3 |
| 包大小 | 较大 | 较小 |
| 启动速度 | 一般 | 快 |
| 适用场景 | 跨平台需求 | 纯 Android 场景 |

### 如何选择

- **Flutter**: 需要同时支持 iOS 和 Android，追求开发效率
- **Android 原生**: 仅需 Android 平台，追求性能和原生体验

## 常见问题

### Web 端登录提示"服务器错误"
- 检查后端是否运行在 5047 端口
- 检查 `vite.config.ts` 中的代理配置是否正确

### 移动端提示"网络错误"
- 检查手机和服务器是否在同一网络
- 配置正确的服务器 IP 地址
- 详见 [mobile/NETWORK_CONFIG.md](mobile/NETWORK_CONFIG.md)

## 开发规范

- 后端遵循 RESTful API 设计规范
- 前端使用 Composition API 风格
- 提交信息遵循 [Conventional Commits](https://www.conventionalcommits.org/)

## 许可证

MIT License
