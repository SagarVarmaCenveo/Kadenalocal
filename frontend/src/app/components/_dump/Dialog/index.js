// @flow
import React from 'react';
import PropTypes from 'prop-types';
/* components */
import SVG from 'app.dump/SVG';
/* flow-types */

type Props = {
  closeDialog: () => void,
  hasCloseBtn: ?boolean,
  title: string,
  body: ?{},
  footer: ?{}
};

const Dialog = (props: Props) => {
  const { closeDialog, hasCloseBtn, title, body, footer } = props;
  const closer = hasCloseBtn
    ? <button onClick={closeDialog} type="button" className="btn--off dialog__closer">
      <SVG name="cross--darker" className="icon-modal" />
    </button>
    : null;

  const bodyElement = body
    ? <div className="dialog__content">{body}</div>
    : null;

  const footerElement = footer
    ? <div className="dialog__footer">{footer}</div>
    : null;

  return (
    <div className="dialog active">
      <div onClick={closeDialog} className="dialog__shadow"> </div>
      <div className="dialog__block">
        <div className="dialog__header">
          <p>{title}</p>
          {closer}
        </div>
        {bodyElement}
        {footerElement}
      </div>
    </div>
  );
};

Dialog.propTypes = {
  closeDialog: PropTypes.func.isRequired,
  title: PropTypes.string.isRequired,
  hasCloseBtn: PropTypes.bool,
  footer: PropTypes.object,
  body: PropTypes.object
};

export default Dialog;
