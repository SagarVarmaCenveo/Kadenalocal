import axios from 'axios';
import validator from 'validator';
import * as constants from '../constants';
import { LOGIN } from '../globals';

export default function requestLogin(loginEmail, password, isKeepMeLoggedIn) {
  return (dispatch) => {

    if (!validator.isEmail(loginEmail)) {
      dispatch({
        type: constants.LOGIN_CLIENT_VALIDATION_ERROR,
        data: {
          isLoading: false,
          response: {
            success: false,
            errorMessage: LOGIN.emailValidationMessage,
            errorPropertyName: 'loginEmail'
          }
        }
      });

      return;
    }

    if (password.length === 0) {
      dispatch({
        type: constants.LOGIN_CLIENT_VALIDATION_ERROR,
        data: {
          isLoading: false,
          response: {
            success: false,
            errorMessage: LOGIN.passwordValidationMessage,
            errorPropertyName: 'password'
          }
        }
      });

      return;
    }

    dispatch({
      type: constants.FETCH_SERVERS,
      isLoading: true
    });

    // ToDo: Change to POST and URL
      axios.post('/KadenaWebService.asmx/LogonUser', { loginEmail, password, isKeepMeLoggedIn })
      .then((response) => {
        dispatch({
          type: constants.FETCH_SERVERS_SUCCESS
        });

        dispatch({
          type: constants.LOGIN_RESPONSE_SUCCESS,
          data: response.data
        });
      })
      .catch(() => {
        dispatch({
          type: constants.FETCH_SERVERS_FAILURE
        });
      });
  };
}
