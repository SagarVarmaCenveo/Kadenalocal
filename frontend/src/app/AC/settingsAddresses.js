import axios from 'axios';
import { toastr } from 'react-redux-toastr';
/* constants */
import { FETCH, SUCCESS, FAILURE, SHOW, HIDE, START, FINISH, INIT_UI, SETTINGS_ADDRESSES, ADD_SHIPPING_ADDRESS,
  MODIFY_SHIPPING_ADDRESS, APP_LOADING, DIALOG, isDevelopment } from 'app.consts';
/* helpers */
import { callAC } from 'app.helpers/ac';
/* globals */
import { USER_SETTINGS, NOTIFICATION } from 'app.globals';

export const getUI = () => {
  return (dispatch) => {
    dispatch({ type: SETTINGS_ADDRESSES + INIT_UI + FETCH });

    axios({
      method: 'get',
      url: USER_SETTINGS.addresses.initUIURL
    }).then((response) => {
      const { payload, success, errorMessage } = response.data;

      if (!success) {
        dispatch({
          type: SETTINGS_ADDRESSES + INIT_UI + FAILURE,
          alert: errorMessage
        });
      } else {
        dispatch({
          type: SETTINGS_ADDRESSES + INIT_UI + SUCCESS,
          payload: {
            ui: payload
          }
        });
      }
    })
      .catch((error) => {
        dispatch({ type: SETTINGS_ADDRESSES + INIT_UI + FAILURE });
      });
  };
};

export const modifyAddress = (data) => {
  return (dispatch) => {
    dispatch({ type: MODIFY_SHIPPING_ADDRESS + FETCH });
    dispatch({ type: APP_LOADING + START });

    axios({
      method: 'post',
      url: USER_SETTINGS.addresses.editAddressURL,
      headers: { 'Content-Type': 'application/json' },
      data
    }).then((response) => {
      const { success, errorMessage, payload } = response.data;

      if (!success) {
        dispatch({
          type: MODIFY_SHIPPING_ADDRESS + FAILURE,
          alert: errorMessage
        });
        dispatch({ type: APP_LOADING + FINISH });
        return;
      }

      dispatch({
        type: MODIFY_SHIPPING_ADDRESS + SUCCESS,
        payload: data
      });

      dispatch({ type: APP_LOADING + FINISH });
      toastr.success(NOTIFICATION.modifyAddress.title, NOTIFICATION.modifyAddress.text);
    }).catch((error) => {
      dispatch({ type: MODIFY_SHIPPING_ADDRESS + FAILURE });
      dispatch({ type: APP_LOADING + FINISH });
    });

  };
};


export const addAddress = (data) => {
  return (dispatch) => {
    dispatch({ type: ADD_SHIPPING_ADDRESS + FETCH });
    dispatch({ type: APP_LOADING + START });

    const prod = () => {
      axios({
        method: 'post',
        url: USER_SETTINGS.addresses.editAddressURL,
        data
      }).then((response) => {
        const { success, errorMessage, payload } = response.data;

        if (!success) {
          dispatch({
            type: ADD_SHIPPING_ADDRESS + FAILURE,
            alert: errorMessage
          });
          dispatch({ type: APP_LOADING + FINISH });
          return;
        }

        const { id } = payload;
        data.id = id;

        dispatch({
          type: ADD_SHIPPING_ADDRESS + SUCCESS,
          payload: data
        });

        dispatch({ type: APP_LOADING + FINISH });
        toastr.success(NOTIFICATION.addAddress.title, NOTIFICATION.addAddress.text);
      })
        .catch((error) => {
          dispatch({ type: ADD_SHIPPING_ADDRESS + FAILURE });
          dispatch({ type: APP_LOADING + FINISH });
        });
    };

    const dev = () => {
      setTimeout(() => {
        data.id = Date.now();

        dispatch({
          type: ADD_SHIPPING_ADDRESS + SUCCESS,
          payload: data
        });
        dispatch({ type: APP_LOADING + FINISH });
        toastr.success(NOTIFICATION.addAddress.title, NOTIFICATION.addAddress.text);
      }, 2000);
    };

    callAC(dev, prod);
  };
};
