// 導入模組
// import Basic from '@/controllers/Basic'
import moment from 'moment'
import DB_MC_PATIENT_INFO from '@/RCS_Data.Models.DB.DB_MC_PATIENT_INFO.json'
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
    PatModel: Object.assign(DB_MC_PATIENT_INFO, {}),
    PatListnow: []
  },
  mutations: {
    SetPatModel (state, payload) {
      state.PatModel = payload.data
    },
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
      for (let index = 0; index < payload.data.length; index++) {
        if (payload.data[index].EXPECTED_ARRIVAL_DATETIME) {
          payload.data[index].jsEXPECTED_ARRIVAL_DATETIME = new Date(
            payload.data[index].EXPECTED_ARRIVAL_DATETIME
          ).getTime()
        }
        if (payload.data[index].SELECTION_DATETIME) {
          payload.data[index].jsSELECTION_DATETIME = new Date(
            payload.data[index].SELECTION_DATETIME
          ).getTime()
        }
        if (payload.data[index].CREATE_DATE) {
          payload.data[index].jsCREATE_DATE = new Date(
            payload.data[index].CREATE_DATE
          ).getTime()
        }
      }
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
