using System.Globalization;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Services;

public interface IExportTemplate
{
    string TemplateName { get; }
    string Generate(IEnumerable<ExportDataContext> data, ExportOptions options);
}

public class ExportDataContext
{
    public string? SemesterName { get; set; }
    public ExperimentTeachingTask? Task { get; set; }
    public ExperimentItemSchedule? Schedule { get; set; }
}

public class ExportOptions
{
    public string? SemesterName { get; set; }
    public int TotalStudentCount { get; set; }
    public string? AcademicYear { get; set; }
}

/// <summary>
/// 实验课程教学任务一览表导出模板
/// </summary>
public class ExperimentTaskListTemplate : IExportTemplate
{
    public string TemplateName => "实验课程教学任务一览表";

    public string Generate(IEnumerable<ExportDataContext> data, ExportOptions options)
    {
        var groups = data
            .Where(d => d.Task != null)
            .GroupBy(d => d.SemesterName ?? "未分学期")
            .OrderBy(g => g.Key)
            .ToList();

        var totalTasks = groups.Sum(g => g.Count());
        var totalStudents = groups.Sum(g => g.Sum(d => d.Task!.StudentCount));

        using var ms = new MemoryStream();
        using (var doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
        {
            var mainPart = doc.AddMainDocumentPart();
            mainPart.Document = new Document();
            var body = mainPart.Document.AppendChild(new Body());

            AddStyles(mainPart);

            body.AppendChild(MakeParagraph("", spacing: 200));
            body.AppendChild(MakeTitle(TemplateName, 28, bold: true, center: true));
            body.AppendChild(MakeParagraph("", spacing: 120));

            body.AppendChild(MakeInfoLine($"学期：{options.SemesterName ?? "全部"}"));
            body.AppendChild(MakeInfoLine($"导出时间：{DateTime.Now:yyyy-MM-dd HH:mm}"));
            body.AppendChild(MakeParagraph("", spacing: 60));

            int globalIndex = 0;
            foreach (var group in groups)
            {
                if (groups.Count > 1)
                {
                    body.AppendChild(MakeParagraph($"学期：{group.Key}", spacing: 80, bold: true));
                }

                var headers = new[] { "序号", "课程名称", "班级", "人数", "层次", "实验学时", "实习学时", "实训学时", "授课教师", "实验教师", "教材/指导书" };
                var colWidths = new[] { 400, 1800, 1200, 500, 600, 700, 700, 700, 1000, 1000, 1500 };

                body.AppendChild(MakeTableHeader(headers, colWidths));

                int idx = 0;
                foreach (var item in group)
                {
                    var t = item.Task!;
                    var row = MakeTableRow(new[]
                    {
                        (++globalIndex).ToString(),
                        t.CourseName ?? "",
                        t.Class?.Name ?? "",
                        t.StudentCount.ToString(),
                        t.StudentLevel ?? "",
                        t.CurrentSemesterExperimentHours.ToString(),
                        t.CurrentSemesterPracticeHours.ToString(),
                        t.CurrentSemesterTrainingHours.ToString(),
                        t.TeacherNames ?? "",
                        t.TechnicalStaff ?? "",
                        MergeText(t.TextbookName, t.ExperimentGuideName)
                    }, colWidths, idx % 2 == 1);
                    body.AppendChild(row);
                    idx++;
                }

                body.AppendChild(MakeParagraph("", spacing: 80));
            }

            body.AppendChild(MakeParagraph("", spacing: 200));
            body.AppendChild(MakeSummaryLine($"共 {totalTasks} 条任务，合计学生 {totalStudents} 人"));
            body.AppendChild(MakeParagraph("", spacing: 200));
            body.AppendChild(MakeSignatures());

            mainPart.Document.Save();
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    private static string MergeText(string? a, string? b)
    {
        var parts = new[] { a, b }.Where(s => !string.IsNullOrWhiteSpace(s));
        return string.Join(" / ", parts);
    }

    private static void AddStyles(MainDocumentPart mainPart)
    {
        var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
        var styles = new Styles();

        var normalStyle = new Style
        {
            Type = StyleValues.Paragraph,
            StyleId = "Normal",
            Default = true
        };
        normalStyle.Append(new StyleName { Val = "Normal" });
        normalStyle.Append(new StyleParagraphProperties(new SpacingBetweenLines { After = "0" }));
        normalStyle.Append(new StyleRunProperties(
            new FontSize { Val = "24" },
            new RunFonts { Ascii = "宋体", EastAsia = "宋体" }
        ));
        styles.Append(normalStyle);
        stylesPart.Styles = styles;
    }

    private static Paragraph MakeTitle(string text, int fontSize, bool bold = false, bool center = false)
    {
        var pp = new StyleParagraphProperties(
            new Justification { Val = center ? JustificationValues.Center : JustificationValues.Left },
            new SpacingBetweenLines { Before = "100", After = "100", Line = "360", LineRule = LineSpacingRuleValues.Auto }
        );
        var rp = new StyleRunProperties(
            new FontSize { Val = fontSize.ToString() },
            new RunFonts { Ascii = "黑体", EastAsia = "黑体" },
            new Bold()
        );
        if (bold) rp.Append(new Bold());
        var p = new Paragraph(new ParagraphProperties(pp), new Run(rp, new Text(text)));
        return p;
    }

    private static Paragraph MakeInfoLine(string text)
    {
        var pp = new StyleParagraphProperties(
            new SpacingBetweenLines { After = "0" },
            new Justification { Val = JustificationValues.Left }
        );
        var rp = new StyleRunProperties(
            new FontSize { Val = "21" },
            new RunFonts { Ascii = "宋体", EastAsia = "宋体" }
        );
        return new Paragraph(new ParagraphProperties(pp), new Run(rp, new Text(text)));
    }

    private static Paragraph MakeSummaryLine(string text)
    {
        var pp = new StyleParagraphProperties(
            new SpacingBetweenLines { After = "0" }
        );
        var rp = new StyleRunProperties(
            new FontSize { Val = "21" },
            new RunFonts { Ascii = "宋体", EastAsia = "宋体" },
            new Bold()
        );
        return new Paragraph(new ParagraphProperties(pp), new Run(rp, new Text(text)));
    }

    private static Table MakeTableHeader(string[] headers, int[] widths)
    {
        var table = new Table();
        table.AppendChild(new TableProperties(
            new TableWidth { Width = "0", Type = TableWidthUnitValues.Auto },
            new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 6, Color = "000000" },
                new BottomBorder { Val = BorderValues.Single, Size = 6, Color = "000000" },
                new LeftBorder { Val = BorderValues.Single, Size = 6, Color = "000000" },
                new RightBorder { Val = BorderValues.Single, Size = 6, Color = "000000" },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4, Color = "000000" },
                new InsideVerticalBorder { Val = BorderValues.Single, Size = 4, Color = "000000" }
            ),
            new TableCellMarginDefault(
                new TopMargin { Width = "60", Type = TableWidthUnitValues.Dxa },
                new BottomMargin { Width = "60", Type = TableWidthUnitValues.Dxa },
                new TableCellLeftMargin { Width = 40, Type = TableWidthValues.Dxa },
                new TableCellRightMargin { Width = 40, Type = TableWidthValues.Dxa }
            )
        ));

        var headerRow = new TableRow();
        for (int i = 0; i < headers.Length; i++)
        {
            headerRow.AppendChild(MakeHeaderCell(headers[i], widths[i]));
        }
        table.AppendChild(headerRow);
        return table;
    }

    private static TableRow MakeTableRow(string[] cells, int[] widths, bool shaded)
    {
        var row = new TableRow();
        for (int i = 0; i < cells.Length; i++)
        {
            row.AppendChild(MakeDataCell(cells[i], widths[i], shaded));
        }
        return row;
    }

    private static TableCell MakeHeaderCell(string text, int width)
    {
        var cell = new TableCell();
        cell.AppendChild(new TableCellProperties(
            new TableCellWidth { Width = width.ToString(), Type = TableWidthUnitValues.Dxa },
            new Shading { Fill = "D9E2F3", Val = ShadingPatternValues.Clear },
            new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center },
            new TableCellMargin(
                new TopMargin { Width = "60", Type = TableWidthUnitValues.Dxa },
                new BottomMargin { Width = "60", Type = TableWidthUnitValues.Dxa }
            )
        ));

        var pp = new ParagraphProperties(
            new Justification { Val = JustificationValues.Center },
            new SpacingBetweenLines { After = "0" }
        );
        var rp = new StyleRunProperties(
            new FontSize { Val = "20" },
            new RunFonts { Ascii = "宋体", EastAsia = "宋体" },
            new Bold()
        );
        cell.AppendChild(new Paragraph(pp, new Run(rp, new Text(text))));
        return cell;
    }

    private static TableCell MakeDataCell(string text, int width, bool shaded)
    {
        var cell = new TableCell();
        var fill = shaded ? "F2F2F2" : "FFFFFF";
        cell.AppendChild(new TableCellProperties(
            new TableCellWidth { Width = width.ToString(), Type = TableWidthUnitValues.Dxa },
            new Shading { Fill = fill, Val = ShadingPatternValues.Clear },
            new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center },
            new TableCellMargin(
                new TopMargin { Width = "40", Type = TableWidthUnitValues.Dxa },
                new BottomMargin { Width = "40", Type = TableWidthUnitValues.Dxa }
            )
        ));

        var pp = new ParagraphProperties(
            new SpacingBetweenLines { After = "0" }
        );
        var rp = new StyleRunProperties(
            new FontSize { Val = "19" },
            new RunFonts { Ascii = "宋体", EastAsia = "宋体" }
        );
        cell.AppendChild(new Paragraph(pp, new Run(rp, new Text(text))));
        return cell;
    }

    private static Paragraph MakeParagraph(string text, int spacing = 0, bool bold = false, bool center = false)
    {
        var pp = new StyleParagraphProperties(
            new SpacingBetweenLines { After = spacing.ToString() }
        );
        if (center) pp.Append(new Justification { Val = JustificationValues.Center });
        var rp = new StyleRunProperties(new FontSize { Val = "21" });
        if (bold) rp.Append(new Bold());
        return new Paragraph(new ParagraphProperties(pp), new Run(rp, new Text(text)));
    }

    private static Paragraph MakeSignatures()
    {
        var pp = new StyleParagraphProperties(
            new SpacingBetweenLines { After = "0" }
        );
        var rp = new StyleRunProperties(new FontSize { Val = "21" });

        var line1 = new Paragraph(new ParagraphProperties(pp),
            new Run(rp, new Text("制表人：________________    审核人：________________    负责人：________________")));

        var line2 = new Paragraph(new ParagraphProperties(pp),
            new Run(rp, new Text($"日期：{DateTime.Now:yyyy} 年 {DateTime.Now.Month} 月 {DateTime.Now.Day} 日")));

        var line3 = new Paragraph(new ParagraphProperties(pp),
            new Run(rp, new Text("（此表由实验室管理员保存）")));

        return line1;
    }
}

/// <summary>
/// 实验教学授课计划表导出模板
/// </summary>
public class ExperimentSchedulePlanTemplate : IExportTemplate
{
    public string TemplateName => "实验教学授课计划表";

    public string Generate(IEnumerable<ExportDataContext> data, ExportOptions options)
    {
        var groups = data
            .Where(d => d.Task != null)
            .GroupBy(d => d.Task!.Id)
            .ToList();

        using var ms = new MemoryStream();
        using (var doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
        {
            var mainPart = doc.AddMainDocumentPart();
            mainPart.Document = new Document();
            var body = mainPart.Document.AppendChild(new Body());

            AddStyles(mainPart);

            for (int g = 0; g < groups.Count; g++)
            {
                var group = groups[g];
                var task = group.First().Task!;

                if (g > 0)
                {
                    body.AppendChild(MakePageBreak());
                }

                body.AppendChild(MakeParagraph("", spacing: 100));
                body.AppendChild(MakeTitle(TemplateName, 28, bold: true, center: true));
                body.AppendChild(MakeParagraph("", spacing: 80));

                body.AppendChild(MakeInfoRow(new[] { ("课程名称", task.CourseName ?? ""), ("班级", task.Class?.Name ?? "") }));
                body.AppendChild(MakeInfoRow(new[] { ("学生人数", $"{task.StudentCount} 人"), ("学生层次", task.StudentLevel ?? "") }));
                body.AppendChild(MakeInfoRow(new[] { ("授课教师", task.TeacherNames ?? ""), ("实验教师", task.TechnicalStaff ?? "") }));
                body.AppendChild(MakeInfoRow(new[] { ("教材", task.TextbookName ?? ""), ("实验指导书", task.ExperimentGuideName ?? "") }));
                body.AppendChild(MakeParagraph("", spacing: 40));

                var headers = new[] { "周次", "星期", "实验项目名称", "实验类型", "必做/选做", "每组人数", "组数", "循环次数", "实验地点" };
                var colWidths = new[] { 500, 600, 2400, 1000, 800, 700, 500, 700, 1400 };

                body.AppendChild(MakeTableHeader(headers, colWidths));

                var schedules = group
                    .Where(d => d.Schedule != null)
                    .OrderBy(d => d.Schedule!.WeekNumber)
                    .ThenBy(d => d.Schedule!.DayOfWeek)
                    .ToList();

                int idx = 0;
                foreach (var item in schedules)
                {
                    var s = item.Schedule!;
                    var rowData = new[]
                    {
                        s.WeekNumber?.ToString() ?? "",
                        FormatDayOfWeek(s.DayOfWeek),
                        s.ExperimentItem?.ExperimentName ?? "",
                        s.ExperimentItem?.ExperimentType ?? "",
                        s.ExperimentRequirement ?? "",
                        s.StudentsPerGroup?.ToString() ?? "",
                        s.ParallelGroups?.ToString() ?? "",
                        s.CycleCount?.ToString() ?? "",
                        FormatLocation(item)
                    };
                    body.AppendChild(MakeTableRow(rowData, colWidths, idx % 2 == 1));
                    idx++;
                }

                if (schedules.Count == 0)
                {
                    body.AppendChild(MakeEmptyRow(headers.Length, colWidths));
                }

                body.AppendChild(MakeParagraph("", spacing: 120));
                body.AppendChild(MakeNoteRow());
                body.AppendChild(MakeParagraph("", spacing: 120));
                body.AppendChild(MakeSignatures());
            }

            mainPart.Document.Save();
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    private static string FormatDayOfWeek(int? day)
    {
        if (!day.HasValue) return "";
        return day.Value switch
        {
            1 => "星期一",
            2 => "星期二",
            3 => "星期三",
            4 => "星期四",
            5 => "星期五",
            6 => "星期六",
            7 => "星期日",
            _ => day.Value.ToString()
        };
    }

    private static string FormatLocation(ExportDataContext item)
    {
        if (item.Schedule == null) return "";
        var parts = new[] {
            item.Schedule.Lab?.Building?.Name,
            item.Schedule.Lab?.Name,
            item.Schedule.Location
        }.Where(s => !string.IsNullOrWhiteSpace(s));
        return string.Join(" ", parts);
    }

    private static void AddStyles(MainDocumentPart mainPart)
    {
        var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
        var styles = new Styles();
        var normalStyle = new Style { Type = StyleValues.Paragraph, StyleId = "Normal", Default = true };
        normalStyle.Append(new StyleName { Val = "Normal" });
        normalStyle.Append(new StyleParagraphProperties(new SpacingBetweenLines { After = "0" }));
        normalStyle.Append(new StyleRunProperties(new FontSize { Val = "24" }, new RunFonts { Ascii = "宋体", EastAsia = "宋体" }));
        styles.Append(normalStyle);
        stylesPart.Styles = styles;
    }

    private static Paragraph MakeTitle(string text, int fontSize, bool bold = false, bool center = false)
    {
        var pp = new StyleParagraphProperties(
            new Justification { Val = center ? JustificationValues.Center : JustificationValues.Left },
            new SpacingBetweenLines { Before = "100", After = "100", Line = "360", LineRule = LineSpacingRuleValues.Auto }
        );
        var rp = new StyleRunProperties(new FontSize { Val = fontSize.ToString() }, new RunFonts { Ascii = "黑体", EastAsia = "黑体" });
        if (bold) rp.Append(new Bold());
        return new Paragraph(new ParagraphProperties(pp), new Run(rp, new Text(text)));
    }

    private static Table MakeInfoRow((string label, string value)[] fields)
    {
        var table = new Table();
        table.AppendChild(new TableProperties(
            new TableWidth { Width = "0", Type = TableWidthUnitValues.Auto },
            new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 6, Color = "000000" },
                new BottomBorder { Val = BorderValues.Single, Size = 6, Color = "000000" },
                new LeftBorder { Val = BorderValues.Single, Size = 6, Color = "000000" },
                new RightBorder { Val = BorderValues.Single, Size = 6, Color = "000000" },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4, Color = "000000" },
                new InsideVerticalBorder { Val = BorderValues.Single, Size = 4, Color = "000000" }
            )
        ));

        for (int i = 0; i < fields.Length; i += 2)
        {
            var row = new TableRow();
            for (int j = 0; j < 4; j++)
            {
                int fi = i + (j % 2 == 0 ? 0 : 1);
                if (fi >= fields.Length) fi = i;
                int span = (j == 0 || j == 1) ? 1200 : 4600;
                int halfSpan = j == 0 || j == 1 ? 1600 : 4600;

                if (j == 0)
                {
                    var labelCell = MakeInfoCell(fields[fi].label, halfSpan, true);
                    row.AppendChild(labelCell);
                }
                else if (j == 1)
                {
                    var valCell = MakeInfoCell(fields[fi].value, halfSpan, false);
                    row.AppendChild(valCell);
                }
                else if (j == 2 && i + 2 < fields.Length)
                {
                    var labelCell = MakeInfoCell(fields[i + 2].label, halfSpan, true);
                    row.AppendChild(labelCell);
                }
                else if (j == 3 && i + 2 < fields.Length)
                {
                    var valCell = MakeInfoCell(fields[i + 2].value, halfSpan, false);
                    row.AppendChild(valCell);
                }
            }
            table.AppendChild(row);
        }
        return table;
    }

    private static TableCell MakeInfoCell(string text, int width, bool isLabel)
    {
        var cell = new TableCell();
        cell.AppendChild(new TableCellProperties(
            new TableCellWidth { Width = width.ToString(), Type = TableWidthUnitValues.Dxa },
            isLabel ? new Shading { Fill = "D9E2F3", Val = ShadingPatternValues.Clear } : null!,
            new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
        ));

        var pp = new ParagraphProperties(new Justification { Val = JustificationValues.Center });
        var rp = new StyleRunProperties(
            new FontSize { Val = "20" },
            new RunFonts { Ascii = "宋体", EastAsia = "宋体" }
        );
        if (isLabel) rp.Append(new Bold());
        cell.AppendChild(new Paragraph(new ParagraphProperties(pp), new Run(rp, new Text(text))));
        return cell;
    }

    private static Table MakeTableHeader(string[] headers, int[] widths)
    {
        var table = new Table();
        table.AppendChild(new TableProperties(
            new TableWidth { Width = "0", Type = TableWidthUnitValues.Auto },
            new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 6, Color = "000000" },
                new BottomBorder { Val = BorderValues.Single, Size = 6, Color = "000000" },
                new LeftBorder { Val = BorderValues.Single, Size = 6, Color = "000000" },
                new RightBorder { Val = BorderValues.Single, Size = 6, Color = "000000" },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4, Color = "000000" },
                new InsideVerticalBorder { Val = BorderValues.Single, Size = 4, Color = "000000" }
            ),
            new TableCellMarginDefault(
                new TopMargin { Width = "60", Type = TableWidthUnitValues.Dxa },
                new BottomMargin { Width = "60", Type = TableWidthUnitValues.Dxa },
                new TableCellLeftMargin { Width = 40, Type = TableWidthValues.Dxa },
                new TableCellRightMargin { Width = 40, Type = TableWidthValues.Dxa }
            )
        ));

        var headerRow = new TableRow();
        for (int i = 0; i < headers.Length; i++)
        {
            headerRow.AppendChild(MakeHeaderCell(headers[i], widths[i]));
        }
        table.AppendChild(headerRow);
        return table;
    }

    private static TableRow MakeTableRow(string[] cells, int[] widths, bool shaded)
    {
        var row = new TableRow();
        for (int i = 0; i < cells.Length; i++)
        {
            row.AppendChild(MakeDataCell(cells[i], widths[i], shaded));
        }
        return row;
    }

    private static TableRow MakeEmptyRow(int colCount, int[] widths)
    {
        var row = new TableRow();
        for (int i = 0; i < colCount; i++)
        {
            row.AppendChild(MakeDataCell("", widths[i], false));
        }
        return row;
    }

    private static TableCell MakeHeaderCell(string text, int width)
    {
        var cell = new TableCell();
        cell.AppendChild(new TableCellProperties(
            new TableCellWidth { Width = width.ToString(), Type = TableWidthUnitValues.Dxa },
            new Shading { Fill = "D9E2F3", Val = ShadingPatternValues.Clear },
            new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center },
            new TableCellMargin(
                new TopMargin { Width = "60", Type = TableWidthUnitValues.Dxa },
                new BottomMargin { Width = "60", Type = TableWidthUnitValues.Dxa }
            )
        ));

        var pp = new ParagraphProperties(new Justification { Val = JustificationValues.Center }, new SpacingBetweenLines { After = "0" });
        var rp = new StyleRunProperties(new FontSize { Val = "20" }, new RunFonts { Ascii = "宋体", EastAsia = "宋体" }, new Bold());
        cell.AppendChild(new Paragraph(pp, new Run(rp, new Text(text))));
        return cell;
    }

    private static TableCell MakeDataCell(string text, int width, bool shaded)
    {
        var cell = new TableCell();
        cell.AppendChild(new TableCellProperties(
            new TableCellWidth { Width = width.ToString(), Type = TableWidthUnitValues.Dxa },
            new Shading { Fill = shaded ? "F2F2F2" : "FFFFFF", Val = ShadingPatternValues.Clear },
            new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center },
            new TableCellMargin(
                new TopMargin { Width = "40", Type = TableWidthUnitValues.Dxa },
                new BottomMargin { Width = "40", Type = TableWidthUnitValues.Dxa }
            )
        ));

        var pp = new ParagraphProperties(new SpacingBetweenLines { After = "0" });
        var rp = new StyleRunProperties(new FontSize { Val = "19" }, new RunFonts { Ascii = "宋体", EastAsia = "宋体" });
        cell.AppendChild(new Paragraph(pp, new Run(rp, new Text(text))));
        return cell;
    }

    private static Paragraph MakeParagraph(string text, int spacing = 0, bool bold = false, bool center = false)
    {
        var pp = new StyleParagraphProperties(new SpacingBetweenLines { After = spacing.ToString() });
        if (center) pp.Append(new Justification { Val = JustificationValues.Center });
        var rp = new StyleRunProperties(new FontSize { Val = "21" });
        if (bold) rp.Append(new Bold());
        return new Paragraph(new ParagraphProperties(pp), new Run(rp, new Text(text)));
    }

    private static Paragraph MakePageBreak()
    {
        return new Paragraph(new Run(new Break { Type = BreakValues.Page }));
    }

    private static Paragraph MakeNoteRow()
    {
        var pp = new StyleParagraphProperties(new SpacingBetweenLines { After = "0" });
        var rp = new StyleRunProperties(new FontSize { Val = "19" }, new RunFonts { Ascii = "宋体", EastAsia = "宋体" });
        return new Paragraph(pp,
            new Run(rp, new Text("备注：1. 实验类型填写\"基础型/综合型/设计型/研究型\"；2. 每组人数指每批次同时做实验的学生人数；3. 循环次数指同一实验内容重复次数。")));
    }

    private static Paragraph MakeSignatures()
    {
        var pp = new StyleParagraphProperties(new SpacingBetweenLines { After = "0" });
        var rp = new StyleRunProperties(new FontSize { Val = "21" });
        return new Paragraph(pp,
            new Run(rp, new Text("制表人：________________    审核人：________________    负责人：________________    日期：________________")));
    }
}

/// <summary>
/// 导出服务 - 统一调度各模板
/// </summary>
public class ExportService
{
    private readonly Dictionary<string, IExportTemplate> _templates;

    public ExportService()
    {
        _templates = new Dictionary<string, IExportTemplate>(StringComparer.OrdinalIgnoreCase)
        {
            ["task_list"] = new ExperimentTaskListTemplate(),
            ["schedule_plan"] = new ExperimentSchedulePlanTemplate()
        };
    }

    public IEnumerable<string> GetAvailableTemplates() => _templates.Keys;

    public string GetTemplateName(string templateKey) =>
        _templates.TryGetValue(templateKey, out var t) ? t.TemplateName : templateKey;

    public string Export(string templateKey, IEnumerable<ExportDataContext> data, ExportOptions options)
    {
        if (!_templates.TryGetValue(templateKey, out var template))
            throw new ArgumentException($"未知的导出模板：{templateKey}");

        return template.Generate(data, options);
    }
}
