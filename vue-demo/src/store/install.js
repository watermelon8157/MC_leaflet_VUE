// 導入模組
// https://blog.toast38coza.me/make-your-vuex-store-globally-available-by-registering-it-as-a-plugin/
import store from '@/store/index'
import Vuex from 'vuex'

export default {
  install (Vue, options) {
    Vue.use(Vuex)
    Vue.prototype.$store = new Vuex.Store(store)
    const initialStateCopy = JSON.parse(JSON.stringify(store.state))
    Vue.prototype.$ResetStore = function () {
      Vue.prototype.$store.replaceState(JSON.parse(JSON.stringify(initialStateCopy)))
    }
  }
}
