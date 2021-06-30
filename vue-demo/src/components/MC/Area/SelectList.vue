<style scoped>
</style>
<template>
  <div>
    <a-row>
      <a-col :span="24">
        <a-card>
          <span slot="title">從待送清單選擇病人</span>
          <span slot="extra">
            待送清單剩餘人數 共
            <span class="m-1">{{ PatList.length }}</span
            >人
          </span>
          <a-table
            class="table-striped"
            :columns="columns"
            :rowClassName="RowClassName"
            :rowKey="(record) => record.PATIENT_ID"
            :dataSource="dataSource"
            :loading="loading"
            size="small"
            :customRow="clickCustomRow"
          >
            <template slot="PATIENT_ID" slot-scope="text, record">{{
              text
            }}</template>
            <template slot="PATIENT_NAME" slot-scope="text, record">{{
              text
            }}</template>
            <template slot="TRIAGE" slot-scope="text, record">
              <div
                :class="{
                  'text-red-500': text === 'Severe',
                  'text-blue-500': text === 'Moderate',
                  'text-green-500': text === 'Mild',
                }"
              >
                {{ text }}
              </div>
            </template>
          </a-table>
        </a-card>
      </a-col>
    </a-row>
    <a-row>
      <a-col :span="24" class="bg-white">
        <Alert class="m-2" ref="alert"></Alert>
      </a-col>
    </a-row>
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
    title: '傷勢',
    dataIndex: 'TRIAGE',
    scopedSlots: { customRender: 'TRIAGE' }
  },
  {
    title: '填寫時間',
    dataIndex: 'CREATE_DATE',
    scopedSlots: { customRender: 'CREATE_DATE' }
  }
]
export default {
  mixins: [Mixin],
  components: {
  },
  data () {
    return {
      model: this.SetNewModel(),
      loading: false,
      columns
    }
  },
  mounted () {
    let vuethis = this
    vuethis.$api.MC.GetPatListByID({ site_id: vuethis.site_id, hosp_id: vuethis.hosp_id }).then((result) => {
      vuethis.$store.commit({
        type: 'Basic/SetPatListBYID',
        data: result.data
      })
    }).catch((err) => {
      console.log(err)
      this.error(err)
    })
  },
  computed: {
    PatList () {
      var vuethis = this
      return vuethis.$store.state.Basic.PatListByID.filter(x => !x.HOSP_KEY)
    },
    dataSource () {
      var vuethis = this
      let temp = Object.assign(vuethis.PatList, {})
      temp = temp.sort((a, b) => (a.TRIAGE > b.TRIAGE) ? 1 : -1)
      temp = temp.reverse()
      return temp
    },
    title () {
      return document.title
    }
  },
  methods: {
    RowClassName (record, index) {
      var vuethis = this
      if (record.PATIENT_ID === vuethis.model.PATIENT_ID) {
        return 'bg-yellow-200'
      }
      return ''
    },
    clickCustomRow (record, index) {
      // 點選row
      var vuethis = this
      return {
        on: {
          click: function () {
            vuethis.model = Object.assign(record, {})
            vuethis.$store.commit({
              type: 'Basic/SetPatModel',
              data: vuethis.model
            })
            vuethis.$router.push({ name: 'mapindex', params: { now: vuethis.now } })
          }
        }
      }
    },
    emitSelectPatData () {
      this.$store.commit('SpinLoading', true)
      if (this.model.PATIENT_ID) {
        this.$store.commit('SetSelctPatList', this.model)
        this.$refs.map.onClose()
        this.model = this.SetNewModel()
        this.$notification.info({
          message: '請請選擇病人!'
        })
        this.$refs.alert.showInfo('傷患已送出!')
      } else {
        this.$notification.error({
          message: '請請選擇病人!'
        })
        this.$refs.alert.show('請請選擇病人!')
      }
      this.$store.commit('SpinLoading', false)
    }
  }
}
</script>
