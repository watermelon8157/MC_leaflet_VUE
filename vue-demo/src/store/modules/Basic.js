// 導入模組
// import Basic from '@/controllers/Basic'
import moment from 'moment'
import 'moment/locale/zh-tw'
moment.locale('zh-tw')

export default {
  namespaced: true,
  state: {
    windowWidth: document.documentElement.clientWidth,
    windowHeight: document.documentElement.clientHeight,
    loadFirst: true,
    PatListAll: [],
    PatListnow: []
  },
  mutations: {
    SetPatListnow (state, payload) {
      state.PatListnow = payload.data
    },
    SetPatListAll (state, payload) {
      state.PatListAll = payload.data
    },
    SetLoadFirst (state) {
      state.loadFirst = false
    },
    SetWindowWidth (state, payload) {
      state.windowWidth = payload.windowWidth
    },
    SetWindowHeight (state, payload) {
      state.windowHeight = payload.windowHeight
    }
  }
}
