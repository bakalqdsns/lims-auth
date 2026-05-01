import axios from 'axios'
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import * as ElementPlusIconsVue from '@element-plus/icons-vue'

import App from './App.vue'
import router from './router'
import permission from './directives/permission'

// 统一响应格式：自动适配 { code, data } 包装和裸数组两种返回格式
axios.interceptors.response.use(response => {
  if (Array.isArray(response.data)) {
    response.data = { data: response.data }
  }
  return response
}, error => Promise.reject(error))

const app = createApp(App)

// 注册所有图标
for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
  app.component(key, component)
}

app.use(createPinia())
app.use(router)
app.use(ElementPlus)
app.use(permission)

app.mount('#app')
