webpackJsonp([43],{371:function(module,exports,__webpack_require__){try{(function(){"use strict";function mailing(){var state=arguments.length>0&&void 0!==arguments[0]?arguments[0]:defaultState,action=arguments.length>1&&void 0!==arguments[1]?arguments[1]:{};switch(action.type){case constants.FETCH_SERVERS:case constants.FETCH_SERVERS_SUCCESS:case constants.FETCH_SERVERS_FAILURE:return _extends({},state,{isLoading:action.isLoading});case constants.MAILING_RESPONSE_SUCCESS:return _extends({},state,{response:action.data,isLoading:!1});default:return state}}Object.defineProperty(exports,"__esModule",{value:!0});var _extends=Object.assign||function(target){for(var i=1;i<arguments.length;i++){var source=arguments[i];for(var key in source)Object.prototype.hasOwnProperty.call(source,key)&&(target[key]=source[key])}return target};exports.default=mailing;var _constants=__webpack_require__(99),constants=function(obj){if(obj&&obj.__esModule)return obj;var newObj={};if(null!=obj)for(var key in obj)Object.prototype.hasOwnProperty.call(obj,key)&&(newObj[key]=obj[key]);return newObj.default=obj,newObj}(_constants),defaultState={response:null,isLoading:!1}}).call(this)}finally{}}});