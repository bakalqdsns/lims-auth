namespace LimsAuth.Api.Data.Configuration;

/// <summary>
/// 各模块配置文件共用的种子数据常量，避免硬编码 GUID
/// </summary>
internal static class SeedKeys
{
    // ===== 日期常量 =====
    public static DateTime SeedDate { get; } = new DateTime(2024, 1, 1, 0, 0, 0);

    // ===== 角色 ID =====
    public static Guid SuperAdminRoleId { get; } = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static Guid LabAdminRoleId { get; } = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static Guid TeacherRoleId { get; } = Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static Guid StudentRoleId { get; } = Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static Guid AuditorRoleId { get; } = Guid.Parse("55555555-5555-5555-5555-555555555555");

    // ===== 用户 ID =====
    public static Guid AdminUserId { get; } = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static Guid TeacherUserId { get; } = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static Guid StudentUserId { get; } = Guid.Parse("33333333-3333-3333-3333-333333333333");

    // ===== 部门 ID =====
    public static Guid RootDeptId { get; } = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static Guid LabDeptId { get; } = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    public static Guid CsDeptId { get; } = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

    // ===== 学期 ID =====
    public static Guid SemesterId { get; } = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

    // ===== 专业/班级 ID =====
    public static Guid MajorId { get; } = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
    public static Guid ClassId { get; } = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
    public static Guid CourseId { get; } = Guid.Parse("11111111-2222-3333-4444-555555555555");
    public static Guid TaskId { get; } = Guid.Parse("22222222-3333-4444-5555-666666666666");

    // ===== 校区/楼宇 ID =====
    public static Guid MainCampusId { get; } = Guid.Parse("c0000000-0000-0000-0000-000000000001");
    public static Guid EastCampusId { get; } = Guid.Parse("c0000000-0000-0000-0000-000000000002");
    public static Guid BuildingAId { get; } = Guid.Parse("b0000000-0000-0000-0000-000000000001");
    public static Guid BuildingBId { get; } = Guid.Parse("b0000000-0000-0000-0000-000000000002");
    public static Guid BuildingCId { get; } = Guid.Parse("b0000000-0000-0000-0000-000000000003");

    // ===== 实验室 ID =====
    public static Guid Lab1Id { get; } = Guid.Parse("f0000000-0000-0000-0000-000000000001");
    public static Guid Lab2Id { get; } = Guid.Parse("f0000000-0000-0000-0000-000000000002");
    public static Guid Lab3Id { get; } = Guid.Parse("f0000000-0000-0000-0000-000000000003");

    // ===== 机构 ID =====
    public static Guid SchoolId { get; } = Guid.Parse("a0000000-0000-0000-0000-000000000001");
    public static Guid DeptCSId { get; } = Guid.Parse("a0000000-0000-0000-0000-000000000002");
    public static Guid DeptPhysicsId { get; } = Guid.Parse("a0000000-0000-0000-0000-000000000003");
    public static Guid DeptChemistryId { get; } = Guid.Parse("a0000000-0000-0000-0000-000000000004");
    public static Guid LabCenterId { get; } = Guid.Parse("a0000000-0000-0000-0000-000000000005");

    // ===== 实验教学任务 ID =====
    public static Guid ExpTask1Id { get; } = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    public static Guid ExpTask2Id { get; } = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb01");
    public static Guid ExpTask3Id { get; } = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb02");
    public static Guid ExpTask4Id { get; } = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb03");

    // ===== 实验项目 ID =====
    public static Guid ExpItem1Id { get; } = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static Guid ExpItem2Id { get; } = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa01");
    public static Guid ExpItem3Id { get; } = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa02");
    public static Guid ExpItem4Id { get; } = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa03");
    public static Guid ExpItem5Id { get; } = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa04");
    public static Guid ExpItem6Id { get; } = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa05");
    public static Guid ExpItem7Id { get; } = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa06");
    public static Guid ExpItem8Id { get; } = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa07");
    public static Guid ExpItem9Id { get; } = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa08");
    public static Guid ExpItem10Id { get; } = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa09");

    // ===== 实验安排 ID =====
    public static Guid ExpSchedule1Id { get; } = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    public static Guid ExpSchedule2Id { get; } = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc01");
    public static Guid ExpSchedule3Id { get; } = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc02");
    public static Guid ExpSchedule4Id { get; } = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc03");
    public static Guid ExpSchedule5Id { get; } = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc04");
    public static Guid ExpSchedule6Id { get; } = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc05");
    public static Guid ExpSchedule7Id { get; } = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc06");
    public static Guid ExpSchedule8Id { get; } = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc07");

    // ===== 教学质量评估 ID =====
    public static Guid Assessment1Id { get; } = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
    public static Guid Assessment2Id { get; } = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddd0001");
    public static Guid Assessment3Id { get; } = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddd0002");

    // ===== 实训教学计划 ID =====
    public static Guid Plan1Id { get; } = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeee0001");
    public static Guid Plan2Id { get; } = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeee0002");
    public static Guid Plan3Id { get; } = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeee0003");

    // ===== 排课预约 ID =====
    public static Guid Entry1Id { get; } = Guid.Parse("e1000000-0000-0000-0000-000000000001");
    public static Guid Entry2Id { get; } = Guid.Parse("e1000000-0000-0000-0000-000000000002");
    public static Guid Entry3Id { get; } = Guid.Parse("e1000000-0000-0000-0000-000000000003");
    public static Guid Entry4Id { get; } = Guid.Parse("e1000000-0000-0000-0000-000000000004");
    public static Guid Res1Id { get; } = Guid.Parse("e2000000-0000-0000-0000-000000000001");
    public static Guid Res2Id { get; } = Guid.Parse("e2000000-0000-0000-0000-000000000002");
    public static Guid Res3Id { get; } = Guid.Parse("e2000000-0000-0000-0000-000000000003");
    public static Guid Reg1Id { get; } = Guid.Parse("e3000000-0000-0000-0000-000000000001");
    public static Guid Reg2Id { get; } = Guid.Parse("e3000000-0000-0000-0000-000000000002");
    public static Guid Reg3Id { get; } = Guid.Parse("e3000000-0000-0000-0000-000000000003");
    public static Guid Reg4Id { get; } = Guid.Parse("e3000000-0000-0000-0000-000000000004");
    public static Guid Ta1Id { get; } = Guid.Parse("e4000000-0000-0000-0000-000000000001");
    public static Guid Ta2Id { get; } = Guid.Parse("e4000000-0000-0000-0000-000000000002");
    public static Guid Ta3Id { get; } = Guid.Parse("e4000000-0000-0000-0000-000000000003");
}
