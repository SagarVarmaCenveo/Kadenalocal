﻿using Kadena.BusinessLogic.Contracts;
using AutoMapper;
using System.Data;
using Kadena.Models.Search;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Kadena.WebAPI.KenticoProviders.Contracts;
using Kadena.Models.Product;
using System.Web;

namespace Kadena.BusinessLogic.Services
{
    public class SearchService : ISearchService
    {
        private readonly IMapper mapper;
        private readonly IKenticoResourceService resources;
        private readonly IKenticoSearchService kenticoSearch;
        private readonly IKenticoProviderService kenticoProvider;

        public SearchService(IMapper mapper, IKenticoResourceService resources, IKenticoSearchService kenticoSearch, IKenticoProviderService kenticoProvider)
        {
            this.mapper = mapper;
            this.resources = resources;
            this.kenticoSearch = kenticoSearch;
            this.kenticoProvider = kenticoProvider;
        }

        public SearchResultPage Search(string phrase, int results = 100)
        {
            var searchResultPages = SearchPages(phrase, results);
            var searchResultProducts = SearchProducts(phrase, results);

            return new SearchResultPage()
            {
                NoResultMessage = resources.GetResourceString("Kadena.Search.NoResults"),
                Pages = searchResultPages,
                Products = searchResultProducts
            };
        }

        public AutocompleteResponse Autocomplete(string phrase, int results = 3)
        {
            var searchResultPages = SearchPages(phrase, results);
            var searchResultProducts = SearchProducts(phrase, results);
            var serpUrl = kenticoProvider.GetDocumentUrl(resources.GetSettingsKey("KDA_SerpPageUrl"));

            var result = new AutocompleteResponse()
            {
                Pages = new AutocompletePages()
                {
                    Url = $"{serpUrl}?phrase={HttpUtility.UrlEncode(phrase)}&tab=pages",
                    Items = mapper.Map<List<AutocompletePage>>(searchResultPages)
                },
                Products = new AutocompleteProducts()
                {
                    Url = $"{serpUrl}?phrase={HttpUtility.UrlEncode(phrase)}&tab=products",
                    Items = searchResultProducts.Select(p => new AutocompleteProduct()
                    {
                        Id = p.Id,
                        Category = p.Category,
                        Image = p.ImgUrl,
                        Stock = p.Stock,
                        Title = p.Title,
                        Url = kenticoProvider.GetDocumentUrl(p.Id)
                    }
                ).ToList()
                },
                Message = string.Empty
            };

            result.UpdateNotFoundMessage(resources.GetResourceString("Kadena.Search.NoResults"));
            return result;
        }

        public List<ResultItemPage> SearchPages(string phrase, int results)
        {
            var site = resources.GetKenticoSite();
            var searchResultPages = new List<ResultItemPage>();
            var indexName = $"KDA_PagesIndex.{site.Name}";
            var datarowsResults = kenticoSearch.Search(phrase, indexName, "/%", results, true);

            foreach (DataRow dr in datarowsResults)
            {
                int documentId = GetDocumentId(dr[0]);

                var resultItem = new ResultItemPage()
                {
                    Id = documentId,
                    Text = Regex.Replace(dr[5].ToString(), @"<[^>]+>|&nbsp;", "").Trim(),
                    Title = dr[4].ToString(),
                    Url = kenticoProvider.GetDocumentUrl(documentId)
                };

                searchResultPages.Add(resultItem);
            }

            return searchResultPages;
        }

        public List<ResultItemProduct> SearchProducts(string phrase, int results)
        {
            var site = resources.GetKenticoSite();
            var searchResultProducts = new List<ResultItemProduct>();
            var indexName = $"KDA_ProductsIndex.{site.Name}";
            var productsPath = resources.GetSettingsKey("KDA_ProductsPageUrl")?.TrimEnd('/');
            var datarowsResults = kenticoSearch.Search(phrase, indexName, productsPath + "/%", results, true);

            foreach (DataRow dr in datarowsResults)
            {
                int documentId = GetDocumentId(dr[0]);
                var resultItem = new ResultItemProduct()
                {
                    Id = documentId,
                    Title = dr[4].ToString(),
                    Breadcrumbs = kenticoProvider.GetBreadcrumbs(documentId),
                    IsFavourite = false,
                    ImgUrl = kenticoProvider.GetProductTeaserImageUrl(documentId)
                };

                var product = kenticoProvider.GetProductByDocumentId(documentId);
                if (product != null)
                {
                    // fill in SKU image if teaser is empty
                    if (string.IsNullOrEmpty(resultItem.ImgUrl))
                    {
                        resultItem.ImgUrl = product.SkuImageUrl;
                    }
                    resultItem.Category = product.Category;
                    if (product.ProductType.Contains(ProductTypes.InventoryProduct))
                    {
                        resultItem.Stock = new Stock()
                        {
                            Text = string.Format(resources.GetResourceString("Kadena.Product.NumberOfAvailableProducts"), product.StockItems),
                            Type = product.Availability
                        };
                    }
                    resultItem.UseTemplateBtn = new UseTemplateBtn()
                    {
                        Text = resources.GetResourceString("Kadena.Search.GoToDetailButton"),
                        Url = product.DocumentUrl
                    };
                }


                searchResultProducts.Add(resultItem);
            }

            return searchResultProducts;
        }

        private int GetDocumentId(object o)
        {
            int documentId = 0;
            var parsedId = o.ToString().Split(new char[] { ';' })?[0].Split(new char[] { '_' })[0];
            int.TryParse(parsedId, out documentId);
            return documentId;
        }
    }
}
