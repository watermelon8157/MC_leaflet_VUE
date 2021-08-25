import Alert from '@/components/Shared/Alert'
import DB_MC_PATIENT_INFO from '@/RCS_Data.Models.DB.DB_MC_PATIENT_INFO.json'

export default {
  components: {
    Alert
  },
  data () {
    return {
      siteList: [],
      hospList2: this.$store.state.Basic.hospList,
      // hospList3: this.state.hospList,
      data: Object.assign(DB_MC_PATIENT_INFO, {}),
      windowHeight: 0,
      windowWidth: 0,
      now: this.$moment().format('YYYY-MM-DD HH:mm')
    }
  },
  mounted: function () {
    let vuethis = this
    vuethis.$nextTick(function () {
      vuethis.hospList3 = vuethis.$store.state.Basic.hospList
      window.addEventListener('resize', this.GetWindowHeight)
      window.addEventListener('resize', this.GtWindowWidth)
      window.addEventListener('scroll', this.handleScroll)
      // Init
      vuethis.GetWindowHeight()
      vuethis.GtWindowWidth()
      vuethis.$api.MC.getMC_SITE_INFO()
        .then(result => {
          vuethis.siteList = result.data
        })
        .catch(err => {
          vuethis.$notification.error({
            message: err.data
          })
        })
    })
  },
  computed: {
    hospList () {
      return this.hospList2
    },
    SITE_AREA () {
      let vuethis = this
      if (vuethis.site_id) {
        let item = vuethis.siteList.filter(x => x.SITE_ID === vuethis.site_id)
        if (item && item.length > 0) {
          return item[0].SITE_AREA
        }
      }
      return ''
    },
    site_title () {
      let vuethis = this
      if (vuethis.site_id) {
        let item = vuethis.siteList.filter(x => x.SITE_ID === vuethis.site_id)
        if (item && item.length > 0) {
          return item[0].SITE_DESC + '(' + item[0].SITE_AREA + ')'
        }
      }
      return ''
    },
    site_id () {
      if (this.$auth.getSiteID()) {
        return this.$auth.getSiteID()
      }
      return ''
    },
    hosp_id () {
      if (this.$auth.getHospID()) {
        return this.$auth.getHospID()
      }
      return ''
    },
    selectPatList () {
      return this.$store.state.selectPatList
    },
    selectPatListLength () {
      return this.$store.state.selectPatList.length
    },
    patListLength () {
      return this.$store.state.Basic.PatListByID.length
    },
    patList () {
      return this.$store.state.patList
    },
    title () {
      return document.title
    }
  },
  watch: {
    titleName () {
      return document.title
    }
  },
  methods: {
    logOut () {
      var vuethis = this
      sessionStorage.clear()
      vuethis.$router.push({ name: 'menu' })
    },
    error (err) {
      if (typeof err === 'string') {
        this.$notification.error({
          message: err
        })
      } else {
        this.$notification.error({
          message: err.data
        })
      }
    },
    info (pVal) {
      if (typeof pVal === 'string') {
        this.$notification.info({
          message: pVal
        })
      }
    },
    FlattenObject (ob) {
      var toReturn = {}

      for (var i in ob) {
        if (!ob.hasOwnProperty(i)) continue

        if (typeof ob[i] === 'object' && ob[i] !== null) {
          var flatObject = this.FlattenObject(ob[i])
          for (var x in flatObject) {
            if (!flatObject.hasOwnProperty(x)) continue

            toReturn[i + '.' + x] = flatObject[x]
          }
        } else {
          toReturn[i] = ob[i]
        }
      }
      return toReturn
    },
    handleScroll () {
      this.scrolled = window.scrollY > 0
    },
    GetWindowHeight: function (event) {
      this.windowHeight = document.documentElement.clientHeight
      this.$store.commit({
        type: 'Basic/SetWindowHeight',
        windowWidth: document.documentElement.clientWidth
      })
    },
    GtWindowWidth: function (event) {
      var vuethis = this
      vuethis.contentTop = 0
      vuethis.windowWidth = document.documentElement.clientWidth
      vuethis.$store.commit({
        type: 'Basic/SetWindowWidth',
        windowWidth: document.documentElement.clientWidth
      })
    },
    SetNewModel () {
      return JSON.parse(JSON.stringify(DB_MC_PATIENT_INFO))
      //   return {
      //     datastatus: '0',
      //     PATIENT_ID: '',
      //     PATIENT_NAME: '',
      //     sex: '',
      //     ageType: '',
      //     TRIAGE: '',
      //     nation_type: '',
      //     start_date: this.$moment().format('YYYY-MM-DD HH:mm:ss'),
      //     amb_id: ''
      //   }
    },
    moment (dateString, format) {
      var _dateString = !!dateString
      return _dateString ? this.$moment(dateString, format) : null
    }
  },
  beforeDestroy: function () {
    window.removeEventListener('resize', this.GetWindowHeight)
    window.removeEventListener('resize', this.GtWindowWidth)
    window.removeEventListener('scroll', this.handleScroll)
  }
}
