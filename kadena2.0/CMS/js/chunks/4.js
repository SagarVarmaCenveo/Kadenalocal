webpackJsonp([4],{295:function(e,t,i){try{(function(){"use strict";function e(e,t){if(e.length<4)return e;var i=Array.from(e),n=[];return i.reverse().forEach(function(e,i){i%3||!i||n.push(t),n.push(e)}),n.reverse().join("")}Object.defineProperty(t,"__esModule",{value:!0}),t.default=e}).call(this)}finally{}},304:function(e,t,i){try{(function(){"use strict";Object.defineProperty(t,"__esModule",{value:!0});var e=i(15),n=function(e){return e&&e.__esModule?e:{default:e}}(e),r=i(231),a=function(e){return e.isShownHeaderShadow?n.default.createElement("div",{className:"header-overlay"}," "):null};t.default=(0,r.connect)(function(e){return{isShownHeaderShadow:e.isShownHeaderShadow}},{})(a)}).call(this)}finally{}},315:function(e,t,i){try{(function(){"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.default={xs:320,sm:576,md:768,lg:1024,xl:1200}}).call(this)}finally{}},323:function(e,t,i){try{(function(){"use strict";function e(e,t,i){return t in e?Object.defineProperty(e,t,{value:i,enumerable:!0,configurable:!0,writable:!0}):e[t]=i,e}function i(e,t){if(!(e instanceof t))throw new TypeError("Cannot call a class as a function")}Object.defineProperty(t,"__esModule",{value:!0});var n=function(){function e(e,t){for(var i=0;i<t.length;i++){var n=t[i];n.enumerable=n.enumerable||!1,n.configurable=!0,"value"in n&&(n.writable=!0),Object.defineProperty(e,n.key,n)}}return function(t,i,n){return i&&e(t.prototype,i),n&&e(t,n),t}}(),r=function(){function t(n){i(this,t),this.container=n;var r=n.querySelector(".js-drop-zone-file"),a=n.querySelector(".js-drop-zone-item");this.fileContainer=n.querySelector(".js-drop-zone-droppped"),this.inputFilesCount=n.querySelector(".js-drop-zone-files-count"),this.nameInput=document.querySelector(".js-drop-zone-name-input"),this.acceptedFormatsStr=n.dataset.accepted,this.acceptedFormats=this.acceptedFormatsStr?this.acceptedFormatsStr.split(","):[],this.selector="isDropped",this.reverseSelector="isNotDropped",this.maxItems=n.dataset.maxItems?+n.dataset.maxItems:1e7,this.idealItem=a.cloneNode(!0),this.idealInput=r.cloneNode(!0),this.count=0,this.number=0,r.addEventListener("change",this.addFile.bind(this)),this.createRemover(a),this.data=e({},this.number,{input:r,item:a}),document.querySelector(".js-drop-zone-submit").addEventListener("click",this.submit.bind(this))}return n(t,[{key:"submit",value:function(){this.inputFilesCount.setAttribute("value",this.count);var e=this.container.querySelectorAll(".js-drop-zone-file"),t=1;e.forEach(function(e){e.value&&(e.setAttribute("name","file"+t),t+=1)})}},{key:"addFile",value:function(e){var i=this,n=t.getFileFullName(e.target.files[0]),r=n.name,a=n.ext;if(!this.isFormatAccepted(a))return this.container.classList.remove(this.selector),this.container.classList.add(this.reverseSelector),this.changeNameInput(""),void this.container.querySelector(".js-drop-zone-invalid-btn").addEventListener("click",function(){i.container.classList.remove(i.selector),i.container.classList.remove(i.reverseSelector),i.container.querySelector(".js-drop-zone-file").value="",i.changeNameInput("")});var s=e.target.dataset.id;t.setNameToItem(r,a,this.data[s].item),this.changeNameInput(r),this.container.classList.remove(this.reverseSelector),this.container.classList.add(this.selector),this.count+=1,this.number+=1;var o=this.idealItem.cloneNode(!0);if(this.createRemover(o),this.fileContainer.insertBefore(this.data[s].item,this.fileContainer.firstChild),this.count===this.maxItems){this.data[s].input.style.display="none";var l=this.idealInput.cloneNode(!0);return l.setAttribute("id","last"),l.style.display="none",void this.container.insertBefore(l,e.target)}var c=this.idealInput.cloneNode(!0);c.setAttribute("data-id",this.number),c.addEventListener("change",this.addFile.bind(this)),this.data[this.number]={input:c,item:o},this.data[s].input.style.display="none",this.container.insertBefore(c,e.target)}},{key:"removeFile",value:function(e){var t=e.target.dataset.id;if(this.count===this.maxItems){var i=this.container.querySelector("#last");i.parentNode.removeChild(i);var n=this.container.querySelector(".js-drop-zone-file");n.style.display="none";var r=this.idealItem.cloneNode(!0);this.createRemover(r);var a=this.idealInput.cloneNode(!0);a.setAttribute("data-id",this.number),a.addEventListener("change",this.addFile.bind(this)),this.data[this.number]={input:a,item:r},this.container.insertBefore(a,n),this.changeNameInput("")}this.count-=1;var s=this.data[t],o=s.item,l=s.input;o.parentNode.removeChild(o),l.parentNode.removeChild(l),this.container.querySelector(".js-drop-zone-file").style.display="block",0===this.count&&(this.container.classList.remove(this.selector),this.container.classList.remove(this.reverseSelector)),delete this.data[t]}},{key:"createRemover",value:function(e){var t=e.querySelector(".js-drop-zone-btn");t.setAttribute("data-id",this.number),t.addEventListener("click",this.removeFile.bind(this))}},{key:"changeNameInput",value:function(e){this.nameInput&&(this.nameInput.hasAttribute("disabled")||(this.nameInput.value=e))}},{key:"isFormatAccepted",value:function(e){return!this.acceptedFormatsStr||this.acceptedFormats.includes(e)}}],[{key:"getFileFullName",value:function(e){var t=e.name,i=t.split(".");return{name:t,ext:i[i.length-1]}}},{key:"setNameToItem",value:function(e,t,i){i.querySelector(".js-drop-zone-name").innerHTML=e,i.querySelector(".js-drop-zone-ext").innerHTML="."+t.toUpperCase()}}]),t}();t.default=r}).call(this)}finally{}},324:function(e,t,i){try{(function(){"use strict";function e(e,t){if(!(e instanceof t))throw new TypeError("Cannot call a class as a function")}Object.defineProperty(t,"__esModule",{value:!0});var n=i(295),r=function(e){return e&&e.__esModule?e:{default:e}}(n),a=function t(i){e(this,t);var n=i.innerHTML,a=n.split("."),s=a[0],o=a[1],l=(0,r.default)(s,",");o&&(l+="."+o),i.innerHTML=l};t.default=a}).call(this)}finally{}}});