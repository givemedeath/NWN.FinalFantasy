﻿using System.Collections.Generic;
using NWN.FinalFantasy.Enumeration;

namespace NWN.FinalFantasy.Entity
{
    public class PlayerHouse: EntityBase
    {
        public PlayerHouse()
        {
            Furnitures = new List<Furniture>();
        }

        public override string KeyPrefix => "PlayerHouse";

        public PlayerHouseType HouseType { get; set; }
        public List<Furniture> Furnitures { get; set; }
    }

    public class Furniture
    {
        public FurnitureType FurnitureType { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Orientation { get; set; }
        public string CustomName { get; set; }
    }
}