import React from 'react';
import PropTypes from 'prop-types';
import ReactCSSTransitionGroup from 'react-addons-css-transition-group';
import SVG from 'app.dump/SVG';
import Method from './Method';

const MethodsGroup = (props) => {
  const { id, title, icon, disabled, pricePrefix, price, datePrefix, date, items, checkedId,
    changeShoppingData, openId, changeOpenId } = props;

  const methods = items.map((item) => {
    return <Method changeShoppingData={changeShoppingData} checkedId={checkedId} key={`m-${item.id}`} {...item} />;
  });

  let className = 'input__wrapper select-accordion__item';

  if (openId === id) className += ' isActive';

  if (disabled) className += ' input__wrapper--disabled';

  let dateElement = null;
  if (datePrefix && date) {
    dateElement = <span>{datePrefix} {date}</span>;
  } else if (datePrefix) {
    dateElement = <span>{datePrefix}</span>;
  } else if (date) {
    dateElement = <span>{date}</span>;
  }

  let priceElement = null;
  if (pricePrefix && price) {
    priceElement = <span>{pricePrefix} {price}</span>;
  } else if (pricePrefix) {
    priceElement = <span>{pricePrefix}</span>;
  } else if (price) {
    priceElement = <span>{price}</span>;
  }

  const stick = dateElement
    ? <span> | </span>
    : null;

  const extraInfo = (priceElement || dateElement)
    ? <span>{`(${dateElement}`}{stick}{`${priceElement})`}</span>
    : null;

  return (
    <div>
      <div className={className}>
        <input checked={openId === id}
               disabled={disabled}
               onChange={() => changeOpenId(id)}
               type="radio"
               name="methodGroup"
               className="input__radio"
               id={`mg-${id}`} />
        <label htmlFor={`mg-${id}`} className="input__label input__label--radio select-accordion__main-label">
          <SVG name={icon} className="icon-shipping"/>
          {title}
          <span className="select-accordion__inner-label">
            {extraInfo}
          </span>
        </label>
      </div>

      <ReactCSSTransitionGroup
        transitionName="select-accordion__content--animation"
        component="div"
        transitionEnterTimeout={700}
        transitionLeaveTimeout={700}>
        {
          openId === id
          ? <div className="select-accordion__content">{methods}</div>
          : null
        }
      </ReactCSSTransitionGroup>
    </div>
  );
};

MethodsGroup.propTypes = {
  items: PropTypes.arrayOf(PropTypes.object.isRequired),
  changeShoppingData: PropTypes.func.isRequired,
  changeOpenId: PropTypes.func.isRequired,
  title: PropTypes.string.isRequired,
  icon: PropTypes.string.isRequired,
  id: PropTypes.number.isRequired,
  pricePrefix: PropTypes.string,
  datePrefix: PropTypes.string,
  checkedId: PropTypes.number,
  disabled: PropTypes.bool,
  openId: PropTypes.number,
  price: PropTypes.string,
  opened: PropTypes.bool,
  date: PropTypes.string
};

export default MethodsGroup;
