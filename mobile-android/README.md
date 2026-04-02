# LIMS Android 原生移动端

基于 Kotlin + Retrofit + MVVM 架构的 Android 原生登录模块。

## 技术栈

- **语言**: Kotlin
- **架构**: MVVM + Repository 模式
- **网络**: Retrofit2 + OkHttp3 + Gson
- **异步**: Kotlin Coroutines + Flow
- **存储**: DataStore (替代 SharedPreferences)
- **UI**: XML Layout + Material Design 3

## 项目结构

```
mobile-android/
├── app/
│   ├── src/main/java/com/lims/mobile/
│   │   ├── LimsApplication.kt          # 应用入口
│   │   ├── data/
│   │   │   └── AuthRepository.kt       # 认证仓库
│   │   ├── model/
│   │   │   └── AuthModels.kt           # 数据模型
│   │   ├── network/
│   │   │   └── AuthApi.kt              # API 接口
│   │   ├── ui/
│   │   │   ├── login/
│   │   │   │   ├── LoginActivity.kt    # 登录界面
│   │   │   │   └── LoginViewModel.kt   # 登录VM
│   │   │   └── home/
│   │   │       └── HomeActivity.kt     # 首页
│   │   └── utils/
│   │       ├── NetworkUtils.kt         # 网络工具
│   │       └── PreferencesManager.kt   # 偏好设置管理
│   ├── src/main/res/
│   │   ├── layout/                     # 布局文件
│   │   ├── drawable/                   # 图标资源
│   │   ├── values/                     # 颜色、字符串、主题
│   │   └── menu/                       # 菜单
│   └── build.gradle.kts                # 模块构建配置
├── build.gradle.kts                    # 项目构建配置
└── settings.gradle.kts                 # 项目设置
```

## 快速开始

### 1. 环境要求
- Android Studio Hedgehog (2023.1.1) 或更高
- JDK 17
- Android SDK 34
- Kotlin 1.9.20

### 2. 打开项目
```bash
# 在 Android Studio 中打开
File -> Open -> 选择 mobile-android 文件夹
```

### 3. 配置服务器地址
- 默认地址: `http://192.168.1.100:5047/api/v1/`
- 模拟器: `http://10.0.2.2:5047/api/v1/`
- 可在登录页面点击"服务器配置"修改

### 4. 构建运行
```bash
# 构建 APK
./gradlew assembleDebug

# 安装到设备
./gradlew installDebug
```

## 功能特性

- ✅ 用户登录 (JWT Token)
- ✅ 自动登录 (Token 持久化)
- ✅ 服务器地址配置
- ✅ 测试账号快捷填充
- ✅ 角色显示 (管理员/教师/学生)
- ✅ 侧边栏导航
- ✅ Material Design 3 风格

## 测试账号

| 角色 | 用户名 | 密码 |
|------|--------|------|
| 管理员 | admin | admin123 |
| 教师 | teacher | teacher123 |
| 学生 | student | student123 |

## 与后端对接

确保后端服务运行在 `http://localhost:5047`，并允许局域网访问：

```bash
cd ../backend/LimsAuth.Api
dotnet run --urls "http://0.0.0.0:5047"
```

## 与 Flutter 版本对比

| 特性 | Flutter | Android 原生 |
|------|---------|-------------|
| 包大小 | 较大 | 较小 |
| 启动速度 | 一般 | 快 |
| 原生体验 | 接近 | 完美 |
| 开发效率 | 高 | 中等 |
| 维护成本 | 单代码库 | 需单独维护 |

## 待办事项

- [ ] 添加密码可见性切换
- [ ] 添加记住密码功能
- [ ] 添加指纹/面容登录
- [ ] 添加网络状态监听
- [ ] 添加日志上报
