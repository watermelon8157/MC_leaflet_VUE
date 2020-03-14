<template>
  <div>
    <a-card title="Chart in hospital status">
      <span slot="extra">more</span>
      <Chart></Chart>
    </a-card>

    <patientList></patientList>
  </div>
</template>
<script>
import Chart from '@/components/chart'
import patientList from '@/components/patListLlist'

export default {
  components: { Chart, patientList },
  data () {
    return {

    }
  }
}
</script>
