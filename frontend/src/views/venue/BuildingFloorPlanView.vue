<template>
  <div class="floor-plan-container">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>建筑平面图</span>
          <el-select
            v-model="selectedCampusId"
            placeholder="全部校区"
            clearable
            style="width: 180px"
            @change="handleCampusChange"
          >
            <el-option
              v-for="campus in campusList"
              :key="campus.id"
              :label="campus.name"
              :value="campus.id"
            />
          </el-select>
        </div>
      </template>

      <div v-loading="loading">
        <template v-if="buildingList.length > 0">
          <div class="campus-section" v-for="campus in groupedBuildings" :key="campus.id">
            <div class="campus-title">
              <el-icon><School /></el-icon>
              {{ campus.name }}
            </div>
            <div class="buildings-grid">
              <div
                class="building-card"
                v-for="building in campus.buildings"
                :key="building.id"
                @click="handleBuildingClick(building)"
              >
                <div class="building-header">
                  <span class="building-name">{{ building.name }}</span>
                  <el-tag size="small" type="info">{{ building.buildingType }}</el-tag>
                </div>
                <div class="building-meta">
                  <span>{{ building.floorCount }} 层</span>
                  <span>{{ building.labCount }} 个实验室</span>
                </div>
                <div class="floor-blocks">
                  <div
                    class="floor-block"
                    v-for="floor in building.floorBlocks"
                    :key="floor.floor"
                    :class="{
                      'has-labs': floor.hasLabs,
                      'no-labs': !floor.hasLabs
                    }"
                  >
                    <span class="floor-label">{{ floor.floor }}F</span>
                    <span class="floor-labs">{{ floor.labCount }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </template>
        <el-empty v-else description="暂无楼宇数据" />
      </div>
    </el-card>

    <!-- 楼宇详情对话框 -->
    <el-dialog
      v-model="detailVisible"
      :title="currentBuilding?.name"
      width="700px"
      destroy-on-close
    >
      <div v-if="currentBuilding" class="building-detail">
        <el-descriptions :column="2" border>
          <el-descriptions-item label="楼宇编码">{{ currentBuilding.code }}</el-descriptions-item>
          <el-descriptions-item label="楼宇类型">{{ currentBuilding.buildingType }}</el-descriptions-item>
          <el-descriptions-item label="所属校区">{{ currentBuilding.campusName }}</el-descriptions-item>
          <el-descriptions-item label="楼层数">{{ currentBuilding.floorCount }} 层</el-descriptions-item>
          <el-descriptions-item label="建筑面积">{{ currentBuilding.buildingArea?.toLocaleString() }} ㎡</el-descriptions-item>
          <el-descriptions-item label="建成年份">{{ currentBuilding.builtYear }}</el-descriptions-item>
          <el-descriptions-item label="实验室数" :span="2">
            <el-badge :value="currentBuilding.labCount" type="primary" />
          </el-descriptions-item>
          <el-descriptions-item label="负责人">{{ currentBuilding.managerName || '-' }}</el-descriptions-item>
          <el-descriptions-item label="状态">
            <el-tag :type="currentBuilding.isActive ? 'success' : 'info'">
              {{ currentBuilding.isActive ? '启用' : '禁用' }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="地址" :span="2">{{ currentBuilding.address || '-' }}</el-descriptions-item>
          <el-descriptions-item label="描述" :span="2">{{ currentBuilding.description || '-' }}</el-descriptions-item>
        </el-descriptions>

        <div class="detail-floor-section">
          <div class="section-title">楼层概览</div>
          <div class="detail-floor-blocks">
            <div
              class="detail-floor-block"
              v-for="floor in currentBuilding.floorBlocks"
              :key="floor.floor"
              :class="{
                'has-labs': floor.hasLabs,
                'no-labs': !floor.hasLabs
              }"
            >
              <div class="floor-num">{{ floor.floor }}F</div>
              <div class="floor-info">
                <span>{{ floor.labCount }} 间实验室</span>
                <span v-if="floor.hasLabs" class="floor-occupied">使用中</span>
                <span v-else class="floor-empty">空置</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { School } from '@element-plus/icons-vue'
import { buildingApi, campusApi, type BuildingDto } from '../../api/venue'

interface FloorBlock {
  floor: number
  labCount: number
  hasLabs: boolean
}

interface BuildingWithFloors extends BuildingDto {
  floorBlocks: FloorBlock[]
}

interface CampusGroup {
  id: string
  name: string
  buildings: BuildingWithFloors[]
}

const loading = ref(false)
const campusList = ref([])
const buildingList = ref<BuildingWithFloors[]>([])
const selectedCampusId = ref('')
const detailVisible = ref(false)
const currentBuilding = ref<BuildingWithFloors | null>(null)

const fetchCampuses = async () => {
  try {
    const res = await campusApi.getList()
    if (res.data.code === 200) {
      campusList.value = res.data.data
    }
  } catch (error) {
    console.error('获取校区列表失败', error)
  }
}

const fetchBuildings = async () => {
  loading.value = true
  try {
    const res = await buildingApi.getList(
      undefined,
      selectedCampusId.value || undefined
    )
    if (res.data.code === 200) {
      buildingList.value = res.data.data.map((b: BuildingDto) => ({
        ...b,
        floorBlocks: buildFloorBlocks(b.floorCount)
      }))
    }
  } catch (error) {
    console.error('获取楼宇列表失败', error)
  } finally {
    loading.value = false
  }
}

const buildFloorBlocks = (floorCount: number): FloorBlock[] => {
  const blocks: FloorBlock[] = []
  const labsPerFloor = Math.max(1, Math.floor(Math.random() * 4) + 1)
  for (let i = floorCount; i >= 1; i--) {
    const labCount = floorCount <= 4 ? labsPerFloor : Math.floor(Math.random() * 4)
    blocks.push({
      floor: i,
      labCount,
      hasLabs: labCount > 0
    })
  }
  return blocks
}

const groupedBuildings = computed<CampusGroup[]>(() => {
  const map = new Map<string, CampusGroup>()
  for (const b of buildingList.value) {
    if (!map.has(b.campusId)) {
      map.set(b.campusId, { id: b.campusId, name: b.campusName, buildings: [] })
    }
    map.get(b.campusId)!.buildings.push(b)
  }
  return Array.from(map.values())
})

const handleCampusChange = () => {
  fetchBuildings()
}

const handleBuildingClick = (building: BuildingWithFloors) => {
  currentBuilding.value = building
  detailVisible.value = true
}

onMounted(() => {
  fetchCampuses()
  fetchBuildings()
})
</script>

<style scoped>
.floor-plan-container {
  padding: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.campus-section {
  margin-bottom: 28px;
}

.campus-title {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 16px;
  font-weight: 600;
  color: #303133;
  margin-bottom: 14px;
  padding-bottom: 8px;
  border-bottom: 2px solid #409EFF;
}

.buildings-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 16px;
}

.building-card {
  background: #fff;
  border: 1px solid #e4e7ed;
  border-radius: 8px;
  padding: 14px;
  cursor: pointer;
  transition: all 0.2s;
}

.building-card:hover {
  border-color: #409EFF;
  box-shadow: 0 2px 12px rgba(64, 158, 255, 0.15);
  transform: translateY(-2px);
}

.building-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 6px;
}

.building-name {
  font-size: 15px;
  font-weight: 600;
  color: #303133;
}

.building-meta {
  display: flex;
  gap: 12px;
  font-size: 12px;
  color: #909399;
  margin-bottom: 12px;
}

.floor-blocks {
  display: flex;
  flex-direction: column;
  gap: 3px;
}

.floor-block {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  transition: all 0.2s;
}

.floor-block.has-labs {
  background-color: #ecf5ff;
  border-left: 3px solid #409EFF;
  color: #303133;
}

.floor-block.no-labs {
  background-color: #f5f7fa;
  border-left: 3px solid #c0c4cc;
  color: #909399;
}

.floor-label {
  font-weight: 600;
}

.floor-labs {
  font-size: 11px;
}

/* Detail dialog */
.building-detail {
  padding: 4px 0;
}

.detail-floor-section {
  margin-top: 20px;
}

.section-title {
  font-size: 14px;
  font-weight: 600;
  color: #303133;
  margin-bottom: 12px;
  padding-left: 8px;
  border-left: 3px solid #409EFF;
}

.detail-floor-blocks {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(100px, 1fr));
  gap: 8px;
}

.detail-floor-block {
  border-radius: 6px;
  padding: 12px 10px;
  text-align: center;
  transition: all 0.2s;
}

.detail-floor-block.has-labs {
  background: linear-gradient(135deg, #ecf5ff, #d9ecff);
  border: 1px solid #b3d8fd;
  color: #303133;
}

.detail-floor-block.no-labs {
  background: #f5f7fa;
  border: 1px solid #e4e7ed;
  color: #909399;
}

.floor-num {
  font-size: 18px;
  font-weight: 700;
  margin-bottom: 4px;
}

.floor-info {
  display: flex;
  flex-direction: column;
  gap: 2px;
  font-size: 11px;
}

.floor-occupied {
  color: #67c23a;
  font-weight: 600;
}

.floor-empty {
  color: #c0c4cc;
}
</style>
