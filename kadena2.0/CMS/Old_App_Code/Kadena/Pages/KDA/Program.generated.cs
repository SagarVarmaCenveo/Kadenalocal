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

[assembly: RegisterDocumentType(Program.CLASS_NAME, typeof(Program))]

namespace CMS.DocumentEngine.Types.KDA
{
	/// <summary>
	/// Represents a content item of type Program.
	/// </summary>
	public partial class Program : TreeNode
	{
		#region "Constants and variables"

		/// <summary>
		/// The name of the data class.
		/// </summary>
		public const string CLASS_NAME = "KDA.Program";


		/// <summary>
		/// The instance of the class that provides extended API for working with Program fields.
		/// </summary>
		private readonly ProgramFields mFields;

		#endregion


		#region "Properties"

		/// <summary>
		/// ProgramID.
		/// </summary>
		[DatabaseIDField]
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
		/// Delivery Date To Distrubutors.
		/// </summary>
		[DatabaseField]
		public DateTime DeliveryDateToDistributors
		{
			get
			{
				return ValidationHelper.GetDateTime(GetValue("DeliveryDateToDistributors"), DateTimeHelper.ZERO_TIME);
			}
			set
			{
				SetValue("DeliveryDateToDistributors", value);
			}
		}


		/// <summary>
		/// Program Name.
		/// </summary>
		[DatabaseField]
		public string ProgramName
		{
			get
			{
				return ValidationHelper.GetString(GetValue("ProgramName"), "");
			}
			set
			{
				SetValue("ProgramName", value);
			}
		}


		/// <summary>
		/// Program Description.
		/// </summary>
		[DatabaseField]
		public string ProgramDescription
		{
			get
			{
				return ValidationHelper.GetString(GetValue("ProgramDescription"), "");
			}
			set
			{
				SetValue("ProgramDescription", value);
			}
		}


		/// <summary>
		/// Brand.
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
		/// Campaign.
		/// </summary>
		[DatabaseField]
		public int CampaignID
		{
			get
			{
				return ValidationHelper.GetInteger(GetValue("CampaignID"), 0);
			}
			set
			{
				SetValue("CampaignID", value);
			}
		}


        /// <summary>
		/// Status.
		/// </summary>
		[DatabaseField]
        public bool Status
        {
            get
            {
                return ValidationHelper.GetBoolean(GetValue("Status"), true);
            }
            set
            {
                SetValue("Status", value);
            }
        }


        /// <summary>
        /// Gets an object that provides extended API for working with Program fields.
        /// </summary>
        [RegisterProperty]
		public ProgramFields Fields
		{
			get
			{
				return mFields;
			}
		}


		/// <summary>
		/// Provides extended API for working with Program fields.
		/// </summary>
		[RegisterAllProperties]
		public partial class ProgramFields : AbstractHierarchicalObject<ProgramFields>
		{
			/// <summary>
			/// The content item of type Program that is a target of the extended API.
			/// </summary>
			private readonly Program mInstance;


			/// <summary>
			/// Initializes a new instance of the <see cref="ProgramFields" /> class with the specified content item of type Program.
			/// </summary>
			/// <param name="instance">The content item of type Program that is a target of the extended API.</param>
			public ProgramFields(Program instance)
			{
				mInstance = instance;
			}


			/// <summary>
			/// ProgramID.
			/// </summary>
			public int ID
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
			/// Delivery Date To Distrubutors.
			/// </summary>
			public DateTime DeliveryDateToDistributors
			{
				get
				{
					return mInstance.DeliveryDateToDistributors;
				}
				set
				{
					mInstance.DeliveryDateToDistributors = value;
				}
			}


			/// <summary>
			/// Program Name.
			/// </summary>
			public string Name
			{
				get
				{
					return mInstance.ProgramName;
				}
				set
				{
					mInstance.ProgramName = value;
				}
			}


			/// <summary>
			/// Program Description.
			/// </summary>
			public string Description
			{
				get
				{
					return mInstance.ProgramDescription;
				}
				set
				{
					mInstance.ProgramDescription = value;
				}
			}


			/// <summary>
			/// Brand.
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
			/// Campaign.
			/// </summary>
			public int CampaignID
			{
				get
				{
					return mInstance.CampaignID;
				}
				set
				{
					mInstance.CampaignID = value;
				}
			}


            /// <summary>
			/// Status.
			/// </summary>
			public bool Status
            {
                get
                {
                    return mInstance.Status;
                }
                set
                {
                    mInstance.Status = value;
                }
            }
        }

		#endregion


		#region "Constructors"

		/// <summary>
		/// Initializes a new instance of the <see cref="Program" /> class.
		/// </summary>
		public Program() : base(CLASS_NAME)
		{
			mFields = new ProgramFields(this);
		}

		#endregion
	}
}