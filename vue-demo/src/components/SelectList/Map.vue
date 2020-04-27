<template>
  <div v-if="loading">
    <a-drawer
      height="300"
      placement="bottom"
      :closable="false"
      :mask="false"
      @close="onClosePatList"
      :visible="visiblePatList"
    >
      <div slot="title">
        <div class="float-right">
          <a-input
            placeholder="快速索引"
            class="w-32"
            :defaultValue="searchText"
            @blur="v => TextBlur(v, 'searchText')"
          />
          <a-button shape="circle" icon="search" @click="onSearch" />
        </div>
        病患清單 共{{PatLength}}人
      </div>
      <a-table
        :pagination="false"
        :columns="columns"
        :rowClassName="RowClassName"
        :rowKey="record => record.PATIENT_ID"
        :dataSource="chunk"
        :loading="loadingPatList"
        size="small"
        :customRow="clickCustomRow"
      >
        <template slot="PATIENT_ID" slot-scope="text,record">{{text}}</template>
        <template slot="PATIENT_NAME" slot-scope="text,record">{{text}}</template>
        <template slot="TRIAGE" slot-scope="text,record">
          <div
            :class="{'text-red-500' : text === 'Severe' ,
              'text-blue-500' : text === 'Moderate' ,
              'text-green-500' : text === 'Mild'
               }"
          >{{text}}</div>
        </template>
      </a-table>
      <div
        v-if="model.hosp_desc"
        class="mt-2 bg-teal-100 border-t-4 border-teal-500 rounded-b text-teal-900 px-4 py-3 shadow-md"
        role="alert"
      >
        <div class="flex">
          <div class="w-full">
            <a-button-group class="float-right">
              <a-button type="primary" @click="selectPatData">送出</a-button>
            </a-button-group>
            <p>
              <span class="font-bold">{{model.hosp_name}}</span>
              <span class="m-2">({{getDistance(model.location)}})</span>
            </p>
            <div>
              <a-form layout="inline">
                <a-form-item label="醫院分數">
                  <a-tag color="red" class="text-red-500 font-extrabold">{{model.hosp_source}}</a-tag>
                  <a-divider class="bg-orange-500" type="vertical" />
                </a-form-item>
                <a-form-item label="醫院收治人數">
                  <a-tag class="font-extrabold">{{model.hosp_ihp}}</a-tag>
                  <a-divider class="bg-orange-500" type="vertical" />
                </a-form-item>
                <a-form-item label="醫院後送人數">
                  <a-tag class="font-extrabold">{{model.hosp_whp}}</a-tag>
                  <a-divider class="bg-orange-500" type="vertical" />
                </a-form-item>
                <a-form-item label="衛福部業務組別">
                  <a-tag class="font-extrabold">{{model.hosp_class}}</a-tag>
                  <a-divider class="bg-orange-500" type="vertical" />
                </a-form-item>
                <a-form-item label="縣市">
                  <a-tag class="font-extrabold">{{model.hosp_city}}</a-tag>
                  <a-divider class="bg-orange-500" type="vertical" />
                </a-form-item>
                <a-form-item label="緊急醫療能力">
                  <a-tag class="font-extrabold">{{model.hosp_injury}}</a-tag>
                  <a-divider class="bg-orange-500" type="vertical" />
                </a-form-item>
                <a-form-item label="急救責任等級">
                  <a-tag class="font-extrabold">{{model.hosp_ranking}}</a-tag>
                  <a-divider class="bg-orange-500" type="vertical" />
                </a-form-item>
                <a-form-item label="急診觀察床">
                  <a-tag class="font-extrabold">{{model.hosp_erbed}}</a-tag>
                  <a-divider class="bg-orange-500" type="vertical" />
                </a-form-item>
              </a-form>
            </div>
          </div>
        </div>
      </div>
      <div
        v-else
        class="mt-2 bg-orange-100 border-l-4 border-orange-500 text-orange-700 p-4"
        role="alert"
      >
        <p class="font-bold">請選擇醫院</p>
        <p></p>
      </div>
    </a-drawer>
    <div class="vue-leaflet">
      <l-map
        ref="Map"
        :style="{'height' : windowHeight/8*6 + 'px' }"
        style="width: 100%;  "
        :zoom="zoom"
        :center="center"
        @update:zoom="zoomUpdated"
        @update:center="centerUpdated"
        @update:bounds="boundsUpdated"
      >
        <l-tile-layer :url="url" :attribution="attribution"></l-tile-layer>
        <l-polyline :lat-lngs="polyline" color="red"></l-polyline>
        <l-marker :lat-lng="circleMarker" :icon="plusMarkerIcon"></l-marker>
        <l-marker :lat-lng="sideMarker"></l-marker>
        <l-marker ref="markerLocation" :lat-lng="location" :icon="centerIcon">
          <l-popup :content="text"></l-popup>
        </l-marker>
        <l-marker
          v-for="(i,index) in markerList"
          :key="index"
          :lat-lng="i.location"
          @click="(e)=>open(i)"
          :icon="get_hosp_color(i.hosp_ranking)"
        >
          <l-popup :content="i.hosp_name"></l-popup>
        </l-marker>
        <l-circle-marker :lat-lng="circleMarker" :radius="zoom*10" color="#bee3f8" />
      </l-map>
    </div>
  </div>
</template>

<script>
import { LMap, LTileLayer, LMarker, LPopup, LCircleMarker, LPolyline } from 'vue2-leaflet'
import Pngbarn from '@/assets/pngbarn.png'
import markerblue from '@/assets/marker/marker-icon-2x-blue.png'
import markergold from '@/assets/marker/marker-icon-2x-gold.png'
import markergreen from '@/assets/marker/marker-icon-2x-green.png'
import markerred from '@/assets/marker/marker-icon-2x-red.png'
import markerviolet from '@/assets/marker/marker-icon-2x-violet.png'
import markershadow from '@/assets/marker/marker-shadow.png'
import L from 'leaflet'
import Mixin from '@/mixin'
const columns = [
  {
    title: '病歷編號',
    dataIndex: 'PATIENT_ID',
    scopedSlots: { customRender: 'PATIENT_ID' }
  },
  {
    title: '姓名',
    dataIndex: 'PATIENT_NAME',
    scopedSlots: { customRender: 'PATIENT_NAME' }
  },
  {
    title: '傷勢',
    dataIndex: 'TRIAGE',
    scopedSlots: { customRender: 'TRIAGE' }
  }
]
export default {
  mixins: [Mixin],
  name: 'VueLeaflet',
  components: {
    LMap,
    LTileLayer,
    LMarker,
    LPopup,
    LCircleMarker,
    LPolyline
  },
  data () {
    return {
      List: [],
      searchText: '',
      markerred,
      markershadow,
      columns,
      model: {
        hosp_desc: '',
        hosp_name: '',
        hosp_class: '',
        hosp_city: '',
        hosp_injury: '',
        hosp_ranking: '',
        hosp_erbed: '',
        hosp_distance: '0',
        hosp_source: 0,
        hosp_ihp: 0,
        hosp_whp: 0,
        location: []
      },
      testL: 23.6,
      testI: 121,
      visiblePatList: true,
      visible: false,
      loading: false,
      loadingPatList: false,
      zoom: 12,
      center: L.latLng(23.6, 121),
      url: 'http://{s}.tile.osm.org/{z}/{x}/{y}.png',
      attribution: '&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors',
      location: L.latLng(23.6, 121),
      circleMarker: L.latLng(23.6, 121),
      text: '現在位置',
      Circle: L.circle([50.5, 30.5], { radius: 200 }),
      bounds: null,
      plusMarkerIcon: new L.Icon({
        iconUrl: Pngbarn,
        iconSize: [25, 41]
      })
    }
  },
  computed: {
    PatLength () {
      return this.$store.state.Basic.PatListnow.length
    },
    PatListnow () {
      return this.$store.state.Basic.PatListnow
    },
    centerIcon () {
      return new L.Icon({
        iconUrl: markerred,
        shadowUrl: markershadow,
        iconSize: [25, 41],
        iconAnchor: [12, 41],
        popupAnchor: [1, -34],
        shadowSize: [41, 41]
      })
    },
    chunk () {
      if (this.List.length > 0) {
        return this.lodash.chunk(this.List, 1)[0]
      }
      return this.List
    },
    title () {
      return document.title
    },
    polyline () {
      if (this.model.location && this.model.location.length > 0) {
        return [this.location, this.model.location]
      }
      return []
    },
    markerList () {
      let temp = Object.assign(this.hospList, {})
      temp = temp.filter(function (x) {
        var hasText = false
        if (x['hosp_erbed'] !== '0') {
          hasText = true
        }

        return hasText
      })
      return temp
    },
    sideMarker () {
      return L.latLng(this.testL, this.testI)
    }
  },
  watch: {
    PatListnow () {
      if (this.PatListnow.length > 0) {
        this.List = this.PatListnow
      }
    }
  },
  mounted () {
    this.List = this.PatListnow
    navigator.geolocation.getCurrentPosition(this.showPosition)
    // this.$refs.Map =
  },
  methods: {
    onSearch () {
      let vuethis = this
      let pList = JSON.parse(JSON.stringify(vuethis.$store.state.Basic.PatListnow))
      let int = vuethis._.findIndex(pList, function (o) {
        return o.PATIENT_ID.indexOf(vuethis.searchText) >= 0 ||
          o.PATIENT_NAME.indexOf(vuethis.searchText) >= 0
      })
      if (int >= 0) {
        this.List = [pList[int]]
      }
    },
    TextBlur (e, id) {
      this.$set(this, id, e.target.value)
    },
    RowClassName (record, index) {
      var vuethis = this
      if (record.PATIENT_ID === vuethis.data.PATIENT_ID) {
        return 'bg-yellow-200'
      }
      return ''
    },
    clickCustomRow (record, index) {
      // 點選row
      var vuethis = this
      return {
        on: {
          click: function () {
            vuethis.data = Object.assign(record, {})
          }
        }
      }
    },
    getDistance (hospLocation) {
      let dis = ''
      if (hospLocation.length > 0) {
        dis = (this.location.distanceTo(L.latLng(hospLocation[0], hospLocation[1]))).toFixed(0) / 1000
      }
      return '距離 ' + dis + ' km'
    },
    get_hosp_color (pVal) {
      let color = ''
      switch (pVal) {
        case '1':
          color = markerblue
          break
        case '2':
        case '3':
        case '4':
        case '5':
          color = markergold
          break
        case '6':
        case '7':
        case '8':
        case '9':
          color = markergreen
          break
        case '10':
          color = markerviolet
          break
        default:
          color = markergreen
          break
      }
      return new L.Icon({
        iconUrl: color,
        shadowUrl: markershadow,
        iconSize: [25, 41],
        iconAnchor: [12, 41],
        popupAnchor: [1, -34],
        shadowSize: [41, 41]
      })
    },
    open (pModel) {
      this.model = Object.assign(this.model, pModel)
      this.visible = !this.visible
    },
    onClosePatList () {
      // this.visiblePatList = false
    },
    onClose () {
      this.visible = false
    },
    zoomUpdated (zoom) {
      this.zoom = zoom
    },
    centerUpdated (center) {
      this.center = center
      this.circleMarker = center
    },
    boundsUpdated (bounds) {
      this.bounds = bounds
    },
    showPosition (position) {
      this.center = L.latLng(position.coords.latitude, position.coords.longitude)
      this.location = L.latLng(position.coords.latitude, position.coords.longitude)
      this.circleMarker = L.latLng(position.coords.latitude, position.coords.longitude)
      this.testL = position.coords.latitude
      this.testI = position.coords.longitude
      this.loading = true
    },
    GPS () {
      navigator.geolocation.getCurrentPosition(this.getGPSlatitude)
    },
    getGPSlatitude (position) {
      return L.latLng(position.coords.latitude, position.coords.longitude)
    },
    clickAlert (e) {
      console.log(e)
    },
    selectPatData () {
      let vuethis = this
      if (this.chunk.length > 0 && this.model.hosp_desc) {
        let patData = this.chunk[0]
        patData.HOSP_KEY = this.model.hosp_desc
        patData.SCORE = 99
        this.$api.MC.UPDATE_PAT_DATA(patData).then((result) => {
          this.info(result.data)
          vuethis.visiblePatList = false
          vuethis.$api.MC.GetPatList().then((result) => {
            vuethis.$store.commit({
              type: 'Basic/SetPatListnow',
              data: result.data
            })
            vuethis.searchText = ''
            vuethis.onSearch()
            setTimeout(() => {
              vuethis.visiblePatList = true
              vuethis.spinning = false
            }, 500)
          }).catch((err) => {
            console.log(err)
            this.error(err)
            setTimeout(() => {
              vuethis.spinning = false
            }, 500)
          })
        }).catch((err) => {
          console.log(err)
          this.error(err)
        })
      }
    }
  }
}
</script>

<style scoped>
</style>
