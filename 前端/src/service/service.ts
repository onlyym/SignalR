import axios from 'axios'
import { ElLoading, ElMessage } from 'element-plus'
let loading:any = null;
//使用create创建axios实例
const Service = axios.create({
    timeout: 8000,
    baseURL: "https://localhost:7012",
    headers: {
        "Content-Type": "application/json;charset:utf-8;"
    }
})


//请求拦截 -  对请求做统一处理，增加loading
Service.interceptors.request.use(config => {
    loading = ElLoading.service({
        lock: true,
        text: 'Loading',
        background: 'rgba(0, 0, 0, 0.7)',
    })
    return config
})


//响应拦截 - 对返回值做统一处理
Service.interceptors.response.use(res => {
    loading.close();
    
    const data = res.data;
    if (!data.data) {
        //请求出错
        // 此处弹出错误信息
        // ElMessage.data.msg
    }
    console.log(data,23);
    
    return data
}, error => {
    loading.close();
    ElMessage.error(error)
}
)


// post请求
export const post = (config:any) => {
    return Service({
        ...config,
        method: "post",
        data: config.data
    })
}
// get 请求
export const get = (config:any) => {
    return Service({
        ...config,
        method: "get",
    })
}