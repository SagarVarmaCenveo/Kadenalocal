﻿using System;

namespace Kadena.Models.Checkout
{
    public class CartItem
    {
        public int Id { get; set; }
        public string CartItemText { get; set; }
        public string ProductType { get; set; }
        public string Image { get; set; }
        public string Template { get; set; }
        public string EditorTemplateId { get; set; }
        public int ProductPageId { get; set; }

        /// <summary>
        /// SKU object ID in DB
        /// </summary>
        public int SKUID { get;set;}

        /// <summary>
        /// Nuber configurable in product's Form -> General -> SKU*
        /// </summary>
        public string SKUNumber { get; set; }
        public string SKUName { get; set; }
        public int LineNumber { get; set; }
        public bool IsMailingList
        {
            get
            {
                return ProductType.Contains("KDA.MailingProduct");
            }
        }
        public bool IsTemplated
        {
            get
            {
                return ProductType.Contains("KDA.TemplatedProduct");
            }
        }
        public bool IsInventory
        {
            get
            {
                return ProductType.Contains("KDA.InventoryProduct");
            }
        }
        public bool IsPOD
        {
            get
            {
                return ProductType.Contains("KDA.POD");
            }
        }
        public bool IsStatic
        {
            get
            {
                return ProductType.Contains("KDA.StaticProduct");
            }
        }
        public string MailingListName { get; set; }
        public Guid MailingListGuid { get; set; }
        public string Delivery
        {
            get
            {
                return IsMailingList && IsTemplated ? $"Delivery to {Quantity} addresses" : string.Empty;
            }
        }
        public string PricePrefix { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }
        public bool IsQuantityEditable
        {
            get
            {
                return IsInventory || IsPOD || IsStatic;
            }
        }
        public string QuantityPrefix { get; set; }
        public int Quantity { get; set; }
        public int StockQuantity { get; set; }
        public double TotalTax { get; set; }
        public string PriceText { get; set; }
        public Guid ProductChiliWorkspaceId { get; set; }
        public bool IsEditable
        {
            get
            {
                return !IsMailingList && IsTemplated;
            }
        }

        public string EditorURL
        {
            get
            {
                return $"/products/product-tools/product-editor?id={ProductPageId}&skuid={SKUID}&templateid={EditorTemplateId}&workspaceid={ProductChiliWorkspaceId}";
            }
        }

        /// <summary>
        /// Main Chili template ID
        /// </summary>
        public Guid ChiliTemplateId { get; set; }

        /// <summary>
        /// Selected template instance ID
        /// </summary>
        public Guid ChiliEditorTemplateId { get; set; }

        public Guid ProductChiliPdfGeneratorSettingsId { get; set; }

        public string DesignFilePath { get; set; }

        /// <summary>
        /// Indicates if it is necessary to obtain design file path
        /// via calling Template product service
        /// </summary>
        public bool DesignFilePathRequired
        {
            get
            {
                return IsTemplated;
            }
        }

        public string UnitOfMeasure { get; set; }

               

        /// <summary>
        /// Template product service's task Id
        /// </summary>
        public string DesignFilePathTaskId { get; set; }
    }
}