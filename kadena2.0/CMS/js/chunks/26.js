webpackJsonp([26],{352:function(module,exports,__webpack_require__){eval('/* REACT HOT LOADER */ if (false) { (function () { var ReactHotAPI = require("c:\\\\projects\\\\cenveo\\\\kentico\\\\frontend\\\\node_modules\\\\react-hot-api\\\\modules\\\\index.js"), RootInstanceProvider = require("c:\\\\projects\\\\cenveo\\\\kentico\\\\frontend\\\\node_modules\\\\react-hot-loader\\\\RootInstanceProvider.js"), ReactMount = require("react-dom/lib/ReactMount"), React = require("react"); module.makeHot = module.hot.data ? module.hot.data.makeHot : ReactHotAPI(function () { return RootInstanceProvider.getRootInstances(ReactMount); }, React); })(); } try { (function () {\n\n\'use strict\';\n\nObject.defineProperty(exports, "__esModule", {\n  value: true\n});\n\nvar _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();\n\nfunction _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }\n\nvar AddTr = function () {\n  function AddTr(container) {\n    var _this = this;\n\n    _classCallCheck(this, AddTr);\n\n    this.lastRow = container;\n    this.count = 1;\n    this.tbody = this.lastRow.parentNode;\n    this.firstRowClass = \'js-first-tr\';\n    this.firstRow = this.tbody.querySelector(\'.\' + this.firstRowClass);\n    this.firstRowTemplate = this.firstRow.cloneNode(true);\n\n    var togglers = Array.from(this.lastRow.getElementsByClassName(\'js-add-tr-toggler\'));\n\n    togglers.forEach(function (toggler) {\n      toggler.addEventListener(\'click\', function () {\n        _this.count += 1;\n        var clonnedRow = _this.firstRowTemplate.cloneNode(true);\n        var newNode = _this.getNewRow(clonnedRow);\n        _this.tbody.insertBefore(newNode, _this.lastRow);\n      });\n    });\n  }\n\n  _createClass(AddTr, [{\n    key: \'getNewRow\',\n    value: function getNewRow(oldRow) {\n      var _this2 = this;\n\n      oldRow.classList.remove(this.firstRowClass);\n      var elements = Array.from(oldRow.querySelectorAll(\'[name]\'));\n      elements.forEach(function (element) {\n        var name = element.dataset.name + \'-\' + _this2.count;\n        element.name = name;\n      });\n      return oldRow;\n    }\n  }]);\n\n  return AddTr;\n}();\n\nexports.default = AddTr;\n\n/* REACT HOT LOADER */ }).call(this); } finally { if (false) { (function () { var foundReactClasses = module.hot.data && module.hot.data.foundReactClasses || false; if (module.exports && module.makeHot) { var makeExportsHot = require("c:\\\\projects\\\\cenveo\\\\kentico\\\\frontend\\\\node_modules\\\\react-hot-loader\\\\makeExportsHot.js"); if (makeExportsHot(module, require("react"))) { foundReactClasses = true; } var shouldAcceptModule = true && foundReactClasses; if (shouldAcceptModule) { module.hot.accept(function (err) { if (err) { console.error("Cannot apply hot update to " + "index.js" + ": " + err.message); } }); } } module.hot.dispose(function (data) { data.makeHot = module.makeHot; data.foundReactClasses = foundReactClasses; }); })(); } }//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiMzUyLmpzIiwic291cmNlcyI6WyJ3ZWJwYWNrOi8vLy4vc3JjL2FwcC9zdGF0aWMvYWRkLXRyL2luZGV4LmpzP2I3N2UiXSwic291cmNlc0NvbnRlbnQiOlsiLyogUkVBQ1QgSE9UIExPQURFUiAqLyBpZiAobW9kdWxlLmhvdCkgeyAoZnVuY3Rpb24gKCkgeyB2YXIgUmVhY3RIb3RBUEkgPSByZXF1aXJlKFwiYzpcXFxccHJvamVjdHNcXFxcY2VudmVvXFxcXGtlbnRpY29cXFxcZnJvbnRlbmRcXFxcbm9kZV9tb2R1bGVzXFxcXHJlYWN0LWhvdC1hcGlcXFxcbW9kdWxlc1xcXFxpbmRleC5qc1wiKSwgUm9vdEluc3RhbmNlUHJvdmlkZXIgPSByZXF1aXJlKFwiYzpcXFxccHJvamVjdHNcXFxcY2VudmVvXFxcXGtlbnRpY29cXFxcZnJvbnRlbmRcXFxcbm9kZV9tb2R1bGVzXFxcXHJlYWN0LWhvdC1sb2FkZXJcXFxcUm9vdEluc3RhbmNlUHJvdmlkZXIuanNcIiksIFJlYWN0TW91bnQgPSByZXF1aXJlKFwicmVhY3QtZG9tL2xpYi9SZWFjdE1vdW50XCIpLCBSZWFjdCA9IHJlcXVpcmUoXCJyZWFjdFwiKTsgbW9kdWxlLm1ha2VIb3QgPSBtb2R1bGUuaG90LmRhdGEgPyBtb2R1bGUuaG90LmRhdGEubWFrZUhvdCA6IFJlYWN0SG90QVBJKGZ1bmN0aW9uICgpIHsgcmV0dXJuIFJvb3RJbnN0YW5jZVByb3ZpZGVyLmdldFJvb3RJbnN0YW5jZXMoUmVhY3RNb3VudCk7IH0sIFJlYWN0KTsgfSkoKTsgfSB0cnkgeyAoZnVuY3Rpb24gKCkge1xuXG4ndXNlIHN0cmljdCc7XG5cbk9iamVjdC5kZWZpbmVQcm9wZXJ0eShleHBvcnRzLCBcIl9fZXNNb2R1bGVcIiwge1xuICB2YWx1ZTogdHJ1ZVxufSk7XG5cbnZhciBfY3JlYXRlQ2xhc3MgPSBmdW5jdGlvbiAoKSB7IGZ1bmN0aW9uIGRlZmluZVByb3BlcnRpZXModGFyZ2V0LCBwcm9wcykgeyBmb3IgKHZhciBpID0gMDsgaSA8IHByb3BzLmxlbmd0aDsgaSsrKSB7IHZhciBkZXNjcmlwdG9yID0gcHJvcHNbaV07IGRlc2NyaXB0b3IuZW51bWVyYWJsZSA9IGRlc2NyaXB0b3IuZW51bWVyYWJsZSB8fCBmYWxzZTsgZGVzY3JpcHRvci5jb25maWd1cmFibGUgPSB0cnVlOyBpZiAoXCJ2YWx1ZVwiIGluIGRlc2NyaXB0b3IpIGRlc2NyaXB0b3Iud3JpdGFibGUgPSB0cnVlOyBPYmplY3QuZGVmaW5lUHJvcGVydHkodGFyZ2V0LCBkZXNjcmlwdG9yLmtleSwgZGVzY3JpcHRvcik7IH0gfSByZXR1cm4gZnVuY3Rpb24gKENvbnN0cnVjdG9yLCBwcm90b1Byb3BzLCBzdGF0aWNQcm9wcykgeyBpZiAocHJvdG9Qcm9wcykgZGVmaW5lUHJvcGVydGllcyhDb25zdHJ1Y3Rvci5wcm90b3R5cGUsIHByb3RvUHJvcHMpOyBpZiAoc3RhdGljUHJvcHMpIGRlZmluZVByb3BlcnRpZXMoQ29uc3RydWN0b3IsIHN0YXRpY1Byb3BzKTsgcmV0dXJuIENvbnN0cnVjdG9yOyB9OyB9KCk7XG5cbmZ1bmN0aW9uIF9jbGFzc0NhbGxDaGVjayhpbnN0YW5jZSwgQ29uc3RydWN0b3IpIHsgaWYgKCEoaW5zdGFuY2UgaW5zdGFuY2VvZiBDb25zdHJ1Y3RvcikpIHsgdGhyb3cgbmV3IFR5cGVFcnJvcihcIkNhbm5vdCBjYWxsIGEgY2xhc3MgYXMgYSBmdW5jdGlvblwiKTsgfSB9XG5cbnZhciBBZGRUciA9IGZ1bmN0aW9uICgpIHtcbiAgZnVuY3Rpb24gQWRkVHIoY29udGFpbmVyKSB7XG4gICAgdmFyIF90aGlzID0gdGhpcztcblxuICAgIF9jbGFzc0NhbGxDaGVjayh0aGlzLCBBZGRUcik7XG5cbiAgICB0aGlzLmxhc3RSb3cgPSBjb250YWluZXI7XG4gICAgdGhpcy5jb3VudCA9IDE7XG4gICAgdGhpcy50Ym9keSA9IHRoaXMubGFzdFJvdy5wYXJlbnROb2RlO1xuICAgIHRoaXMuZmlyc3RSb3dDbGFzcyA9ICdqcy1maXJzdC10cic7XG4gICAgdGhpcy5maXJzdFJvdyA9IHRoaXMudGJvZHkucXVlcnlTZWxlY3RvcignLicgKyB0aGlzLmZpcnN0Um93Q2xhc3MpO1xuICAgIHRoaXMuZmlyc3RSb3dUZW1wbGF0ZSA9IHRoaXMuZmlyc3RSb3cuY2xvbmVOb2RlKHRydWUpO1xuXG4gICAgdmFyIHRvZ2dsZXJzID0gQXJyYXkuZnJvbSh0aGlzLmxhc3RSb3cuZ2V0RWxlbWVudHNCeUNsYXNzTmFtZSgnanMtYWRkLXRyLXRvZ2dsZXInKSk7XG5cbiAgICB0b2dnbGVycy5mb3JFYWNoKGZ1bmN0aW9uICh0b2dnbGVyKSB7XG4gICAgICB0b2dnbGVyLmFkZEV2ZW50TGlzdGVuZXIoJ2NsaWNrJywgZnVuY3Rpb24gKCkge1xuICAgICAgICBfdGhpcy5jb3VudCArPSAxO1xuICAgICAgICB2YXIgY2xvbm5lZFJvdyA9IF90aGlzLmZpcnN0Um93VGVtcGxhdGUuY2xvbmVOb2RlKHRydWUpO1xuICAgICAgICB2YXIgbmV3Tm9kZSA9IF90aGlzLmdldE5ld1JvdyhjbG9ubmVkUm93KTtcbiAgICAgICAgX3RoaXMudGJvZHkuaW5zZXJ0QmVmb3JlKG5ld05vZGUsIF90aGlzLmxhc3RSb3cpO1xuICAgICAgfSk7XG4gICAgfSk7XG4gIH1cblxuICBfY3JlYXRlQ2xhc3MoQWRkVHIsIFt7XG4gICAga2V5OiAnZ2V0TmV3Um93JyxcbiAgICB2YWx1ZTogZnVuY3Rpb24gZ2V0TmV3Um93KG9sZFJvdykge1xuICAgICAgdmFyIF90aGlzMiA9IHRoaXM7XG5cbiAgICAgIG9sZFJvdy5jbGFzc0xpc3QucmVtb3ZlKHRoaXMuZmlyc3RSb3dDbGFzcyk7XG4gICAgICB2YXIgZWxlbWVudHMgPSBBcnJheS5mcm9tKG9sZFJvdy5xdWVyeVNlbGVjdG9yQWxsKCdbbmFtZV0nKSk7XG4gICAgICBlbGVtZW50cy5mb3JFYWNoKGZ1bmN0aW9uIChlbGVtZW50KSB7XG4gICAgICAgIHZhciBuYW1lID0gZWxlbWVudC5kYXRhc2V0Lm5hbWUgKyAnLScgKyBfdGhpczIuY291bnQ7XG4gICAgICAgIGVsZW1lbnQubmFtZSA9IG5hbWU7XG4gICAgICB9KTtcbiAgICAgIHJldHVybiBvbGRSb3c7XG4gICAgfVxuICB9XSk7XG5cbiAgcmV0dXJuIEFkZFRyO1xufSgpO1xuXG5leHBvcnRzLmRlZmF1bHQgPSBBZGRUcjtcblxuLyogUkVBQ1QgSE9UIExPQURFUiAqLyB9KS5jYWxsKHRoaXMpOyB9IGZpbmFsbHkgeyBpZiAobW9kdWxlLmhvdCkgeyAoZnVuY3Rpb24gKCkgeyB2YXIgZm91bmRSZWFjdENsYXNzZXMgPSBtb2R1bGUuaG90LmRhdGEgJiYgbW9kdWxlLmhvdC5kYXRhLmZvdW5kUmVhY3RDbGFzc2VzIHx8IGZhbHNlOyBpZiAobW9kdWxlLmV4cG9ydHMgJiYgbW9kdWxlLm1ha2VIb3QpIHsgdmFyIG1ha2VFeHBvcnRzSG90ID0gcmVxdWlyZShcImM6XFxcXHByb2plY3RzXFxcXGNlbnZlb1xcXFxrZW50aWNvXFxcXGZyb250ZW5kXFxcXG5vZGVfbW9kdWxlc1xcXFxyZWFjdC1ob3QtbG9hZGVyXFxcXG1ha2VFeHBvcnRzSG90LmpzXCIpOyBpZiAobWFrZUV4cG9ydHNIb3QobW9kdWxlLCByZXF1aXJlKFwicmVhY3RcIikpKSB7IGZvdW5kUmVhY3RDbGFzc2VzID0gdHJ1ZTsgfSB2YXIgc2hvdWxkQWNjZXB0TW9kdWxlID0gdHJ1ZSAmJiBmb3VuZFJlYWN0Q2xhc3NlczsgaWYgKHNob3VsZEFjY2VwdE1vZHVsZSkgeyBtb2R1bGUuaG90LmFjY2VwdChmdW5jdGlvbiAoZXJyKSB7IGlmIChlcnIpIHsgY29uc29sZS5lcnJvcihcIkNhbm5vdCBhcHBseSBob3QgdXBkYXRlIHRvIFwiICsgXCJpbmRleC5qc1wiICsgXCI6IFwiICsgZXJyLm1lc3NhZ2UpOyB9IH0pOyB9IH0gbW9kdWxlLmhvdC5kaXNwb3NlKGZ1bmN0aW9uIChkYXRhKSB7IGRhdGEubWFrZUhvdCA9IG1vZHVsZS5tYWtlSG90OyBkYXRhLmZvdW5kUmVhY3RDbGFzc2VzID0gZm91bmRSZWFjdENsYXNzZXM7IH0pOyB9KSgpOyB9IH1cblxuXG4vLy8vLy8vLy8vLy8vLy8vLy9cbi8vIFdFQlBBQ0sgRk9PVEVSXG4vLyAuL3NyYy9hcHAvc3RhdGljL2FkZC10ci9pbmRleC5qc1xuLy8gbW9kdWxlIGlkID0gMzUyXG4vLyBtb2R1bGUgY2h1bmtzID0gMjYiXSwibWFwcGluZ3MiOiJBQUFBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBIiwic291cmNlUm9vdCI6IiJ9')}});