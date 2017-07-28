const { localization } = config; // eslint-disable-line no-undef

const {
  login,
  checkout,
  spotfire,
  userSettings,
  searchPage,
  search,
  orderDetail,
  recentOrders,
  ui,
  cartPreview
} = localization;


export const LOGIN = login;
export const CHECKOUT = checkout;
export const SPOTFIRE = spotfire;
export const USER_SETTINGS = userSettings;
export const ORDER_DETAIL = orderDetail;
export const SEARCH_PAGE = searchPage;
export const SEARCH = search;
export const RECENT_ORDERS = recentOrders;
export const CART_PREVIEW = cartPreview;

/* UI */
export const MODIFY_MAILING_LIST_UI = ui ? ui.modifyMailingList : {};
