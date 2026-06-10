import 'dart:io';
import 'package:file_picker/file_picker.dart';
import 'package:excel/excel.dart';
import 'package:path_provider/path_provider.dart';
import '../models/models.dart';

class ExcelUtils {
  /// 选择 Excel 文件
  static Future<File?> pickExcelFile() async {
    final result = await FilePicker.platform.pickFiles(
      type: FileType.custom,
      allowedExtensions: ['xlsx', 'xls'],
    );
    if (result != null && result.files.single.path != null) {
      return File(result.files.single.path!);
    }
    return null;
  }

  /// 解析设备导入 Excel
  static Future<List<Map<String, dynamic>>> parseEquipmentExcel(File file) async {
    final bytes = await file.readAsBytes();
    final excel = Excel.decodeBytes(bytes);
    final results = <Map<String, dynamic>>[];

    for (final table in excel.tables.keys) {
      final sheet = excel.tables[table];
      if (sheet == null) continue;

      // 跳过表头
      for (var i = 1; i < sheet.maxRows; i++) {
        final row = sheet.row(i);
        if (row.isEmpty) continue;

        results.add({
          'code': _cellValue(row[0]),
          'name': _cellValue(row[1]),
          'model': _cellValue(row[2]),
          'manufacturer': _cellValue(row[3]),
          'category': _cellValue(row[4]),
          'labId': _cellValue(row[5]),
          'status': _cellValue(row[6]) ?? 'Normal',
        });
      }
    }
    return results;
  }

  /// 下载设备导入模板
  static Future<File> generateEquipmentTemplate() async {
    final excel = Excel.createExcel();
    final sheet = excel['设备导入模板'];

    // 表头
    sheet.appendRow([
      TextCellValue('设备编号 *'),
      TextCellValue('设备名称 *'),
      TextCellValue('型号'),
      TextCellValue('厂商'),
      TextCellValue('类别'),
      TextCellValue('实验室ID'),
      TextCellValue('状态'),
    ]);

    // 示例行
    sheet.appendRow([
      TextCellValue('EQ001'),
      TextCellValue('示例设备'),
      TextCellValue('Model-X'),
      TextCellValue('厂商名'),
      TextCellValue('教学设备'),
      TextCellValue(''),
      TextCellValue('Normal'),
    ]);

    final dir = await getApplicationDocumentsDirectory();
    final file = File('${dir.path}/equipment_template.xlsx');
    final bytes = excel.encode();
    if (bytes != null) {
      await file.writeAsBytes(bytes);
    }
    return file;
  }

  /// 导出设备列表到 Excel
  static Future<File> exportEquipments(List<Equipment> equipments) async {
    final excel = Excel.createExcel();
    final sheet = excel['设备列表'];

    sheet.appendRow([
      TextCellValue('编号'),
      TextCellValue('名称'),
      TextCellValue('型号'),
      TextCellValue('厂商'),
      TextCellValue('类别'),
      TextCellValue('状态'),
      TextCellValue('总数量'),
      TextCellValue('可用数量'),
      TextCellValue('所属实验室'),
    ]);

    for (final eq in equipments) {
      sheet.appendRow([
        TextCellValue(eq.code),
        TextCellValue(eq.name),
        TextCellValue(eq.model ?? ''),
        TextCellValue(eq.manufacturer ?? ''),
        TextCellValue(eq.category ?? ''),
        TextCellValue(eq.status ?? ''),
        IntCellValue(eq.totalQuantity),
        IntCellValue(eq.availableQuantity),
        TextCellValue(eq.lab?.name ?? ''),
      ]);
    }

    final dir = await getApplicationDocumentsDirectory();
    final file = File('${dir.path}/equipments_${DateTime.now().millisecondsSinceEpoch}.xlsx');
    final bytes = excel.encode();
    if (bytes != null) {
      await file.writeAsBytes(bytes);
    }
    return file;
  }

  static String? _cellValue(CellValue? cell) {
    if (cell == null) return null;
    return cell.toString().trim();
  }
}
