"use strict";
const merge = require("webpack-merge");
const prodEnv = require("./prod.env");

module.exports = merge(prodEnv, {
  WEB: '"/"',
  NODE_ENV: '"development"'
});
