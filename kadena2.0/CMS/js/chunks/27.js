webpackJsonp([27,54,57],{333:function(module,exports,__webpack_require__){try{(function(){"use strict";Object.defineProperty(exports,"__esModule",{value:!0});var _config=config,localization=_config.localization,login=localization.login,checkout=localization.checkout,spotfire=localization.spotfire,userSettings=localization.userSettings;exports.LOGIN=login,exports.CHECKOUT=checkout,exports.SPOTFIRE=spotfire,exports.USER_SETTINGS=userSettings}).call(this)}finally{}},334:function(module,exports,__webpack_require__){try{(function(){"use strict";function removeProps(obj,props){var objRemovedProps=Object.assign({},obj);return props.forEach(function(prop){delete objRemovedProps[prop]}),objRemovedProps}Object.defineProperty(exports,"__esModule",{value:!0}),exports.default=removeProps}).call(this)}finally{}},351:function(module,exports,__webpack_require__){try{(function(){"use strict";function _interopRequireDefault(obj){return obj&&obj.__esModule?obj:{default:obj}}function _classCallCheck(instance,Constructor){if(!(instance instanceof Constructor))throw new TypeError("Cannot call a class as a function")}function _possibleConstructorReturn(self,call){if(!self)throw new ReferenceError("this hasn't been initialised - super() hasn't been called");return!call||"object"!=typeof call&&"function"!=typeof call?self:call}function _inherits(subClass,superClass){if("function"!=typeof superClass&&null!==superClass)throw new TypeError("Super expression must either be null or a function, not "+typeof superClass);subClass.prototype=Object.create(superClass&&superClass.prototype,{constructor:{value:subClass,enumerable:!1,writable:!0,configurable:!0}}),superClass&&(Object.setPrototypeOf?Object.setPrototypeOf(subClass,superClass):subClass.__proto__=superClass)}Object.defineProperty(exports,"__esModule",{value:!0});var _extends=Object.assign||function(target){for(var i=1;i<arguments.length;i++){var source=arguments[i];for(var key in source)Object.prototype.hasOwnProperty.call(source,key)&&(target[key]=source[key])}return target},_createClass=function(){function defineProperties(target,props){for(var i=0;i<props.length;i++){var descriptor=props[i];descriptor.enumerable=descriptor.enumerable||!1,descriptor.configurable=!0,"value"in descriptor&&(descriptor.writable=!0),Object.defineProperty(target,descriptor.key,descriptor)}}return function(Constructor,protoProps,staticProps){return protoProps&&defineProperties(Constructor.prototype,protoProps),staticProps&&defineProperties(Constructor,staticProps),Constructor}}(),_react=__webpack_require__(21),_react2=_interopRequireDefault(_react),_object=__webpack_require__(334),_object2=_interopRequireDefault(_object),_globals=__webpack_require__(333),PasswordInput=function(_Component){function PasswordInput(){_classCallCheck(this,PasswordInput);var _this=_possibleConstructorReturn(this,(PasswordInput.__proto__||Object.getPrototypeOf(PasswordInput)).call(this));return _this.state={isShown:!1},_this.handleToggle=_this.handleToggle.bind(_this),_this.passwordHideText=_globals.LOGIN.passwordHide,_this.passwordShowText=_globals.LOGIN.passwordShow,_this}return _inherits(PasswordInput,_Component),_createClass(PasswordInput,[{key:"handleToggle",value:function(){this.setState(function(prevState){return{isShown:!prevState.isShown}})}},{key:"render",value:function(){var isShown=this.state.isShown,_props=this.props,error=_props.error,label=_props.label,disabled=_props.disabled,inputProps=(0,_object2.default)(this.props,["error","label"]),labelElement=label?_react2.default.createElement("span",{className:"input__label"},label):null,toggler=isShown?this.passwordHideText:this.passwordShowText,type=isShown?"text":"password",className=disabled?"input__wrapper input__wrapper--disabled":"input__wrapper",onClick=disabled?void 0:this.handleToggle,errorElement=error?_react2.default.createElement("span",{className:"input__error input__error--noborder"},error):null,errorClass=error?"input--error":"";return _react2.default.createElement("div",{className:className},labelElement,_react2.default.createElement("div",{className:"input__inner"},_react2.default.createElement("input",_extends({type:type,className:"input__password "+errorClass},inputProps)),_react2.default.createElement("span",{onClick:onClick,className:"input__toggler"},toggler)),errorElement)}}]),PasswordInput}(_react.Component);exports.default=PasswordInput,PasswordInput.propTypes={error:_react.PropTypes.string,label:_react.PropTypes.string,disabled:_react.PropTypes.bool,placeholder:_react.PropTypes.string}}).call(this)}finally{}}});