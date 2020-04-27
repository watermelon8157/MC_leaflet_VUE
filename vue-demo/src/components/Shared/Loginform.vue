<template>
  <div>
    <a-form layout="inline">
      <div>
        <a-form-item
          required
          :validate-status="!!site_id ? '' : 'error'"
          :help="!!site_id ? ' ':'請輸入事件代碼'"
        >
          <a-input v-model="site_id" placeholder="事件代碼" @pressEnter="(e)=>Login()">
            <a-icon slot="prefix" type="user" style="color:rgba(0,0,0,.25)" />
          </a-input>
        </a-form-item>
      </div>
      <div>
        <a-form-item>
          <a-button :disabled="disabled" type="primary" @click="Login">{{btnTxt}}</a-button>
        </a-form-item>
      </div>
    </a-form>
  </div>
</template>
<script>
export default {
  props: {
    loginDrawer: {
      type: Boolean,
      default: false
    },
    btnTxt: {
      type: String,
      default: '登入'
    },
    user_id: {
      type: String,
      default: ''
    }
  },
  data () {
    return {
      User_pwd: '',
      site_id: '',
      disabled: false
    }
  },

  computed: {
    now () {
      return this.$moment().format('YYYYMMDDHHmmss')
    }
  },
  methods: {
    Login () {
      this.$api.MC.LoginForm({ site_id: this.site_id })
        .then(result => {
          if (result.ok) {
            this.$auth.setToken(result.data.token)
            this.disabled = false
            this.$emit('emitLogin')
          }
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
