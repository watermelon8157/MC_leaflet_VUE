<style scoped>
.slide-fade-enter-active {
  transition: all 0.3s;
}
.slide-fade-leave-active {
  transition: all 0.8s;
}
.slide-fade-enter, .slide-fade-leave-to
/* .slide-fade-leave-active for below version 2.1.8 */ {
  opacity: 0;
}
</style>
<template>
  <div>
    <transition name="slide-fade">
      <div v-if="visible">
        <a-alert
          class="fade_in"
          v-bind:type="messageType"
          v-bind:closable="closable"
          v-bind:showIcon="showIcon"
          v-bind:afterClose="handleClose"
        >
          <span slot="message" v-html="message"></span>
        </a-alert>
      </div>
    </transition>
  </div>
</template>
<script>
export default {
  props: {
    config: {
      type: Object,
      default: function () {
        return {}
      }
    }
  },
  data: function () {
    return {
      visible: false,
      message: 'Alert Message Text',
      messageType: 'warning', // Type of Alert styles, options: success, info, warning, error
      closable: true,
      showIcon: true
    }
  },
  mounted () { },
  methods: {
    // Vue Instance 在初始化時可設定選項物件，其中可設定 method，執行一些動作。
    handleClose: function () {
      this.visible = false
    },
    showInfo (data, pType) {
      this.message = data || ''
      this.messageType = pType || 'info'
      this.showType()
    },
    showErr (err, pType) {
      this.message = err.data || ''
      this.messageType = pType || 'error'
      this.showType()
    },
    show (pMessage, pType) {
      this.message = pMessage || ''
      this.messageType = pType || 'warning'
      this.showType()
    },
    showError (pMessage, pType) {
      this.message = pMessage || '程式發生錯誤，請洽資訊人員!'
      this.messageType = pType || 'error'
      this.showType()
    },
    showType () {
      this.visible = true // 訊息顯示在指定位置
      // this.showMessage();// 訊息顯示浮動方式
    },
    showMessage () {
      if (this.messageType === 'success') {
        this.$notification.success({
          message: this.message
        })
      } else if (this.messageType === 'info') {
        this.$notification.info({
          message: this.message
        })
      } else if (this.messageType === 'warning') {
        this.$notification.warning({
          message: this.message
        })
      } else if (this.messageType === 'error') {
        this.$notification.warn({
          message: this.message
        })
      } else {
        this.$notification.info({
          message: this.message
        })
      }
    },
    success (pMessage) {
      this.$notification.success({
        message: pMessage
      })
    },
    warning (pMessage) {
      this.$notification.warning({
        message: pMessage
      })
    },
    info (pMessage) {
      this.$notification.info({
        message: pMessage
      })
    },
    errorMessage (pMessage) {
      this.$message.error(pMessage)
    },
    error (err) {
      console.log(err)
      if (err.data === 'Network Error' && !err.ok) {
        this.$store.commit('SetNetworkError', true)
      } else {
        this.$store.commit('SetNetworkError', false)
      }
      this.$notification.error({
        message: err.data
      })
    }
  },
  computed: {},
  watch: {
    visible: function () {
      if (this.visible) {
        setTimeout(this.handleClose, 3000)
      }
    }
  }
}
</script>
