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
    <router-view id="app" :key="$route.path"></router-view>
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
          vuethis.$api.MC.GetPatListAll().then((result) => {
            vuethis.$store.commit({
              type: 'Basic/SetPatListAll',
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
          vuethis.$api.MC.GetPatListByID({ site_id: vuethis.site_id, hosp_id: vuethis.hosp_id }).then((result) => {
            vuethis.$store.commit({
              type: 'Basic/SetPatListBYID',
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

          vuethis.$api.MC.GetHospList().then((result) => {
            vuethis.$store.commit({
              type: 'Basic/SetGetHospList',
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

          vuethis.$api.MC.GetHospListDTLByID({ site_id: vuethis.site_id }).then((result) => {
            vuethis.$store.commit({
              type: 'Basic/SetGetHospListDTL',
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
