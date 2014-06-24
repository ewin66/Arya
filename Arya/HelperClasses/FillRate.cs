using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Arya.Data;
using FreeHandFilters.Filters;
using Arya.Framework.Collections.Generic;
using Attribute = Arya.Data.Attribute;

namespace Arya.HelperClasses
{
	public class FillRate
	{
		#region Fields (4) 

		private readonly DateTime _asOnDate;
		private readonly DataState _dataState;
		private readonly DoubleKeyDictionary<Attribute, Filter, double> _fillRateValues = new DoubleKeyDictionary<Attribute, Filter, double>();
		private IEnumerable<Sku> _filteredSkus;

		#endregion Fields 

		#region Enums (1) 

		public enum DataState
		{
			Active, Before, AsOnDate
		}

		#endregion Enums 

		#region Constructors (1) 

        public FillRate(TaxonomyInfo taxonomy, IEnumerable<string> filterExpressions, IEnumerable<string> excludeExpressions, DataState dataState, DateTime asOnDate)
		{
			Taxonomy = taxonomy;
			_dataState = dataState;
			_asOnDate = asOnDate;
		}

		#endregion Constructors 

		#region Properties (1) 

		public TaxonomyInfo Taxonomy { get; private set; }

		#endregion Properties 

		#region Methods (4) 

		// Public Methods (2) 

		public double FetchFillRate(Attribute attribute, Filter filter)
		{
			if (_fillRateValues.ContainsKeys(attribute, filter))
				return _fillRateValues[attribute, filter];
			try
			{
				var skus = FilteredSkus(filter).ToList();
				var totalSkuCount = skus.Count;
				var filterSkuCount =
					GetEntities(skus).Where(ed => ed.Attribute.Equals(attribute)).Select(ed => ed.EntityInfo.SkuID).
						Distinct().Count();
				var value = (100.0 * filterSkuCount / totalSkuCount);
				_fillRateValues.Add(attribute, filter, value);

				return value;
			}
			catch (Exception)
			{
				return double.NaN;
			}
		}

		public double? TryGetFillRate(Attribute attribute, Filter filter)
		{
			return _fillRateValues.ContainsKeys(attribute, filter) ? (double?)_fillRateValues[attribute, filter] : null;
		}
		// Private Methods (2) 

		IEnumerable<Sku> FilteredSkus(Filter filter)
		{
 
			if (_filteredSkus == null)
			{
				_filteredSkus = from si in AryaTools.Instance.InstanceData.Dc.SkuInfos
								where si.Active && si.TaxonomyInfo.Equals(Taxonomy)
								select si.Sku;
			}
			Expression<Func<Sku, bool>> filterExpression = filter.GetFilterExpression<Sku>();
			return _filteredSkus.Where(filterExpression.Compile());
		}

		private IEnumerable<EntityData> GetEntities(IEnumerable<Sku> filteredSkus)
		{
			IEnumerable<EntityData> entityDatas = from sku in filteredSkus
												  from ei in sku.EntityInfos
												  from ed in ei.EntityDatas
												  select ed;
			switch (_dataState)
			{
				case DataState.Active:
					entityDatas = entityDatas.Where(ed => ed.Active);
					break;
				case DataState.Before:
					entityDatas = entityDatas.Where(ed => ed.BeforeEntity.Equals(true));
					break;
				case DataState.AsOnDate:
					entityDatas =
						entityDatas.Where(
							ed => ed.CreatedOn <= _asOnDate && (ed.DeletedOn == null || ed.DeletedOn > _asOnDate));
					break;
			}
			return entityDatas;
		}

		#endregion Methods 
	}

	public class FillRateWorker
	{
		public class FillRateWorkUnit
		{
			public FillRate FillRateUnit;
			public Attribute AttributeUnit;
			public Filter FilterUnit;

			public FillRateWorkUnit(FillRate fillRate, Attribute attribute, Filter filter)
			{
				FillRateUnit = fillRate;
				AttributeUnit = attribute;
				FilterUnit = filter;
			}
		}

		public readonly Queue<FillRateWorkUnit> CalculateFillRateQueue = new Queue<FillRateWorkUnit>();
		public readonly List<FillRate> FillRates = new List<FillRate>();
		public bool UseBackgroundWorker = true;

		private readonly Thread _workerThread;
		public bool Working;

		private void UpdateFillRates()
		{
			Working = true;
			while (true)
			{
				FillRateWorkUnit work;
				lock (CalculateFillRateQueue)
				{
					while (CalculateFillRateQueue.Count == 0)
					{
						Working = false;
						Monitor.Wait(CalculateFillRateQueue);
						Working = true;
					}

					work = CalculateFillRateQueue.Dequeue();
				}

				work.FillRateUnit.FetchFillRate(work.AttributeUnit, work.FilterUnit);
			}
		}

		public FillRateWorker()
		{
			_workerThread = new Thread(UpdateFillRates) { IsBackground = true };
			_workerThread.Start();
		}

		private void EnqueWork(FillRate fillRate, Attribute attribute, Filter filter)
		{
			lock (CalculateFillRateQueue)
			{
				CalculateFillRateQueue.Enqueue(new FillRateWorkUnit(fillRate, attribute, filter));
				if (CalculateFillRateQueue.Count == 1)
					Monitor.PulseAll(CalculateFillRateQueue);
			}
		}

		public double? GetFillRate(TaxonomyInfo taxonomy, Attribute attribute, Filter filter)
		{
			var fillRateObject = FillRates.FirstOrDefault(fr => fr.Taxonomy.Equals(taxonomy));
			if (fillRateObject == null)
			{
				fillRateObject = new FillRate(taxonomy, null, null, FillRate.DataState.Active, DateTime.Now);
				FillRates.Add(fillRateObject);
				if (UseBackgroundWorker)
				{
					EnqueWork(fillRateObject, attribute, filter);
					return double.MinValue;
				}
			}

			double? fillRateValue = fillRateObject.TryGetFillRate(attribute, filter);
			if (fillRateValue == null)
			{
				if (UseBackgroundWorker)
				{
					EnqueWork(fillRateObject, attribute, filter);
					return double.MinValue;
				}
				fillRateValue = fillRateObject.FetchFillRate(attribute, filter);
			}

			if (double.IsNaN((double)fillRateValue))
				return null;

			//return string.Format("{0:0.00}", fillRateValue);
			return Math.Round((double)fillRateValue, 2);
		}
	}
}
