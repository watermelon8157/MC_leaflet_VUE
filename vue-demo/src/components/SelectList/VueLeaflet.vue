<template>
  <div v-if="loading">
    <a-drawer placement="bottom" :closable="false" @close="onClose" :visible="visible">
      <div slot="title">
        {{model.hosp_name}}
        <span class="m-2">({{getDistance(model.location)}})</span>
        <a-button-group class="float-right">
          <a-button type="primary" @click="selectPatData">送出</a-button>
        </a-button-group>
      </div>
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
    </a-drawer>
    <a-card>
      <span slot="title">
        選擇醫院
        <a-tag class="font-extrabold bg-red-500 text-white">Severe</a-tag>
        <a-tag class="font-extrabold bg-orange-500 text-white">Medium</a-tag>
        <a-tag class="font-extrabold bg-yellow-500 text-white">Ceneral</a-tag>
        <a-tag class="font-extrabold bg-green-500 text-white">Non-ECRH</a-tag>
        <span v-if="model.PATIENT_NAME" class="m-2">
          (
          病患
          <span class="m-1 p-2 bg-yellow-200">{{model.PATIENT_NAME}}</span>)
        </span>
      </span>

      <div>
        <div class="vue-leaflet">
          <l-map
            ref="Map"
            style="width: 100%; height: 600px;"
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
    </a-card>

    <!-- <a-input placeholder="Basic usage" v-model="testL" />
    <a-input placeholder="Basic usage" v-model="testI" />
    {{center}}
    <br />
    {{zoom}}
    <br />
    {{bounds}}
    <br />-->
  </div>
</template>

<script>
import { LMap, LTileLayer, LMarker, LPopup, LCircleMarker, LPolyline } from 'vue2-leaflet'
import Pngbarn from '@/assets/pngbarn.png'
import L from 'leaflet'
import Mixin from '@/mixin'
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
      visible: false,
      loading: true,
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
