#!/bin/bash

# LIMS 登录模块 API 测试脚本

BASE_URL="http://localhost:5000/api/v1"

echo "======================================"
echo "LIMS 登录模块 API 测试"
echo "======================================"
echo ""

# 检查后端服务是否运行
echo "检查后端服务..."
health_response=$(curl -s -o /dev/null -w "%{http_code}" "$BASE_URL/auth/health" 2>/dev/null)
if [ "$health_response" != "200" ]; then
    echo -e "\033[0;31m错误: 后端服务未启动或无法连接\033[0m"
    echo "请确保后端服务已启动:"
    echo "  cd backend/LimsAuth.Api && dotnet run --urls \"http://localhost:5000\""
    exit 1
fi
echo -e "\033[0;32m后端服务运行正常\033[0m"
echo ""

# 颜色定义
GREEN='\033[0;32m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# 测试计数
PASSED=0
FAILED=0

# 测试函数 - 检查业务状态码
test_api() {
    local name=$1
    local method=$2
    local endpoint=$3
    local data=$4
    local expected_business_code=$5
    
    echo -n "测试: $name ... "
    
    if [ -n "$data" ]; then
        response=$(curl -s -w "\n%{http_code}" -X $method "$BASE_URL$endpoint" \
            -H "Content-Type: application/json" \
            -d "$data")
    else
        response=$(curl -s -w "\n%{http_code}" -X $method "$BASE_URL$endpoint")
    fi
    
    http_code=$(echo "$response" | tail -n1)
    body=$(echo "$response" | sed '$d')
    
    # 提取业务状态码 (code 字段)
    business_code=$(echo "$body" | grep -o '"code":[0-9]*' | cut -d':' -f2)
    
    if [ "$business_code" -eq "$expected_business_code" ]; then
        echo -e "${GREEN}通过${NC} (业务码: $business_code)"
        ((PASSED++))
    else
        echo -e "${RED}失败${NC} (期望业务码: $expected_business_code, 实际: $business_code)"
        echo "响应: $body"
        ((FAILED++))
    fi
}

# 1. 健康检查
echo "--- 健康检查 ---"
test_api "健康检查" "GET" "/auth/health" "" 200

echo ""

# 2. 登录测试
echo "--- 登录测试 ---"
test_api "登录成功 - admin" "POST" "/auth/login" \
    '{"username": "admin", "password": "admin123"}' 200

test_api "登录失败 - 错误密码" "POST" "/auth/login" \
    '{"username": "admin", "password": "wrongpassword"}' 401

test_api "登录失败 - 用户不存在" "POST" "/auth/login" \
    '{"username": "nonexistent", "password": "password"}' 401

test_api "登录失败 - 空用户名" "POST" "/auth/login" \
    '{"username": "", "password": "password"}' 400

echo ""

# 3. 获取当前用户信息（需要 Token）
echo "--- 获取用户信息测试 ---"

# 先获取 token
login_response=$(curl -s -X POST "$BASE_URL/auth/login" \
    -H "Content-Type: application/json" \
    -d '{"username": "admin", "password": "admin123"}')
token=$(echo "$login_response" | grep -o '"token":"[^"]*"' | cut -d'"' -f4)

if [ -n "$token" ]; then
    echo -n "测试: 获取当前用户信息（带Token）... "
    response=$(curl -s -w "\n%{http_code}" -X GET "$BASE_URL/auth/me" \
        -H "Authorization: Bearer $token")
    http_code=$(echo "$response" | tail -n1)
    
    if [ "$http_code" -eq 200 ]; then
        echo -e "${GREEN}通过${NC} (HTTP $http_code)"
        ((PASSED++))
    else
        echo -e "${RED}失败${NC} (HTTP $http_code)"
        ((FAILED++))
    fi
    
    echo -n "测试: 获取当前用户信息（无Token）... "
    response=$(curl -s -w "\n%{http_code}" -X GET "$BASE_URL/auth/me")
    http_code=$(echo "$response" | tail -n1)
    
    if [ "$http_code" -eq 401 ]; then
        echo -e "${GREEN}通过${NC} (HTTP $http_code)"
        ((PASSED++))
    else
        echo -e "${RED}失败${NC} (HTTP $http_code)"
        ((FAILED++))
    fi
else
    echo -e "${RED}无法获取 token，跳过用户相关测试${NC}"
    ((FAILED+=2))
fi

echo ""
echo "======================================"
echo "测试结果"
echo "======================================"
echo -e "通过: ${GREEN}$PASSED${NC}"
echo -e "失败: ${RED}$FAILED${NC}"
echo "总计: $((PASSED + FAILED))"
echo ""

if [ $FAILED -eq 0 ]; then
    echo -e "${GREEN}所有测试通过!${NC}"
    exit 0
else
    echo -e "${RED}存在失败的测试${NC}"
    exit 1
fi
