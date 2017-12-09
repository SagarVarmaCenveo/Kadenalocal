using CMS.CustomTables;
using CMS.CustomTables.Types.KDA;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.EventLog;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using System;
using System.Linq;
using System.Web.UI.WebControls;

public partial class CMSWebParts_Kadena_Address_CreateAddress : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Selected country ID.
    /// </summary>
    public int CountryID
    {
        get
        {
            return ValidationHelper.GetInteger(uniSelectorCountry.Value, 0);
        }
        set
        {
            if (value > 0)
            {
                uniSelectorCountry.Value = value;
                uniSelectorState.WhereCondition = "CountryID = " + value;
            }
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
        if (!this.StopProcessing)
        {
            if (AuthenticationHelper.IsAuthenticated() && !IsPostBack)
            {
                BindResourceStrings();
                var countryInfo = CountryInfoProvider.GetCountryInfo((SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".KDA_AddressDefaultCountry")));
                CountryID = countryInfo != null ? countryInfo.CountryID : 0;
                uniSelectorCountry.Value = ValidationHelper.GetString(CountryID, string.Empty);
                uniSelectorState.WhereCondition = "CountryID =" + CountryID;
                uniSelectorState.Enabled = true;
                int itemID = QueryHelper.GetInteger("id", 0);
                if (itemID > 0)
                {
                    BindAddressData(itemID);
                }
            }
        }
    }

    /// <summary>
    ///  Binds all resource strings
    /// </summary>
    private void BindResourceStrings()
    {
        try
        {
            lblName.InnerText = ResHelper.GetString("Kadena.Address.Name");
            lblAddressType.InnerText = ResHelper.GetString("Kadena.Address.AddressType");
            lblCompany.InnerText = ResHelper.GetString("Kadena.Address.Company");
            lblAddressLine1.InnerText = ResHelper.GetString("Kadena.Address.AddressLine1");
            lblAddressLine2.InnerText = ResHelper.GetString("Kadena.Address.AddressLine2");
            lblCity.InnerText = ResHelper.GetString("Kadena.Address.City");
            lblCountry.InnerText = ResHelper.GetString("Kadena.Address.Country");
            lblZipcode.InnerText = ResHelper.GetString("Kadena.Address.Zipcode");
            lblTelephone.InnerText = ResHelper.GetString("Kadena.Address.Telephone");
            lblEmail.InnerText = ResHelper.GetString("Kadena.Address.Email");
            rfName.ErrorMessage = ResHelper.GetString("Kadena.Address.NameRequired");
            rfAddressLine1.ErrorMessage = ResHelper.GetString("Kadena.Address.Line1Required");
            rfCity.ErrorMessage = ResHelper.GetString("Kadena.Address.CityRequired");
            rfEmail.ErrorMessage = ResHelper.GetString("Kadena.Address.EmailRequired");
            rfZipcode.ErrorMessage = ResHelper.GetString("Kadena.Address.ZipcodeRequired");
            btnSave.Text = Request.QueryString["id"] != null ? ResHelper.GetString("Kadena.Address.Update") : ResHelper.GetString("Kadena.Address.Save");
            btnCancel.Text = ResHelper.GetString("Kadena.Address.Cancel");
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("CreateAddress.ascx.cs", "BindResourceStrings()", ex);
        }
    }

    /// <summary>
    ///  Updates the existing address data
    /// </summary>
    /// <param name="itemID">item id of the address data</param>
    private void UpdateAddressData(int customerID)
    {
        try
        {
            var addressObj = BindAddressObject(customerID);
            if (!DataHelper.DataSourceIsEmpty(addressObj) && addressObj != null)
            {
                AddressInfoProvider.SetAddressInfo(addressObj);
                var shippingObj = BindShippingAddressObject(addressObj);
                if (!DataHelper.DataSourceIsEmpty(shippingObj) && shippingObj != null)
                {
                    shippingObj.Update();
                }
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("CreateAddress.ascx.cs", "UpdateAddressData()", ex);
        }
    }

    /// <summary>
    /// Binds the address data to controls
    /// </summary>
    /// <param name="itemID">item id of the address data</param>
    private void BindAddressData(int itemID)
    {
        try
        {
            AddressInfo addressData = AddressInfoProvider.GetAddresses()
                                                            .WhereEquals("AddressID", itemID)
                                                            .FirstOrDefault();
            if (!DataHelper.DataSourceIsEmpty(addressData))
            {
                txtAddressLine1.Text = addressData.AddressLine1;
                txtAddressLine2.Text = addressData.AddressLine2;
                txtCity.Text = addressData.AddressCity;
                txtZipcode.Text = addressData.AddressZip;
                txtName.Text = addressData.AddressPersonalName;
                txtTelephone.Text = addressData.AddressPhone;
                uniSelectorCountry.Value = addressData.AddressCountryID;
                uniSelectorState.Value = addressData.AddressStateID;
                txtEmail.Text = addressData.GetStringValue("Email", string.Empty);
                txtComapnyName.Text = addressData.GetStringValue("CompanyName", string.Empty);
                ddlAddressType.Value = addressData.GetStringValue("AddressTypeID", string.Empty);
                var shippingData = CustomTableItemProvider.GetItems<ShippingAddressItem>()
                                                            .WhereEquals("COM_AddressID", addressData.AddressID)
                                                            .FirstOrDefault();
                if (!DataHelper.DataSourceIsEmpty(shippingData))
                {
                    txtEmail.Text = shippingData.GetStringValue("Email", string.Empty);
                    txtComapnyName.Text = shippingData.GetStringValue("CompanyName", string.Empty);
                }
                if (addressData.AddressStateID <= 0)
                {
                    uniSelectorState.Enabled = false;
                }
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("CreateAddress.ascx.cs", "BindAddressData()", ex);
        }
    }

    /// <summary>
    /// Creates new address for a particular customer
    /// </summary>
    /// <param name="customerID">Customer id of the logged in user</param>
    private void CreateNewAddress(int customerID)
    {
        try
        {
            var objAddress = BindAddressObject(customerID);
            if (!DataHelper.DataSourceIsEmpty(objAddress) && objAddress != null)
            {
                AddressInfoProvider.SetAddressInfo(objAddress);
                var objShipping = BindShippingAddressObject(objAddress);
                if (!DataHelper.DataSourceIsEmpty(objShipping) && objShipping != null)
                {
                    objShipping.Insert();
                }
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("CreateAddress.ascx.cs", "CreateNewAddress()", ex);
        }
    }

    /// <summary>
    /// Binding the shipping address object
    /// </summary>
    /// <param name="objAddress">Address info object</param>
    /// <returns> ShippingAddressItem object</returns>

    private ShippingAddressItem BindShippingAddressObject(AddressInfo objAddress)
    {
        try
        {
            ShippingAddressItem item = new ShippingAddressItem();
            item.AddressTypeID = ValidationHelper.GetInteger(ddlAddressType.Value, default(int));
            item.COM_AddressID = objAddress.AddressID;
            item.UserID = CurrentUser.UserID;
            item.Email = ValidationHelper.GetString(txtEmail.Text, string.Empty);
            item.CompanyName = ValidationHelper.GetString(txtComapnyName.Text, string.Empty);
            int itemID = Request.QueryString["id"] != null ? ValidationHelper.GetInteger(Request.QueryString["id"], default(int)) : default(int);
            if (itemID != default(int))
            {
                var shippingData = CustomTableItemProvider.GetItems<ShippingAddressItem>()
                                                            .WhereEquals("COM_AddressID", objAddress.AddressID)
                                                            .FirstOrDefault();
                if (!DataHelper.DataSourceIsEmpty(shippingData) && shippingData != null)
                {
                    item.ItemID = shippingData.ItemID;
                }
            }
            return item;
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("CreateAddress.ascx.cs", "BindShippingAddressObject()", ex);
        }
        return null;
    }

    /// <summary>
    /// Binds address object from controls
    /// </summary>
    /// <param name="customerID"></param>
    /// <returns> AddressInfo object</returns>
    private AddressInfo BindAddressObject(int customerID)
    {
        try
        {
            int itemID = Request.QueryString["id"] != null ? ValidationHelper.GetInteger(Request.QueryString["id"], default(int)) : default(int);
            if (itemID != default(int))
            {
                var customerData = IsUserCustomer(CurrentUser.UserID);
                if (!DataHelper.DataSourceIsEmpty(customerData))
                {
                    var addressData = AddressInfoProvider.GetAddressInfo(itemID);
                    if (!DataHelper.DataSourceIsEmpty(addressData))
                    {
                        addressData.AddressLine1 = ValidationHelper.GetString(txtAddressLine1.Text.Trim(), string.Empty);
                        addressData.AddressLine2 = ValidationHelper.GetString(txtAddressLine2.Text.Trim(), string.Empty);
                        addressData.AddressCity = ValidationHelper.GetString(txtCity.Text.Trim(), string.Empty);
                        addressData.AddressZip = ValidationHelper.GetString(txtZipcode.Text.Trim(), string.Empty);
                        addressData.AddressName = string.Format("{0}{1}{2}", !string.IsNullOrEmpty(addressData.AddressLine1) ? addressData.AddressLine1 + "," : addressData.AddressLine1, !string.IsNullOrEmpty(addressData.AddressLine2) ? addressData.AddressLine2 + "," : addressData.AddressLine2, addressData.AddressCity);
                        addressData.AddressPhone = ValidationHelper.GetString(txtTelephone.Text.Trim(), string.Empty);
                        addressData.AddressPersonalName = ValidationHelper.GetString(txtName.Text.Trim(), string.Empty);
                        addressData.AddressCountryID = ValidationHelper.GetInteger(uniSelectorCountry.Value, 0);
                        addressData.AddressStateID = ValidationHelper.GetInteger(uniSelectorState.Value, 0);
                        addressData.SetValue("AddressType", ddlAddressType.ValueDisplayName);
                        addressData.SetValue("Email", txtEmail.Text.Trim());
                        addressData.SetValue("CompanyName", txtComapnyName.Text.Trim());
                        addressData.SetValue("AddressTypeID", ddlAddressType.Value);
                        return addressData;
                    }
                }
            }
            else
            {
                AddressInfo objAddress = new AddressInfo();
                objAddress.AddressLine1 = ValidationHelper.GetString(txtAddressLine1.Text.Trim(), string.Empty);
                objAddress.AddressLine2 = ValidationHelper.GetString(txtAddressLine2.Text.Trim(), string.Empty);
                objAddress.AddressCity = ValidationHelper.GetString(txtCity.Text.Trim(), string.Empty);
                objAddress.AddressZip = ValidationHelper.GetString(txtZipcode.Text.Trim(), string.Empty);
                objAddress.AddressCustomerID = customerID;
                objAddress.AddressName = string.Format("{0}{1}{2}", !string.IsNullOrEmpty(objAddress.AddressLine1) ? objAddress.AddressLine1 + "," : objAddress.AddressLine1,
                    !string.IsNullOrEmpty(objAddress.AddressLine2) ? objAddress.AddressLine2 + "," : objAddress.AddressLine2, objAddress.AddressCity);
                objAddress.AddressPhone = ValidationHelper.GetString(txtTelephone.Text.Trim(), string.Empty);
                objAddress.AddressPersonalName = ValidationHelper.GetString(txtName.Text.Trim(), string.Empty);
                objAddress.AddressCountryID = ValidationHelper.GetInteger(uniSelectorCountry.Value, 0);
                objAddress.AddressStateID = ValidationHelper.GetInteger(uniSelectorState.Value, 0);
                objAddress.SetValue("AddressType", ddlAddressType.ValueDisplayName);
                objAddress.SetValue("Email", txtEmail.Text.Trim());
                objAddress.SetValue("CompanyName", txtComapnyName.Text.Trim());
                objAddress.SetValue("AddressTypeID", ddlAddressType.Value);
                return objAddress;
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("CreateAddress.ascx.cs", "BindAddressObject()", ex);
        }
        return null;
    }

    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }

    #endregion "Methods"

    #region methods

   /// <summary>
    /// Create cusotmer based on  logged in user details
    /// </summary>
    /// <returns>Customer id</returns>
    private int CreateCustomer()
    {
        try
        {
            CustomerInfo objCustomer = new CustomerInfo();
            objCustomer.CustomerUserID = CurrentUser.UserID;
            objCustomer.CustomerEmail = CurrentUser.Email;
            objCustomer.CustomerFirstName = CurrentUser.FirstName;
            objCustomer.CustomerLastName = CurrentUser.LastName;
            objCustomer.CustomerSiteID = CurrentSite.SiteID;
            CustomerInfoProvider.SetCustomerInfo(objCustomer);
            return objCustomer != null ? objCustomer.CustomerID : default(int);
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("CreateAddress.ascx.cs", "CreateCustomer()", ex);
        }
        return default(int);
    }

    /// <summary>
    /// Checks whether the suer is already a customer or not
    /// </summary>
    /// <param name="userID">user id</param>
    /// <returns>customer id</returns>
    private int IsUserCustomer(int userID)
    {
        try
        {
            CustomerInfo customer = CustomerInfoProvider.GetCustomers()
                .WhereEquals("CustomerUserID", userID)
                .FirstOrDefault();
            if (!DataHelper.DataSourceIsEmpty(customer))
            {
                return customer.CustomerID;
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("CreateAddress.ascx.cs", "CreateCustomer()", ex);
        }
        return default(int);
    }

    #endregion methods

    /// <summary>
    /// Creates and updates address data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            int itemID = Request.QueryString["id"] != null ? ValidationHelper.GetInteger(Request.QueryString["id"], default(int)) : default(int);
            var customerID = IsUserCustomer(CurrentUser.UserID);
            if (itemID != default(int))
            {
                UpdateAddressData(itemID);
            }
            else
            {
                if (customerID != default(int))
                {
                    CreateNewAddress(customerID);
                }
                else
                {
                    customerID = CreateCustomer();
                    CreateNewAddress(customerID);
                }
            }
            URLHelper.Redirect(CurrentDocument.Parent.DocumentUrlPath);
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("CreateAddress.ascx.cs", "btnSave_Click()", ex);
        }
    }

    /// <summary>
    /// Redirects to listing page
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        URLHelper.Redirect(CurrentDocument.Parent.DocumentUrlPath);
    }

    /// <summary>
    /// /validates the length of the mobile number
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    protected void cvTelephone_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            args.IsValid = string.IsNullOrEmpty(txtTelephone.Text.Trim()) ? true : txtTelephone.Text.Trim().Length >= 10 && txtTelephone.Text.Trim().Length
                <= 25 ? true : false;
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("CreateAddress.ascx.cs", "cvTelephone_ServerValidate()", ex);
        }
    }

    protected void uniSelectorCountry_OnSelectionChanged(object sender, EventArgs e)
    {
        if (CountryID > 0)
        {
            uniSelectorState.Enabled = true;
            uniSelectorState.WhereCondition = "CountryID = " + CountryID;
            uniSelectorState.StopProcessing = false;
            uniSelectorState.Reload(true);
        }
        else
        {
            uniSelectorState.StopProcessing = true;
            uniSelectorState.Enabled = false;
        }
    }
}