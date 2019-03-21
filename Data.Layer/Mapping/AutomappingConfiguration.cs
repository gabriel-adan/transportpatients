using System;
using FluentNHibernate.Automapping;

namespace Data.Layer.Mapping
{
    public class AutomappingConfiguration : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            return type.Namespace.StartsWith("Bussiness.Layer.Model");
        }
    }
}
