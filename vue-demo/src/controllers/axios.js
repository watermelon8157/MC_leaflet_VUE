import axios from 'axios'
import auth from '@/auth/index'
import qs from 'qs'
import Moment from 'moment'
import 'moment/locale/zh-tw'
Moment.locale('zh-tw')
// 建立 axios
let http = axios.create({
  transformRequest: [function (data, headers) {
    // Do whatever you want to transform the data
    headers['Authorization'] = auth.getToken()
    return data
  }],
  timeout: 60000
})

// 添加請求攔截
http.interceptors.request.use(config => {
  if (config.method === 'post') {
    config.data = qs.stringify(Object.assign(config.data, {
      'payload': auth.getPaylaod(),
      'pat_info': auth.getPatientInfo()
    }))
  }
  document.now_time = Moment()
  return config
}, error => {
  // 請求錯誤處理
  return Promise.reject(error)
})

//  添加響應攔截
http.interceptors.response.use(response => {
  let data = response
  data.ok = true
  return data
}, error => {
  let info = {}
  if (!error.response) {
    info = {
      data: 'Network Error'
    }
  } else {
    // 整理錯誤訊息
    info = error.response
    if (error.response.status === 401) {
      auth.clearToken()
    }
    if (error.response.status === 500) {
      info.data = '程式錯誤，請洽資訊人員(代碼:500)'
    }
  }
  info.ok = false
  return Promise.reject(info)
})

export default function () {
  return http
}
