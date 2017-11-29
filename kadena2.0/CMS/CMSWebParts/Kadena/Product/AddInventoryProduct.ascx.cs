﻿using CMS.CustomTables;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.KDA;
using CMS.DocumentEngine.Web.UI;
using CMS.EventLog;
using CMS.Globalization;
using CMS.Helpers;
using CMS.MediaLibrary;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using Kadena.Old_App_Code.Kadena.ImageUpload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Kadena.CMSWebParts.Kadena.Product
{
    public partial class AddInventoryProduct : CMSAbstractWebPart
    {
        #region variables
        private int productId = 0;
        private string folderpath = "/";
        private int PageSize = 10;
        private static List<AllocateProduct> lstUsers = new List<AllocateProduct>();
        string libraryFolderName = string.Empty;
        string redirectUrl = string.Empty;
        #endregion
        #region WebpartSetupMethods
        public override void OnContentLoaded()
        {
            base.OnContentLoaded();
            SetupControl();
        }
        public override void ReloadData()
        {
            base.ReloadData();

            SetupControl();
        }
        protected void SetupControl()
        {
            if (!this.StopProcessing)
            {
                // Do not process
                BindData();
                if (Request.QueryString["ID"] != null)
                {

                    btnSave.Click += btnUpdate_Click;
                    productId = ValidationHelper.GetInteger(Request.QueryString["ID"], 0);

                }
                else
                {
                    btnSave.Click += btnSave_SavePOS;
                }
                if (!IsPostBack)
                {
                    if (Request.QueryString["ID"] != null)
                    {
                        SetFeild(productId);
                        if(Request.UrlReferrer !=null)
                        ViewState["LastPageUrl"] = Request.UrlReferrer.ToString();
                    }
                    BindUsers(1);
                }
                btnSave.Click += btnSave_SavePOS;
                btnAllocateProduct.Click += AllocateProduct_Click;
                btnCancel.Click += BtnCancel_Cancel;
                hdnDatepickerUrl.Value= SettingsKeyInfoProvider.GetValue("KDA_DatePickerPath", CurrentSiteName);
                rfvBrand.ErrorMessage = ResHelper.GetString("Kadena.InvProductForm.BrandRequired");
                rfvActualPrice.ErrorMessage = ResHelper.GetString("Kadena.InvProductForm.ActualPriceRequired");
                rfvPosNo.ErrorMessage = ResHelper.GetString("Kadena.InvProductForm.POSCodeRequired");
                rfvProdCategory.ErrorMessage = ResHelper.GetString("Kadena.InvProductForm.POSCategroyRequired");
                rfvExpDate.ErrorMessage = ResHelper.GetString("Kadena.InvProductForm.ExpiryDateRequired");
                rfvEstPrice.ErrorMessage = ResHelper.GetString("Kadena.InvProductForm.EstimatedPriceRequired");
                rfvLongDes.ErrorMessage = ResHelper.GetString("Kadena.InvProductForm.LongDescritpionRequired");
                rfvShortDes.ErrorMessage = ResHelper.GetString("Kadena.InvProductForm.ShortDescriptionRequired");
                rfvState.ErrorMessage = ResHelper.GetString("Kadena.InvProductForm.StateRequired");
                rfvBundleQnt.ErrorMessage = ResHelper.GetString("Kadena.InvProductForm.BundleQntRequired");
                folderpath = SettingsKeyInfoProvider.GetValue("KDA.InventoryProductFolderPath", CurrentSiteName);
                libraryFolderName = SettingsKeyInfoProvider.GetValue("KDA_InventoryProductFolderPath", CurrentSiteName);
            }
        }
        #endregion
        #region ButtonclickEvents
        /// <summary>
        /// Save the Product to Product pagetype
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_SavePOS(object sender, EventArgs e)
        {
            Guid imageGuid = default(Guid);
            try
            {
                if (ddlBrand.SelectedIndex > 0 && ddlPosNo.SelectedIndex > 0 && ddlProdCategory.SelectedIndex > 0 && ddlState.SelectedIndex > 0)
                {
                    TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
                    CMS.DocumentEngine.TreeNode parentPage = tree.SelectNodes().Path(folderpath).OnCurrentSite().Culture(DocumentContext.CurrentDocument.DocumentCulture).FirstObject;
                    if (parentPage != null)
                    {
                       
                        CMS.DocumentEngine.TreeNode newProduct = CMS.DocumentEngine.TreeNode.New("KDA.CampaignProduct", tree);

                        // Sets the properties of the new page
                        newProduct.DocumentCulture = DocumentContext.CurrentDocument.DocumentCulture;
                        newProduct.DocumentName = txtShortDes.Text;
                        newProduct.SetValue("BundleQty", ValidationHelper.GetString(txtBundleQnt.Text, string.Empty));
                        newProduct.SetValue("POSNumber", ValidationHelper.GetInteger(ddlPosNo.SelectedValue, 0));
                        newProduct.SetValue("BrandID", ValidationHelper.GetInteger(ddlBrand.SelectedValue, 0));
                        newProduct.SetValue("EstimatedPrice", ValidationHelper.GetString(txtEstPrice.Text, string.Empty));
                        newProduct.SetValue("ActualPrice", ValidationHelper.GetString(txtActualPrice.Text, string.Empty));
                        newProduct.SetValue("ShortDescription", ValidationHelper.GetString(txtShortDes.Text, string.Empty));
                        newProduct.SetValue("State", ValidationHelper.GetString(ddlState.SelectedValue, string.Empty));
                        newProduct.SetValue("ExpirationDate", ValidationHelper.GetDate(txtExpDate.Text, DateTime.Now.Date));
                        newProduct.SetValue("CategoryID", ValidationHelper.GetInteger(ddlProdCategory.SelectedValue, 0));
                        newProduct.SetValue("Cancelled", ValidationHelper.GetBoolean(chkcancel.Checked, false));
                        newProduct.SetValue("CVOProductID", ValidationHelper.GetString(txtCVOProductId.Text, string.Empty));
                        newProduct.SetValue("TotalQty", ValidationHelper.GetString(txtQuantity.Text, string.Empty));
                        newProduct.SetValue("Status", ValidationHelper.GetString(ddlStatus.SelectedValue, string.Empty));
                        newProduct.SetValue("StorefrontProductID", ValidationHelper.GetString(txtStroeFrontId.Text, string.Empty));
                        newProduct.SetValue("LongDescription", ValidationHelper.GetString(txtLongDes.Text, string.Empty));
                        if (productImage.HasFile)
                        {
                            imageGuid = UploadImage.UploadImageToMeadiaLibrary(productImage, libraryFolderName);
                            newProduct.SetValue("Image", ValidationHelper.GetString(imageGuid, string.Empty));
                        }
                        //// Inserts the new page as a child of the parent page
                        newProduct.Insert(parentPage);
                        var productID = ValidationHelper.GetInteger(newProduct.GetValue("CampaignProductID"), 0);
                        AllocateProductToUsers(productID);
                        lblSuccessMsg.Visible = true;
                        lblFailureText.Visible = false;
                        EmptyFields();
                    }
                    else
                    {
                        lblFailureText.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                string libraryFolderName = SettingsKeyInfoProvider.GetValue(CurrentSite.SiteName + ".KDA_InventoryProductFolderName");
                EventLogProvider.LogException("SaveProductFromButtonClick", "EXCEPTION", ex);
                if (imageGuid != default(Guid))
                {
                    UploadImage.DeleteImage(imageGuid, libraryFolderName);
                }
                 EventLogProvider.LogInformation("CMSWebParts_Kadena_Add_inventory_Products", "btnSave_Click", ex.Message);
            }

        }
        
        /// <summary>
        /// on Cancel button Click It will redirect to the last page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnCancel_Cancel(object sender, EventArgs e)
        {
            try
            {
                EmptyFields();
                lstUsers = new List<AllocateProduct>();
                var redirectUrl = ValidationHelper.GetString(ViewState["LastPageUrl"], string.Empty);
                Response.Redirect(redirectUrl, false);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("CancePOSFormButtonClick", "EXCEPTION", ex);
            }
        }
        /// <summary>
        /// Handles the Click event of the Allocate product button click .
        /// </summary>
        protected void AllocateProduct_Click(object sender, EventArgs e)
        {
            // Assigns the value of the UniSelector control to be displayed by the Label
            try
            {
                foreach (RepeaterItem ri in RepterDetails.Items)
                {
                    CheckBox item_check = (CheckBox)ri.FindControl("chkAllocate");
                    if (item_check.Checked)
                    {
                        int index = lstUsers.FindIndex(item => item.UserID == ValidationHelper.GetInteger(((Label)ri.FindControl("lblUserid")).Text, 0));
                        if (index == -1)
                        {
                            AllocateProduct objAllocateProduct = new AllocateProduct();
                            objAllocateProduct.UserID = ValidationHelper.GetInteger(((Label)ri.FindControl("lblUserid")).Text, 0);
                            objAllocateProduct.UserName = ValidationHelper.GetString(((Label)ri.FindControl("lblUserName")).Text, string.Empty);
                            objAllocateProduct.EmailID = ValidationHelper.GetString(((Label)ri.FindControl("lblEmail")).Text, string.Empty);
                            objAllocateProduct.Quantity = ValidationHelper.GetInteger(((TextBox)ri.FindControl("txtAllQuantity")).Text, 0);
                            lstUsers.Add(objAllocateProduct);
                        }
                    }
                    else
                    {
                        int index = lstUsers.FindIndex(item => item.UserID == ValidationHelper.GetInteger(((Label)ri.FindControl("lblUserid")).Text, 0));
                        if (index != -1)
                        {
                            lstUsers.RemoveAt(index);
                        }
                    }
                }

                RepSelectedUser.DataSource = lstUsers;
                RepSelectedUser.DataBind();
            }
            catch (Exception ex)
            {
                EventLogProvider.LogInformation("CMSWebParts_Kadena_Add_inventory_Products_AllocateProduct", "AllocateProduct_Click", ex.Message);
            }
        }
       
        /// <summary>
        /// This method will get the users record when user click on pagination
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Changed(object sender, EventArgs e)
        {
            try
            {
                foreach (RepeaterItem ri in RepterDetails.Items)
                {
                    CheckBox item_check = (CheckBox)ri.FindControl("chkAllocate");
                    if (item_check.Checked)
                    {
                        int index = lstUsers.FindIndex(item => item.UserID == ValidationHelper.GetInteger(((Label)ri.FindControl("lblUserid")).Text, 0));
                        if (index == -1)
                        {
                            AllocateProduct objAllocateProduct = new AllocateProduct();
                            objAllocateProduct.UserID = ValidationHelper.GetInteger(((Label)ri.FindControl("lblUserid")).Text, 0);
                            objAllocateProduct.UserName = ValidationHelper.GetString(((Label)ri.FindControl("lblUserName")).Text, string.Empty);
                            objAllocateProduct.EmailID = ValidationHelper.GetString(((Label)ri.FindControl("lblEmail")).Text, string.Empty);
                            objAllocateProduct.Quantity = ValidationHelper.GetInteger(((TextBox)ri.FindControl("txtAllQuantity")).Text, 0);
                            lstUsers.Add(objAllocateProduct);
                        }
                    }
                    else
                    {
                        int index = lstUsers.FindIndex(item => item.UserID == ValidationHelper.GetInteger(((Label)ri.FindControl("lblUserid")).Text, 0));
                        if (index != -1)
                        {
                            lstUsers.RemoveAt(index);
                        }
                    }
                }
                int pageIndex = int.Parse((sender as LinkButton).CommandArgument);
                this.BindUsers(pageIndex);
            }
            catch(Exception ex)
            {
                EventLogProvider.LogInformation("CMSWebParts_Kadena_Add_inventory_Products_Pagination", "Page_Changed", ex.Message);
            }
          
        }
      
        /// <summary>
        /// Method to update the product
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            Guid imageGuid = default(Guid);
            try
            {
                if (ddlBrand.SelectedIndex > 0 && ddlPosNo.SelectedIndex > 0 && ddlProdCategory.SelectedIndex > 0 && ddlState.SelectedIndex > 0)
                {
                    CampaignProduct product = CampaignProductProvider.GetCampaignProducts().WhereEquals("NodeSiteID", CurrentSite.SiteID).WhereEquals("CampaignProductID", productId).FirstOrDefault();
                    if (product != null)
                    {

                        //// Sets the properties of the new page
                        product.DocumentCulture = DocumentContext.CurrentDocument.DocumentCulture;
                        product.SetValue("BundleQty", ValidationHelper.GetString(txtBundleQnt.Text, string.Empty));
                        product.SetValue("POSNumber", ValidationHelper.GetInteger(ddlPosNo.SelectedValue, 0));
                        product.SetValue("BrandID", ValidationHelper.GetInteger(ddlBrand.SelectedValue, 0));
                        product.SetValue("EstimatedPrice", ValidationHelper.GetString(txtEstPrice.Text, string.Empty));
                        product.SetValue("ActualPrice", ValidationHelper.GetString(txtActualPrice.Text, string.Empty));
                        product.SetValue("ShortDescription", ValidationHelper.GetString(txtShortDes.Text, string.Empty));
                        product.SetValue("State", ValidationHelper.GetString(ddlState.SelectedValue, string.Empty));
                        product.SetValue("ExpirationDate", ValidationHelper.GetDate(txtExpDate.Text, DateTime.Now));
                        product.SetValue("CategoryID", ValidationHelper.GetInteger(ddlProdCategory.SelectedValue, 0));
                        product.SetValue("Cancelled", ValidationHelper.GetBoolean(chkcancel.Checked, false));
                        product.SetValue("CVOProductID", ValidationHelper.GetString(txtCVOProductId.Text, string.Empty));
                        product.SetValue("TotalQty", ValidationHelper.GetString(txtQuantity.Text, string.Empty));
                        product.SetValue("Status", ValidationHelper.GetBoolean(ddlStatus.SelectedValue, false));
                        product.SetValue("StorefrontProductID", ValidationHelper.GetString(txtStroeFrontId.Text, string.Empty));
                        product.SetValue("LongDescription", ValidationHelper.GetString(txtLongDes.Text, string.Empty));

                        if (productImage.HasFile)
                        {
                            if (product.Image != default(Guid))
                            {
                                UploadImage.DeleteImage(product.Image, libraryFolderName);
                            }
                            imageGuid = UploadImage.UploadImageToMeadiaLibrary(productImage, libraryFolderName);
                            product.Image = imageGuid;
                        }
                        //// Inserts the new page as a child of the parent page
                        product.Update();
                        var productID = ValidationHelper.GetInteger(product.GetValue("CampaignProductID"), 0);
                        UpdateAllocateProduct(productID);
                        lblSuccessMsg.Visible = true;
                        lblFailureText.Visible = false;
                        EmptyFields();
                    }
                    else
                    {
                        lblFailureText.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("SaveProductFromButtonClick", "EXCEPTION", ex);
                if (imageGuid != default(Guid))
                {
                    UploadImage.DeleteImage(imageGuid, libraryFolderName);
                }
                EventLogProvider.LogInformation("CMSWebParts_Kadena_Add_inventory_Products", "btnSave_Click", ex.Message);
            }
        }
        #endregion
        #region PrivateMethods
        /// <summary>
        /// This method will save the allocated product w.r.t to User in custome tabel
        /// </summary>
        /// <param name="productID">The id of the product which user wants to Allocate</param>
        private void AllocateProductToUsers(int productID)
        {
            string customTableClassName = "KDA.UserAllocatedProducts";
            DataClassInfo customTable = DataClassInfoProvider.GetDataClassInfo(customTableClassName);
            if (customTable != null)
            {
                // Creates a new custom table item
                CustomTableItem newCustomTableItem = CustomTableItem.New(customTableClassName);

                // Sets the values for the fields of the custom table (ItemText in this case)
                foreach (AllocateProduct User in lstUsers)
                {
                    newCustomTableItem.SetValue("UserID", User.UserID);
                    newCustomTableItem.SetValue("ProductID", productID);
                    newCustomTableItem.SetValue("Quantity", User.Quantity);
                    newCustomTableItem.SetValue("EmailID", User.EmailID);
                    // Save the new custom table record into the database
                    newCustomTableItem.Insert();
                }
            }
        }
        /// <summary>
        /// Update the Product allocation
        /// </summary>
        /// <param name="productID">The id of the product which user wants to Update</param>
        private void UpdateAllocateProduct(int productID)
        {
            try
            {
                string customTableClassName = "KDA.UserAllocatedProducts";
                DataClassInfo customTable = DataClassInfoProvider.GetDataClassInfo(customTableClassName);
                if (customTable != null)
                {
                    var customTableData = CustomTableItemProvider.GetItems(customTableClassName).WhereStartsWith("ProductID", productID.ToString());

                    // Sets the values for the fields of the custom table (ItemText in this case)
                    foreach (CustomTableItem customitem in customTableData)
                    {
                        int index = lstUsers.FindIndex(item => item.UserID == ValidationHelper.GetInteger(customitem.GetValue("UserID"), 0));
                        if (index > -1)              //If Item is selected
                        {
                            customitem.SetValue("Quantity", lstUsers[index].Quantity);
                            customitem.Update();
                            lstUsers.RemoveAt(index);
                        }
                        else    //If item is not selected
                        {
                            customitem.Delete();
                            lstUsers.RemoveAt(index);
                        }

                    }
                }
                CustomTableItem newCustomTableItem = CustomTableItem.New(customTableClassName);

                // Sets the values for the fields of the custom table (ItemText in this case)
                foreach (AllocateProduct User in lstUsers)
                {
                    newCustomTableItem.SetValue("UserID", User.UserID);
                    newCustomTableItem.SetValue("ProductID", productID);
                    newCustomTableItem.SetValue("Quantity", User.Quantity);
                    newCustomTableItem.SetValue("EmailID", User.EmailID);
                    // Save the new custom table record into the database
                    newCustomTableItem.Insert();
                }

            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("Product allocation update", "EXCEPTION", ex);
            }

        }
       
        /// <summary>
        /// Set the field with the record which user wants to edit
        /// </summary>
        /// <param name="productid">The id of the product which user wants to edit</param>
        private void SetFeild(int productid)
        {
            Guid imageGuid = default(Guid);
            try
            {
                CampaignProduct product = CampaignProductProvider.GetCampaignProducts().WhereEquals("NodeSiteID", CurrentSite.SiteID).WhereEquals("CampaignProductID", productid).FirstOrDefault();
                if (product != null)
                {
                    // Sets the properties of the new page
                    product.DocumentCulture = DocumentContext.CurrentDocument.DocumentCulture;
                    txtBundleQnt.Text = product.GetValue("BundleQty", string.Empty);
                    ddlPosNo.SelectedValue = product.GetValue("POSNumber", string.Empty);
                    ddlBrand.SelectedValue = product.GetValue("BrandID", string.Empty);
                    txtEstPrice.Text = product.GetValue("EstimatedPrice", string.Empty);
                    txtLongDes.Text = product.GetValue("LongDescription", string.Empty);
                    txtActualPrice.Text = product.GetValue("ActualPrice", string.Empty);
                    txtShortDes.Text = product.GetValue("ShortDescription", string.Empty);
                    ddlState.SelectedValue = product.GetValue("State", string.Empty);
                    txtExpDate.Text = product.GetValue("ExpirationDate", string.Empty);
                    ddlProdCategory.SelectedValue = product.GetValue("CategoryID", string.Empty);
                    chkcancel.Checked = product.GetBooleanValue("Cancelled", false);
                    txtCVOProductId.Text = product.GetValue("CVOProductID", string.Empty);
                    txtQuantity.Text = product.GetValue("TotalQty", string.Empty);
                    ddlStatus.SelectedValue = product.GetValue("Status", string.Empty);
                    txtStroeFrontId.Text = product.GetValue("StorefrontProductID", string.Empty);
                    if (product.Image != default(Guid) && !product.Image.Equals(Guid.Empty))
                    {
                        MediaFileInfo image = MediaFileInfoProvider.GetMediaFileInfo(product.Image, SiteContext.CurrentSiteName);
                        if (image != null)
                        {
                            imgProduct.ImageUrl = MediaFileURLProvider.GetMediaFileAbsoluteUrl(product.Image, image.FileName);
                            imgProduct.Visible = true;

                        }
                    }

                    BindEditProduct(ValidationHelper.GetInteger(product.GetValue("CampaignProductID"), 0));
                }
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("GetProductFromButtonClick", "EXCEPTION", ex);
            }
        }
    
        /// <summary>
        /// Empty all the field of the form
        /// </summary>
        private void EmptyFields()
        {
            ddlBrand.SelectedIndex = 0;
            ddlPosNo.SelectedIndex = 0;
            ddlProdCategory.SelectedIndex = 0;
            ddlState.SelectedIndex = 0;
            ddlStatus.SelectedIndex = 0;
            txtActualPrice.Text = string.Empty;
            txtBundleQnt.Text = string.Empty;
            txtCVOProductId.Text = string.Empty;
            txtEstPrice.Text = string.Empty;
            txtExpDate.Text = string.Empty;
            txtLongDes.Text = string.Empty;
            txtQuantity.Text = string.Empty;
            txtShortDes.Text = string.Empty;
            txtStroeFrontId.Text = string.Empty;
            RepSelectedUser.DataSource = string.Empty;
            RepSelectedUser.DataBind();
            lstUsers = new List<Product.AllocateProduct>();
        }
        /// <summary>
        /// This method will get all the users and bind it to repeater
        /// </summary>
        /// <param name="pageIndex"></param>
            private void BindUsers(int pageIndex)
            {
            List<AllocateProduct> lstAllocatedProd = new List<AllocateProduct>();
            var users = UserInfoProvider.GetUsers().Columns("Email", "UserID", "FullName").Skip(PageSize * (pageIndex - 1)).Take(PageSize);
            foreach (UserInfo user in users)
            {
                AllocateProduct objProduct = new AllocateProduct();
                objProduct.EmailID = user.Email;
                objProduct.UserID = user.UserID;
                objProduct.UserName = user.FullName;
                if (lstUsers.FindIndex(item => item.UserID == user.UserID) > -1)
                {
                    objProduct.Selected = true;
                }
                lstAllocatedProd.Add(objProduct);
            }
            RepterDetails.DataSource = lstAllocatedProd;
            RepterDetails.DataBind();
            PopulatePager(UserInfoProvider.GetUsers().Count(), pageIndex);
        }
        /// <summary>
        /// This method will create pagination for repeater
        /// </summary>
        /// <param name="recordCount">The no of record to fetch</param>
        /// <param name="currentPage">Current page no</param>
       
        private void PopulatePager(int recordCount, int currentPage)
        {
            double dblPageCount = (double)((decimal)recordCount / Convert.ToDecimal(PageSize));
            int pageCount = (int)Math.Ceiling(dblPageCount);
            List<ListItem> pages = new List<ListItem>();
            if (pageCount > 0)
            {
                for (int i = 1; i <= pageCount; i++)
                {
                    pages.Add(new ListItem(i.ToString(), i.ToString(), i != currentPage));
                }
            }
            rptPager.DataSource = pages;
            rptPager.DataBind();
        }
        /// <summary>
        ///  This method will get all the allocated user
         ////    and checked it in repeater
        /// </summary>
        /// <param name="ProductId"></param>

        private void BindEditProduct(int ProductId)
        {
            try
            {
                lstUsers = new List<AllocateProduct>();
                List<AllocateProduct> lstProduct = new List<AllocateProduct>();
                string customTableClassName = "KDA.UserAllocatedProducts";
                DataClassInfo customTable = DataClassInfoProvider.GetDataClassInfo(customTableClassName);
                if (customTable != null)
                {
                    var customTableData = CustomTableItemProvider.GetItems(customTableClassName)
                                                             .WhereStartsWith("ProductID", ProductId.ToString());
                    foreach (CustomTableItem item in customTableData)
                    {
                        AllocateProduct objProduct = new AllocateProduct();
                        objProduct.EmailID = ValidationHelper.GetString(item.GetValue("EmailID"), string.Empty);
                        objProduct.UserName = UserInfoProvider.GetUserInfo(ValidationHelper.GetInteger(item.GetValue("UserID"), 0)).FullName;
                        objProduct.Quantity = ValidationHelper.GetInteger(item.GetValue("Quantity"), 0);
                        objProduct.UserID = ValidationHelper.GetInteger(item.GetValue("UserID"), 0);
                        lstProduct.Add(objProduct);


                    }
                    lstUsers.AddRange(lstProduct);
                }
                RepSelectedUser.DataSource = lstProduct;
                RepSelectedUser.DataBind();
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("Product allocation update BindEdit", "EXCEPTION", ex);
            }
        }
        /// <summary>
        ///  Method to bind the data to the dropdowns
        /// </summary>
        private void BindData()
        {
            try
            {
                //Binding data to Brand dropdownlist
                ddlBrand.Items.Insert(0, new ListItem(ResHelper.GetString("Kadena.InvProductForm.BrandWaterMark"), "0"));
                ObjectQuery<CustomTableItem> Brands = GetBrands();
                int brandindex = 1;
                foreach (CustomTableItem Brand in Brands)
                {
                    ddlBrand.Items.Insert(brandindex++, new ListItem(Brand.GetValue("BrandName").ToString(), Brand.GetValue("BrandCode").ToString()));
                }
                // Adds the '(any)' and '(default)' filtering options
                ddlPosNo.Items.Insert(0, new ListItem(ResHelper.GetString("Kadena.InvProductForm.PosNoWaterMark"), "0"));
                ObjectQuery<CustomTableItem> PosNumbers = GetPosNumber();
                int Posindex = 1;
                foreach (CustomTableItem PosNumber in PosNumbers)
                {
                    ddlPosNo.Items.Insert(Posindex++, new ListItem(PosNumber.GetValue("POSNumber").ToString(), PosNumber.GetValue("ItemID").ToString()));
                }
                //Product Category DropdownList  
                int ProdCategoryindex = 1;
                ddlProdCategory.Items.Insert(0, new ListItem(ResHelper.GetString("Kadena.InvProductForm.CategoryWaterMark"), "0"));
                List<ProductCategory> lstProdcategories = GetProductCategory();
                foreach (ProductCategory lstProdcategory in lstProdcategories)
                {
                    ddlProdCategory.Items.Insert(ProdCategoryindex++, new ListItem(lstProdcategory.CategoryName, lstProdcategory.CategoryId.ToString()));
                }
               
                //Bind all the state to dropdown
                // Gets the state
                ObjectQuery<StateInfo> States = StateInfoProvider.GetStates();
                ddlState.Items.Insert(0, new ListItem(ResHelper.GetString("Kadena.InvProductForm.StateWaterMark"), "0"));
                int Stateindex = 1;
                foreach (StateInfo State in States)
                {
                    ddlState.Items.Insert(Stateindex++, new ListItem(State.StateDisplayName, State.StateCode));
                }

            }
            catch (Exception ex)
            {
                EventLogProvider.LogInformation("CMSWebParts_Kadena_POS_POSForm_BindDataToDropdowns", "BindData", ex.Message);
            }
        }
        /// <summary>
        /// Method to return the list of product categories
        /// </summary>
        /// <returns></returns>
        private List<ProductCategory> GetProductCategory()
        {
            // Creates an instance of the Tree provider
            List<ProductCategory> lstProdcategroy = new List<ProductCategory>();
            TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
            var pages = tree.SelectNodes("KDA.ProductCategory");
            foreach (CMS.DocumentEngine.TreeNode page in pages)
            {
                ProductCategory category = new ProductCategory();
                category.CategoryId = page.GetValue("ProductCategoryID", 0);
                category.CategoryName = page.GetValue("ProductCategoryTitle", string.Empty);
                lstProdcategroy.Add(category);
            }
            return lstProdcategroy;
        }
      
        /// <summary>
        /// This method will return the POSNUmber list
        /// </summary>
        /// <returns>POS Number</returns>
        private static ObjectQuery<CustomTableItem> GetPosNumber()
        {

            // Prepares the code name (class name) of the custom table
            ObjectQuery<CustomTableItem> items = new ObjectQuery<CustomTableItem>();
            string customTableClassName = "KDA.POSNumber";
            try
            {
                // Gets the custom table
                DataClassInfo PosTable = DataClassInfoProvider.GetDataClassInfo(customTableClassName);
                if (PosTable != null)
                {
                    // Gets a custom table records 
                    items = CustomTableItemProvider.GetItems(customTableClassName).OrderBy("POSNumber");

                }
            }
            catch (Exception ex)
            {
                EventLogProvider.LogInformation("CMSWebParts_Kadena_Product_ProductForm", "GetPosNumber", ex.Message);
            }

            return items;
        }
        /// <summary>
        /// This method will return the Brand list and bind it to Brand dropdowns
        /// </summary>
        /// <returns>The list of Brand</returns>
        private static ObjectQuery<CustomTableItem> GetBrands()
        {

            // Prepares the code name (class name) of the custom table
            ObjectQuery<CustomTableItem> items = new ObjectQuery<CustomTableItem>();
            string customTableClassName = "KDA.Brand";
            try
            {
                // Gets the custom table
                DataClassInfo brandTable = DataClassInfoProvider.GetDataClassInfo(customTableClassName);
                if (brandTable != null)
                {
                    // Gets a custom table records 
                    items = CustomTableItemProvider.GetItems(customTableClassName).OrderBy("BrandName");
                }
            }
            catch (Exception ex)
            {
                EventLogProvider.LogInformation("CMSWebParts_Kadena_POS_ProductForm", "GetBrands", ex.Message);
            }

            return items;
        }

        #endregion

    }
    #region class
    /// <summary>
    /// Properties for Product category
    /// </summary>
    public class ProductCategory
    {
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
    }
    /// <summary>
    /// Properties related to product allocation
    /// </summary>
    public class AllocateProduct
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string EmailID { get; set; }
        public bool Selected { get; set; }
        public int Quantity { get; set; }
    }
    #endregion
}