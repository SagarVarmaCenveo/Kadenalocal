import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Immutable from 'immutable';
import ImmutablePropTypes from 'react-immutable-proptypes';
import axios from 'axios';
/* globals */
import { PRODUCT_DETAIL, BUTTONS_UI } from 'app.globals';
/* constants */
import { FAILURE, CART_PREVIEW_CHANGE_ITEMS, HIDE, HEADER_SHADOW } from 'app.consts';
/* helpers */
import { toggleDialogAlert } from 'app.helpers/ac';
/* components */
import ProductThumbnail from './ProductThumbnail';
import Info from './Info';
import Table from './Table';
import Stock from './Stock';
import Proceed from './Proceed';
import ProductOptions from './ProductOptions';

class ProductDetail extends Component {
  static defaultProps = {
    ui: Immutable.fromJS(PRODUCT_DETAIL)
  };

  static propTypes = {
    ui: ImmutablePropTypes.mapContains({
      thumbnail: ImmutablePropTypes.map.isRequired,
      requiresApproval: ImmutablePropTypes.map.isRequired,
      attachments: ImmutablePropTypes.list,
      info: ImmutablePropTypes.map,
      estimates: ImmutablePropTypes.map,
      dynamicPrices: ImmutablePropTypes.map,
      availability: PropTypes.string,
      packagingInfo: PropTypes.string,
      quantityText: PropTypes.string,
      addToCart: ImmutablePropTypes.mapContains({
        quantity: PropTypes.number,
        minQuantity: PropTypes.number,
        maxQuantity: PropTypes.number,
        documentId: PropTypes.string.isRequired,
        url: PropTypes.string.isRequired,
        quantityErrorText: PropTypes.string.isRequired
      }),
      openTemplate: ImmutablePropTypes.map,
      description: ImmutablePropTypes.mapContains({
        title: PropTypes.string.isRequired,
        text: PropTypes.string.isRequired
      }),
      productOptions: ImmutablePropTypes.mapContains({
        priceUrl: PropTypes.string.isRequired,
        priceElementId: PropTypes.string.isRequired
      })
    }).isRequired
  };

  constructor(props) {
    super(props);

    const options = {};
    const categories = props.ui.getIn(['productOptions', 'categories']);
    const tiersItems = props.ui.getIn(['addToCart', 'tiers', 'items']);

    if (categories) {
      categories.forEach((category) => {
        const selected = category.get('options').find((option) => {
          return option.get('selected') && !option.get('disabled');
        });

        if (selected) {
          options[category.get('name')] = selected.get('id');
        } else {
          options[category.get('name')] = null;
        }
      });
    }

    const quantity = (tiersItems && tiersItems.count()) ? '' : this.props.ui.getIn(['addToCart', 'quantity'], 1);

    this.state = {
      options: Immutable.Map(options),
      optionsPrice: null,
      quantity,
      isLoading: false,
      optionsError: false,
      quanityError: '',
      availability: null
    };
  }

  componentDidMount() {
    const getAvailabilityUrl = this.props.ui.get('availability');
    if (getAvailabilityUrl) {
      this.setState({
        availability: Immutable.Map({ type: '', text: '' })
      }, () => {
        this.getAvailability(getAvailabilityUrl);
      });
    }
  }

  getAvailability = async () => {
    const url = this.props.ui.get('availability');
    if (url) {
      try {
        const availability = this.state.availability
          .set('type', '')
          .set('text', '');
        this.setState({ availability, isLoading: true });
        const response = await axios.get(url);
        const { success, payload, errorMessage } = response.data;
        if (success) {
          const availability = this.state.availability
            .set('type', payload.type)
            .set('text', payload.text);
          this.setState({ availability });
        } else {
          window.store.dispatch({ type: FAILURE, alert: errorMessage });
        }
      } catch (e) {
        window.store.dispatch({ type: FAILURE });
      } finally {
        this.setState({ isLoading: false });
      }
    }
  };

  isValid = () => {
    const { ui } = this.props;
    // validation
    // options
    // we have productOptions and categories
    if (ui.get('productOptions') &&
      ui.getIn(['productOptions', 'categories']) &&
      ui.getIn(['productOptions', 'categories']).count()) {
      // check options state for null
      const emptyOption = this.state.options.findEntry(value => !value);
      if (emptyOption) {
        this.setState({ optionsError: true });
        return false;
      }
      this.setState({ optionsError: false });
    }

    // quantity
    const minQuantity = parseInt(ui.getIn(['addToCart', 'minQuantity']), 10);
    const maxQuantity = parseInt(ui.getIn(['addToCart', 'maxQuantity']), 10);
    const { quantity } = this.state;

    // check min max
    if (minQuantity && maxQuantity) {
      if (quantity < minQuantity || quantity > maxQuantity) {
        this.setState({ quanityError: ui.getIn(['addToCart', 'quantityErrorText']) });
        return false;
      }
    // check min
    }

    if (minQuantity) {
      if (quantity < minQuantity) {
        this.setState({ quanityError: ui.getIn(['addToCart', 'quantityErrorText']) });
        return false;
      }
    // check max
    }

    if (maxQuantity) {
      if (quantity > maxQuantity) {
        this.setState({ quanityError: ui.getIn(['addToCart', 'quantityErrorText']) });
        return false;
      }
    }

    if (quantity < 1) {
      this.setState({ quanityError: ui.getIn(['addToCart', 'quantityErrorText']) });
      return false;
    }

    if (isNaN(+quantity)) {
      this.setState({ quanityError: ui.getIn(['addToCart', 'quantityErrorText']) });
      return false;
    }


    return true;
  };

  proceedProduct = () => {
    // validation
    if (!this.isValid()) return;

    this.setState({ isLoading: true });
    const { ui } = this.props;
    const { quantity, options } = this.state;

    // collect body
      // Quantity
      // Options
      // documentId
    const body = {
      quantity,
      options: options.toJS(),
      documentId: ui.getIn(['addToCart', 'documentId'])
    };

    // send
    axios
      .post(ui.getIn(['addToCart', 'url']), body)
      .then((response) => {
        this.setState({ isLoading: false });
        const { payload, success, errorMessage } = response.data;

        if (!success) {
          this.setState({ quanityError: errorMessage });
          return;
        }

        const closeDialog = () => {
          toggleDialogAlert(false);
          window.store.dispatch({ type: HEADER_SHADOW + HIDE });
        };

        const { confirmation, cartPreview } = payload;

        window.store.dispatch({
          type: CART_PREVIEW_CHANGE_ITEMS,
          payload: {
            items: cartPreview.items,
            summaryPrice: cartPreview.summaryPrice
          }
        });

        const confirmBtn = [
          {
            label: BUTTONS_UI.products.text,
            func: () => window.location.assign(BUTTONS_UI.products.url)
          },
          {
            label: BUTTONS_UI.checkout.text,
            func: () => window.location.assign(BUTTONS_UI.checkout.url)
          }
        ];

        // show notification
        toggleDialogAlert(true, confirmation.alertMessage, closeDialog, confirmBtn);
        this.getAvailability();
      })
      .catch(() => {
        window.store.dispatch({ type: CART_PREVIEW_CHANGE_ITEMS + FAILURE });
        this.setState({ isLoading: false });
      });
  };

  handleChangeQuantity = (quantity) => {
    this.setState({
      quantity,
      quanityError: ''
    });
  };

  handleChangeOptions = (name, value) => {
    const options = this.state.options.set(name, value);

    const emptyOption = options.findEntry(value => !value);
    if (!emptyOption) {
      this.setState({ options, optionsError: false }, this.getOptionsPrice);
    } else {
      this.setState({ options, optionsError: true });
    }
  };

  getOptionsPrice = () => {
    axios
      .post(this.props.ui.getIn(['productOptions', 'priceUrl']), this.state.options.toJS())
      .then((res) => {
        const { errorMessage, payload, success } = res.data;
        if (success) {
          this.setState({ optionsPrice: `${payload.pricePrefix} ${payload.priceValue.toFixed(2)}` });
        } else {
          window.store.dispatch({
            type: FAILURE,
            alert: errorMessage
          });
        }
      })
      .catch(() => {
        window.store.dispatch({ type: FAILURE });
      });
  };

  render() {
    const { ui } = this.props;

    const packagingInfoComponent = ui.get('packagingInfo')
      ? (
        <span className="add-to-cart__info mr-3">{ui.get('packagingInfo')}</span>
      ) : null;

    const descriptionComponent = ui.get('description') && ui.getIn(['description', 'text'])
      ? (
        <div className="block">
          <h2 className="block__heading pt-4 pb-2">{ui.getIn(['description', 'title'])}</h2>
          <p dangerouslySetInnerHTML={{ __html: ui.getIn(['description', 'text']) }} />
        </div>
      ) : null;

    const quantityTextComponent = ui.get('quantityText')
      ? (
        <div className="block">
          <h2 className="block__heading text--danger pt-4 pb-2">{ui.get('quantityText')}</h2>
        </div>
      ) : null;

    return (
      <div>
        <div className="product-view">

          <ProductThumbnail
            thumbnail={ui.get('thumbnail')}
            attachments={ui.get('attachments')}
            requiresApproval={ui.get('requiresApproval')}
          />

          <div className="product-view__block">
            <Info
              info={ui.get('info')}
            />
            <ProductOptions
              productOptions={ui.get('productOptions')}
              handleChangeOptions={this.handleChangeOptions}
              stateOptions={this.state.options}
              optionsError={this.state.optionsError}
            />
            <div className="product-view__tables">
              <Table
                data={ui.get('estimates')}
                optionsPrice={this.state.optionsPrice}
                priceElementId={ui.getIn(['productOptions', 'priceElementId'])}
                estimates
              />
              <Table
                data={ui.get('dynamicPrices')}
                optionsPrice={this.state.optionsPrice}
                priceElementId={ui.getIn(['productOptions', 'priceElementId'])}
              />
            </div>
            <div className="product-view__footer">
              <Stock
                availability={this.state.availability}
              />
              {packagingInfoComponent}
              <Proceed
                addToCart={ui.get('addToCart')}
                openTemplate={ui.get('openTemplate')}
                handleChangeQuantity={this.handleChangeQuantity}
                quantity={this.state.quantity}
                proceedProduct={this.proceedProduct}
                isLoading={this.state.isLoading}
                quanityError={this.state.quanityError}
              />
            </div>
          </div>
        </div>
        {descriptionComponent}
        {quantityTextComponent}
      </div>
    );
  }
}

export default ProductDetail;