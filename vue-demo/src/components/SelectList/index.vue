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
            :rowKey="record => record.char_no"
            :dataSource="data"
            :loading="loading"
            size="small"
            :customRow="clickCustomRow"
          >
            <template slot="char_no" slot-scope="text,record">{{text}}</template>
            <template slot="pat_name" slot-scope="text,record">{{text}}</template>
            <template slot="injury_classification" slot-scope="text,record">
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
import VueLeaflet from '@/components/VueLeaflet'
import Mixin from '@/mixin'
const columns = [
  {
    title: '病歷編號',
    dataIndex: 'char_no',
    scopedSlots: { customRender: 'char_no' }
  },
  {
    title: '姓名',
    dataIndex: 'pat_name',
    scopedSlots: { customRender: 'pat_name' }
  },
  {
    title: '傷勢',
    dataIndex: 'injury_classification',
    scopedSlots: { customRender: 'injury_classification' }
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
      temp = temp.sort((a, b) => (a.injury_classification > b.injury_classification) ? 1 : -1)
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
      if (record.char_no === vuethis.model.char_no) {
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
      if (this.model.char_no) {
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
