webpackJsonp([28],{347:function(module,exports,__webpack_require__){eval('/* REACT HOT LOADER */ if (false) { (function () { var ReactHotAPI = require("c:\\\\inetpub\\\\wwwroot\\\\Kadena-k10-core\\\\frontend\\\\node_modules\\\\react-hot-api\\\\modules\\\\index.js"), RootInstanceProvider = require("c:\\\\inetpub\\\\wwwroot\\\\Kadena-k10-core\\\\frontend\\\\node_modules\\\\react-hot-loader\\\\RootInstanceProvider.js"), ReactMount = require("react-dom/lib/ReactMount"), React = require("react"); module.makeHot = module.hot.data ? module.hot.data.makeHot : ReactHotAPI(function () { return RootInstanceProvider.getRootInstances(ReactMount); }, React); })(); } try { (function () {\n\n\'use strict\';\n\nObject.defineProperty(exports, "__esModule", {\n  value: true\n});\nexports.default = separate;\nfunction separate(str) {\n  if (str.length < 4) return str;\n\n  var array = Array.from(str);\n  var formattedArray = [];\n\n  array.reverse().forEach(function (item, index) {\n    if (!(index % 3) && index) formattedArray.push(\' \');\n    formattedArray.push(item);\n  });\n\n  return formattedArray.reverse().join(\'\');\n}\n\n/* REACT HOT LOADER */ }).call(this); } finally { if (false) { (function () { var foundReactClasses = module.hot.data && module.hot.data.foundReactClasses || false; if (module.exports && module.makeHot) { var makeExportsHot = require("c:\\\\inetpub\\\\wwwroot\\\\Kadena-k10-core\\\\frontend\\\\node_modules\\\\react-hot-loader\\\\makeExportsHot.js"); if (makeExportsHot(module, require("react"))) { foundReactClasses = true; } var shouldAcceptModule = true && foundReactClasses; if (shouldAcceptModule) { module.hot.accept(function (err) { if (err) { console.error("Cannot apply hot update to " + "numbers.js" + ": " + err.message); } }); } } module.hot.dispose(function (data) { data.makeHot = module.makeHot; data.foundReactClasses = foundReactClasses; }); })(); } }//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiMzQ3LmpzIiwic291cmNlcyI6WyJ3ZWJwYWNrOi8vLy4vc3JjL2FwcC9oZWxwZXJzL251bWJlcnMuanM/N2M5YSJdLCJzb3VyY2VzQ29udGVudCI6WyIvKiBSRUFDVCBIT1QgTE9BREVSICovIGlmIChtb2R1bGUuaG90KSB7IChmdW5jdGlvbiAoKSB7IHZhciBSZWFjdEhvdEFQSSA9IHJlcXVpcmUoXCJjOlxcXFxpbmV0cHViXFxcXHd3d3Jvb3RcXFxcS2FkZW5hLWsxMC1jb3JlXFxcXGZyb250ZW5kXFxcXG5vZGVfbW9kdWxlc1xcXFxyZWFjdC1ob3QtYXBpXFxcXG1vZHVsZXNcXFxcaW5kZXguanNcIiksIFJvb3RJbnN0YW5jZVByb3ZpZGVyID0gcmVxdWlyZShcImM6XFxcXGluZXRwdWJcXFxcd3d3cm9vdFxcXFxLYWRlbmEtazEwLWNvcmVcXFxcZnJvbnRlbmRcXFxcbm9kZV9tb2R1bGVzXFxcXHJlYWN0LWhvdC1sb2FkZXJcXFxcUm9vdEluc3RhbmNlUHJvdmlkZXIuanNcIiksIFJlYWN0TW91bnQgPSByZXF1aXJlKFwicmVhY3QtZG9tL2xpYi9SZWFjdE1vdW50XCIpLCBSZWFjdCA9IHJlcXVpcmUoXCJyZWFjdFwiKTsgbW9kdWxlLm1ha2VIb3QgPSBtb2R1bGUuaG90LmRhdGEgPyBtb2R1bGUuaG90LmRhdGEubWFrZUhvdCA6IFJlYWN0SG90QVBJKGZ1bmN0aW9uICgpIHsgcmV0dXJuIFJvb3RJbnN0YW5jZVByb3ZpZGVyLmdldFJvb3RJbnN0YW5jZXMoUmVhY3RNb3VudCk7IH0sIFJlYWN0KTsgfSkoKTsgfSB0cnkgeyAoZnVuY3Rpb24gKCkge1xuXG4ndXNlIHN0cmljdCc7XG5cbk9iamVjdC5kZWZpbmVQcm9wZXJ0eShleHBvcnRzLCBcIl9fZXNNb2R1bGVcIiwge1xuICB2YWx1ZTogdHJ1ZVxufSk7XG5leHBvcnRzLmRlZmF1bHQgPSBzZXBhcmF0ZTtcbmZ1bmN0aW9uIHNlcGFyYXRlKHN0cikge1xuICBpZiAoc3RyLmxlbmd0aCA8IDQpIHJldHVybiBzdHI7XG5cbiAgdmFyIGFycmF5ID0gQXJyYXkuZnJvbShzdHIpO1xuICB2YXIgZm9ybWF0dGVkQXJyYXkgPSBbXTtcblxuICBhcnJheS5yZXZlcnNlKCkuZm9yRWFjaChmdW5jdGlvbiAoaXRlbSwgaW5kZXgpIHtcbiAgICBpZiAoIShpbmRleCAlIDMpICYmIGluZGV4KSBmb3JtYXR0ZWRBcnJheS5wdXNoKCcgJyk7XG4gICAgZm9ybWF0dGVkQXJyYXkucHVzaChpdGVtKTtcbiAgfSk7XG5cbiAgcmV0dXJuIGZvcm1hdHRlZEFycmF5LnJldmVyc2UoKS5qb2luKCcnKTtcbn1cblxuLyogUkVBQ1QgSE9UIExPQURFUiAqLyB9KS5jYWxsKHRoaXMpOyB9IGZpbmFsbHkgeyBpZiAobW9kdWxlLmhvdCkgeyAoZnVuY3Rpb24gKCkgeyB2YXIgZm91bmRSZWFjdENsYXNzZXMgPSBtb2R1bGUuaG90LmRhdGEgJiYgbW9kdWxlLmhvdC5kYXRhLmZvdW5kUmVhY3RDbGFzc2VzIHx8IGZhbHNlOyBpZiAobW9kdWxlLmV4cG9ydHMgJiYgbW9kdWxlLm1ha2VIb3QpIHsgdmFyIG1ha2VFeHBvcnRzSG90ID0gcmVxdWlyZShcImM6XFxcXGluZXRwdWJcXFxcd3d3cm9vdFxcXFxLYWRlbmEtazEwLWNvcmVcXFxcZnJvbnRlbmRcXFxcbm9kZV9tb2R1bGVzXFxcXHJlYWN0LWhvdC1sb2FkZXJcXFxcbWFrZUV4cG9ydHNIb3QuanNcIik7IGlmIChtYWtlRXhwb3J0c0hvdChtb2R1bGUsIHJlcXVpcmUoXCJyZWFjdFwiKSkpIHsgZm91bmRSZWFjdENsYXNzZXMgPSB0cnVlOyB9IHZhciBzaG91bGRBY2NlcHRNb2R1bGUgPSB0cnVlICYmIGZvdW5kUmVhY3RDbGFzc2VzOyBpZiAoc2hvdWxkQWNjZXB0TW9kdWxlKSB7IG1vZHVsZS5ob3QuYWNjZXB0KGZ1bmN0aW9uIChlcnIpIHsgaWYgKGVycikgeyBjb25zb2xlLmVycm9yKFwiQ2Fubm90IGFwcGx5IGhvdCB1cGRhdGUgdG8gXCIgKyBcIm51bWJlcnMuanNcIiArIFwiOiBcIiArIGVyci5tZXNzYWdlKTsgfSB9KTsgfSB9IG1vZHVsZS5ob3QuZGlzcG9zZShmdW5jdGlvbiAoZGF0YSkgeyBkYXRhLm1ha2VIb3QgPSBtb2R1bGUubWFrZUhvdDsgZGF0YS5mb3VuZFJlYWN0Q2xhc3NlcyA9IGZvdW5kUmVhY3RDbGFzc2VzOyB9KTsgfSkoKTsgfSB9XG5cblxuLy8vLy8vLy8vLy8vLy8vLy8vXG4vLyBXRUJQQUNLIEZPT1RFUlxuLy8gLi9zcmMvYXBwL2hlbHBlcnMvbnVtYmVycy5qc1xuLy8gbW9kdWxlIGlkID0gMzQ3XG4vLyBtb2R1bGUgY2h1bmtzID0gMTMgMjgiXSwibWFwcGluZ3MiOiJBQUFBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0EiLCJzb3VyY2VSb290IjoiIn0=')}});