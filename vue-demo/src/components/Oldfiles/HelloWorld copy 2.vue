<template>
  <div
    class="wrapper bg-gray-300"
    :style="{background: 'url('+BG+')','background-size': 'cover'}"
  >
    <a-card title="Edit" class="mt-20 w-10/12">
      <a-button slot="extra" type="primary">Save</a-button>
      <v1></v1>
    </a-card>
  </div>
</template>

<script>
import BG from '@/assets/iphone.png'
import v1 from '@/components/view1'
import v2 from '@/components/view2'
import v3 from '@/components/view3'

export default {
  components: { v1, v2, v3 },
  name: 'HelloWorld',
  data () {
    return {
      data: [
        'Racing car sprays burning fuel into crowd.',
        'Japanese princess to wed commoner.',
        'Australian walks 100km after outback crash.',
        'Man charged over missing wedding girl.',
        'Los Angeles battles huge wildfires.'
      ],
      desc: ['normal ', 'normal', 'normal ', 'hard ', 'very hard'],
      BG: BG
    }
  }
}
</script>
<style scoped>
.wrapper {
  display: flex;
  align-items: center;
  flex-direction: column;
  width: 100%;
  min-height: 100%;
  padding: 20px;
}
</style>
<!-- Add "scoped" attribute to limit CSS to this component only -->
