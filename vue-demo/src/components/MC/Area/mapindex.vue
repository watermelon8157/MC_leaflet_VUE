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
            <l-marker
              :lat-lng="sideMarker"
              :icon="get_hosp_color('')"
            ></l-marker>
            <l-marker
              v-for="(i, index) in markerList"
              :key="index"
              :lat-lng="i.location"
              @click="(e) => open(i)"
              :icon="get_hosp_color(i.hosp_ranking)"
            >
              <l-popup :content="i.hosp_name"></l-popup>
            </l-marker>
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
          <span class="m-2">
              <span class=" text-red-500 font-bold" v-if=" $store.state.Basic.PatModel.TRIAGE === 'Severe'">重傷</span>
              <span class=" text-yellow-500 font-bold" v-if=" $store.state.Basic.PatModel.TRIAGE === 'Moderate'">中傷</span>
              <span class="  text-blue-500 font-bold" v-if=" $store.state.Basic.PatModel.TRIAGE === 'Mild'">輕傷</span>
          </span>
        </span>
        <a-tag class="font-extrabold bg-yellow-500 text-white">第三推薦</a-tag>
        <a-tag class="font-extrabold bg-blue-500 text-white">第二推薦</a-tag>
        <a-tag class="font-extrabold bg-green-500 text-white">最推薦</a-tag>
        <a-tag class="font-extrabold bg-red-500 text-white">目前位置</a-tag>
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
        <span class="m-2">(距離 {{ getDistance(model.location) }}km)</span>
        <a-button-group class="float-right">
          <a-button type="primary" @click="selectPatData">送出</a-button>
        </a-button-group>
      </div>
      <div>
        <a-form layout="inline">
          <a-form-item label="醫院分數">
            <a-tag color="red" class="text-red-500 font-extrabold">{{
              model.hosp_source ? model.hosp_source: Math.floor(Math.random() * 100)
            }}</a-tag>
            <a-divider class="bg-orange-500" type="vertical" />
          </a-form-item>
          <a-form-item label="選擇醫院人數">
            <a-tag class="font-extrabold">{{ model.hosp_ihp }}</a-tag>
            <a-divider class="bg-orange-500" type="vertical" />
          </a-form-item>
          <a-form-item label="尚未抵達醫院人數">
            <a-tag class="font-extrabold">{{ model.hosp_INp }}</a-tag>
            <a-divider class="bg-orange-500" type="vertical" />
          </a-form-item>
          <a-form-item label="抵達醫院人數">
            <a-tag class="font-extrabold">{{ model.hosp_ATp }}</a-tag>
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
          <a-form-item label="急救責任等級">
            <a-tag class="font-extrabold">
                <span v-if="model.hosp_ranking==='0'">非急救責任醫院</span>
                 <span v-if="model.hosp_ranking==='1'">一般</span>
                  <span v-if="model.hosp_ranking==='2'">中度</span>
                   <span v-if="model.hosp_ranking==='3'">重度</span>
               </a-tag>
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
import blue from '@/assets/marker/marker-icon-2x-blue.png'
import red from '@/assets/marker/marker-icon-2x-red.png'
import gold from '@/assets/marker/marker-icon-2x-gold.png'
import green from '@/assets/marker/marker-icon-2x-green.png'
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
        hosp_INp: 0, // 尚未抵達
        hosp_ATp: 0, // 已經抵達
        hosp_whp: 0,
        location: []
      },
      visible: false,
      visibleMemu: false,
      loading: false,
      zoom: 12,
      center: L.latLng(this.testL, this.testI),
      url: 'http://{s}.tile.osm.org/{z}/{x}/{y}.png',
      attribution: '&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors',
      location: L.latLng(this.testL, this.testI),
      circleMarker: L.latLng(this.testL, this.testI),
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
    testL () {
      let vuethis = this
      if (vuethis.site_id) {
        let item = vuethis.siteList.filter(
          x => x.SITE_ID === vuethis.site_id
        )
        if (item && item.length > 0) {
          return item[0].LATITUDE
        }
      }
      return 23.6
    },
    testI () {
      let vuethis = this
      if (vuethis.site_id) {
        let item = vuethis.siteList.filter(
          x => x.SITE_ID === vuethis.site_id
        )
        if (item && item.length > 0) {
          return item[0].LONGITUDE
        }
      }
      return 121
    },
    polyline () {
      if (this.model.location.length > 0) {
        return [this.location, this.model.location]
      }
      return []
    },
    markerList () {
      let vuethis = this
      let temp = Object.assign(this.hospList, {})
      temp = temp.filter(function (x) {
        var hasText = false
        if (x['hosp_erbed'] !== '0') {
          if (vuethis.SITE_AREA === x['hosp_class'].trim()) {
            hasText = true
          }
        }

        return hasText
      })
      return temp
    },
    sideMarker () {
      let vuethis = this
      if (vuethis.site_id) {
        let item = vuethis.siteList.filter(
          x => x.SITE_ID === vuethis.site_id
        )
        if (item && item.length > 0) {
          return L.latLng(item[0].LATITUDE, item[0].LONGITUDE)
        }
      }
      return L.latLng(this.testL, this.testI)
    }
  },
  mounted () {
    let vuethis = this
    this.$nextTick(function () {
      vuethis.loading = false
      vuethis.$store.commit('SpinLoading', true)

      vuethis.$api.MC.getMC_SITE_INFO()
        .then(result => {
          vuethis.siteList = result.data
          navigator.geolocation.getCurrentPosition(vuethis.showPosition)
          // this.$refs.Map =
          setTimeout(() => {
            vuethis.loading = true
            vuethis.$store.commit('SpinLoading', false)
          }, 3000)
        })
        .catch(err => {
          vuethis.$notification.error({
            message: err.data
          })
        })
    })
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
      return dis
    },
    get_hosp_color (pVal) {
      let color = ''
      switch (pVal) {
        case '1':
        case '2':
          color = gold
          break
        case '3':
        case '4':
        case '5':
        case '6':
        case '7':
        case '8':
        case '9':
          color = blue
          break
        case '10':
          color = green
          break
        default:
          color = red
          break
      }
      return new L.Icon({
        iconUrl: color,
        shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
        iconSize: [25, 41],
        iconAnchor: [12, 41],
        popupAnchor: [1, -34],
        shadowSize: [41, 41]
      })
    },
    open (pModel) {
      let vuethis = this
      vuethis.model = Object.assign(this.model, pModel)
      console.log(vuethis.model)
      vuethis.model.hosp_ihp = vuethis.$store.state.Basic.PatListByID.filter((x) => x.HOSP_KEY === vuethis.model.HOSP_KEY).length
      vuethis.model.hosp_INp = vuethis.$store.state.Basic.PatListByID.filter((x) => x.HOSP_KEY === vuethis.model.HOSP_KEY && x.EXPECTED_ARRIVAL_DATETIME &&
      (Date.parse(x.EXPECTED_ARRIVAL_DATETIME)).valueOf() > Date.now().valueOf()).length
      vuethis.model.hosp_ATp = vuethis.$store.state.Basic.PatListByID.filter((x) => x.HOSP_KEY === vuethis.model.HOSP_KEY && x.EXPECTED_ARRIVAL_DATETIME &&
      (Date.parse(x.EXPECTED_ARRIVAL_DATETIME)).valueOf() <= Date.now().valueOf()).length
      vuethis.visible = !vuethis.visible
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
      let vuethis = this
      vuethis.center = L.latLng(this.testL, this.testI)
      vuethis.location = L.latLng(this.testL, this.testI)
      vuethis.circleMarker = L.latLng(this.testL, this.testI)
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
      let minutes = 60 / 15 * vuethis.getDistance(vuethis.model.location)
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
