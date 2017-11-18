using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.PortalEngine.Web.UI;
using CMS.Helpers;
using CMS.Ecommerce;
using CMS.CustomTables.Types.KDA;
using CMS.Globalization;
using System.Linq;
using CMS.Membership;
using CMS.CustomTables;
using CMS.EventLog;

public partial class CMSWebParts_Kadena_Address_CreateAddress : CMSAbstractWebPart
{

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
            // Do not process
        }
        else
        {
            if (AuthenticationHelper.IsAuthenticated())
            {
                ddlCountry.Value = "USA";
                BindResourceStrings();
                int itemID = QueryHelper.GetInteger("id", 0);
                if (itemID > 0)
                    BindAddressData(itemID);
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
            //Binding label text
            lblName.InnerText = ResHelper.GetString("Kadena.Address.Name");
            lblAddressType.InnerText = ResHelper.GetString("Kadena.Address.AddressType");
            lblCompany.InnerText = ResHelper.GetString("Kadena.Address.Company");
            lblAddressLine1.InnerText = ResHelper.GetString("Kadena.Address.AddressLine1");
            lblAddressLine2.InnerText = ResHelper.GetString("Kadena.Address.AddressLine2");
            lblCity.InnerText = ResHelper.GetString("Kadena.Address.City");
            lblCountry.InnerText = ResHelper.GetString("Kadena.Address.Country");
            //ResHelper.GetString("Kadena.Address.State");
            lblZipcode.InnerText = ResHelper.GetString("Kadena.Address.Zipcode");
            lblTelephone.InnerText = ResHelper.GetString("Kadena.Address.Telephone");
            lblEmail.InnerText = ResHelper.GetString("Kadena.Address.Email");

            //Binding error labels
            rfName.ErrorMessage = ResHelper.GetString("Kadena.Address.NameRequired");
            rfAddressLine1.ErrorMessage = ResHelper.GetString("Kadena.Address.Line1Required");
            rfCity.ErrorMessage = ResHelper.GetString("Kadena.Address.CityRequired");
            rfEmail.ErrorMessage = ResHelper.GetString("Kadena.Address.EmailRequired");
            rfZipcode.ErrorMessage = ResHelper.GetString("Kadena.Address.ZipcodeRequired");


            //Binding button text
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
            if (addressObj != null)
            {
                AddressInfoProvider.SetAddressInfo(addressObj);
                var shippingObj = BindShippingAddressObject(addressObj);
                if (shippingObj != null)
                    shippingObj.Update();
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
            var addressData = AddressInfoProvider.GetAddresses().WhereEquals("AddressID", itemID).TopN(1).FirstOrDefault();
            if (!DataHelper.DataSourceIsEmpty(addressData))
            {
                txtAddressLine1.Text = addressData.AddressLine1;
                txtAddressLine2.Text = addressData.AddressLine2;
                txtCity.Text = addressData.AddressCity;
                txtZipcode.Text = addressData.AddressZip;
                txtName.Text = addressData.AddressPersonalName;
                txtTelephone.Text = addressData.AddressPhone;
                ddlCountry.Value = GetCountryName(addressData.AddressCountryID) + ";" + GetStateName(addressData.AddressStateID);

                ddlAddressType.Value = addressData.GetValue("AddressType", string.Empty);

                var shippingData = CustomTableItemProvider.GetItems<ShippingAddressItem>().WhereEquals("COM_AddressID", addressData.AddressID).TopN(1).FirstOrDefault();
                if (!DataHelper.DataSourceIsEmpty(shippingData))
                {
                    txtEmail.Text = shippingData.GetStringValue("Email", string.Empty);
                    txtComapnyName.Text = shippingData.GetStringValue("CompanyName", string.Empty);
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
            if (objAddress != null)
            {
                AddressInfoProvider.SetAddressInfo(objAddress);
                var objShipping = BindShippingAddressObject(objAddress);
                if (objShipping != null)
                    objShipping.Insert();
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
                var shippingData = CustomTableItemProvider.GetItems<ShippingAddressItem>().WhereEquals("COM_AddressID", objAddress.AddressID).TopN(1).FirstOrDefault();
                item.ItemID = shippingData.ItemID;
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
                        addressData.AddressLine1 = ValidationHelper.GetString(txtAddressLine1.Text, string.Empty);
                        addressData.AddressLine2 = ValidationHelper.GetString(txtAddressLine2.Text, string.Empty);
                        addressData.AddressCity = ValidationHelper.GetString(txtCity.Text, string.Empty);
                        addressData.AddressZip = ValidationHelper.GetString(txtZipcode.Text, string.Empty);
                        //addressData.AddressCustomerID = customerID;
                        addressData.AddressName = string.Format("{0}{1}{2}", !string.IsNullOrEmpty(addressData.AddressLine1) ? addressData.AddressLine1 + "," : addressData.AddressLine1, !string.IsNullOrEmpty(addressData.AddressLine2) ? addressData.AddressLine2 + "," : addressData.AddressLine2, addressData.AddressCity);

                        addressData.AddressPhone = ValidationHelper.GetString(txtTelephone.Text, string.Empty);

                        addressData.AddressPersonalName = ValidationHelper.GetString(txtName.Text, string.Empty);

                        var country = ddlCountry.Value != null ? ddlCountry.Value.ToString() : string.Empty;
                        addressData.AddressCountryID = !string.IsNullOrEmpty(country) ? GetCountryID(country.Split(';').First()) : default(int);
                        addressData.AddressStateID = !string.IsNullOrEmpty(country) ? GetStateID(country.Split(';').Last()) : default(int);
                        addressData.SetValue("AddressTypeID", ddlAddressType.Value);
                        return addressData;
                    }
                }

            }
            else
            {
                AddressInfo objAddress = new AddressInfo();

                objAddress.AddressLine1 = ValidationHelper.GetString(txtAddressLine1.Text, string.Empty);
                objAddress.AddressLine2 = ValidationHelper.GetString(txtAddressLine2.Text, string.Empty);
                objAddress.AddressCity = ValidationHelper.GetString(txtCity.Text, string.Empty);
                objAddress.AddressZip = ValidationHelper.GetString(txtZipcode.Text, string.Empty);
                objAddress.AddressCustomerID = customerID;
                objAddress.AddressName = string.Format("{0}{1}{2}", !string.IsNullOrEmpty(objAddress.AddressLine1) ? objAddress.AddressLine1 + "," : objAddress.AddressLine1,
                    !string.IsNullOrEmpty(objAddress.AddressLine2) ? objAddress.AddressLine2 + "," : objAddress.AddressLine2, objAddress.AddressCity);

                objAddress.AddressPhone = ValidationHelper.GetString(txtTelephone.Text, string.Empty);

                objAddress.AddressPersonalName = ValidationHelper.GetString(txtName.Text, string.Empty);

                var country = ddlCountry.Value != null ? ddlCountry.Value.ToString() : string.Empty;
                objAddress.AddressCountryID = !string.IsNullOrEmpty(country) ? GetCountryID(country.Split(';').First()) : default(int);
                objAddress.AddressStateID = !string.IsNullOrEmpty(country) ? GetStateID(country.Split(';').Last()) : default(int);
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

    #endregion

    #region methods

    /// <summary>
    /// Gets the country id from name
    /// </summary>
    /// <param name="countryName">name of the country</param>
    /// <returns>Country id</returns>
    private int GetCountryID(string countryName)
    {
        try
        {
            var countryData = CountryInfoProvider.GetCountries().WhereEquals("CountryDisplayName", countryName).TopN(1).FirstOrDefault();
            if (!DataHelper.DataSourceIsEmpty(countryData)) return countryData.CountryID;
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("CreateAddress.ascx.cs", "GetCountryID()", ex);
        }
        return default(int);
    }

    /// <summary>
    /// Gets the state id from name
    /// </summary>
    /// <param name="stateName">name of the state</param>
    /// <returns>State id id</returns>
    private int GetStateID(string stateName)
    {
        try
        {
            var stateData = StateInfoProvider.GetStates().WhereEquals("StateDisplayName", stateName).TopN(1).FirstOrDefault();
            if (!DataHelper.DataSourceIsEmpty(stateData)) return stateData.StateID;
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("CreateAddress.ascx.cs", "GetStateID()", ex);
        }
        return default(int);

    }

    /// <summary>
    /// Gets the country name
    /// </summary>
    /// <param name="countryID">id of the country</param>
    /// <returns>Country anme</returns>
    private string GetCountryName(int countryID)
    {
        try
        {
            var countryData = CountryInfoProvider.GetCountries().WhereEquals("CountryID", countryID).Column("CountryDisplayName").TopN(1).FirstOrDefault();
            if (!DataHelper.DataSourceIsEmpty(countryData)) return countryData.CountryDisplayName;

        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("CreateAddress.ascx.cs", "GetCountryName()", ex);
        }
        return string.Empty;
    }

    /// <summary>
    /// Gets the state name from id
    /// </summary>
    /// <param name="stateID">state id</param>
    /// <returns>state name</returns>
    private string GetStateName(int stateID)
    {
        try
        {
            var stateData = StateInfoProvider.GetStates().WhereEquals("StateID", stateID).Column("StateDisplayName").TopN(1).FirstOrDefault();
            if (!DataHelper.DataSourceIsEmpty(stateData)) return stateData.StateDisplayName;
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("CreateAddress.ascx.cs", "GetStateName()", ex);
        }
        return string.Empty;
    }

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
            objCustomer.CustomerLastName = CurrentUser.FirstName;
            objCustomer.CustomerSiteID = CurrentSite.SiteID;
            CustomerInfoProvider.SetCustomerInfo(objCustomer);
            return objCustomer.CustomerID;
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
                                                    .WhereEquals("CustomerUserID", userID).FirstOrDefault();
            if (!DataHelper.DataSourceIsEmpty(customer)) return customer.CustomerID;
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("CreateAddress.ascx.cs", "CreateCustomer()", ex);
        }
        return default(int);
    }

    #endregion
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
                UpdateAddressData(itemID);
            else
            {
                if (customerID != default(int))
                    CreateNewAddress(customerID);
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
            if (string.IsNullOrEmpty(txtTelephone.Text)) args.IsValid = true;
            else
            {
                args.IsValid = txtTelephone.Text.Length >= 10 && txtTelephone.MaxLength <= 25 ? true : false;
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("CreateAddress.ascx.cs", "cvTelephone_ServerValidate()", ex);
        }
    }
}



