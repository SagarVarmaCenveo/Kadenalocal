webpackJsonp([76],{439:function(e,t,a){try{(function(){"use strict";function e(e,t){if(!(e instanceof t))throw new TypeError("Cannot call a class as a function")}Object.defineProperty(t,"__esModule",{value:!0});var a=function t(a){e(this,t);var o=a.dataset,n=o.storageActive,r=o.storageKey,c=o.storageValue,l=o.storageChange;"true"===n&&localStorage.setItem(r,c),"true"===l&&a.addEventListener("click",function(){localStorage.getItem(r)===c?localStorage.removeItem(r):localStorage.setItem(r,c)})};t.default=a}).call(this)}finally{}}});