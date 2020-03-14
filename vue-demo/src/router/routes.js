
import HelloWorld from '@/components/HelloWorld'
import ExercisePlans from '@/components/ExercisePlans'
import SearshHosp from '@/components/searsh_hosp'
import ChartHosp from '@/components/ChartHosp'
import SelectList from '@/components/SelectList'
import Form from '@/components/Form'
import HospAdmission from '@/components/HospAdmission'
import HospEvacuation from '@/components/HospEvacuation'
import PatList from '@/components/PatList'
import VueLeaflet from '@/components/VueLeaflet'
import moment from 'moment'
const webTitle = 'MC-'
const isDev = process.env.NODE_ENV === 'development'
let _redirect = '/Login'
if (isDev) {
  _redirect = '/MC/Form/' + moment().format('YYYYMMDDHHmmss')
}
export default [{
  path: '*',
  redirect: _redirect,
  meta: {
    title: webTitle
  }
}, {
  path: '/MC',
  name: 'HelloWorld',
  component: HelloWorld
},
{
  path: '/MC/Form/:now',
  name: 'Form',
  component: Form,
  meta: {
    title: '建立病患資料'
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
    title: '醫院收治狀況'
  }
},
{
  path: '/MC/HospEvacuation/:now',
  name: 'HospEvacuation',
  component: HospEvacuation,
  meta: {
    title: '醫院後送狀況'
  }
},
{
  path: '/MC/PatList/:now',
  name: 'PatList',
  component: PatList,
  meta: {
    title: '病患清單'
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
}, {
  path: '/VueLeaflet',
  name: 'VueLeaflet',
  component: VueLeaflet,
  meta: {
    title: '地圖測試'
  }
}

]
