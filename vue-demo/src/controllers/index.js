import axios from '@/controllers/axios'

let instance = axios()

const asyncPromisePost = async function (pUrl, params, options) {
  let res = await instance.post(pUrl, params, options)
  return res
  // eslint-disable-next-line semi
};
const asyncPromiseGet = async function (pUrl, options) {
  let res = await instance.get(pUrl, options)
  return res
  // eslint-disable-next-line semi
};
const asyncPromisePut = async function (pUrl, params, options) {
  let res = await instance.put(pUrl, params, options)
  return res
  // eslint-disable-next-line semi
};

const asyncPromiseDelete = async function (pUrl, options) {
  let res = await instance.get(pUrl, options)
  return res
  // eslint-disable-next-line semi
};

export default {
  http: axios,
  get (url, params, headers) {
    let options = {}
    if (params) {
      options.params = params
    }
    if (headers) {
      options.headers = headers
    }
    return asyncPromiseGet(url, options)
  },
  post (url, params, headers) {
    let options = {}
    return asyncPromisePost(url, params, options)
  },
  put (url, params, headers) {
    let options = {}
    if (headers) {
      options.headers = headers
    }
    return asyncPromisePut(url, params, options)
  },
  delete (url, params, headers) {
    let options = {}
    if (params) {
      options.params = params
    }
    if (headers) {
      options.headers = headers
    }
    return asyncPromiseDelete(url, options)
  }
}
