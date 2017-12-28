﻿namespace Kadena.Old_App_Code.Kadena.Constants
{
    /// <summary>
    /// This Class represents SQl Queries used.
    /// </summary>
    public static class SQLQueries
    {
        public const string shoppingCartCartItems = "Ecommerce.Shoppingcart.GetCartItems";
        public const string distributorCartData = "Ecommerce.Shoppingcart.DistributorCartData";
        public const string loggedInUserCartData = "Ecommerce.Shoppingcart.LoggedInUserCartData";
        public const string getShoppingCartCount = "Ecommerce.Shoppingcart.GetShoppingCartCount";
        public const string getShoppingCartTotal = "Ecommerce.Shoppingcart.GetShoppingCartTotal";
    }
    /// <summary>
    /// This Class represents Transformations used .
    /// </summary>
    public static class TransformationNames
    {
        public const string cartItems = "KDA.Transformations.xCartItems";
    }
}