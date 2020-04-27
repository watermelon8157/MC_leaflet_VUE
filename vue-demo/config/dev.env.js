'use strict'
const merge = require('webpack-merge')
const prodEnv = require('./prod.env')
let params = process.argv[4]
let VER = '"MC"'
let API = '"http://localhost:5467/"'
// API = '"https://8674d450.ngrok.io/MC"'
module.exports = merge(prodEnv, {
	NODE_ENV: '"development"',
	API: API,
	WEB: '"/"', 
	VER: VER
})
