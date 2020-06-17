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
          <a-col :span="7" class="m-2">
            <a-card class="bg-blue-300" @click="(e)=>open('Area')">
              <a-button type="link" class="text-4xl font-bold">現場入口</a-button>
            </a-card>
          </a-col>
          <a-col :span="7" class="m-2">
            <a-card class="bg-orange-300" @click="(e)=>open('Center')">
              <a-button type="link" class="text-4xl font-bold">中心入口</a-button>
            </a-card>
          </a-col>
          <a-col :span="7" class="m-2">
            <a-card class="bg-green-300" @click="(e)=>open('Hosp')">
              <a-button type="link" class="text-4xl font-bold">醫院入口</a-button>
            </a-card>
          </a-col>
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
