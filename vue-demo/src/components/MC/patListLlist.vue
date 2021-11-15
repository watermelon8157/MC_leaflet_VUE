<template>
  <div>
    <a-card>
      <div slot="title">
        傷患清單
        <span class="float-right">共{{ patlist.length }}人</span>
      </div>
      <a-table
        class="table-striped"
        :columns="columns"
        :dataSource="patlist"
        :pagination="false"
        bordered
      >
        <template slot="PATIENT_NAME" slot-scope="text, record">
          {{ text.substring(0, 1) }}OO
        </template>
        <template slot="SITE_ID" slot-scope="text, record">
          {{ showSiteList(text) }}
        </template>
        <template slot="GENDER" slot-scope="text, record">
          {{ text === "1" ? "男" : "女" }}
        </template>
      </a-table>
    </a-card>
  </div>
</template>

<script>
import Mixin from '@/mixin'

const columns = [
  {
    title: '姓名',
    dataIndex: 'PATIENT_NAME',
    scopedSlots: { customRender: 'PATIENT_NAME' }
  },
  {
    title: '性別',
    dataIndex: 'GENDER',
    scopedSlots: { customRender: 'GENDER' }
  },
  {
    title: '年齡',
    dataIndex: 'AGE'
  },
  {
    title: '傷勢',
    dataIndex: 'TRIAGE_CHT',
    scopedSlots: { customRender: 'TRIAGE_CHT' }
  },
  {
    title: '送出時間',
    dataIndex: 'SELECTION_DATETIME',
    scopedSlots: { customRender: 'SELECTION_DATETIME' }
  },
  {
    title: '抵達時間',
    dataIndex: 'EXPECTED_ARRIVAL_DATETIME',
    scopedSlots: { customRender: 'EXPECTED_ARRIVAL_DATETIME' }
  },
  {
    title: '救護車代碼',
    dataIndex: 'AMB_ID'
  },
  {
    title: '醫院',
    dataIndex: 'HOSPITAL_SHOW_NAME'
  },

  {
    title: '事件',
    dataIndex: 'SITE_ID',
    scopedSlots: { customRender: 'SITE_ID' }
  }
]

const data = [
  { Date: '2019/6/27', Departure: '21:00', Arrival: '21:15', Gender: '女', Age: '21', Last_name: 'Wang', Triage: '80%，2-3度', Sever: 'V', Minor: '', code119: 'A92', Hospital: '淡水馬偕' },
  { Date: '2019/6/28', Departure: '21:00', Arrival: '21:15', Gender: '女', Age: '21', Last_name: 'NA', Triage: '30-40%，2-3度', Sever: 'V', Minor: '', code119: 'A93', Hospital: '淡水馬偕' },
  { Date: '2019/6/29', Departure: '21:00', Arrival: '21:15', Gender: '女', Age: '17', Last_name: 'Lee', Triage: '30-40%，2-3度', Sever: 'V', Minor: '', code119: 'A94', Hospital: '淡水馬偕' },
  { Date: '2019/6/30', Departure: '21:00', Arrival: '21:15', Gender: '男', Age: '21', Last_name: 'Lee', Triage: '30-40%，2-3度', Sever: 'V', Minor: '', code119: 'A95', Hospital: '淡水馬偕' },
  { Date: '2019/6/31', Departure: '20:54', Arrival: '21:15', Gender: '女', Age: '19', Last_name: 'Shung', Triage: '2度 38%', Sever: 'V', Minor: '', code119: 'B92', Hospital: '淡水馬偕' },
  { Date: '2019/6/32', Departure: '20:54', Arrival: '21:15', Gender: '女', Age: '21', Last_name: 'Chung', Triage: '2度 57%', Sever: 'V', Minor: '', code119: 'B93', Hospital: '林口長庚' },
  { Date: '2019/6/33', Departure: '21:45', Arrival: '22:01', Gender: '男', Age: '26', Last_name: 'Dai', Triage: '2度 40%', Sever: 'V', Minor: '', code119: 'C92', Hospital: '林口長庚' },
  { Date: '2019/6/34', Departure: '21:28', Arrival: '21:44', Gender: '男', Age: '26', Last_name: 'Hung', Triage: '2度 72%', Sever: 'V', Minor: '', code119: 'C93', Hospital: '林口長庚' },
  { Date: '2019/6/35', Departure: '21:28', Arrival: '21:44', Gender: '男', Age: '24', Last_name: 'Chen', Triage: '2度 3% ', Sever: '', Minor: 'V', code119: 'C94', Hospital: '三總' },
  { Date: '2019/6/36', Departure: '21:28', Arrival: '21:44', Gender: '女', Age: '20', Last_name: 'Mon', Triage: '2度 10%', Sever: '', Minor: 'V', code119: 'C95', Hospital: '淡水馬偕' }
]
export default {
  mixins: [Mixin],
  name: 'patList',
  props: {
    config: {
      type: Object,
      default: function () {
        return {
          hospid: '',
          siteid: ''
        }
      }
    }
  },
  computed: {
    hospkey () {
      return this.config.hospid
    },
    siteid () {
      return this.config.siteid
    },
    patlist () {
      let vuethis = this
      if (vuethis.hospkey) {
        if (vuethis.hospkey === 'hospid') {
          return vuethis.$store.state.Basic.PatListByID.filter((x) => !x.HOSP_KEY)
        }
        return vuethis.$store.state.Basic.PatListByID.filter((x) => x.HOSP_KEY === vuethis.hospkey)
      }
      return vuethis.$store.state.Basic.PatListByID
      // return []
    }
  },
  data () {
    return {
      siteList: [],
      data,
      columns
    }
  },
  created () {
    let vuethis = this
    vuethis.$api.MC.getMC_SITE_INFO()
      .then(result => {
        vuethis.siteList = result.data
      })
      .catch(err => {
        vuethis.$notification.error({
          message: err.data
        })
      })
  },
  methods: {
    showSiteList (pVal) {
      let desc = pVal
      let vuethis = this
      vuethis.siteList.forEach(element => {
        if (element.SITE_ID === pVal) {
          desc = element.SITE_DESC
        }
      })
      return desc
    }
  }
}
</script>
<style>
th.column-money,
td.column-money {
  text-align: right !important;
}
</style>
