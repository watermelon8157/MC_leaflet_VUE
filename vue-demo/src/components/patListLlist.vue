<template>
  <div>
    <a-card title="Patient List">
      <a-table :columns="columns" :dataSource="data" :pagination="false" bordered>
        <template slot="Last_name" slot-scope="text, record">
          <a-icon
            type="user"
            class="p-2 pb-2"
            :class="{'text-red-500': record.Gender === '女','text-blue-500': record.Gender === '男'}"
          />
          {{text}}
        </template>
      </a-table>
    </a-card>
  </div>
</template>

<script>
const columns = [
  {
    title: 'Date',
    dataIndex: 'Date',
    scopedSlots: { customRender: 'Date' }
  },
  {
    title: 'Departure',
    className: 'Departure',
    dataIndex: 'Departure'
  },
  {
    title: 'Arrival',
    dataIndex: 'Arrival'
  },
  {
    title: 'Gender',
    dataIndex: 'Gender'
  },
  {
    title: 'Age',
    dataIndex: 'Age'
  },
  {
    title: 'Last name',
    dataIndex: 'Last_name'
  },
  {
    title: 'Triage',
    dataIndex: 'Triage'
  },
  {
    title: 'Sever',
    dataIndex: 'Sever'
  },
  {
    title: 'Minor',
    dataIndex: 'Minor'
  },
  {
    title: '119 code',
    dataIndex: 'code119'
  },
  {
    title: 'Hospital',
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
