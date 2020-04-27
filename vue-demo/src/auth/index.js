// const Buffer = require('buffer/').Buffer
const jwtDecode = require('jwt-decode')
export default {
  // token 存在判斷
  TokenExist () {
    var token = !!sessionStorage.getItem('Authorization')
    return token
  },
  // 取得Paylaod token資料
  getPaylaod () {
    let auth = this.getToken()
    let payload = {}
    let isAuth = !!auth
    if (isAuth) {
      payload = jwtDecode(auth)
    }
    return payload
  },
  getSiteID () {
    let auth = this.getToken()
    let payload = {}
    let isAuth = !!auth
    if (isAuth) {
      payload = jwtDecode(auth)
    }
    return payload.site_id
  },
  // 取得登入使用者名稱
  getUserName () {
    let auth = this.getToken()
    let payload = {}
    let isAuth = !!auth
    if (isAuth) {
      payload = jwtDecode(auth)
    }
    return payload.user_name
  },
  ReadRole () {
    let auth = this.getToken()
    let payload = {}
    let isAuth = !!auth
    if (isAuth) {
      payload = jwtDecode(auth)
    }
    let ispayload = !!payload
    if (ispayload) {
      return payload.role
    }
    return null
  },
  // 取得head token 資料
  getToken () {
    // 從sessionStorage取得之前存的資料
    var token = sessionStorage.getItem('Authorization')
    return token
  },
  // 設定token 開發模式下固定使用
  setToken (token) {
    // 將資料存到sessionStorage
    // dev 模式用token
    sessionStorage.setItem('Authorization', 'Bearer ' + token)
  },
  // 設定token 開發模式下固定使用
  setPatientInfo (Info) {
    // 將資料存到sessionStorage
    // dev 模式用token
    sessionStorage.setItem(
      'pat_info',
      encodeURIComponent(JSON.stringify(Info))
    )
  },
  // 取得head token 資料
  getPatientInfo () {
    // 從sessionStorage取得之前存的資料
    var patInfo = JSON.parse(
      decodeURIComponent(sessionStorage.getItem('pat_info'))
    )
    return patInfo
  },
  PatientInfoExist () {
    var data = !!sessionStorage.getItem('pat_info')
    return data
  },
  setPatList (Info) {
    // 將資料存到sessionStorage
    // dev 模式用token
    sessionStorage.setItem('PatList', encodeURIComponent(JSON.stringify(Info)))
  },
  // clear token
  clearToken () {
    // 將資料存到sessionStorage
    // dev 模式用token
    sessionStorage.removeItem('Authorization')
    sessionStorage.removeItem('pat_info')
    sessionStorage.removeItem('isGuide')
  },
  isGuide () {
    var isGuide = !!sessionStorage.getItem('isGuide')
    return isGuide
  },
  getTypeMode () {
    return sessionStorage.getItem('type_mode')
  }
}
