# LIMS 登录模块测试用例

## 1. 后端 API 测试

### 1.1 健康检查接口
```bash
curl -X GET http://localhost:5000/api/v1/auth/health
```
**预期结果：**
```json
{
  "code": 200,
  "message": "服务正常运行",
  "data": { "time": "2024-01-01T00:00:00Z" }
}
```

### 1.2 登录接口 - 成功
```bash
curl -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username": "admin", "password": "admin123"}'
```
**预期结果：**
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

### 1.3 登录接口 - 失败（错误密码）
```bash
curl -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username": "admin", "password": "wrongpassword"}'
```
**预期结果：**
```json
{
  "code": 401,
  "message": "用户名或密码错误"
}
```

### 1.4 登录接口 - 失败（用户不存在）
```bash
curl -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username": "nonexistent", "password": "password"}'
```
**预期结果：**
```json
{
  "code": 401,
  "message": "用户名或密码错误"
}
```

### 1.5 获取当前用户信息（需要 Token）
```bash
curl -X GET http://localhost:5000/api/v1/auth/me \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```
**预期结果：**
```json
{
  "code": 200,
  "message": "success",
  "data": {
    "id": "...",
    "username": "admin",
    "role": "Admin",
    "fullName": "系统管理员"
  }
}
```

### 1.6 获取当前用户信息（无 Token）
```bash
curl -X GET http://localhost:5000/api/v1/auth/me
```
**预期结果：**
```json
{
  "code": 401,
  "message": "未授权"
}
```

## 2. 前端功能测试

### 2.1 登录页面显示
- 访问 http://localhost:5173
- 预期：显示登录表单，包含用户名、密码输入框和登录按钮

### 2.2 登录成功流程
1. 输入用户名：admin
2. 输入密码：admin123
3. 点击登录按钮
4. 预期：
   - 显示 "登录成功" 提示
   - 跳转到首页 (/home)
   - 显示用户信息

### 2.3 登录失败流程
1. 输入用户名：admin
2. 输入密码：wrongpassword
3. 点击登录按钮
4. 预期：
   - 显示 "用户名或密码错误" 提示
   - 停留在登录页面

### 2.4 表单验证
1. 不输入用户名，直接点击登录
2. 预期：显示 "请输入用户名" 提示

### 2.5 记住登录状态
1. 成功登录
2. 刷新页面
3. 预期：保持登录状态，不需要重新登录

### 2.6 退出登录
1. 登录后点击右上角用户头像
2. 选择 "退出登录"
3. 预期：
   - 显示确认对话框
   - 确认后显示 "已退出登录"
   - 跳转到登录页面

## 3. 角色权限测试

### 3.1 管理员登录
- 用户名：admin / admin123
- 预期：显示管理员角色标签（红色）

### 3.2 教师登录
- 用户名：teacher / teacher123
- 预期：显示教师角色标签（绿色）

### 3.3 学生登录
- 用户名：student / student123
- 预期：显示学生角色标签（灰色）

## 4. 响应式测试

### 4.1 PC 端显示
- 在 1920x1080 分辨率下访问
- 预期：登录框居中，布局正常

### 4.2 移动端显示
- 在 375x667 分辨率下访问
- 预期：登录框自适应宽度，字体大小合适

## 5. 性能测试

### 5.1 登录响应时间
- 预期：登录 API 响应时间 < 500ms

### 5.2 页面加载时间
- 预期：登录页面加载时间 < 2s

## 6. 安全测试

### 6.1 SQL 注入测试
```bash
curl -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username": "admin\' OR \'1\'=\'1", "password": "anything"}'
```
- 预期：登录失败，返回 401

### 6.2 XSS 测试
- 在用户名输入框输入：`<script>alert('xss')</script>`
- 预期：脚本不会执行，作为普通文本处理

## 7. 自动化测试脚本

### 7.1 运行后端测试
```bash
cd backend/LimsAuth.Api
dotnet test
```

### 7.2 API 集成测试
```bash
# 测试登录 API
./tests/api/test_login.sh
```

### 7.3 前端 E2E 测试
```bash
cd frontend
npm run test:e2e
```
