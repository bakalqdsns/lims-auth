# LIMS 移动端网络配置指南

## 问题描述
安卓真机测试时出现"网络错误"，通常是因为服务器地址配置不正确。

## 修复内容

### 1. 可配置的服务器地址
- 新增 `lib/config/api_config.dart` - 集中管理 API 配置
- 登录界面新增"服务器配置"折叠面板
- 支持动态修改服务器地址

### 2. 改进的错误提示
- 区分不同类型的网络错误
- 提供具体的排查建议
- 连接超时时显示配置按钮

### 3. 网络诊断工具
- 新增 `lib/utils/network_diagnostics.dart`
- 可检测连接状态和网络配置

### 4. 后端监听地址
- 修改 `launchSettings.json` 使用 `0.0.0.0` 代替 `localhost`
- 允许局域网内其他设备访问

## 使用步骤

### 步骤 1: 启动后端服务
```bash
cd /home/vboxuser/.openclaw/workspace-scool/lims-auth/backend/LimsAuth.Api
dotnet run
```
确保服务监听在 `http://0.0.0.0:5047`

### 步骤 2: 获取服务器 IP 地址
在服务器机器上运行：
```bash
ip addr show | grep "inet " | head -5
```
或
```bash
hostname -I
```

找到局域网 IP，例如 `192.168.1.100`

### 步骤 3: 配置移动端
1. 打开 App
2. 点击登录界面的"服务器配置"
3. 输入服务器地址：`http://192.168.1.100:5047`
4. 点击保存
5. 使用测试账号登录

## 常见配置

| 场景 | 服务器地址 |
|------|-----------|
| Android 模拟器 | `http://10.0.2.2:5047` |
| iOS 模拟器 | `http://localhost:5047` |
| 真机（同一WiFi）| `http://<服务器IP>:5047` |
| 生产环境 | `https://your-api.com` |

## 故障排查

### 1. 检查后端是否运行
```bash
curl http://localhost:5047/api/v1/auth/login -X POST \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}'
```

### 2. 检查防火墙
确保端口 5047 未被防火墙阻止：
```bash
sudo ufw allow 5047/tcp
```

### 3. 检查网络连通性
从手机浏览器访问：
```
http://<服务器IP>:5047/swagger
```
如果能看到 Swagger 页面，说明网络连通。

### 4. 查看详细错误
在 `auth_service.dart` 中已添加 `print` 语句，可在控制台查看详细连接信息。

## 测试账号
- 管理员: `admin` / `admin123`
- 教师: `teacher` / `teacher123`
- 学生: `student` / `student123`
