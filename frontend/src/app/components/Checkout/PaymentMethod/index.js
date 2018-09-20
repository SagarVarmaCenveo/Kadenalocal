import React, { Component } from 'react';
import PropTypes from 'prop-types';
/* components */
import Alert from 'app.dump/Alert';
import Dialog from 'app.dump/Dialog';
/* local components */
import MethodsGroup from './MethodsGroup';
import Button from '../../_dump/Button';


class PaymentMethod extends Component {

  constructor(props) {
    super(props);

    this.state = {
      isDialogOpen: false
    };
  }

  toggleDialog = () => {
    this.setState(prevState => ({ isDialogOpen: !prevState.isDialogOpen }));
  };

  static propTypes = {
    validationMessage: PropTypes.string.isRequired,
    changePaymentMethod: PropTypes.func.isRequired,
    checkedObj: PropTypes.object.isRequired,
    ui: PropTypes.shape({
      title: PropTypes.string.isRequired,
      items: PropTypes.arrayOf(PropTypes.object),
      unPayableText: PropTypes.string,
      description: PropTypes.string,
      isPayable: PropTypes.bool,
      approvalRequiredText: PropTypes.string,
      approvalRequiredDesc: PropTypes.string,
      approvalRequiredButton: PropTypes.string
    })
  };

  render() {
    const { ui, checkedObj, changePaymentMethod, validationMessage } = this.props;
    const { title, description, items, isPayable, unPayableText, approvalRequiredText, approvalRequiredDesc, approvalRequiredButton } = ui;

    const descriptionElement = description ? <p className="cart-fill__info">{description}</p> : null;

    const methods = items.map((item) => {
      const className = 'select-accordion__item input__wrapper input__wrapper--icon-label cart-fill__block-input-wrapper';
      // if (item.hasInput) className += ' cart-fill__block-input-wrapper';

      return (
        <MethodsGroup
          changePaymentMethod={changePaymentMethod}
          checkedObj={checkedObj}
          {...item}
          className={className}
          validationMessage={validationMessage}
          key={`pm-${item.id}`}
          toggleDialog={this.toggleDialog}
          approvalRequiredText={approvalRequiredText}
        />
      );
    });

    const getDialogBody = () => {
      return (
        <p>{ui.approvalRequiredDesc}</p>
      );
    };

    const getDialogFooter = () => {

      return (
        <div className="text-right">
          <Button
            text={approvalRequiredButton}
            type='Button'
            btnClass='btn-action'
            onClick={this.toggleDialog}
          />
        </div>
      );
    };

    const content = isPayable
    ? <div className="cart-fill__block">
        {descriptionElement}
        <div className="cart-fill__block-inner">
          {methods}
        </div>
      </div>
    : <Alert type="grey" text={unPayableText} />;


    return (
      <div id="payment-method">
        <h2>{title}</h2>
        <Dialog
          closeDialog={this.toggleDialog}
          hasCloseBtn={true}
          title={approvalRequiredText}
          body={getDialogBody()}
          footer={getDialogFooter()}
          open={this.state.isDialogOpen}
        />
        {content}
      </div>
    );
  }
}

export default PaymentMethod;
