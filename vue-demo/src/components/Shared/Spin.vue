<style scoped>
.basic-spin {
  text-align: center;
  background: rgba(0, 0, 0, 0.05);
  border-radius: 4px;
  margin-bottom: 20px;
  padding: 30px 50px;
  margin: 20px 0;
}
</style>
<template>
  <a-modal :visible="visible" :closable="false" :footer="null">
    <div class="basic-spin">
      <a-spin :tip="msg"></a-spin>
    </div>
  </a-modal>
</template>
<script>
export default {
  data () {
    return {}
  },
  computed: {
    visible () {
      return this.$store.state.spin
    },
    msg () {
      return this.$store.state.spinMsg
    }
  },
  methods: {
    getContainer () {
      return document.querySelector('#app')
    }
  }
}
</script>
