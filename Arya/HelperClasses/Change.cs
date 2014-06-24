using LinqKit;
using Arya.Framework.Collections.Generic;
using Arya.Framework.Common.ComponentModel;
using Arya.HelperForms;
using Arya.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Arya.Data;
using Attribute = Arya.Data.Attribute;

namespace Arya.HelperClasses
{
    using Framework.Math;

    public class Change
    {
        #region Fields (13)

        readonly List<Sku> _blankCells;
        readonly List<EntityData> _entityDatas;
        readonly HashSet<Sku> _processedSkus = new HashSet<Sku>();
        readonly List<Sku> _skus;
        private readonly int _totalSkuCount;
        internal bool Delete;
        public EntityData LastEntityDataCreated;
        public HashSet<string> NewAttributeNames = new HashSet<string>();
        internal AllFields NewValues;
        internal int NoOfDecimalPlaces;
        internal int UnitPrecision;
        internal UnitOfMeasure UnitofMeasure;
        internal readonly AllFields OldValues;
        internal Tool RunTool = Tool.None;
        internal string SliceDelimiter;

        #endregion Fields

        #region Enums (1)

        internal enum Tool
        {
            None, SliceInPlace, ConvertToDecimal, ConvertToUom,
            SliceIntoNewAttributes,
            ExtractMinimumValue,
            ExtractMaximumValue,
            ExtractCountOfValues
        }

        #endregion Enums

        #region Constructors (1)

        internal Change(int itemCount, int totalSkuCount)
        {
            _totalSkuCount = totalSkuCount;
            _entityDatas = new List<EntityData>(itemCount);
            _skus = new List<Sku>(itemCount);
            _blankCells = new List<Sku>(itemCount);
            OldValues = new AllFields();
            NewValues = new AllFields();
        }

        #endregion Constructors

        #region Methods (15)

        // Public Methods (1) 

        public bool ExecuteOnBuildView(TaxonomyInfo currentTaxonomy, Stack<EntityDataGridView.ChangeItem> undoHistory, bool createNewEntityDatas)
        {
            bool refreshColumns = false;
            if (!HasChange())
                return false;

            List<EntityData> entityDatas = GetEntityDatas();
            List<Sku> blanks = GetBlanks();
            refreshColumns = ChangeOrCreateEntities(entityDatas, blanks, undoHistory, createNewEntityDatas);
            return refreshColumns;
     }

        public bool Execute(TaxonomyInfo currentTaxonomy, Stack<EntityDataGridView.ChangeItem> undoHistory, bool createNewEntityDatas)
        {
            bool refreshColumns = false;
            if (!HasChange())
                return false;

            //Rename an attribute to change case
            if (!string.IsNullOrEmpty(OldValues.AttributeName) && !string.IsNullOrEmpty(NewValues.AttributeName) &&
                OldValues.AttributeName.ToLower().Equals(NewValues.AttributeName.ToLower()) &&
                !OldValues.AttributeName.Equals(NewValues.AttributeName))
            {
                if (
                    MessageBox.Show(@"This will rename the attribute globally (for the entire project). Are you sure?",
                                    @"Global Attribute Rename", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    OldValues.Att.AttributeName = NewValues.AttributeName;
                    //if (string.IsNullOrEmpty(NewValues.Val) && string.IsNullOrEmpty(NewValues.Uom) &&
                    //    string.IsNullOrEmpty(NewValues.Field1) && string.IsNullOrEmpty(NewValues.Field2) &&
                    //    string.IsNullOrEmpty(NewValues.Field3) && string.IsNullOrEmpty(NewValues.Field4) &&
                    //    string.IsNullOrEmpty(NewValues.Field5))
                    //    return true;
                }
                else
                    return false;
            }

            List<Sku> blanks = GetBlanks();
            if (NewValues.Tax != null)
            {
                // Taxonomy Change
                bool moveBlanks = false;
                if (blanks.Count > 0)
                    moveBlanks =
                        MessageBox.Show(
                            string.Format(
                                "There are {0} blank values in the current selection. Would you like to move SKUs associated with blanks?",
                                blanks.Count),
                            @"Move blanks?",
                            MessageBoxButtons.YesNo) == DialogResult.Yes;

                ChangeTaxonomy(currentTaxonomy, moveBlanks, undoHistory);
            }
            else
            {
                //Items to change
                List<EntityData> entityDatas = GetEntityDatas();

                Attribute newAttribute = NewValues.Att;
                if (newAttribute == null)
                {
                    if (NewValues.AttributeName != null)
                    {
                        newAttribute = Attribute.GetAttributeFromName(NewValues.AttributeName, true);
                        NewValues.Att = newAttribute;
                    }
                    //else
                    //    return false;
                }

                //if (entityDatas.Count + blanks.Count == _totalSkuCount && currentTaxonomy != null && _oldValues.Att != null)
                //{
                //    TaxonomyAttribute oldTaxAtt =
                //        _oldValues.Att.TaxonomyAttributes.Where(ta => ta.TaxonomyInfo.Equals(currentTaxonomy)).
                //            FirstOrDefault();
                //    if (oldTaxAtt != null && !_oldValues.Att.Equals(newAttribute) &&
                //        (oldTaxAtt.NavigationalOrder > 0 || oldTaxAtt.DisplayOrder > 0))
                //    {
                //        currentTaxonomy.InsertDisplayOrder(newAttribute, oldTaxAtt.DisplayOrder, false);
                //        currentTaxonomy.InsertNavigationOrder(newAttribute, oldTaxAtt.NavigationalOrder, false);
                //        oldTaxAtt.DisplayOrder = 0;
                //        oldTaxAtt.NavigationalOrder = 0;
                //        refreshColumns = true;
                //    }
                //}

                refreshColumns = ChangeOrCreateEntities(entityDatas, blanks, undoHistory, createNewEntityDatas);
            }

            return refreshColumns;
        }
        // Private Methods (7) 

        bool ChangeOrCreateEntities(
            List<EntityData> entityDatas, ICollection<Sku> blanks, Stack<EntityDataGridView.ChangeItem> undoHistory, bool createNewEntityDatas)
        {
            bool? copySchemati = null;
            var affectedTaxAtts = new DoubleKeyDictionary<TaxonomyInfo, Attribute, bool>();
            var nodesToReorderAttributeRanksIn = new HashSet<TaxonomyInfo>();

            var changeId = Guid.NewGuid();
            //List<string> columnsToRefresh= new List<string>();

            if (OldValues.Att != null && !entityDatas.Any())
            {
                foreach (var sku in blanks)
                    affectedTaxAtts[sku.Taxonomy, OldValues.Att] = true;
            }

            // Update existing Entities
            //bool entityDataAddedToRefreshList = false;
            foreach (EntityData entityData in entityDatas)
            {

                var sku = entityData.EntityInfo.Sku;
                var attribute = entityData.Attribute;

                if (AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.ContainsKeys(attribute, sku))
                    AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache[attribute].Remove(sku);
                if (NewValues.Att != null)
                {
                    if (!AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.ContainsKey(NewValues.Att))
                        AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.Add(NewValues.Att, sku, new List<EntityData>());
                    if (AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.ContainsKeys(NewValues.Att, sku))
                        AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache[NewValues.Att].Remove(sku);
                }

                if (Delete && entityData.Active)
                {
                    entityData.Active = false;
                    entityData.DeletedRemark = AryaTools.Instance.RemarkID;
                    if (undoHistory != null)
                        undoHistory.Push(new EntityDataGridView.ChangeItem(entityData, null, changeId));
                    continue;
                }

                //if (attribute.AttributeType != null && attribute.AttributeType.Equals(Attribute.AttributeTypeCalculated))
                //    return false;

                affectedTaxAtts[sku.Taxonomy, attribute] = true;

                // Make the existing EntityInfo Inactive
                if (RunTool != Tool.None)
                {
                    switch (RunTool)
                    {
                        case Tool.SliceInPlace:
                            SliceEntities(entityData, undoHistory, changeId, true);
                            break;

                        case Tool.SliceIntoNewAttributes:
                            SliceEntities(entityData, undoHistory, changeId, false);
                            break;

                        case Tool.ConvertToDecimal:
                            ConvertToDecimal(entityData, undoHistory, changeId);
                            break;

                        case Tool.ConvertToUom:
                            ConvertToUnitOfMeaure(entityData, undoHistory, changeId);
                            break;


                        case Tool.ExtractCountOfValues:
                        case Tool.ExtractMaximumValue:
                        case Tool.ExtractMinimumValue:
                            if (!_processedSkus.Contains(sku))
                            {
                                ExtractAggregate(sku, attribute.AttributeName, undoHistory, changeId);
                                _processedSkus.Add(sku);
                            }
                            break;
                    }
                    //This block must return right here.
                }

                else if (entityData.Active == false)
                {
                    entityData.Active = true;
                    entityData.Value = NewValues.Val ?? entityData.Value;
                    entityData.Uom = NewValues.FieldUomDelete ? null : NewValues.Uom ?? entityData.Uom;
                    entityData.Field1 = NewValues.Field1Delete ? null : NewValues.Field1 ?? entityData.Field1;
                    entityData.Field2 = NewValues.Field2Delete ? null : NewValues.Field2 ?? entityData.Field2;
                    entityData.Field3 = NewValues.Field3Delete ? null : NewValues.Field3 ?? entityData.Field3;

                    //Reserved for Base unit of measure and Status/PT Flag
                    //entityData.Field4 = NewValues.Field4Delete ? null : NewValues.Field4 ?? entityData.Field3;
                    //entityData.Field5 = NewValues.Field5Delete ? null : NewValues.Field5 ?? entityData.Field3;
                    LastEntityDataCreated = entityData;
                }
                else
                {
                    // Create a new entityData and add it to the entityInfo
                    entityData.Active = false;
                    var newEntityData = new EntityData
                                            {
                                                Attribute = NewValues.Att ?? attribute,
                                                Value = NewValues.Val ?? entityData.Value,
                                                Uom = NewValues.FieldUomDelete ? null : NewValues.Uom ?? entityData.Uom,
                                                Field1 = NewValues.Field1Delete ? null : NewValues.Field1 ?? entityData.Field1,
                                                Field2 = NewValues.Field2Delete ? null : NewValues.Field2 ?? entityData.Field2,
                                                Field3 = NewValues.Field3Delete ? null : NewValues.Field3 ?? entityData.Field3,
                                                Field4 = NewValues.Field4Delete ? null : NewValues.Field4 ?? entityData.Field4

                                                //Reserved for Base unit of measure and Status/PT Flag
                                                //Field4 = BaseUnitConvertion.GetBaseUnitValueAsString(NewValues.Val ?? entityData.Value, NewValues.Uom ?? entityData.Uom)  ,
                                                //Field5orStatus = NewValues.Field5 ?? entityData.Field5OrStatus
                                            };

                    if (!entityData.Sku.Project.EntityField5IsStatus)
                        newEntityData.Field5 = NewValues.Field5 ?? entityData.Field5;

                    entityData.EntityInfo.EntityDatas.Add(newEntityData);
                    LastEntityDataCreated = newEntityData;

                    // Add these two to change history for Undo
                    if (undoHistory != null)
                        undoHistory.Push(new EntityDataGridView.ChangeItem(entityData, newEntityData, changeId));
                }
            }

            if (RunTool != Tool.None)
            {
                //This block must return right here.
                switch (RunTool)
                {
                    case Tool.ConvertToDecimal:
                    case Tool.SliceInPlace:
                        return false;

                    case Tool.ExtractCountOfValues:
                    case Tool.ExtractMaximumValue:
                    case Tool.ExtractMinimumValue:
                    case Tool.SliceIntoNewAttributes:
                        return true;
                }
            }

            if (NewValues.Att != null)
                foreach (DoubleKeyPairValue<TaxonomyInfo, Attribute, bool> taxAtt in affectedTaxAtts)
                {
                    var taxonomy = taxAtt.Key1;
                    var oldAttribute = taxAtt.Key2;

                    var oldIsMapped =
                        SchemaAttribute.GetValue(taxonomy, oldAttribute, SchemaAttribute.SchemaAttributeIsMapped) != null;
                    var newIsMapped =
                        SchemaAttribute.GetValue(taxonomy, NewValues.Att, SchemaAttribute.SchemaAttributeIsMapped) != null;

                    if (oldIsMapped && !newIsMapped)
                    {
                        copySchemati = CopySchemati(copySchemati);

                        if ((bool)copySchemati)
                        {
                            var newSchemaData =
                                taxonomy.SchemaInfos.FirstOrDefault(si => si.Attribute.Equals(oldAttribute)).
                                    SchemaData;
                            SchemaAttribute.SetValue(taxonomy, NewValues.Att, true, null, newSchemaData);

                            SchemaAttribute.SecondarySchemati.ForEach(
                                schematus =>
                                {
                                    object newValue = SchemaAttribute.GetValue(taxonomy, oldAttribute, schematus);
                                    if (newValue != null)
                                        SchemaAttribute.SetValue(taxonomy, NewValues.Att, true, schematus, newValue);
                                });
                            //SchemaAttribute.UnmapNodeAttribute(taxonomy, oldAttribute);
                        }
                    }

                    /*
                     * This part is obsolete unless required
                     */

                    //var oldTaxAtt =
                    //    taxonomy.TaxonomyAttributes.FirstOrDefault(
                    //        ta => ta.Attribute.Equals(oldAttribute) && (ta.DisplayOrder > 0 || ta.NavigationalOrder > 0));
                    //var newTaxAtt =
                    //    taxonomy.TaxonomyAttributes.FirstOrDefault(
                    //        ta =>
                    //        ta.Attribute.Equals(NewValues.Att) && (ta.DisplayOrder > 0 || ta.NavigationalOrder > 0));
                    //if (oldTaxAtt != null && newTaxAtt == null)
                    //{
                    //    moveAttributeRanks = MoveAttributeRanks(moveAttributeRanks);
                    //    if ((bool)(moveAttributeRanks))
                    //    {
                    //        taxonomy.InsertNavigationOrder(NewValues.Att, oldTaxAtt.NavigationalOrder, false);
                    //        taxonomy.InsertDisplayOrder(NewValues.Att, oldTaxAtt.DisplayOrder, false);
                    //        nodesToReorderAttributeRanksIn.Add(taxonomy);
                    //    }
                    //}
                }

            //nodesToReorderAttributeRanksIn.ForEach(tax => tax.TryReorderAttributeOrders(true));

            if (!createNewEntityDatas || blanks.Count == 0)
                return (copySchemati != null && (bool)copySchemati) || nodesToReorderAttributeRanksIn.Count > 0;

            // Create new Entities
            Attribute att = NewValues.Att ??
                            (OldValues.Att ?? Attribute.GetAttributeFromName(OldValues.AttributeName, true));

            foreach (Sku sku in blanks)
            {
                if (!AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.ContainsKey(att))
                    AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.Add(att, sku, new List<EntityData>());
                if (AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.ContainsKeys(att, sku))
                    AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache[att].Remove(sku);

                var currentSku = sku;
                var newEntityData = new EntityData
                                        {
                                            Attribute = att,
                                            Value = NewValues.Val ?? string.Empty,
                                            Uom = NewValues.Uom,
                                            Field1 = NewValues.Field1,
                                            Field2 = NewValues.Field2,
                                            Field3 = NewValues.Field3,
                                            //Field4 = BaseUnitConvertion.GetBaseUnitValueAsString(NewValues.Val ?? string.Empty, NewValues.Uom)
                                            //Field5orStatus = NewValues.Field5
                                        };
                currentSku.EntityInfos.Add(new EntityInfo { EntityDatas = { newEntityData } });
                LastEntityDataCreated = newEntityData;

                // Add this to change history for Undo
                if (undoHistory != null)
                    undoHistory.Push(new EntityDataGridView.ChangeItem(null, newEntityData, changeId));
            }

            return nodesToReorderAttributeRanksIn.Count > 0;
        }

        void ChangeTaxonomy(TaxonomyInfo currentTaxonomy, bool moveBlanks, Stack<EntityDataGridView.ChangeItem> undoHistory)
        {
            var waitkey = FrmWaitScreen.ShowMessage("Moving SKUs");
            var newTaxonomy = NewValues.Tax;
            var changeId = Guid.NewGuid();

            if (currentTaxonomy != null && !newTaxonomy.SchemaInfos.SelectMany(si => si.SchemaDatas).Any(sd => sd.Active) && currentTaxonomy.SchemaInfos.SelectMany(si => si.SchemaDatas).Any(sd => sd.Active))
            {
                if (MessageBox.Show(
                    @"The selected Destination node does not have an existing schema, do you wish it to be created from the Source Taxonomy?",
                    @"Empty Schema", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    FrmWaitScreen.UpdateMessage(waitkey, "Copying Schema");
                    var sourceSchemaDataList = currentTaxonomy.SchemaInfos.Where(si => si.SchemaData != null).Select(si => si.SchemaData).ToList();

                    CopySchema(sourceSchemaDataList, newTaxonomy);
                }

            }
            else if (currentTaxonomy != null && newTaxonomy.SchemaInfos.SelectMany(si => si.SchemaDatas).Any(sd => sd.Active) && currentTaxonomy.SchemaInfos.SelectMany(si => si.SchemaDatas).Any(sd => sd.Active))
            {
                //Schema Append Process
                if (MessageBox.Show(
                    string.Format("The selected destination node has an existing schema. Do you want to append additional schema attributes from the source node to the destination node? {0}{0}NOTE: Rank order of the appended schema attributes will be preserved and placed at the end of the existing schema.", Environment.NewLine),
                    @"Append Schema", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    FrmWaitScreen.UpdateMessage(waitkey, "Copying Schema");

                    var sourceAttributeList = currentTaxonomy.SchemaInfos.Where(si => si.SchemaData != null).Select(
                        si => si.Attribute).Except
                        (
                            newTaxonomy.SchemaInfos.Where(si => si.SchemaData != null).Select(si => si.Attribute)
                        ).ToList();

                    var sourceSchemaDataList =
                        currentTaxonomy.SchemaInfos.Where(si => sourceAttributeList.Contains(si.Attribute)).Select(
                            si => si.SchemaData).ToList();

                    CopySchema(sourceSchemaDataList, newTaxonomy, true);
                }
            }

            int skuCount = 0;
            var resetTaxonomies = new List<TaxonomyInfo> { newTaxonomy };
            var skus = GetSkus().Union(moveBlanks ? GetBlanks() : new List<Sku>()).ToList();
            foreach (var sku in skus)
            {
                FrmWaitScreen.UpdateMessage(waitkey, string.Format("Moving SKUs ({0} of {1})", ++skuCount, skus.Count));

                var currentSku = sku;

                // Make existing SkuInfos Inactive
                var oldSkuInfo = currentSku.SkuInfos.FirstOrDefault(si => si.Active);
                if (oldSkuInfo != null)
                {
                    oldSkuInfo.Active = false;
                    resetTaxonomies.Add(oldSkuInfo.TaxonomyInfo);
                }
                // Create a new SkuInfo
                var newSkuInfo = new SkuInfo { TaxonomyInfo = newTaxonomy };
                currentSku.SkuInfos.Add(newSkuInfo);

                // Add these two to change history for Undo
                if (undoHistory != null)
                    undoHistory.Push(new EntityDataGridView.ChangeItem(oldSkuInfo, newSkuInfo, changeId));
            }

            resetTaxonomies.Distinct().ForEach(AryaTools.Instance.Forms.TreeForm.taxonomyTree.ResetSkuCount);

            FrmWaitScreen.HideMessage(waitkey);
        }

        private void CopySchema(IEnumerable<SchemaData> sourceSchemaDataList, TaxonomyInfo newTaxonomy, bool isAppend = false)
        {
            foreach (var srcSchemaData in sourceSchemaDataList)
            {

                var newSchemaInfo =
                    newTaxonomy.SchemaInfos.SingleOrDefault(p => p.AttributeID == srcSchemaData.SchemaInfo.AttributeID)
                    ?? new SchemaInfo
                        { AttributeID = srcSchemaData.SchemaInfo.AttributeID, TaxonomyID = newTaxonomy.ID };

                //copy LOVs
                srcSchemaData.SchemaInfo.ActiveListOfValues.ForEach(aLov =>
                                newSchemaInfo.ListOfValues.Add(new ListOfValue { Active = true, Value = aLov, SchemaID = newSchemaInfo.ID }));

                //copy active schema data
                var newSchemaData = new SchemaData
                                        {
                                            SchemaID = newSchemaInfo.ID,
                                            InSchema = srcSchemaData.InSchema,
                                            DisplayOrder = isAppend ?
                                                            (
                                                                newTaxonomy.SchemaInfos.Where(si => si.SchemaData != null).Any(si => si.SchemaData.DisplayOrder >= 500) ?
                                                                    newTaxonomy.SchemaInfos.Where(si => si.SchemaData != null).Max(si => si.SchemaData.DisplayOrder) + 1 :
                                                                    500
                                                            )
                                                                    : srcSchemaData.DisplayOrder,
                                            NavigationOrder = isAppend ? 0 : srcSchemaData.NavigationOrder,
                                            DataType = srcSchemaData.DataType,
                                            Active = true
                                        };


                //copy schema meta info
                foreach (var srcSchemaMetaInfo in srcSchemaData.SchemaInfo.SchemaMetaInfos)
                {
                    var newSchemaMetaInfo = new SchemaMetaInfo
                                                {
                                                    SchemaID = newSchemaInfo.ID,
                                                    MetaAttributeID = srcSchemaMetaInfo.MetaAttributeID
                                                };

                    //copy active schema meta data if it exists
                    if (srcSchemaMetaInfo.SchemaMetaData != null)
                    {
                        var newSchemaMetaData = new SchemaMetaData
                                                    {
                                                        Active = true,
                                                        MetaID = newSchemaMetaInfo.ID,
                                                        Value = srcSchemaMetaInfo.SchemaMetaData.Value
                                                    };
                        newSchemaMetaInfo.SchemaMetaDatas.Add(newSchemaMetaData);
                    }

                    newSchemaInfo.SchemaMetaInfos.Add(newSchemaMetaInfo);
                }

                newSchemaInfo.SchemaDatas.Add(newSchemaData);

                newTaxonomy.SchemaInfos.Add(newSchemaInfo);
            }
        }

        private void ConvertToUnitOfMeaure(EntityData entityData, Stack<EntityDataGridView.ChangeItem> undoHistory, Guid changeId)
        {
            double currentValue;
            if (!MathUtils.TryConvertToNumber(entityData.Value, out currentValue))
                return;

            var newValue = currentValue.ConvertValueUsingExpression(entityData.Uom, UnitofMeasure, UnitPrecision);

            if (newValue == double.MinValue)
                return;

            var targetUom = BaseUnitConversion.ProjectUoms.First(pu => pu.UnitOfMeasure == UnitofMeasure).Uom;

            var newEntityData = new EntityData
            {
                Attribute = entityData.Attribute,
                Value = newValue.ToString(),
                Uom = targetUom,
                Field1 = entityData.Field1,
                Field2 = entityData.Field2,
                Field3 = entityData.Field3,
                Field4 = "UoM Conversion"
            };
            entityData.Active = false;
            entityData.EntityInfo.EntityDatas.Add(newEntityData);

            if (undoHistory != null)
                undoHistory.Push(new EntityDataGridView.ChangeItem(null, newEntityData, changeId));
        }

        private void ConvertToDecimal(EntityData entityData, Stack<EntityDataGridView.ChangeItem> undoHistory, Guid changeId)
        {
            double newValue;
            if (!MathUtils.TryConvertToNumber(entityData.Value, out newValue))
                return;

            var newString = string.Format(string.Format("{{0:F{0}}}", NoOfDecimalPlaces), newValue);
            if (newString.Equals(entityData.Value))
                return;

            var newEntityData = new EntityData
                                    {
                                        Attribute = entityData.Attribute,
                                        Value = newString,
                                        Uom = entityData.Uom,
                                        Field1 = entityData.Field1,
                                        Field2 = entityData.Field2,
                                        Field3 = entityData.Field3
                                        //Field4 = BaseUnitConvertion.GetBaseUnitValueAsString(newString, entityData.Uom)
                                    };
            entityData.Active = false;
            entityData.EntityInfo.EntityDatas.Add(newEntityData);

            if (undoHistory != null)
                undoHistory.Push(new EntityDataGridView.ChangeItem(null, newEntityData, changeId));
        }

        private void ExtractAggregate(Sku sku, string attributeName, Stack<EntityDataGridView.ChangeItem> undoHistory, Guid changeId)
        {
            var values = sku.GetValuesForAttribute(attributeName);
            if (values.Count == 0)
                return;

            EntityData entityData = null;
            var newValue = string.Empty;
            var newAttributeNamePrefix = string.Empty;
            switch (RunTool)
            {
                case Tool.ExtractMinimumValue:
                    newAttributeNamePrefix = "Minimum ";
                    entityData = values.OrderBy(val => val.Value, new CompareForAlphaNumericSort()).First();
                    break;
                case Tool.ExtractMaximumValue:
                    newAttributeNamePrefix = "Maximum ";
                    entityData = values.OrderByDescending(val => val.Value, new CompareForAlphaNumericSort()).First();
                    break;
                case Tool.ExtractCountOfValues:
                    newAttributeNamePrefix = "No of ";
                    newValue = values.Count.ToString();
                    break;
            }

            var newAttribute = Attribute.GetAttributeFromName(string.Format("{0}{1}", newAttributeNamePrefix, attributeName), true);
            NewAttributeNames.Add(newAttribute.AttributeName);

            EntityData newEntityData;
            if (entityData != null)
                newEntityData = new EntityData
                                    {
                                        Attribute = newAttribute,
                                        Value = entityData.Value,
                                        Uom = entityData.Uom,
                                        Field1 = entityData.Field1,
                                        Field2 = entityData.Field2,
                                        Field3 = entityData.Field3
                                        //Field4 = BaseUnitConvertion.GetBaseUnitValueAsString(entityData.Value, entityData.Uom)
                                    };
            else
                newEntityData = new EntityData { Attribute = newAttribute, Value = newValue };

            var currentValue = sku.GetValuesForAttribute(newAttribute.AttributeName).FirstOrDefault();

            if (currentValue != null)
            {
                currentValue.EntityInfo.EntityDatas.Add(newEntityData);
                currentValue.Active = false;
            }
            else
                sku.EntityInfos.Add(new EntityInfo { EntityDatas = { newEntityData } });

            LastEntityDataCreated = newEntityData;

            if (!AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.ContainsKey(newAttribute))
                AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.Add(newAttribute, sku, new List<EntityData>());
            if (AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.ContainsKeys(newAttribute, sku))
                AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache[newAttribute].Remove(sku);

            // Add this to change history for Undo
            if (undoHistory != null)
                undoHistory.Push(new EntityDataGridView.ChangeItem(currentValue ?? null, newEntityData, changeId));
        }

        private bool MoveAttributeRanks(bool? moveAttributeRanks)
        {
            if (moveAttributeRanks == null)
            {
                moveAttributeRanks = MessageBox.Show(
                    string.Format("Do you want to copy attribute ranks to [{0}]?",
                                  NewValues.Att.AttributeName),
                    @"Move Attribute Ranks?", MessageBoxButtons.YesNo) == DialogResult.Yes;
            }
            return (bool)moveAttributeRanks;
        }

        private bool CopySchemati(bool? copySchemati)
        {
            if (copySchemati == null)
            {
                copySchemati = MessageBox.Show(
                    string.Format("Do you want to copy all Schema Meta-attributes to [{0}]?",
                                  NewValues.Att.AttributeName),
                    @"Move Schema Meta-attributes?", MessageBoxButtons.YesNo) == DialogResult.Yes;
            }
            return (bool)copySchemati;
        }

        private void SliceEntities(EntityData entityData, Stack<EntityDataGridView.ChangeItem> undoHistory, Guid changeId, bool sliceInPlace)
        {
            var parts = entityData.Value.Split(new[] { SliceDelimiter }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length <= 1) return;

            var currentSku = entityData.EntityInfo.Sku;
            Attribute currentAttribute = entityData.Attribute;
            if (sliceInPlace)
            {
                entityData.Active = false;

                if (!AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.ContainsKey(currentAttribute))
                    AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.Add(currentAttribute, currentSku, new List<EntityData>());
                if (AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.ContainsKeys(currentAttribute, currentSku))
                    AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache[currentAttribute].Remove(currentSku);
            }

            var oldEntityInfo = entityData.EntityInfo;

            if (undoHistory != null)
                undoHistory.Push(new EntityDataGridView.ChangeItem(entityData, null, changeId));

            for (int i = 0; i < parts.Length; i++)
            {
                var slice = parts[i];
                Attribute attribute = sliceInPlace
                                          ? currentAttribute
                                          : Attribute.GetAttributeFromName(
                                              string.Format(
                                                  "{0} - Part {1}", currentAttribute.AttributeName, (i + 1)),
                                              true);
                if (!sliceInPlace)
                {
                    NewAttributeNames.Add(attribute.AttributeName);

                    if (!AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.ContainsKey(attribute))
                        AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.Add(
                            attribute, currentSku, new List<EntityData>());
                    if (AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.ContainsKeys(attribute, currentSku))
                        AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache[attribute].Remove(currentSku);
                }

                var newEntityData = new EntityData
                                        {
                                            Attribute = attribute,
                                            Value = slice.Trim(),
                                            Uom = entityData.Uom,
                                            Field1 = entityData.Field1,
                                            Field2 = entityData.Field2,
                                            Field3 = entityData.Field3
                                            //Field4 = BaseUnitConvertion.GetBaseUnitValueAsString(slice.Trim(), entityData.Uom)
                                            //Field5orStatus = entityData.Field5OrStatus
                                        };

                //preserve the first link
                if (i == 0)
                {
                    oldEntityInfo.EntityDatas.Add(newEntityData);
                }
                else
                {
                    currentSku.EntityInfos.Add(new EntityInfo { EntityDatas = { newEntityData } });
                }

                LastEntityDataCreated = newEntityData;

                if (undoHistory != null)
                    undoHistory.Push(new EntityDataGridView.ChangeItem(null, newEntityData, changeId));
            }
        }
        // Internal Methods (7) 

        internal void AddNew(EntityData entityData)
        {
            _entityDatas.Add(entityData);

        }

        internal void Add(EntityData entityData)
        {
            if (entityData == null || entityData.EntityInfo == null || entityData.EntityInfo.Sku == null)
                return;

            _entityDatas.Add(entityData);
            _skus.Add(entityData.EntityInfo.Sku);
        }

        internal void Add(EntityData entityData, Sku sku)
        {
            if(entityData == null || sku == null) return;

            _entityDatas.Add(entityData);
            _skus.Add(sku);
        }

        internal void Add(Sku sku)
        {
            _skus.Add(sku);
        }

        internal void AddBlank(Sku sku)
        {
            _blankCells.Add(sku);
        }

        internal List<Sku> GetBlanks()
        {
            return _blankCells;
        }

        internal List<EntityData> GetEntityDatas()
        {
            return _entityDatas;
        }

        internal List<Sku> GetSkus()
        {
            return _skus;
        }

        internal bool HasChange()
        {
            return Delete || RunTool != Tool.None || NewValues.Tax != null || NewValues.Att != null ||
                   NewValues.AttributeName != null || NewValues.Val != null || NewValues.Uom != null ||
                   NewValues.Field1 != null || NewValues.Field2 != null || NewValues.Field3 != null ||
                   NewValues.Field4 != null || NewValues.Field5 != null || NewValues.FieldUomDelete ||
                   NewValues.Field1Delete || NewValues.Field2Delete || NewValues.Field3Delete || NewValues.Field4Delete ||
                   NewValues.Field5Delete;
        }

        #endregion Methods



        #region Nested type: AllFields

        internal class AllFields
        {
            internal Attribute Att { get; set; }
            internal TaxonomyInfo Tax { get; set; }

            private string _attributeName;
            internal string AttributeName
            {
                get { return _attributeName; }
                set {
                    _attributeName = string.IsNullOrWhiteSpace(value) ? null : value;
                }
            }

            private string _field1;
            internal string Field1
            {
                get { return _field1; }
                set { _field1 = string.IsNullOrWhiteSpace(value) ? null : value; }
            }

            private string _field2;
            internal string Field2
            {
                get { return _field2; }
                set { _field2 = string.IsNullOrWhiteSpace(value) ? null : value; }
            }

            private string _field3;
            internal string Field3
            {
                get { return _field3; }
                set { _field3 = string.IsNullOrWhiteSpace(value) ? null : value; }
            }

            private string _field4;
            internal string Field4
            {
                get { return _field4; }
                set { _field4 = string.IsNullOrWhiteSpace(value) ? null : value; }
            }

            private string _field5;
            internal string Field5
            {
                get { return _field5; }
                set { _field5 = string.IsNullOrWhiteSpace(value) ? null : value; }
            }
            
            private string _uom;
            internal string Uom
            {
                get { return _uom; }
                set { _uom = string.IsNullOrWhiteSpace(value) ? null : value; }
            }

            private string _val;
            internal string Val
            {
                get { return _val; }
                set { _val = string.IsNullOrWhiteSpace(value) ? null : value; }
            }

            public bool Field1Delete { get; set; }
            public bool Field2Delete { get; set; }
            public bool Field3Delete { get; set; }
            public bool Field4Delete { get; set; }
            public bool Field5Delete { get; set; }
            public bool FieldUomDelete { get; set; }
        }

        #endregion
    }
}