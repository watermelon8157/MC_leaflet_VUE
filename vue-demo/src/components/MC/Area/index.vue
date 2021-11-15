<style >
body {
  font-family: "微軟正黑體";
  overflow-x: hidden;
  font-size: 1.875rem;
}
.anticon {
  vertical-align: 0.125em;
}
.table th {
  background-color: #5ba198;
  color: #fff;
  border-top: 1px solid #ddd;
  border-left: 1px;
}
/*Table-striped Color*/
.table .table-striped > tbody > tr:nth-of-type(odd) {
  background-color: #ecffee;
}
::-webkit-scrollbar {
  width: 8px;
  height: 10px;
}
::-webkit-scrollbar-thumb {
  border-radius: 8px;
  background: #c2c9d2;
}
</style>
<template>
  <a-layout id="app">
    <div class="fixed top-0 right-0 mt-2 mx-2 pb-4 z-50">
      <button
        type="button"
        @click="logOut"
        class="btn btn-secondary btn-lg btn-block"
      >
        <a-icon type="logout" />
        <span>登出</span>
      </button>
    </div>
    <a-layout id="components-layout-demo-top" class="layout">
      <a-layout-header class="bg-blue-400">
        <div class="logo text-white" />
        {{ titleName }}
        <span v-if="site_id"> - {{ site_title }}</span>
      </a-layout-header>
      <a-layout-content class="container">
        <div class="m-2">
          <transition name="fade" mode="out-in">
            <router-view :key="$route.path"></router-view>
          </transition>
        </div>
      </a-layout-content>
      <a-layout-footer style="text-align: center"
        >Ant Design 2020 Created by MC</a-layout-footer
      >
    </a-layout>
    <a-back-top :visibilityHeight="100" />
    <a-button
      class="fixed z-50 mt-3 mx-2 pb-4 left-0 top-0"
      shape="circle"
      @click="(e) => showDrawer()"
    >
      <a-icon type="menu-unfold" class="m-2" />
    </a-button>
    <a-drawer
      placement="left"
      :closable="false"
      @close="onClose"
      :visible="visible"
    >
      {{ site_title }}
      <a-menu
        style="width: 256px"
        v-model="routeName"
        mode="vertical"
        @click="handleClick"
      >
        <a-menu-item key="AreaForm">建立傷病患資料</a-menu-item>
        <a-menu-item key="AreaSelectList">選擇後送醫院</a-menu-item>
        <a-menu-item key="AreaHospAdmission">後送醫院狀況</a-menu-item>
        <a-menu-item key="AreaHospEvacuation">到達醫院狀況</a-menu-item>
        <a-menu-item key="AreaPatList">傷患查詢</a-menu-item>
      </a-menu>
    </a-drawer>
    <a-modal
      title="選擇事件"
      :visible="visibleLogin"
      :closable="false"
      :footer="null"
    >
      <div>
        <a-form layout="inline">
          <a-tabs default-active-key="1">
            <a-tab-pane key="1" tab="事件清單">
              <div>
                <a-form-item
                  required
                  :validate-status="!!siteid ? '' : 'error'"
                  :help="!!siteid ? ' ' : '請輸入事件代碼'"
                >
                  <a-select v-model="siteid" style="width: 280px">
                    <a-select-option value="">請選擇</a-select-option>
                    <a-select-option
                      v-for="(i, index) in siteList"
                      :key="index"
                      :value="i.SITE_ID"
                      >{{ i.SITE_DESC }}({{ i.SITE_AREA }})
                    </a-select-option>
                  </a-select>
                </a-form-item>
              </div>
              <div>
                <a-form-item>
                  <a-button
                    :disabled="disabled"
                    type="primary"
                    @click="SiteLogin"
                    >選擇</a-button
                  >
                </a-form-item>
              </div>
            </a-tab-pane>
          </a-tabs>
        </a-form>
      </div>
      <a-alert
        v-if="visibleLoginAlert"
        type="error"
        message="登入驗證失敗，請重新登入!"
        banner
        closable
      />
    </a-modal>
    <Spin class="z-50"></Spin>
  </a-layout>
</template>

<script>
import Spin from '@/components/Shared/Spin'
import Mixin from '@/mixin'
export default {
  mixins: [Mixin],
  components: {
    Spin
  },
  name: 'App',
  data () {
    return {
      routeName: [this.$route.name],
      siteList: [],
      loginDrawer: false,
      btnTxt: '登入',
      siteid: '',
      site_desc: '',
      User_pwd: '',
      latitude: '',
      longitude: '',
      disabled: false,
      visibleLoginAlert: false,
      visibleLogin: false,
      titleName: document.title,
      visible: false
    }
  },
  created () {
    this.$nextTick(function () {
      var vuethis = this
      vuethis.spinning = true
      if (vuethis.$auth.TokenExist()) {
        vuethis.$api.MC.JwtAuthCheck().then((result) => {
          if (vuethis.$store.state.Basic.loadFirst) {
            vuethis.$store.commit({
              type: 'Basic/SetLoadFirst'
            })
          }
          vuethis.$api.MC.GetPatList().then((result) => {
            vuethis.$store.commit({
              type: 'Basic/SetPatListnow',
              data: result.data
            })
            setTimeout(() => {
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
          vuethis.visibleLoginAlert = true
          vuethis.ShowLoginAlert()
          setTimeout(() => {
            vuethis.spinning = false
          }, 500)
        })
      } else {
        vuethis.visibleLoginAlert = true
        vuethis.ShowLoginAlert()
      }
    })
  },
  mounted () {
    this.GPS()
    this.$api.MC.getMC_SITE_INFO()
      .then(result => {
        this.disabled = false
        this.siteList = result.data
      })
      .catch(err => {
        this.$notification.error({
          message: err.data
        })
        this.disabled = false
      })
  },
  methods: {
    GPS () {
      navigator.geolocation.getCurrentPosition(this.getGPSlatitude)
    },
    getGPSlatitude (position) {
      this.latitude = position.coords.latitude
      this.longitude = position.coords.longitude
    },
    SiteLogin () {
      this.$api.MC.SiteLogin({ site_id: this.siteid, LATITUDE: this.latitude, LONGITUDE: this.longitude })
        .then(result => {
          this.$auth.setToken(result.data.token)
          this.disabled = false
          this.Loginform()
        })
        .catch(err => {
          this.$notification.error({
            message: err.data
          })
          this.disabled = false
        })
    },
    Loginform () {
      location.reload()
    },
    ShowLoginAlert () {
      var vuethis = this
      vuethis.visibleLogin = true
      vuethis.$auth.clearToken()
      setTimeout(() => {
        vuethis.visibleLoginAlert = false
      }, 3000)
      setTimeout(() => {
        vuethis.$store.commit('SpinLoading', false)
      }, 500)
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
    showDrawer () {
      this.visible = true
    },
    onClose () {
      this.visible = false
    },
    Login () {
      this.$api.MC.LoginForm({ site_id: this.siteid, LATITUDE: this.latitude, LONGITUDE: this.longitude })
        .then(result => {
          this.$auth.setToken(result.data.token)
          this.disabled = false
          this.Loginform()
        })
        .catch(err => {
          this.$notification.error({
            message: err.data
          })
          this.disabled = false
        })
    }
  }
}
</script>
<style >
.anticon {
  vertical-align: 0.125em;
}
</style>
