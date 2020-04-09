// 導入模組
import apiList from '@/controllers/apiList'

const install = function (Vue) {
  if (install.installed) return
  install.installed = true
  Object.defineProperties(Vue.prototype, {
    $api: {
      get () {
        return apiList
      },
      enumerable: false,
      configurable: false // 不可重寫
    }
  })
}

export default {
  install
}
