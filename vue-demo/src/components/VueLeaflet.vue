<template>
    <div v-if="loading">
        {{center}}<br>
        {{zoom}}<br>
        {{bounds}}<br>
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
                <l-marker :lat-lng="circleMarker" :icon="plusMarkerIcon"> </l-marker>
                <l-circle-marker :lat-lng="circleMarker" :radius="zoom*10" color="#bee3f8" />
                <l-marker :lat-lng="location" >
                    <l-popup :content="text"></l-popup>
                </l-marker>
                <l-marker v-for="(i,index) in markerList" :key="index" :lat-lng="i.location" @click="clickAlert" :icon="i.icon">
                    <l-popup :content="i.hosp"></l-popup>
                </l-marker>
            </l-map>
        </div>
    </div>
</template>

<script>
import { LMap, LTileLayer, LMarker, LPopup, LCircleMarker } from 'vue2-leaflet'
import Pngbarn from '@/assets/pngbarn.png'
import L from 'leaflet'
export default {
  name: 'VueLeaflet',
  components: {
    LMap,
    LTileLayer,
    LMarker,
    LPopup,
    LCircleMarker
  },
  data () {
    return {
      loading: true,
      zoom: 18,
      center: L.latLng(23.6, 121),
      url: 'http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',
      attribution: '&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors',
      location: L.latLng(23.6, 121),
      circleMarker: L.latLng(23.6, 121),
      text: '現在位置',
      Circle: L.circle([50.5, 30.5], { radius: 200 }),
      bounds: null,
      plusMarkerIcon: new L.Icon({
        iconUrl: Pngbarn,
        iconSize: [25, 41]
      }),
      markerList: [{'location': [24.991099199999997, 121.4935257],
        'hosp': '雙和醫院',
        'icon':
       new L.Icon({
         iconUrl: 'https://cdn.rawgit.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-green.png',
         shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
         iconSize: [25, 41],
         iconAnchor: [12, 41],
         popupAnchor: [1, -34],
         shadowSize: [41, 41]
       })},
      {'location': [25.000057599999998, 121.55823059999999],
        'hosp': '萬芳醫院',
        'icon':
       new L.Icon({
         iconUrl: 'https://cdn.rawgit.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-orange.png',
         shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
         iconSize: [25, 41],
         iconAnchor: [12, 41],
         popupAnchor: [1, -34],
         shadowSize: [41, 41]
       })},
      {'location': [25.0252575, 121.56150799999998],
        'hosp': '臺北醫學大學附設醫院',
        'icon':
       new L.Icon({
         iconUrl: 'https://cdn.rawgit.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-red.png',
         shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
         iconSize: [25, 41],
         iconAnchor: [12, 41],
         popupAnchor: [1, -34],
         shadowSize: [41, 41]
       })}]
    }
  },
  mounted () {
    navigator.geolocation.getCurrentPosition(this.showPosition)
    // this.$refs.Map =
  },
  methods: {
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
    },
    GPS () {
      navigator.geolocation.getCurrentPosition(this.getGPSlatitude)
    },
    getGPSlatitude (position) {
      return L.latLng(position.coords.latitude, position.coords.longitude)
    },
    clickAlert (e) {
      console.log(e)
    }
  }
}
</script>

<style scoped>
</style>
