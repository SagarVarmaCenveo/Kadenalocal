webpackJsonp([37],{380:function(module,exports,__webpack_require__){try{(function(){"use strict";function _classCallCheck(instance,Constructor){if(!(instance instanceof Constructor))throw new TypeError("Cannot call a class as a function")}Object.defineProperty(exports,"__esModule",{value:!0});var _createClass=function(){function defineProperties(target,props){for(var i=0;i<props.length;i++){var descriptor=props[i];descriptor.enumerable=descriptor.enumerable||!1,descriptor.configurable=!0,"value"in descriptor&&(descriptor.writable=!0),Object.defineProperty(target,descriptor.key,descriptor)}}return function(Constructor,protoProps,staticProps){return protoProps&&defineProperties(Constructor.prototype,protoProps),staticProps&&defineProperties(Constructor,staticProps),Constructor}}(),Notification=function(){function Notification(container){_classCallCheck(this,Notification),this.container=container,this.timeForAnimation=+container.dataset.timeForAnimation,this.timeToRemove=+container.dataset.timeToRemove;this.notification={};for(var i=1;i<=3;i+=1)this.notification[i]=void 0}return _createClass(Notification,[{key:"addNotification",value:function(type){function createNewNotification(){var idealElement=that.container.querySelector('[data-notification-type="'+type+'"]'),newElement=idealElement.cloneNode(!0);return newElement.querySelector(".js-notification-closer").addEventListener("click",function(event){var notification=event.currentTarget.parentNode;notification.classList.add("hide"),pullDownNotificationsAbove(notification),setTimeout(function(){notification.remove()},that.timeForAnimation)}),newElement}function pullDownNotificationsAbove(notification){var keys=Object.keys(that.notification),values=Object.values(that.notification),value=values.indexOf(notification),key=keys[value];that.notification[key]=void 0,Object.keys(that.notification).forEach(function(index){if(index>key&&that.notification[index]){var element=that.notification[index],newIndex=index-1;element.classList.remove("show-"+index),element.classList.add("show-"+newIndex),that.notification[newIndex]=element,that.notification[index]=void 0}})}var that=this;!function(){var lastNumber=Object.keys(that.notification).length,element=that.notification[lastNumber];element&&(element.classList.add("hide"),that.notification[lastNumber]=void 0,setTimeout(function(){element.remove()},that.timeForAnimation))}(),function(){for(var elementNumbers=Object.keys(that.notification).length,index=elementNumbers;index>0;index-=1)if(index!==elementNumbers){var element=that.notification[index];if(element){element.classList.remove("show-"+index);var newIndex=index+1;element.classList.add("show-"+newIndex),that.notification[newIndex]=element}}}(),function(){var newElement=createNewNotification();that.notification[1]=newElement,that.container.appendChild(newElement),window.getComputedStyle(newElement).opacity,newElement.classList.add("show-1"),setTimeout(function(){newElement.classList.add("hide")},that.timeToRemove),setTimeout(function(){newElement.remove()},that.timeForAnimation+that.timeToRemove)}()}}]),Notification}();exports.default=Notification}).call(this)}finally{}}});