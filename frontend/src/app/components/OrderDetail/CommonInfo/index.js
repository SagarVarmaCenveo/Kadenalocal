import React from 'react';
import PropTypes from 'prop-types';
/* components */
import SVG from 'app.dump/SVG';
/* helpers */
import timeFormat from 'app.helpers/time';

const CommonInfo = ({ ui }) => {
  const { status, orderDate, shippingDate, totalCost, dateTimeNAString } = ui;

  const standardizeData = (value) => {
    let date = dateTimeNAString;
    if (value) date = timeFormat(value);

    return date;
  };

  const tiles = [
    {
      title: status.title,
      value: status.value,
      icon: 'flag'
    },
    {
      title: orderDate.title,
      value: standardizeData(orderDate.value),
      icon: 'calendar'
    },
    {
      title: shippingDate.title,
      value: standardizeData(shippingDate.value),
      icon: 'truck'
    },
    {
      title: totalCost.title,
      value: totalCost.value,
      icon: 'dollar'
    }
  ];

  const tileList = tiles.map((tile) => {
    const { value, icon, title } = tile;
    return (
      <div key={icon} className="tile-bar__item">
        <SVG name={icon} className="icon-tile"/>
        <div>
          <p className="tile-bar__title">{title}</p>
          <p className="tile-bar__description">{value}</p>
        </div>
      </div>
    );
  });

  return (
    <div className="order-block">
      <div className="tile-bar">
        {tileList}
      </div>
    </div>
  );
};

CommonInfo.propTypes = {
  ui: PropTypes.shape({
    dateTimeNAString: PropTypes.string.isRequired,
    status: PropTypes.shape({
      title: PropTypes.string.isRequired,
      value: PropTypes.string.isRequired
    }).isRequired,
    orderDate: PropTypes.shape({
      title: PropTypes.string.isRequired,
      value: PropTypes.string
    }).isRequired,
    shippingDate: PropTypes.shape({
      title: PropTypes.string.isRequired,
      value: PropTypes.string
    }).isRequired,
    totalCost: PropTypes.shape({
      title: PropTypes.string.isRequired,
      value: PropTypes.string.isRequired
    }).isRequired
  })
};

export default CommonInfo;
