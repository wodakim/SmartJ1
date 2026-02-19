using System;
using System.Collections.Generic;
using UnityEngine;

namespace EntropySyndicate.Data
{
    [CreateAssetMenu(fileName = "ShopCatalog", menuName = "EntropySyndicate/Shop Catalog")]
    public class ShopCatalog : ScriptableObject
    {
        [Serializable]
        public struct ShopItem
        {
            public string productId;
            public string displayName;
            public string description;
            public bool oneTime;
            public int prismAmount;
            public int priceUsdCents;
        }

        public List<ShopItem> starterAndCurrencyPacks = new List<ShopItem>();
        public List<ShopItem> cosmetics = new List<ShopItem>();
    }
}
