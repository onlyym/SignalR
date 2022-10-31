<template>
  <el-form :model="form" label-width="120px">
    <el-form-item label="Activity name">
      <el-input v-model="form.username" />
    </el-form-item>
    <el-form-item label="Activity password">
      <el-input v-model="form.password" />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="onSubmit">登录</el-button>
      <el-button @click="onConcel">重置</el-button>
    </el-form-item>
  </el-form>
</template>

<script lang="ts" >
import { reactive, getCurrentInstance } from "vue";
import {post,get} from '../service/service'
import {useRouter} from 'vue-router'

export default {
  name: "aboutView",
  setup() {
    // 使用axios
    const internalInstance = getCurrentInstance();
    const axios = internalInstance?.appContext.config.globalProperties.$http;

    //router
    const router = useRouter();
    // do not use same name with ref
    const form = reactive({
      username: 'admin',
      password: '123456',
    })

    const  onSubmit = () => {
      const option = {params:form,url:'/api/Login/Login'}
     
     const res =  get(option);
     
      console.log(res)
      console.log('submit!')
    }
    const onConcel = () => {
      console.log(666)
      form.username = '';
      form.password = '';
    }
    return {
      form,
      onConcel,
      onSubmit,
    }
  }
}

</script>