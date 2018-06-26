import React from 'react';
import PropTypes from 'prop-types';
/* components */
import { Tooltip } from 'react-tippy';
import TextInput from 'app.dump/Form/TextInput';

const EditOrder = (props) => {
  const removeButton = props.removeButton
    ? (
      <button
        type="button"
        className="cart-product__btn mt-2"
        onClick={props.removeOrder}
      >
        {props.removeButton}
      </button>
    ) : null;

  return (
    <div className="edit-order">
      <img
        src={props.image}
        className="edit-order__image"
        alt={props.template}
      />
      <div className="edit-order__info">
        <p>{props.templatePrefix}: <strong>{props.template}</strong></p>
        <p className="edit-order__price">{props.price}</p>
        <div className="edit-order__quantity">
          {props.quantityPrefix}
          <Tooltip
            title={props.titleTooltip}
            position="bottom"
            animation="fade"
            open={props.openTooltip}
            arrow
            theme="danger"
          >
            <TextInput
              type="number"
              onChange={props.onChange}
              value={props.value}
            />
          </Tooltip>
          {props.unitOfMeasure}
        </div>
        {removeButton}
      </div>
    </div>
  );
};

EditOrder.propTypes = {
  quantityPrefix: PropTypes.string.isRequired,
  template: PropTypes.string.isRequired,
  quantity: PropTypes.number.isRequired,
  image: PropTypes.string.isRequired,
  price: PropTypes.string.isRequired,
  templatePrefix: PropTypes.string.isRequired,
  unitOfMeasure: PropTypes.string.isRequired,
  openTooltip: PropTypes.bool.isRequired,
  titleTooltip: PropTypes.string.isRequired,
  onChange: PropTypes.func.isRequired,
  value: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  removeButton: PropTypes.string,
  removeOrder: PropTypes.func
};

export default EditOrder;