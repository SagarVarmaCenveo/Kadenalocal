webpackJsonp([54,66,98],{350:function(e,t,n){try{(function(){"use strict";function e(e){return e&&e.__esModule?e:{default:e}}function r(e,t){if(!(e instanceof t))throw new TypeError("Cannot call a class as a function")}function a(e,t){if(!e)throw new ReferenceError("this hasn't been initialised - super() hasn't been called");return!t||"object"!=typeof t&&"function"!=typeof t?e:t}function o(e,t){if("function"!=typeof t&&null!==t)throw new TypeError("Super expression must either be null or a function, not "+typeof t);e.prototype=Object.create(t&&t.prototype,{constructor:{value:e,enumerable:!1,writable:!0,configurable:!0}}),t&&(Object.setPrototypeOf?Object.setPrototypeOf(e,t):e.__proto__=t)}Object.defineProperty(t,"__esModule",{value:!0});var l=function(){function e(e,t){for(var n=0;n<t.length;n++){var r=t[n];r.enumerable=r.enumerable||!1,r.configurable=!0,"value"in r&&(r.writable=!0),Object.defineProperty(e,r.key,r)}}return function(t,n,r){return n&&e(t.prototype,n),r&&e(t,r),t}}(),u=n(20),i=e(u),c=n(48),f=(e(c),function(e){function t(){return r(this,t),a(this,(t.__proto__||Object.getPrototypeOf(t)).apply(this,arguments))}return o(t,e),l(t,[{key:"render",value:function(){var e=this.props,t=e.title,n=e.value,r=e.className;return i.default.createElement("div",{className:r},i.default.createElement("span",{className:"summary-table__info"},t),i.default.createElement("span",{className:"summary-table__line"}),i.default.createElement("span",{className:"summary-table__amount"},n))}}]),t}(u.Component));t.default=f}).call(this)}finally{}},353:function(e,t,n){try{(function(){"use strict";function e(e){return e&&e.__esModule?e:{default:e}}Object.defineProperty(t,"__esModule",{value:!0});var r=Object.assign||function(e){for(var t=1;t<arguments.length;t++){var n=arguments[t];for(var r in n)Object.prototype.hasOwnProperty.call(n,r)&&(e[r]=n[r])}return e},a=n(20),o=e(a),l=n(48),u=e(l),i=n(350),c=e(i),f=function(e){var t=e.items,n=t.map(function(e,n){var a="summary-table__row";return n===t.length-1&&(a+=" summary-table__amount--emphasized"),o.default.createElement(c.default,r({className:a,key:e.title},e))});return o.default.createElement("div",{className:"summary-table"},n)};f.propTypes={items:u.default.array.isRequired},t.default=f}).call(this)}finally{}},406:function(e,t,n){try{(function(){"use strict";function e(e){return e&&e.__esModule?e:{default:e}}function r(e,t){if(!(e instanceof t))throw new TypeError("Cannot call a class as a function")}function a(e,t){if(!e)throw new ReferenceError("this hasn't been initialised - super() hasn't been called");return!t||"object"!=typeof t&&"function"!=typeof t?e:t}function o(e,t){if("function"!=typeof t&&null!==t)throw new TypeError("Super expression must either be null or a function, not "+typeof t);e.prototype=Object.create(t&&t.prototype,{constructor:{value:e,enumerable:!1,writable:!0,configurable:!0}}),t&&(Object.setPrototypeOf?Object.setPrototypeOf(e,t):e.__proto__=t)}Object.defineProperty(t,"__esModule",{value:!0});var l=function(){function e(e,t){for(var n=0;n<t.length;n++){var r=t[n];r.enumerable=r.enumerable||!1,r.configurable=!0,"value"in r&&(r.writable=!0),Object.defineProperty(e,r.key,r)}}return function(t,n,r){return n&&e(t.prototype,n),r&&e(t,r),t}}(),u=n(20),i=e(u),c=n(48),f=(e(c),n(353)),s=e(f),p=function(e){function t(){return r(this,t),a(this,(t.__proto__||Object.getPrototypeOf(t)).apply(this,arguments))}return o(t,e),l(t,[{key:"render",value:function(){var e=this.props.ui,t=e.title,n=e.description,r=e.items,a=n?i.default.createElement("p",{className:"cart-fill__info"},n):null;return i.default.createElement("div",null,i.default.createElement("h2",null,t),i.default.createElement("div",{className:"cart-fill__block"},a,i.default.createElement("div",{className:"cart-fill__block-inner"},i.default.createElement("div",{className:"cart-fill__summary-table"},i.default.createElement(s.default,{items:r})))))}}]),t}(u.Component);t.default=p}).call(this)}finally{}}});