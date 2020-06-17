<template>
  <div>
    <a-card title="Form">
      <a-form>
        <a-form-item required label="Patient" :label-col="{ span: 8 }" :wrapper-col="{ span: 12 }">
          <a-input value="Tom" />
        </a-form-item>
        <a-form-item
          required
          label="Ambulance plate number"
          :label-col="{ span: 8 }"
          :wrapper-col="{ span: 12 }"
        >
          <a-input value="HT-1234" />
        </a-form-item>
        <a-table class="table-striped" :columns="columns" :dataSource="data" :pagination="false">
          <template slot="operation" slot-scope="text, record, index">
            <a-checkbox></a-checkbox>
          </template>
        </a-table>
      </a-form>
    </a-card>
  </div>
</template>
<script>
const columns = [
  {
    title: 'hospital',
    dataIndex: 'hospital',
    scopedSlots: { customRender: 'hospital' }
  },
  {
    title: 'level',
    dataIndex: 'level',
    scopedSlots: { customRender: 'level' }
  },
  {
    title: 'time',
    dataIndex: 'time',
    scopedSlots: { customRender: 'time' }
  },
  {
    title: '',
    dataIndex: 'operation',
    scopedSlots: { customRender: 'operation' }
  }
]
export default {
  data () {
    return {
      columns: columns,
      data: [{
        hospital: 'TMUH',
        level: '1',
        'time': 'about 32 min'
      },
      {
        hospital: 'SHH',
        level: '1',
        'time': 'about 20 min'
      },
      {
        hospital: 'WF',
        level: '1',
        'time': ' about 25 min'
      }, {
        hospital: 'TMUH',
        level: '1',
        'time': 'about 32 min'
      },
      {
        hospital: 'SHH',
        level: '1',
        'time': 'about 20 min'
      },
      {
        hospital: 'WF',
        level: '1',
        'time': ' about 25 min'
      }, {
        hospital: 'TMUH',
        level: '1',
        'time': 'about 32 min'
      },
      {
        hospital: 'SHH',
        level: '1',
        'time': 'about 20 min'
      },
      {
        hospital: 'WF',
        level: '1',
        'time': ' about 25 min'
      }, {
        hospital: 'TMUH',
        level: '1',
        'time': 'about 32 min'
      },
      {
        hospital: 'SHH',
        level: '1',
        'time': 'about 20 min'
      },
      {
        hospital: 'WF',
        level: '1',
        'time': ' about 25 min'
      }, {
        hospital: 'TMUH',
        level: '1',
        'time': 'about 32 min'
      },
      {
        hospital: 'SHH',
        level: '1',
        'time': 'about 20 min'
      },
      {
        hospital: 'WF',
        level: '1',
        'time': ' about 25 min'
      }, {
        hospital: 'TMUH',
        level: '1',
        'time': 'about 32 min'
      },
      {
        hospital: 'SHH',
        level: '1',
        'time': 'about 20 min'
      },
      {
        hospital: 'WF',
        level: '1',
        'time': ' about 25 min'
      }]
    }
  }
}
</script>
