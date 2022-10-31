import { createApp } from 'vue'

import router from './router'
import store from './store'
import axios from "axios";
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import * as ElementPlusIconsVue from '@element-plus/icons-vue'
import App from './App.vue'
axios.defaults.baseURL = "https://localhost:7012"; // 服务端地址
 

const app = createApp(App);
// 全局挂载axios
app.config.globalProperties.$http = axios;

for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
    app.component(key, component)
  }
app.use(store).use(router).use(ElementPlus).mount('#app')
