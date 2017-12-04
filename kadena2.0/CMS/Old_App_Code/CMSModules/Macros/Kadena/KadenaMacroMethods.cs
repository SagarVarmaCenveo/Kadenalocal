﻿using Kadena.Old_App_Code.CMSModules.Macros.Kadena;
using CMS.Helpers;
using CMS.MacroEngine;
using System;
using System.Linq;
using CMS.DocumentEngine;
using CMS.Membership;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.DataEngine;
using CMS.CustomTables;
using System.Collections.Generic;
using Kadena.Models;
using Kadena.Old_App_Code.Kadena.Forms;
using Kadena.WebAPI.KenticoProviders;
using Kadena.Models.Product;
using AutoMapper;
using Kadena.WebAPI;
using AutoMapper;
using CMS.EventLog;
using CMS.DocumentEngine.Types.KDA;
using CMS.CustomTables.Types.KDA;
using static Kadena.Helpers.SerializerConfig;
using Kadena.BusinessLogic.Services;

[assembly: CMS.RegisterExtension(typeof(Kadena.Old_App_Code.CMSModules.Macros.Kadena.KadenaMacroMethods), typeof(KadenaMacroNamespace))]
namespace Kadena.Old_App_Code.CMSModules.Macros.Kadena
{
    public class KadenaMacroMethods : MacroMethodContainer
    {
        static KadenaMacroMethods()
        {
            MapperBuilder.InitializeAll();
        }

        [MacroMethod(typeof(bool), "Checks whether sku weight is required for given combination of product types", 1)]
        [MacroMethodParam(0, "productTypes", typeof(string), "Product types piped string")]
        public static object IsSKUWeightRequired(EvaluationContext context, params object[] parameters)
        {
            if (parameters.Length != 1)
            {
                throw new NotSupportedException();
            }
            
            var productTypes = ValidationHelper.GetString(parameters[0], "");
            var product = new Product { ProductType = productTypes };
            var isWeightRequired = new ProductValidator().IsSKUWeightRequired(product);
            return isWeightRequired;
        }

        [MacroMethod(typeof(bool), "Validates product type and sku weight", 1)]
        [MacroMethodParam(0, "productTypes", typeof(string), "Product types piped string")]
        [MacroMethodParam(1, "skuWeight", typeof(double), "SKU weight")]
        public static object IsSKUWeightValid(EvaluationContext context, params object[] parameters)
        {
            if (parameters.Length != 2)
            {
                throw new NotSupportedException();
            }

            var productTypes = ValidationHelper.GetString(parameters[0], "");
            var skuWeight = ValidationHelper.GetDouble(parameters[1], 0, LocalizationContext.CurrentCulture.CultureCode);
            var product = new Product { Weight = skuWeight, ProductType = productTypes };

            var isValid = new ProductValidator().ValidateWeight(product);
            return isValid;
        }

        [MacroMethod(typeof(bool), "Validates combination of product types - static type variant.", 1)]
        [MacroMethodParam(0, "productTypes", typeof(string), "Product types piped string")]
        public static object IsStaticProductTypeCombinationValid(EvaluationContext context, params object[] parameters)
        {
            if (parameters.Length != 1)
            {
                throw new NotSupportedException();
            }
            var selectedProductTypeCodeNames = ValidationHelper.GetString(parameters[0], "").Split("|".ToCharArray());
            // Static product - can be of type Inventory or can be print on demand (POD) or can be withh add-on
            if (selectedProductTypeCodeNames.Contains(ProductTypes.StaticProduct))
            {
                if (selectedProductTypeCodeNames.Contains(ProductTypes.MailingProduct) ||
                    selectedProductTypeCodeNames.Contains(ProductTypes.TemplatedProduct))
                {
                    return false;
                }
            }
            return true;
        }

        [MacroMethod(typeof(bool), "Validates combination of product types - inventory type variant.", 1)]
        [MacroMethodParam(0, "productTypes", typeof(string), "Product types piped string")]
        public static object IsInventoryProductTypeCombinationValid(EvaluationContext context, params object[] parameters)
        {
            if (parameters.Length != 1)
            {
                throw new NotSupportedException();
            }
            var selectedProductTypeCodeNames = ValidationHelper.GetString(parameters[0], "").Split("|".ToCharArray());
            // Inventory product - Must be of type static
            if (selectedProductTypeCodeNames.Contains(ProductTypes.InventoryProduct))
            {
                if (selectedProductTypeCodeNames.Contains(ProductTypes.POD) ||
                    selectedProductTypeCodeNames.Contains(ProductTypes.MailingProduct) ||
                    selectedProductTypeCodeNames.Contains(ProductTypes.TemplatedProduct))
                {
                    return false;
                }
            }
            return true;
        }

        [MacroMethod(typeof(bool), "Validates combination of product types - mailing type variant.", 1)]
        [MacroMethodParam(0, "productTypes", typeof(string), "Product types piped string")]
        public static object IsMailingProductTypeCombinationValid(EvaluationContext context, params object[] parameters)
        {
            if (parameters.Length != 1)
            {
                throw new NotSupportedException();
            }
            var selectedProductTypeCodeNames = ValidationHelper.GetString(parameters[0], "").Split("|".ToCharArray());
            // Mailing product - Must be of type Template
            if (selectedProductTypeCodeNames.Contains(ProductTypes.MailingProduct))
            {
                if (!selectedProductTypeCodeNames.Contains(ProductTypes.TemplatedProduct) ||
                    selectedProductTypeCodeNames.Contains(ProductTypes.StaticProduct) ||
                    selectedProductTypeCodeNames.Contains(ProductTypes.InventoryProduct) ||
                    selectedProductTypeCodeNames.Contains(ProductTypes.POD))
                {
                    return false;
                }
            }
            return true;
        }

        [MacroMethod(typeof(bool), "Validates combination of product types - mailing type variant.", 1)]
        [MacroMethodParam(0, "productTypes", typeof(string), "Product types piped string")]
        public static object IsTemplatedProductTypeCombinationValid(EvaluationContext context, params object[] parameters)
        {
            if (parameters.Length != 1)
            {
                throw new NotSupportedException();
            }
            var selectedProductTypeCodeNames = ValidationHelper.GetString(parameters[0], "").Split("|".ToCharArray());
            // Templated product - Can be of type Mailing
            if (selectedProductTypeCodeNames.Contains(ProductTypes.TemplatedProduct))
            {
                if (selectedProductTypeCodeNames.Contains(ProductTypes.StaticProduct) ||
                    selectedProductTypeCodeNames.Contains(ProductTypes.InventoryProduct) ||
                    selectedProductTypeCodeNames.Contains(ProductTypes.POD))
                {
                    return false;
                }
            }
            return true;
        }

        [MacroMethod(typeof(string), "Validates whether product is an invertory product type.", 1)]
        [MacroMethodParam(0, "productType", typeof(string), "Current product type")]
        [MacroMethodParam(1, "numberOfAvailableProducts", typeof(object), "NumberOfAvailableProducts")]
        [MacroMethodParam(2, "cultureCode", typeof(string), "Current culture code")]
        [MacroMethodParam(3, "numberOfAvailableProductsHelper", typeof(object), "NumberOfAvailableProducts of ECommerce")]
        public static object GetAvailableProductsString(EvaluationContext context, params object[] parameters)
        {
            if (parameters.Length != 4)
            {
                throw new NotSupportedException();
            }

            if (!ValidationHelper.GetString(parameters[0], "").Contains(ProductTypes.InventoryProduct))
            {
                return string.Empty;
            }
            else
            {
                string formattedValue = string.Empty;

                if (parameters[1] == null)
                {
                    formattedValue = ResHelper.GetString("Kadena.Product.Unavailable", ValidationHelper.GetString(parameters[2], ""));
                }
                else if ((int)parameters[3] == 0)
                {
                    formattedValue = ResHelper.GetString("Kadena.Product.OutOfStock", ValidationHelper.GetString(parameters[2], ""));
                }
                else
                {
                    formattedValue = string.Format(
                    ResHelper.GetString("Kadena.Product.NumberOfAvailableProducts", ValidationHelper.GetString(parameters[3], "")),
                    ValidationHelper.GetString(parameters[3], ""));
                }

                return formattedValue;
            }
        }

        [MacroMethod(typeof(string), "Gets appropriate css class for label that holds amount of products in stock", 1)]
        [MacroMethodParam(0, "numberOfAvailableProducts", typeof(object), "NumberOfAvailableProducts")]
        [MacroMethodParam(1, "productType", typeof(string), "Current product type")]
        [MacroMethodParam(2, "numberOfAvailableProductsHelper", typeof(object), "NumberOfAvailableProducts of ECommerce")]
        public static object GetAppropriateCssClassOfAvailability(EvaluationContext context, params object[] parameters)
        {
            if (parameters.Length != 3)
            {
                throw new NotSupportedException();
            }

            if (ValidationHelper.GetString(parameters[1], "").Contains(ProductTypes.InventoryProduct))
            {
                if (parameters[0] == null)
                {
                    return "stock stock--unavailable";
                }

                if ((int)parameters[2] == 0)
                {
                    return "stock stock--out";
                }

                return "stock stock--available";

            }

            return string.Empty;
        }

        [MacroMethod(typeof(string), "Returns html (set) of products, that could be in a kit with particular product (for particular user).", 1)]
        [MacroMethodParam(0, "nodeID", typeof(int), "ID of Node, that represents the base product")]
        [MacroMethodParam(1, "productsAliasPath", typeof(string), "Alias path for products")]
        public static object GetKitProductsHtml(EvaluationContext context, params object[] parameters)
        {
            var result = string.Empty;
            var selectedItemTemplate = "<div class=\"input__wrapper input__wrapper--disabled\"><input id=\"dom-0\" type=\"checkbox\" class=\"input__checkbox\" data-id=\"{1}\" checked disabled><label for=\"dom-0\" class=\"input__label input__label--checkbox\">{0}</label></div>";
            var itemTemplate = "<div class=\"input__wrapper\"><input id=\"dom-{0}\" type=\"checkbox\" class=\"input__checkbox\" data-id=\"{2}\"><label for=\"dom-{0}\" class=\"input__label input__label--checkbox\">{1}</label></div>";

            if (parameters.Length != 2)
            {
                throw new NotSupportedException();
            }
            var originalNodeID = ValidationHelper.GetInteger(parameters[0], 0);
            var productsPath = ValidationHelper.GetString(parameters[1], string.Empty);

            var tree = new TreeProvider(MembershipContext.AuthenticatedUser);

            if (originalNodeID > 0)
            {
                var originalDocument = tree.SelectSingleNode(originalNodeID, LocalizationContext.CurrentCulture.CultureCode);
                result += string.Format(selectedItemTemplate, originalDocument.DocumentName, originalDocument.NodeID);
            }

            var wantedTypes = new[] { ProductTypes.InventoryProduct, ProductTypes.StaticProduct, "KDA.POD" };

            var allKitDocuments = tree.SelectNodes()
                .OnCurrentSite()
                .Path(productsPath, PathTypeEnum.Children)
                .Culture(LocalizationContext.CurrentCulture.CultureCode)
                .Types("KDA.Product")
                .CheckPermissions();

            var kitDocuments = allKitDocuments.Where(x => IsProductType(x, wantedTypes));

            if (kitDocuments != null)
            {
                var kitList = kitDocuments.ToList();
                for (int i = 1; i <= kitList.Count; i++)
                {
                    var node = kitList[i - 1];
                    if (node.NodeID != originalNodeID && !node.IsLink)
                    {
                        result += string.Format(itemTemplate, i, node.DocumentName, node.NodeID);
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// Checks if TreeNode's value "ProductType" contains any of given type strings
        /// </summary>
        private static bool IsProductType(TreeNode tn, IEnumerable<string> types)
        {
            var nodeType = tn.GetStringValue("ProductType", string.Empty);
            return types.Any(t => nodeType.Contains(t));
        }

        [MacroMethod(typeof(string), "Returns where codition for one of main navigation repeaters based on enabled modules for customer.", 1)]
        [MacroMethodParam(0, "forEnabledItems", typeof(bool), "For enabled items")]
        public static object GetMainNavigationWhereCondition(EvaluationContext context, params object[] parameters)
        {
            if (parameters.Length != 1)
            {
                throw new NotSupportedException();
            }
            var isForEnabledItems = ValidationHelper.GetBoolean(parameters[0], false);

            return CacheHelper.Cache(cs => GetMainNavigationWhereConditionInternal(isForEnabledItems), new CacheSettings(20, "Kadena.MacroMethods.GetMainNavigationWhereCondition_" + SiteContext.CurrentSiteName + "|" + isForEnabledItems));
        }

        [MacroMethod(typeof(string[]), "Returns array of parsed urls items.", 1)]
        [MacroMethodParam(0, "fieldValue", typeof(string), "Value stored MediaMultiField field")]
        public static object GetUrlsFromMediaMultiField(EvaluationContext context, params object[] parameters)
        {
            if (parameters.Length != 1)
            {
                throw new NotSupportedException();
            }
            var fieldValue = parameters[0] as string;
            var urls = MediaMultiField.GetValues(fieldValue);
            return urls;
        }

        [MacroMethod(typeof(string), "Returns file name from media attachment url.", 1)]
        [MacroMethodParam(0, "url", typeof(string), "Url")]
        public static object GetFilenameFromMediaUrl(EvaluationContext context, params object[] parameters)
        {
            if (parameters.Length != 1)
            {
                throw new NotSupportedException();
            }
            var url = parameters[0] as string;
            var filename = MediaMultiField.ParseFrom(url).Name;
            return filename;
        }

        [MacroMethod(typeof(string), "Returns localized url of the document for current culture.", 1)]
        [MacroMethodParam(0, "aliasPath", typeof(string), "Alias path of the document.")]
        public static object GetLocalizedDocumentUrl(EvaluationContext context, params object[] parameters)
        {
            var aliasPath = ValidationHelper.GetString(parameters[0], string.Empty);
            if (!string.IsNullOrWhiteSpace(aliasPath))
            {
                var documents = new KenticoDocumentProvider(new KenticoResourceService(), new KenticoLogger(), Mapper.Instance);
                return documents.GetDocumentUrl(aliasPath);
            }
            return string.Empty;
        }

        [MacroMethod(typeof(string), "Returns localized urls for language selector.", 1)]
        [MacroMethodParam(0, "aliasPath", typeof(string), "Alias path of the document.")]
        public static object GetUrlsForLanguageSelector(EvaluationContext context, params object[] parameters)
        {
            var aliasPath = ValidationHelper.GetString(parameters[0], string.Empty);
            if (!string.IsNullOrWhiteSpace(aliasPath))
            {
                var logger = new KenticoLogger();
                var documents = new KenticoDocumentProvider(new KenticoResourceService(), logger, Mapper.Instance);
                var kenticoService = new KenticoProviderService(new KenticoResourceService(), logger, documents, Mapper.Instance);
                return Newtonsoft.Json.JsonConvert.SerializeObject(kenticoService.GetUrlsForLanguageSelector(aliasPath), CamelCaseSerializer);
            }
            return string.Empty;
        }

        [MacroMethod(typeof(string), "Returns unified date string in Kadena format", 1)]
        [MacroMethodParam(0, "datetime", typeof(DateTime), "DateTime to format")]
        public static object FormatDate(EvaluationContext context, params object[] parameters)
        {
            var datetime = ValidationHelper.GetDateTime(parameters[0], DateTime.MinValue);
            return new DateTimeFormatter(new KenticoResourceService()).Format(datetime);
        }

        [MacroMethod(typeof(string), "Returns unified date format string", 0)]
        public static object GetDateFormatString(EvaluationContext context, params object[] parameters)
        {
            return new DateTimeFormatter(new KenticoResourceService()).GetFormatString();
        }

        private static string GetMainNavigationWhereConditionInternal(bool isForEnabledItems)
        {
            var result = string.Empty;
            var pageTypes = new List<string>();

            var moduleSettingsMappingsDataClassInfo = DataClassInfoProvider.GetDataClassInfo("KDA.KadenaModuleAndPageTypeConnection");
            if (moduleSettingsMappingsDataClassInfo != null)
            {
                var mappingItems = CustomTableItemProvider.GetItems("KDA.KadenaModuleAndPageTypeConnection");

                if (mappingItems != null)
                {
                    foreach (var mappingItem in mappingItems)
                    {
                        var moduleState = SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSiteName}.{mappingItem.GetStringValue("SettingsKeyCodeName", string.Empty)}");
                        if ((isForEnabledItems && moduleState.ToLowerInvariant().Equals(KadenaModuleState.enabled.ToString())) || (!isForEnabledItems && moduleState.ToLowerInvariant().Equals(KadenaModuleState.disabled.ToString())))
                        {
                            pageTypes.Add(mappingItem.GetStringValue("PageTypeCodeName", string.Empty));
                        }
                    }
                }
            }
            foreach (var pageType in pageTypes)
            {
                result += $"ClassName = N'{pageType}' OR ";
            }
            if (result.Length > 0)
            {
                result = result.Substring(0, result.Length - 3);
            }
            else
            {
                result = "1 = 0";
            }
            return result;
        }

        #region TWE macro methods
        /// <summary>
        /// Returns Division name based on Division ID
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [MacroMethod(typeof(string), "Returns Division name based on Division ID", 1)]
        [MacroMethodParam(0, "DivisionID", typeof(int), "DivisionID")]
        public static object GetDivisionName(EvaluationContext context, params object[] parameters)
        {
            try
            {
                int divisionID = ValidationHelper.GetInteger(parameters[0], 0);
                DivisionItem division = CustomTableItemProvider.GetItem<DivisionItem>(divisionID);
                string divisionName = division?.DivisionName ?? string.Empty;
                return divisionName;
            }
            catch (Exception ex)
            {
                EventLogProvider.LogInformation("Kadena Macro methods", "GetDivisionName", ex.Message);
                return string.Empty;
            }
        }
        /// <summary>
        /// Returns Program name based on Program ID
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [MacroMethod(typeof(string), "Returns Program name based on Program ID", 1)]
        [MacroMethodParam(0, "ProgramID", typeof(int), "ProgramID")]
        public static object GetProgramName(EvaluationContext context, params object[] parameters)
        {
            try
            {
                int programID = ValidationHelper.GetInteger(parameters[0], 0);
                string programName = string.Empty;
                Program program = ProgramProvider.GetPrograms().WhereEquals("NodeSiteID", SiteContext.CurrentSite.SiteID).WhereEquals("ProgramID", programID).Columns("ProgramName").FirstObject;
                programName = program?.ProgramName ?? string.Empty;
                return programName;
            }
            catch (Exception ex)
            {
                EventLogProvider.LogInformation("Kadena Macro methods", "GetProgramName", ex.Message);
                return string.Empty;
            }
        }
        /// <summary>
        /// Returns Category name based on Category ID
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [MacroMethod(typeof(string), "Returns Category name based on Category ID", 1)]
        [MacroMethodParam(0, "CategoryID", typeof(int), "CategoryID")]
        public static object GetCategoryName(EvaluationContext context, params object[] parameters)
        {
            try
            {
                int categoryID = ValidationHelper.GetInteger(parameters[0], 0);
                string categoryName = string.Empty;
                ProductCategory category = ProductCategoryProvider.GetProductCategories().WhereEquals("NodeSiteID", SiteContext.CurrentSite.SiteID).WhereEquals("ProductCategoryID", categoryID).Columns("ProductCategoryTitle").FirstObject;
                categoryName = category?.ProductCategoryTitle ?? string.Empty;
                return categoryName;
            }
            catch (Exception ex)
            {
                EventLogProvider.LogInformation("Kadena Macro methods", "GetProgramName", ex.Message);
                return string.Empty;
            }
        }
        /// <summary>
        /// Returns Currently opened campaign name
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [MacroMethod(typeof(string), "Returns Currently opened campaign name", 1)]
        public static object GetCampaignName(EvaluationContext context, params object[] parameters)
        {
            try
            {
                string campaignName = string.Empty;
                var campaign = CampaignProvider.GetCampaigns().Columns("Name").WhereEquals("OpenCampaign", true).WhereEquals("NodeSiteID", SiteContext.CurrentSite.SiteID).FirstOrDefault();
                if (campaign != null)
                {
                    campaignName = ValidationHelper.GetString(campaign.GetValue("Name"), string.Empty);
                }
                return campaignName;
            }
            catch (Exception ex)
            {
                EventLogProvider.LogInformation("Kadena Macro methods", "BindPrograms", ex.Message);
                return string.Empty;
            }
        }
        #endregion
    }
}