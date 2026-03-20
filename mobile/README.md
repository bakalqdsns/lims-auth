# 实验室管理系统 - 移动客户端

基于 Flutter 开发的 Android/iOS 客户端，与 Web 端共用同一套后端 API。

## 项目结构

```
lib/
├── main.dart              # 应用入口
├── screens/               # 页面
│   ├── login_screen.dart  # 登录页
│   └── home_screen.dart   # 首页
├── services/              # 服务
│   └── auth_service.dart  # 认证服务
└── ...
```

## 功能特性

- ✅ JWT 认证登录
- ✅ 本地 Token 持久化
- ✅ 角色显示（管理员/教师/学生）
- ✅ 底部导航栏
- ✅ 响应式 UI

## 测试账号

- admin / admin123
- teacher / teacher123
- student / student123

## 运行说明

### 1. 安装依赖
```bash
flutter pub get
```

### 2. 启动后端服务
确保后端运行在 `http://localhost:5000`

### 3. 运行 App
```bash
# Android 模拟器
flutter run

# 或构建 APK
flutter build apk
```

## 网络配置

Android 模拟器使用 `10.0.2.2` 访问宿主机 localhost。
如需使用真机测试，请修改 `lib/services/auth_service.dart` 中的 `_baseUrl`。

## 与 Web 端对比

| 功能 | Web (Vue) | Mobile (Flutter) |
|------|-----------|------------------|
| 登录页面 | ✅ | ✅ |
| JWT 认证 | ✅ | ✅ |
| 角色显示 | ✅ | ✅ |
| 响应式布局 | ✅ | ✅ |
| 底部导航 | ❌ | ✅ |
| 本地存储 | localStorage | SharedPreferences |
