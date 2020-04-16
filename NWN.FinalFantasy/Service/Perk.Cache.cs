﻿using System;
using System.Collections.Generic;
using System.Linq;
using NWN.FinalFantasy.Core;
using NWN.FinalFantasy.Enumeration;
using NWN.FinalFantasy.Extension;
using NWN.FinalFantasy.Service.PerkService;

namespace NWN.FinalFantasy.Service
{
    public static partial class Perk
    {
        // All categories, including inactive
        private static readonly Dictionary<PerkCategoryType, PerkCategoryAttribute> _allCategories = new Dictionary<PerkCategoryType, PerkCategoryAttribute>();

        // Active categories only
        private static readonly Dictionary<PerkCategoryType, PerkCategoryAttribute> _activeCategories = new Dictionary<PerkCategoryType, PerkCategoryAttribute>();

        // All perks, including inactive
        private static readonly Dictionary<PerkType, PerkDetail> _allPerks = new Dictionary<PerkType, PerkDetail>();
        private static readonly Dictionary<PerkCategoryType, List<PerkType>> _allPerksByCategory = new Dictionary<PerkCategoryType, List<PerkType>>();

        // Active perks only
        private static readonly Dictionary<PerkType, PerkDetail> _activePerks = new Dictionary<PerkType, PerkDetail>();
        private static readonly Dictionary<PerkCategoryType, List<PerkType>> _activePerksByCategory = new Dictionary<PerkCategoryType, List<PerkType>>();

        // Recast Group Descriptions
        private static readonly Dictionary<RecastGroup, string> _recastDescriptions = new Dictionary<RecastGroup, string>();

        [NWNEventHandler("mod_load")]
        public static void CacheData()
        {
            Console.WriteLine("Caching perk data.");

            var categories = Enum.GetValues(typeof(PerkCategoryType)).Cast<PerkCategoryType>();
            foreach (var category in categories)
            {
                var categoryDetail = category.GetAttribute<PerkCategoryType, PerkCategoryAttribute>();
                _allCategories[category] = categoryDetail;
                _allPerksByCategory[category] = new List<PerkType>();

                if (categoryDetail.IsActive)
                {
                    _activePerksByCategory[category] = new List<PerkType>();
                }
            }

            // Organize perks to make later reads quicker.
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(w => typeof(IPerkListDefinition).IsAssignableFrom(w) && !w.IsInterface && !w.IsAbstract);

            foreach (var type in types)
            {
                var instance = (IPerkListDefinition) Activator.CreateInstance(type);
                var perks = instance.BuildPerks();

                foreach (var (perkType, perkDetail) in perks)
                {
                    Console.WriteLine($"Registering perk: {perkDetail.Name}");
                    var categoryDetail = _allCategories[perkDetail.Category];

                    // Add to the perks cache
                    _allPerks[perkType] = perkDetail;

                    // Add to active cache if the perk is active
                    if (perkDetail.IsActive)
                    {
                        _activePerks[perkType] = perkDetail;

                        if(!_activePerksByCategory.ContainsKey(perkDetail.Category))
                            _activePerksByCategory[perkDetail.Category] = new List<PerkType>();

                        _activePerksByCategory[perkDetail.Category].Add(perkType);
                    }

                    // Add to active category cache if the perk and category are both active.
                    if (perkDetail.IsActive && categoryDetail.IsActive)
                    {
                        _activeCategories[perkDetail.Category] = categoryDetail;
                    }

                    // Add to the perks by category cache.
                    _allPerksByCategory[perkDetail.Category].Add(perkType);
                }
            }

            CacheRecastGroupNames();
            Console.WriteLine("Perk data cached successfully.");
        }

        /// <summary>
        /// Reads all of the enum values on the RecastGroup enumeration and stores their description
        /// attribute into the cache.
        /// </summary>
        private static void CacheRecastGroupNames()
        {
            foreach (var recast in Enum.GetValues(typeof(RecastGroup)).Cast<RecastGroup>())
            {
                _recastDescriptions[recast] = recast.GetDescriptionAttribute();
            }
        }

        /// <summary>
        /// Retrieves the human-readable name of a recast group.
        /// </summary>
        /// <param name="recastGroup">The recast group to retrieve.</param>
        /// <returns>The name of a recast group.</returns>
        public static string GetRecastGroupName(RecastGroup recastGroup)
        {
            if(!_recastDescriptions.ContainsKey(recastGroup))
                throw new KeyNotFoundException($"Recast group {recastGroup} has not been registered. Did you forget the Description attribute?");

            return _recastDescriptions[recastGroup];
        }

        /// <summary>
        /// Retrieves a list of all perks, including inactive ones.
        /// </summary>
        /// <returns>A list of all perks.</returns>
        public static Dictionary<PerkType, PerkDetail> GetAllPerks()
        {
            return _allPerks.ToDictionary(x => x.Key, y => y.Value);
        }

        /// <summary>
        /// Retrieves a list of all active perks, excluding inactive ones.
        /// </summary>
        /// <returns>A list of all active perks.</returns>
        public static Dictionary<PerkType, PerkDetail> GetAllActivePerks()
        {
            return _activePerks.ToDictionary(x => x.Key, y => y.Value);
        }

        /// <summary>
        /// Retrieves a list of all perk categories, including inactive ones.
        /// </summary>
        /// <returns>A list of all perk categories.</returns>
        public static Dictionary<PerkCategoryType, PerkCategoryAttribute> GetAllPerkCategories()
        {
            return _allCategories.ToDictionary(x => x.Key, y => y.Value);
        }

        /// <summary>
        /// Retrieves a list of all active perk categories, excluding inactive ones.
        /// </summary>
        /// <returns>A list of all active perk categories.</returns>
        public static Dictionary<PerkCategoryType, PerkCategoryAttribute> GetAllActivePerkCategories()
        {
            return _activeCategories.ToDictionary(x => x.Key, y => y.Value);
        }

        /// <summary>
        /// Retrieves details about an individual perk.
        /// </summary>
        /// <param name="perkType">The type of perk to retrieve.</param>
        /// <returns>An object containing a perk's details.</returns>
        public static PerkDetail GetPerkDetails(PerkType perkType)
        {
            return _allPerks[perkType];
        }

        /// <summary>
        /// Retrieves details about an individual perk category.
        /// </summary>
        /// <param name="categoryType">The type of category to retrieve.</param>
        /// <returns>An object containing a perk category's details.</returns>
        public static PerkCategoryAttribute GetPerkCategoryDetails(PerkCategoryType categoryType)
        {
            return _allCategories[categoryType];
        }
    }
}
