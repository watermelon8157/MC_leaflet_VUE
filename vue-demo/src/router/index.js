import Vue from 'vue'
import Router from 'vue-router'
import routeConfig from '@/router/routeConfig'
import routers from '@/router/routes.js'

Vue.use(Router)
const router = new Router({
  routes: routers
})
router.beforeEach(routeConfig)
export default router
