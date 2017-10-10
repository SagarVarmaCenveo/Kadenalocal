import React from 'react';
import PropTypes from 'prop-types';
/* components */
import Product from 'app.dump/Product/Checkout';

const Products = (props) => {
  const { ui, removeProduct, changeProductQuantity, disableInteractivity } = props;
  const { number, items, buttonLabels } = ui;

  const products = items.map((item) => {
    return <Product key={item.id} {...item}
                    removeProduct={removeProduct}
                    disableInteractivity={disableInteractivity}
                    buttonLabels={buttonLabels}
                    changeProductQuantity={changeProductQuantity} />;
  });

  return (
    <div>
      <p className="text-right">{number}</p>
      {products}
    </div>
  );
};

Products.propTypes = {
  ui: PropTypes.shape({
    number: PropTypes.string.isRequired,
    items: PropTypes.arrayOf(PropTypes.object.isRequired).isRequired,
    buttonLabels: PropTypes.object.isRequired
  }).isRequired,
  removeProduct: PropTypes.func.isRequired,
  changeProductQuantity: PropTypes.func.isRequired,
  disableInteractivity: PropTypes.bool.isRequired
};

export default Products;
