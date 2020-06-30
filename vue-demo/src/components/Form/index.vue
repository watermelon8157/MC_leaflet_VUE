<style scoped>
</style>
<template>
  <div class="m-2">
    <a-row>
      <a-col :span="24">
        填寫總人數
        <span class="m-2 px-4 bg-white">{{patListLength}}</span>
      </a-col>
    </a-row>
    <a-row>
      <a-card title="傷患基本資料卡">
        <a-button-group slot="extra">
          <a-button type="primary" @click="AutoCreatePatData">產生下一筆傷患資料</a-button>
        </a-button-group>
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
              style="display:block;"
              :labelCol="{span: 3}"
              required
              label="傷患號碼"
              :validate-status="!!model.PATIENT_ID ? '' : 'error'"
              :help="!!model.PATIENT_ID ? ' ':'請輸入傷患號碼'"
            >
              <a-input v-model="model.PATIENT_ID" placeholder="傷患號碼" />
            </a-form-item>
            <a-form-item
              style="display:block;"
              :labelCol="{span: 3}"
              required
              label="姓名"
              :validate-status="!!model.PATIENT_NAME ? '' : 'error'"
              :help="!!model.PATIENT_NAME ? ' ':'請輸入姓名'"
            >
              <a-input v-model="model.PATIENT_NAME" placeholder="姓名" />
            </a-form-item>
            <a-form-item
              style="display:block;"
              :labelCol="{span: 3}"
              required
              label="性別"
              :validate-status="!!model.GENDER ? '' : 'error'"
              :help="!!model.GENDER ? ' ':'請輸入性別'"
            >
              <a-radio-group v-model="model.GENDER" buttonStyle="solid">
                <a-radio-button value="1">男生</a-radio-button>
                <a-radio-button value="0">女生</a-radio-button>
              </a-radio-group>
            </a-form-item>
            <a-form-item
              style="display:block;"
              :labelCol="{span: 3}"
              required
              label="年齡"
              :validate-status="!!model.AGE ? '' : 'error'"
              :help="!!model.AGE ? ' ':'請輸入年齡'"
            >
              <a-input v-model="model.AGE" placeholder="年齡" />
            </a-form-item>
            <a-form-item
              style="display:block;"
              :labelCol="{span: 3}"
              required
              label="傷勢"
              :validate-status="!!model.TRIAGE ? '' : 'error'"
              :help="!!model.TRIAGE ? ' ':'請輸傷勢'"
            >
              <a-radio-group v-model="model.TRIAGE">
                <a-radio-button class="text-red-500 bg-red-200" value="Severe">Severe</a-radio-button>
                <a-radio-button class="text-blue-500 bg-blue-200" value="Moderate">Moderate</a-radio-button>
                <a-radio-button class="text-green-500 bg-green-200" value="Mild">Mild</a-radio-button>
              </a-radio-group>
              <span
                class="m-2"
                :class="{'text-red-500' : model.TRIAGE === 'Severe' ,
              'text-blue-500' : model.TRIAGE === 'Moderate' ,
              'text-green-500' : model.TRIAGE === 'Mild'
               }"
              >{{model.TRIAGE}}</span>
            </a-form-item>
            <a-form-item
              style="display:block;"
              :labelCol="{span: 3}"
              required
              label="國籍"
              :validate-status="!!model.CITY ? '' : 'error'"
              :help="!!model.CITY ? ' ':'請輸國籍'"
            >
              <a-radio-group v-model="model.CITY" buttonStyle="solid">
                <a-radio-button value="1">本國籍</a-radio-button>
                <a-radio-button value="2">非本國籍</a-radio-button>
              </a-radio-group>
            </a-form-item>
            <a-form-item
              v-if="model.CITY === '2'"
              style="display:block;"
              :labelCol="{span: 3}"
              label="請輸入國家"
              :validate-status=" model.CITY=== '1' ? '' : 'error'"
              :help="model.CITY=== '1' ? ' ':'請輸入國家'"
            >
              <a-select style="width: 120px">
                <a-select-option value>請選擇</a-select-option>
                <a-select-option value="日本">日本</a-select-option>
                <a-select-option value="美國">美國</a-select-option>
              </a-select>
            </a-form-item>
            <a-form-item
              style="display:block;"
              required
              :labelCol="{span: 3}"
              label="記錄時間"
              :validate-status="!!model.SELECTION_DATETIME ? '' : 'error'"
              :help="!!model.SELECTION_DATETIME ? ' ':'請輸入記錄時間'"
            >
              <a-date-picker
                :allowClear="false"
                format="YYYY-MM-DD HH:mm:ss"
                :value="moment(model.SELECTION_DATETIME, 'YYYY-MM-DD  HH:mm:ss')"
                :showTime="{ format:  'HH:mm:ss'  }"
                @change="(date, dateString) => SetModelDate(date, dateString, 'SELECTION_DATETIME')"
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
      this.$store.commit('SpinLoading', true)
      if (document.getElementsByClassName('has-error').length > 0) {
        this.$notification.error({
          message: '請填寫必要欄位!'
        })
        this.$refs.alert.show('請填寫必要欄位!')
      } else {
        this.$store.commit({
          type: 'Basic/pushPatListBYID',
          data: this.model
        })
        this.model.SITE_ID = this.site_id
        this.$api.MC.INSERT_PAT_DATA(this.model).then((result) => {
          this.info(result.data)
          this.$set(this, 'model', JSON.parse(JSON.stringify(this.SetNewModel())))
        }).catch((err) => {
          console.log(err)
          this.error(err)
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
    }
  }
}
</script>
