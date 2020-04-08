<template>
  <div v-if="loading">
    <a-drawer
      title="病患清單"
      placement="bottom"
      :closable="false"
      :mask="false"
      @close="onClosePatList"
      :visible="visiblePatList"
    >
      <a-table
        :columns="columns"
        :rowClassName="RowClassName"
        :rowKey="record => record.char_no"
        :dataSource="data"
        :loading="loadingPatList"
        size="small"
        :customRow="clickCustomRow"
      >
        <template slot="char_no" slot-scope="text,record">{{text}}</template>
        <template slot="pat_name" slot-scope="text,record">{{text}}</template>
        <template slot="injury_classification" slot-scope="text,record">
          <div
            :class="{'text-red-500' : text === 'Severe' ,
              'text-blue-500' : text === 'Moderate' ,
              'text-green-500' : text === 'Mild'
               }"
          >{{text}}</div>
        </template>
      </a-table>
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
        <l-marker ref="markerLocation" :lat-lng="location">
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
import L from 'leaflet'
import Mixin from '@/mixin'
const columns = [
  {
    title: '病歷編號',
    dataIndex: 'char_no',
    scopedSlots: { customRender: 'char_no' }
  },
  {
    title: '姓名',
    dataIndex: 'pat_name',
    scopedSlots: { customRender: 'pat_name' }
  },
  {
    title: '傷勢',
    dataIndex: 'injury_classification',
    scopedSlots: { customRender: 'injury_classification' }
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
    data () {
      var vuethis = this
      let temp = Object.assign(vuethis.patList, {})
      temp = temp.sort((a, b) => (a.injury_classification > b.injury_classification) ? 1 : -1)
      temp = temp.reverse()
      return temp
    },
    title () {
      return document.title
    },
    polyline () {
      if (this.model.location.length > 0) {
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
  mounted () {
    navigator.geolocation.getCurrentPosition(this.showPosition)
    // this.$refs.Map =
  },
  methods: {
    RowClassName (record, index) {
      var vuethis = this
      if (record.char_no === vuethis.model.char_no) {
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
            vuethis.model = Object.assign(record, {})
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
          color = 'marker-icon-2x-green.png'
          break
        case '2':
        case '3':
        case '4':
        case '5':
          color = 'marker-icon-2x-gold.png'
          break
        case '6':
        case '7':
        case '8':
        case '9':
          color = 'marker-icon-2x-orange.png'
          break
        case '10':
          color = 'marker-icon-2x-red.png'
          break
        default:
          color = 'marker-icon-2x-green.png'
          break
      }
      return new L.Icon({
        iconUrl: 'https://cdn.rawgit.com/pointhi/leaflet-color-markers/master/img/' + color,
        shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
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
      this.$emit('emitSelectPatData')
    }
  }
}
</script>

<style scoped>
</style>
