import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../controllers/auth_controller.dart';
import '../controllers/borrow_controller.dart';
import '../models/models.dart';
import '../services/services.dart';
import '../widgets/widgets.dart';
import '../utils/theme/app_theme.dart';

const BORROW_STATUSES = [
  '待老师审批',
  '待管理员审批',
  '已借出',
  '已归还',
  '已拒绝',
  '已逾期',
  '续借审批中',
  '管理员审批中',
];

const RETURN_CONDITIONS = ['完好', '损坏', '丢失'];

class BorrowRecordsScreen extends StatefulWidget {
  const BorrowRecordsScreen({super.key});

  @override
  State<BorrowRecordsScreen> createState() => _BorrowRecordsScreenState();
}

class _BorrowRecordsScreenState extends State<BorrowRecordsScreen>
    with SingleTickerProviderStateMixin {
  late TabController _tabController;
  final _auth = Get.find<AuthController>();
  final _borrowCtrl = Get.find<BorrowController>();

  bool _canApprove = false;
  bool _canReadAll = false;
  bool _canDelete = false;

  List<BorrowRecord> _myRecords = [];
  List<BorrowRecord> _pendingRecords = [];
  List<BorrowRecord> _allRecords = [];
  List<BorrowRecord> _overdueRecords = [];

  bool _myLoading = false;
  bool _pendingLoading = false;
  bool _allLoading = false;
  bool _overdueLoading = false;

  String _myStatus = '';
  String _allKeyword = '';
  String _allStatus = '';

  @override
  void initState() {
    super.initState();
    _initPermissions();
    _tabController = TabController(length: _getTabCount(), vsync: this);
    _tabController.addListener(_onTabChanged);
    _loadMyRecords();
  }

  @override
  void dispose() {
    _tabController.removeListener(_onTabChanged);
    _tabController.dispose();
    super.dispose();
  }

  void _initPermissions() {
    _canApprove = _auth.hasPermission('equipment:approve') || _auth.isAdmin || _auth.isTeacher;
    _canReadAll = _auth.hasPermission('equipment:read') || _auth.isAdmin;
    _canDelete = _auth.hasPermission('equipment:delete') || _auth.isAdmin;
  }

  int _getTabCount() => _canApprove ? 4 : 2;

  void _onTabChanged() {
    if (!_tabController.indexIsChanging) {
      switch (_tabController.index) {
        case 0:
          _loadMyRecords();
          break;
        case 1:
          if (_canApprove) _loadPendingRecords();
          break;
        case 2:
          if (_canReadAll) _loadAllRecords();
          break;
        case 3:
          if (_canApprove) _loadOverdueRecords();
          break;
      }
    }
  }

  // 我的申请
  Future<void> _loadMyRecords() async {
    setState(() => _myLoading = true);
    try {
      _myRecords = await BorrowRecordService().getMyRecords(
        status: _myStatus.isNotEmpty ? _myStatus : null,
      );
    } catch (_) {
      Get.snackbar('错误', '获取申请记录失败', snackPosition: SnackPosition.TOP);
    } finally {
      setState(() => _myLoading = false);
    }
  }

  // 待审批
  Future<void> _loadPendingRecords() async {
    setState(() => _pendingLoading = true);
    try {
      _pendingRecords = await BorrowRecordService().getPending();
    } catch (_) {
      Get.snackbar('错误', '获取待审批记录失败', snackPosition: SnackPosition.TOP);
    } finally {
      setState(() => _pendingLoading = false);
    }
  }

  // 全部记录
  Future<void> _loadAllRecords() async {
    setState(() => _allLoading = true);
    try {
      _allRecords = await BorrowRecordService().getRecords(
        status: _allStatus.isNotEmpty ? _allStatus : null,
        keyword: _allKeyword.isNotEmpty ? _allKeyword : null,
      );
    } catch (_) {
      Get.snackbar('错误', '获取记录失败', snackPosition: SnackPosition.TOP);
    } finally {
      setState(() => _allLoading = false);
    }
  }

  // 逾期清单
  Future<void> _loadOverdueRecords() async {
    setState(() => _overdueLoading = true);
    try {
      _overdueRecords = await BorrowRecordService().getOverdue();
    } catch (_) {
      Get.snackbar('错误', '获取逾期清单失败', snackPosition: SnackPosition.TOP);
    } finally {
      setState(() => _overdueLoading = false);
    }
  }

  Color _getStatusColor(String? status) {
    switch (status) {
      case '待老师审批':
        return Colors.grey;
      case '待管理员审批':
        return AppTheme.warning;
      case '管理员审批中':
        return Colors.grey;
      case '续借审批中':
        return AppTheme.warning;
      case '已借出':
        return AppTheme.info;
      case '已归还':
        return AppTheme.success;
      case '已拒绝':
        return AppTheme.error;
      case '已逾期':
        return AppTheme.error;
      case '待领取':
        return AppTheme.warning;
      case '待归还':
        return AppTheme.warning;
      default:
        return AppTheme.textSecondary;
    }
  }

  void _showDetailDialog(BorrowRecord record) {
    showDialog(
      context: context,
      builder: (_) => _DetailDialog(
        record: record,
        canApprove: _canApprove,
        onApprove: () {
          Navigator.pop(context);
          _loadCurrentTab();
        },
        onRenew: () {
          Navigator.pop(context);
          _showRenewDialog(record);
        },
        onReturn: () {
          Navigator.pop(context);
          _showReturnDialog(record);
        },
        getStatusColor: _getStatusColor,
      ),
    );
  }

  void _showRenewDialog(BorrowRecord record) {
    DateTime? newReturnDate;
    bool isLoading = false;

    showDialog(
      context: context,
      builder: (ctx) => StatefulBuilder(
        builder: (ctx, setDialogState) => AlertDialog(
          title: const Text('续借申请'),
          content: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                '当前计划归还: ${record.expectedReturnDate?.substring(0, 10) ?? '-'}',
                style: const TextStyle(fontSize: 13, color: AppTheme.textSecondary),
              ),
              const SizedBox(height: 12),
              InkWell(
                onTap: () async {
                  final d = await showDatePicker(
                    context: ctx,
                    initialDate: DateTime.now().add(const Duration(days: 7)),
                    firstDate: DateTime.now(),
                    lastDate: DateTime.now().add(const Duration(days: 365)),
                  );
                  if (d != null) setDialogState(() => newReturnDate = d);
                },
                child: InputDecorator(
                  decoration: const InputDecoration(
                    labelText: '新计划归还日期 *',
                    suffixIcon: Icon(Icons.calendar_today),
                  ),
                  child: Text(
                    newReturnDate != null
                        ? '${newReturnDate!.year}-${newReturnDate!.month.toString().padLeft(2, '0')}-${newReturnDate!.day.toString().padLeft(2, '0')}'
                        : '请选择日期',
                    style: TextStyle(
                      color: newReturnDate != null ? null : AppTheme.textHint,
                    ),
                  ),
                ),
              ),
            ],
          ),
          actions: [
            TextButton(
              onPressed: isLoading ? null : () => Navigator.pop(ctx),
              child: const Text('取消'),
            ),
            ElevatedButton(
              onPressed: isLoading
                  ? null
                  : () async {
                      if (newReturnDate == null) {
                        Get.snackbar('提示', '请选择新归还日期', snackPosition: SnackPosition.TOP);
                        return;
                      }
                      setDialogState(() => isLoading = true);
                      try {
                        final ok = await BorrowRecordService().renew(
                          record.id,
                          RenewRequest(
                            newReturnDate:
                                '${newReturnDate!.year}-${newReturnDate!.month.toString().padLeft(2, '0')}-${newReturnDate!.day.toString().padLeft(2, '0')}',
                          ),
                        );
                        if (ok) {
                          Get.snackbar('成功', '续借申请已提交', snackPosition: SnackPosition.TOP);
                          Navigator.pop(ctx);
                          _loadCurrentTab();
                        }
                      } catch (_) {
                        Get.snackbar('错误', '续借申请失败', snackPosition: SnackPosition.TOP);
                      }
                    },
              child: isLoading
                  ? const SizedBox(
                      width: 20,
                      height: 20,
                      child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
                    )
                  : const Text('提交续借'),
            ),
          ],
        ),
      ),
    );
  }

  void _showReturnDialog(BorrowRecord record) {
    String condition = '完好';
    String remarks = '';
    bool isLoading = false;

    showDialog(
      context: context,
      builder: (ctx) => StatefulBuilder(
        builder: (ctx, setDialogState) => AlertDialog(
          title: const Text('归还确认'),
          content: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                '归还设备: ${record.equipmentName ?? '-'}',
                style: const TextStyle(fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 16),
              DropdownButtonFormField<String>(
                value: condition,
                decoration: const InputDecoration(labelText: '归还条件 *'),
                items: RETURN_CONDITIONS
                    .map((c) => DropdownMenuItem(value: c, child: Text(c)))
                    .toList(),
                onChanged: (v) => setDialogState(() => condition = v ?? '完好'),
              ),
              const SizedBox(height: 12),
              TextField(
                controller: TextEditingController(text: remarks),
                decoration: const InputDecoration(labelText: '归还备注'),
                maxLines: 2,
                onChanged: (v) => remarks = v,
              ),
            ],
          ),
          actions: [
            TextButton(
              onPressed: isLoading ? null : () => Navigator.pop(ctx),
              child: const Text('取消'),
            ),
            ElevatedButton(
              onPressed: isLoading
                  ? null
                  : () async {
                      setDialogState(() => isLoading = true);
                      try {
                        final ok = await BorrowRecordService().confirmReturn(
                          record.id,
                          ReturnConfirmRequest(condition: condition, remarks: remarks.isNotEmpty ? remarks : null),
                        );
                        if (ok) {
                          Get.snackbar('成功', '归还确认成功', snackPosition: SnackPosition.TOP);
                          Navigator.pop(ctx);
                          _loadCurrentTab();
                        }
                      } catch (_) {
                        Get.snackbar('错误', '归还确认失败', snackPosition: SnackPosition.TOP);
                      }
                    },
              child: isLoading
                  ? const SizedBox(
                      width: 20,
                      height: 20,
                      child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
                    )
                  : const Text('确认归还'),
            ),
          ],
        ),
      ),
    );
  }

  void _showApprovalDialog(BorrowRecord record) {
    String condition = '完好';
    bool approved = true;
    String remarks = '';
    bool isLoading = false;
    bool isReturnApproval = record.status == '管理员审批中';

    showDialog(
      context: context,
      builder: (ctx) => StatefulBuilder(
        builder: (ctx, setDialogState) => AlertDialog(
          title: const Text('审批确认'),
          content: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  const Text('审批结果: '),
                  const SizedBox(width: 8),
                  ChoiceChip(
                    label: const Text('批准'),
                    selected: approved,
                    onSelected: (v) => setDialogState(() => approved = true),
                    selectedColor: AppTheme.success.withOpacity(0.2),
                  ),
                  const SizedBox(width: 8),
                  ChoiceChip(
                    label: const Text('拒绝'),
                    selected: !approved,
                    onSelected: (v) => setDialogState(() => approved = false),
                    selectedColor: AppTheme.error.withOpacity(0.2),
                  ),
                ],
              ),
              if (isReturnApproval && approved) ...[
                const SizedBox(height: 12),
                DropdownButtonFormField<String>(
                  value: condition,
                  decoration: const InputDecoration(labelText: '归还条件 *'),
                  items: RETURN_CONDITIONS
                      .map((c) => DropdownMenuItem(value: c, child: Text(c)))
                      .toList(),
                  onChanged: (v) => setDialogState(() => condition = v ?? '完好'),
                ),
              ],
              const SizedBox(height: 12),
              TextField(
                controller: TextEditingController(text: remarks),
                decoration: const InputDecoration(labelText: '审批备注'),
                maxLines: 2,
                onChanged: (v) => remarks = v,
              ),
            ],
          ),
          actions: [
            TextButton(
              onPressed: isLoading ? null : () => Navigator.pop(ctx),
              child: const Text('取消'),
            ),
            ElevatedButton(
              onPressed: isLoading
                  ? null
                  : () async {
                      setDialogState(() => isLoading = true);
                      try {
                        bool ok;
                        if (record.status == '管理员审批中') {
                          ok = await BorrowRecordService().approveReturn(
                            record.id,
                            ReturnConfirmRequest(condition: condition, remarks: remarks.isNotEmpty ? remarks : null),
                          );
                        } else if (record.status == '续借审批中') {
                          ok = await BorrowRecordService().approveRenew(
                            record.id,
                            BorrowApprovalRequest(approved: approved, remark: remarks.isNotEmpty ? remarks : null),
                          );
                        } else if (record.status == '待管理员审批') {
                          ok = await BorrowRecordService().adminApprove(
                            record.id,
                            BorrowApprovalRequest(approved: approved, remark: remarks.isNotEmpty ? remarks : null),
                          );
                        } else {
                          ok = await BorrowRecordService().supervisorApprove(
                            record.id,
                            BorrowApprovalRequest(approved: approved, remark: remarks.isNotEmpty ? remarks : null),
                          );
                        }
                        if (ok) {
                          Get.snackbar('成功', '审批完成', snackPosition: SnackPosition.TOP);
                          Navigator.pop(ctx);
                          _loadCurrentTab();
                        }
                      } catch (_) {
                        Get.snackbar('错误', '审批失败', snackPosition: SnackPosition.TOP);
                      }
                    },
              child: isLoading
                  ? const SizedBox(
                      width: 20,
                      height: 20,
                      child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
                    )
                  : const Text('确定'),
            ),
          ],
        ),
      ),
    );
  }

  void _loadCurrentTab() {
    switch (_tabController.index) {
      case 0:
        _loadMyRecords();
        break;
      case 1:
        if (_canApprove) _loadPendingRecords();
        break;
      case 2:
        if (_canReadAll) _loadAllRecords();
        break;
      case 3:
        if (_canApprove) _loadOverdueRecords();
        break;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('设备借还管理'),
        bottom: TabBar(
          controller: _tabController,
          indicatorColor: Colors.white,
          labelColor: Colors.white,
          unselectedLabelColor: Colors.white70,
          tabs: [
            const Tab(text: '我的申请'),
            if (_canApprove) const Tab(text: '待审批'),
            if (_canReadAll) const Tab(text: '全部记录'),
            if (_canApprove) const Tab(text: '逾期清单'),
          ],
        ),
      ),
      body: TabBarView(
        controller: _tabController,
        children: [
          _MyApplyTab(
            records: _myRecords,
            loading: _myLoading,
            myStatus: _myStatus,
            onStatusChanged: (v) => _myStatus = v ?? '',
            onSearch: _loadMyRecords,
            onShowDetail: _showDetailDialog,
            onRenew: _showRenewDialog,
            onReturn: _showReturnDialog,
            getStatusColor: _getStatusColor,
          ),
          if (_canApprove)
            _PendingTab(
              records: _pendingRecords,
              loading: _pendingLoading,
              onShowDetail: _showDetailDialog,
              onApprove: _showApprovalDialog,
              getStatusColor: _getStatusColor,
            ),
          if (_canReadAll)
            _AllRecordsTab(
              records: _allRecords,
              loading: _allLoading,
              keyword: _allKeyword,
              allStatus: _allStatus,
              canDelete: _canDelete,
              onKeywordChanged: (v) => _allKeyword = v,
              onStatusChanged: (v) => _allStatus = v ?? '',
              onSearch: _loadAllRecords,
              onShowDetail: _showDetailDialog,
              onDelete: (r) => _deleteRecord(r),
              getStatusColor: _getStatusColor,
            ),
          if (_canApprove)
            _OverdueTab(
              records: _overdueRecords,
              loading: _overdueLoading,
              onShowDetail: _showDetailDialog,
              getStatusColor: _getStatusColor,
            ),
        ],
      ),
    );
  }

  Future<void> _deleteRecord(BorrowRecord record) async {
    final ok = await ConfirmDialog.show(
      context,
      title: '删除确认',
      message: '确定要删除借还记录「${record.recordNo ?? record.id}」吗？此操作不可撤销。',
      confirmLabel: '删除',
      isDanger: true,
    );
    if (!ok) return;
    try {
      final ok2 = await BorrowRecordService().deleteRecord(record.id);
      if (ok2) {
        Get.snackbar('成功', '记录已删除', snackPosition: SnackPosition.TOP);
        _loadCurrentTab();
      }
    } catch (_) {
      Get.snackbar('错误', '删除失败', snackPosition: SnackPosition.TOP);
    }
  }
}

// ---------- 我的申请 Tab ----------
class _MyApplyTab extends StatelessWidget {
  final List<BorrowRecord> records;
  final bool loading;
  final String myStatus;
  final ValueChanged<String?> onStatusChanged;
  final VoidCallback onSearch;
  final void Function(BorrowRecord) onShowDetail;
  final void Function(BorrowRecord) onRenew;
  final void Function(BorrowRecord) onReturn;
  final Color Function(String?) getStatusColor;

  const _MyApplyTab({
    required this.records,
    required this.loading,
    required this.myStatus,
    required this.onStatusChanged,
    required this.onSearch,
    required this.onShowDetail,
    required this.onRenew,
    required this.onReturn,
    required this.getStatusColor,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Padding(
          padding: const EdgeInsets.all(12),
          child: Row(
            children: [
              Expanded(
                child: DropdownButtonFormField<String>(
                  value: myStatus.isEmpty ? null : myStatus,
                  decoration: InputDecoration(
                    hintText: '状态筛选',
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
                    const DropdownMenuItem(value: '', child: Text('全部')),
                    ...BORROW_STATUSES.map((s) => DropdownMenuItem(value: s, child: Text(s))),
                  ],
                  onChanged: (v) {
                    onStatusChanged(v);
                    onSearch();
                  },
                ),
              ),
              const SizedBox(width: 8),
              ElevatedButton(
                onPressed: onSearch,
                child: const Text('查询'),
              ),
            ],
          ),
        ),
        Expanded(
          child: _buildList(),
        ),
      ],
    );
  }

  Widget _buildList() {
    if (loading) return const Center(child: CircularProgressIndicator());
    if (records.isEmpty) {
      return const EmptyState(icon: Icons.receipt_long, message: '暂无申请记录');
    }
    return RefreshIndicator(
      onRefresh: () async => onSearch(),
      child: ListView.builder(
        padding: const EdgeInsets.symmetric(horizontal: 12),
        itemCount: records.length,
        itemBuilder: (_, i) => _BorrowRecordCard(
          record: records[i],
          showActions: true,
          onShowDetail: onShowDetail,
          onRenew: onRenew,
          onReturn: onReturn,
          getStatusColor: getStatusColor,
        ),
      ),
    );
  }
}

// ---------- 待审批 Tab ----------
class _PendingTab extends StatelessWidget {
  final List<BorrowRecord> records;
  final bool loading;
  final void Function(BorrowRecord) onShowDetail;
  final void Function(BorrowRecord) onApprove;
  final Color Function(String?) getStatusColor;

  const _PendingTab({
    required this.records,
    required this.loading,
    required this.onShowDetail,
    required this.onApprove,
    required this.getStatusColor,
  });

  @override
  Widget build(BuildContext context) {
    if (loading) return const Center(child: CircularProgressIndicator());
    if (records.isEmpty) {
      return const EmptyState(icon: Icons.pending_actions, message: '暂无待审批记录');
    }
    return RefreshIndicator(
      onRefresh: () async {},
      child: ListView.builder(
        padding: const EdgeInsets.all(12),
        itemCount: records.length,
        itemBuilder: (_, i) => _BorrowRecordCard(
          record: records[i],
          showApproveActions: true,
          onShowDetail: onShowDetail,
          onApprove: onApprove,
          getStatusColor: getStatusColor,
        ),
      ),
    );
  }
}

// ---------- 全部记录 Tab ----------
class _AllRecordsTab extends StatelessWidget {
  final List<BorrowRecord> records;
  final bool loading;
  final String keyword;
  final String allStatus;
  final bool canDelete;
  final ValueChanged<String> onKeywordChanged;
  final ValueChanged<String?> onStatusChanged;
  final VoidCallback onSearch;
  final void Function(BorrowRecord) onShowDetail;
  final void Function(BorrowRecord) onDelete;
  final Color Function(String?) getStatusColor;

  const _AllRecordsTab({
    required this.records,
    required this.loading,
    required this.keyword,
    required this.allStatus,
    required this.canDelete,
    required this.onKeywordChanged,
    required this.onStatusChanged,
    required this.onSearch,
    required this.onShowDetail,
    required this.onDelete,
    required this.getStatusColor,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Padding(
          padding: const EdgeInsets.all(12),
          child: Column(
            children: [
              SearchBarWidget(
                hintText: '搜索单号/设备/申请人...',
                onChanged: onKeywordChanged,
                onClear: () {
                  onKeywordChanged('');
                  onSearch();
                },
              ),
              const SizedBox(height: 8),
              Row(
                children: [
                  Expanded(
                    child: DropdownButtonFormField<String>(
                      value: allStatus.isEmpty ? null : allStatus,
                      decoration: InputDecoration(
                        hintText: '状态筛选',
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
                        const DropdownMenuItem(value: '', child: Text('全部')),
                        ...BORROW_STATUSES.map((s) => DropdownMenuItem(value: s, child: Text(s))),
                      ],
                      onChanged: (v) {
                        onStatusChanged(v);
                        onSearch();
                      },
                    ),
                  ),
                  const SizedBox(width: 8),
                  ElevatedButton(
                    onPressed: onSearch,
                    child: const Text('查询'),
                  ),
                ],
              ),
            ],
          ),
        ),
        Expanded(child: _buildList()),
      ],
    );
  }

  Widget _buildList() {
    if (loading) return const Center(child: CircularProgressIndicator());
    if (records.isEmpty) {
      return const EmptyState(icon: Icons.receipt, message: '暂无记录');
    }
    return RefreshIndicator(
      onRefresh: () async => onSearch(),
      child: ListView.builder(
        padding: const EdgeInsets.symmetric(horizontal: 12),
        itemCount: records.length,
        itemBuilder: (_, i) => _BorrowRecordCard(
          record: records[i],
          showDeleteAction: canDelete,
          onShowDetail: onShowDetail,
          onDelete: onDelete,
          getStatusColor: getStatusColor,
        ),
      ),
    );
  }
}

// ---------- 逾期清单 Tab ----------
class _OverdueTab extends StatelessWidget {
  final List<BorrowRecord> records;
  final bool loading;
  final void Function(BorrowRecord) onShowDetail;
  final Color Function(String?) getStatusColor;

  const _OverdueTab({
    required this.records,
    required this.loading,
    required this.onShowDetail,
    required this.getStatusColor,
  });

  @override
  Widget build(BuildContext context) {
    if (loading) return const Center(child: CircularProgressIndicator());
    if (records.isEmpty) {
      return const EmptyState(icon: Icons.check_circle_outline, message: '暂无逾期记录');
    }
    return RefreshIndicator(
      onRefresh: () async {},
      child: ListView.builder(
        padding: const EdgeInsets.all(12),
        itemCount: records.length,
        itemBuilder: (_, i) => _BorrowRecordCard(
          record: records[i],
          showDaysOverdue: true,
          onShowDetail: onShowDetail,
          getStatusColor: getStatusColor,
        ),
      ),
    );
  }
}

// ---------- 借还记录卡片组件 ----------
class _BorrowRecordCard extends StatelessWidget {
  final BorrowRecord record;
  final bool showActions;
  final bool showApproveActions;
  final bool showDeleteAction;
  final bool showDaysOverdue;
  final void Function(BorrowRecord) onShowDetail;
  final void Function(BorrowRecord)? onRenew;
  final void Function(BorrowRecord)? onReturn;
  final void Function(BorrowRecord)? onApprove;
  final void Function(BorrowRecord)? onDelete;
  final Color Function(String?) getStatusColor;

  const _BorrowRecordCard({
    required this.record,
    this.showActions = false,
    this.showApproveActions = false,
    this.showDeleteAction = false,
    this.showDaysOverdue = false,
    required this.onShowDetail,
    this.onRenew,
    this.onReturn,
    this.onApprove,
    this.onDelete,
    required this.getStatusColor,
  });

  String _formatDate(String? d) => d != null && d.length >= 10 ? d.substring(0, 10) : '-';

  @override
  Widget build(BuildContext context) {
    final statusColor = getStatusColor(record.status);
    final isBorrowedOrOverdue = record.status == '已借出' || record.status == '已逾期';
    final canRenewOrReturn = ['已借出', '已逾期'].contains(record.status);
    final canApproveAction = ['待老师审批', '续借审批中', '待管理员审批', '管理员审批中'].contains(record.status);
    final daysOverdue = _calcDaysOverdue();

    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: InkWell(
        onTap: () => onShowDetail(record),
        borderRadius: BorderRadius.circular(12),
        child: Padding(
          padding: const EdgeInsets.all(12),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Container(
                    padding: const EdgeInsets.all(8),
                    decoration: BoxDecoration(
                      color: AppTheme.primary.withOpacity(0.1),
                      borderRadius: BorderRadius.circular(8),
                    ),
                    child: const Icon(Icons.receipt, color: AppTheme.primary, size: 20),
                  ),
                  const SizedBox(width: 10),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          record.equipmentName ?? '-',
                          style: const TextStyle(
                            fontWeight: FontWeight.bold,
                            fontSize: 14,
                          ),
                          overflow: TextOverflow.ellipsis,
                        ),
                        if (record.recordNo != null)
                          Text(
                            '单号: ${record.recordNo}',
                            style: const TextStyle(
                              fontSize: 11,
                              color: AppTheme.textSecondary,
                            ),
                          ),
                      ],
                    ),
                  ),
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.end,
                    children: [
                      Container(
                        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                        decoration: BoxDecoration(
                          color: statusColor.withOpacity(0.12),
                          borderRadius: BorderRadius.circular(6),
                        ),
                        child: Text(
                          record.status ?? '-',
                          style: TextStyle(
                            fontSize: 11,
                            color: statusColor,
                            fontWeight: FontWeight.w500,
                          ),
                        ),
                      ),
                      if (showDaysOverdue && daysOverdue > 0) ...[
                        const SizedBox(height: 4),
                        Container(
                          padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                          decoration: BoxDecoration(
                            color: AppTheme.error.withOpacity(0.1),
                            borderRadius: BorderRadius.circular(4),
                          ),
                          child: Text(
                            '逾期${daysOverdue}天',
                            style: const TextStyle(
                              fontSize: 10,
                              color: AppTheme.error,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                        ),
                      ],
                    ],
                  ),
                ],
              ),
              const SizedBox(height: 8),
              // 借用日期行
              Row(
                children: [
                  _DateChip(label: '借出', date: _formatDate(record.borrowDate)),
                  const SizedBox(width: 8),
                  _DateChip(label: '计划归还', date: _formatDate(record.expectedReturnDate)),
                  if (record.actualReturnDate != null) ...[
                    const SizedBox(width: 8),
                    _DateChip(label: '实际归还', date: _formatDate(record.actualReturnDate)),
                  ],
                ],
              ),
              const SizedBox(height: 6),
              if (record.purpose != null)
                Text(
                  '用途: ${record.purpose}',
                  style: const TextStyle(fontSize: 12, color: AppTheme.textSecondary),
                  overflow: TextOverflow.ellipsis,
                  maxLines: 1,
                ),
              // 操作按钮
              if (showActions || showApproveActions || showDeleteAction) ...[
                const Divider(height: 16),
                Wrap(
                  spacing: 8,
                  runSpacing: 4,
                  children: [
                    _ActionButton(
                      icon: Icons.visibility,
                      label: '详情',
                      color: AppTheme.info,
                      onTap: () => onShowDetail(record),
                    ),
                    if (showActions && canRenewOrReturn && onRenew != null)
                      _ActionButton(
                        icon: Icons.refresh,
                        label: '续借',
                        color: AppTheme.warning,
                        onTap: () => onRenew!(record),
                      ),
                    if (showActions && canRenewOrReturn && onReturn != null)
                      _ActionButton(
                        icon: Icons.keyboard_return,
                        label: '归还',
                        color: AppTheme.success,
                        onTap: () => onReturn!(record),
                      ),
                    if (showApproveActions && canApproveAction && onApprove != null)
                      _ActionButton(
                        icon: Icons.check_circle,
                        label: '批准',
                        color: AppTheme.success,
                        onTap: () => onApprove!(record),
                      ),
                    if (showApproveActions && canApproveAction && onApprove != null)
                      _ActionButton(
                        icon: Icons.cancel,
                        label: '拒绝',
                        color: AppTheme.error,
                        onTap: () => onApprove!(record),
                      ),
                    if (showDeleteAction &&
                        (record.status == '已归还' || record.status == '已拒绝') &&
                        onDelete != null)
                      _ActionButton(
                        icon: Icons.delete,
                        label: '删除',
                        color: AppTheme.error,
                        onTap: () => onDelete!(record),
                      ),
                  ],
                ),
              ],
            ],
          ),
        ),
      ),
    );
  }

  int _calcDaysOverdue() {
    if (record.expectedReturnDate == null) return 0;
    try {
      final expected = DateTime.parse(record.expectedReturnDate!.substring(0, 10));
      final now = DateTime.now();
      final diff = now.difference(expected).inDays;
      return diff > 0 ? diff : 0;
    } catch (_) {
      return 0;
    }
  }
}

class _DateChip extends StatelessWidget {
  final String label;
  final String date;

  const _DateChip({required this.label, required this.date});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
      decoration: BoxDecoration(
        color: Colors.grey.shade100,
        borderRadius: BorderRadius.circular(6),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(
            '$label: ',
            style: const TextStyle(fontSize: 11, color: AppTheme.textSecondary),
          ),
          Text(
            date,
            style: const TextStyle(fontSize: 11, fontWeight: FontWeight.w500),
          ),
        ],
      ),
    );
  }
}

class _ActionButton extends StatelessWidget {
  final IconData icon;
  final String label;
  final Color color;
  final VoidCallback onTap;

  const _ActionButton({
    required this.icon,
    required this.label,
    required this.color,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(6),
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(icon, size: 16, color: color),
            const SizedBox(width: 4),
            Text(
              label,
              style: TextStyle(fontSize: 12, color: color, fontWeight: FontWeight.w500),
            ),
          ],
        ),
      ),
    );
  }
}

// ---------- 详情对话框 ----------
class _DetailDialog extends StatelessWidget {
  final BorrowRecord record;
  final bool canApprove;
  final VoidCallback onApprove;
  final VoidCallback onRenew;
  final VoidCallback onReturn;
  final Color Function(String?) getStatusColor;

  const _DetailDialog({
    required this.record,
    required this.canApprove,
    required this.onApprove,
    required this.onRenew,
    required this.onReturn,
    required this.getStatusColor,
  });

  String _formatDate(String? d) => d != null && d.length >= 10 ? d.substring(0, 10) : '-';

  @override
  Widget build(BuildContext context) {
    final statusColor = getStatusColor(record.status);
    final canRenewOrReturn = ['已借出', '已逾期'].contains(record.status);
    final canApproveAction = ['待老师审批', '续借审批中', '待管理员审批', '管理员审批中'].contains(record.status);

    return AlertDialog(
      title: Row(
        children: [
          const Text('借还记录详情'),
          const Spacer(),
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
            decoration: BoxDecoration(
              color: statusColor.withOpacity(0.12),
              borderRadius: BorderRadius.circular(6),
            ),
            child: Text(
              record.status ?? '-',
              style: TextStyle(fontSize: 12, color: statusColor, fontWeight: FontWeight.w500),
            ),
          ),
        ],
      ),
      content: SingleChildScrollView(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            _DetailRow(label: '单号', value: record.recordNo ?? '-'),
            _DetailRow(label: '设备名称', value: record.equipmentName ?? '-'),
            _DetailRow(label: '申请人', value: record.borrowerName ?? '-'),
            _DetailRow(label: '联系电话', value: record.phone ?? '-'),
            _DetailRow(label: '计划借出', value: _formatDate(record.borrowDate)),
            _DetailRow(label: '计划归还', value: _formatDate(record.expectedReturnDate)),
            _DetailRow(label: '实际借出', value: _formatDate(record.borrowDate)),
            _DetailRow(label: '实际归还', value: _formatDate(record.actualReturnDate)),
            if (record.usageLocation != null)
              _DetailRow(label: '使用地点', value: record.usageLocation!),
            _DetailRow(label: '借用用途', value: record.purpose ?? '-'),
            if (record.remarks != null)
              _DetailRow(label: '备注', value: record.remarks!),
          ],
        ),
      ),
      actions: [
        TextButton(
          onPressed: () => Navigator.pop(context),
          child: const Text('关闭'),
        ),
        if (canRenewOrReturn)
          OutlinedButton(
            onPressed: onRenew,
            child: const Text('续借'),
          ),
        if (canRenewOrReturn)
          ElevatedButton(
            onPressed: onReturn,
            style: ElevatedButton.styleFrom(backgroundColor: AppTheme.success),
            child: const Text('归还'),
          ),
        if (canApprove && canApproveAction)
          ElevatedButton(
            onPressed: onApprove,
            child: const Text('审批'),
          ),
      ],
    );
  }
}

class _DetailRow extends StatelessWidget {
  final String label;
  final String value;

  const _DetailRow({required this.label, required this.value});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 80,
            child: Text(
              '$label:',
              style: const TextStyle(fontSize: 13, color: AppTheme.textSecondary),
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: const TextStyle(fontSize: 13, color: AppTheme.textPrimary),
            ),
          ),
        ],
      ),
    );
  }
}
