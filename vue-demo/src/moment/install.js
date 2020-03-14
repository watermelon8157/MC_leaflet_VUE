import moment from 'moment'
import 'moment/locale/zh-tw'
moment.locale('zh-tw')

export default {
  install (Vue, options) {
    Vue.prototype.$moment = moment
  }
}
