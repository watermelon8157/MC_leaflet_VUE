// https://vue-chartjs.org/
// https://chartjs-doc.abingoal.com
// https://www.chartjs.org/
import { Line, mixins } from 'vue-chartjs'
export default {
  extends: Line,
  mixins: [mixins.reactiveProp],
  props: {
    chartData: {
      type: Object,
      default: null
    }
  },
  data () {
    return {
      data: {},
      options: {
        responsive: true,
        maintainAspectRatio: false,
        title: {
          display: true,
          text: ''
        },
        scales: {
          xAxes: [
            {
              type: 'time',
              distribution: 'series',
              display: true,
              scaleLabel: {
                display: true,
                labelString: 'Date'
              },
              time: {
                min: this.$moment(this.eDate, 'YYYY-MM-DD'),
                max: this.$moment(this.sDate, 'YYYY-MM-DD')
              }
            }
          ]
        },
        legendCallback (chart) {
          var legendHtml = []
          return legendHtml.join('')
        }
      }
    }
  },
  mounted () {
    console.log(this.chart)
    this.renderChart(this.chartData, this.options)
    this.$emit('emitLegend', this.generateLegend())
  }
}
