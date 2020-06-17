// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import 'babel-polyfill'
import 'es6-promise/auto'
import Vue from 'vue'
import App from './App'
import router from './router'
import store from '@/store/install' // store
import Controllers from '@/controllers/install'
import moment from '@/moment/install' // moment
import 'leaflet/dist/leaflet.css'
import { Icon } from 'leaflet'
import Antd from 'ant-design-vue'
import 'bootstrap-css-only/css/bootstrap.min.css'
import 'ant-design-vue/dist/antd.css'
import 'tailwindcss/dist/base.min.css'
import 'tailwindcss/dist/components.min.css'
import 'tailwindcss/dist/utilities.min.css'
import lodash from '@/lodash/install'
import auth from '@/auth/install' //

Vue.use(store)
Vue.use(auth)
Vue.use(Controllers)
Vue.use(moment)
Vue.use(Antd)
Vue.use(lodash)
delete Icon.Default.prototype._getIconUrl
Icon.Default.mergeOptions({
  iconRetinaUrl: require('leaflet/dist/images/marker-icon-2x.png'),
  iconUrl: require('leaflet/dist/images/marker-icon.png'),
  shadowUrl: require('leaflet/dist/images/marker-shadow.png')
})
Vue.config.productionTip = false
Vue.prototype.$notification.config({
  placement: 'topLeft',
  bottom: '50px',
  duration: 3
})

const isDev = process.env.NODE_ENV === 'development'
if (isDev) {
  if (!Vue.prototype.$auth.TokenExist()) {
    // 設定測試機用token
    Vue.prototype.$auth.setToken(
      'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzaXRlX2lkIjoiMTExMSIsInVzZXJfaWQiOiJhYWFhIiwidXNlcl9uYW1lIjoiYWFhYSIsInJvbGUiOm51bGwsImlhdCI6bnVsbH0.C-2ZkCHBkqUeVYzjE-zphsfQeVcCqten5EpBFjNGk2Q'
    ) // 設定開發模式 toekn
  }
}
/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  components: { App },
  template: '<App/>'
})
