import VueLodash from 'vue-lodash'
// https://www.html.cn/doc/lodash/#_droparray-n1
import { head, random, chunk, compact, findIndex } from 'lodash'
export default {
  install (Vue, options) {
    Vue.use(VueLodash, {
      lodash: {
        head, // https://www.html.cn/doc/lodash/#_headarray
        random,
        chunk,
        findIndex,
        compact
      }
    })
  }
}
