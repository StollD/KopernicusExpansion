﻿using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Kopernicus;
using Kopernicus.Constants;
using Kopernicus.Configuration;
using Kopernicus.Configuration.ModLoader;

using KopernicusExpansion.Utility;
using KopernicusExpansion.Creatures.AI;

using UnityEngine;

namespace KopernicusExpansion.Creatures
{
	[RequireConfigType(ConfigType.Node)]
	public class Creature : IParserEventSubscriber
	{
		//constructor
		public Creature()
		{
		}

		//fields
		[ParserTarget("name", optional = false)]
		public string name;

		[ParserTarget("part", optional = false)]
		public string part;

		[ParserTargetCollection("AI", optional = true, typePrefix = "KopernicusExpansion.Creatures.AI.", nameSignificance = NameSignificance.Type, allowMerge = true)]
		public List<AIModule> AIModules = new List<AIModule>();

		//apply/postApply
		public void Apply(ConfigNode node)
		{

		}
		public void PostApply(ConfigNode node)
		{
			//sort the list by priority
			AIModules.Sort (delegate(AIModule a, AIModule b) {
				return a.priority.CompareTo (b.priority);
			});

			for(int i = 0; i < AIModules.Count; i++)
			{
				var m = AIModules [i];
				Logger.Active.Log (i + ": " + m.GetType().Name + " => " + m.priority);
			}
		}

		//loading
		public static Creature Load(ConfigNode node)
		{
			var creature = new Creature ();
			Parser.LoadObjectFromConfigurationNode (creature, node);
			return creature;
		}

		//creation
		public static Vessel Create(Creature creature, Vector3 positionOffset)
		{
			try
			{
				AvailablePart partInfo = PartLoader.LoadedPartsList.First(ap => ap.name == creature.part);
				Part referencePart = FlightGlobals.ActiveVessel.rootPart;

				Part part = (Part)UnityEngine.Object.Instantiate (partInfo.partPrefab);
				part.gameObject.SetActive(true);
				part.gameObject.name = partInfo.name;
				part.partInfo = partInfo;
				part.highlightType = Part.HighlightType.Disabled;
				part.SetHighlightType(Part.HighlightType.Disabled);
				part.SetMirror(Vector3.one);

				ShipConstruct sc = new ShipConstruct();
				sc.Add(part);
				sc.SaveShip();
				sc.shipName = creature.name;

				Vessel vessel = sc.parts[0].localRoot.gameObject.AddComponent<Vessel>();
				vessel.id = Guid.NewGuid();
				vessel.vesselName = sc.shipName;
				vessel.vesselType = VesselType.Unknown;
				//vessel.DiscoveryInfo.Load(ProtoVessel.CreateDiscoveryNode(DiscoveryLevels.Unowned, UntrackedObjectClass.A, 10000, 10000));
				vessel.ctrlState = new FlightCtrlState();
				vessel.ctrlState.NeutralizeAll();
				vessel.Initialize(false);
				vessel.Landed = false;
				UnityEngine.Object.Destroy(vessel.orbitDriver.Renderer);
				vessel.SetPosition(referencePart.transform.position + positionOffset);
				vessel.SetRotation(Quaternion.identity);

				CreaturePart cp = (CreaturePart)vessel.rootPart;
				cp.creature = creature;
				cp.flightID = ShipConstruction.GetUniqueFlightID(HighLogic.CurrentGame.flightState);
				cp.missionID = referencePart.missionID;
				cp.flagURL = referencePart.flagURL;

				vessel.gameObject.AddComponent<CreatureVessel>();

				return vessel;
			}
			catch(Exception e)
			{
				Utils.LogError ("Error encountered while spawning part");
				if(e != null)
					Debug.LogException (e);
			}

			return null;
		}
	}
}

