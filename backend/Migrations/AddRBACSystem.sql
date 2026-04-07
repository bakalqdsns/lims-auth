-- 数据库升级脚本: 添加RBAC系统所需字段
-- 在现有 users 表基础上添加新字段

-- 1. 添加 users 表的新字段
ALTER TABLE users ADD COLUMN IF NOT EXISTS email VARCHAR(100);
ALTER TABLE users ADD COLUMN IF NOT EXISTS phone VARCHAR(20);
ALTER TABLE users ADD COLUMN IF NOT EXISTS avatar_url VARCHAR(500);
ALTER TABLE users ADD COLUMN IF NOT EXISTS department_id UUID;
ALTER TABLE users ADD COLUMN IF NOT EXISTS updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP;

-- 2. 创建 departments 表
CREATE TABLE IF NOT EXISTS departments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    code VARCHAR(50) NOT NULL UNIQUE,
    name VARCHAR(100) NOT NULL,
    parent_id UUID REFERENCES departments(id) ON DELETE RESTRICT,
    manager_id UUID,
    description VARCHAR(500),
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- 3. 创建 roles 表
CREATE TABLE IF NOT EXISTS roles (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    code VARCHAR(50) NOT NULL UNIQUE,
    name VARCHAR(100) NOT NULL,
    description VARCHAR(500),
    is_system BOOLEAN DEFAULT FALSE,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- 4. 创建 permissions 表
CREATE TABLE IF NOT EXISTS permissions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    code VARCHAR(100) NOT NULL UNIQUE,
    name VARCHAR(100) NOT NULL,
    module VARCHAR(50) NOT NULL,
    description VARCHAR(500),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- 5. 创建 user_roles 关联表
CREATE TABLE IF NOT EXISTS user_roles (
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    role_id UUID NOT NULL REFERENCES roles(id) ON DELETE CASCADE,
    assigned_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id, role_id)
);

-- 6. 创建 role_permissions 关联表
CREATE TABLE IF NOT EXISTS role_permissions (
    role_id UUID NOT NULL REFERENCES roles(id) ON DELETE CASCADE,
    permission_id UUID NOT NULL REFERENCES permissions(id) ON DELETE CASCADE,
    assigned_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (role_id, permission_id)
);

-- 7. 添加外键约束
ALTER TABLE users ADD CONSTRAINT fk_users_department 
    FOREIGN KEY (department_id) REFERENCES departments(id) ON DELETE SET NULL;

-- 8. 插入默认部门
INSERT INTO departments (id, code, name, description, is_active, created_at) VALUES
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'ROOT', '根部门', '系统根部门', TRUE, '2024-01-01 00:00:00+00')
ON CONFLICT (id) DO NOTHING;

INSERT INTO departments (id, code, name, parent_id, description, is_active, created_at) VALUES
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'LAB_CENTER', '实验中心', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '学校实验中心', TRUE, '2024-01-01 00:00:00+00'),
('cccccccc-cccc-cccc-cccc-cccccccccccc', 'CS_LAB', '计算机实验室', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', '计算机专业实验室', TRUE, '2024-01-01 00:00:00+00')
ON CONFLICT (id) DO NOTHING;

-- 9. 插入默认角色
INSERT INTO roles (id, code, name, description, is_system, is_active, created_at) VALUES
('11111111-1111-1111-1111-111111111111', 'super_admin', '超级管理员', '系统超级管理员，拥有所有权限', TRUE, TRUE, '2024-01-01 00:00:00+00'),
('22222222-2222-2222-2222-222222222222', 'lab_admin', '实验室管理员', '管理实验室、设备、预约', TRUE, TRUE, '2024-01-01 00:00:00+00'),
('33333333-3333-3333-3333-333333333333', 'teacher', '教师', '课程管理、学生管理', TRUE, TRUE, '2024-01-01 00:00:00+00'),
('44444444-4444-4444-4444-444444444444', 'student', '学生', '预约设备、提交报告', TRUE, TRUE, '2024-01-01 00:00:00+00'),
('55555555-5555-5555-5555-555555555555', 'auditor', '审计员', '查看日志、报表', TRUE, TRUE, '2024-01-01 00:00:00+00')
ON CONFLICT (id) DO NOTHING;

-- 10. 插入权限数据
INSERT INTO permissions (id, code, name, module, description, created_at) VALUES
-- 用户管理权限
('10000000-0000-0000-0000-000000000001', 'user:create', '创建用户', 'user', '创建新用户', '2024-01-01 00:00:00+00'),
('10000000-0000-0000-0000-000000000002', 'user:read', '查看用户', 'user', '查看用户信息', '2024-01-01 00:00:00+00'),
('10000000-0000-0000-0000-000000000003', 'user:update', '编辑用户', 'user', '编辑用户信息', '2024-01-01 00:00:00+00'),
('10000000-0000-0000-0000-000000000004', 'user:delete', '删除用户', 'user', '删除用户', '2024-01-01 00:00:00+00'),
('10000000-0000-0000-0000-000000000005', 'user:reset_password', '重置密码', 'user', '重置用户密码', '2024-01-01 00:00:00+00'),
-- 角色管理权限
('20000000-0000-0000-0000-000000000001', 'role:create', '创建角色', 'role', '创建新角色', '2024-01-01 00:00:00+00'),
('20000000-0000-0000-0000-000000000002', 'role:read', '查看角色', 'role', '查看角色信息', '2024-01-01 00:00:00+00'),
('20000000-0000-0000-0000-000000000003', 'role:update', '编辑角色', 'role', '编辑角色信息', '2024-01-01 00:00:00+00'),
('20000000-0000-0000-0000-000000000004', 'role:delete', '删除角色', 'role', '删除角色', '2024-01-01 00:00:00+00'),
('20000000-0000-0000-0000-000000000005', 'role:assign', '分配角色', 'role', '为用户分配角色', '2024-01-01 00:00:00+00'),
-- 权限管理权限
('30000000-0000-0000-0000-000000000001', 'permission:read', '查看权限', 'permission', '查看权限列表', '2024-01-01 00:00:00+00'),
('30000000-0000-0000-0000-000000000002', 'permission:assign', '分配权限', 'permission', '为角色分配权限', '2024-01-01 00:00:00+00'),
-- 部门管理权限
('40000000-0000-0000-0000-000000000001', 'department:create', '创建部门', 'department', '创建新部门', '2024-01-01 00:00:00+00'),
('40000000-0000-0000-0000-000000000002', 'department:read', '查看部门', 'department', '查看部门信息', '2024-01-01 00:00:00+00'),
('40000000-0000-0000-0000-000000000003', 'department:update', '编辑部门', 'department', '编辑部门信息', '2024-01-01 00:00:00+00'),
('40000000-0000-0000-0000-000000000004', 'department:delete', '删除部门', 'department', '删除部门', '2024-01-01 00:00:00+00'),
-- 设备管理权限
('50000000-0000-0000-0000-000000000001', 'equipment:create', '创建设备', 'equipment', '创建新设备', '2024-01-01 00:00:00+00'),
('50000000-0000-0000-0000-000000000002', 'equipment:read', '查看设备', 'equipment', '查看设备信息', '2024-01-01 00:00:00+00'),
('50000000-0000-0000-0000-000000000003', 'equipment:update', '编辑设备', 'equipment', '编辑设备信息', '2024-01-01 00:00:00+00'),
('50000000-0000-0000-0000-000000000004', 'equipment:delete', '删除设备', 'equipment', '删除设备', '2024-01-01 00:00:00+00'),
('50000000-0000-0000-0000-000000000005', 'equipment:borrow', '借用设备', 'equipment', '借用设备', '2024-01-01 00:00:00+00'),
-- 实验室管理权限
('60000000-0000-0000-0000-000000000001', 'lab:create', '创建实验室', 'lab', '创建新实验室', '2024-01-01 00:00:00+00'),
('60000000-0000-0000-0000-000000000002', 'lab:read', '查看实验室', 'lab', '查看实验室信息', '2024-01-01 00:00:00+00'),
('60000000-0000-0000-0000-000000000003', 'lab:update', '编辑实验室', 'lab', '编辑实验室信息', '2024-01-01 00:00:00+00'),
('60000000-0000-0000-0000-000000000004', 'lab:delete', '删除实验室', 'lab', '删除实验室', '2024-01-01 00:00:00+00'),
-- 课程管理权限
('70000000-0000-0000-0000-000000000001', 'course:create', '创建课程', 'course', '创建新课程', '2024-01-01 00:00:00+00'),
('70000000-0000-0000-0000-000000000002', 'course:read', '查看课程', 'course', '查看课程信息', '2024-01-01 00:00:00+00'),
('70000000-0000-0000-0000-000000000003', 'course:update', '编辑课程', 'course', '编辑课程信息', '2024-01-01 00:00:00+00'),
('70000000-0000-0000-0000-000000000004', 'course:delete', '删除课程', 'course', '删除课程', '2024-01-01 00:00:00+00'),
('70000000-0000-0000-0000-000000000005', 'course:schedule', '排课', 'course', '课程排期', '2024-01-01 00:00:00+00'),
-- 报告管理权限
('80000000-0000-0000-0000-000000000001', 'report:create', '创建报告', 'report', '创建新报告', '2024-01-01 00:00:00+00'),
('80000000-0000-0000-0000-000000000002', 'report:read', '查看报告', 'report', '查看报告', '2024-01-01 00:00:00+00'),
('80000000-0000-0000-0000-000000000003', 'report:approve', '审批报告', 'report', '审批报告', '2024-01-01 00:00:00+00'),
-- 系统管理权限
('90000000-0000-0000-0000-000000000001', 'system:config', '系统配置', 'system', '系统配置管理', '2024-01-01 00:00:00+00'),
('90000000-0000-0000-0000-000000000002', 'system:log', '查看日志', 'system', '查看系统日志', '2024-01-01 00:00:00+00')
ON CONFLICT (id) DO NOTHING;

-- 11. 为超级管理员分配所有权限
INSERT INTO role_permissions (role_id, permission_id, assigned_at)
SELECT '11111111-1111-1111-1111-111111111111', id, '2024-01-01 00:00:00+00'
FROM permissions
ON CONFLICT (role_id, permission_id) DO NOTHING;

-- 12. 为现有用户分配默认角色
-- admin -> super_admin
INSERT INTO user_roles (user_id, role_id, assigned_at)
SELECT id, '11111111-1111-1111-1111-111111111111', '2024-01-01 00:00:00+00'
FROM users WHERE username = 'admin'
ON CONFLICT (user_id, role_id) DO NOTHING;

-- teacher -> teacher
INSERT INTO user_roles (user_id, role_id, assigned_at)
SELECT id, '33333333-3333-3333-3333-333333333333', '2024-01-01 00:00:00+00'
FROM users WHERE username = 'teacher'
ON CONFLICT (user_id, role_id) DO NOTHING;

-- student -> student
INSERT INTO user_roles (user_id, role_id, assigned_at)
SELECT id, '44444444-4444-4444-4444-444444444444', '2024-01-01 00:00:00+00'
FROM users WHERE username = 'student'
ON CONFLICT (user_id, role_id) DO NOTHING;

-- 13. 更新现有用户的部门
UPDATE users SET department_id = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa' WHERE department_id IS NULL;

-- 14. 创建索引
CREATE INDEX IF NOT EXISTS idx_users_department_id ON users(department_id);
CREATE INDEX IF NOT EXISTS idx_users_email ON users(email) WHERE email IS NOT NULL;
CREATE INDEX IF NOT EXISTS idx_departments_parent_id ON departments(parent_id);
CREATE INDEX IF NOT EXISTS idx_user_roles_user_id ON user_roles(user_id);
CREATE INDEX IF NOT EXISTS idx_user_roles_role_id ON user_roles(role_id);
CREATE INDEX IF NOT EXISTS idx_role_permissions_role_id ON role_permissions(role_id);
CREATE INDEX IF NOT EXISTS idx_role_permissions_permission_id ON role_permissions(permission_id);
CREATE INDEX IF NOT EXISTS idx_permissions_module ON permissions(module);

COMMIT;
