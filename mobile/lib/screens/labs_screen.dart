import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../controllers/app_controller.dart';
import '../controllers/auth_controller.dart';
import '../models/models.dart';
import '../services/lab_service.dart';
import '../widgets/widgets.dart';
import '../utils/theme/app_theme.dart';

class LabsScreen extends StatefulWidget {
  const LabsScreen({super.key});

  @override
  State<LabsScreen> createState() => _LabsScreenState();
}

class _LabsScreenState extends State<LabsScreen> {
  final _labService = LabService();
  final _auth = Get.find<AuthController>();

  List<Lab> _labs = [];
  bool _loading = false;
  String _keyword = '';
  String? _selectedLabType;

  @override
  void initState() {
    super.initState();
    _loadLabs();
  }

  Future<void> _loadLabs() async {
    setState(() => _loading = true);
    try {
      _labs = await _labService.getLabs(
        keyword: _keyword.isNotEmpty ? _keyword : null,
      );
      // 客户端筛选实验室类型
      if (_selectedLabType != null && _selectedLabType!.isNotEmpty) {
        _labs = _labs.where((l) => l.labType == _selectedLabType).toList();
      }
    } catch (e) {
      Get.snackbar('错误', '获取实验室列表失败', snackPosition: SnackPosition.TOP);
    } finally {
      setState(() => _loading = false);
    }
  }

  bool get _canCreate => _auth.hasPermission('lab:create') || _auth.isAdmin;
  bool get _canUpdate => _auth.hasPermission('lab:update') || _auth.isAdmin;
  bool get _canDelete => _auth.hasPermission('lab:delete') || _auth.isAdmin;

  Color _getSafetyLevelColor(String? level) {
    switch (level) {
      case '一般':
        return Colors.green;
      case '中等':
        return Colors.orange;
      case '高危':
        return Colors.red;
      default:
        return AppTheme.textSecondary;
    }
  }

  void _showLabForm({Lab? lab}) {
    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (_) => _LabFormDialog(
        lab: lab,
        onSaved: () {
          _loadLabs();
          Get.find<AppController>().loadLabs();
        },
      ),
    );
  }

  Future<void> _toggleStatus(Lab lab) async {
    try {
      await _labService.toggleLabStatus(lab.id, !lab.isActive);
      Get.snackbar(
        '成功',
        lab.isActive ? '实验室已禁用' : '实验室已启用',
        snackPosition: SnackPosition.TOP,
      );
      _loadLabs();
    } catch (e) {
      Get.snackbar('错误', '状态修改失败', snackPosition: SnackPosition.TOP);
    }
  }

  Future<void> _deleteLab(Lab lab) async {
    final ok = await ConfirmDialog.show(
      context,
      title: '删除确认',
      message: '确定要删除实验室「${lab.name}」吗？此操作不可撤销。',
      confirmLabel: '删除',
      isDanger: true,
    );
    if (!ok) return;

    setState(() => _loading = true);
    try {
      await _labService.deleteLab(lab.id);
      Get.snackbar('成功', '实验室已删除', snackPosition: SnackPosition.TOP);
      _loadLabs();
      Get.find<AppController>().loadLabs();
    } catch (e) {
      Get.snackbar('错误', '删除失败', snackPosition: SnackPosition.TOP);
    } finally {
      setState(() => _loading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('实验室管理'),
        actions: [
          if (_canCreate)
            IconButton(
              icon: const Icon(Icons.add),
              onPressed: () => _showLabForm(),
            ),
        ],
      ),
      body: Column(
        children: [
          // 搜索栏
          Padding(
            padding: const EdgeInsets.all(12),
            child: Column(
              children: [
                SearchBarWidget(
                  hintText: '搜索实验室代码/名称...',
                  onChanged: (v) => _keyword = v,
                  onClear: () {
                    _keyword = '';
                    _loadLabs();
                  },
                ),
                const SizedBox(height: 8),
                Row(
                  children: [
                    Expanded(
                      child: DropdownButtonFormField<String>(
                        value: _selectedLabType,
                        decoration: InputDecoration(
                          hintText: '实验室类型',
                          contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 0),
                          border: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(10),
                            borderSide: BorderSide.none,
                          ),
                          filled: true,
                          fillColor: Colors.white,
                        ),
                        isExpanded: true,
                        items: [
                          const DropdownMenuItem(value: '', child: Text('全部类型')),
                          ..._labTypes.map((t) => DropdownMenuItem(value: t, child: Text(t))),
                        ],
                        onChanged: (v) {
                          setState(() => _selectedLabType = v);
                          _loadLabs();
                        },
                      ),
                    ),
                    const SizedBox(width: 8),
                    ElevatedButton.icon(
                      onPressed: _loadLabs,
                      icon: const Icon(Icons.search, size: 18),
                      label: const Text('搜索'),
                      style: ElevatedButton.styleFrom(
                        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),
          // 列表
          Expanded(
            child: _loading
                ? const Center(child: CircularProgressIndicator())
                : _labs.isEmpty
                    ? const EmptyState(
                        icon: Icons.science_outlined,
                        message: '暂无实验室',
                      )
                    : RefreshIndicator(
                        onRefresh: _loadLabs,
                        child: ListView.builder(
                          padding: const EdgeInsets.symmetric(horizontal: 12),
                          itemCount: _labs.length,
                          itemBuilder: (_, i) => _LabCard(
                            lab: _labs[i],
                            canUpdate: _canUpdate,
                            canDelete: _canDelete,
                            onEdit: () => _showLabForm(lab: _labs[i]),
                            onToggleStatus: () => _toggleStatus(_labs[i]),
                            onDelete: () => _deleteLab(_labs[i]),
                            getSafetyLevelColor: _getSafetyLevelColor,
                          ),
                        ),
                      ),
          ),
        ],
      ),
      floatingActionButton: _canCreate
          ? FloatingActionButton(
              onPressed: () => _showLabForm(),
              child: const Icon(Icons.add),
            )
          : null,
    );
  }
}

const _labTypes = ['教学', '科研', '实训', '一般', '特殊'];

class _LabCard extends StatelessWidget {
  final Lab lab;
  final bool canUpdate;
  final bool canDelete;
  final VoidCallback onEdit;
  final VoidCallback onToggleStatus;
  final VoidCallback onDelete;
  final Color Function(String?) getSafetyLevelColor;

  const _LabCard({
    required this.lab,
    required this.canUpdate,
    required this.canDelete,
    required this.onEdit,
    required this.onToggleStatus,
    required this.onDelete,
    required this.getSafetyLevelColor,
  });

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        lab.name,
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                          color: AppTheme.textPrimary,
                        ),
                      ),
                      const SizedBox(height: 2),
                      Text(
                        '代码: ${lab.code}',
                        style: const TextStyle(
                          fontSize: 12,
                          color: AppTheme.textSecondary,
                        ),
                      ),
                    ],
                  ),
                ),
                if (canUpdate)
                  Switch(
                    value: lab.isActive,
                    onChanged: (_) => onToggleStatus(),
                    activeColor: AppTheme.success,
                  ),
              ],
            ),
            const SizedBox(height: 8),
            Wrap(
              spacing: 8,
              runSpacing: 4,
              children: [
                if (lab.labType != null)
                  Chip(
                    label: Text(lab.labType!, style: const TextStyle(fontSize: 11)),
                    backgroundColor: AppTheme.primary.withOpacity(0.1),
                    padding: EdgeInsets.zero,
                    materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
                    visualDensity: VisualDensity.compact,
                  ),
                if (lab.safetyLevel != null)
                  Chip(
                    label: Text(
                      lab.safetyLevel!,
                      style: TextStyle(
                        fontSize: 11,
                        color: getSafetyLevelColor(lab.safetyLevel),
                      ),
                    ),
                    backgroundColor: getSafetyLevelColor(lab.safetyLevel).withOpacity(0.1),
                    padding: EdgeInsets.zero,
                    materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
                    visualDensity: VisualDensity.compact,
                  ),
                if (lab.location != null)
                  Chip(
                    avatar: const Icon(Icons.location_on, size: 14, color: AppTheme.textSecondary),
                    label: Text(lab.location!, style: const TextStyle(fontSize: 11)),
                    padding: EdgeInsets.zero,
                    materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
                    visualDensity: VisualDensity.compact,
                  ),
              ],
            ),
            const SizedBox(height: 8),
            Row(
              children: [
                _InfoChip(
                  icon: Icons.people,
                  label: '容纳 ${lab.capacity} 人',
                ),
                const SizedBox(width: 8),
                if (lab.floor > 0) _InfoChip(icon: Icons.layers, label: '${lab.floor}层'),
                const Spacer(),
                if (canUpdate)
                  TextButton.icon(
                    onPressed: onEdit,
                    icon: const Icon(Icons.edit, size: 16),
                    label: const Text('编辑'),
                    style: TextButton.styleFrom(
                      foregroundColor: AppTheme.info,
                      padding: const EdgeInsets.symmetric(horizontal: 8),
                      visualDensity: VisualDensity.compact,
                    ),
                  ),
                if (canDelete)
                  TextButton.icon(
                    onPressed: onDelete,
                    icon: const Icon(Icons.delete, size: 16),
                    label: const Text('删除'),
                    style: TextButton.styleFrom(
                      foregroundColor: AppTheme.error,
                      padding: const EdgeInsets.symmetric(horizontal: 8),
                      visualDensity: VisualDensity.compact,
                    ),
                  ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

class _InfoChip extends StatelessWidget {
  final IconData icon;
  final String label;

  const _InfoChip({required this.icon, required this.label});

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Icon(icon, size: 14, color: AppTheme.textSecondary),
        const SizedBox(width: 4),
        Text(
          label,
          style: const TextStyle(fontSize: 12, color: AppTheme.textSecondary),
        ),
      ],
    );
  }
}

class _LabFormDialog extends StatefulWidget {
  final Lab? lab;
  final VoidCallback onSaved;

  const _LabFormDialog({this.lab, required this.onSaved});

  @override
  State<_LabFormDialog> createState() => _LabFormDialogState();
}

class _LabFormDialogState extends State<_LabFormDialog> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _codeCtrl;
  late final TextEditingController _nameCtrl;
  late final TextEditingController _locationCtrl;
  late final TextEditingController _capacityCtrl;
  late final TextEditingController _descriptionCtrl;
  late final TextEditingController _roomNumberCtrl;
  late final TextEditingController _floorCtrl;

  String? _labType;
  String? _safetyLevel;
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _codeCtrl = TextEditingController(text: widget.lab?.code);
    _nameCtrl = TextEditingController(text: widget.lab?.name);
    _locationCtrl = TextEditingController(text: widget.lab?.location);
    _capacityCtrl = TextEditingController(text: widget.lab?.capacity.toString());
    _descriptionCtrl = TextEditingController(text: widget.lab?.description);
    _roomNumberCtrl = TextEditingController(text: widget.lab?.roomNumber);
    _floorCtrl = TextEditingController(text: widget.lab?.floor.toString());
    _labType = widget.lab?.labType;
    _safetyLevel = widget.lab?.safetyLevel;
  }

  @override
  void dispose() {
    _codeCtrl.dispose();
    _nameCtrl.dispose();
    _locationCtrl.dispose();
    _capacityCtrl.dispose();
    _descriptionCtrl.dispose();
    _roomNumberCtrl.dispose();
    _floorCtrl.dispose();
    super.dispose();
  }

  bool get _isEdit => widget.lab != null;

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;
    setState(() => _isLoading = true);

    final req = CreateLabRequest(
      code: _codeCtrl.text.trim(),
      name: _nameCtrl.text.trim(),
      location: _locationCtrl.text.trim().isNotEmpty ? _locationCtrl.text.trim() : null,
      capacity: int.tryParse(_capacityCtrl.text.trim()) ?? 0,
      labType: _labType,
      safetyLevel: _safetyLevel,
      description: _descriptionCtrl.text.trim().isNotEmpty ? _descriptionCtrl.text.trim() : null,
      roomNumber: _roomNumberCtrl.text.trim().isNotEmpty ? _roomNumberCtrl.text.trim() : null,
      floor: int.tryParse(_floorCtrl.text.trim()) ?? 0,
    );

    try {
      final labService = LabService();
      if (_isEdit) {
        await labService.updateLab(widget.lab!.id, req);
        Get.snackbar('成功', '实验室已更新', snackPosition: SnackPosition.TOP);
      } else {
        await labService.createLab(req);
        Get.snackbar('成功', '实验室已创建', snackPosition: SnackPosition.TOP);
      }
      widget.onSaved();
      if (mounted) Navigator.pop(context);
    } catch (e) {
      Get.snackbar('错误', _isEdit ? '更新失败' : '创建失败', snackPosition: SnackPosition.TOP);
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text(_isEdit ? '编辑实验室' : '新增实验室'),
      content: SingleChildScrollView(
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextFormField(
                controller: _codeCtrl,
                decoration: const InputDecoration(labelText: '实验室代码 *'),
                validator: (v) => v?.isEmpty == true ? '请输入代码' : null,
                enabled: !_isEdit,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _nameCtrl,
                decoration: const InputDecoration(labelText: '实验室名称 *'),
                validator: (v) => v?.isEmpty == true ? '请输入名称' : null,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _locationCtrl,
                decoration: const InputDecoration(labelText: '地点'),
              ),
              const SizedBox(height: 12),
              Row(
                children: [
                  Expanded(
                    child: TextFormField(
                      controller: _roomNumberCtrl,
                      decoration: const InputDecoration(labelText: '房间号'),
                    ),
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: TextFormField(
                      controller: _floorCtrl,
                      decoration: const InputDecoration(labelText: '楼层'),
                      keyboardType: TextInputType.number,
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 12),
              Row(
                children: [
                  Expanded(
                    child: DropdownButtonFormField<String>(
                      value: _labType,
                      decoration: const InputDecoration(labelText: '实验室类型'),
                      items: _labTypes
                          .map((t) => DropdownMenuItem(value: t, child: Text(t)))
                          .toList(),
                      onChanged: (v) => setState(() => _labType = v),
                    ),
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: DropdownButtonFormField<String>(
                      value: _safetyLevel,
                      decoration: const InputDecoration(labelText: '安全等级'),
                      items: ['一般', '中等', '高危']
                          .map((s) => DropdownMenuItem(value: s, child: Text(s)))
                          .toList(),
                      onChanged: (v) => setState(() => _safetyLevel = v),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _capacityCtrl,
                decoration: const InputDecoration(labelText: '容纳人数'),
                keyboardType: TextInputType.number,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _descriptionCtrl,
                decoration: const InputDecoration(labelText: '描述'),
                maxLines: 2,
              ),
            ],
          ),
        ),
      ),
      actions: [
        TextButton(
          onPressed: _isLoading ? null : () => Navigator.pop(context),
          child: const Text('取消'),
        ),
        ElevatedButton(
          onPressed: _isLoading ? null : _submit,
          child: _isLoading
              ? const SizedBox(
                  width: 20,
                  height: 20,
                  child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
                )
              : Text(_isEdit ? '保存' : '创建'),
        ),
      ],
    );
  }
}
