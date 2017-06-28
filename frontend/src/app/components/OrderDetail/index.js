import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import CommonInfo from './CommonInfo';
import ShippingInfo from './ShippingInfo';
import PaymentInfo from './PaymentInfo';
import PricingInfo from './PricingInfo';
import OrderedItems from './OrderedItems';
import getUI from '../../AC/orderDetail';
import Spinner from '../Spinner';
import { getSearchObj } from '../../helpers/location';

class OrderDetail extends Component {
  static propTypes = {
    getUI: PropTypes.func.isRequired,
    orderDetail: PropTypes.shape({
      ui: PropTypes.shape({
        commonInfo: PropTypes.object,
        orderedItems: PropTypes.object,
        paymentInfo: PropTypes.object,
        pricingInfo: PropTypes.object,
        shippingInfo: PropTypes.object
      }).isRequired
    }).isRequired
  };

  componentDidMount() {
    const { getUI } = this.props;
    const { orderID } = getSearchObj();

    if (orderID) {
      getUI(orderID);
    } else {
      getUI('');
    }
  }

  render() {
    const { orderDetail } = this.props;
    const { ui } = orderDetail;
    const { commonInfo, shippingInfo, paymentInfo, pricingInfo, orderedItems } = ui;

    const content = <div>
      <CommonInfo ui={commonInfo} />

      <div className="order-block">
        <div className="row">
          <div className="col-lg-4 mb-4">
            <ShippingInfo ui={shippingInfo} />
          </div>

          <div className="col-lg-4 mb-4">
            <PaymentInfo ui={paymentInfo} />
          </div>

          <div className="col-lg-4 mb-4">
            <PricingInfo ui={pricingInfo} />
          </div>
        </div>
      </div>

      <OrderedItems ui={orderedItems}/>
    </div>;

    return Object.keys(ui).length
      ? content
      : <Spinner />;
  }
}

export default connect(({ orderDetail }) => ({ orderDetail }), {
  getUI
})(OrderDetail);
