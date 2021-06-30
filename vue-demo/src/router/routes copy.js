import HelloWorld from '@/components/HelloWorld'
import ExercisePlans from '@/components/ExercisePlans'
import SearshHosp from '@/components/searsh_hosp'
import ChartHosp from '@/components/ChartHosp'
import SelectList from '@/components/SelectList/Map'
import SelectListIndex from '@/components/SelectList'
import Form from '@/components/Form'
import HospAdmission from '@/components/HospAdmission'
import HospEvacuation from '@/components/HospEvacuation'
import PatList from '@/components/PatList'
import Area from '@/components/Area'
import Center from '@/components/Center'
import Hosp from '@/components/Hosp'
import menu from '@/components/menu'
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
    path: '/MC/Area/',
    name: 'Area',
    redirect: '/MC/Area/Form/' + moment().format('YYYYMMDDHHmmss'),
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
          title: '待送清單'
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
        component: PatList,
        meta: {
          title: '傷患查詢'
        }
      }
    ],
    component: Area
  },
  {
    path: '/MC/Center',
    name: 'Center',
    redirect: '/MC/Center/PatList/' + moment().format('YYYYMMDDHHmmss'),
    children: [
      {
        path: '/MC/Center/Form/:now',
        name: 'CenterForm',
        component: Form,
        meta: {
          title: '中心-' + '建立傷患資料'
        }
      },
      {
        path: '/MC/Center/SelectList/:now',
        name: 'CenterSelectList',
        component: SelectList,
        meta: {
          title: '中心-' + '待送清單'
        }
      },
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
        component: PatList,
        meta: {
          title: '中心-' + '傷患查詢'
        }
      }
    ],
    component: Center
  },
  {
    path: '/MC/Hosp',
    name: 'Hosp',
    redirect: '/MC/Hosp/PatList/' + moment().format('YYYYMMDDHHmmss'),
    children: [
      {
        path: '/MC/Hosp/Form/:now',
        name: 'HospForm',
        component: Form,
        meta: {
          title: '建立傷患資料'
        }
      },
      {
        path: '/MC/Hosp/SelectList/:now',
        name: 'HospSelectList',
        component: SelectList,
        meta: {
          title: '醫院-' + '待送清單'
        }
      },
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
        component: PatList,
        meta: {
          title: '醫院-' + '傷患查詢'
        }
      }
    ],
    component: Hosp
  },
  {
    path: '/MC',
    name: 'HelloWorld',
    component: HelloWorld
  },
  {
    path: '/MC/Form/:now',
    name: 'Form',
    component: Form,
    meta: {
      title: '建立傷患資料'
    }
  },
  {
    path: '/MC/SelectList/:now',
    name: 'SelectList',
    component: SelectList,
    meta: {
      title: '待送清單'
    }
  },
  {
    path: '/MC/HospAdmission/:now',
    name: 'HospAdmission',
    component: HospAdmission,
    meta: {
      title: '後送醫院狀況'
    }
  },
  {
    path: '/MC/HospEvacuation/:now',
    name: 'HospEvacuation',
    component: HospEvacuation,
    meta: {
      title: '到達醫院狀況'
    }
  },
  {
    path: '/MC/PatList/:now',
    name: 'PatList',
    component: PatList,
    meta: {
      title: '傷患查詢'
    }
  },
  {
    path: '/ExercisePlans',
    name: 'ExercisePlans',
    component: ExercisePlans,
    meta: {
      title: 'ExercisePlans'
    }
  },
  {
    path: '/SearshHosp',
    name: 'SearshHosp',
    component: SearshHosp,
    meta: {
      title: 'SearshHosp'
    }
  },
  {
    path: '/ChartHosp',
    name: 'ChartHosp',
    component: ChartHosp,
    meta: {
      title: 'ChartHosp'
    }
  },
  {
    path: '/VueLeaflet',
    name: 'VueLeaflet',
    component: SelectListIndex,
    meta: {
      title: '地圖測試'
    }
  },
  {
    path: '/menu',
    name: 'menu',
    component: menu,
    meta: {
      title: '選擇功能'
    }
  }
]
