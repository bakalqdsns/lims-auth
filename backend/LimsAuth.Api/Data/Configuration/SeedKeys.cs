using System;

namespace LimsAuth.Api.Data.Configuration;

/// <summary>
/// 跨模块共享的种子数据常量(GUID/DateTime),供各 ModelConfiguration 引用。
/// 必须保证 ID 跨模块引用一致(例如 SemesterId 出现在 Teaching、Experiment、Schedule 模块中)。
/// </summary>
internal static class SeedKeys
{
    public static readonly DateTime SeedDate = new DateTime(2024, 1, 1, 0, 0, 0);

    // ========== 部门 ==========
    public static readonly Guid RootDeptId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid LabDeptId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    public static readonly Guid CsDeptId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

    // ========== 角色 ==========
    public static readonly Guid SuperAdminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid LabAdminRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid TeacherRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static readonly Guid StudentRoleId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static readonly Guid AuditorRoleId = Guid.Parse("55555555-5555-5555-5555-555555555555");

    // ========== 用户 ==========
    public static readonly Guid AdminUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid TeacherUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid StudentUserId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    // ========== 教学(Teaching) ==========
    public static readonly Guid SemesterId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
    public static readonly Guid MajorId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
    public static readonly Guid ClassId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
    public static readonly Guid CourseId = Guid.Parse("11111111-2222-3333-4444-555555555555");
    public static readonly Guid TaskId = Guid.Parse("22222222-3333-4444-5555-666666666666");

    // ========== 场地(Venue) ==========
    public static readonly Guid MainCampusId = Guid.Parse("c0000000-0000-0000-0000-000000000001");
    public static readonly Guid EastCampusId = Guid.Parse("c0000000-0000-0000-0000-000000000002");
    public static readonly Guid BuildingAId = Guid.Parse("b0000000-0000-0000-0000-000000000001");
    public static readonly Guid BuildingBId = Guid.Parse("b0000000-0000-0000-0000-000000000002");
    public static readonly Guid BuildingCId = Guid.Parse("b0000000-0000-0000-0000-000000000003");
    public static readonly Guid Lab1Id = Guid.Parse("f0000000-0000-0000-0000-000000000001");
    public static readonly Guid Lab2Id = Guid.Parse("f0000000-0000-0000-0000-000000000002");
    public static readonly Guid Lab3Id = Guid.Parse("f0000000-0000-0000-0000-000000000003");

    // ========== 实验(Experiment) ==========
    public static readonly Guid TasksId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    public static readonly Guid LabCenterId = Guid.Parse("a0000000-0000-0000-0000-000000000005");
    public static readonly Guid SchoolId = Guid.Parse("a0000000-0000-0000-0000-000000000001");
    public static readonly Guid DeptCSId = Guid.Parse("a0000000-0000-0000-0000-000000000002");
    public static readonly Guid DeptPhysicsId = Guid.Parse("a0000000-0000-0000-0000-000000000003");
    public static readonly Guid DeptChemistryId = Guid.Parse("a0000000-0000-0000-0000-000000000004");
    public static readonly Guid Task2Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb01");
    public static readonly Guid Task3Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb02");
    public static readonly Guid Task4Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb03");
    public static readonly Guid ItemId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid Item2Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa01");
    public static readonly Guid Item3Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa02");
    public static readonly Guid Item4Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa03");
    public static readonly Guid Item5Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa04");
    public static readonly Guid Item6Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa05");
    public static readonly Guid Item7Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa06");
    public static readonly Guid Item8Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa07");
    public static readonly Guid Item9Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa08");
    public static readonly Guid Item10Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa09");
    public static readonly Guid ScheduleId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    public static readonly Guid Schedule2Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc01");
    public static readonly Guid Schedule3Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc02");
    public static readonly Guid Schedule4Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc03");
    public static readonly Guid Schedule5Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc04");
    public static readonly Guid Schedule6Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc05");
    public static readonly Guid Schedule7Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc06");
    public static readonly Guid Schedule8Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc07");
    public static readonly Guid AssessmentId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
    public static readonly Guid Assessment2Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddd0001");
    public static readonly Guid Assessment3Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddd0002");
    public static readonly Guid Plan1Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeee0001");
    public static readonly Guid Plan2Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeee0002");
    public static readonly Guid Plan3Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeee0003");

    // ========== 排课(Schedule) ==========
    public static readonly Guid Entry1Id = Guid.Parse("e1000000-0000-0000-0000-000000000001");
    public static readonly Guid Entry2Id = Guid.Parse("e1000000-0000-0000-0000-000000000002");
    public static readonly Guid Entry3Id = Guid.Parse("e1000000-0000-0000-0000-000000000003");
    public static readonly Guid Entry4Id = Guid.Parse("e1000000-0000-0000-0000-000000000004");
    public static readonly Guid Res1Id = Guid.Parse("e2000000-0000-0000-0000-000000000001");
    public static readonly Guid Res2Id = Guid.Parse("e2000000-0000-0000-0000-000000000002");
    public static readonly Guid Res3Id = Guid.Parse("e2000000-0000-0000-0000-000000000003");
    public static readonly Guid Reg1Id = Guid.Parse("e3000000-0000-0000-0000-000000000001");
    public static readonly Guid Reg2Id = Guid.Parse("e3000000-0000-0000-0000-000000000002");
    public static readonly Guid Reg3Id = Guid.Parse("e3000000-0000-0000-0000-000000000003");
    public static readonly Guid Reg4Id = Guid.Parse("e3000000-0000-0000-0000-000000000004");
    public static readonly Guid Ta1Id = Guid.Parse("e4000000-0000-0000-0000-000000000001");
    public static readonly Guid Ta2Id = Guid.Parse("e4000000-0000-0000-0000-000000000002");
    public static readonly Guid Ta3Id = Guid.Parse("e4000000-0000-0000-0000-000000000003");
}
