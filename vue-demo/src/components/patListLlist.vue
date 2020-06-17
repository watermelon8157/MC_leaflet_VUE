<template>
  <div class="bg-white">
    <a-card title="傷患清單">
      <a-table
        class="table-striped"
        :columns="columns"
        :dataSource="data"
        :pagination="false"
        bordered
      >
        <template slot="event" slot-scope="text, record">
          1111
        </template>
        <template slot="Triage" slot-scope="text, record">
          Severe
        </template>
      </a-table>
    </a-card>
  </div>
</template>

<script>
const columns = [
  {
    title: '事件',
    dataIndex: 'event',
    scopedSlots: { customRender: 'event' }
  },
  {
    title: '傷勢',
    dataIndex: 'Triage',
    scopedSlots: { customRender: 'Triage' }
  },
  {
    title: '日期',
    dataIndex: 'Date',
    scopedSlots: { customRender: 'Date' }
  },
  {
    title: '出發時間',
    className: 'Departure',
    dataIndex: 'Departure'
  },
  {
    title: '抵達時間',
    dataIndex: 'Arrival'
  },
  {
    title: '性別',
    dataIndex: 'Gender'
  },
  {
    title: '年齡',
    dataIndex: 'Age'
  },
  {
    title: '姓名',
    dataIndex: 'Last_name'
  },
  {
    title: '救護車代碼',
    dataIndex: 'code119'
  },
  {
    title: '醫院',
    dataIndex: 'Hospital'
  }
]

const data = [
  { Date: '2019/6/27', Departure: '21:00', Arrival: '21:15', Gender: '女', Age: '21', Last_name: 'Wang', Triage: '80%，2-3度', Sever: 'V', Minor: '', code119: 'A92', Hospital: '淡水馬偕' },
  { Date: '2019/6/28', Departure: '21:00', Arrival: '21:15', Gender: '女', Age: '21', Last_name: 'NA', Triage: '30-40%，2-3度', Sever: 'V', Minor: '', code119: 'A93', Hospital: '淡水馬偕' },
  { Date: '2019/6/29', Departure: '21:00', Arrival: '21:15', Gender: '女', Age: '17', Last_name: 'Lee', Triage: '30-40%，2-3度', Sever: 'V', Minor: '', code119: 'A94', Hospital: '淡水馬偕' },
  { Date: '2019/6/30', Departure: '21:00', Arrival: '21:15', Gender: '男', Age: '21', Last_name: 'Lee', Triage: '30-40%，2-3度', Sever: 'V', Minor: '', code119: 'A95', Hospital: '淡水馬偕' },
  { Date: '2019/6/31', Departure: '20:54', Arrival: '21:15', Gender: '女', Age: '19', Last_name: 'Shung', Triage: '2度 38%', Sever: 'V', Minor: '', code119: 'B92', Hospital: '淡水馬偕' },
  { Date: '2019/6/32', Departure: '20:54', Arrival: '21:15', Gender: '女', Age: '21', Last_name: 'Chung', Triage: '2度 57%', Sever: 'V', Minor: '', code119: 'B93', Hospital: '林口長庚' },
  { Date: '2019/6/33', Departure: '21:45', Arrival: '22:01', Gender: '男', Age: '26', Last_name: 'Dai', Triage: '2度 40%', Sever: 'V', Minor: '', code119: 'C92', Hospital: '林口長庚' },
  { Date: '2019/6/34', Departure: '21:28', Arrival: '21:44', Gender: '男', Age: '26', Last_name: 'Hung', Triage: '2度 72%', Sever: 'V', Minor: '', code119: 'C93', Hospital: '林口長庚' },
  { Date: '2019/6/35', Departure: '21:28', Arrival: '21:44', Gender: '男', Age: '24', Last_name: 'Chen', Triage: '2度 3% ', Sever: '', Minor: 'V', code119: 'C94', Hospital: '三總' },
  { Date: '2019/6/36', Departure: '21:28', Arrival: '21:44', Gender: '女', Age: '20', Last_name: 'Mon', Triage: '2度 10%', Sever: '', Minor: 'V', code119: 'C95', Hospital: '淡水馬偕' }
]

export default {
  name: 'patList',
  data () {
    return {
      data,
      columns
    }
  }
}
</script>
<style>
th.column-money,
td.column-money {
  text-align: right !important;
}
</style>
