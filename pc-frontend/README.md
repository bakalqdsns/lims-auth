# LimsAuth PC Client

基于 Electron + Vue 3 的桌面客户端，内嵌浏览器方式运行全部 Web 前端功能。

## 功能

- 完整保留原 Web 前端所有功能
- 独立桌面窗口（最小化/最大化/关闭）
- 系统托盘支持（最小化到托盘、双击恢复）
- 桌面菜单栏（文件/视图/帮助）
- 缩放控制（放大/缩小/重置）
- 全屏切换（F11）
- 应用内打开外部链接（Swagger 文档等）

## 运行

```bash
# 安装依赖
npm install

# 开发模式（前端 Vite Dev Server + Electron）
npm run electron:dev

# 生产构建
npm run electron:build
```

构建完成后，安装包位于 `release/` 目录。

## 技术栈

- Electron 28
- Vue 3 + TypeScript
- Vite 5 + vite-plugin-electron
- Element Plus
- Pinia
- Vue Router

## 后端依赖

需要后台运行 `LimsAuth.Api`（默认 http://127.0.0.1:5047）。
