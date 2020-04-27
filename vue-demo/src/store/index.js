
import Basic from './modules/Basic'
const store = {
  modules: {
    Basic: Basic
  },
  state: {
    patList: [],
    selectPatList: [],
    networkError: false, // 網路問題
    spin: false,
    spinMsg: 'Loading...'
  },
  mutations: {
    // this.$store.commit('SetNetworkError', true)  顯示載入中遮罩
    // this.$store.commit('SetNetworkError', false) 關閉載入中遮罩
    SetNetworkError (state, payload) {
      state.networkError = payload
    },
    SetPatList (state, payload) {
      state.patList.push(payload)
    },
    SetSelctPatList (state, payload) {
      for (let index = 0; index < state.patList.length; index++) {
        const element = state.patList[index]
        if (element.PATIENT_ID === payload.PATIENT_ID) {
          state.patList.splice(index, 1)
        }
      }
      state.selectPatList.push(payload)
    },
    // this.$store.commit('SpinLoading', true)  顯示載入中遮罩
    // this.$store.commit('SpinLoading', false) 關閉載入中遮罩
    SpinLoading (state, payload) {
      state.spin = payload
      state.spinMsg = 'Loading...'
    },
    // this.$store.commit('SpinLoading_WithMsg', {loading:true, Msg: '!@#$%^'})  顯示載入中遮罩
    SpinLoading_WithMsg (state, payload) {
      state.spin = payload.loading
      state.spinMsg = payload.Msg
    }
  }
}
export default store
