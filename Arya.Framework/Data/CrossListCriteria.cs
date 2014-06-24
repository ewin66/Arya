using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Arya.Framework.Data
{
	[Serializable]
	[XmlRoot("CrossListFilters")]
	public class CrossListCriteria : ISerializable
	{
		#region Fields (5) 

		[XmlArrayItem("Type")] public HashSet<string> AttributeTypeFilters;
		public bool IncludeChildren;
		public bool MatchAllTerms;
		[XmlArrayItem("TaxonomyID")] public List<Guid> TaxonomyIDFilter;
		[XmlArrayItem("ValueFilters")] public List<ValueFilter> ValueFilters;

		#endregion Fields 

		#region Constructors (3) 

		public CrossListCriteria(
			List<Guid> taxonomyIDFilter, List<ValueFilter> valueFilters, HashSet<string> attributeTypeFilters,
			bool matchAllTerms, bool includeChildren)
		{
			TaxonomyIDFilter = taxonomyIDFilter;
			ValueFilters = valueFilters;
			AttributeTypeFilters = attributeTypeFilters;
			MatchAllTerms = matchAllTerms;
			IncludeChildren = includeChildren;
		}

		public CrossListCriteria(SerializationInfo info, StreamingContext ctxt)
		{
			TaxonomyIDFilter = (List<Guid>) info.GetValue("TaxonomyIDFilter", typeof (List<Guid>));
			ValueFilters = (List<ValueFilter>) info.GetValue("ValueFilter", typeof (ValueFilter));
			AttributeTypeFilters = (HashSet<string>) info.GetValue("AttributeTypeFilters", typeof (HashSet<string>));
			MatchAllTerms = (bool) info.GetValue("MatchAllTerms", typeof (bool));
			IncludeChildren = (bool) info.GetValue("IncludeChildren", typeof (bool));
		}

		public CrossListCriteria()
		{
		}

		#endregion Constructors 

		#region Methods (1) 

		// Private Methods (1) 

		void ISerializable.GetObjectData(SerializationInfo oInfo, StreamingContext oContext)
		{
			oInfo.AddValue("TaxonomyIDFilter", TaxonomyIDFilter);
			oInfo.AddValue("ValueFilter", ValueFilters);
			oInfo.AddValue("AttributeTypeFilters", AttributeTypeFilters);
			oInfo.AddValue("MatchAllTerms", MatchAllTerms);
			oInfo.AddValue("IncludeChildren", IncludeChildren);
		}

		#endregion Methods 
	}
}