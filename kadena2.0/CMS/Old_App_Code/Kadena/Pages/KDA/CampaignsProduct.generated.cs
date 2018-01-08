//--------------------------------------------------------------------------------------------------
// <auto-generated>
//
//     This code was generated by code generator tool.
//
//     To customize the code use your own partial class. For more info about how to use and customize
//     the generated code see the documentation at http://docs.kentico.com.
//
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

using CMS;
using CMS.Base;
using CMS.Helpers;
using CMS.DataEngine;
using CMS.DocumentEngine.Types.KDA;
using CMS.DocumentEngine;
using CMS.Ecommerce;

[assembly: RegisterDocumentType(CampaignsProduct.CLASS_NAME, typeof(CampaignsProduct))]

namespace CMS.DocumentEngine.Types.KDA
{
    /// <summary>
    /// Represents a content item of type CampaignsProduct.
    /// </summary>
    public partial class CampaignsProduct : SKUTreeNode
    {
        #region "Constants and variables"

        /// <summary>
        /// The name of the data class.
        /// </summary>
        public const string CLASS_NAME = "KDA.CampaignsProduct";


        /// <summary>
        /// The instance of the class that provides extended API for working with CampaignsProduct fields.
        /// </summary>
        private readonly CampaignsProductFields mFields;


        /// <summary>
        /// The instance of the class that provides extended API for working with SKU fields.
        /// </summary>
        private readonly ProductFields mProduct;

        #endregion


        #region "Properties"

        /// <summary>
        /// CampaignsProductID.
        /// </summary>
        [DatabaseIDField]
        public int CampaignsProductID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("CampaignsProductID"), 0);
            }
            set
            {
                SetValue("CampaignsProductID", value);
            }
        }


        /// <summary>
        /// Estimated Price.
        /// </summary>
        [DatabaseField]
        public double EstimatedPrice
        {
            get
            {
                return ValidationHelper.GetDouble(GetValue("EstimatedPrice"), 0);
            }
            set
            {
                SetValue("EstimatedPrice", value);
            }
        }


        /// <summary>
        /// Produc tName.
        /// </summary>
        [DatabaseField]
        public string ProductName
        {
            get
            {
                return ValidationHelper.GetString(GetValue("ProductName"), "");
            }
            set
            {
                SetValue("ProductName", value);
            }
        }


        /// <summary>
        /// ProgramID.
        /// </summary>
        [DatabaseField]
        public int ProgramID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("ProgramID"), 0);
            }
            set
            {
                SetValue("ProgramID", value);
            }
        }


        /// <summary>
        /// BrandID.
        /// </summary>
        [DatabaseField]
        public int BrandID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("BrandID"), 0);
            }
            set
            {
                SetValue("BrandID", value);
            }
        }


        /// <summary>
        /// State.
        /// </summary>
        [DatabaseField]
        public int State
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("State"), 0);
            }
            set
            {
                SetValue("State", value);
            }
        }


        /// <summary>
        /// CategoryID.
        /// </summary>
        [DatabaseField]
        public int CategoryID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("CategoryID"), 0);
            }
            set
            {
                SetValue("CategoryID", value);
            }
        }


        /// <summary>
        /// Qty Per Pack.
        /// </summary>
        [DatabaseField]
        public int QtyPerPack
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("QtyPerPack"), 0);
            }
            set
            {
                SetValue("QtyPerPack", value);
            }
        }


        /// <summary>
        /// Item Specs.
        /// </summary>
        [DatabaseField]
        public string ItemSpecs
        {
            get
            {
                return ValidationHelper.GetString(GetValue("ItemSpecs"), "");
            }
            set
            {
                SetValue("ItemSpecs", value);
            }
        }


        /// <summary>
        /// Cancelled.
        /// </summary>
        [DatabaseField]
        public bool Cancelled
        {
            get
            {
                return ValidationHelper.GetBoolean(GetValue("Cancelled"), false);
            }
            set
            {
                SetValue("Cancelled", value);
            }
        }


        /// <summary>
        /// CVO ProductID.
        /// </summary>
        [DatabaseField]
        public string CVOProductID
        {
            get
            {
                return ValidationHelper.GetString(GetValue("CVOProductID"), "");
            }
            set
            {
                SetValue("CVOProductID", value);
            }
        }


        /// <summary>
        /// Store Front ProductID.
        /// </summary>
        [DatabaseField]
        public int StoreFrontProductID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("StoreFrontProductID"), 0);
            }
            set
            {
                SetValue("StoreFrontProductID", value);
            }
        }


        /// <summary>
        /// Product Weight.
        /// </summary>
        [DatabaseField]
        public decimal ProductWeight
        {
            get
            {
                return ValidationHelper.GetDecimal(GetValue("ProductWeight"), 0);
            }
            set
            {
                SetValue("ProductWeight", value);
            }
        }


        /// <summary>
        /// Custom Item Specs.
        /// </summary>
        [DatabaseField]
        public string CustomItemSpecs
        {
            get
            {
                return ValidationHelper.GetString(GetValue("CustomItemSpecs"), "");
            }
            set
            {
                SetValue("CustomItemSpecs", value);
            }
        }


        /// <summary>
        /// Thumbnail.
        /// </summary>
        [DatabaseField]
        public Guid ProductThumbnail
        {
            get
            {
                return ValidationHelper.GetGuid(GetValue("ProductThumbnail"), Guid.Empty);
            }
            set
            {
                SetValue("ProductThumbnail", value);
            }
        }


        /// <summary>
        /// Gets an object that provides extended API for working with CampaignsProduct fields.
        /// </summary>
        [RegisterProperty]
        public CampaignsProductFields Fields
        {
            get
            {
                return mFields;
            }
        }


        /// <summary>
        /// Gets an object that provides extended API for working with SKU fields.
        /// </summary>
        public ProductFields Product
        {
            get
            {
                return mProduct;
            }
        }


        /// <summary>
        /// Provides extended API for working with CampaignsProduct fields.
        /// </summary>
        [RegisterAllProperties]
        public partial class CampaignsProductFields : AbstractHierarchicalObject<CampaignsProductFields>
        {
            /// <summary>
            /// The content item of type CampaignsProduct that is a target of the extended API.
            /// </summary>
            private readonly CampaignsProduct mInstance;


            /// <summary>
            /// Initializes a new instance of the <see cref="CampaignsProductFields" /> class with the specified content item of type CampaignsProduct.
            /// </summary>
            /// <param name="instance">The content item of type CampaignsProduct that is a target of the extended API.</param>
            public CampaignsProductFields(CampaignsProduct instance)
            {
                mInstance = instance;
            }


            /// <summary>
            /// CampaignsProductID.
            /// </summary>
            public int ID
            {
                get
                {
                    return mInstance.CampaignsProductID;
                }
                set
                {
                    mInstance.CampaignsProductID = value;
                }
            }


            /// <summary>
            /// Estimated Price.
            /// </summary>
            public double EstimatedPrice
            {
                get
                {
                    return mInstance.EstimatedPrice;
                }
                set
                {
                    mInstance.EstimatedPrice = value;
                }
            }


            /// <summary>
            /// Produc tName.
            /// </summary>
            public string ProductName
            {
                get
                {
                    return mInstance.ProductName;
                }
                set
                {
                    mInstance.ProductName = value;
                }
            }


            /// <summary>
            /// ProgramID.
            /// </summary>
            public int ProgramID
            {
                get
                {
                    return mInstance.ProgramID;
                }
                set
                {
                    mInstance.ProgramID = value;
                }
            }


            /// <summary>
            /// BrandID.
            /// </summary>
            public int BrandID
            {
                get
                {
                    return mInstance.BrandID;
                }
                set
                {
                    mInstance.BrandID = value;
                }
            }


            /// <summary>
            /// State.
            /// </summary>
            public int State
            {
                get
                {
                    return mInstance.State;
                }
                set
                {
                    mInstance.State = value;
                }
            }


            /// <summary>
            /// CategoryID.
            /// </summary>
            public int CategoryID
            {
                get
                {
                    return mInstance.CategoryID;
                }
                set
                {
                    mInstance.CategoryID = value;
                }
            }


            /// <summary>
            /// Qty Per Pack.
            /// </summary>
            public int QtyPerPack
            {
                get
                {
                    return mInstance.QtyPerPack;
                }
                set
                {
                    mInstance.QtyPerPack = value;
                }
            }


            /// <summary>
            /// Item Specs.
            /// </summary>
            public string ItemSpecs
            {
                get
                {
                    return mInstance.ItemSpecs;
                }
                set
                {
                    mInstance.ItemSpecs = value;
                }
            }


            /// <summary>
            /// Cancelled.
            /// </summary>
            public bool Cancelled
            {
                get
                {
                    return mInstance.Cancelled;
                }
                set
                {
                    mInstance.Cancelled = value;
                }
            }


            /// <summary>
            /// CVO ProductID.
            /// </summary>
            public string CVOProductID
            {
                get
                {
                    return mInstance.CVOProductID;
                }
                set
                {
                    mInstance.CVOProductID = value;
                }
            }


            /// <summary>
            /// Store Front ProductID.
            /// </summary>
            public int StoreFrontProductID
            {
                get
                {
                    return mInstance.StoreFrontProductID;
                }
                set
                {
                    mInstance.StoreFrontProductID = value;
                }
            }


            /// <summary>
            /// Product Weight.
            /// </summary>
            public decimal ProductWeight
            {
                get
                {
                    return mInstance.ProductWeight;
                }
                set
                {
                    mInstance.ProductWeight = value;
                }
            }


            /// <summary>
            /// Custom Item Specs.
            /// </summary>
            public string CustomItemSpecs
            {
                get
                {
                    return mInstance.CustomItemSpecs;
                }
                set
                {
                    mInstance.CustomItemSpecs = value;
                }
            }


            /// <summary>
            /// Thumbnail.
            /// </summary>
            public Attachment ProductThumbnail
            {
                get
                {
                    return mInstance.GetFieldAttachment("ProductThumbnail");
                }
            }
        }


        /// <summary>
        /// Provides extended API for working with SKU fields.
        /// </summary>
        public class ProductFields
        {
            /// <summary>
            /// The content item of type <see cref="CampaignsProduct" /> that is a target of the extended API.
            /// </summary>
            private readonly CampaignsProduct mInstance;


            /// <summary>
            /// The <see cref="PublicStatusInfo" /> object related to product based on value of <see cref="SKUInfo.SKUPublicStatusID" /> column. 
            /// </summary>
            private PublicStatusInfo mPublicStatus = null;


            /// <summary>
            /// The <see cref="ManufacturerInfo" /> object related to product based on value of <see cref="SKUInfo.SKUManufacturerID" /> column. 
            /// </summary>
            private ManufacturerInfo mManufacturer = null;


            /// <summary>
            /// The <see cref="DepartmentInfo" /> object related to product based on value of <see cref="SKUInfo.SKUDepartmentID" /> column. 
            /// </summary>
            private DepartmentInfo mDepartment = null;


            /// <summary>
            /// The <see cref="SupplierInfo" /> object related to product based on value of <see cref="SKUInfo.SKUSupplierID" /> column. 
            /// </summary>
            private SupplierInfo mSupplier = null;


            /// <summary>
            /// The shortcut to <see cref="SKUInfo" /> object which is a target of this extended API.
            /// </summary>
            private SKUInfo SKU
            {
                get
                {
                    return mInstance.SKU;
                }
            }


            /// <summary>
            /// Initializes a new instance of the <see cref="ProductFields" /> class with SKU related fields of type <see cref="CampaignsProduct" /> .
            /// </summary>
            /// <param name="instance">The content item of type CampaignsProduct that is a target of the extended API.</param>
            public ProductFields(CampaignsProduct instance)
            {
                mInstance = instance;
            }


            /// <summary>
            /// SKUID.
            /// </summary>
            public int ID
            {
                get
                {
                    return (SKU != null) ? SKU.SKUID : 0;
                }
                set
                {
                    if (SKU != null)
                    {
                        SKU.SKUID = value;
                    }
                }
            }


            /// <summary>
            /// Allows you to specify the product identifier. You can use this number or string, for example, in your accounting records.
            /// </summary>
            public string SKUNumber
            {
                get
                {
                    return (SKU != null) ? SKU.SKUNumber : "";
                }
                set
                {
                    if (SKU != null)
                    {
                        SKU.SKUNumber = value;
                    }
                }
            }


            /// <summary>
            /// Package weight.
            /// </summary>
            public double Weight
            {
                get
                {
                    return (SKU != null) ? SKU.SKUWeight : 0;
                }
                set
                {
                    if (SKU != null)
                    {
                        SKU.SKUWeight = value;
                    }
                }
            }


            /// <summary>
            /// Package height.
            /// </summary>
            public double Height
            {
                get
                {
                    return (SKU != null) ? SKU.SKUHeight : 0;
                }
                set
                {
                    if (SKU != null)
                    {
                        SKU.SKUHeight = value;
                    }
                }
            }


            /// <summary>
            /// Package width.
            /// </summary>
            public double Width
            {
                get
                {
                    return (SKU != null) ? SKU.SKUWidth : 0;
                }
                set
                {
                    if (SKU != null)
                    {
                        SKU.SKUWidth = value;
                    }
                }
            }


            /// <summary>
            /// Package depth.
            /// </summary>
            public double Depth
            {
                get
                {
                    return (SKU != null) ? SKU.SKUDepth : 0;
                }
                set
                {
                    if (SKU != null)
                    {
                        SKU.SKUDepth = value;
                    }
                }
            }


            /// <summary>
            /// Gets <see cref="PublicStatusInfo" /> object based on value of <see cref="SKUInfo.SKUPublicStatusID" /> column. 
            /// </summary>
            public PublicStatusInfo PublicStatus
            {
                get
                {
                    if (SKU == null)
                    {
                        return null;
                    }

                    var id = SKU.SKUPublicStatusID;

                    if ((mPublicStatus == null) && (id > 0))
                    {
                        mPublicStatus = PublicStatusInfoProvider.GetPublicStatusInfo(id);
                    }

                    return mPublicStatus;
                }
            }


            /// <summary>
            /// Gets <see cref="ManufacturerInfo" /> object based on value of <see cref="SKUInfo.SKUManufacturerID" /> column. 
            /// </summary>
            public ManufacturerInfo Manufacturer
            {
                get
                {
                    if (SKU == null)
                    {
                        return null;
                    }

                    var id = SKU.SKUManufacturerID;

                    if ((mManufacturer == null) && (id > 0))
                    {
                        mManufacturer = ManufacturerInfoProvider.GetManufacturerInfo(id);
                    }

                    return mManufacturer;
                }
            }


            /// <summary>
            /// Gets <see cref="DepartmentInfo" /> object based on value of <see cref="SKUInfo.SKUDepartmentID" /> column. 
            /// </summary>
            public DepartmentInfo Department
            {
                get
                {
                    if (SKU == null)
                    {
                        return null;
                    }

                    var id = SKU.SKUDepartmentID;

                    if ((mDepartment == null) && (id > 0))
                    {
                        mDepartment = DepartmentInfoProvider.GetDepartmentInfo(id);
                    }

                    return mDepartment;
                }
            }


            /// <summary>
            /// Gets <see cref="SupplierInfo" /> object based on value of <see cref="SKUInfo.SKUSupplierID" /> column. 
            /// </summary>
            public SupplierInfo Supplier
            {
                get
                {
                    if (SKU == null)
                    {
                        return null;
                    }

                    var id = SKU.SKUSupplierID;

                    if ((mSupplier == null) && (id > 0))
                    {
                        mSupplier = SupplierInfoProvider.GetSupplierInfo(id);
                    }

                    return mSupplier;
                }
            }


            /// <summary>
            /// Localized name of product.
            /// </summary>
            public string Name
            {
                get
                {
                    return mInstance.DocumentSKUName;
                }
                set
                {
                    mInstance.DocumentSKUName = value;
                }
            }


            /// <summary>
            /// Localized description of product.
            /// </summary>
            public string Description
            {
                get
                {
                    return mInstance.DocumentSKUDescription;
                }
                set
                {
                    mInstance.DocumentSKUDescription = value;
                }
            }


            /// <summary>
            /// Localized short description of product.
            /// </summary>
            public string ShortDescription
            {
                get
                {
                    return mInstance.DocumentSKUShortDescription;
                }
                set
                {
                    mInstance.DocumentSKUShortDescription = value;
                }
            }
        }

        #endregion


        #region "Constructors"

        /// <summary>
        /// Initializes a new instance of the <see cref="CampaignsProduct" /> class.
        /// </summary>
        public CampaignsProduct() : base(CLASS_NAME)
        {
            mFields = new CampaignsProductFields(this);
            mProduct = new ProductFields(this);
        }

        #endregion
    }
}