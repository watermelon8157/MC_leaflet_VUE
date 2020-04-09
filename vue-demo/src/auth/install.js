// 導入模組
import auth from '@/auth/index'

const install = function (Vue) {
  if (install.installed) return
  install.installed = true
  Object.defineProperties(Vue.prototype, {
    $auth: {
      get () {
        return auth
      },
      enumerable: false,
      configurable: false // 不可重寫
    }
  })
}

export default {
  install
}
