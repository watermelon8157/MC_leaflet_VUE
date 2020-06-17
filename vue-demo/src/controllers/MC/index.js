import api from '@/controllers/index'
import config from '@/controllers/config'
import DB_MC_PATIENT_INFO from '@/RCS_Data.Models.DB.DB_MC_PATIENT_INFO.json'
const baseUrl = config.localhost + 'api/MC/'

export default {
  //  登入
  HelloWord (params) {
    let _params = {}
    let setConfig = {
      url: baseUrl + 'HelloWord',
      params: Object.assign(_params, params)
    }
    return api.post(setConfig.url, setConfig.params)
  },
  INSERT_PAT_DATA (params) {
    let _params = Object.assign(DB_MC_PATIENT_INFO, {})
    let setConfig = {
      url: baseUrl + 'INSERT_PAT_DATA',
      params: Object.assign(_params, params)
    }
    return api.post(setConfig.url, setConfig.params)
  },
  UPDATE_PAT_DATA (params) {
    let _params = Object.assign(DB_MC_PATIENT_INFO, {})
    let setConfig = {
      url: baseUrl + 'UPDATE_PAT_DATA',
      params: Object.assign(_params, params)
    }
    return api.post(setConfig.url, setConfig.params)
  },
  LoginForm (params) {
    let _params = {
      userName: '',
      password: '',
      site_id: '',
      hosp_id: '',
      LATITUDE: '',
      LONGITUDE: ''
    }
    let setConfig = {
      url: baseUrl + 'Login',
      params: Object.assign(_params, params)
    }
    return api.post(setConfig.url, setConfig.params)
  },
  // 是否合法登入
  JwtAuthCheck (params) {
    let _params = {}
    let setConfig = {
      url: baseUrl + 'JwtAuthCheck',
      params: Object.assign(_params, params)
    }
    return api.post(setConfig.url, setConfig.params)
  },
  // 取得照護清單
  GetPatList (params) {
    let _params = {}
    let setConfig = {
      url: baseUrl + 'GetPatList',
      params: Object.assign(_params, params)
    }
    return api.post(setConfig.url, setConfig.params)
  },
  // 取得照護清單
  GetPatLisAll (params) {
    let _params = {}
    let setConfig = {
      url: baseUrl + 'GetPatListAll',
      params: Object.assign(_params, params)
    }
    return api.post(setConfig.url, setConfig.params)
  }
}
