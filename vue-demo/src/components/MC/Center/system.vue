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
        <a-tab-pane key="card" tab="事件維護">
          <a-card>
            <div slot="title">
              事件清單
              <a-button type="primary" @click="openSite(data)">
                新增事件
              </a-button>
              <span class="float-right">共{{ siteList.length }}筆</span>
            </div>
            <a-table
              class="table-striped"
              :columns="columns"
              :dataSource="siteList"
              :pagination="false"
              bordered
              :customRow="clickCustomRow"
            >
            </a-table>
          </a-card>
        </a-tab-pane>
        <!-- <a-tab-pane key="hosp" tab="醫院維護">
          <p>醫院維護</p>
        </a-tab-pane> -->
      </a-tabs>
      <a-drawer
        title="事件維護"
        placement="right"
        width="512"
        :visible="SiteVisible"
        @close="onSiteClose"
      >
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
            <a-form-item>
              <a-button type="primary" @click="saveSite()"> 儲存 </a-button>
            </a-form-item>
          </div>
        </a-form>
      </a-drawer>
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
  'LONGITUDE': ''
}

const columns = [
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
export default {
  mixins: [Mixin],
  data () {
    return {
      data: data,
      latitude: '',
      longitude: '',
      form: data,
      SiteVisible: false,
      columns: columns,
      siteid: '',
      siteList: []

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
    openSite (pData) {
      let vuethis = this
      vuethis.form = JSON.parse(JSON.stringify(pData))
      vuethis.form.SITE_ID = vuethis.$moment().format('YYYYMMDDHHmmss')
      vuethis.form.LATITUDE = vuethis.latitude
      vuethis.form.LONGITUDE = vuethis.longitude
      vuethis.SiteVisible = true
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
