﻿using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Kopernicus;
using Kopernicus.Constants;
using Kopernicus.Configuration;
using Kopernicus.Configuration.ModLoader;

using KopernicusExpansion;
using KopernicusExpansion.Effects;
using KopernicusExpansion.Configuration;

using UnityEngine;

namespace KopernicusExpansion.Configuration
{
	[RequireConfigType(ConfigType.Node)]
	public class DebugHeightColor : ModLoader<PQSMod_DebugHeightColor>, IParserEventSubscriber
	{
		[ParserTarget("maxHeight", optional = true)]
		public NumericParser<double> maxHeight
		{
			set
			{
				mod.maxHeight = value.value;
			}
		}

		public void Apply(ConfigNode node)
		{
		}
		public void PostApply(ConfigNode node)
		{
		}
	}
}

namespace KopernicusExpansion.Effects
{
	public class PQSMod_DebugHeightColor : PQSMod
	{
		//noise
		public double maxHeight = 0;

		public override void OnSetup ()
		{
			requirements = PQS.ModiferRequirements.MeshColorChannel;
		}

		public override void OnVertexBuild (PQS.VertexBuildData data)
		{
			float value = (float)((data.vertHeight - sphere.radius) / maxHeight);
			Color color = new Color (value, value, value, value);
			data.vertColor = color;
		}
	}
}

