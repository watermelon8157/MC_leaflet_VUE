<template>
  <div
    class="wrapper bg-gray-300"
    :style="{background: 'url('+BG+')','background-size': 'cover'}"
  >
    <a-card title="Exercise Plans" class="mt-20">
      <a-list bordered>
        <a-list-item>
          <a-icon type="solution" class="mt-3 mr-4" />
          <span>
            <a-checkbox>Injury toll in OOOO explosions reaches 400.</a-checkbox>
            <a-rate :defaultValue="5" :tooltips="desc">
              <a-icon slot="character" type="fire" theme="twoTone" twoToneColor="#eb2f96" />
            </a-rate>
            <span class="ant-rate-text">{{desc[5 - 1]}}</span>
          </span>
        </a-list-item>
        <a-list-item>
          <a-icon type="solution" class="mt-3 mr-4" />
          <span>
            <a-checkbox>Injury toll in OOOO earthquake reaches 500.</a-checkbox>
            <a-rate :defaultValue="5" :tooltips="desc">
              <a-icon slot="character" type="fire" theme="twoTone" twoToneColor="#eb2f96" />
            </a-rate>
            <span class="ant-rate-text">{{desc[5 - 1]}}</span>
          </span>
        </a-list-item>
        <a-list-item>
          <a-icon type="solution" class="mt-3 mr-4" />
          <span>
            <a-checkbox>Injury toll in OOOO landslide reaches 300.</a-checkbox>
            <a-rate :defaultValue="3" :tooltips="desc">
              <a-icon slot="character" type="fire" />
            </a-rate>
            <span class="ant-rate-text">{{desc[3 - 1]}}</span>
          </span>
        </a-list-item>
        <a-list-item>
          <a-icon type="solution" class="mt-3 mr-4" />
          <span>
            <a-checkbox>Injury toll in OOOO typhoon reaches 200.</a-checkbox>
            <a-rate :defaultValue="3" :tooltips="desc">
              <a-icon slot="character" type="fire" />
            </a-rate>
            <span class="ant-rate-text">{{desc[3 - 1]}}</span>
          </span>
        </a-list-item>
      </a-list>
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
