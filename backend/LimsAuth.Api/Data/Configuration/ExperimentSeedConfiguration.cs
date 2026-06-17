using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Models;
using static LimsAuth.Api.Data.Configuration.SeedKeys;

namespace LimsAuth.Api.Data.Configuration;

/// <summary>
/// 实验/实训 实体 HasData 种子
/// </summary>
internal static class ExperimentSeedConfiguration
{
    public static void SeedExperiment(this ModelBuilder modelBuilder)
    {
        // =========================
        // 实验教学任务
        // =========================

        var TasksId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var LabCenterId = Guid.Parse("a0000000-0000-0000-0000-000000000005");

        modelBuilder.Entity<ExperimentTeachingTask>().HasData(
            new ExperimentTeachingTask
            {
                Id = TasksId,
                SemesterId = SemesterId,
                MajorId = MajorId,
                ClassId = ClassId,

                StudentCount = 30,
                StudentLevel = "本科",

                CourseName = "计算机网络实验",
                CourseType = "专业课",
                IsIndependentCourse = true,

                TotalExperimentHours = 32,
                CurrentSemesterExperimentHours = 16,

                TotalPracticeHours = 20,
                CurrentSemesterPracticeHours = 10,

                TotalTrainingHours = 10,
                CurrentSemesterTrainingHours = 5,

                InstitutionId = LabCenterId,
                DepartmentId = CsDeptId,

                TeacherIds = "teacher-001,teacher-002",
                TeacherNames = "张教授,李讲师",
                TeacherTitles = "教授,讲师",

                TechnicalStaff = "实验员A",
                TechnicalTitle = "工程师",

                TextbookName = "计算机网络实验指导书",
                ExperimentGuideName = "网络实验手册",

                Status = "Active",
                SortOrder = 1,

                Description = "网络实验课程教学任务",

                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            }
        );

        // =========================
        // 实验项目
        // =========================

        var ItemId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        modelBuilder.Entity<ExperimentItem>().HasData(
            new ExperimentItem
            {
                Id = ItemId,
                CourseCode = "NET-EXP-01",
                ExperimentName = "网络拓扑搭建实验",
                ExperimentHours = 4,
                ExperimentType = "基础实验",
                ExperimentRequirement = "必修",

                Status = "Active",
                SortOrder = 1,

                Description = "学习基本网络拓扑结构",

                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            }
        );

        // =========================
        // 实验安排
        // =========================

        var ScheduleId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

        modelBuilder.Entity<ExperimentItemSchedule>().HasData(
            new ExperimentItemSchedule
            {
                Id = ScheduleId,
                ExperimentTaskId = TasksId,
                ExperimentItemId = ItemId,
                LabId = Lab2Id,

                WeekNumber = 1,
                DayOfWeek = 2,
                PeriodNumber = 3,

                ParallelGroups = 2,
                StudentsPerGroup = 15,
                CycleCount = 1,

                ExperimentRequirement = "必做",
                Location = "实验楼A-101",

                IsConducted = false,

                Status = "Active",
                SortOrder = 1,

                Description = "第一周实验安排",

                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            }
        );

        // =========================
        // 教学质量评估
        // =========================

        var AssessmentId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

        modelBuilder.Entity<ExperimentQualityAssessment>().HasData(
            new ExperimentQualityAssessment
            {
                Id = AssessmentId,
                ExperimentTaskId = TasksId,

                InstitutionId = null,

                CourseName = "计算机网络实验",
                ExperimentHours = 16,
                IsIndependentCourse = true,

                MainTeacher = "张三",
                TeacherTitle = "教授",

                TechnicalStaff = "实验员A",
                TechnicalTitle = "工程师",

                ClassName = "计科1班",
                ClassStudentCount = 30,

                PlannedExperimentCount = 5,
                ActualExperimentCount = 5,

                MissedExperimentItems = "",

                AssessmentMethod = "报告+操作",
                AssessmentStudentCount = 30,
                AssessmentTime = "第18周",

                Status = "Active",
                SortOrder = 1,

                Description = "教学质量良好",

                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            }
        );


        // =========================
        // 更多实验教学任务
        // =========================

        var Task2Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb01");
        var Task3Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb02");
        var Task4Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb03");

        modelBuilder.Entity<ExperimentTeachingTask>().HasData(
            // 任务1已在上面定义 (TasksId)
            new ExperimentTeachingTask
            {
                Id = Task2Id,
                SemesterId = SemesterId,
                MajorId = MajorId,
                ClassId = ClassId,
                StudentCount = 30,
                StudentLevel = "本科",
                CourseName = "程序设计综合实验",
                CourseType = "必修课",
                IsIndependentCourse = true,
                TotalExperimentHours = 48,
                CurrentSemesterExperimentHours = 24,
                TotalPracticeHours = 16,
                CurrentSemesterPracticeHours = 8,
                TotalTrainingHours = 0,
                CurrentSemesterTrainingHours = 0,
                InstitutionId = LabCenterId,
                DepartmentId = CsDeptId,
                TeacherIds = TeacherUserId.ToString(),
                TeacherNames = "李老师",
                TeacherTitles = "讲师",
                TechnicalStaff = "实验员B",
                TechnicalTitle = "实验师",
                TextbookName = "C语言程序设计实验教程",
                ExperimentGuideName = "程序设计实验指导",
                Status = "Active",
                SortOrder = 2,
                Description = "C语言程序设计综合实验课程",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new ExperimentTeachingTask
            {
                Id = Task3Id,
                SemesterId = SemesterId,
                MajorId = MajorId,
                ClassId = ClassId,
                StudentCount = 30,
                StudentLevel = "本科",
                CourseName = "数据结构与算法实验",
                CourseType = "必修课",
                IsIndependentCourse = true,
                TotalExperimentHours = 32,
                CurrentSemesterExperimentHours = 16,
                TotalPracticeHours = 0,
                CurrentSemesterPracticeHours = 0,
                TotalTrainingHours = 0,
                CurrentSemesterTrainingHours = 0,
                InstitutionId = LabCenterId,
                DepartmentId = CsDeptId,
                TeacherIds = TeacherUserId.ToString(),
                TeacherNames = "王老师",
                TeacherTitles = "讲师",
                TechnicalStaff = "实验员C",
                TechnicalTitle = "高级实验师",
                TextbookName = "数据结构实验教程",
                ExperimentGuideName = "数据结构实验指导书",
                Status = "Active",
                SortOrder = 3,
                Description = "数据结构与算法分析实验",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new ExperimentTeachingTask
            {
                Id = Task4Id,
                SemesterId = SemesterId,
                MajorId = MajorId,
                ClassId = ClassId,
                StudentCount = 30,
                StudentLevel = "本科",
                CourseName = "操作系统实验",
                CourseType = "专业核心课",
                IsIndependentCourse = false,
                TotalExperimentHours = 24,
                CurrentSemesterExperimentHours = 12,
                TotalPracticeHours = 8,
                CurrentSemesterPracticeHours = 4,
                TotalTrainingHours = 0,
                CurrentSemesterTrainingHours = 0,
                InstitutionId = LabCenterId,
                DepartmentId = CsDeptId,
                TeacherIds = TeacherUserId.ToString(),
                TeacherNames = "赵老师",
                TeacherTitles = "副教授",
                TechnicalStaff = "实验员D",
                TechnicalTitle = "工程师",
                TextbookName = "操作系统实验教程",
                ExperimentGuideName = "Linux系统实验指导",
                Status = "Active",
                SortOrder = 4,
                Description = "操作系统原理与Linux实验",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            }
        );

        // =========================
        // 更多实验项目
        // =========================

        var Item2Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa01");
        var Item3Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa02");
        var Item4Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa03");
        var Item5Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa04");
        var Item6Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa05");
        var Item7Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa06");
        var Item8Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa07");
        var Item9Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa08");
        var Item10Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa09");

        modelBuilder.Entity<ExperimentItem>().HasData(
            // 项目1已在上面定义 (ItemId)
            new ExperimentItem
            {
                Id = Item2Id,
                CourseCode = "NET-EXP-02",
                ExperimentName = "交换机配置实验",
                ExperimentHours = 4,
                ExperimentType = "基础实验",
                ExperimentRequirement = "必修",
                Status = "Active",
                SortOrder = 2,
                Description = "学习交换机VLAN配置",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new ExperimentItem
            {
                Id = Item3Id,
                CourseCode = "NET-EXP-03",
                ExperimentName = "路由器配置实验",
                ExperimentHours = 4,
                ExperimentType = "基础实验",
                ExperimentRequirement = "必修",
                Status = "Active",
                SortOrder = 3,
                Description = "学习路由器静态路由和动态路由配置",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new ExperimentItem
            {
                Id = Item4Id,
                CourseCode = "NET-EXP-04",
                ExperimentName = "网络协议分析实验",
                ExperimentHours = 4,
                ExperimentType = "综合实验",
                ExperimentRequirement = "必修",
                Status = "Active",
                SortOrder = 4,
                Description = "使用Wireshark分析TCP/IP协议",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new ExperimentItem
            {
                Id = Item5Id,
                CourseCode = "NET-EXP-05",
                ExperimentName = "网络安全基础实验",
                ExperimentHours = 4,
                ExperimentType = "综合实验",
                ExperimentRequirement = "选修",
                Status = "Active",
                SortOrder = 5,
                Description = "防火墙配置与入侵检测基础",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            // 程序设计实验项目
            new ExperimentItem
            {
                Id = Item6Id,
                CourseCode = "C-PROG-01",
                ExperimentName = "顺序结构程序设计",
                ExperimentHours = 2,
                ExperimentType = "验证性实验",
                ExperimentRequirement = "必修",
                Status = "Active",
                SortOrder = 1,
                Description = "基本输入输出和算术运算",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new ExperimentItem
            {
                Id = Item7Id,
                CourseCode = "C-PROG-02",
                ExperimentName = "选择结构程序设计",
                ExperimentHours = 2,
                ExperimentType = "验证性实验",
                ExperimentRequirement = "必修",
                Status = "Active",
                SortOrder = 2,
                Description = "if-else和switch语句练习",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new ExperimentItem
            {
                Id = Item8Id,
                CourseCode = "C-PROG-03",
                ExperimentName = "循环结构程序设计",
                ExperimentHours = 4,
                ExperimentType = "设计性实验",
                ExperimentRequirement = "必修",
                Status = "Active",
                SortOrder = 3,
                Description = "for、while、do-while循环应用",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new ExperimentItem
            {
                Id = Item9Id,
                CourseCode = "C-PROG-04",
                ExperimentName = "函数与模块化设计",
                ExperimentHours = 4,
                ExperimentType = "设计性实验",
                ExperimentRequirement = "必修",
                Status = "Active",
                SortOrder = 4,
                Description = "函数的定义、调用与参数传递",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new ExperimentItem
            {
                Id = Item10Id,
                CourseCode = "C-PROG-05",
                ExperimentName = "综合设计项目",
                ExperimentHours = 8,
                ExperimentType = "综合性实验",
                ExperimentRequirement = "必修",
                Status = "Active",
                SortOrder = 5,
                Description = "学生成绩管理系统设计",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            }
        );

        // =========================
        // 更多实验安排
        // =========================

        var Schedule2Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc01");
        var Schedule3Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc02");
        var Schedule4Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc03");
        var Schedule5Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc04");
        var Schedule6Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc05");
        var Schedule7Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc06");
        var Schedule8Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc07");

        modelBuilder.Entity<ExperimentItemSchedule>().HasData(
            // 安排1已在上面定义 (ScheduleId)
            new ExperimentItemSchedule
            {
                Id = Schedule2Id,
                ExperimentTaskId = TasksId,
                ExperimentItemId = Item2Id,
                LabId = Lab2Id,
                WeekNumber = 3,
                DayOfWeek = 2,
                PeriodNumber = 3,
                ParallelGroups = 2,
                StudentsPerGroup = 15,
                CycleCount = 1,
                ExperimentRequirement = "必做",
                Location = "实验楼A-102",
                IsConducted = false,
                Status = "Active",
                SortOrder = 2,
                Description = "交换机VLAN配置实验",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new ExperimentItemSchedule
            {
                Id = Schedule3Id,
                ExperimentTaskId = TasksId,
                ExperimentItemId = Item3Id,
                LabId = Lab2Id,
                WeekNumber = 5,
                DayOfWeek = 4,
                PeriodNumber = 4,
                ParallelGroups = 2,
                StudentsPerGroup = 15,
                CycleCount = 1,
                ExperimentRequirement = "必做",
                Location = "实验楼A-102",
                IsConducted = false,
                Status = "Active",
                SortOrder = 3,
                Description = "路由器配置实验",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new ExperimentItemSchedule
            {
                Id = Schedule4Id,
                ExperimentTaskId = TasksId,
                ExperimentItemId = Item4Id,
                LabId = Lab1Id,
                WeekNumber = 7,
                DayOfWeek = 2,
                PeriodNumber = 1,
                ParallelGroups = 2,
                StudentsPerGroup = 15,
                CycleCount = 1,
                ExperimentRequirement = "必做",
                Location = "实验楼A-101",
                IsConducted = false,
                Status = "Active",
                SortOrder = 4,
                Description = "网络协议分析实验",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new ExperimentItemSchedule
            {
                Id = Schedule5Id,
                ExperimentTaskId = TasksId,
                ExperimentItemId = Item5Id,
                LabId = Lab2Id,
                WeekNumber = 12,
                DayOfWeek = 2,
                PeriodNumber = 3,
                ParallelGroups = 2,
                StudentsPerGroup = 15,
                CycleCount = 1,
                ExperimentRequirement = "选做",
                Location = "实验楼A-102",
                IsConducted = false,
                Status = "Active",
                SortOrder = 5,
                Description = "网络安全基础实验",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            // 程序设计实验安排
            new ExperimentItemSchedule
            {
                Id = Schedule6Id,
                ExperimentTaskId = Task2Id,
                ExperimentItemId = Item6Id,
                LabId = Lab1Id,
                WeekNumber = 1,
                DayOfWeek = 3,
                PeriodNumber = 1,
                ParallelGroups = 3,
                StudentsPerGroup = 10,
                CycleCount = 1,
                ExperimentRequirement = "必做",
                Location = "实验楼A-101",
                IsConducted = false,
                Status = "Active",
                SortOrder = 1,
                Description = "顺序结构程序设计",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new ExperimentItemSchedule
            {
                Id = Schedule7Id,
                ExperimentTaskId = Task2Id,
                ExperimentItemId = Item7Id,
                LabId = Lab1Id,
                WeekNumber = 2,
                DayOfWeek = 3,
                PeriodNumber = 1,
                ParallelGroups = 3,
                StudentsPerGroup = 10,
                CycleCount = 1,
                ExperimentRequirement = "必做",
                Location = "实验楼A-101",
                IsConducted = false,
                Status = "Active",
                SortOrder = 2,
                Description = "选择结构程序设计",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new ExperimentItemSchedule
            {
                Id = Schedule8Id,
                ExperimentTaskId = Task2Id,
                ExperimentItemId = Item8Id,
                LabId = Lab1Id,
                WeekNumber = 4,
                DayOfWeek = 5,
                PeriodNumber = 2,
                ParallelGroups = 3,
                StudentsPerGroup = 10,
                CycleCount = 1,
                ExperimentRequirement = "必做",
                Location = "实验楼A-101",
                IsConducted = false,
                Status = "Active",
                SortOrder = 3,
                Description = "循环结构程序设计",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            }
        );

        // =========================
        // 更多教学质量评估
        // =========================

        var Assessment2Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddd0001");
        var Assessment3Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddd0002");

        modelBuilder.Entity<ExperimentQualityAssessment>().HasData(
            new ExperimentQualityAssessment
            {
                Id = Assessment2Id,
                ExperimentTaskId = Task2Id,
                InstitutionId = LabCenterId,
                CourseName = "程序设计综合实验",
                ExperimentHours = 24,
                IsIndependentCourse = true,
                MainTeacher = "李老师",
                TeacherTitle = "讲师",
                TechnicalStaff = "实验员B",
                TechnicalTitle = "实验师",
                ClassName = "计算机2024级1班",
                ClassStudentCount = 30,
                PlannedExperimentCount = 8,
                ActualExperimentCount = 8,
                MissedExperimentItems = "",
                AssessmentMethod = "实验报告+现场操作",
                AssessmentStudentCount = 30,
                AssessmentTime = "第17周",
                Status = "Active",
                SortOrder = 2,
                Description = "程序设计实验教学评估良好",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new ExperimentQualityAssessment
            {
                Id = Assessment3Id,
                ExperimentTaskId = Task3Id,
                InstitutionId = LabCenterId,
                CourseName = "数据结构与算法实验",
                ExperimentHours = 16,
                IsIndependentCourse = true,
                MainTeacher = "王老师",
                TeacherTitle = "副教授",
                TechnicalStaff = "实验员C",
                TechnicalTitle = "高级实验师",
                ClassName = "计算机2024级1班",
                ClassStudentCount = 30,
                PlannedExperimentCount = 6,
                ActualExperimentCount = 6,
                MissedExperimentItems = "",
                AssessmentMethod = "实验报告+代码评审",
                AssessmentStudentCount = 30,
                AssessmentTime = "第16周",
                Status = "Active",
                SortOrder = 3,
                Description = "数据结构实验教学评估优秀",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            }
        );

        // =========================
        // 实训教学计划
        // =========================

        var Plan1Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeee0001");
        var Plan2Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeee0002");
        var Plan3Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeee0003");

        modelBuilder.Entity<TrainingTeachingPlan>().HasData(
            new TrainingTeachingPlan
            {
                Id = Plan1Id,
                SemesterId = SemesterId,
                CourseId = CourseId,
                CourseName = "工程制图实训",
                CourseCode = "ENG-DRW-001",
                MajorId = MajorId,
                ClassId = ClassId,
                StudentCount = 30,
                StudentLevel = "本科",
                TeachingOrganizationMethod = "校内集中",
                TeachingLocation = "工程实训楼A座1层A101",
                TeachingPurpose = "培养学生工程制图能力、空间想象能力和严谨的工作作风",
                TeachingRequirements = "掌握AutoCAD软件操作、能够独立完成工程图纸的绘制",
                TeachingContent = "机械制图基础、三维建模、工程图纸绘制、标准件与常用件表达",
                TeachingProgressSchedule = "第1-2周：制图基本知识；第3-4周：AutoCAD基础；第5-8周：零件图绘制；第9-12周：装配图绘制；第13-16周：三维建模",
                TrainingMethod = "项目驱动、分组教学",
                CycleGroupInfo = "每班分为3组，每组10人，循环实训",
                AssessmentMethod = "作品评价+答辩",
                AssessmentRequirements = "提交完整的工程图纸集，含零件图3张、装配图1张",
                QualityAssuranceMeasures = "过程考核+成果验收+答辩评分",
                QualityAssuranceDetails = "平时表现占20%，作品完成度占50%，答辩表现占30%",
                ExperimentCenterOpinion = "同意开课",
                ExperimentCenterOpinionStatus = "Approved",
                ExperimentCenterApprovedBy = "管理员",
                ExperimentCenterApprovalDate = SeedDate,
                DepartmentOpinion = "符合培养方案要求",
                DepartmentOpinionStatus = "Approved",
                DepartmentApprovedBy = "系主任",
                DepartmentApprovalDate = SeedDate,
                Status = "Active",
                SortOrder = 1,
                Description = "工程制图实训教学计划",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new TrainingTeachingPlan
            {
                Id = Plan2Id,
                SemesterId = SemesterId,
                CourseId = CourseId,
                CourseName = "大学物理实验",
                CourseCode = "PHY-EXP-001",
                MajorId = MajorId,
                ClassId = ClassId,
                StudentCount = 30,
                StudentLevel = "本科",
                TeachingOrganizationMethod = "校内集中",
                TeachingLocation = "理学实验楼B座1层B101",
                TeachingPurpose = "加深对物理原理的理解，培养实验操作技能和数据处理能力",
                TeachingRequirements = "掌握基本物理量的测量方法、正确使用仪器、能够分析实验误差",
                TeachingContent = "力学实验、热学实验、电磁学实验、光学实验",
                TeachingProgressSchedule = "第1-3周：力学实验；第4-6周：热学实验；第7-10周：电磁学实验；第11-13周：光学实验；第14-15周：综合实验",
                TrainingMethod = "理实一体化教学",
                CycleGroupInfo = "每班分为3组，每组10人，循环进行各模块实验",
                AssessmentMethod = "实验操作考核+实验报告",
                AssessmentRequirements = "完成全部必做实验，提交规范实验报告",
                QualityAssuranceMeasures = "实验过程监控+报告评阅+操作考核",
                QualityAssuranceDetails = "实验操作占40%，实验报告占40%，考勤占20%",
                ExperimentCenterOpinion = "教学质量良好",
                ExperimentCenterOpinionStatus = "Approved",
                ExperimentCenterApprovedBy = "管理员",
                ExperimentCenterApprovalDate = SeedDate,
                DepartmentOpinion = "课程设置合理",
                DepartmentOpinionStatus = "Approved",
                DepartmentApprovedBy = "系主任",
                DepartmentApprovalDate = SeedDate,
                Status = "Active",
                SortOrder = 2,
                Description = "大学物理实验教学计划",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new TrainingTeachingPlan
            {
                Id = Plan3Id,
                SemesterId = SemesterId,
                CourseId = CourseId,
                CourseName = "创新创业实践",
                CourseCode = "INN-ENT-001",
                MajorId = MajorId,
                ClassId = ClassId,
                StudentCount = 30,
                StudentLevel = "本科",
                TeachingOrganizationMethod = "校内分散",
                TeachingLocation = "创新创业中心C座1层创客空间",
                TeachingPurpose = "培养学生的创新思维、团队协作能力和创业实践能力",
                TeachingRequirements = "完成创新项目策划与实施，能够进行项目路演和答辩",
                TeachingContent = "创新方法论、创业基础、项目实践、商业计划书撰写",
                TeachingProgressSchedule = "第1-2周：创新思维训练；第3-4周：创业基础理论；第5-8周：项目分组实践；第9-12周：中期检查与指导；第13-15周：路演准备；第16周：项目路演与答辩",
                TrainingMethod = "案例分析+项目实战+路演展示",
                CycleGroupInfo = "学生自由组队，每组4-5人，共6组",
                AssessmentMethod = "项目成果+路演评分",
                AssessmentRequirements = "提交完整商业计划书、进行项目路演答辩",
                QualityAssuranceMeasures = "企业导师参与+阶段性评审",
                QualityAssuranceDetails = "项目创新性占30%，团队协作占20%，路演表现占30%，商业可行性占20%",
                ExperimentCenterOpinion = "教学模式创新",
                ExperimentCenterOpinionStatus = "Approved",
                ExperimentCenterApprovedBy = "管理员",
                ExperimentCenterApprovalDate = SeedDate,
                DepartmentOpinion = "有助于学生全面发展",
                DepartmentOpinionStatus = "Approved",
                DepartmentApprovedBy = "系主任",
                DepartmentApprovalDate = SeedDate,
                Status = "Active",
                SortOrder = 3,
                Description = "创新创业实训教学计划",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            }
        );

    }
}