<style scoped>
</style>
<template>
  <div>
    <a-row>
      <a-col :span="24">
        <a-card>
          <span slot="title">選擇病人</span>
          <span slot="extra">
            後送人數 共
            <span class="m-1">{{patListLength}}</span>人
          </span>
          <a-table
            :columns="columns"
            :rowClassName="RowClassName"
            :rowKey="record => record.PATIENT_ID"
            :dataSource="data"
            :loading="loading"
            size="small"
            :customRow="clickCustomRow"
          >
            <template slot="PATIENT_ID" slot-scope="text,record">{{text}}</template>
            <template slot="PATIENT_NAME" slot-scope="text,record">{{text}}</template>
            <template slot="TRIAGE" slot-scope="text,record">
              <div
                :class="{'text-red-500' : text === 'Severe' ,
              'text-blue-500' : text === 'Moderate' ,
              'text-green-500' : text === 'Mild'
               }"
              >{{text}}</div>
            </template>
          </a-table>
        </a-card>
      </a-col>
    </a-row>
    <a-row>
      <a-col :span="24" class="bg-white">
        <Alert class="m-2" ref="alert"></Alert>
        <VueLeaflet ref="map" @emitSelectPatData="emitSelectPatData"></VueLeaflet>
      </a-col>
    </a-row>
  </div>
</template>
<script>
import VueLeaflet from '@/components/SelectList/VueLeaflet'
import Mixin from '@/mixin'
const columns = [
  {
    title: '病歷編號',
    dataIndex: 'PATIENT_ID',
    scopedSlots: { customRender: 'PATIENT_ID' }
  },
  {
    title: '姓名',
    dataIndex: 'PATIENT_NAME',
    scopedSlots: { customRender: 'PATIENT_NAME' }
  },
  {
    title: '傷勢',
    dataIndex: 'TRIAGE',
    scopedSlots: { customRender: 'TRIAGE' }
  }
]
export default {
  mixins: [Mixin],
  components: {
    VueLeaflet
  },
  data () {
    return {
      model: this.SetNewModel(),
      loading: false,
      columns
    }
  },
  computed: {
    data () {
      var vuethis = this
      let temp = Object.assign(vuethis.patList, {})
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
        this.$refs.alert.showInfo('病患已送出!')
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
