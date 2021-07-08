<style scoped>
body {
  height: 100vh;
  background-size: cover;
  font: 100% 微軟正黑體;
  font-family: Microsoft JhengHei, Arial, sans-serif;
  font-size: 16px;
}
.login-icon-field {
  background-size: contain;
}

.wrapper {
  display: flex;
  align-items: center;
  flex-direction: column;
  justify-content: center;
  width: 100%;
  min-height: 100%;
  padding: 20px;
}

.login {
  border-radius: 2px 2px 5px 5px;
  padding: 10px 20px 20px 20px;
  width: 90%;
  max-width: 320px;
  position: relative;
  padding-bottom: 60px;
  box-shadow: 0px 1px 5px rgba(0, 0, 0, 0.3);
}

.login-icon-field {
  height: 250px;
  width: 100%;
}

.title {
  color: #444;
  font-size: 1.2em;
  font-weight: bold;
  margin: 10px 0 30px 0;
  border-bottom: 1px solid #eee;
  padding-bottom: 20px;
}

.button {
  width: 75%;
  height: 100%;
  position: absolute;
  max-height: 60px;
  margin: 0px;
}
</style>

<template>
  <a-layout
    class="wrapper"
    style="background-color: #cbd5e0"
    :style="{ 'background-size': 'cover' }"
  >
    <div class="login-icon-field"></div>
    <a-card title="大量傷患登打查詢">
      <div>
        <a-form layout="inline">
          <div>
            <a-form-item>
              <a-select v-model="func"
                style="width: 120px"
              >
                <a-select-option value=""> 請選擇入口 </a-select-option>
                <a-select-option value="Area"> 現場入口 </a-select-option>
                <a-select-option value="Hosp"> 醫院入口 </a-select-option>
                <a-select-option value="Center"> 中心入口 </a-select-option>
              </a-select>
            </a-form-item>
          </div>
          <div>
            <a-form-item>
              <a-button type="primary" @click="Login"> 登入 </a-button>
            </a-form-item>
          </div>
        </a-form>
      </div>
    </a-card>
  </a-layout>
</template>
<script>
export default {
  data () {
    return {
      func: ''
    }
  },
  methods: {
    Login () {
      this.$router.push({
        name: this.func,
        params: { now: this.$moment().format('YYYYMMDDHHmmss') }
      })
    }
  },
  mounted: function () {
    // 登入頁面
    this.$ResetStore() // 清空 store資料
  }
}
</script>
