<style scoped>
</style>
<template>
    <div>
        <p class="error">{{ error }}</p>

        <p class="decode-result">
            Last result:
            <b>{{ result }}</b>
        </p>

        <qrcode-stream @decode="onDecode" @init="onInit" />
    </div>
</template>
<script>
// https://github.com/gruhn/vue-qrcode-reader
import { QrcodeStream } from 'vue-qrcode-reader'
export default {
    components: { QrcodeStream },

    data () {
        return {
            result: '',
            error: ''
        }
    },

    methods: {
        onDecode (result) {
            this.result = result
        }
    }
}
</script>
