webpackJsonp([32],{386:function(module,exports,__webpack_require__){try{(function(){"use strict";function _classCallCheck(instance,Constructor){if(!(instance instanceof Constructor))throw new TypeError("Cannot call a class as a function")}Object.defineProperty(exports,"__esModule",{value:!0});var Storage=function Storage(container){_classCallCheck(this,Storage);var _container$dataset=container.dataset,storageActive=_container$dataset.storageActive,storageKey=_container$dataset.storageKey,storageValue=_container$dataset.storageValue,storageChange=_container$dataset.storageChange;"true"===storageActive&&localStorage.setItem(storageKey,storageValue),"true"===storageChange&&container.addEventListener("click",function(){localStorage.getItem(storageKey)===storageValue?localStorage.removeItem(storageKey):localStorage.setItem(storageKey,storageValue)})};exports.default=Storage}).call(this)}finally{}}});