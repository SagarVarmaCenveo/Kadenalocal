webpackJsonp([22],{358:function(module,exports,__webpack_require__){eval("/* REACT HOT LOADER */ if (false) { (function () { var ReactHotAPI = require(\"c:\\\\inetpub\\\\wwwroot\\\\Kadena-k10-core\\\\frontend\\\\node_modules\\\\react-hot-api\\\\modules\\\\index.js\"), RootInstanceProvider = require(\"c:\\\\inetpub\\\\wwwroot\\\\Kadena-k10-core\\\\frontend\\\\node_modules\\\\react-hot-loader\\\\RootInstanceProvider.js\"), ReactMount = require(\"react-dom/lib/ReactMount\"), React = require(\"react\"); module.makeHot = module.hot.data ? module.hot.data.makeHot : ReactHotAPI(function () { return RootInstanceProvider.getRootInstances(ReactMount); }, React); })(); } try { (function () {\n\n'use strict';\n\nObject.defineProperty(exports, \"__esModule\", {\n  value: true\n});\n\nfunction _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError(\"Cannot call a class as a function\"); } }\n\nvar Password = function Password(container) {\n  _classCallCheck(this, Password);\n\n  var classInput = 'js-password-input';\n  var classToggler = 'js-password-toggler';\n\n  var input = container.querySelector('.' + classInput);\n  var toggler = container.querySelector('.' + classToggler);\n\n  var _toggler$dataset = toggler.dataset,\n      passwordShow = _toggler$dataset.passwordShow,\n      passwordHide = _toggler$dataset.passwordHide;\n\n\n  toggler.addEventListener('click', function () {\n    if (input.getAttribute('type') === 'text') {\n      input.setAttribute('type', 'password');\n      toggler.innerHTML = passwordShow;\n    } else {\n      input.setAttribute('type', 'text');\n      toggler.innerHTML = passwordHide;\n    }\n  });\n};\n\nexports.default = Password;\n\n/* REACT HOT LOADER */ }).call(this); } finally { if (false) { (function () { var foundReactClasses = module.hot.data && module.hot.data.foundReactClasses || false; if (module.exports && module.makeHot) { var makeExportsHot = require(\"c:\\\\inetpub\\\\wwwroot\\\\Kadena-k10-core\\\\frontend\\\\node_modules\\\\react-hot-loader\\\\makeExportsHot.js\"); if (makeExportsHot(module, require(\"react\"))) { foundReactClasses = true; } var shouldAcceptModule = true && foundReactClasses; if (shouldAcceptModule) { module.hot.accept(function (err) { if (err) { console.error(\"Cannot apply hot update to \" + \"index.js\" + \": \" + err.message); } }); } } module.hot.dispose(function (data) { data.makeHot = module.makeHot; data.foundReactClasses = foundReactClasses; }); })(); } }//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiMzU4LmpzIiwic291cmNlcyI6WyJ3ZWJwYWNrOi8vLy4vc3JjL2FwcC9zdGF0aWMvcGFzc3dvcmQvaW5kZXguanM/NzQ0YyJdLCJzb3VyY2VzQ29udGVudCI6WyIvKiBSRUFDVCBIT1QgTE9BREVSICovIGlmIChtb2R1bGUuaG90KSB7IChmdW5jdGlvbiAoKSB7IHZhciBSZWFjdEhvdEFQSSA9IHJlcXVpcmUoXCJjOlxcXFxpbmV0cHViXFxcXHd3d3Jvb3RcXFxcS2FkZW5hLWsxMC1jb3JlXFxcXGZyb250ZW5kXFxcXG5vZGVfbW9kdWxlc1xcXFxyZWFjdC1ob3QtYXBpXFxcXG1vZHVsZXNcXFxcaW5kZXguanNcIiksIFJvb3RJbnN0YW5jZVByb3ZpZGVyID0gcmVxdWlyZShcImM6XFxcXGluZXRwdWJcXFxcd3d3cm9vdFxcXFxLYWRlbmEtazEwLWNvcmVcXFxcZnJvbnRlbmRcXFxcbm9kZV9tb2R1bGVzXFxcXHJlYWN0LWhvdC1sb2FkZXJcXFxcUm9vdEluc3RhbmNlUHJvdmlkZXIuanNcIiksIFJlYWN0TW91bnQgPSByZXF1aXJlKFwicmVhY3QtZG9tL2xpYi9SZWFjdE1vdW50XCIpLCBSZWFjdCA9IHJlcXVpcmUoXCJyZWFjdFwiKTsgbW9kdWxlLm1ha2VIb3QgPSBtb2R1bGUuaG90LmRhdGEgPyBtb2R1bGUuaG90LmRhdGEubWFrZUhvdCA6IFJlYWN0SG90QVBJKGZ1bmN0aW9uICgpIHsgcmV0dXJuIFJvb3RJbnN0YW5jZVByb3ZpZGVyLmdldFJvb3RJbnN0YW5jZXMoUmVhY3RNb3VudCk7IH0sIFJlYWN0KTsgfSkoKTsgfSB0cnkgeyAoZnVuY3Rpb24gKCkge1xuXG4ndXNlIHN0cmljdCc7XG5cbk9iamVjdC5kZWZpbmVQcm9wZXJ0eShleHBvcnRzLCBcIl9fZXNNb2R1bGVcIiwge1xuICB2YWx1ZTogdHJ1ZVxufSk7XG5cbmZ1bmN0aW9uIF9jbGFzc0NhbGxDaGVjayhpbnN0YW5jZSwgQ29uc3RydWN0b3IpIHsgaWYgKCEoaW5zdGFuY2UgaW5zdGFuY2VvZiBDb25zdHJ1Y3RvcikpIHsgdGhyb3cgbmV3IFR5cGVFcnJvcihcIkNhbm5vdCBjYWxsIGEgY2xhc3MgYXMgYSBmdW5jdGlvblwiKTsgfSB9XG5cbnZhciBQYXNzd29yZCA9IGZ1bmN0aW9uIFBhc3N3b3JkKGNvbnRhaW5lcikge1xuICBfY2xhc3NDYWxsQ2hlY2sodGhpcywgUGFzc3dvcmQpO1xuXG4gIHZhciBjbGFzc0lucHV0ID0gJ2pzLXBhc3N3b3JkLWlucHV0JztcbiAgdmFyIGNsYXNzVG9nZ2xlciA9ICdqcy1wYXNzd29yZC10b2dnbGVyJztcblxuICB2YXIgaW5wdXQgPSBjb250YWluZXIucXVlcnlTZWxlY3RvcignLicgKyBjbGFzc0lucHV0KTtcbiAgdmFyIHRvZ2dsZXIgPSBjb250YWluZXIucXVlcnlTZWxlY3RvcignLicgKyBjbGFzc1RvZ2dsZXIpO1xuXG4gIHZhciBfdG9nZ2xlciRkYXRhc2V0ID0gdG9nZ2xlci5kYXRhc2V0LFxuICAgICAgcGFzc3dvcmRTaG93ID0gX3RvZ2dsZXIkZGF0YXNldC5wYXNzd29yZFNob3csXG4gICAgICBwYXNzd29yZEhpZGUgPSBfdG9nZ2xlciRkYXRhc2V0LnBhc3N3b3JkSGlkZTtcblxuXG4gIHRvZ2dsZXIuYWRkRXZlbnRMaXN0ZW5lcignY2xpY2snLCBmdW5jdGlvbiAoKSB7XG4gICAgaWYgKGlucHV0LmdldEF0dHJpYnV0ZSgndHlwZScpID09PSAndGV4dCcpIHtcbiAgICAgIGlucHV0LnNldEF0dHJpYnV0ZSgndHlwZScsICdwYXNzd29yZCcpO1xuICAgICAgdG9nZ2xlci5pbm5lckhUTUwgPSBwYXNzd29yZFNob3c7XG4gICAgfSBlbHNlIHtcbiAgICAgIGlucHV0LnNldEF0dHJpYnV0ZSgndHlwZScsICd0ZXh0Jyk7XG4gICAgICB0b2dnbGVyLmlubmVySFRNTCA9IHBhc3N3b3JkSGlkZTtcbiAgICB9XG4gIH0pO1xufTtcblxuZXhwb3J0cy5kZWZhdWx0ID0gUGFzc3dvcmQ7XG5cbi8qIFJFQUNUIEhPVCBMT0FERVIgKi8gfSkuY2FsbCh0aGlzKTsgfSBmaW5hbGx5IHsgaWYgKG1vZHVsZS5ob3QpIHsgKGZ1bmN0aW9uICgpIHsgdmFyIGZvdW5kUmVhY3RDbGFzc2VzID0gbW9kdWxlLmhvdC5kYXRhICYmIG1vZHVsZS5ob3QuZGF0YS5mb3VuZFJlYWN0Q2xhc3NlcyB8fCBmYWxzZTsgaWYgKG1vZHVsZS5leHBvcnRzICYmIG1vZHVsZS5tYWtlSG90KSB7IHZhciBtYWtlRXhwb3J0c0hvdCA9IHJlcXVpcmUoXCJjOlxcXFxpbmV0cHViXFxcXHd3d3Jvb3RcXFxcS2FkZW5hLWsxMC1jb3JlXFxcXGZyb250ZW5kXFxcXG5vZGVfbW9kdWxlc1xcXFxyZWFjdC1ob3QtbG9hZGVyXFxcXG1ha2VFeHBvcnRzSG90LmpzXCIpOyBpZiAobWFrZUV4cG9ydHNIb3QobW9kdWxlLCByZXF1aXJlKFwicmVhY3RcIikpKSB7IGZvdW5kUmVhY3RDbGFzc2VzID0gdHJ1ZTsgfSB2YXIgc2hvdWxkQWNjZXB0TW9kdWxlID0gdHJ1ZSAmJiBmb3VuZFJlYWN0Q2xhc3NlczsgaWYgKHNob3VsZEFjY2VwdE1vZHVsZSkgeyBtb2R1bGUuaG90LmFjY2VwdChmdW5jdGlvbiAoZXJyKSB7IGlmIChlcnIpIHsgY29uc29sZS5lcnJvcihcIkNhbm5vdCBhcHBseSBob3QgdXBkYXRlIHRvIFwiICsgXCJpbmRleC5qc1wiICsgXCI6IFwiICsgZXJyLm1lc3NhZ2UpOyB9IH0pOyB9IH0gbW9kdWxlLmhvdC5kaXNwb3NlKGZ1bmN0aW9uIChkYXRhKSB7IGRhdGEubWFrZUhvdCA9IG1vZHVsZS5tYWtlSG90OyBkYXRhLmZvdW5kUmVhY3RDbGFzc2VzID0gZm91bmRSZWFjdENsYXNzZXM7IH0pOyB9KSgpOyB9IH1cblxuXG4vLy8vLy8vLy8vLy8vLy8vLy9cbi8vIFdFQlBBQ0sgRk9PVEVSXG4vLyAuL3NyYy9hcHAvc3RhdGljL3Bhc3N3b3JkL2luZGV4LmpzXG4vLyBtb2R1bGUgaWQgPSAzNThcbi8vIG1vZHVsZSBjaHVua3MgPSAyMiJdLCJtYXBwaW5ncyI6IkFBQUE7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSIsInNvdXJjZVJvb3QiOiIifQ==")}});