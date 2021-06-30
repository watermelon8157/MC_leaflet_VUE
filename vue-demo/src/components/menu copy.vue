<style scoped>
</style>
<template>
  <a-layout id="components-layout-demo-top" class="layout">
    <a-layout-header>
      <div class="logo" />
    </a-layout-header>
    <a-layout-content style="padding: 0 50px">

    </a-layout-content>
    <a-layout-footer style="text-align: center"></a-layout-footer>
  </a-layout>
</template>
<script>
export default {
  data () {
    return {

    }
  },
  methods: {
    open (pName) {
      this.$router.push({
        name: pName,
        params: { now: this.$moment().format('YYYYMMDDHHmmss') }
      })
    }
  }
}
</script>
