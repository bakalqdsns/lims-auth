# LIMS 实验室信息化管理系统

高校实验室信息化管理系统 (Laboratory Information Management System) - 登录认证模块

## 项目结构

```
lims-auth/
├── backend/          # .NET 8.0 Web API 后端
│   └── LimsAuth.Api/
├── frontend/         # Vue 3 + Element Plus 前端
├── mobile/           # Flutter 移动端
│   └── NETWORK_CONFIG.md  # 移动端网络配置指南
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
- **Flutter** - 跨平台框架
- **http** - 网络请求
- **shared_preferences** - 本地存储

## 快速开始

### 环境要求
- .NET 8.0 SDK
- Node.js 18+
- PostgreSQL 14+
- Flutter 3.0+ (移动端)

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

### 3. 启动移动端

```bash
cd mobile
flutter pub get
flutter run
```

**注意**: 真机测试时需要配置服务器地址，详见 [mobile/NETWORK_CONFIG.md](mobile/NETWORK_CONFIG.md)

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
