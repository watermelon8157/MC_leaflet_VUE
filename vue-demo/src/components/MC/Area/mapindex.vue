<template>
  <div>
    <div v-if="loading">
      <div>
        <a-drawer
          placement="left"
          :closable="false"
          @close="onCloseMemu"
          :visible="visibleMemu"
        >
          <a-menu
            style="width: 256px"
            v-model="routeName"
            mode="vertical"
            @click="handleClick"
          >
            <a-menu-item key="AreaForm">建立資料</a-menu-item>
            <a-menu-item key="AreaSelectList">後送</a-menu-item>
            <a-menu-item key="AreaHospAdmission">後送醫院狀況</a-menu-item>
            <a-menu-item key="AreaHospEvacuation">到達醫院狀況</a-menu-item>
            <a-menu-item key="AreaPatList">傷患查詢</a-menu-item>
          </a-menu>
        </a-drawer>
        <div class="vue-leaflet z-30">
          <l-map
            ref="Map"
            :style="{
              width: ' 100%',
              height: windowHeight + 'px',
            }"
            :options="{ zoomControl: false }"
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
              v-for="(i, index) in markerList"
              :key="index"
              :lat-lng="i.location"
              @click="(e) => open(i)"
              :icon="get_hosp_color(i.hosp_ranking)"
            >
              <l-popup :content="i.hosp_name"></l-popup>
            </l-marker>
            <l-circle-marker
              :lat-lng="circleMarker"
              :radius="zoom * 10"
              color="#bee3f8"
            />
            <l-control-zoom position="bottomright"></l-control-zoom>
          </l-map>
        </div>
      </div>
    </div>
    <Spin class="z-50"></Spin>
    <a-drawer
      ref=""
      placement="top"
      height="50"
      :mask="false"
      :maskClosable="false"
      :closable="false"
      :visible="visibleFun"
    >
      <div slot="title">
        <span class="m-2">
          病患:
          <span class="m-2">{{
            $store.state.Basic.PatModel.PATIENT_NAME
          }}</span>
        </span>
        <span class="m-2">
          傷勢:
          <span class="m-2">{{ $store.state.Basic.PatModel.TRIAGE }}</span>
        </span>
        <a-tag class="font-extrabold bg-red-500 text-white">很多人</a-tag>
        <a-tag class="font-extrabold bg-orange-500 text-white">普通</a-tag>
        <a-tag class="font-extrabold bg-yellow-500 text-white">有點多</a-tag>
        <a-tag class="font-extrabold bg-green-500 text-white">推薦</a-tag>
        <a-tag class="font-extrabold bg-blue-500 text-white">現在位置</a-tag>
      </div>
      <a-button
        class="fixed z-50 mt-3 mx-2 pb-4 right-0 top-0"
        shape="circle"
        @click="(e) => showDrawer()"
      >
        <a-icon type="menu-unfold" class="m-2" />
      </a-button>
    </a-drawer>
    <a-drawer
      placement="bottom"
      :closable="false"
      @close="onClose"
      :visible="visible"
    >
      <div slot="title">
        {{ model.hosp_name }}
        <span class="m-2">({{ getDistance(model.location) }})</span>
        <a-button-group class="float-right">
          <a-button type="primary" @click="selectPatData">送出</a-button>
        </a-button-group>
      </div>
      <div>
        <a-form layout="inline">
          <a-form-item label="醫院分數">
            <a-tag color="red" class="text-red-500 font-extrabold">{{
              model.hosp_source
            }}</a-tag>
            <a-divider class="bg-orange-500" type="vertical" />
          </a-form-item>
          <a-form-item label="後送醫院人數">
            <a-tag class="font-extrabold">{{ model.hosp_ihp }}</a-tag>
            <a-divider class="bg-orange-500" type="vertical" />
          </a-form-item>
          <a-form-item label="到達醫院人數">
            <a-tag class="font-extrabold">{{ model.hosp_whp }}</a-tag>
            <a-divider class="bg-orange-500" type="vertical" />
          </a-form-item>
          <a-form-item label="衛福部業務組別">
            <a-tag class="font-extrabold">{{ model.hosp_class }}</a-tag>
            <a-divider class="bg-orange-500" type="vertical" />
          </a-form-item>
          <a-form-item label="縣市">
            <a-tag class="font-extrabold">{{ model.hosp_city }}</a-tag>
            <a-divider class="bg-orange-500" type="vertical" />
          </a-form-item>
          <a-form-item label="緊急醫療能力">
            <a-tag class="font-extrabold">{{ model.hosp_injury }}</a-tag>
            <a-divider class="bg-orange-500" type="vertical" />
          </a-form-item>
          <a-form-item label="急救責任等級">
            <a-tag class="font-extrabold">{{ model.hosp_ranking }}</a-tag>
            <a-divider class="bg-orange-500" type="vertical" />
          </a-form-item>
          <a-form-item label="急診觀察床">
            <a-tag class="font-extrabold">{{ model.hosp_erbed }}</a-tag>
            <a-divider class="bg-orange-500" type="vertical" />
          </a-form-item>
          <a-form-item label="救護車">
            <a-input v-model="model.amb_id" />
            <a-divider class="bg-orange-500" type="vertical" />
          </a-form-item>
        </a-form>
      </div>
    </a-drawer>
  </div>
</template>

<script>
import { LMap, LTileLayer, LMarker, LPopup, LCircleMarker, LPolyline, LControlZoom } from 'vue2-leaflet'
import Pngbarn from '@/assets/pngbarn.png'
import L from 'leaflet'
import Mixin from '@/mixin'
import Spin from '@/components/Shared/Spin'
import moment from 'moment'
export default {
  mixins: [Mixin],
  name: 'VueLeaflet',
  components: {
    Spin,
    LMap,
    LTileLayer,
    LMarker,
    LPopup,
    LCircleMarker,
    LPolyline,
    LControlZoom
  },
  data () {
    return {
      routeName: [this.$route.name],
      visibleFun: true,
      baseUrl: 'http://mesonet.agron.iastate.edu/cgi-bin/wms/nexrad/n0r.cgi',
      layers: [
        {
          name: 'Weather Data',
          visible: true,
          format: 'image/png',
          layers: 'nexrad-n0r-900913',
          transparent: true,
          attribution: 'Weather data © 2012 IEM Nexrad'
        }
      ],
      model: {
        hosp_desc: '',
        hosp_name: '',
        hosp_class: '',
        hosp_city: '',
        hosp_injury: '',
        hosp_ranking: '',
        hosp_erbed: '',
        hosp_distance: '0',
        amb_id: '',
        hosp_source: 0,
        hosp_ihp: 0,
        hosp_whp: 0,
        location: []
      },
      testL: 23.6,
      testI: 121,
      visible: false,
      visibleMemu: false,
      loading: false,
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
    let vuethis = this
    vuethis.loading = false
    vuethis.$store.commit('SpinLoading', true)
    navigator.geolocation.getCurrentPosition(this.showPosition)
    // this.$refs.Map =
    setTimeout(() => {
      vuethis.loading = true
      vuethis.$store.commit('SpinLoading', false)
    }, 3000)
  },
  methods: {
    showDrawer () {
      this.visibleMemu = true
    },
    onCloseMemu () {
      this.visibleMemu = false
    },
    handleClick (e) {
      var vuethis = this
      // vuethis.$store.commit('SpinLoading', true)
      vuethis.visible = false
      vuethis.$router.push({ name: e.key, params: { now: this.now } })
      setTimeout(() => {
        vuethis.titleName = document.title
        vuethis.$store.commit('SpinLoading', false)
      }, 500)
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
      let vuethis = this
      let pModel = vuethis.$store.state.Basic.PatModel
      pModel.HOSP_KEY = vuethis.model.hosp_desc
      pModel.AMB_ID = vuethis.model.amb_id
      pModel.HOSPITAL_SHOW_NAME = vuethis.model.hosp_name
      pModel.SCORE = vuethis.model.hosp_source
      pModel.SELECTION_DATETIME = moment().format('YYYY-MM-DD HH:mm:ss')
      let minutes = vuethis.getDistance(vuethis.model.location) / 15 * 60
      console.log(parseInt(minutes))
      pModel.EXPECTED_ARRIVAL_DATETIME = moment().add(parseInt(minutes), 'minutes').format('YYYY-MM-DD HH:mm:ss')
      this.$api.MC.UPDATE_PAT_DATA(pModel).then((result) => {
        vuethis.info(result.data)
        vuethis.$api.MC.GetPatListByID({ site_id: vuethis.site_id, hosp_id: vuethis.hosp_id }).then((result) => {
          vuethis.$store.commit({
            type: 'Basic/SetPatListBYID',
            data: result.data
          })
          vuethis.$router.push({ name: 'AreaSelectList', params: { now: vuethis.now } })
        }).catch((err) => {
          console.log(err)
          this.error(err)
        })
      }).catch((err) => {
        console.log(err)
        vuethis.error(err)
      })
    }
  }
}
</script>

<style scoped>
</style>
