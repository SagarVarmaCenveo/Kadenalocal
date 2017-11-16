﻿using CMS.EventLog;
using CMS.UIControls;
using System;
using System.IO;
using System.Web;
using Kadena.Old_App_Code.Kadena.Imports;
using Kadena.Old_App_Code.Kadena.Imports.Products;

namespace Kadena.CMSModules.Kadena.Pages.Products
{
    public partial class ProductImagesImport : CMSPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HideErrorMessage();
        }

        private int SelectedSiteID => Convert.ToInt32(siteSelector.Value);

        protected void btnUploadProductList_Click(object sender, EventArgs e)
        {
            var file = importFile.PostedFile;
            if (string.IsNullOrWhiteSpace(file.FileName))
            {
                ShowErrorMessage("You need to choose the import file.");
                return;
            }

            if (SelectedSiteID == 0)
            {
                ShowErrorMessage("You need to choose the Site.");
                return;
            }

            var fileData = ReadFileFromRequest(file);
            var excelType = ImportHelper.GetExcelTypeFromFileName(file.FileName);

            try
            {
                var result = new ProductImportService().ProcessProductImagesImportFile(fileData, excelType, SelectedSiteID);
                if (result.ErrorMessages.Length > 0)
                {
                    ShowErrorMessage(FormatImportResult(result)); // TODO format results properly
                }
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("Import product images", "EXCEPTION", ex);
                ShowErrorMessage("There was an error while processing the request. Detailed information was placed in log.");
            }
        }

        private string FormatImportResult(ImportResult result)
        {
            var headline = $"There was {result.AllMessagesCount} error(s) while processing the request. First {result.ErrorMessages.Length} errors: <br /><br />";
            return headline + string.Join("<br />", result.ErrorMessages);
        }

        protected void btnDownloadTemplate_Click(object sender, EventArgs e)
        {
            var bytes = new ProductImagesTemplateService().GetTemplateFile<ProductImageDto>(SelectedSiteID);
            var templateFileName = "productimages-upload-template.xlsx";
            WriteFileToResponse(templateFileName, bytes);
        }

        private byte[] ReadFileFromRequest(HttpPostedFile fileRequest)
        {
            using (var binaryReader = new BinaryReader(fileRequest.InputStream))
            {
                return binaryReader.ReadBytes(fileRequest.ContentLength);
            }
        }

        private void WriteFileToResponse(string filename, byte[] data)
        {
            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);

            Response.OutputStream.Write(data, 0, data.Length);
            Response.Flush();

            Response.Close();
        }

        private void ShowErrorMessage(string message)
        {
            errorMessageContainer.Visible = true;
            errorMessage.Text = message;
        }

        private void HideErrorMessage()
        {
            errorMessageContainer.Visible = false;
        }
    }
}