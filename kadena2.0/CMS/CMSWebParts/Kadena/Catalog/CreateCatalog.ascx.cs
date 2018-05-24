using CMS.CustomTables;
using CMS.CustomTables.Types.KDA;
using CMS.DataEngine;
using CMS.DocumentEngine.Types.KDA;
using CMS.Ecommerce;
using CMS.EventLog;
using CMS.Helpers;
using CMS.MediaLibrary;
using CMS.PortalEngine.Web.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Text;
using Kadena.Models.CustomCatalog;
using Kadena.Old_App_Code.Kadena.PDFHelpers;

public partial class CMSWebParts_Kadena_Catalog_CreateCatalog : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Select all text
    /// </summary>
    public string SelectAllText
    {
        get
        {
            return ValidationHelper.GetString(ResHelper.GetString("Kadena.Catalog.SelectAllText"), string.Empty);
        }
        set
        {
            SetValue("SelectAllText", value);
        }
    }

    /// <summary>
    /// Select all text
    /// </summary>
    public string NoProductSelected
    {
        get
        {
            return ValidationHelper.GetString(ResHelper.GetString("Kadena.Catalog.NoProductSelected"), string.Empty);
        }
        set
        {
            SetValue("NoProductSelected", value);
        }
    }

    /// <summary>
    /// Select all text
    /// </summary>
    public string NoDataFoundText
    {
        get
        {
            return ValidationHelper.GetString(ResHelper.GetString("Kadena.Catalog.NoDataFoundText"), string.Empty);
        }
        set
        {
            SetValue("NoDataFoundText", value);
        }
    }

    /// <summary>
    /// Select all text
    /// </summary>
    public string NoCampaignOpen
    {
        get
        {
            return ValidationHelper.GetString(ResHelper.GetString("Kadena.Catalog.NoCampaignOpen"), string.Empty);
        }
        set
        {
            SetValue("NoCampaignOpen", value);
        }
    }

    /// <summary>
    /// Gets or sets the value of product type
    /// </summary>
    public int TypeOfProduct
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ProductType"), 0);
        }
        set
        {
            SetValue("ProductType", value);
        }
    }

    /// <summary>
    /// Gets or sets the value of Program filter
    /// </summary>
    public bool ShowProgramFilter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowProgramFilter"), true);
        }
        set
        {
            SetValue("ShowProgramFilter", value);
        }
    }

    /// <summary>
    /// Gets or sets the value of Brand
    /// </summary>
    public bool ShowBrandFilter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowBrandFilter"), true);
        }
        set
        {
            SetValue("ShowBrandFilter", value);
        }
    }

    /// <summary>
    /// Gets or sets the value of Product category
    /// </summary>
    public bool ShowProductCategoryFilter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowProductCategoryFilter"), true);
        }
        set
        {
            SetValue("ShowProductCategoryFilter", value);
        }
    }

    /// <summary>
    /// Gets or sets the value of POS
    /// </summary>
    public bool ShowPosFilter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowPosFilter"), true);
        }
        set
        {
            SetValue("ShowPosFilter", value);
        }
    }

    /// <summary>
    /// Get current open campaign
    /// </summary>
    public Campaign OpenCampaign
    {
        get
        {
            return CampaignProvider.GetCampaigns().Columns("CampaignID,Name,StartDate,EndDate")
                                .WhereEquals("OpenCampaign", true)
                                .Where(new WhereCondition().WhereEquals("CloseCampaign", false).Or()
                                .WhereEquals("CloseCampaign", null))
                                .WhereEquals("NodeSiteID", CurrentSite.SiteID).FirstOrDefault();
        }
        set
        {
            SetValue("OpenCampaign", value);
        }
    }

    /// <summary>
    /// Search placeholder text
    /// </summary>
    public string PosSearchPlaceholder
    {
        get
        {
            return ValidationHelper.GetString(ResHelper.GetString("Kadena.Catalog.PosSearchPlaceholderText"), string.Empty);
        }
        set
        {
            SetValue("PosSearchPlaceholder", value);
        }
    }

    /// <summary>
    /// POS number text
    /// </summary>
    public string POSNumberText
    {
        get
        {
            return ValidationHelper.GetString(ResHelper.GetString("Kadena.Catalog.POSNumberText"), string.Empty);
        }
        set
        {
            SetValue("POSNumberText", value);
        }
    }

    #endregion "Properties"

    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }

    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (this.StopProcessing)
        {
            this.Visible = false;
        }
        else
        {
            catalogControls.Visible = true;
            lblNoProducts.Visible = false;
            ddlBrands.Visible = ShowBrandFilter;
            ddlProductTypes.Visible = ShowProductCategoryFilter;
            ddlPrograms.Visible = ShowProgramFilter;
            if (ShowPosFilter)
            {
                searchDiv.Visible = true;
                posNumber.Visible = ShowPosFilter;
            }
            else
            {
                searchDiv.Visible = false;
                posNumber.Visible = ShowPosFilter;
            }
            if (!IsPostBack)
            {
                posNumber.Attributes.Add("placeholder", PosSearchPlaceholder);
                Bindproducts();
            }
        }
    }

    /// <summary>
    /// Binding products on page load
    /// </summary>
    private void Bindproducts()
    {
        List<CampaignsProduct> products = null;
        if (TypeOfProduct == (int)ProductsType.PreBuy && OpenCampaign != null)
        {
            BindBrands();
            BindProductTypes();
            if (OpenCampaign != null)
            {
                products = CampaignsProductProvider.GetCampaignsProducts()
                           .WhereIn("ProgramID", GetProgramIDs(OpenCampaign.CampaignID)).WhereEquals("NodeSiteID", CurrentSite.SiteID).ToList();
            }
            BindingProductsToRepeater(products);
        }
        else if (TypeOfProduct == (int)ProductsType.PreBuy && OpenCampaign == null)
        {
            catalogControls.Visible = false;
            lblNoProducts.Visible = false;
            campaignIsNotOpen.Visible = true;
        }
        if (TypeOfProduct == (int)ProductsType.GeneralInventory)
        {
            BindBrands();
            BindProductTypes();
            products = CampaignsProductProvider.GetCampaignsProducts().WhereEquals("NodeSiteID", CurrentSite.SiteID)
                .Where(new WhereCondition().WhereEquals("ProgramID", null).Or().WhereEquals("ProgramID", 0)).ToList();
            BindingProductsToRepeater(products);
        }
    }

    /// <summary>
    /// Getting programs based on open campaign
    /// </summary>
    /// <param name="campaignID"></param>
    /// <returns></returns>
    public List<int> GetProgramIDs(int campaignID)
    {
        try
        {
            List<int> programIds = new List<int>();
            var programs = ProgramProvider.GetPrograms()
                                    .WhereEquals("CampaignID", OpenCampaign.CampaignID)
                                    .Columns("ProgramID")
                                    .ToList();
            if (!DataHelper.DataSourceIsEmpty(programs))
            {
                foreach (var program in programs)
                {
                    programIds.Add(program.ProgramID);
                }
            }
            return programIds;
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("get program based on open campaign", ex.Message, ex);
            return default(List<int>);
        }
    }

    /// <summary>
    /// Binding product types
    /// </summary>
    private void BindProductTypes()
    {
        try
        {
            var productCategories = ProductCategoryProvider.GetProductCategories()
                                        .Columns("ProductCategoryTitle,ProductCategoryID")
                                        .WhereEquals("NodeSiteID", CurrentSite.SiteID)
                                        .Select(x => new ProductCategory { ProductCategoryID = x.ProductCategoryID, ProductCategoryTitle = x.ProductCategoryTitle })
                                        .ToList()
                                        .OrderBy(y => y.ProductCategoryTitle);
            ddlProductTypes.DataSource = productCategories;
            ddlProductTypes.DataTextField = "ProductCategoryTitle";
            ddlProductTypes.DataValueField = "ProductCategoryID";
            ddlProductTypes.DataBind();
            ddlProductTypes.Items.Insert(0, new ListItem(ResHelper.GetString("Kadena.Catalog.SelectProductTypeText"), "0"));
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Binding producttypes based on program", ex.Message, ex);
        }
    }

    /// <summary>
    /// Binding programs based on campaign
    /// </summary>
    public void BindPrograms()
    {
        try
        {
            if ((OpenCampaign?.CampaignID ?? default(int)) != default(int))
            {
                var programs = ProgramProvider.GetPrograms()
                                    .WhereEquals("NodeSiteID", CurrentSite.SiteID)
                                    .WhereEquals("CampaignID", ValidationHelper.GetInteger(OpenCampaign.GetValue("CampaignID"), default(int)))
                                    .Columns("ProgramName,ProgramID").Select(x => new Program { ProgramID = x.ProgramID, ProgramName = x.ProgramName })
                                    .ToList()
                                    .OrderBy(y => y.ProgramName);
                if (programs != null)
                {
                    ddlPrograms.DataSource = programs;
                    ddlPrograms.DataTextField = "ProgramName";
                    ddlPrograms.DataValueField = "ProgramID";
                    string selectText = ValidationHelper.GetString(ResHelper.GetString("Kadena.Catalog.SelectProgramText"), string.Empty);
                    ddlPrograms.DataBind();
                }
            }
            ddlPrograms.Items.Insert(0, new ListItem(ResHelper.GetString("Kadena.Catalog.SelectProgramText"), "0"));
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("CMSWebParts_Kadena_CustomCatalogFilter BindPrograms", ex.Message, ex);
        }
    }

    /// <summary>
    /// Binding brands based on program
    /// </summary>
    /// <param name="programID"></param>
    private void BindBrands(String programID = "0")
    {
        try
        {
            {
                var brand = CustomTableItemProvider.GetItems(BrandItem.CLASS_NAME)
                                .Columns("BrandName,ItemID")
                                .Select(x => new BrandItem { ItemID = x.Field<int>("ItemID"), BrandName = x.Field<string>("BrandName") })
                                .OrderBy(x => x.BrandName)
                                .ToList();
                ddlBrands.DataSource = brand;
                ddlBrands.DataTextField = "BrandName";
                ddlBrands.DataValueField = "ItemID";
                ddlBrands.DataBind();
                ddlBrands.Items.Insert(0, new ListItem(ResHelper.GetString("Kadena.Catalog.SelectBrandText"), "0"));
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Binding brands based on program", ex.Message, ex);
        }
    }

    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }

    /// <summary>
    /// Filtering products based on filters
    /// </summary>
    public void SetFilter(int programID = default(int), int categoryID = default(int), int brandID = default(int), string posNum = null)
    {
        try
        {
            rptCatalogProducts.DataSource = null;
            rptCatalogProducts.DataBind();
            lblNoProducts.Visible = false;
            noProductSelected.Visible = false;
            if (TypeOfProduct == (int)ProductsType.PreBuy && OpenCampaign != null)
            {
                var products = CampaignsProductProvider.GetCampaignsProducts().WhereNotEquals("ProgramID", null).WhereEquals("NodeSiteID", CurrentSite.SiteID).WhereIn("ProgramID", GetProgramIDs(OpenCampaign.CampaignID)).ToList();
                if (brandID != default(int) && !DataHelper.DataSourceIsEmpty(products))
                {
                    products = products.Where(x => x.BrandID == brandID).ToList();
                }
                if (categoryID != default(int) && !DataHelper.DataSourceIsEmpty(products))
                {
                    products = products.Where(x => x.CategoryID == categoryID).ToList();
                }
                BindingProductsToRepeater(products, posNum);
            }
            if (TypeOfProduct == (int)ProductsType.GeneralInventory)
            {
                var products = CampaignsProductProvider.GetCampaignsProducts().WhereEquals("NodeSiteID", CurrentSite.SiteID).Where(new WhereCondition().WhereEquals("ProgramID", null).Or().WhereEquals("ProgramID", 0)).ToList();
                if (brandID != default(int) && !DataHelper.DataSourceIsEmpty(products))
                {
                    products = products.Where(x => x.BrandID == brandID).ToList();
                }
                if (categoryID != default(int) && !DataHelper.DataSourceIsEmpty(products))
                {
                    products = products.Where(x => x.CategoryID == categoryID).ToList();
                }
                BindingProductsToRepeater(products, posNum);
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Setting where condition to filter", ex.Message, ex);
        }
    }

    /// <summary>
    /// event for Product type drop down change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddlProductTypes_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetFilter(ValidationHelper.GetInteger(ddlPrograms.SelectedValue, default(int)), ValidationHelper.GetInteger(ddlProductTypes.SelectedValue, default(int)), ValidationHelper.GetInteger(ddlBrands.SelectedValue, default(int)), ValidationHelper.GetString(posNumber.Text, null));
    }

    /// <summary>
    /// event for Brands drop down change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddlBrands_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetFilter(ValidationHelper.GetInteger(ddlPrograms.SelectedValue, default(int)), ValidationHelper.GetInteger(ddlProductTypes.SelectedValue, default(int)), ValidationHelper.GetInteger(ddlBrands.SelectedValue, default(int)), ValidationHelper.GetString(posNumber.Text, null));
    }

    /// <summary>
    /// event for programs drop down change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddlPrograms_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetFilter(ValidationHelper.GetInteger(ddlPrograms.SelectedValue, default(int)), ValidationHelper.GetInteger(ddlProductTypes.SelectedValue, default(int)), ValidationHelper.GetInteger(ddlBrands.SelectedValue, default(int)), ValidationHelper.GetString(posNumber.Text, null));
    }

    /// <summary>
    /// Creating PDF for all the catalog.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void llbSaveSelection_Click(object sender, EventArgs e)
    {
        try
        {
            CreateProductPDF(hdncheckedValues.Value);
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("saving pdf file", ex.Message, ex);
        }
    }

    /// <summary>
    /// Creating products PDF from Html
    /// </summary>
    /// <returns></returns>
    public void CreateProductPDF(string selectedValues)
    {
        try
        {
            if (!string.IsNullOrEmpty(selectedValues))
            {
                lblNoProducts.Visible = false;
                var selectedProducts = selectedValues.Split(',').ToList();
                var skuDetails = SKUInfoProvider.GetSKUs()
                                            .WhereIn(nameof(SKUInfo.SKUID), selectedProducts)
                                            .ToList();
                var htmlTextheader = SettingsKeyInfoProvider.GetValue("ProductsPDFHeader", CurrentSite.SiteID);
                var programFooterText = SettingsKeyInfoProvider.GetValue("KDA_ProgramFooterText", CurrentSite.SiteID);
                if (TypeOfProduct == (int)ProductsType.PreBuy && OpenCampaign != null)
                {
                    htmlTextheader = htmlTextheader.Replace("CAMPAIGNNAME", OpenCampaign?.Name);
                    htmlTextheader = htmlTextheader.Replace("OrderStartDate", OpenCampaign.StartDate == default(DateTime) ? string.Empty : OpenCampaign.StartDate.ToString("MMM dd, yyyy"));
                    htmlTextheader = htmlTextheader.Replace("OrderEndDate", OpenCampaign.EndDate == default(DateTime) ? string.Empty : OpenCampaign.EndDate.ToString("MMM dd, yyyy"));
                }
                var generalInventory = string.Empty;
                if (TypeOfProduct == (int)ProductsType.GeneralInventory)
                {
                    generalInventory = SettingsKeyInfoProvider.GetValue("KDA_GeneralInventoryCover", CurrentSite.SiteID);
                }
                var brands = new List<int>();
                var programs = ProgramProvider.GetPrograms()
                                       .Columns(string.Join(",", nameof(Program.ProgramName), nameof(Program.BrandID), nameof(Program.DeliveryDateToDistributors)))
                                       .WhereEquals(nameof(Program.CampaignID), OpenCampaign?.CampaignID ?? default(int))
                                       .ToList();
                var programsContent = string.Empty;
                if (TypeOfProduct == (int)ProductsType.PreBuy && OpenCampaign != null)
                {
                    foreach (var program in programs)
                    {
                        brands.Add(program.BrandID);
                    }
                }
                else
                {
                    var productItems = CampaignsProductProvider.GetCampaignsProducts()
                                        .WhereEquals(nameof(CampaignsProduct.NodeSiteID), CurrentSite.SiteID)
                                        .Where(new WhereCondition().WhereEquals(nameof(CampaignsProduct.ProgramID), null).Or().WhereEquals(nameof(CampaignsProduct.ProgramID), 0))
                                        .ToList();
                    var inventoryList = productItems
                                        .Join(skuDetails, x => x.NodeSKUID, y => y.SKUID, (x, y) => new { x.BrandID, y.SKUNumber, x.Product.SKUProductCustomerReferenceNumber })
                                        .ToList();
                    foreach (var giProducts in inventoryList)
                    {
                        brands.Add(giProducts.BrandID);
                    }
                }
                var brandData = brands.Distinct();
                brands = CustomTableItemProvider.GetItems(BrandItem.CLASS_NAME)
                        .WhereIn(nameof(CustomTableItem.ItemID), brandData.ToList())
                        .Columns(nameof(CustomTableItem.ItemID))
                        .OrderBy(nameof(BrandItem.BrandName))
                        .Select(x => x.Field<int>(nameof(CustomTableItem.ItemID)))
                        .ToList();
                var pdfProductsContentWithBrands = string.Empty;
                var closingDiv = SettingsKeyInfoProvider.GetValue("ClosingDIV", CurrentSite.SiteID).ToString();
                if (!DataHelper.DataSourceIsEmpty(selectedProducts))
                {
                    foreach (var brand in brands.Distinct())
                    {
                        var productBrandHeader = SettingsKeyInfoProvider.GetValue("PDFBrand", CurrentSite.SiteID);
                        if (TypeOfProduct == (int)ProductsType.PreBuy)
                        {
                            productBrandHeader = productBrandHeader.Replace("^PROGRAMNAME^", programs.Where(x => x.BrandID == brand).Select(y => y.ProgramName).FirstOrDefault());
                            productBrandHeader = productBrandHeader.Replace("^BrandName^", GetBrandName(brand));
                        }
                        else if (TypeOfProduct == (int)ProductsType.GeneralInventory)
                        {
                            productBrandHeader = productBrandHeader.Replace("^BrandName^", GetBrandName(brand));
                            productBrandHeader = productBrandHeader.Replace("^PROGRAMNAME^", string.Empty);
                        }
                        var productItems = new List<CampaignsProduct>();
                        if (TypeOfProduct == (int)ProductsType.PreBuy)
                        {
                            productItems = CampaignsProductProvider
                                .GetCampaignsProducts()
                                .WhereNotEquals(nameof(CampaignsProduct.ProgramID), null)
                                .WhereEquals(nameof(CampaignsProduct.NodeSiteID), CurrentSite.SiteID)
                                .WhereIn(nameof(CampaignsProduct.ProgramID), GetProgramIDs(OpenCampaign.CampaignID))
                                .ToList();
                        }
                        else if (TypeOfProduct == (int)ProductsType.GeneralInventory)
                        {
                            productItems = CampaignsProductProvider
                                .GetCampaignsProducts()
                                .Where(new WhereCondition()
                                .WhereEquals(nameof(CampaignsProduct.ProgramID), null).Or().WhereEquals(nameof(CampaignsProduct.ProgramID), 0))
                                .WhereEquals(nameof(CampaignsProduct.NodeSiteID), CurrentSite.SiteID).ToList();
                        }
                        var catalogList = productItems
                                        .Join(skuDetails,
                                              cp => cp.NodeSKUID,
                                              sku => sku.SKUID,
                                              (cp, sku) => new
                                              {
                                                  cp.ProductName,
                                                  cp.EstimatedPrice,
                                                  cp.BrandID,
                                                  cp.ProgramID,
                                                  QtyPerPack = sku.GetIntegerValue("SKUNumberOfItemsInPackage", 1),
                                                  cp.State,
                                                  sku.SKUPrice,
                                                  sku.SKUNumber,
                                                  cp.Product.SKUProductCustomerReferenceNumber,
                                                  sku.SKUDescription,
                                                  sku.SKUShortDescription,
                                                  cp.ProductImage,
                                                  sku.SKUValidUntil
                                              })
                                        .Where(x => x.BrandID == brand)
                                        .ToList();
                        if (catalogList != null && TypeOfProduct == (int)ProductsType.PreBuy)
                        {
                            var programList = catalogList.GroupBy(p => p.ProgramID).ToList();
                            foreach (var product in programList)
                            {
                                var program = ProgramProvider.GetPrograms().Where(x => x.ProgramID == product.Key).FirstOrDefault();
                                var programContent = SettingsKeyInfoProvider.GetValue("ProgramsContent", CurrentSite.SiteID);
                                programContent = programContent.Replace("^ProgramName^", program?.ProgramName);
                                programContent = programContent.Replace("^ProgramBrandName^", GetBrandName(program.BrandID));
                                programContent = programContent.Replace("ProgramDate", program.DeliveryDateToDistributors == default(DateTime) ? string.Empty : program.DeliveryDateToDistributors.ToString("MMM dd, yyyy"));
                                programsContent += programContent;
                                programContent = string.Empty;
                            }
                            programsContent += programFooterText.Replace("PROGRAMFOOTERTEXT", ResHelper.GetString("Kadena.Catalog.ProgramFooterText"));
                        }
                        var pdfProductsContent = string.Empty;
                        if (!DataHelper.DataSourceIsEmpty(catalogList))
                        {
                            foreach (var product in catalogList)
                            {
                                var stateInfo = CustomTableItemProvider.GetItems<StatesGroupItem>().WhereEquals("ItemID", product.State).FirstOrDefault();
                                var pdfProductContent = SettingsKeyInfoProvider.GetValue("PDFInnerHTML", CurrentSite.SiteID);
                                pdfProductContent = pdfProductContent.Replace("IMAGEGUID", CartPDFHelper.GetThumbnailImageAbsolutePath(product.ProductImage));
                                pdfProductContent = pdfProductContent.Replace("PRODUCTPARTNUMBER", product?.SKUProductCustomerReferenceNumber ?? string.Empty);
                                pdfProductContent = pdfProductContent.Replace("PRODUCTBRANDNAME", GetBrandName(product.BrandID));
                                pdfProductContent = pdfProductContent.Replace("PRODUCTSHORTDESCRIPTION", product?.ProductName ?? string.Empty);
                                pdfProductContent = pdfProductContent.Replace("PRODUCTDESCRIPTION", product?.SKUDescription ?? string.Empty);
                                pdfProductContent = pdfProductContent.Replace("PRODUCTVALIDSTATES", stateInfo?.States.Replace(",", ", ") ?? string.Empty);
                                pdfProductContent = pdfProductContent.Replace("PRODUCTCOSTBUNDLE", TypeOfProduct == (int)ProductsType.PreBuy ? ($"{CurrencyInfoProvider.GetFormattedPrice(ValidationHelper.GetDouble(product.EstimatedPrice, default(double)), CurrentSite.SiteID, true)}") : ($"{CurrencyInfoProvider.GetFormattedPrice(ValidationHelper.GetDouble(product.SKUPrice, default(double)), CurrentSite.SiteID, true)}"));
                                pdfProductContent = pdfProductContent.Replace("PRODUCTBUNDLEQUANTITY", product?.QtyPerPack.ToString() ?? string.Empty);
                                pdfProductContent = pdfProductContent.Replace("PRODUCTEXPIRYDATE", product?.SKUValidUntil != default(DateTime) ? product?.SKUValidUntil.ToString("MMM dd, yyyy") : string.Empty ?? string.Empty);
                                pdfProductsContent += pdfProductContent;
                                pdfProductContent = string.Empty;
                                selectedProducts.Remove(product.SKUNumber);
                            }
                            pdfProductsContentWithBrands += productBrandHeader + pdfProductsContent + closingDiv;
                            productBrandHeader = string.Empty;
                        }
                    }
                }
                var pdfClosingDivs = SettingsKeyInfoProvider.GetValue("PdfEndingTags", CurrentSite.SiteID);
                var html = pdfProductsContentWithBrands + pdfClosingDivs;
                var pdfByte = default(byte[]);
                NReco.PdfGenerator.HtmlToPdfConverter PDFConverter = new NReco.PdfGenerator.HtmlToPdfConverter();
                PDFConverter.License.SetLicenseKey(SettingsKeyInfoProvider.GetValue("KDA_NRecoOwner", CurrentSite.SiteID), SettingsKeyInfoProvider.GetValue("KDA_NRecoKey", CurrentSite.SiteID));
                PDFConverter.LowQuality = SettingsKeyInfoProvider.GetBoolValue("KDA_NRecoLowQuality", CurrentSite.SiteID);
                if (TypeOfProduct == (int)ProductsType.PreBuy)
                {
                    pdfByte = PDFConverter.GeneratePdf(html, htmlTextheader + programsContent + closingDiv);
                }
                else
                {
                    pdfByte = PDFConverter.GeneratePdf(html, generalInventory + closingDiv);
                }
                var fileName = string.Empty;
                if (TypeOfProduct == (int)ProductsType.PreBuy)
                {
                    fileName = ValidationHelper.GetString(ResHelper.GetString("KDA.CatalogGI.PrebuyFileName"), string.Empty) + ".pdf";
                }
                else
                {
                    fileName = ValidationHelper.GetString(ResHelper.GetString("KDA.CatalogGI.GeneralInventory"), string.Empty) + ".pdf";
                }
                Response.Clear();
                var ms = new MemoryStream(pdfByte);
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                Response.Buffer = true;
                ms.WriteTo(Response.OutputStream);
                Response.End();
            }
            else
            {
                Bindproducts();
                noProductSelected.Visible = true;
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("creating html", ex.Message, ex);
        }
    }

    /// <summary>
    /// getting brand name based on programID
    /// </summary>
    /// <param name="BrandID"></param>
    /// <returns></returns>
    public string GetBrandName(int BrandID)
    {
        try
        {
            var brand = CustomTableItemProvider.GetItems(BrandItem.CLASS_NAME)
                        .WhereEquals("ItemID", BrandID).Columns("BrandName")
                        .Select(x => new BrandItem { BrandName = x.Field<string>("BrandName") })
                        .FirstOrDefault();
            return brand?.BrandName ?? string.Empty;
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("GetBrandName", ex.Message, ex);
            return string.Empty;
        }
    }

    /// <summary>
    /// saving full catalog to PDF
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void llbSaveFull_Click(object sender, EventArgs e)
    {
        try
        {
            if (TypeOfProduct == (int)ProductsType.PreBuy)
                GeneratePBFullPDF();
            else if (TypeOfProduct == (int)ProductsType.GeneralInventory)
                GenerateGIFullPDF();
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("printing all the products in catalog", ex.Message, ex);
        }
    }

    /// <summary>
    /// Binding products to repeater
    /// </summary>
    /// <param name="productsList"></param>
    public void BindingProductsToRepeater(List<CampaignsProduct> productsList, string posNum = null)
    {
        try
        {
            List<int> skuIds = new List<int>();
            if (!DataHelper.DataSourceIsEmpty(productsList))
            {
                foreach (var product in productsList)
                {
                    skuIds.Add(product.NodeSKUID);
                }
            }
            if (!DataHelper.DataSourceIsEmpty(skuIds))
            {
                var skuDetails = SKUInfoProvider.GetSKUs()
                                .WhereIn("SKUID", skuIds)
                                .And()
                                .WhereEquals("SKUEnabled", true)
                                .Columns("SKUProductCustomerReferenceNumber,SKUNumber,SKUName,SKUPrice,SKUEnabled,SKUAvailableItems,SKUID,SKUDescription")
                                .ToList();
                if (!DataHelper.DataSourceIsEmpty(skuDetails))
                {
                    var catalogList = productsList
                                      .Join(skuDetails,
                                            x => x.NodeSKUID,
                                            y => y.SKUID,
                                            (cp, sku) => new
                                            {
                                                cp.ProductName,
                                                cp.NodeSKUID,
                                                QtyPerPack = sku.GetIntegerValue("SKUNumberOfItemsInPackage", 1),
                                                cp.State,
                                                cp.BrandID,
                                                sku.SKUNumber,
                                                cp.Product.SKUProductCustomerReferenceNumber,
                                                sku.SKUDescription,
                                                sku.SKUShortDescription,
                                                cp.ProductImage,
                                                sku.SKUValidUntil,
                                                cp.EstimatedPrice
                                            })
                                      .OrderBy(p => p.ProductName)
                                      .ToList();
                    if (!DataHelper.DataSourceIsEmpty(catalogList) && posNum != null)
                    {
                        catalogList = catalogList.Where(x => x.SKUNumber.Contains(posNum)).ToList();
                    }
                    if (!DataHelper.DataSourceIsEmpty(catalogList))
                    {
                        noData.Visible = false;
                        rptCatalogProducts.DataSource = catalogList;
                        rptCatalogProducts.DataBind();
                    }
                    else
                    {
                        noData.Visible = true;
                    }
                }
                else
                {
                    noData.Visible = true;
                }
            }
            else
            {
                noData.Visible = true;
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Binding products to repeater", ex.Message, ex);
        }
    }

    /// <summary>
    /// getting brand name based on programID
    /// </summary>
    /// <param name="BrandID"></param>
    /// <returns></returns>
    public string GetProgramName(int brandID)
    {
        try
        {
            var programs = ProgramProvider.GetPrograms()
                                    .WhereEquals("BrandID", brandID)
                                    .Columns("ProgramName").FirstOrDefault();
            return programs?.ProgramName ?? string.Empty;
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("GetBrandName", ex.Message, ex);
            return string.Empty;
        }
    }

    /// <summary>
    /// Filtering data by POS number
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void posNumber_TextChanged(object sender, EventArgs e)
    {
        SetFilter(ValidationHelper.GetInteger(ddlPrograms.SelectedValue, default(int)), ValidationHelper.GetInteger(ddlProductTypes.SelectedValue, default(int)), ValidationHelper.GetInteger(ddlBrands.SelectedValue, default(int)), ValidationHelper.GetString(posNumber.Text, null));
    }

    /// <summary>
    /// Generates full pdf for pre buy products
    /// </summary>
    private void GeneratePBFullPDF()
    {
        try
        {
            var programs = ProgramProvider.GetPrograms()
                                   .Columns("ProgramName,BrandID,DeliveryDateToDistributors,ProgramID")
                                   .WhereEquals("CampaignID", OpenCampaign?.CampaignID ?? default(int))
                                   .ToList();
            lblNoProducts.Visible = false;
            var programIDs = new List<int>();
            programs.ForEach(x =>
            {
                programIDs.Add(x.ProgramID);
            });
            var productData = CampaignsProductProvider.GetCampaignsProducts()
                .WhereEquals("NodeSiteID", CurrentSite.SiteID)
                .WhereNotNull("ProgramID")
                .WhereGreaterThan("ProgramID", default(int))
                .WhereIn("ProgramID", programIDs)
                .ToList();
            var skuDetails = SKUInfoProvider.GetSKUs()
                .WhereIn("SKUID", productData.Select(s => s.SKU.SKUID).ToList())
                .ToList();
            string htmlTextheader = SettingsKeyInfoProvider.GetValue($@"{CurrentSiteName}.ProductsPDFHeader");
            string programFooterText = SettingsKeyInfoProvider.GetValue($@"{CurrentSiteName}.KDA_ProgramFooterText");
            htmlTextheader = htmlTextheader.Replace("CAMPAIGNNAME", OpenCampaign?.Name);
            htmlTextheader = htmlTextheader.Replace("OrderStartDate", OpenCampaign.StartDate == default(DateTime) ? string.Empty : OpenCampaign.StartDate.ToString("MMM dd, yyyy"));
            htmlTextheader = htmlTextheader.Replace("OrderEndDate", OpenCampaign.EndDate == default(DateTime) ? string.Empty : OpenCampaign.EndDate.ToString("MMM dd, yyyy"));
            List<int> brands = new List<int>();
            string programsContent = string.Empty;
            foreach (var program in programs)
            {
                string programContent = SettingsKeyInfoProvider.GetValue($@"{CurrentSiteName}.ProgramsContent");
                brands.Add(program.BrandID);
                programContent = programContent.Replace("^ProgramName^", program?.ProgramName);
                programContent = programContent.Replace("^ProgramBrandName^", GetBrandName(program.BrandID));
                programContent = programContent.Replace("ProgramDate", program.DeliveryDateToDistributors == default(DateTime) ? string.Empty : program.DeliveryDateToDistributors.ToString("MMM dd, yyyy"));
                programsContent += programContent;
                programContent = string.Empty;
            }
            programsContent += programFooterText.Replace("PROGRAMFOOTERTEXT", ResHelper.GetString("Kadena.Catalog.ProgramFooterText"));
            string pdfProductsContentWithBrands = string.Empty;
            string closingDiv = SettingsKeyInfoProvider.GetValue("ClosingDIV").ToString();
            foreach (var brand in brands.Distinct())
            {
                string productBrandHeader = SettingsKeyInfoProvider.GetValue($@"{CurrentSiteName}.PDFBrand");
                productBrandHeader = productBrandHeader.Replace("^PROGRAMNAME^", programs.Where(x => x.BrandID == brand).Select(y => y.ProgramName).FirstOrDefault());
                productBrandHeader = productBrandHeader.Replace("^BrandName^", GetBrandName(brand));
                var catalogList = productData
                 .Join(skuDetails,
                       cp => cp.NodeSKUID,
                       sku => sku.SKUID,
                       (cp, sku) => new
                       {
                           cp.ProductName,
                           cp.EstimatedPrice,
                           cp.BrandID,
                           cp.ProgramID,
                           QtyPerPack = sku.GetIntegerValue("SKUNumberOfItemsInPackage", 1),
                           cp.State,
                           sku.SKUPrice,
                           sku.SKUNumber,
                           cp.Product.SKUProductCustomerReferenceNumber,
                           sku.SKUDescription,
                           sku.SKUShortDescription,
                           cp.ProductImage,
                           sku.SKUValidUntil
                       })
                 .Where(x => x.BrandID == brand)
                 .ToList();
                string pdfProductsContent = string.Empty;
                if (!DataHelper.DataSourceIsEmpty(catalogList))
                {
                    foreach (var product in catalogList)
                    {
                        var stateInfo = CustomTableItemProvider.GetItems<StatesGroupItem>().WhereEquals("ItemID", product.State).FirstOrDefault();
                        string pdfProductContent = SettingsKeyInfoProvider.GetValue($@"{CurrentSiteName}.PDFInnerHTML");
                        pdfProductContent = pdfProductContent.Replace("IMAGEGUID", CartPDFHelper.GetThumbnailImageAbsolutePath(product.ProductImage));
                        pdfProductContent = pdfProductContent.Replace("PRODUCTPARTNUMBER", product?.SKUProductCustomerReferenceNumber ?? string.Empty);
                        pdfProductContent = pdfProductContent.Replace("PRODUCTBRANDNAME", GetBrandName(product.BrandID));
                        pdfProductContent = pdfProductContent.Replace("PRODUCTSHORTDESCRIPTION", product?.ProductName ?? string.Empty);
                        pdfProductContent = pdfProductContent.Replace("PRODUCTDESCRIPTION", product?.SKUDescription ?? string.Empty);
                        pdfProductContent = pdfProductContent.Replace("PRODUCTVALIDSTATES", stateInfo?.States.Replace(",", ", ") ?? string.Empty);
                        pdfProductContent = pdfProductContent.Replace("PRODUCTCOSTBUNDLE", TypeOfProduct == (int)ProductsType.PreBuy ? ($"{CurrencyInfoProvider.GetFormattedPrice(ValidationHelper.GetDouble(product.EstimatedPrice, default(double)), CurrentSite.SiteID, true)}") : ($"{CurrencyInfoProvider.GetFormattedPrice(ValidationHelper.GetDouble(product.SKUPrice, default(double)), CurrentSite.SiteID, true)}"));
                        pdfProductContent = pdfProductContent.Replace("PRODUCTBUNDLEQUANTITY", product?.QtyPerPack.ToString() ?? string.Empty);
                        pdfProductContent = pdfProductContent.Replace("PRODUCTEXPIRYDATE", product?.SKUValidUntil != default(DateTime) ? product?.SKUValidUntil.ToString("MMM dd, yyyy") : string.Empty ?? string.Empty);
                        pdfProductsContent += pdfProductContent;
                        pdfProductContent = string.Empty;
                    }
                    pdfProductsContentWithBrands += productBrandHeader + pdfProductsContent + closingDiv;
                    productBrandHeader = string.Empty;
                }
            }
            string pdfClosingDivs = SettingsKeyInfoProvider.GetValue($@"{CurrentSiteName}.PdfEndingTags");
            string html = pdfProductsContentWithBrands + pdfClosingDivs;
            byte[] pdfByte = default(byte[]);
            NReco.PdfGenerator.HtmlToPdfConverter PDFConverter = new NReco.PdfGenerator.HtmlToPdfConverter();
            PDFConverter.License.SetLicenseKey(SettingsKeyInfoProvider.GetValue("KDA_NRecoOwner", CurrentSite.SiteID), SettingsKeyInfoProvider.GetValue("KDA_NRecoKey", CurrentSite.SiteID));
            PDFConverter.LowQuality = SettingsKeyInfoProvider.GetBoolValue("KDA_NRecoLowQuality", CurrentSite.SiteID);
            pdfByte = PDFConverter.GeneratePdf(html, htmlTextheader + programsContent + closingDiv);
            string fileName = string.Empty;
            fileName = ValidationHelper.GetString(ResHelper.GetString("KDA.CatalogGI.PrebuyFileName"), string.Empty) + ".pdf";
            Response.Clear();
            MemoryStream ms = new MemoryStream(pdfByte);
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            Response.Buffer = true;
            ms.WriteTo(Response.OutputStream);
            Response.End();
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("creating pdf for general inventory full catalog", ex.Message, ex);
        }
    }

    /// <summary>
    /// Generates full pdf for inventory products
    /// </summary>
    private void GenerateGIFullPDF()
    {
        try
        {
            var productData = CampaignsProductProvider.GetCampaignsProducts()
                 .WhereEquals("NodeSiteID", CurrentSite.SiteID)
                 .Where(new WhereCondition().WhereNull("ProgramID").Or().WhereEquals("ProgramID", 0))
                 .ToList();
            lblNoProducts.Visible = false;
            var skuDetails = SKUInfoProvider.GetSKUs()
                                        .WhereIn("SKUID", productData.Select(s => s.SKU?.SKUID ?? -1).ToList())
                                        .ToList();
            string generalInventory = string.Empty;
            generalInventory = SettingsKeyInfoProvider.GetValue($@"{CurrentSiteName}.KDA_GeneralInventoryCover");
            List<int> brands = new List<int>();
            var inventoryList = productData
                                .Join(skuDetails, x => x.NodeSKUID, y => y.SKUID, (x, y) => new { x.BrandID, y.SKUNumber, x.Product.SKUProductCustomerReferenceNumber })
                                .ToList();
            foreach (var giProducts in inventoryList)
            {
                brands.Add(giProducts.BrandID);
            }
            string pdfProductsContentWithBrands = string.Empty;
            string closingDiv = SettingsKeyInfoProvider.GetValue("ClosingDIV").ToString();
            if (!DataHelper.DataSourceIsEmpty(inventoryList))
            {
                foreach (var brand in brands.Distinct())
                {
                    string productBrandHeader = SettingsKeyInfoProvider.GetValue($@"{CurrentSiteName}.PDFBrand");
                    productBrandHeader = productBrandHeader.Replace("^BrandName^", GetBrandName(brand));
                    productBrandHeader = productBrandHeader.Replace("^PROGRAMNAME^", string.Empty);
                    var catalogList = productData
                                    .Join(skuDetails,
                                          cp => cp.NodeSKUID,
                                          sku => sku.SKUID,
                                          (cp, sku) => new
                                          {
                                              cp.ProductName,
                                              cp.EstimatedPrice,
                                              cp.BrandID,
                                              cp.ProgramID,
                                              QtyPerPack = sku.GetIntegerValue("SKUNumberOfItemsInPackage", 1),
                                              cp.State,
                                              sku.SKUPrice,
                                              sku.SKUNumber,
                                              cp.Product.SKUProductCustomerReferenceNumber,
                                              sku.SKUDescription,
                                              sku.SKUShortDescription,
                                              cp.ProductImage,
                                              sku.SKUValidUntil
                                          })
                                    .Where(x => x.BrandID == brand)
                                    .ToList();
                    string pdfProductsContent = string.Empty;
                    if (!DataHelper.DataSourceIsEmpty(catalogList))
                    {
                        foreach (var product in catalogList)
                        {
                            var stateInfo = CustomTableItemProvider.GetItems<StatesGroupItem>().WhereEquals("ItemID", product.State).FirstOrDefault();
                            string pdfProductContent = SettingsKeyInfoProvider.GetValue($@"{CurrentSiteName}.PDFInnerHTML");
                            pdfProductContent = pdfProductContent.Replace("IMAGEGUID", CartPDFHelper.GetThumbnailImageAbsolutePath(product.ProductImage));
                            pdfProductContent = pdfProductContent.Replace("PRODUCTPARTNUMBER", product?.SKUProductCustomerReferenceNumber ?? string.Empty);
                            pdfProductContent = pdfProductContent.Replace("PRODUCTBRANDNAME", GetBrandName(product.BrandID));
                            pdfProductContent = pdfProductContent.Replace("PRODUCTSHORTDESCRIPTION", product?.ProductName ?? string.Empty);
                            pdfProductContent = pdfProductContent.Replace("PRODUCTDESCRIPTION", product?.SKUDescription ?? string.Empty);
                            pdfProductContent = pdfProductContent.Replace("PRODUCTVALIDSTATES", stateInfo?.States.Replace(",", ", ") ?? string.Empty);
                            pdfProductContent = pdfProductContent.Replace("PRODUCTCOSTBUNDLE", TypeOfProduct == (int)ProductsType.PreBuy ? ($"{CurrencyInfoProvider.GetFormattedPrice(ValidationHelper.GetDouble(product.EstimatedPrice, default(double)), CurrentSite.SiteID, true)}") : ($"{CurrencyInfoProvider.GetFormattedPrice(ValidationHelper.GetDouble(product.SKUPrice, default(double)), CurrentSite.SiteID, true)}"));
                            pdfProductContent = pdfProductContent.Replace("PRODUCTBUNDLEQUANTITY", product?.QtyPerPack.ToString() ?? string.Empty);
                            pdfProductContent = pdfProductContent.Replace("PRODUCTEXPIRYDATE", product?.SKUValidUntil != default(DateTime) ? product?.SKUValidUntil.ToString("MMM dd, yyyy") : string.Empty ?? string.Empty);
                            pdfProductsContent += pdfProductContent;
                            pdfProductContent = string.Empty;
                        }
                        pdfProductsContentWithBrands += productBrandHeader + pdfProductsContent + closingDiv;
                        productBrandHeader = string.Empty;
                    }
                }
            }
            string pdfClosingDivs = SettingsKeyInfoProvider.GetValue($@"{CurrentSiteName}.PdfEndingTags");
            string html = pdfProductsContentWithBrands + pdfClosingDivs;
            byte[] pdfByte = default(byte[]);
            NReco.PdfGenerator.HtmlToPdfConverter PDFConverter = new NReco.PdfGenerator.HtmlToPdfConverter();
            PDFConverter.License.SetLicenseKey(SettingsKeyInfoProvider.GetValue("KDA_NRecoOwner", CurrentSite.SiteID), SettingsKeyInfoProvider.GetValue("KDA_NRecoKey", CurrentSite.SiteID));
            PDFConverter.LowQuality = SettingsKeyInfoProvider.GetBoolValue("KDA_NRecoLowQuality", CurrentSite.SiteID);
            pdfByte = PDFConverter.GeneratePdf(html, generalInventory + closingDiv);
            string fileName = string.Empty;
            fileName = ValidationHelper.GetString(ResHelper.GetString("KDA.CatalogGI.GeneralInventory"), string.Empty) + ".pdf";
            Response.Clear();
            MemoryStream ms = new MemoryStream(pdfByte);
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            Response.Buffer = true;
            ms.WriteTo(Response.OutputStream);
            Response.End();
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("creating pdf for general inventory full catalog", ex.Message, ex);
        }
    }

    #endregion "Methods"

    protected void llbExportFull_Click(object sender, EventArgs e)
    {
        List<CampaignsProduct> products = new List<CampaignsProduct>();
        List<PrebuyProduct> exportList = new List<PrebuyProduct>();
        string fileName = "Kadena.Catalog.ExcelExportPrebuy";
        if (TypeOfProduct == (int)ProductsType.PreBuy)
        {
            products = CampaignsProductProvider.GetCampaignsProducts().WhereNotEquals("ProgramID", null).WhereEquals("NodeSiteID", CurrentSite.SiteID).WhereIn("ProgramID", GetProgramIDs(OpenCampaign.CampaignID)).ToList();
            if (!DataHelper.DataSourceIsEmpty(products))
            {
                products.ForEach(p =>
            {
                exportList.Add(new PrebuyProduct()
                {
                    ProductId = p.CampaignsProductID,
                    ProductName = p.ProductName,
                    ShortDescription = p.DocumentSKUShortDescription,
                    BundleQuantity = p.SKU.GetIntegerValue("SKUNumberOfItemsInPackage", 1),
                    ProductCost = CurrencyInfoProvider.GetFormattedPrice(ValidationHelper.GetDouble(p.EstimatedPrice, default(double)), CurrentSite.SiteID, true),
                    ProgramName = GetProgramFormId(p.ProgramID),
                    BrandName = GetBrandName(p.BrandID),
                    PosNumber = GetPosNumber(p.SKU.SKUID),
                    States = GetStateInfo(p.State)
                });
            });
            }
        }
        else if (TypeOfProduct == (int)ProductsType.GeneralInventory)
        {
            fileName = "Kadena.Catalog.ExcelExportInventory";
            products = CampaignsProductProvider.GetCampaignsProducts().WhereEquals("NodeSiteID", CurrentSite.SiteID)
                            .Where(new WhereCondition().WhereEquals("ProgramID", null).Or().WhereEquals("ProgramID", 0)).ToList();
            if (!DataHelper.DataSourceIsEmpty(products))
            {
                products.ForEach(p =>
            {
                exportList.Add(new PrebuyProduct()
                {
                    ProductId = p.CampaignsProductID,
                    ProductName = p.ProductName,
                    ShortDescription = p.DocumentSKUShortDescription,
                    BundleQuantity = p.SKU.GetIntegerValue("SKUNumberOfItemsInPackage", 1),
                    ProductCost = CurrencyInfoProvider.GetFormattedPrice(ValidationHelper.GetDouble(p.SKU.SKUPrice, default(double)), CurrentSite.SiteID, true),
                    BrandName = GetBrandName(p.BrandID),
                    PosNumber = GetPosNumber(p.SKU.SKUID),
                    States = GetStateInfo(p.State)
                });
            });
            }
        }
        DownloadExcel(exportList, fileName);
    }


    public void DownloadExcel<T>(List<T> exportList, string fileName)
    {
        DataGrid dg = new DataGrid();
        dg.AllowPaging = false;
        dg.DataSource = exportList;
        dg.DataBind();
        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.Buffer = true;
        HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;
        HttpContext.Current.Response.Charset = "";
        HttpContext.Current.Response.AddHeader("Content-Disposition",
          "attachment; filename=" + ResHelper.GetString(fileName) + ".xls");
        HttpContext.Current.Response.ContentType =
          "application/vnd.ms-excel";
        StringWriter stringWriter = new StringWriter();
        System.Web.UI.HtmlTextWriter htmlTextWriter =
          new System.Web.UI.HtmlTextWriter(stringWriter);
        dg.RenderControl(htmlTextWriter);
        HttpContext.Current.Response.Write(stringWriter.ToString());
        HttpContext.Current.Response.End();
    }

    public string GetProgramFormId(int programId)
    {
        if (programId > 0)
        {
            var programData = ProgramProvider.GetPrograms().WhereEquals("ProgramID", programId).FirstOrDefault();
            if (!DataHelper.DataSourceIsEmpty(programData)) return programData.ProgramName;
            return string.Empty;
        }
        return string.Empty;
    }

    public string GetPosNumber(int skuId)
    {
        if (skuId > 0)
        {
            var skuData = SKUInfoProvider.GetSKUs().WhereEquals("SKUID", skuId).FirstOrDefault();
            if (!DataHelper.DataSourceIsEmpty(skuData)) return skuData.GetValue("SKUProductCustomerReferenceNumber", string.Empty);
        }
        return string.Empty;
    }

    public string GetStateInfo(int stateId)
    {
        var stateData = CustomTableItemProvider.GetItems<StatesGroupItem>().WhereEquals("ItemID", stateId).FirstOrDefault();
        return stateData?.States.Replace(",", ", ") ?? string.Empty;
    }

    protected void llbExportSelection_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(hdncheckedValues.Value))
        {
            Bindproducts();
            noProductSelected.Visible = true;
            return;
        }
        string fileName = "Kadena.Catalog.ExcelExportPrebuy";
        List<string> selectedProducts = hdncheckedValues.Value.Split(',').ToList();
        List<PrebuyProduct> exportList = new List<PrebuyProduct>();
        var skuDetails = SKUInfoProvider.GetSKUs().WhereIn("SKUID", selectedProducts).ToList();
        if (TypeOfProduct == (int)ProductsType.GeneralInventory)
        {
            fileName = "Kadena.Catalog.ExcelExportInventory";
            skuDetails.ForEach(p =>
        {
            var productData = CampaignsProductProvider.GetCampaignsProducts().WhereEquals("NodeSKUID", p.SKUID).FirstOrDefault();
            exportList.Add(new PrebuyProduct()
            {
                ProductId = productData.CampaignsProductID,
                ProductName = productData.ProductName,
                ShortDescription = p.SKUShortDescription,
                BundleQuantity = productData.SKU.GetIntegerValue("SKUNumberOfItemsInPackage", 1),
                ProductCost = CurrencyInfoProvider.GetFormattedPrice(ValidationHelper.GetDouble(p.SKUPrice, default(double)), CurrentSite.SiteID, true),
                BrandName = GetBrandName(productData.BrandID),
                PosNumber = GetPosNumber(productData.SKU.SKUID),
                States = GetStateInfo(productData.State)
            });
        });
        }
        if (TypeOfProduct == (int)ProductsType.PreBuy)
        {
            skuDetails.ForEach(p =>
            {
                var productData = CampaignsProductProvider.GetCampaignsProducts().WhereEquals("NodeSKUID", p.SKUID).FirstOrDefault();
                exportList.Add(new PrebuyProduct()
                {
                    ProductId = productData.CampaignsProductID,
                    ProductName = productData.ProductName,
                    ShortDescription = p.SKUShortDescription,
                    BundleQuantity = productData.SKU.GetIntegerValue("SKUNumberOfItemsInPackage", 1),
                    ProductCost = CurrencyInfoProvider.GetFormattedPrice(ValidationHelper.GetDouble(productData.EstimatedPrice, default(double)), CurrentSite.SiteID, true),
                    BrandName = GetBrandName(productData.BrandID),
                    PosNumber = GetPosNumber(productData.SKU.SKUID),
                    States = GetStateInfo(productData.State),
                    ProgramName = GetProgramFormId(productData.ProgramID)
                });
            });
        }
        DownloadExcel(exportList, fileName);
    }
}