<style scoped>
</style>
<template>
  <a-layout id="components-layout-demo-top" class="layout">
    <a-layout-header>
      <div class="logo" />
    </a-layout-header>
    <a-layout-content style="padding: 0 50px">
      <div :style="{ background: '#fff', padding: '24px', minHeight: '280px' }">
        <a-row class="text-center">
          <a-list size="small" class="w-64" bordered>
            <a-list-item
              class="bg-blue-300 cursor-pointer text-4xl text-center font-bold"
              @click="(e) => open('Area')"
              >現場入口</a-list-item
            >
            <a-list-item
              class="bg-orange-300 cursor-pointer text-4xl text-center font-bold"
              @click="(e) => open('Center')"
              >中心入口</a-list-item
            >
            <a-list-item
              class="bg-green-300 cursor-pointer text-4xl text-center font-bold"
              @click="(e) => open('Hosp')"
              >醫院入口</a-list-item
            >
          </a-list>
        </a-row>
      </div>
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
