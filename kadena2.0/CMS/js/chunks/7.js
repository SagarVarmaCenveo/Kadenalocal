webpackJsonp([7],{330:function(s,t,e){"use strict";function i(s,t){if(!(s instanceof t))throw new TypeError("Cannot call a class as a function")}Object.defineProperty(t,"__esModule",{value:!0});var a=function s(t){var e=this;i(this,s),this.clicker=t,this.activeClass="active",this.html=document.querySelector("html");var a=t.dataset.dialog;this.dialog=document.querySelector(a),this.closerNodes=this.dialog.querySelectorAll(".dialog__closer"),this.clicker.addEventListener("click",function(){!e.dialog.classList.contains(e.activeClass)&&e.dialog.classList.add(e.activeClass),e.html.classList.add("css-overflow-hidden")}),Array.from(this.closerNodes).forEach(function(s){s.addEventListener("click",function(){e.dialog.classList.contains(e.activeClass)&&e.dialog.classList.remove(e.activeClass),e.html.classList.remove("css-overflow-hidden")})})};t.default=a}});