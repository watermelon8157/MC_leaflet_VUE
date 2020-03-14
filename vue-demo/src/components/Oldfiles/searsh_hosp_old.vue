<template>
  <div>
    <a-tabs defaultActiveKey="1" @change="callback">
      <a-tab-pane tab="民眾版" key="1">
        <v1></v1>
      </a-tab-pane>
      <a-tab-pane tab="專業版" key="2">
        <v2></v2>
      </a-tab-pane>
      <a-tab-pane tab="各院狀況" key="3">
        <v3></v3>
      </a-tab-pane>
    </a-tabs>
  </div>
</template>

<script>
import v1 from '@/components/view1'
import v2 from '@/components/view2'
import v3 from '@/components/view3'

export default {
  components: { v1, v2, v3 },
  name: 'HelloWorld',
  data () {
    return {
      msg: 'Welcome to Your Vue.js App'
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
