import axios from 'axios';
import { toastr } from 'react-redux-toastr';
/* helpers */
import { consoleException } from 'app.helpers/io';
import { getSecondLevelDomain } from 'app.helpers/location';
/* globals */
import { CHILI_SAVE, NOTIFICATION } from 'app.globals';
/* classes */
import AddToCart from '../add-to-cart';

class ChiliEditor extends AddToCart {
  constructor(frame) {
    super();
    this.editor = null;
    this.frameWindow = null;

    const newDomain = getSecondLevelDomain();
    if (newDomain) document.domain = newDomain;

    this.initEditor = this.initEditor.bind(this);
    this.addToCart = this.addToCart.bind(this);
    this.initActions = this.initActions.bind(this);
    this.editorLoaded = this.editorLoaded.bind(this);
    this.saveTemplate = this.saveTemplate.bind(this);
    this.revertTemplate = this.revertTemplate.bind(this);

    window.addToCart = this.addToCart;

    frame.addEventListener('load', () => {
      this.initActions();
      this.initEditor(frame);
    });
  }

  editorLoaded() {
    this.editor = this.frameWindow.editorObject;
  }

  initEditor(frame) {
    this.frameWindow = frame.contentWindow;
    this.frameWindow.GetEditor(this.editorLoaded);
  }

  async addToCart(isAddToCart) {
    try {
      const { data: { success, errorMessage } } = await axios.post(CHILI_SAVE.url, this.getBody());
      if (success) {
        toastr.success(NOTIFICATION.serverNotAvailable.title, NOTIFICATION.serverNotAvailable.text);
        if (isAddToCart) this.addToCartRequest();
      } else {
        toastr.error(errorMessage);
      }
    } catch (e) {
      toastr.error(NOTIFICATION.serverNotAvailable.title, NOTIFICATION.serverNotAvailable.text);
    }
  }

  initActions() {
    const saveBtn = document.querySelector('.js-chili-save');
    if (saveBtn) {
      saveBtn.disabled = false;
      saveBtn.addEventListener('click', () => this.saveTemplate(false));
    }

    const addToCartBtn = document.querySelector('.js-chili-addtocart');
    if (addToCartBtn) {
      addToCartBtn.disabled = false;
      addToCartBtn.addEventListener('click', () => this.saveTemplate(true));
    }

    const revertBtn = document.querySelector('.js-chili-revert');
    if (revertBtn) revertBtn.addEventListener('click', this.revertTemplate);
  }

  saveTemplate(isAddToCart) {
    this.addToCart(isAddToCart);
    this.editor.ExecuteFunction('document', 'Save');
  }

  revertTemplate() {
    this.editor.ExecuteFunction('document', 'Revert');
  }
}

export default ChiliEditor;
