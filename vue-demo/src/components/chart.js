// https://vue-chartjs.org/
// https://chartjs-doc.abingoal.com
// https://www.chartjs.org/
import vuechart from 'vue-chartjs'
export default {
  extends: vuechart.HorizontalBar,
  props: {
    chartdata: {
      type: Object,
      default: null
    },
    chartoptions: {
      type: Object,
      default: null
    }
  },
  data () {
    return {
      data: {
        labels: ['林口長庚', '淡水馬偕'],
        datasets: [{
          label: 'Arrival 119 Pserson',
          data: [12, 19],
          stack: 'Stack 0',
          backgroundColor: 'rgba(255, 200, 132, 0.2)',
          borderColor: 'rgba(255, 200, 132, 1)',
          borderWidth: 1
        }, {
          label: 'Arrival self Pserson',
          data: [12, 19],
          stack: 'Stack 0',
          backgroundColor: 'rgba(255, 99, 132, 0.2)',
          borderColor: 'rgba(255, 99, 132, 1)',
          borderWidth: 1
        }, {
          label: 'PreArrival 119 person',
          data: [12, 19],
          stack: 'Stack 1',
          backgroundColor: 'rgba(150, 99, 00, 0.2)',
          borderColor: 'rgba(255, 99, 132, 1)',
          borderWidth: 1
        }, {
          label: 'PreArrival self person',
          data: [4, 12],
          stack: 'Stack 1',
          backgroundColor: 'rgba(255, 99, 00, 0.2)',
          borderColor: 'rgba(255, 99, 132, 1)',
          borderWidth: 1
        }]
      },
      options: {
        legendCallback (chart) {
          var legendHtml = []
          legendHtml.push('<table>')
          var item = chart.data.datasets[0]
          for (var i = 0; i < item.data.length; i++) {
            legendHtml.push('<tr><td>')
            legendHtml.push('<span class="chart-legend" style="background-color:' + item.backgroundColor[i] + '"></span>')
            legendHtml.push('<span class="chart-legend-label-text">' + item.data[i] + ' person - ' + chart.data.labels[i] + ' times</span>')
            legendHtml.push('</td></tr>')
          }

          legendHtml.push('</table>')
          return legendHtml.join('')
        },
        responsive: true,
        maintainAspectRatio: false
      }
    }
  },
  mounted () {
    this.renderChart(
      Object.assign(this.data, this.chartdata),
      Object.assign(this.options, this.chartoptions))
    this.$emit('emitLegend', this.generateLegend())
  }
}
