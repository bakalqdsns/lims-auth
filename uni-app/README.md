# LimsAuth UniApp 实验室预约管理微信小程序

基于 UniApp + Vue 3 + TypeScript 构建的实验室预约管理微信小程序，与现有 ASP.NET Core 后端无缝对接。

## 功能模块

### 用户端
- 登录/退出
- 首页数据看板
- 实验室列表与详情预约
- 我的预约管理
- 设备借用与归还
- 借用记录与流程跟踪
- 课程中心
- 我的课表
- 个人中心

### 管理端
- 用户管理 (CRUD)
- 角色权限管理
- 部门管理
- 学期管理
- 设备管理 (CRUD + 统计)
- 授课审批中心
- 数据统计看板

## 技术栈

| 层级 | 技术 |
|------|------|
| 框架 | UniApp + Vue 3 + TypeScript |
| 状态管理 | Pinia + pinia-plugin-unistorage |
| HTTP | uni.request (原生封装) |
| 样式 | SCSS |
| 构建 | Vite |
| 目标平台 | 微信小程序 (主), H5 |

## 项目结构

```
uni-app/
├── src/
│   ├── api/          # 17 个 API 模块 (auth, user, lab, equipment...)
│   ├── stores/       # Pinia 状态管理 (auth, app, semester, lab...)
│   ├── pages/        # 页面 (login, home, dashboard, labs, borrow...)
│   ├── components/   # 可复用组件
│   ├── utils/        # 工具函数 (request, storage, permission, date...)
│   ├── types/        # TypeScript 类型定义
│   ├── pages.json    # 页面路由 + TabBar 配置
│   ├── manifest.json # UniApp 应用配置
│   ├── App.vue       # 应用入口
│   └── main.ts       # Vue 入口
├── package.json
└── README.md
```

## 快速开始

### 1. 安装依赖

```bash
cd uni-app
npm install
```

### 2. 配置微信小程序 AppID

编辑 `src/manifest.json`，将 `mp-weixin.appid` 替换为你的微信小程序 AppID。

### 3. 配置后端地址

编辑 `src/utils/request.ts`，将 `BASE_URL` 替换为实际的后端地址。

```typescript
const BASE_URL = 'http://172.16.155.113:5047'  // 开发环境
```

### 4. 运行开发

```bash
# 微信小程序
npm run dev:mp-weixin

# H5
npm run dev:h5
```

### 5. 微信开发者工具

1. 下载 [微信开发者工具](https://developers.weixin.qq.com/miniprogram/dev/devtools/download.html)
2. 打开微信开发者工具，导入 `uni-app` 目录
3. 使用自定义编译模式，编译运行

## API 对接

后端接口 Base URL: `http://localhost:5047/api/v1`

所有接口使用 JWT Bearer Token 认证。登录成功后 token 自动存储，后续请求自动注入 `Authorization: Bearer {token}` 头。

## 测试账号

| 角色 | 用户名 | 密码 |
|------|--------|------|
| 管理员 | admin | admin123 |
| 教师 | teacher | teacher123 |
| 学生 | student | student123 |

## 开发说明

- 主题色与 Flutter 版保持一致: Primary `#667eea`, PrimaryDark `#764ba2`
- 使用 Pinia 状态管理，与前端 Vue 项目 (`frontend/`) 技术栈一致
- uni.request 封装参考 Flutter Dio 实现，包含 401 自动登出逻辑
- 微信小程序需在 `manifest.json` 配置 AppID 才能调用微信 API
