﻿using CMS.CustomTables;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using Kadena.Old_App_Code.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace Kadena.CMSWebParts.Kadena.MailingList
{
    public partial class MailingListUploader : CMSAbstractWebPart
    {
        private readonly string _mailTypeTableName = "KDA.MailingType";
        private readonly string _productTableName = "KDA.MailingProductType";
        private readonly string _validityTableName = "KDA.MailingValidity";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnHelp.Attributes["title"] = GetString("Kadena.MailingList.HelpUpload");
                textFileToUpload.InnerText = GetString("Kadena.MailingList.FileToUpload");
                textOr.InnerText = GetString("Kadena.MailingList.Or");
                textSkipField.InnerText = GetString("Kadena.MailingList.SkipField");
                btnSubmit.InnerText = GetString("Kadena.MailingList.Create");
                textFileName1.InnerText = GetString("Kadena.MailingList.FileName");
                textFileName2.InnerText = GetString("Kadena.MailingList.FileName");
                textFileNameDescr.InnerText = GetString("Kadena.MailingList.FileNameDescription");
                inpFileName.Attributes["placeholder"] = GetString("Kadena.MailingList.FileName");
            }

            var mailTypes = CustomTableItemProvider.GetItems(_mailTypeTableName)
                    .OrderBy("ItemOrder")
                    .ToDictionary(row => row["CodeName"].ToString(), row => row["DisplayName"].ToString());
            phMailType.Controls.Add(new LiteralControl(
                    GetDictionaryHTML(GetString("Kadena.MailingList.MailType")
                                    , GetString("Kadena.MailingList.MailTypeDescription")
                                    , mailTypes)));

            var products = CustomTableItemProvider.GetItems(_productTableName)
                    .OrderBy("ItemOrder")
                    .ToDictionary(row => row["CodeName"].ToString(), row => row["DisplayName"].ToString());
            phProduct.Controls.Add(new LiteralControl(
                GetDictionaryHTML(GetString("Kadena.MailingList.Product")
                                    , GetString("Kadena.MailingList.ProductDescription")
                                    , products)));

            var validity = CustomTableItemProvider.GetItems(_validityTableName)
                    .OrderBy("ItemOrder")
                    .ToDictionary(row => row["DayNumber"].ToString(), row => row["DisplayName"].ToString());
            phValidity.Controls.Add(new LiteralControl(
                GetDictionaryHTML(GetString("Kadena.MailingList.Validity")
                                    , GetString("Kadena.MailingList.ValidityDescription")
                                    , validity)));
        }

        /// <summary>
        /// Creates radio button group for specified set with list of options.
        /// </summary>
        /// <param name="name">Name of set.</param>
        /// <param name="description">Description of set.</param>
        /// <param name="options">Set of options.</param>
        /// <returns>String with html-code of radio button group.</returns>
        private static string GetDictionaryHTML(string name, string description, IDictionary<string, string> options)
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

                            html.AddAttribute(HtmlTextWriterAttribute.Class, "input__wrapper");
                            html.RenderBeginTag(HtmlTextWriterTag.Div);

                            html.AddAttribute(HtmlTextWriterAttribute.Class, "input__radio");
                            html.AddAttribute(HtmlTextWriterAttribute.Type, "radio");
                            html.AddAttribute(HtmlTextWriterAttribute.Name, name);
                            html.AddAttribute(HtmlTextWriterAttribute.Id, id);
                            html.AddAttribute(HtmlTextWriterAttribute.Value, o.Key);
                            if (!isChecked)
                            {
                                html.AddAttribute(HtmlTextWriterAttribute.Checked, string.Empty);
                                isChecked = true;
                            }
                            html.RenderBeginTag(HtmlTextWriterTag.Input);
                            html.RenderEndTag();

                            html.AddAttribute(HtmlTextWriterAttribute.Class, "input__label input__label--radio");
                            html.AddAttribute(HtmlTextWriterAttribute.For, id);
                            html.RenderBeginTag(HtmlTextWriterTag.Label);
                            html.Write(o.Value);
                            html.RenderEndTag();

                            html.RenderEndTag();
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
            if (IsPostBack
                && !string.IsNullOrWhiteSpace(Request.Form[GetString("Kadena.MailingList.MailType")])
                && !string.IsNullOrWhiteSpace(Request.Form[GetString("Kadena.MailingList.Product")])
                && !string.IsNullOrWhiteSpace(Request.Form[GetString("Kadena.MailingList.Validity")]))
            {
                var validity = -1;
                if (int.TryParse(Request.Form[GetString("Kadena.MailingList.Validity")], out validity))
                {
                    var fileId = ServiceHelper.UploadFile(inpFile.PostedFile.InputStream, inpFileName.Value);

                    var containerId = ServiceHelper.CreateMailingContainer(Request.Form[GetString("Kadena.MailingList.MailType")]
                        , Request.Form[GetString("Kadena.MailingList.Product")]
                        , validity);

                    var nextStepUrl = GetStringValue("RedirectPage", string.Empty);
                    nextStepUrl = URLHelper.AddParameterToUrl(nextStepUrl, "containerid", containerId.ToString());
                    nextStepUrl = URLHelper.AddParameterToUrl(nextStepUrl, "fileid", fileId.ToString());
                    Response.Redirect(nextStepUrl);
                }
            }
        }
    }
}