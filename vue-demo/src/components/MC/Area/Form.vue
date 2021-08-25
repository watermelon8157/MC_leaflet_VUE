<style scoped>
</style>
<template>
  <div class="m-2">
    <a-row>
      <a-col :span="24">
        填寫總人數
        <span class="m-2 px-4 bg-white">{{ patListLength }}</span>
      </a-col>
    </a-row>
    <a-row>
      <a-card title="傷患基本資料卡">
        <div slot="extra">
          <a-input :addonBefore="a" v-model="a1" class="w-48" addonAfter="重傷">
          </a-input>
          <a-input v-model="a2" class="w-24" addonAfter="中傷"> </a-input>
          <a-input v-model="a3" class="w-24" addonAfter="輕傷"> </a-input>
          <a-button-group>
            <a-button type="danger" @click="AutoCreatePatDataList"
              >產生測試資料</a-button
            >
            <a-button type="primary" @click="AutoCreatePatData"
              >產生下一筆傷患資料</a-button
            >
          </a-button-group>
        </div>
        <div v-if="loading">
          <a-skeleton active />
          <a-skeleton active />
        </div>
        <div v-else>
          <a-form layout="inline">
            <div class="absolute right-0">
              <!-- <img class="mr-16" src="@/assets/TMU.png" alt="TMU" />-->
            </div>
            <a-form-item
              style="display: block"
              :labelCol="{ span: 3 }"
              required
              label="傷患號碼"
              :validate-status="!!model.PATIENT_ID ? '' : 'error'"
              :help="!!model.PATIENT_ID ? ' ' : '請輸入傷患號碼'"
            >
              <a-input v-model="model.PATIENT_ID" placeholder="傷患號碼" />
            </a-form-item>
            <a-form-item
              style="display: block"
              :labelCol="{ span: 3 }"
              required
              label="姓名"
              :validate-status="!!model.PATIENT_NAME ? '' : 'error'"
              :help="!!model.PATIENT_NAME ? ' ' : '請輸入姓名'"
            >
              <a-input v-model="model.PATIENT_NAME" placeholder="姓名" />
            </a-form-item>
            <a-form-item
              style="display: block"
              :labelCol="{ span: 3 }"
              required
              label="性別"
              :validate-status="!!model.GENDER ? '' : 'error'"
              :help="!!model.GENDER ? ' ' : '請輸入性別'"
            >
              <a-radio-group v-model="model.GENDER" buttonStyle="solid">
                <a-radio-button value="1">男生</a-radio-button>
                <a-radio-button value="0">女生</a-radio-button>
              </a-radio-group>
            </a-form-item>
            <a-form-item
              style="display: block"
              :labelCol="{ span: 3 }"
              required
              label="年齡"
              :validate-status="!!model.AGE ? '' : 'error'"
              :help="!!model.AGE ? ' ' : '請輸入年齡'"
            >
              <a-input v-model="model.AGE" placeholder="年齡" />
            </a-form-item>
            <a-form-item
              style="display: block"
              :labelCol="{ span: 3 }"
              required
              label="傷勢"
              :validate-status="!!model.TRIAGE ? '' : 'error'"
              :help="!!model.TRIAGE ? ' ' : '請輸傷勢'"
            >
              <a-radio-group v-model="model.TRIAGE">
                <a-radio-button class="text-red-500 bg-red-200" value="Severe"
                  >重傷</a-radio-button
                >
                <a-radio-button
                  class="text-yellow-500 bg-yellow-200"
                  value="Moderate"
                  >中傷</a-radio-button
                >
                <a-radio-button class="text-blue-500 bg-blue-200" value="Mild"
                  >輕傷</a-radio-button
                >
              </a-radio-group>
              <span
                class="m-2"
                :class="{
                  'text-red-500': model.TRIAGE === 'Severe',
                  'text-yellow-500': model.TRIAGE === 'Moderate',
                  'text-blue-500': model.TRIAGE === 'Mild',
                }"
                >{{ model.TRIAGE }}</span
              >
            </a-form-item>
            <a-form-item
              style="display: block"
              :labelCol="{ span: 3 }"
              required
              label="國籍"
              :validate-status="!!model.CITY ? '' : 'error'"
              :help="!!model.CITY ? ' ' : '請輸國籍'"
            >
              <a-radio-group v-model="model.CITY" buttonStyle="solid">
                <a-radio-button value="1">本國籍</a-radio-button>
                <a-radio-button value="2">非本國籍</a-radio-button>
              </a-radio-group>
            </a-form-item>
            <a-form-item
              v-if="model.CITY === '2'"
              style="display: block"
              :labelCol="{ span: 3 }"
              label="請輸入國家"
              :validate-status="model.CITY === '1' ? '' : 'error'"
              :help="model.CITY === '1' ? ' ' : '請輸入國家'"
            >
              <a-select style="width: 120px">
                <a-select-option value>請選擇</a-select-option>
                <a-select-option value="日本">日本</a-select-option>
                <a-select-option value="美國">美國</a-select-option>
              </a-select>
            </a-form-item>
            <a-form-item
              style="display: block"
              required
              :labelCol="{ span: 3 }"
              label="記錄時間"
              :validate-status="!!model.SELECTION_DATETIME ? '' : 'error'"
              :help="!!model.SELECTION_DATETIME ? ' ' : '請輸入記錄時間'"
            >
              <a-date-picker
                :allowClear="false"
                format="YYYY-MM-DD HH:mm:ss"
                :value="
                  moment(model.SELECTION_DATETIME, 'YYYY-MM-DD  HH:mm:ss')
                "
                :showTime="{ format: 'HH:mm:ss' }"
                @change="
                  (date, dateString) =>
                    SetModelDate(date, dateString, 'SELECTION_DATETIME')
                "
              />
            </a-form-item>
          </a-form>
        </div>
        <Alert class="m-2" ref="alert"></Alert>
        <a-row class="text-center">
          <a-button type="primary" @click="clickAdddPat">儲存</a-button>
        </a-row>
      </a-card>
    </a-row>
  </div>
</template>
<script>
import DB_MC_PATIENT_INFO from '@/RCS_Data.Models.DB.DB_MC_PATIENT_INFO.json'
import Mixin from '@/mixin'
const info = JSON.parse(JSON.stringify(DB_MC_PATIENT_INFO))
export default {
  mixins: [Mixin],
  data () {
    return {
      a1: 1,
      a2: 1,
      a3: 1,
      loading: false,
      selectPat: '',
      hospEvacuationVal: 0,
      totalVal: 0,
      model: info
    }
  },
  created () {
    this.model.SELECTION_DATETIME = this.$moment().format('YYYY-MM-DD HH:mm:ss')
  },
  computed: {
    a () {
      return '共' + (parseInt(this.a1) + parseInt(this.a2) + parseInt(this.a3)) + '人'
    },
    title () {
      return document.title
    }
  },
  methods: {
    SetNewModel () {
      let modelVal = info
      return modelVal
    },
    SetModelDate (date, dateString, pId) {
      this.model[pId] = dateString
    },
    // 儲存病人資料
    clickAdddPat () {
      let vuethis = this
      vuethis.$store.commit('SpinLoading', true)
      if (document.getElementsByClassName('has-error').length > 0) {
        vuethis.$notification.error({
          message: '請填寫必要欄位!'
        })
        vuethis.$refs.alert.show('請填寫必要欄位!')
      } else {
        vuethis.$store.commit({
          type: 'Basic/pushPatListBYID',
          data: vuethis.model
        })
        vuethis.model.SITE_ID = this.site_id
        vuethis.$api.MC.INSERT_PAT_DATA(this.model).then((result) => {
          vuethis.info(result.data)
          vuethis.$set(vuethis, 'model', JSON.parse(JSON.stringify(vuethis.SetNewModel())))
          vuethis.$api.MC.GetPatListByID({ site_id: vuethis.site_id, hosp_id: vuethis.hosp_id }).then((result) => {
            vuethis.$store.commit({
              type: 'Basic/SetPatListBYID',
              data: result.data
            })
          }).catch((err) => {
            console.log(err)
            vuethis.error(err)
          })
        }).catch((err) => {
          console.log(err)
          vuethis.error(err)
        })
      }
      this.$store.commit('SpinLoading', false)
    },
    // 自動新增病人資料
    AutoCreatePatData () {
      let tempModel = JSON.parse(JSON.stringify(this.SetNewModel()))
      tempModel.PATIENT_ID = this.$moment().format('YYYYMMDDHHmmss')
      tempModel.PATIENT_NAME = this.$moment().format('mmss') + '傷患'
      tempModel.SELECTION_DATETIME = this.$moment().format('YYYY-MM-DD HH:mm:ss')
      this.model = tempModel
      this.$refs.alert.showInfo('產生下一筆傷患資料!')
    },
    AutoCreatePatDataList () {
      let pList = []
      let vuethis = this
      for (let index = 0; index < parseInt(vuethis.a1); index++) {
        let tempModel = JSON.parse(JSON.stringify(this.SetNewModel()))
        tempModel.PATIENT_ID = this.$moment().format('YYYYMMDDHHmmss')
        tempModel.PATIENT_NAME = this.$moment().format('mmss') + '傷患'
        tempModel.SELECTION_DATETIME = this.$moment().format('YYYY-MM-DD HH:mm:ss')
        tempModel.GENDER = '1'
        tempModel.AGE = '30'
        tempModel.TRIAGE = 'Severe'
        tempModel.CITY = '1'
        pList.push(tempModel)
      }
      for (let index = 0; index < parseInt(vuethis.a2); index++) {
        let tempModel = JSON.parse(JSON.stringify(this.SetNewModel()))
        tempModel.PATIENT_ID = this.$moment().format('YYYYMMDDHHmmss')
        tempModel.PATIENT_NAME = this.$moment().format('mmss') + '傷患'
        tempModel.SELECTION_DATETIME = this.$moment().format('YYYY-MM-DD HH:mm:ss')
        tempModel.GENDER = '1'
        tempModel.AGE = '30'
        tempModel.TRIAGE = 'Moderate'
        tempModel.CITY = '1'
        pList.push(tempModel)
      }
      for (let index = 0; index < parseInt(vuethis.a3); index++) {
        let tempModel = JSON.parse(JSON.stringify(this.SetNewModel()))
        tempModel.PATIENT_ID = this.$moment().format('YYYYMMDDHHmmss')
        tempModel.PATIENT_NAME = this.$moment().format('mmss') + '傷患'
        tempModel.SELECTION_DATETIME = this.$moment().format('YYYY-MM-DD HH:mm:ss')
        tempModel.GENDER = '1'
        tempModel.AGE = '30'
        tempModel.TRIAGE = 'Mild'
        tempModel.CITY = '1'
        pList.push(tempModel)
      }
      for (let index = 0; index < pList.length; index++) {
        const element = pList[index]
        element.SITE_ID = this.site_id
        element.PATIENT_ID = element.PATIENT_ID + index
        console.log(element.PATIENT_ID)
        console.log(element.TRIAGE)
        setTimeout(() => {
          element.SELECTION_DATETIME = this.$moment().add(1, 'minutes').format('YYYY-MM-DD HH:mm:ss')
          vuethis.$api.MC.INSERT_PAT_DATA(element).then((result) => {
            console.log(result)
            vuethis.$api.MC.GetPatListByID({ site_id: vuethis.site_id, hosp_id: vuethis.hosp_id }).then((result) => {
              vuethis.$store.commit({
                type: 'Basic/SetPatListBYID',
                data: result.data
              })
            }).catch((err) => {
              console.log(err)
              vuethis.error(err)
            })
          }).catch((err) => {
            console.log(err)
            vuethis.error(err)
          })
        }, 1000 * index)
      }
    }
  }
}
</script>
