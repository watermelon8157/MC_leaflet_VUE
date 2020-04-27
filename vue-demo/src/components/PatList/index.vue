<style scoped>
</style>
<template>
 <div class="m-4">
    {{patList}}
    <a-table
    :columns="columns"
    :rowKey="record => record.PATIENT_ID"
    :dataSource="patList"
  >
  </a-table>
  </div>
</template>
<script>
import Mixin from '@/mixin'
const columns = [
  {
    title: '病歷號',
    dataIndex: 'name',
    scopedSlots: { customRender: 'PATIENT_ID' }
  },
  {
    title: '病患姓名',
    dataIndex: 'PATIENT_NAME',
    scopedSlots: { customRender: 'PATIENT_NAME' }
  },
  {
    title: '年齡',
    dataIndex: 'ageType',
    scopedSlots: { customRender: 'ageType' }
  },
  {
    title: '傷勢',
    dataIndex: 'TRIAGE',
    scopedSlots: { customRender: 'TRIAGE' }
  },
  {
    title: '救護車',
    dataIndex: 'amb_id',
    scopedSlots: { customRender: 'amb_id' }
  },
  {
    title: '狀態',
    dataIndex: 'datastatus',
    scopedSlots: { customRender: 'datastatus' }
  }
]
export default {
  mixins: [Mixin],
  data () {
    return {
      columns
    }
  },
  computed: {
    title () {
      return document.title
    }
  },
  methods: {

  }
}
</script>
