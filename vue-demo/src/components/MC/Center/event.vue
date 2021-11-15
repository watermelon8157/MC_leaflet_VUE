<style scoped>
.card-container {
  background: #f5f5f5;
  overflow: hidden;
  padding: 24px;
}
.card-container > .ant-tabs-card > .ant-tabs-content {
  height: 120px;
  margin-top: -16px;
}

.card-container > .ant-tabs-card > .ant-tabs-content > .ant-tabs-tabpane {
  background: #fff;
  padding: 16px;
}

.card-container > .ant-tabs-card > .ant-tabs-bar {
  border-color: #fff;
}

.card-container > .ant-tabs-card > .ant-tabs-bar .ant-tabs-tab {
  border-color: transparent;
  background: transparent;
}

.card-container > .ant-tabs-card > .ant-tabs-bar .ant-tabs-tab-active {
  border-color: #fff;
  background: #fff;
}
</style>
<template>
  <div>
    <div class="card-container">
      <a-tabs type="card">
        <a-tab-pane key="card" tab="開案">
         <a-form layout="inline">
          <div>
            <a-form-item label="事件代碼">
              <a-input v-model="form.SITE_ID" placeholder="事件代碼" />
            </a-form-item>
          </div>
          <div>
            <a-form-item label="事件名稱">
              <a-input v-model="form.SITE_DESC" placeholder="事件名稱" />
            </a-form-item>
          </div>
          <div>
            <a-form-item label="地區">
              <a-select v-model="form.SITE_AREA" style="width: 120px">
                <a-select-option value="">請選擇地區</a-select-option>
                <a-select-option value="中區">中區</a-select-option>
                <a-select-option value="北區">北區</a-select-option>
                <a-select-option value="台北區">台北區 </a-select-option>
                <a-select-option value="東區">東區</a-select-option>
                <a-select-option value="南區">南區</a-select-option>
                <a-select-option value="高屏區">高屏區</a-select-option>
              </a-select>
            </a-form-item>
          </div>
          <div>
            <a-form-item label="經度">
              <a-input v-model="form.LONGITUDE" placeholder="經度" />
            </a-form-item>
          </div>
          <div>
            <a-form-item label="緯度">
              <a-input v-model="form.LATITUDE" placeholder="緯度" />
            </a-form-item>
          </div>
          <div>
            <a-form-item label="災害類別">
              <a-select v-model="form.CLASS_TYPE" style="width: 120px">
                <a-select-option value="">請選擇災害類別</a-select-option>
                <a-select-option value="火災爆炸">火災爆炸</a-select-option>
                <a-select-option value="交通事故">交通事故</a-select-option>
                <a-select-option value="毒化災">毒化災</a-select-option>
                <a-select-option value="地震">地震</a-select-option>
                <a-select-option value="食物中毒">食物中毒</a-select-option>
                <a-select-option value="其他">其他</a-select-option>
              </a-select>
            </a-form-item>
          </div>
          <div>
            <a-form-item>
              <a-button type="primary" @click="saveSite()"> 儲存 </a-button>
            </a-form-item>
          </div>
        </a-form>
        </a-tab-pane>
      </a-tabs>
    </div>
  </div>
</template>
<script>

import Mixin from '@/mixin'

const data = {
  'SITE_ID': '',
  'SITE_AREA': '',
  'SITE_DESC': '',
  'CREATE_ID': '',
  'CREATE_NAME': '',
  'CREATE_DATE': '',
  'MODIFY_ID': '',
  'MODIFY_NAME': '',
  'MODIFY_DATE': '',
  'DATASTATUS': '',
  'LATITUDE': '',
  'LONGITUDE': '',
  'CLASS_TYPE': ''
}
const datahosp = {
  'SITE_ID': '',
  'SITE_AREA': '',
  'SITE_DESC': '',
  'CREATE_ID': '',
  'CREATE_NAME': '',
  'CREATE_DATE': '',
  'MODIFY_ID': '',
  'MODIFY_NAME': '',
  'MODIFY_DATE': '',
  'DATASTATUS': '',
  'LATITUDE': '',
  'LONGITUDE': '',
  'CLASS_TYPE': ''
}
const columns = [
  {
    title: '選擇',
    dataIndex: 'select',
    scopedSlots: { customRender: 'select' }
  },
  {
    title: '事件代碼',
    dataIndex: 'SITE_ID',
    scopedSlots: { customRender: 'SITE_ID' }
  },
  {
    title: '事件名稱',
    dataIndex: 'SITE_DESC',
    scopedSlots: { customRender: 'SITE_DESC' }
  },
  {
    title: '地區',
    dataIndex: 'SITE_AREA',
    scopedSlots: { customRender: 'SITE_AREA' }
  },
  {
    title: '經度',
    dataIndex: 'LONGITUDE',
    scopedSlots: { customRender: 'LONGITUDE' }
  },
  {
    title: '緯度',
    dataIndex: 'LATITUDE',
    scopedSlots: { customRender: 'LATITUDE' }
  }
]
const columnshosp = [
  {
    title: '醫院名稱',
    width: 350,
    dataIndex: 'hosp_name',
    scopedSlots: { customRender: 'hosp_name' }
  },
  {
    title: '經度',
    width: 150,
    dataIndex: 'LONGITUDE',
    scopedSlots: { customRender: 'LONGITUDE' }
  },
  {
    title: '緯度',
    width: 150,
    dataIndex: 'LATITUDE',
    scopedSlots: { customRender: 'LATITUDE' }
  },
  {
    title: 'CONTYPE',
    width: 150,
    dataIndex: 'CONTYPE',
    scopedSlots: { customRender: 'CONTYPE' }
  },
  {
    title: 'SEVERE',
    width: 150,
    dataIndex: 'SEVERE',
    scopedSlots: { customRender: 'SEVERE' }
  },
  {
    title: 'MODERATE',
    width: 150,
    dataIndex: 'MODERATE',
    scopedSlots: { customRender: 'MODERATE' }
  },
  {
    title: 'MILD',
    width: 150,
    dataIndex: 'MILD',
    scopedSlots: { customRender: 'MILD' }
  },
  {
    title: 'SEVERE_TC',
    width: 150,
    dataIndex: 'SEVERE_TC',
    scopedSlots: { customRender: 'SEVERE_TC' }
  },
  {
    title: 'MODERATE_TC',
    width: 150,
    dataIndex: 'MODERATE_TC',
    scopedSlots: { customRender: 'MODERATE_TC' }
  },
  {
    title: 'MILD_TC',
    width: 150,
    dataIndex: 'MILD_TC',
    scopedSlots: { customRender: 'MILD_TC' }
  }
]
export default {
  mixins: [Mixin],
  data () {
    return {
      data: data,
      latitude: '',
      longitude: '',
      form: data,
      formhosp: datahosp,
      SiteVisible: false,
      columns: columns,
      columnshosp: columnshosp,
      siteid: '',
      siteList: []
    }
  },
  computed: {
    hospList () {
      let pList = this.$store.state.Basic.hospList
      console.log(pList)
      return pList
    }
  },
  mounted () {
    this.GPS()
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
    vuethis.$api.MC.GetHospList().then((result) => {
      vuethis.$store.commit({
        type: 'Basic/SetGetHospList',
        data: result.data
      })
      setTimeout(() => {
        vuethis.spinning = false
        vuethis.form = JSON.parse(JSON.stringify(data))
        vuethis.form.SITE_ID = vuethis.$moment().format('YYYYMMDDHHmmss')
        vuethis.form.LATITUDE = vuethis.latitude
        vuethis.form.LONGITUDE = vuethis.longitude
      }, 500)
    }).catch((err) => {
      console.log(err)
      this.error(err)
      setTimeout(() => {
        vuethis.spinning = false
      }, 500)
    })
  },
  methods: {
    GPS () {
      navigator.geolocation.getCurrentPosition(this.getGPSlatitude)
    },
    getGPSlatitude (position) {
      this.latitude = position.coords.latitude
      this.longitude = position.coords.longitude
    },
    onSiteClose () {
      this.SiteVisible = false
    },
    openHosp (pData) {
      let vuethis = this
      vuethis.form = JSON.parse(JSON.stringify(pData))
      vuethis.form.SITE_ID = vuethis.$moment().format('YYYYMMDDHHmmss')
      vuethis.form.LATITUDE = vuethis.latitude
      vuethis.form.LONGITUDE = vuethis.longitude
    },
    openSite (pData) {
      let vuethis = this
      vuethis.form = JSON.parse(JSON.stringify(pData))
      vuethis.form.SITE_ID = vuethis.$moment().format('YYYYMMDDHHmmss')
      vuethis.form.LATITUDE = vuethis.latitude
      vuethis.form.LONGITUDE = vuethis.longitude
    },
    saveSite () {
      let vuethis = this
      vuethis.$api.MC.saveSite(vuethis.form)
        .then(result => {
          vuethis.info(result.data)
          vuethis.SiteVisible = false
          vuethis.$api.MC.getMC_SITE_INFO()
            .then(result => {
              vuethis.siteList = result.data
            })
            .catch(err => {
              vuethis.$notification.error({
                message: err.data
              })
            })
        })
        .catch(err => {
          vuethis.$notification.error({
            message: err.data
          })
        })
    },
    clickhospCustomRow (record, index) {
      var vuethis = this
      return {
        on: {
          click: function () {
            vuethis.formhosp = JSON.parse(JSON.stringify(record))
            vuethis.SiteVisible = true
          }
        }
      }
    },
    clickCustomRow (record, index) { // 點選row
      var vuethis = this
      return {
        on: {
          click: function () {
            vuethis.form = JSON.parse(JSON.stringify(record))
            vuethis.SiteVisible = true
          }
        }
      }
    }
  }
}
</script>
