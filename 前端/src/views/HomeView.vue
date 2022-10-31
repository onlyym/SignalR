<template>
  <el-container class="map-container">
    <el-header height="45px">
      <el-row :gutter="20">
        <el-col :span="4"><img class="logo" src="../assets/logo.png" /></el-col>
        <el-col :span="16"><h2>测试进度条</h2></el-col>
        <el-col :span="4"></el-col>
      </el-row>
    </el-header>
    <el-container>
      <el-container class="main-container">
        <el-main>
          <el-steps :active="stepnum" align-center space="420px">
            <el-step title="上传图片" :icon="Edit" />
            <el-step title="处理中..." :icon="Upload" />
            <el-step title="处理完成" :icon="Picture" />
          </el-steps>
          <div class="demo-progress">
            <el-progress
              type="circle"
              :percentage="progressUpload"
              :status="finishedUpload"
            />
            <el-progress
              type="circle"
              :percentage="progressHandle"
              :status="finishedHandle"
            />
            <el-progress type="circle" :percentage="completeHandle" :status="finishedComplete" />
          </div>
          <div class="progress">
            <el-button type="primary" @click="StartProgress"
              >开始执行</el-button
            >
            <br />
            <el-tag v-if="isShow" effect="dark" type="success" class="result"
              >执行结果: {{ message }}</el-tag
            >
          </div>
        </el-main>
      </el-container>
    </el-container>
  </el-container>
</template>
<script lang="ts">
import { ref, defineComponent, reactive, getCurrentInstance } from "vue";
import { ElMessage } from "element-plus";
import { Edit, Picture, Upload } from "@element-plus/icons-vue";
import * as signalR from "@microsoft/signalr";
 
export default defineComponent({
  name: "HomeView",
  setup() {
    let progressUpload = ref(0);
    let progressHandle = ref(0);
    let completeHandle = ref(0);
    let stepnum = ref(0);
 
    let message = ref("");
    let isShow = ref(false);
    let hub = reactive({
      connection: {},
      HubConnId: "",
      resultInfo: {},
    });
    let finishedUpload = ref("");
    let finishedHandle = ref("");
    let finishedComplete = ref("");
    // #region
    // 与服务器建立连接
    /**
     * 连接有2种方式，第一种是协议协商，第二种是禁止协商，霸王硬上弓
     * 协议协商在分布式项目不适用，因为无法保证这一次连接的和上一次连接的服务器是同一台
     */
    //正常协商连接
    const connectionServer = (hub.connection =
      new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7012/progressHub")
        .withAutomaticReconnect()
        .build());
    //禁止协商 直接连接,指定为websockets
    //  const options = {
    //   skipNegotiation:true,
    //   transport:signalR.HttpTransportType.WebSockets
    //  }
    //  const connectionServer = (hub.connection =
    //   new signalR.HubConnectionBuilder()
    //     .withUrl("https://localhost:7012/progressHub",options)
    //     .withAutomaticReconnect()
    //     .build());   
    connectionServer
      .start()
      .then(() => {
        ElMessage({
          message: "服务器连接成功了",
          type: "success",
        });
      })
      .catch((error) => {
        ElMessage.error(`服务器连接失败了\r${error.message.toString()}`);
      });
    connectionServer.onclose((error) => {
      ElMessage.error(`服务器疑似断开了\r${error?.message.toString()}`);
    });
 
    connectionServer.on("SetHubConnId", (id) => {
      hub.HubConnId = id;
      console.log(`id为 ${id} 的客户端已登录`)
    });
    connectionServer.on("UpdProgress", (percent) => {
      progressUpload.value = percent;
    });
 
    connectionServer.on("HandleProgress", (percent) => {
      progressHandle.value = percent;
    });
    // 使用axios
    const internalInstance = getCurrentInstance();
    const axios = internalInstance?.appContext.config.globalProperties.$http;
 
    async function StartProgress() {

      ElMessage({
        message: "开始上传图片",
        type: "success",
      });
      isShow.value = true;
      stepnum.value=0;
      stepnum.value =stepnum.value+1 ;

      progressUpload.value = 0;
      progressHandle.value = 0;
      completeHandle.value = 0;
      finishedUpload.value = "";
      finishedHandle.value = "";
      finishedComplete.value = "";
      message.value = "正在执行中...";
      debugger
      const { data: res } = await axios.post("api/progress", hub.resultInfo);
      if (res.status !== 200 + "") {
        finishedUpload.value = "exception";
        return ElMessage.error("上传图片失败了, 请重试");
      }
      finishedUpload.value = "success";
      message.value = res.ResultMsg;
      stepnum.value =stepnum.value+1 ;
      const res1 = await axios.get("api/progress");
      console.log(res1);
      if (res1.status !== 202) {
        finishedHandle.value = "exception";
        return ElMessage.error("图片处理失败了, 请重试");
      }
      finishedHandle.value = "success";
      stepnum.value =stepnum.value+1 ;
      message.value = "处理已完成";
      completeHandle.value = 100;
      finishedComplete.value = "success";
    }
    // #endregion
 
    return {
      progressUpload,
      progressHandle,
      completeHandle,
      message,
      isShow,
      hub,
      finishedUpload,
      finishedHandle,
      finishedComplete,
      Edit,
      Picture,
      Upload,
      StartProgress,
      stepnum,
    };
  },
});
</script>
<style  scoped>
.map-container {
  height: 100%;
}
.logo {
  height: 40px;
}
h2 {
  margin-left: 12px;
  letter-spacing: 0.06em;
  color: #fff;
  font-size: 20px;
  height: 40px;
  line-height: 40px;
}
.el-header {
  background-color: rgb(26, 54, 82);
  display: flex;
  justify-content: space-between;
  padding-left: 0;
  font-size: 22px;
  align-items: center;
}
.main-container {
  background-color: #eee;
}
.demo-progress {
  display: flex;
  justify-content: center;
  margin-left: 150px;
}
.demo-progress .el-progress--line {
  margin-bottom: 15px;
  width: 500px;
}
.demo-progress .el-progress--circle {
  margin-right: 288px;
}
.el-steps {
  padding-bottom: 20px;
}
:deep(.el-step__icon.is-icon) {
  width: 40px;
  background-color: #eee;
}
.progress {
  margin: 3% 12%;
 
  .result {
    margin-top: 20px;
    font-size: 14px;
  }
}
</style>