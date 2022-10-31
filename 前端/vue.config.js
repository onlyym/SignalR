const { defineConfig } = require('@vue/cli-service')
module.exports = defineConfig({
  transpileDependencies: true,
  //取消格式化校验
  lintOnSave: false,
})
