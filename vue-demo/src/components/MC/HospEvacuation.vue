<style scoped>
</style>
<template>
  <div class="m-4">
    <a-card title="到達醫院狀況時間圖">
      <span slot="extra">
          {{hosp_id}}
        <span class="m-2">請選擇醫院:</span>
        <select v-model="hospkey" class="form-control inline w-64 mx-2">
          <option value="">所有醫院</option>
          <option
            v-for="(i, index) in hospList"
            :key="index"
            :value="i.HOSP_KEY"
          >
            {{ i.HOSPITAL_SHOW_NAME }}{{ i.HOSP_KEY }}
          </option>
        </select>
      </span>
      <div>
        查詢時間: {{ now }} <br />
        <span class="bg-red-300">
          <span class="m-2">總人數:{{ patlist.length }}</span>
          <span class="m-2"
            >送出人數: {{ patlist.filter((x) => x.HOSP_KEY).length }}</span
          >
          <span class="m-2"
            >未送人數: {{ patlist.filter((x) => !x.HOSP_KEY).length }}</span
          >
          <span class="m-2"
            >重傷:
            {{ patlist.filter((x) => x.TRIAGE === "Severe").length }}</span
          >
          <span class="m-2"
            >中傷:
            {{ patlist.filter((x) => x.TRIAGE === "Moderate").length }}</span
          >
          <span class="m-2"
            >輕傷: {{ patlist.filter((x) => x.TRIAGE === "Mild").length }}</span
          >
        </span>
      </div>
      <Chart :chartData="chartData"></Chart>
    </a-card>
    <patientList
      :config="{
        hospid: hospkey,
        siteid: site_id,
      }"
    ></patientList>
  </div>
</template>
<script>
import Chart from '@/components/chart'
import patientList from '@/components/MC/patListLlist'
import Mixin from '@/mixin'
export default {
  mixins: [Mixin],
  components: { Chart, patientList },
  data () {
    return {
      hospkey: this.hosp_id

    }
  },
  computed: {
    chartlist () {
      return {
        Severe: this.pointData('Severe'),
        Moderate: this.pointData('Moderate'),
        Mild: this.pointData('Mild'),
        atSevere: this.atPointData('Severe'),
        atModerate: this.atPointData('Moderate'),
        atMild: this.atPointData('Mild')
      }
    },
    chartData () {
      return {
        datasets: [
          {
            label: '尚未抵達重傷',
            backgroundColor: 'rgba(255, 0, 0)',
            borderColor: 'rgba(255, 0, 0)',
            fill: false,
            borderDash: [5, 5],
            data: this.chartlist.Severe
          },
          {
            label: '已抵達重傷',
            backgroundColor: 'rgba(255, 0, 0)',
            borderColor: 'rgba(255, 0, 0)',
            fill: false,
            data: this.chartlist.atSevere
          },
          {
            label: '尚未抵達中傷',
            backgroundColor: 'rgba(239, 192, 40)',
            borderColor: 'rgba(239, 192, 40)',
            fill: false,
            borderDash: [5, 5],
            data: this.chartlist.Moderate
          },
          {
            label: '已抵達中傷',
            backgroundColor: 'rgba(239, 192, 40)',
            borderColor: 'rgba(239, 192, 40)',
            fill: false,
            data: this.chartlist.atModerate
          },
          {
            label: '尚未抵達輕傷',
            backgroundColor: 'rgba(44, 130, 201)',
            borderColor: 'rgba(44, 130, 201)',
            fill: false,
            borderDash: [5, 5],
            data: this.chartlist.Mild
          },
          {
            label: '已抵達輕傷',
            backgroundColor: 'rgba(44, 130, 201)',
            borderColor: 'rgba(44, 130, 201)',
            fill: false,
            data: this.chartlist.atMild
          }
        ]
      }
    },
    hospList () {
      let vuethis = this
      let pList = []
      vuethis.$store.state.Basic.PatListByID.forEach(element => {
        if (element.HOSP_KEY && pList.filter((x) => x.HOSP_KEY === element.HOSP_KEY).length === 0) {
          pList.push(element)
        }
      })
      return pList
    },
    patlist () {
      let vuethis = this
      if (vuethis.hospkey) {
        if (vuethis.hospkey === 'hospid') {
          return vuethis.$store.state.Basic.PatListByID.filter((x) => !x.HOSP_KEY)
        }
        return vuethis.$store.state.Basic.PatListByID.filter((x) => x.HOSP_KEY === vuethis.hospkey)
      }
      return vuethis.$store.state.Basic.PatListByID
      // return []
    },
    title () {
      return document.title
    }
  },
  methods: {
    onlyUnique (value, index, self) {
      return self.indexOf(value.HOSP_KEY) === index
    },
    pointData (pVal) {
      let pList = []
      let vuethis = this
      let tempList = vuethis.patlist.filter((x) => x.TRIAGE === pVal && x.jsSELECTION_DATETIME)
      tempList.map(item => item.jsSELECTION_DATETIME)
        .filter((value, index, self) => self.indexOf(value) === index).sort(function (a, b) { return a - b }).forEach(element => {
          let sumVal = 0
          if (pList.length > 0) {
            sumVal = pList.map(cc => cc.y)[pList.length - 1]
          }
          pList.push({
            x: element,
            y: (sumVal +
              tempList.filter((x) => x.jsSELECTION_DATETIME === element).length)
          })
        })
      return pList
    },
    atPointData (pVal) {
      let pList = []
      let vuethis = this
      let tempList = vuethis.patlist.filter((x) => x.TRIAGE === pVal && x.jsEXPECTED_ARRIVAL_DATETIME)
      tempList.map(item => item.jsEXPECTED_ARRIVAL_DATETIME)
        .filter((value, index, self) => self.indexOf(value) === index).sort(function (a, b) { return a - b }).forEach(element => {
          let sumVal = 0
          if (pList.length > 0) {
            sumVal = pList.map(cc => cc.y)[pList.length - 1]
          }
          pList.push({
            x: element,
            y: (sumVal +
              tempList.filter((x) => x.jsEXPECTED_ARRIVAL_DATETIME === element).length)
          })
        })
      return pList
    }
  }
}
</script>
