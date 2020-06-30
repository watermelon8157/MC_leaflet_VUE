// 導入模組
// import Basic from '@/controllers/Basic'
import moment from 'moment'
import 'moment/locale/zh-tw'
moment.locale('zh-tw')

export default {
  namespaced: true,
  state: {
    hospList: [],
    hospListDTL: [],
    windowWidth: document.documentElement.clientWidth,
    windowHeight: document.documentElement.clientHeight,
    loadFirst: true,
    PatListAll: [],
    PatListByID: [],
    PatListnow: []
  },
  mutations: {
    SetPatListnow (state, payload) {
      state.PatListnow = payload.data
    },
    SetGetHospList (state, payload) {
      state.hospList = payload.data
    },
    SetGetHospListDTL (state, payload) {
      state.hospListDTL = payload.data
    },
    SetPatListBYID (state, payload) {
      state.PatListByID = payload.data
    },
    pushPatListBYID (state, payload) {
      state.PatListByID.push(payload)
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
