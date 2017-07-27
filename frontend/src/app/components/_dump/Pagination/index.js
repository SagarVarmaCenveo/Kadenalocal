import React from 'react';
import PropTypes from 'prop-types';
// import ReactPaginate from 'react-paginate';
import ReactPaginate from './PaginationBoxView';

const Pagination = (props) => {
  const { pagesNumber, initialPage, onPageChange, itemsOnPage, itemsNumber, currPage } = props;

  const pagesFrom = (currPage * itemsOnPage) + 1;

  const pagesToByDefault = (currPage + 1) * itemsOnPage;
  const pagesTo = pagesToByDefault > itemsNumber ? itemsNumber : pagesToByDefault;

  return pagesNumber > 1
    ? (
      <div className="row flex-align--center mt-4">
        <div className="col-6">
          <span>Showing {pagesFrom} to {pagesTo} of {itemsNumber} entries</span>
        </div>
        <div className="col-6 text--right">
          <ReactPaginate pageCount={pagesNumber}
                         pageRangeDisplayed={3}
                         marginPagesDisplayed={1}
                         onPageChange={onPageChange}
                         initialPage={initialPage}
                         previousClassName="pagination__item"
                         nextClassName="pagination__item"
                         containerClassName="pagination mb-0 text--right list--unstyled"
                         pageClassName="pagination__item"
                         pageLinkClassName="pagination__page"
                         activeClassName="pagination__page--active" />
        </div>
      </div>
    )
    : null;

};

Pagination.propTypes = {
  pagesNumber: PropTypes.number.isRequired,
  initialPage: PropTypes.number.isRequired,
  onPageChange: PropTypes.func.isRequired,
  itemsNumber: PropTypes.number.isRequired,
  itemsOnPage: PropTypes.number.isRequired,
  currPage: PropTypes.number.isRequired
};

export default Pagination;
