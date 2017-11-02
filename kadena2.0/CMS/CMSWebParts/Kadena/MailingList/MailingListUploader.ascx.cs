﻿using CMS.CustomTables;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.EventLog;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using Kadena.Dto.MailingList.MicroserviceResponses;
using Kadena2.MicroserviceClients;
using Kadena2.MicroserviceClients.Clients;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;

namespace Kadena.CMSWebParts.Kadena.MailingList
{
    public partial class MailingListUploader : CMSAbstractWebPart
    {
        private readonly string _mailTypeTableName = "KDA.MailingType";
        private readonly string _productTableName = "KDA.MailingProductType";
        private readonly string _validityTableName = "KDA.MailingValidity";
        private readonly string _fileServiceUrlSettingKey = "KDA_FileServiceUrl";
        private readonly string _mailingServiceUrlSettingKey = "KDA_MailingServiceUrl";
        private MailingListDataDTO _container;

        public string RedirectPage
        {
            get
            {
                return GetStringValue("RedirectPage", string.Empty);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            btnSubmit.Text = ResHelper.GetString("Kadena.MailingList.Create");

            var containerId = Request.QueryString["containerid"];
            if (!string.IsNullOrWhiteSpace(containerId))
            {
                var id = new Guid(containerId);

                var mailingServiceUrl = SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSiteName}.{_mailingServiceUrlSettingKey}");
                var mailingListClient = new MailingListClient();

                var mailingListResponse = mailingListClient.GetMailingList(mailingServiceUrl, SiteContext.CurrentSiteName, id).Result;
                if (mailingListResponse.Success)
                {
                    _container = mailingListResponse.Payload;
                }
                else
                {
                    EventLogProvider.LogEvent(EventType.ERROR, GetType().Name, "MailingListClient", mailingListResponse.ErrorMessages, siteId: CurrentSite.SiteID);
                }
            }
            if (!IsPostBack)
            {
                btnHelp.Attributes["title"] = GetString("Kadena.MailingList.HelpUpload");
                inpFileName.Attributes["placeholder"] = GetString("Kadena.MailingList.FileName");
            }

            var mailTypes = CustomTableItemProvider.GetItems(_mailTypeTableName)
                    .OrderBy("ItemOrder")
                    .ToDictionary(row => row["CodeName"].ToString(), row => row["DisplayName"].ToString());
            phMailType.Controls.Add(new LiteralControl(
                    GetDictionaryHTML(GetString("Kadena.MailingList.MailType")
                                    , GetString("Kadena.MailingList.MailTypeDescription")
                                    , mailTypes
                                    , _container?.MailType)));

            var products = CustomTableItemProvider.GetItems(_productTableName)
                    .OrderBy("ItemOrder")
                    .ToDictionary(row => row["CodeName"].ToString(), row => row["DisplayName"].ToString());
            phProduct.Controls.Add(new LiteralControl(
                GetDictionaryHTML(GetString("Kadena.MailingList.Product")
                                    , GetString("Kadena.MailingList.ProductDescription")
                                    , products
                                    , _container?.ProductType)));

            var validity = CustomTableItemProvider.GetItems(_validityTableName)
                    .OrderBy("ItemOrder")
                    .ToDictionary(row => row["DayNumber"].ToString(), row => row["DisplayName"].ToString());
            phValidity.Controls.Add(new LiteralControl(
                GetDictionaryHTML(GetString("Kadena.MailingList.Validity")
                                    , GetString("Kadena.MailingList.ValidityDescription")
                                    , validity
                                    , _container != null ? (_container.ValidTo - _container.CreateDate).TotalDays.ToString() : null
                                    )));

            if (_container != null)
            {
                divFileName.CssClass = "input__wrapper input__wrapper--disabled";
                inpFileName.Value = _container.Name;
                inpFileName.Disabled = true;
            }
        }

        /// <summary>
        /// Creates radio button group for specified set with list of options.
        /// </summary>
        /// <param name="name">Name of set.</param>
        /// <param name="description">Description of set.</param>
        /// <param name="options">Set of options.</param>
        /// <returns>String with html-code of radio button group.</returns>
        private static string GetDictionaryHTML(string name, string description, IDictionary<string, string> options, string predefinedOption = null)
        {
            // We could use classes from System.Web.UI.HtmlControls namespace but Kentico encrypts some attributes of tags for them.

            using (var stringWriter = new StringWriter())
            {
                using (var html = new HtmlTextWriter(stringWriter))
                {
                    html.AddAttribute(HtmlTextWriterAttribute.Class, "upload-mail__row");
                    html.RenderBeginTag(HtmlTextWriterTag.Div);
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        html.RenderBeginTag(HtmlTextWriterTag.H2);
                        html.Write(name);
                        html.RenderEndTag();
                    }
                    if (!string.IsNullOrWhiteSpace(description))
                    {
                        html.RenderBeginTag(HtmlTextWriterTag.P);
                        html.Write(description);
                        html.RenderEndTag();
                    }
                    if ((options?.Count()).GetValueOrDefault() > 0)
                    {
                        html.AddAttribute(HtmlTextWriterAttribute.Class, "row");
                        html.RenderBeginTag(HtmlTextWriterTag.Div);
                        bool isChecked = false;
                        foreach (var o in options)
                        {
                            var id = $"{name}{o.Key}Id";

                            html.AddAttribute(HtmlTextWriterAttribute.Class, "col-lg-4 col-xl-3");
                            html.RenderBeginTag(HtmlTextWriterTag.Div);

                            // <div class="input__wrapper">
                            if (string.IsNullOrWhiteSpace(predefinedOption))
                            {
                                html.AddAttribute(HtmlTextWriterAttribute.Class, "input__wrapper");
                            }
                            else
                            {
                                html.AddAttribute(HtmlTextWriterAttribute.Class, "input__wrapper input__wrapper--disabled");
                            }

                            html.RenderBeginTag(HtmlTextWriterTag.Div);

                            html.AddAttribute(HtmlTextWriterAttribute.Class, "input__radio");
                            html.AddAttribute(HtmlTextWriterAttribute.Type, "radio");
                            html.AddAttribute(HtmlTextWriterAttribute.Name, name);
                            html.AddAttribute(HtmlTextWriterAttribute.Id, id);
                            html.AddAttribute(HtmlTextWriterAttribute.Value, o.Key);
                            if (string.IsNullOrWhiteSpace(predefinedOption))
                            {
                                if (!isChecked)
                                {
                                    html.AddAttribute(HtmlTextWriterAttribute.Checked, string.Empty);
                                    isChecked = true;
                                }
                            }
                            else
                            {
                                html.AddAttribute(HtmlTextWriterAttribute.Disabled, string.Empty);
                                if (predefinedOption.Equals(o.Key))
                                {
                                    html.AddAttribute(HtmlTextWriterAttribute.Checked, string.Empty);
                                    isChecked = true;
                                }
                            }
                            html.RenderBeginTag(HtmlTextWriterTag.Input);
                            html.RenderEndTag();

                            html.AddAttribute(HtmlTextWriterAttribute.Class, "input__label input__label--radio");
                            html.AddAttribute(HtmlTextWriterAttribute.For, id);
                            html.RenderBeginTag(HtmlTextWriterTag.Label);
                            html.Write(o.Value);
                            html.RenderEndTag();

                            html.RenderEndTag(); // </div class="input__wrapper">

                            html.RenderEndTag();
                        }
                        html.RenderEndTag();
                    }
                    html.RenderEndTag();
                    return stringWriter.ToString();
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Stream fileStream = null;
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                if (file.ContentLength > 0
                    && (file.ContentType == "application/vnd.ms-excel"
                    || file.ContentType == "text/csv"))
                {
                    fileStream = file.InputStream;
                    break;
                }
            }

            if (fileStream != null)
            {
                try
                {
                    var fileServiceUrl = SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSiteName}.{_fileServiceUrlSettingKey}");
                    var fileName = inpFileName.Value;
                    var module = FileModule.KList;

                    var client = new FileClient();
                    var uploadResult = client.UploadToS3(fileServiceUrl, SiteContext.CurrentSiteName, FileFolder.OriginalMailing, module,
                        fileStream, fileName).Result;
                    if (uploadResult.Success)
                    {
                        var mailingServiceUrl = SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSiteName}.{_mailingServiceUrlSettingKey}");
                        var containerId = Guid.Empty;
                        var mailingClient = new MailingListClient();
                        if (_container == null)
                        {
                            var mailType = Request.Form[GetString("Kadena.MailingList.MailType")];
                            var product = Request.Form[GetString("Kadena.MailingList.Product")];
                            var validity = int.Parse(Request.Form[GetString("Kadena.MailingList.Validity")]);
                            var customerName = SiteContext.CurrentSiteName;
                            var createResult = mailingClient.CreateMailingContainer(
                                mailingServiceUrl,
                                customerName,
                                fileName,
                                mailType,
                                product,
                                validity,
                                ECommerceContext.CurrentCustomer?.CustomerID.ToString()).Result;
                            if (createResult.Success)
                            {
                                containerId = createResult.Payload;
                            }
                            else
                            {
                                EventLogProvider.LogEvent(EventType.ERROR, GetType().Name, "MailingListClient", createResult.ErrorMessages, siteId: CurrentSite.SiteID);
                            }
                        }
                        else
                        {
                            containerId = new Guid(_container.Id);
                            var customerName = SiteContext.CurrentSiteName;
                            var removeResult = mailingClient.RemoveAddresses(mailingServiceUrl, customerName, containerId).Result;
                            if (!removeResult.Success)
                            {
                                EventLogProvider.LogEvent(EventType.ERROR, GetType().Name, "MailingListClient", removeResult.ErrorMessages, siteId: CurrentSite.SiteID);
                            }
                        }
                        var fileId = uploadResult.Payload;
                        var nextStepUrl = RedirectPage;
                        nextStepUrl = URLHelper.AddParameterToUrl(nextStepUrl, "containerid", containerId.ToString());
                        nextStepUrl = URLHelper.AddParameterToUrl(nextStepUrl, "fileid", URLHelper.URLEncode(fileId));
                        Response.Redirect(nextStepUrl, false);
                        Context.ApplicationInstance.CompleteRequest();
                    }
                    else
                    {
                        throw new InvalidOperationException(uploadResult.ErrorMessages);
                    }
                }
                catch (Exception exc)
                {
                    inpError.Value = exc.Message;
                    EventLogProvider.LogException(GetType().Name, "EXCEPTION", exc, CurrentSite.SiteID);
                }
            }
            else
            {
                inpError.Value = ResHelper.GetString("Kadena.MailingList.FileNotUploadedOrInvalid");
            }
        }
    }
}