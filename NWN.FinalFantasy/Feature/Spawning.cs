﻿using System;
using System.Collections.Generic;
using System.Linq;
using NWN.FinalFantasy.Core;
using NWN.FinalFantasy.Core.NWNX;
using NWN.FinalFantasy.Core.NWScript.Enum;
using NWN.FinalFantasy.Service;
using NWN.FinalFantasy.Service.SpawnService;
using static NWN.FinalFantasy.Core.NWScript.NWScript;
using Object = NWN.FinalFantasy.Core.NWNX.Object;

namespace NWN.FinalFantasy.Feature
{
    public static class Spawning
    {
        public const int DespawnMinutes = 20;
        public const int DefaultRespawnMinutes = 5;

        private class SpawnDetail
        {
            public string SerializedObject { get; set; }
            public int SpawnTableId { get; set; }
            public uint Area { get; set; }
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
            public float Facing { get; set; }
            public int RespawnDelayMinutes { get; set; }
        }

        private class ActiveSpawn
        {
            public Guid SpawnDetailId { get; set; }
            public uint SpawnObject { get; set; }
        }

        private class QueuedSpawn
        {
            public DateTime RespawnTime { get; set; }
            public Guid SpawnDetailId { get; set; }
        }

        private static readonly Dictionary<Guid, SpawnDetail> _spawns = new Dictionary<Guid, SpawnDetail>();
        private static readonly List<QueuedSpawn> _queuedSpawns = new List<QueuedSpawn>();
        private static readonly Dictionary<uint, List<QueuedSpawn>> _queuedSpawnsByArea = new Dictionary<uint, List<QueuedSpawn>>();
        private static readonly Dictionary<uint, DateTime> _queuedAreaDespawns = new Dictionary<uint, DateTime>();
        private static readonly Dictionary<int, SpawnTable> _spawnTables = new Dictionary<int, SpawnTable>();
        private static readonly Dictionary<uint, List<Guid>> _allSpawnsByArea = new Dictionary<uint, List<Guid>>();
        private static readonly Dictionary<uint, List<ActiveSpawn>> _activeSpawnsByArea = new Dictionary<uint, List<ActiveSpawn>>();

        [NWNEventHandler("mod_load")]
        public static void CacheData()
        {
            LoadSpawnTables();
            StoreSpawns();
        }

        /// <summary>
        /// When the module loads, all spawn tables are loaded with reflection and stored into a dictionary cache.
        /// If any spawn tables with the same ID are found, an exception will be raised.
        /// </summary>
        public static void LoadSpawnTables()
        {
            // Get all implementations of spawn table definitions.
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(w => typeof(ISpawnTableDefinition).IsAssignableFrom(w) && !w.IsInterface && !w.IsAbstract);

            foreach (var type in types)
            {
                var instance = (ISpawnTableDefinition)Activator.CreateInstance(type);
                var builtTables = instance.BuildSpawnTables();

                foreach (var table in builtTables)
                {
                    if (table.Key <= 0)
                    {
                        Log.Write(LogGroup.Error, $"Spawn table {table.Key} has an invalid key. Values must be greater than zero.");
                        continue;
                    }

                    if (_spawnTables.ContainsKey(table.Key))
                    {
                        Log.Write(LogGroup.Error, $"Spawn table {table.Key} has already been registered. Please make sure all spawn tables use a unique ID.");
                        continue;
                    }

                    _spawnTables[table.Key] = table.Value;
                }
            }
        }

        /// <summary>
        /// When the module loads, spawns are located in all areas. Details about those spawns are stored
        /// into the cached data.
        /// </summary>
        public static void StoreSpawns()
        {
            for(var area = GetFirstArea(); GetIsObjectValid(area); area = GetNextArea())
            {
                for (var obj = GetFirstObjectInArea(area); GetIsObjectValid(obj); obj = GetNextObjectInArea(area))
                {
                    var type = GetObjectType(obj);
                    var position = GetPosition(obj);
                    var facing = GetFacing(obj);
                    var id = Guid.NewGuid();

                    // Hand-placed creature information is stored and the actual NPC is destroyed so it can be spawned by the system.
                    if (type == ObjectType.Creature)
                    {
                        _spawns.Add(id, new SpawnDetail
                        {
                            SerializedObject = Object.Serialize(obj),
                            X = position.X,
                            Y = position.Y,
                            Z = position.Z,
                            Facing = facing,
                            Area = area,
                            RespawnDelayMinutes = 5
                        });

                        // Add this entry to the spawns by area cache.
                        if (!_allSpawnsByArea.ContainsKey(area))
                            _allSpawnsByArea[area] = new List<Guid>();

                        _allSpawnsByArea[area].Add(id);

                        DestroyObject(obj);
                    }
                    // Waypoints with a spawn table ID 
                    else if (type == ObjectType.Waypoint)
                    {
                        var spawnTableId = GetLocalInt(obj, "SPAWN_TABLE_ID");
                        if (spawnTableId > 0)
                        {
                            if (!_spawnTables.ContainsKey(spawnTableId))
                            {
                                Log.Write(LogGroup.Error, $"Waypoint has an invalid spawn table Id. ({spawnTableId}) is not defined. Do you have the right spawn table Id?");
                                continue;
                            }

                            var spawnTable = _spawnTables[spawnTableId];
                            _spawns.Add(id, new SpawnDetail
                            {
                                SpawnTableId = spawnTableId,
                                X = position.X,
                                Y = position.Y,
                                Z = position.Z,
                                Facing = facing,
                                Area = area,
                                RespawnDelayMinutes = spawnTable.RespawnDelayMinutes
                            });

                            // Add this entry to the spawns by area cache.
                            if (!_allSpawnsByArea.ContainsKey(area))
                                _allSpawnsByArea[area] = new List<Guid>();

                            _allSpawnsByArea[area].Add(id);
                        }
                    }
                }
            }
        }

        [NWNEventHandler("area_enter")]
        public static void SpawnArea()
        {
            var player = GetEnteringObject();
            if (!GetIsPC(player) && !GetIsDM(player)) return;

            var area = OBJECT_SELF;

            // Area isn't registered. Could be an instanced area? No need to spawn.
            if (!_allSpawnsByArea.ContainsKey(area)) return;

            if (!_activeSpawnsByArea.ContainsKey(area))
                _activeSpawnsByArea[area] = new List<ActiveSpawn>();

            if (!_queuedSpawnsByArea.ContainsKey(area))
                _queuedSpawnsByArea[area] = new List<QueuedSpawn>();

            var activeSpawns = _activeSpawnsByArea[area];
            var queuedSpawns = _queuedSpawnsByArea[area];

            // Spawns are currently active for this area. No need to spawn.
            if (activeSpawns.Count > 0 || queuedSpawns.Count > 0) return;

            var now = DateTime.UtcNow;
            // No spawns are active. Spawn the area.
            foreach (var spawn in _allSpawnsByArea[area])
            {
                CreateQueuedSpawn(spawn, now);
            }
        }

        /// <summary>
        /// When the last player in an area leaves, a despawn request is queued up.
        /// The heartbeat processor will despawn all objects when this happens
        /// </summary>
        [NWNEventHandler("area_exit")]
        public static void QueueDespawnArea()
        {
            var player = GetExitingObject();
            if (!GetIsPC(player) && !GetIsDM(player)) return;
            
            var area = OBJECT_SELF;
            var playerCount = Area.GetNumberOfPlayersInArea(area);
            if (playerCount > 0) return;

            var now = DateTime.UtcNow;
            _queuedAreaDespawns[area] = now.AddMinutes(DespawnMinutes);
        }

        /// <summary>
        /// Creates a queued spawn record which is picked up by the processor.
        /// The spawn object will be created when the respawn time has passed.
        /// </summary>
        /// <param name="spawnDetailId">The ID of the spawn detail.</param>
        /// <param name="respawnTime">The time the spawn will be created.</param>
        private static void CreateQueuedSpawn(Guid spawnDetailId, DateTime respawnTime)
        {
            var queuedSpawn = new QueuedSpawn
            {
                RespawnTime = respawnTime,
                SpawnDetailId = spawnDetailId
            };
            _queuedSpawns.Add(queuedSpawn);

            var spawnDetail = _spawns[spawnDetailId];
            if(!_queuedSpawnsByArea.ContainsKey(spawnDetail.Area))
                _queuedSpawnsByArea[spawnDetail.Area] = new List<QueuedSpawn>();

            _queuedSpawnsByArea[spawnDetail.Area].Add(queuedSpawn);
        }

        /// <summary>
        /// Removes a queued spawn.
        /// </summary>
        /// <param name="queuedSpawn">The queued spawn to remove.</param>
        private static void RemoveQueuedSpawn(QueuedSpawn queuedSpawn)
        {
            var spawnDetail = _spawns[queuedSpawn.SpawnDetailId];
            _queuedSpawns.Remove(queuedSpawn);
            _queuedSpawnsByArea[spawnDetail.Area].Remove(queuedSpawn);
        }

        /// <summary>
        /// When a creature dies, its details need to be queued up for a respawn.
        /// </summary>
        [NWNEventHandler("crea_death")]
        public static void QueueRespawn()
        {
            var creature = OBJECT_SELF;
            var spawnId = GetLocalString(creature, "SPAWN_ID");
            if (string.IsNullOrWhiteSpace(spawnId)) return;

            var spawnGuid = new Guid(spawnId);
            var detail = _spawns[spawnGuid];
            var respawnTime = DateTime.UtcNow.AddMinutes(detail.RespawnDelayMinutes);

            CreateQueuedSpawn(spawnGuid, respawnTime);
        }

        /// <summary>
        /// On each module heartbeat, process queued spawns and
        /// process dequeue area event requests.
        /// </summary>
        [NWNEventHandler("mod_heartbeat")]
        public static void ProcessSpawnSystem()
        {
            ProcessQueuedSpawns();
            ProcessDespawnAreas();
        }

        /// <summary>
        /// On each module heartbeat, iterate over the list of queued spawns.
        /// If enough time has elapsed and spawn table rules are met, spawn the object and remove it from the queue.
        /// </summary>
        public static void ProcessQueuedSpawns()
        {
            var now = DateTime.UtcNow;
            for (var index = _queuedSpawns.Count - 1; index >= 0; index--)
            {
                var queuedSpawn = _queuedSpawns.ElementAt(index);

                if (now > queuedSpawn.RespawnTime)
                {
                    var detail = _spawns[queuedSpawn.SpawnDetailId];
                    var spawnedObject = SpawnObject(queuedSpawn.SpawnDetailId, detail);

                    // A valid spawn wasn't found because the spawn table didn't provide a resref.
                    // Either the table is configured wrong or the requirements for that specific table weren't met.
                    // In this case, we bump the next respawn time and move to the next queued respawn.
                    if (spawnedObject == OBJECT_INVALID)
                    {
                        queuedSpawn.RespawnTime = now.AddMinutes(detail.RespawnDelayMinutes);
                        continue;
                    }

                    var activeSpawn = new ActiveSpawn
                    {
                        SpawnDetailId = queuedSpawn.SpawnDetailId,
                        SpawnObject = spawnedObject
                    };

                    _activeSpawnsByArea[detail.Area].Add(activeSpawn);
                    RemoveQueuedSpawn(queuedSpawn);
                }
            }
        }

        /// <summary>
        /// On each module heartbeat, iterate over the list of areas which are scheduled to
        /// be despawned. If players have since entered the area, remove it from the queue list.
        /// </summary>
        private static void ProcessDespawnAreas()
        {
            var now = DateTime.UtcNow;
            for (var index = _queuedAreaDespawns.Count - 1; index >= 0; index--)
            {
                var (area, despawnTime) = _queuedAreaDespawns.ElementAt(index);
                // Players have entered this area. Remove it and move to the next entry.
                if (Area.GetNumberOfPlayersInArea(area) > 0)
                {
                    _queuedAreaDespawns.Remove(area);
                    continue;
                }

                if (now > despawnTime)
                {
                    // Destroy active spawned objects from the module.
                    foreach (var activeSpawn in _activeSpawnsByArea[area])
                    {
                        DestroyObject(activeSpawn.SpawnObject);
                    }

                    if (!_queuedSpawnsByArea.ContainsKey(area))
                        _queuedSpawnsByArea[area] = new List<QueuedSpawn>();

                    // Removing all spawn Ids from the master queue list.
                    var spawnIds = _queuedSpawnsByArea[area].Select(s => s.SpawnDetailId);
                    _queuedSpawns.RemoveAll(x => spawnIds.Contains(x.SpawnDetailId));

                    // Remove area from the various cache collections.
                    _queuedSpawnsByArea.Remove(area);
                    _activeSpawnsByArea.Remove(area);
                    _queuedAreaDespawns.Remove(area);
                }
            }
        }

        /// <summary>
        /// Creates a new spawn object into its spawn area.
        /// Hand-placed objects are deserialized and added to the area.
        /// Spawn tables run their own logic to determine which object to spawn.
        /// </summary>
        /// <param name="spawnId">The ID of the spawn</param>
        /// <param name="detail">The details of the spawn</param>
        private static uint SpawnObject(Guid spawnId, SpawnDetail detail)
        {
            // Hand-placed spawns are stored as a serialized string.
            // Deserialize and add it to the area.
            if (!string.IsNullOrWhiteSpace(detail.SerializedObject))
            {
                var deserialized = Object.Deserialize(detail.SerializedObject);
                var position = new Vector(detail.X, detail.Y, detail.Z);
                Object.AddToArea(deserialized, detail.Area, position);

                AssignCommand(deserialized, () => SetFacing(detail.Facing));
                SetLocalString(deserialized, "SPAWN_ID", spawnId.ToString());

                return deserialized;
            }
            // Spawn tables have their own logic which must be run to determine the spawn to use.
            // Create the object at the stored location.
            else if(detail.SpawnTableId > 0)
            {
                var spawnTable = _spawnTables[detail.SpawnTableId];
                var (objectType, resref) = spawnTable.GetNextSpawnResref();

                // It's possible that the rules of the spawn table don't have a spawn ready to be created.
                // In this case, exit early.
                if (string.IsNullOrWhiteSpace(resref))
                {
                    return OBJECT_INVALID;
                }

                var position = new Vector(detail.X, detail.Y, detail.Z);
                var location = Location(detail.Area, position, detail.Facing);

                var spawn = CreateObject(objectType, resref, location);
                SetLocalString(spawn, "SPAWN_ID", spawnId.ToString());

                return spawn;
            }

            return OBJECT_INVALID;
        }
    }
}
