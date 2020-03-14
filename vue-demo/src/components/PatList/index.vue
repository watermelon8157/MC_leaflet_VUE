<style scoped>
</style>
<template>
 <div class="m-4">
    {{patList}}
    <a-table
    :columns="columns"
    :rowKey="record => record.char_no"
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
    scopedSlots: { customRender: 'char_no' }
  },
  {
    title: '病患姓名',
    dataIndex: 'pat_name',
    scopedSlots: { customRender: 'pat_name' }
  },
  {
    title: '年齡',
    dataIndex: 'ageType',
    scopedSlots: { customRender: 'ageType' }
  },
  {
    title: '傷勢',
    dataIndex: 'injury_classification',
    scopedSlots: { customRender: 'injury_classification' }
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
