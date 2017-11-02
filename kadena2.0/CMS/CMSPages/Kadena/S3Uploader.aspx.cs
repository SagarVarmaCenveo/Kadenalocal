﻿using CMS.DataEngine;
using CMS.EventLog;
using CMS.IO;
using CMS.SiteProvider;
using CMS.UIControls;
using Kadena2.MicroserviceClients;
using Kadena2.MicroserviceClients.Clients;
using Kadena2.MicroserviceClients.Contracts;
using System;

namespace Kadena.CMSFormControls
{
    public partial class S3Uploader : CMSPage
    {
        private readonly string _fileServiceUrlSettingKey = "KDA_FileServiceUrl";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                UploadFile();
            }
        }

        private void UploadFile()
        {
            if ((inpFile.PostedFile?.ContentLength ?? 0) == 0)
            {
                lblMessage.Text = GetString("Kadena.Admin.NotValidFile");
                lnkFile.NavigateUrl = string.Empty;
                lnkFile.Text = string.Empty;
                return;
            }
            
            try
            {
                var fileServiceUrl = SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSiteName}.{_fileServiceUrlSettingKey}");
                var fileName = Path.GetFileName(inpFile.PostedFile.FileName);
                var module = FileModule.KProducts;

                var client = new FileClient();
                var uploadResult = client.UploadToS3(fileServiceUrl, SiteContext.CurrentSiteName, FileFolder.Artworks, module,
                    inpFile.PostedFile.InputStream, fileName).Result;

                if (uploadResult.Success)
                {
                    var fileKey = uploadResult.Payload;
                    string fileUrl = client.GetFileUrl(fileServiceUrl, fileKey, module);

                    lblMessage.Text = string.Empty;
                    lnkFile.Text = fileName;
                    lnkFile.NavigateUrl = fileUrl;
                }
                else
                {
                    throw new InvalidOperationException(uploadResult.ErrorMessages);
                }
            }
            catch (Exception exc)
            {
                lblMessage.Text = exc.Message;
                lnkFile.NavigateUrl = string.Empty;
                lnkFile.Text = string.Empty;
                EventLogProvider.LogException(Title, "EXCEPTION", exc);
            }
        }
    }
}