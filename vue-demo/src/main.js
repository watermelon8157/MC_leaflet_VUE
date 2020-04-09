// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import 'babel-polyfill'
import 'es6-promise/auto'
import Vue from 'vue'
import App from './App'
import router from './router'
import store from '@/store/install' // store
import moment from '@/moment/install' // moment
import 'leaflet/dist/leaflet.css'
import { Icon } from 'leaflet'
import Antd from 'ant-design-vue'
import 'bootstrap-css-only/css/bootstrap.min.css'
import 'ant-design-vue/dist/antd.css'
import 'tailwindcss/dist/base.min.css'
import 'tailwindcss/dist/components.min.css'
import 'tailwindcss/dist/utilities.min.css'
Vue.use(store)
Vue.use(moment)
Vue.use(Antd)
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
/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  components: { App },
  template: '<App/>'
})
