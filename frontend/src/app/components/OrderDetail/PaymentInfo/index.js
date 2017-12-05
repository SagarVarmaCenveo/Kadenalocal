import React from 'react';
import PropTypes from 'prop-types';
/* components */
import SVG from 'app.dump/SVG';
/* helpers */
import timeFormat from 'app.helpers/time';

const PaymentInfo = ({ ui }) => {
  const { title, paymentIcon, paidBy, paymentDetail, date, datePrefix } = ui;

  const paymentMethodInfo = paymentDetail ? <p>{paidBy},<br /> {paymentDetail}</p> : <p>{paidBy}</p>;

  return (
    <div className="order-block order-block--tile ">
      <h2 className="order-block__header">{title}</h2>
      <div className="order-block__detail">
        <SVG name={paymentIcon}/>
        {paymentMethodInfo}
        <p>{datePrefix}: <span className="weight--bold">{timeFormat(date)}</span></p>
      </div>
    </div>
  );
};

PaymentInfo.propTypes = {
  ui: PropTypes.shape({
    title: PropTypes.string.isRequired,
    paymentIcon: PropTypes.string.isRequired,
    paidBy: PropTypes.string.isRequired,
    paymentDetail: PropTypes.string.isRequired,
    date: PropTypes.string.isRequired,
    datePrefix: PropTypes.string.isRequired
  })
};

export default PaymentInfo;
