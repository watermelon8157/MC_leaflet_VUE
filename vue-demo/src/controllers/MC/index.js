import api from '@/controllers/index'
import config from '@/controllers/config'
import DB_MC_PATIENT_INFO from '@/RCS_Data.Models.DB.DB_MC_PATIENT_INFO.json'
const baseUrl = config.localhost + 'api/MC/'

export default {
  getMC_SITE_INFO (params) {
    let _params = {}
    let setConfig = {
      url: baseUrl + 'getMC_SITE_INFO',
      params: Object.assign(_params, params)
    }
    return api.get(setConfig.url, setConfig.params)
  },
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
  hospLogin (params) {
    let _params = {
      userName: '',
      password: '',
      site_id: '',
      hosp_id: '',
      LATITUDE: '',
      LONGITUDE: ''
    }
    let setConfig = {
      url: baseUrl + 'hospLogin',
      params: Object.assign(_params, params)
    }
    return api.post(setConfig.url, setConfig.params)
  },
  SiteLogin (params) {
    let _params = {
      userName: '',
      password: '',
      site_id: '',
      site_desc: '',
      hosp_id: '',
      LATITUDE: '',
      LONGITUDE: ''
    }
    let setConfig = {
      url: baseUrl + 'SiteLogin',
      params: Object.assign(_params, params)
    }
    return api.post(setConfig.url, setConfig.params)
  },
  SiteLoginNew (params) {
    let _params = {
      userName: '',
      password: '',
      site_id: '',
      site_desc: '',
      hosp_id: '',
      LATITUDE: '',
      LONGITUDE: ''
    }
    let setConfig = {
      url: baseUrl + 'SiteLoginNew',
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
  GetPatList (params) {
    let _params = {}
    let setConfig = {
      url: baseUrl + 'GetPatList',
      params: Object.assign(_params, params)
    }
    return api.post(setConfig.url, setConfig.params)
  },
  GetHospList (params) {
    let _params = {}
    let setConfig = {
      url: baseUrl + 'GetHospList',
      params: Object.assign(_params, params)
    }
    return api.post(setConfig.url, setConfig.params)
  },
  GetHospListDTLByID (params) {
    let _params = { site_id: '' }
    let setConfig = {
      url: baseUrl + 'GetHospListDTLByID',
      params: Object.assign(_params, params)
    }
    return api.post(setConfig.url, setConfig.params)
  },
  GetPatListAll (params) {
    let _params = {}
    let setConfig = {
      url: baseUrl + 'GetPatListAll',
      params: Object.assign(_params, params)
    }
    return api.post(setConfig.url, setConfig.params)
  },
  GetPatListByID (params) {
    let _params = { site_id: '', hosp_id: '' }
    let setConfig = {
      url: baseUrl + 'GetPatListByID',
      params: Object.assign(_params, params)
    }
    return api.post(setConfig.url, setConfig.params)
  },
  saveSite (params) {
    let _params = {
      SITE_ID: '',
      SITE_AREA: '',
      SITE_DESC: '',
      CREATE_ID: '',
      CREATE_NAME: '',
      CREATE_DATE: '',
      MODIFY_ID: '',
      MODIFY_NAME: '',
      MODIFY_DATE: '',
      DATASTATUS: '',
      LATITUDE: '',
      LONGITUDE: ''
    }
    let setConfig = {
      url: baseUrl + 'saveSite',
      params: Object.assign(_params, params)
    }
    return api.post(setConfig.url, setConfig.params)
  }
}
