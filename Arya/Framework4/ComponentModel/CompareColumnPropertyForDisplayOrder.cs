namespace Natalie.Framework4.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using Natalie.Data;
    using Natalie.UserControls;
    using Attribute = Data.Attribute;

    internal class CompareColumnPropertyForDisplayOrder : IComparer<ColumnProperty>
	{
		#region Fields (6) 

		private readonly IDictionary<string, Attribute> _allAttributes;
		private readonly Dictionary<ColumnProperty, decimal[]> _comparerCache = new Dictionary<ColumnProperty, decimal[]>();
		private readonly bool _sortDisplayOrders;
		private readonly bool _sortNavigationOrders;
		private readonly TaxonomyInfo _taxonomy;

		#endregion Fields 

		#region Constructors (1) 

		public CompareColumnPropertyForDisplayOrder(TaxonomyInfo taxonomy, IDictionary<string, Attribute> allAttributes,
			bool sortNavigationOrders, bool sortDisplayOrders)
		{
			_taxonomy = taxonomy;
			_allAttributes = allAttributes;
			_sortNavigationOrders = sortNavigationOrders;
			_sortDisplayOrders = sortDisplayOrders;
		}

		#endregion Constructors 

		#region Methods (1) 

		// Private Methods (1) 

		private decimal[] GetComparerValue(ColumnProperty columnProperty)
		{
			if (_comparerCache.ContainsKey(columnProperty))
				return _comparerCache[columnProperty];

			int majorRank = 0;
			decimal minorRank = 0;
			try
			{
				var att = columnProperty.Attribute;
				decimal navOrder, dispOrder;
				bool inSchema;
				EntityDataGridView.GetAttributeOrders(
					att.AttributeName.ToLower(), _allAttributes, _taxonomy, out navOrder,
					out dispOrder, out inSchema);

				if (navOrder > 0 && _sortNavigationOrders)
				{
					majorRank = 2;
					minorRank = navOrder;
				}
				else if (dispOrder > 0 && _sortDisplayOrders)
				{
					majorRank = 3;
					minorRank = dispOrder;
				}
				else if (columnProperty.AttributeType != null &&
						 columnProperty.AttributeType.Contains(Attribute.AttributeTypeGlobal))
				{
					majorRank = 0;
					minorRank = 0;
				}
				else if (columnProperty.AttributeType != null &&
						 columnProperty.AttributeType.Contains(Attribute.AttributeTypeCalculated))
				{
					majorRank = 1;
					minorRank = 0;
				}
				else if (inSchema)
				{
					majorRank = 4;
					minorRank = 0;
				}
				else
				{
					majorRank = 5;
					minorRank = 0;
				}

				//if (!columnProperty.Visible || !AttributeHasValueInSkus(columnProperty.Attribute))
				//    majorRank += 10;
			}
			catch (Exception)
			{
			}

			var ranks = new[] {majorRank, minorRank};
			_comparerCache.Add(columnProperty, ranks);
			return ranks;
		}

		#endregion Methods 



		#region IComparer<ColumnProperty> Members

		public int Compare(ColumnProperty x, ColumnProperty y)
		{
			decimal [] xComparerValues = GetComparerValue(x);
			decimal [] yComparerValues = GetComparerValue(y);

			return xComparerValues[0] != yComparerValues[0]
					   ? xComparerValues[0].CompareTo(yComparerValues[0])
					   : (xComparerValues[1] != yComparerValues[1]
							  ? xComparerValues[1].CompareTo(yComparerValues[1])
							  : x.Attribute.AttributeName.CompareTo(y.Attribute.AttributeName));
		}

		#endregion
	}
}