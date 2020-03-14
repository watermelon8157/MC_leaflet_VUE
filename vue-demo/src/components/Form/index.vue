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
      <a-card title="病患基本資料卡">
        <a-button-group slot="extra">
          <a-button type="primary" @click="AutoCreatePatData">自動產生病人</a-button>
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
              label="病歷編號"
              :validate-status="!!model.char_no ? '' : 'error'"
              :help="!!model.char_no ? ' ':'請輸入病歷編號'"
            >
              <a-input v-model="model.char_no" placeholder="病歷編號" />
            </a-form-item>
            <a-form-item
              style="display:block;"
              :labelCol="{span: 3}"
              required
              label="姓名"
              :validate-status="!!model.pat_name ? '' : 'error'"
              :help="!!model.pat_name ? ' ':'請輸入姓名'"
            >
              <a-input v-model="model.pat_name" placeholder="姓名" />
            </a-form-item>
            <a-form-item
              style="display:block;"
              :labelCol="{span: 3}"
              required
              label="性別"
              :validate-status="!!model.sex ? '' : 'error'"
              :help="!!model.sex ? ' ':'請輸入性別'"
            >
              <a-radio-group v-model="model.sex" buttonStyle="solid">
                <a-radio-button value="1">男生</a-radio-button>
                <a-radio-button value="0">女生</a-radio-button>
              </a-radio-group>
            </a-form-item>
            <a-form-item
              style="display:block;"
              :labelCol="{span: 3}"
              required
              label="年齡層"
              :validate-status="!!model.ageType ? '' : 'error'"
              :help="!!model.ageType ? ' ':'請輸年齡層'"
            >
              <a-radio-group v-model="model.ageType" buttonStyle="solid">
                <a-radio-button value="0">0-14歲 (幼年)</a-radio-button>
                <a-radio-button value="1">15-64歲 (青壯年)</a-radio-button>
                <a-radio-button value="2">65歲以上 (老年)</a-radio-button>
              </a-radio-group>
            </a-form-item>
            <a-form-item
              style="display:block;"
              :labelCol="{span: 3}"
              required
              label="傷勢"
              :validate-status="!!model.injury_classification ? '' : 'error'"
              :help="!!model.injury_classification ? ' ':'請輸傷勢'"
            >
              <a-radio-group v-model="model.injury_classification" buttonStyle="solid">
                <a-radio-button class="text-red-500 bg-red-200" value="Severe">Severe</a-radio-button>
                <a-radio-button class="text-blue-500 bg-blue-200" value="Moderate">Moderate</a-radio-button>
                <a-radio-button class="text-green-500 bg-green-200" value="Mild">Mild</a-radio-button>
              </a-radio-group>
              <span
                class="m-2"
                :class="{'text-red-500' : model.injury_classification === 'Severe' ,
              'text-blue-500' : model.injury_classification === 'Moderate' ,
              'text-green-500' : model.injury_classification === 'Mild'
               }"
              >{{model.injury_classification}}</span>
            </a-form-item>
            <a-form-item
              style="display:block;"
              :labelCol="{span: 3}"
              required
              label="國籍"
              :validate-status="!!model.nation_type ? '' : 'error'"
              :help="!!model.nation_type ? ' ':'請輸國籍'"
            >
              <a-radio-group v-model="model.nation_type" buttonStyle="solid">
                <a-radio-button value="1">本國籍</a-radio-button>
                <a-radio-button value="2">非本國籍</a-radio-button>
              </a-radio-group>
            </a-form-item>
            <a-form-item style="display:block;" :labelCol="{span: 3}" label="救護車ID">
              <a-input v-model="model.amb_id" placeholder="救護車ID" />
            </a-form-item>
            <a-form-item
              style="display:block;"
              required
              :labelCol="{span: 3}"
              label="出發時間"
              :validate-status="!!model.start_date ? '' : 'error'"
              :help="!!model.start_date ? ' ':'請輸入出發時間'"
            >
              <a-date-picker
                :allowClear="false"
                format="YYYY-MM-DD HH:mm:ss"
                :value="moment(model.start_date, 'YYYY-MM-DD  HH:mm:ss')"
                :showTime="{ format:  'HH:mm:ss'  }"
                @change="(date, dateString) => SetModelDate(date, dateString, 'start_date')"
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
import Mixin from '@/mixin'
export default {
  mixins: [Mixin],
  data () {
    return {
      loading: false,
      selectPat: '',
      hospEvacuationVal: 0,
      totalVal: 0,
      model: this.SetNewModel()
    }
  },
  computed: {
    title () {
      return document.title
    }
  },
  methods: {
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
        this.$store.commit('SetPatList', this.model)
        this.model = this.SetNewModel()
        this.$refs.alert.showInfo('儲存成功!')
      }
      this.$store.commit('SpinLoading', false)
    },
    // 自動新增病人資料
    AutoCreatePatData () {
      let tempModel = this.SetNewModel()
      tempModel.char_no = this.$moment().format('YYYYMMDDHHmmss')
      tempModel.pat_name = this.$moment().format('mmss') + '病患'
      this.model = tempModel
      this.$refs.alert.showInfo('自動產生病人!')
    }
  }
}
</script>
