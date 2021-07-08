import menu from '@/components/MC/menu'
import Area from '@/components/MC/Area'
import Center from '@/components/MC/Center'
import Hosp from '@/components/MC/Hosp'
import Form from '@/components/MC/Area/Form'
import SelectList from '@/components/MC/Area/SelectList'
import SelectListIndex from '@/components/MC/Area/mapindex'
import HospAdmission from '@/components/MC/HospAdmission'
import HospEvacuation from '@/components/MC/HospEvacuation'
import AreaPatList from '@/components/MC/HospAllPatList'
import CenterSystem from '@/components/MC/Center/system'
import moment from 'moment'
const webTitle = 'MC-'
let _redirect = '/menu'
export default [
  {
    path: '*',
    redirect: _redirect,
    meta: {
      title: webTitle
    }
  },
  {
    path: '/menu',
    name: 'menu',
    component: menu,
    meta: {
      title: webTitle + '選擇功能'
    }
  },
  {
    path: '/MC/Area/',
    name: 'Area',
    redirect: '/MC/Area/Form/' + moment().format('YYYYMMDDHHmmss'),
    component: Area,
    children: [
      {
        path: '/MC/Area/Form/:now',
        name: 'AreaForm',
        component: Form,
        meta: {
          title: '建立傷患資料'
        }
      },
      {
        path: '/MC/Area/SelectList/:now',
        name: 'AreaSelectList',
        component: SelectList,
        meta: {
          title: '選擇病人後送'
        }
      },
      {
        path: '/MC/Area/HospAdmission/:now',
        name: 'AreaHospAdmission',
        component: HospAdmission,
        meta: {
          title: '後送醫院狀況'
        }
      },
      {
        path: '/MC/Area/HospEvacuation/:now',
        name: 'AreaHospEvacuation',
        component: HospEvacuation,
        meta: {
          title: '到達醫院狀況'
        }
      },
      {
        path: '/MC/Area/PatList/:now',
        name: 'AreaPatList',
        component: AreaPatList,
        meta: {
          title: '傷患查詢'
        }
      }
    ]
  },
  {
    path: '/MC/Center',
    name: 'Center',
    redirect: '/MC/Center/PatList/' + moment().format('YYYYMMDDHHmmss'),
    component: Center,
    children: [
      {
        path: '/MC/Center/HospAdmission/:now',
        name: 'CenterHospAdmission',
        component: HospAdmission,
        meta: {
          title: '中心-' + '後送醫院狀況'
        }
      },
      {
        path: '/MC/Center/HospEvacuation/:now',
        name: 'CenterHospEvacuation',
        component: HospEvacuation,
        meta: {
          title: '中心-' + '到達醫院狀況'
        }
      },
      {
        path: '/MC/Center/PatList/:now',
        name: 'CenterPatList',
        component: AreaPatList,
        meta: {
          title: '中心-' + '傷患查詢'
        }
      },
      {
        path: '/MC/Center/system/:now',
        name: 'CenterSystem',
        component: CenterSystem,
        meta: {
          title: '中心-' + '後臺管理'
        }
      }
    ]
  },
  {
    path: '/MC/Hosp',
    name: 'Hosp',
    redirect: '/MC/Hosp/PatList/' + moment().format('YYYYMMDDHHmmss'),
    component: Hosp,
    children: [
      {
        path: '/MC/Hosp/HospAdmission/:now',
        name: 'HospHospAdmission',
        component: HospAdmission,
        meta: {
          title: '醫院-' + '後送醫院狀況'
        }
      },
      {
        path: '/MC/Hosp/HospEvacuation/:now',
        name: 'HospHospEvacuation',
        component: HospEvacuation,
        meta: {
          title: '醫院-' + '到達醫院狀況'
        }
      },
      {
        path: '/MC/Hosp/PatList/:now',
        name: 'HospPatList',
        component: AreaPatList,
        meta: {
          title: '醫院-' + '傷患查詢'
        }
      }
    ]
  },
  {
    path: '/MC/Area/Map/:now',
    name: 'mapindex',
    component: SelectListIndex,
    meta: {
      title: '地圖'
    }
  }
]
