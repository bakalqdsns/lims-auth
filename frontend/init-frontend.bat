@echo off
title LIMS Frontend Init

echo ==============================
echo   LIMS Frontend 初始化脚本
echo ==============================

cd /d %~dp0
cd frontend

echo.
echo [1/5] 检查 Node.js 环境...
node -v >nul 2>nul
if %errorlevel% neq 0 (
    echo ❌ 未检测到 Node.js，请先安装 Node.js 18+
    pause
    exit
)

echo ✔ Node.js 已安装

echo.
echo [2/5] 检查 npm...
npm -v >nul 2>nul
if %errorlevel% neq 0 (
    echo ❌ npm 不可用
    pause
    exit
)

echo ✔ npm 正常

echo.
echo [3/5] 清理旧依赖...
if exist node_modules (
    rd /s /q node_modules
    echo ✔ 已删除 node_modules
)

if exist package-lock.json (
    del package-lock.json
    echo ✔ 已删除 package-lock.json
)

echo.
echo [4/5] 安装依赖（可能需要1~3分钟）...
npm install

if %errorlevel% neq 0 (
    echo ❌ 依赖安装失败，请检查网络或 npm 源
    pause
    exit
)

echo ✔ 依赖安装完成

echo.
echo [5/5] 启动开发服务器...
echo ==============================
echo   启动中：http://localhost:5173
echo ==============================

npm run dev

pause