<template>
  <div>
    <a-card title="到達醫院狀況時間圖">
      <span v-if="showHospSelect" slot="extra">
        <span class="m-2">請選擇醫院:</span>
        <select class="form-control inline w-64 mx-2">
          <option>所有醫院</option>
          <option>雙和</option>
          <option>萬芳</option>
          <option>北醫</option>
        </select>
      </span>
      <Chart :chartData="chart"></Chart>
    </a-card>
  </div>
</template>
<script>
import Chart from '@/components/chart'
import Mixin from '@/mixin'
const chartlist = [
  {
    label: '重傷',
    data: [
      { x: 1587839422000, y: 0 },
      { x: 1587867275000, y: 0 },
      { x: 1587925376000, y: 5 },
      { x: 1587941159000, y: 5 },
      { x: 1587961262000, y: 5 },
      { x: 1587970216000, y: 5 },
      { x: 1587982423000, y: 10 },
      { x: 1588011404000, y: 10 },
      { x: 1588038050000, y: 10 },
      { x: 1588055583000, y: 20 },
      { x: 1588098474000, y: 20 },
      { x: 1588123454000, y: 35 },
      { x: 1588140358000, y: 35 },
      { x: 1588181315000, y: 35 },
      { x: 1588201687000, y: 40 },
      { x: 1588209404000, y: 45 },
      { x: 1588215504000, y: 47 },
      { x: 1588225543000, y: 50 },
      { x: 1588239167000, y: 51 },
      { x: 1588256358000, y: 60 },
      { x: 1588269567000, y: 60 },
      { x: 1588295528000, y: 60 },
      { x: 1588351221000, y: 60 },
      { x: 1588383387000, y: 60 }
    ]
  },
  {
    label: '中傷',
    data: [
      { x: 1587839422000, y: 0 },
      { x: 1587867275000, y: 0 },
      { x: 1587925376000, y: 0 },
      { x: 1587941159000, y: 0 },
      { x: 1587961262000, y: 0 },
      { x: 1587970216000, y: 0 },
      { x: 1587982423000, y: 0 },
      { x: 1588011404000, y: 0 },
      { x: 1588038050000, y: 0 },
      { x: 1588055583000, y: 5 },
      { x: 1588098474000, y: 6 },
      { x: 1588123454000, y: 7 },
      { x: 1588140358000, y: 8 },
      { x: 1588181315000, y: 9 },
      { x: 1588201687000, y: 10 },
      { x: 1588209404000, y: 11 },
      { x: 1588215504000, y: 12 },
      { x: 1588225543000, y: 13 },
      { x: 1588239167000, y: 14 },
      { x: 1588256358000, y: 15 },
      { x: 1588269567000, y: 16 },
      { x: 1588295528000, y: 20 },
      { x: 1588351221000, y: 20 },
      { x: 1588383387000, y: 20 }
    ]
  },
  {
    label: '輕傷',
    data: [
      { x: 1587839422000, y: 0 },
      { x: 1587867275000, y: 0 },
      { x: 1587925376000, y: 0 },
      { x: 1587941159000, y: 0 },
      { x: 1587961262000, y: 0 },
      { x: 1587970216000, y: 0 },
      { x: 1587982423000, y: 0 },
      { x: 1588011404000, y: 0 },
      { x: 1588038050000, y: 0 },
      { x: 1588055583000, y: 0 },
      { x: 1588098474000, y: 0 },
      { x: 1588123454000, y: 0 },
      { x: 1588140358000, y: 0 },
      { x: 1588181315000, y: 2 },
      { x: 1588201687000, y: 2 },
      { x: 1588209404000, y: 2 },
      { x: 1588215504000, y: 2 },
      { x: 1588225543000, y: 2 },
      { x: 1588239167000, y: 4 },
      { x: 1588256358000, y: 4 },
      { x: 1588269567000, y: 4 },
      { x: 1588295528000, y: 4 },
      { x: 1588351221000, y: 6 },
      { x: 1588383387000, y: 6 }
    ]
  }
]
export default {
  mixins: [Mixin],
  components: { Chart },
  data () {
    return {
      chart: {
        datasets: [
          {
            label: '尚未送出',
            backgroundColor: 'rgba(105,105,105)',
            borderColor: 'rgba(105,105,105)',
            fill: false,
            borderDash: [5, 5],
            data: this.pointData('重傷', 10)
          },
          {
            label: '已送出',
            backgroundColor: 'rgba(105,105,105)',
            borderColor: 'rgba(105,105,105)',
            fill: false,
            data: this.pointData('重傷', 5)
          },
          {
            label: '重傷',
            backgroundColor: 'rgba(255, 0, 0)',
            borderColor: 'rgba(255, 0, 0)',
            fill: false,
            data: this.pointData('重傷')
          },
          {
            label: '中傷',
            backgroundColor: 'rgba(239, 192, 40)',
            borderColor: 'rgba(239, 192, 40)',
            fill: false,
            data: this.pointData('中傷')
          },
          {
            label: '輕傷',
            backgroundColor: 'rgba(44, 130, 201)',
            borderColor: 'rgba(44, 130, 201)',
            fill: false,
            data: this.pointData('輕傷')
          }
        ]
      }
    }
  },
  computed: {
    showHospSelect () {
      console.log(document.URL)
      if (document.URL) {
        return (document.URL.indexOf('/Hosp/') < 0)
      }
      return false
    }
  },
  methods: {
    pointData (pKey, pval) {
      let temp = []
      console.log(chartlist)
      if (chartlist) {
        chartlist.forEach(element => {
          if (element.label === pKey) {
            element.data.forEach(e => {
              let t = JSON.parse(JSON.stringify(e))
              if (pval) {
                t.y = t.y + this._.random(1.2, pval)
              }
              temp.push(t)
            })
          }
        })
      }
      return temp
    }
  }
}
</script>
