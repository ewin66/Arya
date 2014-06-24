using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arya.HelperClasses
{
    internal class MSCFillRateHelper
    {
        private IEnumerable<FillRateData> GetFilteredFillRates1(Guid taxonomyID,string webLoadAttributeName,string webLoadValue,string normalOnlyAttributeName,string qaTypeAttributeName,string qaTypeValue)
        {
            return AryaTools.Instance.InstanceData.Dc.ExecuteQuery<FillRateData>(@"DECLARE @MyTable AS FilteredSkuList
                                                                DECLARE @SourceTaxID uniqueidentifier

                                                                SET @SourceTaxID = {0};
                                                                WITH Taxonomy1( NodeName,TaxId,ParentTaxId ) AS (

                                                                SELECT      T1.NodeName, T1.TaxonomyID AS TaxId,T1.ParentTaxonomyID
                                                                FROM        dbo.TaxonomyInfo AS Ti 
			                                                                INNER  JOIN
			                                                                dbo.TaxonomyData AS T1 ON Ti.ID = T1.TaxonomyID AND T1.Active = 1
                                                                WHERE       T1.TaxonomyID = @SourceTaxID
                                                                UNION ALL
                                                                SELECT      T1.NodeName, T1.TaxonomyID AS TaxId,T1.ParentTaxonomyID
                                                                FROM        dbo.TaxonomyInfo AS Ti 
			                                                                INNER JOIN dbo.TaxonomyData AS T1 ON Ti.ID = T1.TaxonomyID AND T1.Active = 1
			                                                                INNER JOIN Taxonomy1 temp on temp.TaxId = T1.ParentTaxonomyID
                                                                ),
                                                                FilteredSkuList(ItemID,TaxonomyID) AS (
	                                                                select distinct ItemID,TaxonomyID
	                                                                from EntityData ed
	                                                                inner join EntityInfo ei on ed.EntityID = ei.ID and ed.Active = 1
	                                                                inner join Attribute a on a.ID = ed.AttributeID
	                                                                inner join Sku s on s.ID = ei.SkuID 
	                                                                inner join SkuInfo si on si.SkuID = s.ID and si.Active = 1
	                                                                where si.TaxonomyID IN (
							                                                                select TaxId as LeafTaxID
							                                                                from Taxonomy1
						                                                                   )
	                                                                AND ( AttributeName = {1} and Value = {2} ) 
	                                                                AND NOT EXISTS
				                                                                (
					                                                                select *
					                                                                from EntityData ed1
					                                                                inner join EntityInfo ei1 on ed1.EntityID = ei1.ID and ed1.Active = 1
					                                                                inner join Attribute a1 on a1.ID = ed1.AttributeID
					                                                                WHERE ei1.SkuID = s.ID 
					                                                                AND a1.AttributeName IN ({3})
				                                                                )
	                                                                AND EXISTS
				                                                                (
					                                                                select *
					                                                                from EntityData ed1
					                                                                inner join EntityInfo ei1 on ed1.EntityID = ei1.ID and ed1.Active = 1
					                                                                inner join Attribute a1 on a1.ID = ed1.AttributeID
					                                                                WHERE ei1.SkuID = s.ID 
					                                                                and ( a1.AttributeName = {4} and ed1.Value not like '{5}' )
				                                                                )
                                                                )

                                                                INSERT INTO @MyTable
                                                                Select *
                                                                from FilteredSkuList 


                                                                exec dbo.GetFillRateByTaxID3 @SourceTaxID,@MyTable", taxonomyID,webLoadAttributeName,webLoadValue,normalOnlyAttributeName,qaTypeAttributeName,qaTypeValue).ToList();
        }

        private IEnumerable<FillRateData> GetFilteredFillRates2(Guid taxonomyID, string webLoadAttributeName, string webLoadValue, string normalOnlyAttributeName)
        {
            return AryaTools.Instance.InstanceData.Dc.ExecuteQuery<FillRateData>(@"DECLARE @MyTable AS FilteredSkuList
                                                                DECLARE @SourceTaxID uniqueidentifier

                                                                SET @SourceTaxID = {0};
                                                                WITH Taxonomy1( NodeName,TaxId,ParentTaxId ) AS (

                                                                SELECT      T1.NodeName, T1.TaxonomyID AS TaxId,T1.ParentTaxonomyID
                                                                FROM        dbo.TaxonomyInfo AS Ti 
			                                                                INNER  JOIN
			                                                                dbo.TaxonomyData AS T1 ON Ti.ID = T1.TaxonomyID AND T1.Active = 1
                                                                WHERE       T1.TaxonomyID = @SourceTaxID
                                                                UNION ALL
                                                                SELECT      T1.NodeName, T1.TaxonomyID AS TaxId,T1.ParentTaxonomyID
                                                                FROM        dbo.TaxonomyInfo AS Ti 
			                                                                INNER JOIN dbo.TaxonomyData AS T1 ON Ti.ID = T1.TaxonomyID AND T1.Active = 1
			                                                                INNER JOIN Taxonomy1 temp on temp.TaxId = T1.ParentTaxonomyID
                                                                ),
                                                                FilteredSkuList(ItemID,TaxonomyID) AS (
	                                                                select distinct ItemID,TaxonomyID
	                                                                from EntityData ed
	                                                                inner join EntityInfo ei on ed.EntityID = ei.ID and ed.Active = 1
	                                                                inner join Attribute a on a.ID = ed.AttributeID
	                                                                inner join Sku s on s.ID = ei.SkuID 
	                                                                inner join SkuInfo si on si.SkuID = s.ID and si.Active = 1
	                                                                where si.TaxonomyID IN (
							                                                                select TaxId as LeafTaxID
							                                                                from Taxonomy1
						                                                                   )
	                                                                and ( AttributeName = {1} and Value = {2} ) 
	                                                                AND NOT EXISTS
				                                                                (
					                                                                select *
					                                                                from EntityData ed1
					                                                                inner join EntityInfo ei1 on ed1.EntityID = ei1.ID and ed1.Active = 1
					                                                                inner join Attribute a1 on a1.ID = ed1.AttributeID
					                                                                WHERE ei1.SkuID = s.ID 
					                                                                AND a1.AttributeName IN ({3})
				                                                                )
                                                                )

                                                                INSERT INTO @MyTable
                                                                Select *
                                                                from FilteredSkuList 


                                                                exec dbo.GetFillRateByTaxID3 @SourceTaxID,@MyTable", taxonomyID, webLoadAttributeName, webLoadValue, normalOnlyAttributeName).ToList();
        }

        private IEnumerable<FillRateData> GetFilteredFillRates3(Guid taxonomyID, string qaTypeAttributeName, string qaTypeValue, string normalOnlyAttributeName)
        {
            return AryaTools.Instance.InstanceData.Dc.ExecuteQuery<FillRateData>(@"DECLARE @MyTable AS FilteredSkuList
                                                                DECLARE @SourceTaxID uniqueidentifier

                                                                SET @SourceTaxID = {0};
                                                                WITH Taxonomy1( NodeName,TaxId,ParentTaxId ) AS (

                                                                SELECT      T1.NodeName, T1.TaxonomyID AS TaxId,T1.ParentTaxonomyID
                                                                FROM        dbo.TaxonomyInfo AS Ti 
			                                                                INNER  JOIN
			                                                                dbo.TaxonomyData AS T1 ON Ti.ID = T1.TaxonomyID AND T1.Active = 1
                                                                WHERE       T1.TaxonomyID = @SourceTaxID
                                                                UNION ALL
                                                                SELECT      T1.NodeName, T1.TaxonomyID AS TaxId,T1.ParentTaxonomyID
                                                                FROM        dbo.TaxonomyInfo AS Ti 
			                                                                INNER JOIN dbo.TaxonomyData AS T1 ON Ti.ID = T1.TaxonomyID AND T1.Active = 1
			                                                                INNER JOIN Taxonomy1 temp on temp.TaxId = T1.ParentTaxonomyID
                                                                ),
                                                                FilteredSkuList(ItemID,TaxonomyID) AS (
	                                                                select distinct ItemID,TaxonomyID
	                                                                from EntityData ed
	                                                                inner join EntityInfo ei on ed.EntityID = ei.ID and ed.Active = 1
	                                                                inner join Attribute a on a.ID = ed.AttributeID
	                                                                inner join Sku s on s.ID = ei.SkuID 
	                                                                inner join SkuInfo si on si.SkuID = s.ID and si.Active = 1
	                                                                where si.TaxonomyID IN (
							                                                                select TaxId as LeafTaxID
							                                                                from Taxonomy1
						                                                                   )
	                                                                AND NOT EXISTS
				                                                                (
					                                                                select *
					                                                                from EntityData ed1
					                                                                inner join EntityInfo ei1 on ed1.EntityID = ei1.ID and ed1.Active = 1
					                                                                inner join Attribute a1 on a1.ID = ed1.AttributeID
					                                                                WHERE ei1.SkuID = s.ID 
					                                                                AND a1.AttributeName IN ({3})
				                                                                )
	                                                                AND EXISTS
				                                                                (
					                                                                select *
					                                                                from EntityData ed1
					                                                                inner join EntityInfo ei1 on ed1.EntityID = ei1.ID and ed1.Active = 1
					                                                                inner join Attribute a1 on a1.ID = ed1.AttributeID
					                                                                WHERE ei1.SkuID = s.ID 
					                                                                and ( a1.AttributeName = {1} and ed1.Value not like {2} )
				                                                                )
                                                                )

                                                                INSERT INTO @MyTable
                                                                Select *
                                                                from FilteredSkuList 


                                                                exec dbo.GetFillRateByTaxID3 @SourceTaxID,@MyTable", taxonomyID, qaTypeAttributeName, qaTypeValue, normalOnlyAttributeName).ToList();
        }

        public List<Tuple<string, Guid, Guid, decimal>> GetFillRates(Guid taxonomyID,string projectName)
        {
            if(projectName.ToLower() == "cuttingtools")
            {
                const string webloadAttributeName = "WEBLOAD";
                const string webloadValue = "YES";
                const string normalOnlyAttributeName = "NormalOnly";
                const string qaTypeAttributeName = "QA_Type";
                const string qaTypeValue = "%not found%";

                var fillRates = new List<Tuple<string, Guid, Guid, decimal>>();
                var fdatas1 = GetFilteredFillRates1(taxonomyID, webloadAttributeName, webloadValue, normalOnlyAttributeName, qaTypeAttributeName, qaTypeValue);

                foreach (var fillRateData in fdatas1)
                {
                    fillRates.Add(new Tuple<string, Guid, Guid, decimal>("Normal Only is Blank & QA_Type = Found & Webload = Yes", fillRateData.TaxID, fillRateData.AttributeID, fillRateData.FilteredFillRate));
                    fillRates.Add(new Tuple<string, Guid, Guid, decimal>("FillRate", fillRateData.TaxID, fillRateData.AttributeID, fillRateData.FillRate));
                }

                var fdatas2 = GetFilteredFillRates2(taxonomyID,webloadAttributeName, webloadValue, normalOnlyAttributeName);
                fillRates.AddRange(fdatas2.Select(fillRateData => new Tuple<string, Guid, Guid, decimal>("Normal Only is Blank & Webload = Yes", fillRateData.TaxID, fillRateData.AttributeID, fillRateData.FilteredFillRate)));

                var fdatas3 = GetFilteredFillRates3(taxonomyID, qaTypeAttributeName, qaTypeValue, normalOnlyAttributeName);
                fillRates.AddRange(fdatas3.Select(fillRateData => new Tuple<string, Guid, Guid, decimal>("Normal Only is Blank & QA_Type = Found", fillRateData.TaxID, fillRateData.AttributeID, fillRateData.FilteredFillRate)));

                return fillRates;
            }

            if (projectName.ToLower() == "mscsj")
            {
                const string webloadAttributeName = "On Webload";
                const string webloadValue = "Y";
                const string normalOnlyAttributeName = "Normalize Only";
                const string qaTypeAttributeName = "QA_Type";
                const string qaTypeValue = "%not found%";

                var fillRates = new List<Tuple<string, Guid, Guid, decimal>>();
                var fdatas1 = GetFilteredFillRates1(taxonomyID, webloadAttributeName, webloadValue, normalOnlyAttributeName, qaTypeAttributeName, qaTypeValue);

                foreach (var fillRateData in fdatas1)
                {
                    fillRates.Add(new Tuple<string, Guid, Guid, decimal>("Normal Only is Blank & QA_Type = Found & Webload = Yes", fillRateData.TaxID, fillRateData.AttributeID, fillRateData.FilteredFillRate));
                    fillRates.Add(new Tuple<string, Guid, Guid, decimal>("FillRate", fillRateData.TaxID, fillRateData.AttributeID, fillRateData.FillRate));
                }

                var fdatas2 = GetFilteredFillRates2(taxonomyID, webloadAttributeName, webloadValue, normalOnlyAttributeName);
                fillRates.AddRange(fdatas2.Select(fillRateData => new Tuple<string, Guid, Guid, decimal>("Normal Only is Blank & Webload = Yes", fillRateData.TaxID, fillRateData.AttributeID, fillRateData.FilteredFillRate)));

                var fdatas3 = GetFilteredFillRates3(taxonomyID, qaTypeAttributeName, qaTypeValue, normalOnlyAttributeName);
                fillRates.AddRange(fdatas3.Select(fillRateData => new Tuple<string, Guid, Guid, decimal>("Normal Only is Blank & QA_Type = Found", fillRateData.TaxID, fillRateData.AttributeID, fillRateData.FilteredFillRate)));

                return fillRates;
            }

            return new List<Tuple<string, Guid, Guid, decimal>>();
        }

    }

    public class FillRateData
    {
        public Guid TaxID;
        public Guid AttributeID;
        public decimal FillRate;
        public decimal FilteredFillRate;
    }
}
