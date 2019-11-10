﻿namespace NWN.FinalFantasy.Data.Entity
{
    public class InventoryItem: EntityBase
    {
        public string Name { get; set; }
        public string Tag { get; set; }
        public string Resref { get; set; }
        public int Quantity { get; set; }
        public string Data { get; set; }
    }
}