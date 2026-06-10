import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../controllers/app_controller.dart';
import '../controllers/auth_controller.dart';
import '../models/models.dart';
import '../services/services.dart';
import '../widgets/widgets.dart';
import '../utils/theme/app_theme.dart';

const EQUIPMENT_CATEGORIES = [
  '计算机类',
  '电子类',
  '机械类',
  '化学类',
  '物理类',
  '其他',
];

const EQUIPMENT_STATUSES = [
  '在库-可用',
  '借出',
  '维修中',
  '报废',
];

class EquipmentsScreen extends StatefulWidget {
  const EquipmentsScreen({super.key});

  @override
  State<EquipmentsScreen> createState() => _EquipmentsScreenState();
}

class _EquipmentsScreenState extends State<EquipmentsScreen> {
  final _equipmentService = EquipmentService();
  final _auth = Get.find<AuthController>();
  final _app = Get.find<AppController>();

  List<Equipment> _equipments = [];
  EquipmentStatistics _stats = EquipmentStatistics();
  bool _loading = false;
  bool _statsLoading = false;

  String _keyword = '';
  String? _selectedLabId;
  String? _selectedCategory;
  String? _selectedStatus;

  @override
  void initState() {
    super.initState();
    _loadLabs();
    _loadEquipments();
    _loadStats();
  }

  void _loadLabs() {
    if (_app.labs.isEmpty) {
      _app.loadLabs();
    }
  }

  Future<void> _loadEquipments() async {
    setState(() => _loading = true);
    try {
      _equipments = await _equipmentService.getEquipments(
        keyword: _keyword.isNotEmpty ? _keyword : null,
        labId: _selectedLabId,
        category: _selectedCategory,
        status: _selectedStatus,
      );
    } catch (e) {
      Get.snackbar('错误', '获取设备列表失败', snackPosition: SnackPosition.TOP);
    } finally {
      setState(() => _loading = false);
    }
  }

  Future<void> _loadStats() async {
    setState(() => _statsLoading = true);
    try {
      _stats = await _equipmentService.getStatistics();
    } catch (_) {
      _stats = EquipmentStatistics();
    } finally {
      setState(() => _statsLoading = false);
    }
  }

  bool get _canCreate => _auth.hasPermission('equipment:create') || _auth.isAdmin;
  bool get _canUpdate => _auth.hasPermission('equipment:update') || _auth.isAdmin;
  bool get _canDelete => _auth.hasPermission('equipment:delete') || _auth.isAdmin;

  Color _getStatusColor(String? status) {
    switch (status) {
      case '在库-可用':
        return AppTheme.success;
      case '借出':
        return AppTheme.info;
      case '维修中':
        return AppTheme.warning;
      case '报废':
        return Colors.grey;
      default:
        return AppTheme.textSecondary;
    }
  }

  Color _getStatCardColor(int index) {
    final colors = [
      AppTheme.primary,
      AppTheme.success,
      AppTheme.info,
      AppTheme.warning,
      Colors.grey,
    ];
    return colors[index % colors.length];
  }

  void _showEquipmentForm({Equipment? equipment}) {
    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (_) => _EquipmentFormDialog(
        equipment: equipment,
        labs: _app.labs,
        onSaved: () {
          _loadEquipments();
          _loadStats();
        },
      ),
    );
  }

  void _showStatusDialog(Equipment equipment) {
    String? newStatus = equipment.status;
    showDialog(
      context: context,
      builder: (_) => StatefulBuilder(
        builder: (ctx, setDialogState) => AlertDialog(
          title: const Text('更新设备状态'),
          content: DropdownButtonFormField<String>(
            value: newStatus,
            decoration: const InputDecoration(labelText: '设备状态'),
            items: EQUIPMENT_STATUSES
                .map((s) => DropdownMenuItem(value: s, child: Text(s)))
                .toList(),
            onChanged: (v) => setDialogState(() => newStatus = v),
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.pop(ctx),
              child: const Text('取消'),
            ),
            ElevatedButton(
              onPressed: () async {
                if (newStatus == null) return;
                Navigator.pop(ctx);
                await _updateStatus(equipment.id, newStatus!);
              },
              child: const Text('确定'),
            ),
          ],
        ),
      ),
    );
  }

  void _showBorrowDialog(Equipment equipment) {
    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (_) => _BorrowDialog(
        equipment: equipment,
        onSaved: () {
          _loadEquipments();
          _loadStats();
        },
      ),
    );
  }

  Future<void> _updateStatus(String id, String status) async {
    try {
      await _equipmentService.updateEquipmentStatus(id, status);
      Get.snackbar('成功', '状态更新成功', snackPosition: SnackPosition.TOP);
      _loadEquipments();
      _loadStats();
    } catch (e) {
      Get.snackbar('错误', '状态更新失败', snackPosition: SnackPosition.TOP);
    }
  }

  Future<void> _deleteEquipment(Equipment equipment) async {
    final ok = await ConfirmDialog.show(
      context,
      title: '删除确认',
      message: '确定要删除设备「${equipment.name}」吗？此操作不可撤销。',
      confirmLabel: '删除',
      isDanger: true,
    );
    if (!ok) return;

    setState(() => _loading = true);
    try {
      await _equipmentService.deleteEquipment(equipment.id);
      Get.snackbar('成功', '设备已删除', snackPosition: SnackPosition.TOP);
      _loadEquipments();
      _loadStats();
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
        title: const Text('设备管理'),
        actions: [
          if (_canCreate)
            IconButton(
              icon: const Icon(Icons.add),
              onPressed: () => _showEquipmentForm(),
            ),
        ],
      ),
      body: Column(
        children: [
          // 统计卡片
          _buildStatsRow(),
          // 搜索栏
          _buildSearchBar(),
          // 设备列表
          Expanded(
            child: _buildEquipmentList(),
          ),
        ],
      ),
      floatingActionButton: _canCreate
          ? FloatingActionButton(
              onPressed: () => _showEquipmentForm(),
              child: const Icon(Icons.add),
            )
          : null,
    );
  }

  Widget _buildStatsRow() {
    if (_statsLoading) {
      return const Padding(
        padding: EdgeInsets.all(12),
        child: SizedBox(
          height: 80,
          child: Center(child: CircularProgressIndicator()),
        ),
      );
    }
    final statItems = [
      _StatItem('设备总数', '${_stats.totalCount}', Icons.devices),
      _StatItem('在库可用', '${_stats.availableCount}', Icons.check_circle_outline),
      _StatItem('借出', '${_stats.inUseCount}', Icons.output),
      _StatItem('维修中', '${_stats.maintenanceCount}', Icons.build_outlined),
      _StatItem('已报废', '${_stats.totalCount - _stats.availableCount - _stats.inUseCount - _stats.maintenanceCount}', Icons.delete_outline),
    ];
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
      child: Row(
        children: statItems.asMap().entries.map((e) {
          return Expanded(
            child: Card(
              margin: const EdgeInsets.symmetric(horizontal: 2),
              child: InkWell(
                onTap: () {
                  // 可选：点击按该状态筛选
                  setState(() {
                    _selectedStatus = e.value.label == '设备总数'
                        ? null
                        : e.value.label == '在库可用'
                            ? '在库-可用'
                            : e.value.label == '借出'
                                ? '借出'
                                : e.value.label == '维修中'
                                    ? '维修中'
                                    : '报废';
                  });
                  _loadEquipments();
                },
                borderRadius: BorderRadius.circular(12),
                child: Padding(
                  padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 6),
                  child: Column(
                    children: [
                      Icon(
                        e.value.icon,
                        color: _getStatCardColor(e.key),
                        size: 20,
                      ),
                      const SizedBox(height: 4),
                      Text(
                        e.value.value,
                        style: TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.bold,
                          color: _getStatCardColor(e.key),
                        ),
                      ),
                      const SizedBox(height: 2),
                      Text(
                        e.value.label,
                        style: const TextStyle(
                          fontSize: 10,
                          color: AppTheme.textSecondary,
                        ),
                        textAlign: TextAlign.center,
                        overflow: TextOverflow.ellipsis,
                      ),
                    ],
                  ),
                ),
              ),
            ),
          );
        }).toList(),
      ),
    );
  }

  Widget _buildSearchBar() {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12),
      child: Column(
        children: [
          SearchBarWidget(
            hintText: '搜索设备代码/名称/型号...',
            onChanged: (v) => _keyword = v,
            onClear: () {
              _keyword = '';
              _loadEquipments();
            },
          ),
          const SizedBox(height: 8),
          Row(
            children: [
              Expanded(
                child: DropdownButtonFormField<String>(
                  value: _selectedLabId,
                  decoration: InputDecoration(
                    hintText: '所属实验室',
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
                    const DropdownMenuItem(value: '', child: Text('全部实验室')),
                    ..._app.labs.map((l) => DropdownMenuItem(value: l.id, child: Text(l.name))),
                  ],
                  onChanged: (v) {
                    setState(() => _selectedLabId = v?.isEmpty == true ? null : v);
                    _loadEquipments();
                  },
                ),
              ),
              const SizedBox(width: 4),
              Expanded(
                child: DropdownButtonFormField<String>(
                  value: _selectedCategory,
                  decoration: InputDecoration(
                    hintText: '设备分类',
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
                    const DropdownMenuItem(value: '', child: Text('全部分类')),
                    ...EQUIPMENT_CATEGORIES.map((c) => DropdownMenuItem(value: c, child: Text(c))),
                  ],
                  onChanged: (v) {
                    setState(() => _selectedCategory = v?.isEmpty == true ? null : v);
                    _loadEquipments();
                  },
                ),
              ),
              const SizedBox(width: 4),
              Expanded(
                child: DropdownButtonFormField<String>(
                  value: _selectedStatus,
                  decoration: InputDecoration(
                    hintText: '状态',
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
                    const DropdownMenuItem(value: '', child: Text('全部状态')),
                    ...EQUIPMENT_STATUSES.map((s) => DropdownMenuItem(value: s, child: Text(s))),
                  ],
                  onChanged: (v) {
                    setState(() => _selectedStatus = v?.isEmpty == true ? null : v);
                    _loadEquipments();
                  },
                ),
              ),
            ],
          ),
          const SizedBox(height: 8),
          SizedBox(
            width: double.infinity,
            child: ElevatedButton.icon(
              onPressed: _loadEquipments,
              icon: const Icon(Icons.search, size: 18),
              label: const Text('搜索'),
            ),
          ),
          const SizedBox(height: 8),
        ],
      ),
    );
  }

  Widget _buildEquipmentList() {
    if (_loading && _equipments.isEmpty) {
      return const Center(child: CircularProgressIndicator());
    }
    if (_equipments.isEmpty) {
      return const EmptyState(
        icon: Icons.devices_other,
        message: '暂无设备',
      );
    }
    return RefreshIndicator(
      onRefresh: () async {
        await _loadEquipments();
        await _loadStats();
      },
      child: ListView.builder(
        padding: const EdgeInsets.symmetric(horizontal: 12),
        itemCount: _equipments.length,
        itemBuilder: (_, i) => _EquipmentCard(
          equipment: _equipments[i],
          canUpdate: _canUpdate,
          canDelete: _canDelete,
          onEdit: () => _showEquipmentForm(equipment: _equipments[i]),
          onUpdateStatus: () => _showStatusDialog(_equipments[i]),
          onBorrow: () => _showBorrowDialog(_equipments[i]),
          onDelete: () => _deleteEquipment(_equipments[i]),
          getStatusColor: _getStatusColor,
        ),
      ),
    );
  }
}

class _StatItem {
  final String label;
  final String value;
  final IconData icon;

  _StatItem(this.label, this.value, this.icon);
}

class _EquipmentCard extends StatelessWidget {
  final Equipment equipment;
  final bool canUpdate;
  final bool canDelete;
  final VoidCallback onEdit;
  final VoidCallback onUpdateStatus;
  final VoidCallback onBorrow;
  final VoidCallback onDelete;
  final Color Function(String?) getStatusColor;

  const _EquipmentCard({
    required this.equipment,
    required this.canUpdate,
    required this.canDelete,
    required this.onEdit,
    required this.onUpdateStatus,
    required this.onBorrow,
    required this.onDelete,
    required this.getStatusColor,
  });

  @override
  Widget build(BuildContext context) {
    final statusColor = getStatusColor(equipment.status);
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Container(
                  padding: const EdgeInsets.all(10),
                  decoration: BoxDecoration(
                    color: AppTheme.primary.withOpacity(0.1),
                    borderRadius: BorderRadius.circular(10),
                  ),
                  child: const Icon(Icons.devices, color: AppTheme.primary, size: 24),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        equipment.name,
                        style: const TextStyle(
                          fontSize: 15,
                          fontWeight: FontWeight.bold,
                          color: AppTheme.textPrimary,
                        ),
                      ),
                      const SizedBox(height: 2),
                      Text(
                        '代码: ${equipment.code}  ${equipment.model != null ? '型号: ${equipment.model}' : ''}',
                        style: const TextStyle(
                          fontSize: 12,
                          color: AppTheme.textSecondary,
                        ),
                        overflow: TextOverflow.ellipsis,
                      ),
                    ],
                  ),
                ),
              ],
            ),
            const SizedBox(height: 10),
            Wrap(
              spacing: 6,
              runSpacing: 4,
              children: [
                if (equipment.category != null)
                  _MiniChip(
                    icon: Icons.category,
                    label: equipment.category!,
                    color: AppTheme.primary,
                  ),
                if (equipment.lab != null)
                  _MiniChip(
                    icon: Icons.science,
                    label: equipment.lab!.name,
                    color: AppTheme.info,
                  ),
                if (equipment.requiresBooking)
                  _MiniChip(
                    icon: Icons.schedule,
                    label: '需预约',
                    color: AppTheme.warning,
                  ),
                if (equipment.status != null)
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                    decoration: BoxDecoration(
                      color: statusColor.withOpacity(0.12),
                      borderRadius: BorderRadius.circular(6),
                    ),
                    child: Text(
                      equipment.status!,
                      style: TextStyle(
                        fontSize: 11,
                        color: statusColor,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                  ),
                if (equipment.price > 0)
                  _MiniChip(
                    icon: Icons.attach_money,
                    label: '¥${equipment.price.toStringAsFixed(2)}',
                    color: AppTheme.textSecondary,
                  ),
              ],
            ),
            const SizedBox(height: 10),
            Row(
              children: [
                const Spacer(),
                if (canUpdate) ...[
                  TextButton.icon(
                    onPressed: onEdit,
                    icon: const Icon(Icons.edit, size: 15),
                    label: const Text('编辑'),
                    style: TextButton.styleFrom(
                      foregroundColor: AppTheme.info,
                      padding: const EdgeInsets.symmetric(horizontal: 6),
                      visualDensity: VisualDensity.compact,
                    ),
                  ),
                  TextButton.icon(
                    onPressed: onUpdateStatus,
                    icon: const Icon(Icons.sync, size: 15),
                    label: const Text('状态'),
                    style: TextButton.styleFrom(
                      foregroundColor: AppTheme.warning,
                      padding: const EdgeInsets.symmetric(horizontal: 6),
                      visualDensity: VisualDensity.compact,
                    ),
                  ),
                ],
                if (equipment.status == '在库-可用')
                  TextButton.icon(
                    onPressed: onBorrow,
                    icon: const Icon(Icons.output, size: 15),
                    label: const Text('借出'),
                    style: TextButton.styleFrom(
                      foregroundColor: AppTheme.success,
                      padding: const EdgeInsets.symmetric(horizontal: 6),
                      visualDensity: VisualDensity.compact,
                    ),
                  ),
                if (canDelete)
                  TextButton.icon(
                    onPressed: onDelete,
                    icon: const Icon(Icons.delete, size: 15),
                    label: const Text('删除'),
                    style: TextButton.styleFrom(
                      foregroundColor: AppTheme.error,
                      padding: const EdgeInsets.symmetric(horizontal: 6),
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

class _MiniChip extends StatelessWidget {
  final IconData icon;
  final String label;
  final Color color;

  const _MiniChip({
    required this.icon,
    required this.label,
    required this.color,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 3),
      decoration: BoxDecoration(
        color: color.withOpacity(0.1),
        borderRadius: BorderRadius.circular(6),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 12, color: color),
          const SizedBox(width: 3),
          Text(
            label,
            style: TextStyle(fontSize: 11, color: color),
          ),
        ],
      ),
    );
  }
}

class _EquipmentFormDialog extends StatefulWidget {
  final Equipment? equipment;
  final List<Lab> labs;
  final VoidCallback onSaved;

  const _EquipmentFormDialog({
    this.equipment,
    required this.labs,
    required this.onSaved,
  });

  @override
  State<_EquipmentFormDialog> createState() => _EquipmentFormDialogState();
}

class _EquipmentFormDialogState extends State<_EquipmentFormDialog> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _codeCtrl;
  late final TextEditingController _nameCtrl;
  late final TextEditingController _modelCtrl;
  late final TextEditingController _manufacturerCtrl;
  late final TextEditingController _serialNumberCtrl;
  late final TextEditingController _priceCtrl;
  late final TextEditingController _locationCtrl;
  late final TextEditingController _descriptionCtrl;

  String? _labId;
  String? _category;
  String? _status;
  bool _requiresBooking = false;
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    final eq = widget.equipment;
    _codeCtrl = TextEditingController(text: eq?.code);
    _nameCtrl = TextEditingController(text: eq?.name);
    _modelCtrl = TextEditingController(text: eq?.model);
    _manufacturerCtrl = TextEditingController(text: eq?.manufacturer);
    _serialNumberCtrl = TextEditingController(text: eq?.serialNumber);
    _priceCtrl = TextEditingController(text: eq?.price != null && eq!.price > 0 ? eq.price.toString() : '');
    _locationCtrl = TextEditingController(text: eq?.location);
    _descriptionCtrl = TextEditingController(text: eq?.description);
    _labId = eq?.labId;
    _category = eq?.category;
    _status = eq?.status ?? '在库-可用';
    _requiresBooking = eq?.requiresBooking ?? false;
  }

  @override
  void dispose() {
    _codeCtrl.dispose();
    _nameCtrl.dispose();
    _modelCtrl.dispose();
    _manufacturerCtrl.dispose();
    _serialNumberCtrl.dispose();
    _priceCtrl.dispose();
    _locationCtrl.dispose();
    _descriptionCtrl.dispose();
    super.dispose();
  }

  bool get _isEdit => widget.equipment != null;

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;
    setState(() => _isLoading = true);

    final eq = widget.equipment ?? Equipment(id: '', code: '', name: '');
    final updated = Equipment(
      id: eq.id,
      code: _codeCtrl.text.trim(),
      name: _nameCtrl.text.trim(),
      model: _modelCtrl.text.trim().isNotEmpty ? _modelCtrl.text.trim() : null,
      manufacturer: _manufacturerCtrl.text.trim().isNotEmpty ? _manufacturerCtrl.text.trim() : null,
      serialNumber: _serialNumberCtrl.text.trim().isNotEmpty ? _serialNumberCtrl.text.trim() : null,
      labId: _labId,
      category: _category,
      status: _status,
      price: double.tryParse(_priceCtrl.text.trim()) ?? 0,
      location: _locationCtrl.text.trim().isNotEmpty ? _locationCtrl.text.trim() : null,
      description: _descriptionCtrl.text.trim().isNotEmpty ? _descriptionCtrl.text.trim() : null,
      requiresBooking: _requiresBooking,
    );

    try {
      final service = EquipmentService();
      if (_isEdit) {
        await service.updateEquipment(widget.equipment!.id, updated);
        Get.snackbar('成功', '设备已更新', snackPosition: SnackPosition.TOP);
      } else {
        await service.createEquipment(updated);
        Get.snackbar('成功', '设备已创建', snackPosition: SnackPosition.TOP);
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
      title: Text(_isEdit ? '编辑设备' : '新增设备'),
      content: SingleChildScrollView(
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextFormField(
                controller: _codeCtrl,
                decoration: const InputDecoration(labelText: '设备代码 *'),
                validator: (v) => v?.isEmpty == true ? '请输入代码' : null,
                enabled: !_isEdit,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _nameCtrl,
                decoration: const InputDecoration(labelText: '设备名称 *'),
                validator: (v) => v?.isEmpty == true ? '请输入名称' : null,
              ),
              const SizedBox(height: 12),
              Row(
                children: [
                  Expanded(
                    child: TextFormField(
                      controller: _modelCtrl,
                      decoration: const InputDecoration(labelText: '型号'),
                    ),
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: TextFormField(
                      controller: _manufacturerCtrl,
                      decoration: const InputDecoration(labelText: '厂商'),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 12),
              Row(
                children: [
                  Expanded(
                    child: DropdownButtonFormField<String>(
                      value: _category,
                      decoration: const InputDecoration(labelText: '设备分类'),
                      items: EQUIPMENT_CATEGORIES
                          .map((c) => DropdownMenuItem(value: c, child: Text(c)))
                          .toList(),
                      onChanged: (v) => setState(() => _category = v),
                    ),
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: DropdownButtonFormField<String>(
                      value: _status,
                      decoration: const InputDecoration(labelText: '状态'),
                      items: EQUIPMENT_STATUSES
                          .map((s) => DropdownMenuItem(value: s, child: Text(s)))
                          .toList(),
                      onChanged: (v) => setState(() => _status = v),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 12),
              DropdownButtonFormField<String>(
                value: _labId,
                decoration: const InputDecoration(labelText: '所属实验室'),
                items: [
                  const DropdownMenuItem(value: '', child: Text('未分配')),
                  ...widget.labs.map((l) => DropdownMenuItem(value: l.id, child: Text(l.name))),
                ],
                onChanged: (v) => setState(() => _labId = v?.isEmpty == true ? null : v),
              ),
              const SizedBox(height: 12),
              Row(
                children: [
                  Expanded(
                    child: TextFormField(
                      controller: _priceCtrl,
                      decoration: const InputDecoration(labelText: '价格(元)'),
                      keyboardType: TextInputType.number,
                    ),
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: TextFormField(
                      controller: _serialNumberCtrl,
                      decoration: const InputDecoration(labelText: '序列号'),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _locationCtrl,
                decoration: const InputDecoration(labelText: '存放地点'),
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _descriptionCtrl,
                decoration: const InputDecoration(labelText: '描述'),
                maxLines: 2,
              ),
              const SizedBox(height: 12),
              SwitchListTile(
                title: const Text('需要预约'),
                value: _requiresBooking,
                onChanged: (v) => setState(() => _requiresBooking = v),
                contentPadding: EdgeInsets.zero,
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

class _BorrowDialog extends StatefulWidget {
  final Equipment equipment;
  final VoidCallback onSaved;

  const _BorrowDialog({required this.equipment, required this.onSaved});

  @override
  State<_BorrowDialog> createState() => _BorrowDialogState();
}

class _BorrowDialogState extends State<_BorrowDialog> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _phoneCtrl;
  late final TextEditingController _locationCtrl;
  late final TextEditingController _purposeCtrl;
  late final TextEditingController _remarksCtrl;

  DateTime? _borrowDate;
  DateTime? _expectedReturnDate;
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _phoneCtrl = TextEditingController();
    _locationCtrl = TextEditingController();
    _purposeCtrl = TextEditingController();
    _remarksCtrl = TextEditingController();
  }

  @override
  void dispose() {
    _phoneCtrl.dispose();
    _locationCtrl.dispose();
    _purposeCtrl.dispose();
    _remarksCtrl.dispose();
    super.dispose();
  }

  String _formatDate(DateTime d) => '${d.year}-${d.month.toString().padLeft(2, '0')}-${d.day.toString().padLeft(2, '0')}';

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;
    if (_borrowDate == null) {
      Get.snackbar('提示', '请选择借用日期', snackPosition: SnackPosition.TOP);
      return;
    }
    if (_expectedReturnDate == null) {
      Get.snackbar('提示', '请选择计划归还日期', snackPosition: SnackPosition.TOP);
      return;
    }
    if (_expectedReturnDate!.isBefore(_borrowDate!) || _expectedReturnDate!.isAtSameMomentAs(_borrowDate!)) {
      Get.snackbar('提示', '计划归还日期必须晚于借用日期', snackPosition: SnackPosition.TOP);
      return;
    }

    setState(() => _isLoading = true);
    try {
      final req = CreateBorrowRequest(
        equipmentId: widget.equipment.id,
        borrowDate: _formatDate(_borrowDate!),
        expectedReturnDate: _formatDate(_expectedReturnDate!),
        purpose: _purposeCtrl.text.trim(),
        phone: _phoneCtrl.text.trim(),
        usageLocation: _locationCtrl.text.trim().isNotEmpty ? _locationCtrl.text.trim() : null,
        remarks: _remarksCtrl.text.trim().isNotEmpty ? _remarksCtrl.text.trim() : null,
      );
      await BorrowRecordService().createRecord(req);
      Get.snackbar('成功', '借出申请已提交', snackPosition: SnackPosition.TOP);
      widget.onSaved();
      if (mounted) Navigator.pop(context);
    } catch (e) {
      Get.snackbar('错误', '提交失败', snackPosition: SnackPosition.TOP);
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Text('设备借出申请'),
      content: SingleChildScrollView(
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // 设备信息展示
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: AppTheme.primary.withOpacity(0.05),
                  borderRadius: BorderRadius.circular(8),
                  border: Border.all(color: Colors.grey.shade200),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      widget.equipment.name,
                      style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 14),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      '设备代码: ${widget.equipment.code}',
                      style: const TextStyle(fontSize: 12, color: AppTheme.textSecondary),
                    ),
                    if (widget.equipment.lab != null)
                      Text(
                        '所属实验室: ${widget.equipment.lab!.name}',
                        style: const TextStyle(fontSize: 12, color: AppTheme.textSecondary),
                      ),
                  ],
                ),
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _phoneCtrl,
                decoration: const InputDecoration(labelText: '联系电话 *'),
                keyboardType: TextInputType.phone,
                validator: (v) => v?.isEmpty == true ? '请输入联系电话' : null,
              ),
              const SizedBox(height: 12),
              InkWell(
                onTap: () async {
                  final d = await showDatePicker(
                    context: context,
                    initialDate: DateTime.now(),
                    firstDate: DateTime.now(),
                    lastDate: DateTime.now().add(const Duration(days: 365)),
                  );
                  if (d != null) setState(() => _borrowDate = d);
                },
                child: InputDecorator(
                  decoration: InputDecoration(
                    labelText: '借用日期 *',
                    suffixIcon: const Icon(Icons.calendar_today),
                    errorText: _borrowDate == null ? null : null,
                  ),
                  child: Text(
                    _borrowDate != null ? _formatDate(_borrowDate!) : '请选择日期',
                    style: TextStyle(
                      color: _borrowDate != null ? null : AppTheme.textHint,
                    ),
                  ),
                ),
              ),
              const SizedBox(height: 12),
              InkWell(
                onTap: () async {
                  final d = await showDatePicker(
                    context: context,
                    initialDate: _borrowDate?.add(const Duration(days: 7)) ?? DateTime.now().add(const Duration(days: 7)),
                    firstDate: _borrowDate?.add(const Duration(days: 1)) ?? DateTime.now(),
                    lastDate: DateTime.now().add(const Duration(days: 365)),
                  );
                  if (d != null) setState(() => _expectedReturnDate = d);
                },
                child: InputDecorator(
                  decoration: InputDecoration(
                    labelText: '计划归还日期 *',
                    suffixIcon: const Icon(Icons.calendar_today),
                  ),
                  child: Text(
                    _expectedReturnDate != null ? _formatDate(_expectedReturnDate!) : '请选择日期',
                    style: TextStyle(
                      color: _expectedReturnDate != null ? null : AppTheme.textHint,
                    ),
                  ),
                ),
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _locationCtrl,
                decoration: const InputDecoration(labelText: '使用地点'),
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _purposeCtrl,
                decoration: const InputDecoration(labelText: '借用用途 *'),
                maxLines: 2,
                validator: (v) => v?.isEmpty == true ? '请输入借用用途' : null,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _remarksCtrl,
                decoration: const InputDecoration(labelText: '备注'),
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
              : const Text('提交申请'),
        ),
      ],
    );
  }
}
