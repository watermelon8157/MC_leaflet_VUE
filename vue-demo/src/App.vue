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
  <a-layout id="app" class="h-screen">
    <a-layout id="components-layout-demo-top" class="layout">
      <a-layout-header class="bg-blue-400">
        <div class="logo text-white" />
        {{titleName}}(事件代碼:{{site_id}})
      </a-layout-header>
      <a-layout-content class="container">
        <div class="m-2">
          <transition name="fade" mode="out-in">
            <router-view :key="$route.path"></router-view>
          </transition>
        </div>
      </a-layout-content>
      <a-layout-footer style="text-align: center">Ant Design 2020 Created by MC</a-layout-footer>
    </a-layout>
    <a-back-top :visibilityHeight="100" />
    <a-button
      class="fixed z-50 mt-3 mx-2 pb-4 left-0 top-0"
      shape="circle"
      @click="(e)=>showDrawer()"
    >
      <a-icon type="menu-unfold" class="m-2" />
    </a-button>
    <a-drawer placement="left" :closable="false" @close="onClose" :visible="visible">
      <a-menu style="width: 256px" mode="vertical" @click="handleClick">
        <a-menu-item key="Form">建立資料</a-menu-item>
        <a-menu-item key="SelectList">待送清單</a-menu-item>
        <a-menu-item key="HospAdmission">醫院收治狀況</a-menu-item>
        <a-menu-item key="HospEvacuation">醫院後送狀況</a-menu-item>
        <a-menu-item key="PatList">病患清單</a-menu-item>
      </a-menu>
    </a-drawer>
    <a-modal title="登入" :visible="visibleLogin" :closable="false" :footer="null">
      <loginform v-on:emitLogin="Loginform" :loginDrawer="true"></loginform>
      <a-alert v-if="visibleLoginAlert" type="error" message="登入驗證失敗，請重新登入!" banner closable />
    </a-modal>
    <Spin class="z-50"></Spin>
  </a-layout>
</template>

<script>
import Loginform from '@/components/Shared/Loginform'
import Spin from '@/components/Shared/Spin'
import Mixin from '@/mixin'
export default {
  mixins: [Mixin],
  components: {
    Spin, Loginform
  },
  name: 'App',
  data () {
    return {
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
  },
  methods: {
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
    }
  }
}
</script>
<style >
.anticon {
  vertical-align: 0.125em;
}
</style>
