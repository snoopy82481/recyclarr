﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Flurl;
using JetBrains.Annotations;
using Trash.Config;
using Trash.Radarr.QualityDefinition;

namespace Trash.Radarr
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class RadarrConfiguration : ServiceConfiguration
    {
        public QualityDefinitionConfig? QualityDefinition { get; init; }
        public List<CustomFormatConfig> CustomFormats { get; init; } = new();
        public bool DeleteOldCustomFormats { get; init; }

        public override string BuildUrl()
        {
            return BaseUrl
                .AppendPathSegment("api/v3")
                .SetQueryParams(new {apikey = ApiKey});
        }

        public override bool IsValid(out string msg)
        {
            if (CustomFormats.Any(cf => cf.TrashIds.Count + cf.Names.Count == 0))
            {
                msg = "'custom_formats' elements must contain at least one element in either 'names' or 'trash_ids'.";
                return false;
            }

            msg = "";
            return true;
        }
    }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class CustomFormatConfig
    {
        public List<string> Names { get; init; } = new();
        public List<string> TrashIds { get; init; } = new();
        public List<QualityProfileConfig> QualityProfiles { get; init; } = new();
    }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class QualityProfileConfig
    {
        [Required(ErrorMessage = "'name' is required for elements under 'quality_profiles'")]
        public string Name { get; init; } = "";

        public int? Score { get; init; }
    }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class QualityDefinitionConfig
    {
        // -1 does not map to a valid enumerator. this is to force validation to fail if it is not set from YAML
        // all of this craziness is to avoid making the enum type nullable which will make using the property
        // frustrating.
        [EnumDataType(typeof(RadarrQualityDefinitionType),
            ErrorMessage = "'type' is required for 'quality_definition'")]
        public RadarrQualityDefinitionType Type { get; init; } = (RadarrQualityDefinitionType) (-1);

        public decimal PreferredRatio { get; set; } = 1.0m;
    }
}
