webpackJsonp([6],{332:function(e,t,i){"use strict";function n(e,t,i){return t in e?Object.defineProperty(e,t,{value:i,enumerable:!0,configurable:!0,writable:!0}):e[t]=i,e}function r(e,t){if(!(e instanceof t))throw new TypeError("Cannot call a class as a function")}Object.defineProperty(t,"__esModule",{value:!0});var s=function(){function e(e,t){for(var i=0;i<t.length;i++){var n=t[i];n.enumerable=n.enumerable||!1,n.configurable=!0,"value"in n&&(n.writable=!0),Object.defineProperty(e,n.key,n)}}return function(t,i,n){return i&&e(t.prototype,i),n&&e(t,n),t}}(),a=function(){function e(t){r(this,e),this.container=t;var i=t.querySelector(".js-drop-zone-file"),s=t.querySelector(".js-drop-zone-item");this.fileContainer=t.querySelector(".js-drop-zone-droppped"),this.inputFilesCount=t.querySelector(".js-drop-zone-files-count"),this.nameInput=document.querySelector(".js-drop-zone-name-input"),this.acceptedFormatsStr=t.dataset.accepted,this.acceptedFormats=this.acceptedFormatsStr?this.acceptedFormatsStr.split(","):[],this.selector="isDropped",this.reverseSelector="isNotDropped",this.maxItems=t.dataset.maxItems?+t.dataset.maxItems:1e7,this.idealItem=s.cloneNode(!0),this.idealInput=i.cloneNode(!0),this.count=0,this.number=0,i.addEventListener("change",this.addFile.bind(this)),this.createRemover(s),this.data=n({},this.number,{input:i,item:s}),document.querySelector(".js-drop-zone-submit").addEventListener("click",this.submit.bind(this))}return s(e,[{key:"submit",value:function(){this.inputFilesCount.setAttribute("value",this.count);var e=this.container.querySelectorAll(".js-drop-zone-file"),t=1;e.forEach(function(e){e.value&&(e.setAttribute("name","file"+t),t+=1)})}},{key:"addFile",value:function(t){var i=this,n=e.getFileFullName(t.target.files[0]),r=n.name,s=n.ext;if(!this.isFormatAccepted(s))return this.container.classList.remove(this.selector),this.container.classList.add(this.reverseSelector),this.changeNameInput(""),void this.container.querySelector(".js-drop-zone-invalid-btn").addEventListener("click",function(){i.container.classList.remove(i.selector),i.container.classList.remove(i.reverseSelector),i.container.querySelector(".js-drop-zone-file").value="",i.changeNameInput("")});var a=t.target.dataset.id;e.setNameToItem(r,s,this.data[a].item),this.changeNameInput(r),this.container.classList.remove(this.reverseSelector),this.container.classList.add(this.selector),this.count+=1,this.number+=1;var o=this.idealItem.cloneNode(!0);if(this.createRemover(o),this.fileContainer.insertBefore(this.data[a].item,this.fileContainer.firstChild),this.count===this.maxItems){this.data[a].input.style.display="none";var c=this.idealInput.cloneNode(!0);return c.setAttribute("id","last"),c.style.display="none",void this.container.insertBefore(c,t.target)}var l=this.idealInput.cloneNode(!0);l.setAttribute("data-id",this.number),l.addEventListener("change",this.addFile.bind(this)),this.data[this.number]={input:l,item:o},this.data[a].input.style.display="none",this.container.insertBefore(l,t.target)}},{key:"removeFile",value:function(e){var t=e.target.dataset.id;if(this.count===this.maxItems){var i=this.container.querySelector("#last");i.parentNode.removeChild(i);var n=this.container.querySelector(".js-drop-zone-file");n.style.display="none";var r=this.idealItem.cloneNode(!0);this.createRemover(r);var s=this.idealInput.cloneNode(!0);s.setAttribute("data-id",this.number),s.addEventListener("change",this.addFile.bind(this)),this.data[this.number]={input:s,item:r},this.container.insertBefore(s,n),this.changeNameInput("")}this.count-=1;var a=this.data[t],o=a.item,c=a.input;o.parentNode.removeChild(o),c.parentNode.removeChild(c),this.container.querySelector(".js-drop-zone-file").style.display="block",0===this.count&&(this.container.classList.remove(this.selector),this.container.classList.remove(this.reverseSelector)),delete this.data[t]}},{key:"createRemover",value:function(e){var t=e.querySelector(".js-drop-zone-btn");t.setAttribute("data-id",this.number),t.addEventListener("click",this.removeFile.bind(this))}},{key:"changeNameInput",value:function(e){this.nameInput&&(this.nameInput.hasAttribute("disabled")||(this.nameInput.value=e))}},{key:"isFormatAccepted",value:function(e){return!this.acceptedFormatsStr||this.acceptedFormats.includes(e)}}],[{key:"getFileFullName",value:function(e){var t=e.name,i=t.split(".");return{name:t,ext:i[i.length-1]}}},{key:"setNameToItem",value:function(e,t,i){i.querySelector(".js-drop-zone-name").innerHTML=e,i.querySelector(".js-drop-zone-ext").innerHTML="."+t.toUpperCase()}}]),e}();t.default=a}});