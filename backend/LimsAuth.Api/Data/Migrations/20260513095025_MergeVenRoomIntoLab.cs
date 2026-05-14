using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LimsAuth.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class MergeVenRoomIntoLab : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: labs 表 - 添加新列
            migrationBuilder.AddColumn<int>(
                name: "seat_count",
                table: "labs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "floor_no",
                table: "labs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "area",
                table: "labs",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "room_type",
                table: "labs",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "photo",
                table: "labs",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_available",
                table: "labs",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "experiment_location_code",
                table: "labs",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            // Step 2: schedule_entries 表 - 重建表（SQLite 不支持 DROP COLUMN）
            // 创建临时表，不包含 building_name 和 room_number
            migrationBuilder.Sql(@"
                CREATE TABLE schedule_entries_new (
                    id TEXT NOT NULL PRIMARY KEY,
                    semester_id TEXT NOT NULL,
                    lab_id TEXT NULL,
                    week_number INTEGER NOT NULL,
                    day_of_week INTEGER NOT NULL,
                    period_number INTEGER NOT NULL,
                    source TEXT NULL,
                    status TEXT NULL,
                    reservation_id TEXT NULL,
                    teaching_application_id TEXT NULL,
                    experiment_task_id TEXT NULL,
                    teaching_task_id TEXT NULL,
                    course_name TEXT NULL,
                    project_name TEXT NULL,
                    course_id TEXT NULL,
                    teacher_id TEXT NULL,
                    teacher_name TEXT NULL,
                    class_id TEXT NULL,
                    class_name TEXT NULL,
                    major_id TEXT NULL,
                    major_name TEXT NULL,
                    student_count INTEGER NULL,
                    remark TEXT NULL,
                    has_conflict INTEGER NOT NULL,
                    conflict_info TEXT NULL,
                    created_at TEXT NOT NULL,
                    created_by TEXT NULL,
                    updated_at TEXT NOT NULL,
                    updated_by TEXT NULL
                );
            ");

            migrationBuilder.Sql(@"
                INSERT INTO schedule_entries_new
                (id, semester_id, lab_id, week_number, day_of_week, period_number,
                 source, status, reservation_id, teaching_application_id, experiment_task_id,
                 teaching_task_id, course_name, project_name, course_id, teacher_id,
                 teacher_name, class_id, class_name, major_id, major_name, student_count,
                 remark, has_conflict, conflict_info, created_at, created_by, updated_at, updated_by)
                SELECT
                id, semester_id, lab_id, week_number, day_of_week, period_number,
                 source, status, reservation_id, teaching_application_id, experiment_task_id,
                 teaching_task_id, course_name, project_name, course_id, teacher_id,
                 teacher_name, class_id, class_name, major_id, major_name, student_count,
                 remark, has_conflict, conflict_info, created_at, created_by, updated_at, updated_by
                FROM schedule_entries;
            ");

            migrationBuilder.Sql("DROP TABLE schedule_entries;");
            migrationBuilder.Sql("ALTER TABLE schedule_entries_new RENAME TO schedule_entries;");

            // Step 3: usage_registrations 表 - 重建表（同样移除 building_name 和 room_number）
            migrationBuilder.Sql(@"
                CREATE TABLE usage_registrations_new (
                    id TEXT NOT NULL PRIMARY KEY,
                    semester_id TEXT NOT NULL,
                    lab_id TEXT NULL,
                    lab_name TEXT NOT NULL,
                    use_date TEXT NOT NULL,
                    week_number INTEGER NOT NULL,
                    day_of_week INTEGER NOT NULL,
                    period_number INTEGER NOT NULL,
                    source TEXT NULL,
                    schedule_entry_id TEXT NULL,
                    reservation_id TEXT NULL,
                    teaching_application_id TEXT NULL,
                    experiment_task_id TEXT NULL,
                    teaching_task_id TEXT NULL,
                    course_name TEXT NULL,
                    project_name TEXT NULL,
                    experiment_item_name TEXT NULL,
                    experiment_item_type TEXT NULL,
                    planned_hours INTEGER NOT NULL,
                    actual_hours REAL NOT NULL,
                    class_name TEXT NULL,
                    expected_student_count INTEGER NULL,
                    actual_student_count INTEGER NULL,
                    attendance_record TEXT NULL,
                    teaching_condition TEXT NULL,
                    equipment_condition TEXT NULL,
                    status TEXT NOT NULL,
                    reminded_at TEXT NULL,
                    filled_by_id TEXT NOT NULL,
                    filled_by_name TEXT NOT NULL,
                    filled_at TEXT NOT NULL,
                    created_at TEXT NOT NULL,
                    created_by TEXT NULL,
                    updated_at TEXT NOT NULL,
                    updated_by TEXT NULL
                );
            ");

            migrationBuilder.Sql(@"
                INSERT INTO usage_registrations_new
                (id, semester_id, lab_id, lab_name, use_date, week_number, day_of_week,
                 period_number, source, schedule_entry_id, reservation_id, teaching_application_id,
                 experiment_task_id, teaching_task_id, course_name, project_name,
                 experiment_item_name, experiment_item_type, planned_hours, actual_hours,
                 class_name, expected_student_count, actual_student_count,
                 attendance_record, teaching_condition, equipment_condition, status,
                 reminded_at, filled_by_id, filled_by_name, filled_at,
                 created_at, created_by, updated_at, updated_by)
                SELECT
                id, semester_id, lab_id, lab_name, use_date, week_number, day_of_week,
                 period_number, source, schedule_entry_id, reservation_id, teaching_application_id,
                 experiment_task_id, teaching_task_id, course_name, project_name,
                 experiment_item_name, experiment_item_type, planned_hours, actual_hours,
                 class_name, expected_student_count, actual_student_count,
                 attendance_record, teaching_condition, equipment_condition, status,
                 reminded_at, filled_by_id, filled_by_name, filled_at,
                 created_at, created_by, updated_at, updated_by
                FROM usage_registrations;
            ");

            migrationBuilder.Sql("DROP TABLE usage_registrations;");
            migrationBuilder.Sql("ALTER TABLE usage_registrations_new RENAME TO usage_registrations;");

            // Step 4: 删除 ven_buildings 和 ven_rooms 表
            migrationBuilder.Sql("DROP TABLE IF EXISTS ven_buildings;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS ven_rooms;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 回滚：删除 labs 表新增的列
            migrationBuilder.DropColumn(name: "seat_count", table: "labs");
            migrationBuilder.DropColumn(name: "floor_no", table: "labs");
            migrationBuilder.DropColumn(name: "area", table: "labs");
            migrationBuilder.DropColumn(name: "room_type", table: "labs");
            migrationBuilder.DropColumn(name: "photo", table: "labs");
            migrationBuilder.DropColumn(name: "is_available", table: "labs");
            migrationBuilder.DropColumn(name: "experiment_location_code", table: "labs");

            // 回滚：schedule_entries 恢复 building_name 和 room_number
            migrationBuilder.Sql(@"
                CREATE TABLE schedule_entries_old (
                    id TEXT NOT NULL PRIMARY KEY,
                    semester_id TEXT NOT NULL,
                    lab_id TEXT NULL,
                    week_number INTEGER NOT NULL,
                    day_of_week INTEGER NOT NULL,
                    period_number INTEGER NOT NULL,
                    source TEXT NULL,
                    status TEXT NULL,
                    reservation_id TEXT NULL,
                    teaching_application_id TEXT NULL,
                    experiment_task_id TEXT NULL,
                    teaching_task_id TEXT NULL,
                    course_name TEXT NULL,
                    project_name TEXT NULL,
                    course_id TEXT NULL,
                    teacher_id TEXT NULL,
                    teacher_name TEXT NULL,
                    class_id TEXT NULL,
                    class_name TEXT NULL,
                    major_id TEXT NULL,
                    major_name TEXT NULL,
                    student_count INTEGER NULL,
                    building_name TEXT NULL,
                    room_number TEXT NULL,
                    remark TEXT NULL,
                    has_conflict INTEGER NOT NULL,
                    conflict_info TEXT NULL,
                    created_at TEXT NOT NULL,
                    created_by TEXT NULL,
                    updated_at TEXT NOT NULL,
                    updated_by TEXT NULL
                );
            ");

            migrationBuilder.Sql(@"
                INSERT INTO schedule_entries_old
                (id, semester_id, lab_id, week_number, day_of_week, period_number,
                 source, status, reservation_id, teaching_application_id, experiment_task_id,
                 teaching_task_id, course_name, project_name, course_id, teacher_id,
                 teacher_name, class_id, class_name, major_id, major_name, student_count,
                 remark, has_conflict, conflict_info, created_at, created_by, updated_at, updated_by)
                SELECT
                id, semester_id, lab_id, week_number, day_of_week, period_number,
                 source, status, reservation_id, teaching_application_id, experiment_task_id,
                 teaching_task_id, course_name, project_name, course_id, teacher_id,
                 teacher_name, class_id, class_name, major_id, major_name, student_count,
                 remark, has_conflict, conflict_info, created_at, created_by, updated_at, updated_by
                FROM schedule_entries;
            ");

            migrationBuilder.Sql("DROP TABLE schedule_entries;");
            migrationBuilder.Sql("ALTER TABLE schedule_entries_old RENAME TO schedule_entries;");

            // 回滚：usage_registrations 恢复 building_name 和 room_number
            migrationBuilder.Sql(@"
                CREATE TABLE usage_registrations_old (
                    id TEXT NOT NULL PRIMARY KEY,
                    semester_id TEXT NOT NULL,
                    lab_id TEXT NULL,
                    lab_name TEXT NOT NULL,
                    building_name TEXT NULL,
                    room_number TEXT NULL,
                    use_date TEXT NOT NULL,
                    week_number INTEGER NOT NULL,
                    day_of_week INTEGER NOT NULL,
                    period_number INTEGER NOT NULL,
                    source TEXT NULL,
                    schedule_entry_id TEXT NULL,
                    reservation_id TEXT NULL,
                    teaching_application_id TEXT NULL,
                    experiment_task_id TEXT NULL,
                    teaching_task_id TEXT NULL,
                    course_name TEXT NULL,
                    project_name TEXT NULL,
                    experiment_item_name TEXT NULL,
                    experiment_item_type TEXT NULL,
                    planned_hours INTEGER NOT NULL,
                    actual_hours REAL NOT NULL,
                    class_name TEXT NULL,
                    expected_student_count INTEGER NULL,
                    actual_student_count INTEGER NULL,
                    attendance_record TEXT NULL,
                    teaching_condition TEXT NULL,
                    equipment_condition TEXT NULL,
                    status TEXT NOT NULL,
                    reminded_at TEXT NULL,
                    filled_by_id TEXT NOT NULL,
                    filled_by_name TEXT NOT NULL,
                    filled_at TEXT NOT NULL,
                    created_at TEXT NOT NULL,
                    created_by TEXT NULL,
                    updated_at TEXT NOT NULL,
                    updated_by TEXT NULL
                );
            ");

            migrationBuilder.Sql(@"
                INSERT INTO usage_registrations_old
                (id, semester_id, lab_id, lab_name, use_date, week_number, day_of_week,
                 period_number, source, schedule_entry_id, reservation_id, teaching_application_id,
                 experiment_task_id, teaching_task_id, course_name, project_name,
                 experiment_item_name, experiment_item_type, planned_hours, actual_hours,
                 class_name, expected_student_count, actual_student_count,
                 attendance_record, teaching_condition, equipment_condition, status,
                 reminded_at, filled_by_id, filled_by_name, filled_at,
                 created_at, created_by, updated_at, updated_by)
                SELECT
                id, semester_id, lab_id, lab_name, use_date, week_number, day_of_week,
                 period_number, source, schedule_entry_id, reservation_id, teaching_application_id,
                 experiment_task_id, teaching_task_id, course_name, project_name,
                 experiment_item_name, experiment_item_type, planned_hours, actual_hours,
                 class_name, expected_student_count, actual_student_count,
                 attendance_record, teaching_condition, equipment_condition, status,
                 reminded_at, filled_by_id, filled_by_name, filled_at,
                 created_at, created_by, updated_at, updated_by
                FROM usage_registrations;
            ");

            migrationBuilder.Sql("DROP TABLE usage_registrations;");
            migrationBuilder.Sql("ALTER TABLE usage_registrations_old RENAME TO usage_registrations;");

            // 回滚：重建 VenRoom 和 VenBuilding 表
            migrationBuilder.CreateTable(
                name: "ven_buildings",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    code = table.Column<string>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    english_name = table.Column<string>(type: "TEXT", nullable: true),
                    address = table.Column<string>(type: "TEXT", nullable: true),
                    total_floors = table.Column<int>(type: "INTEGER", nullable: false),
                    area = table.Column<double>(type: "REAL", nullable: false),
                    build_year = table.Column<int>(type: "INTEGER", nullable: true),
                    use_type = table.Column<string>(type: "TEXT", nullable: true),
                    sort_order = table.Column<int>(type: "INTEGER", nullable: false),
                    status = table.Column<string>(type: "TEXT", nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<string>(type: "TEXT", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    updated_by = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table => table.PrimaryKey("PK_ven_buildings", x => x.id));

            migrationBuilder.CreateTable(
                name: "ven_rooms",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    code = table.Column<string>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    building_id = table.Column<string>(type: "TEXT", nullable: true),
                    floor_no = table.Column<int>(type: "INTEGER", nullable: true),
                    room_number = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    seat_count = table.Column<int>(type: "INTEGER", nullable: false),
                    area = table.Column<double>(type: "REAL", nullable: false),
                    room_type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    photo = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    is_available = table.Column<bool>(type: "INTEGER", nullable: false),
                    sort_order = table.Column<int>(type: "INTEGER", nullable: false),
                    status = table.Column<string>(type: "TEXT", nullable: false),
                    experiment_location_code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<string>(type: "TEXT", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    updated_by = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ven_rooms", x => x.id);
                    table.ForeignKey(
                        name: "FK_ven_rooms_ven_buildings_building_id",
                        column: x => x.building_id,
                        principalTable: "ven_buildings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });
        }
    }
}
