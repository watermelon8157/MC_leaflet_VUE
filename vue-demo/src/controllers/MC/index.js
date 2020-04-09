import api from '@/controllers/index'
import config from '@/controllers/config'
const baseUrl = config.localhost + '/api/MC/'

export default {
  //  登入
  HelloWord (params) {
    let _params = {
    }
    let setConfig = {
      url: baseUrl + 'HelloWord',
      params: Object.assign(_params, params)
    }
    return api.post(setConfig.url, setConfig.params)
  },
  INSERT_PAT_DATA (params) {
    let _params = {
    }
    let setConfig = {
      url: baseUrl + 'INSERT_PAT_DATA',
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
  }
}
