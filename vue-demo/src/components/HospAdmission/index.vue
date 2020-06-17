<style scoped>
</style>
<template>
  <div class="m-4">
    <Chart></Chart>
  </div>
</template>
<script>
import Chart from '@/components/ChartHospcopy.vue'
import Mixin from '@/mixin'
export default {
  mixins: [Mixin],
  components: { Chart },
  data () {
    return {

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
