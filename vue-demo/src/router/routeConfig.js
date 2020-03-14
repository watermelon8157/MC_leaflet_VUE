export default function (to, from, next) {
  document.title = to.meta.title
  document.titleHead = to.meta.title
  next()
}
