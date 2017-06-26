import axios from 'axios';
import { SEARCH_RESULTS_HIDE, SEARCH_RESULTS_SHOW, SEARCH_RESULT_GET_SUCCESS, SEARCH_RESULT_GET_FAILURE,
  SEARCH_RESULT_GET_FETCH, SEARCH_QUERY_CHANGE, HEADER_SHADOW_SHOW, HEADER_SHADOW_HIDE } from '../constants';
import { SEARCH } from '../globals';
import ui from '../testServices/searchUI';

export const closeDropdown = () => {
  return (dispatch) => {
    dispatch({ type: SEARCH_RESULTS_HIDE });
    dispatch({ type: HEADER_SHADOW_HIDE });
  };
};

export const changeSearchQuery = (query) => {
  return (dispatch) => {
    dispatch({
      type: SEARCH_QUERY_CHANGE,
      payload: {
        query
      }
    });
  };
};

export const sendQuery = (query) => {
  return (dispatch) => {
    dispatch({ type: SEARCH_RESULT_GET_FETCH });

    axios({
      method: 'post',
      url: `${SEARCH.queryUrl}?phrase=${encodeURI(query)}`,
      data: {
        query
      }
    }).then((response) => {
      const { payload, success, errorMessage } = response.data;

      if (!success) {
        dispatch({ type: SEARCH_RESULT_GET_FAILURE });
        alert(errorMessage); // eslint-disable-line no-alert
      } else {
        dispatch({
          type: SEARCH_RESULT_GET_SUCCESS,
          payload
        });
        dispatch({ type: HEADER_SHADOW_SHOW });
        dispatch({ type: SEARCH_RESULTS_SHOW });
      }
    })
      .catch((error) => {
        dispatch({ type: SEARCH_RESULT_GET_FAILURE });
        alert(error.message); // eslint-disable-line no-alert
      });

    // setTimeout(() => {
    //   dispatch({
    //     type: SEARCH_RESULT_GET_SUCCESS,
    //     payload: ui
    //   });
    //
    //   dispatch({ type: HEADER_SHADOW_SHOW });
    //   dispatch({ type: SEARCH_RESULTS_SHOW });
    // }, 200);
  };
};