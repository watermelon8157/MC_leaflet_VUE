<template>
  <a-card title="Exercise Plans">
    <a-list bordered itemLayout="horizontal">
      <template v-for="(i) in 10">
        <a-list-item :key="i+ '1'">
          <a-button type="primary" slot="actions">comply</a-button>
          <a-list-item-meta>
            <span slot="title">Injury toll in OOOO explosions reaches 400.</span>
            <a-avatar slot="avatar" icon="solution" />
            <span slot="description">
              <a-rate :defaultValue="5" :tooltips="desc">
                <a-icon slot="character" type="fire" theme="twoTone" twoToneColor="#eb2f96" />
              </a-rate>
              <span class="ant-rate-text">{{desc[5 - 1]}}</span>
            </span>
          </a-list-item-meta>
        </a-list-item>
        <a-list-item :key="i+ '2'">
          <a-button type="primary" slot="actions">comply</a-button>
          <a-list-item-meta>
            <span slot="title">Injury toll in OOOO earthquake reaches 500.</span>
            <a-avatar slot="avatar" icon="solution" />
            <span slot="description">
              <a-rate :defaultValue="5" :tooltips="desc">
                <a-icon slot="character" type="fire" theme="twoTone" twoToneColor="#eb2f96" />
              </a-rate>
              <span class="ant-rate-text">{{desc[5 - 1]}}</span>
            </span>
          </a-list-item-meta>
        </a-list-item>
        <a-list-item :key="i+ '3'">
          <a-button type="primary" slot="actions">comply</a-button>
          <a-list-item-meta>
            <span slot="title">Injury toll in OOOO landslide reaches 300.</span>
            <a-avatar slot="avatar" icon="solution" />
            <span slot="description">
              <a-rate :defaultValue="5" :tooltips="desc">
                <a-icon slot="character" type="fire" theme="twoTone" twoToneColor="#eb2f96" />
              </a-rate>
              <span class="ant-rate-text">{{desc[5 - 1]}}</span>
            </span>
          </a-list-item-meta>
        </a-list-item>
        <a-list-item :key="i+ '4'">
          <a-button type="primary" slot="actions">comply</a-button>
          <a-list-item-meta>
            <span slot="title">Injury toll in OOOO typhoon reaches 200.</span>
            <a-avatar slot="avatar" icon="solution" />
            <span slot="description">
              <a-rate :defaultValue="5" :tooltips="desc">
                <a-icon slot="character" type="fire" theme="twoTone" twoToneColor="#eb2f96" />
              </a-rate>
              <span class="ant-rate-text">{{desc[5 - 1]}}</span>
            </span>
          </a-list-item-meta>
        </a-list-item>
      </template>
    </a-list>
  </a-card>
</template>

<script>

export default {
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
      desc: ['normal ', 'normal', 'normal ', 'hard ', 'very hard']
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
