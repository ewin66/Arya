using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LinqKit;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Math;
using Attribute = Arya.Data.Attribute;
using EntityData = Arya.Data.EntityData;
using Sku = Arya.Data.Sku;
using TaxonomyInfo = Arya.Data.TaxonomyInfo;

namespace Arya.HelperClasses
{
    // This class is used to caluculate the value of a Calculated attribute for a sku in a taxonomy node
    internal class TaxAttValueCalculator
    {
        #region Fields

        private const string AllOtherAttributesExpression = "[...]";
        private const string AllOtherAttributesToken = "...";
        private const string AttributeNameExpression = "[Att Name]";
        private const string AttributeNameToken = "att";
        private const string AttributeValueExpression = "[Att Value]";
        private const string AttributeValueToken = "val";
        private const string DisplayAttributesExpression = "[Other Display Attributes]";
        private const string DisplayAttributesToken = "dispAtt";
        private const string InSchemaAttributesExpression = "[Other InSchema Attributes]";
        private const string InSchemaAttributesToken = "inschAtt";
        private const string NavigationAttributesExpression = "[Other Navigation Attributes]";
        private const string NavigationAttributesToken = "navAtt";
        private const string NewTaxonomyExpression = "[Special: New Taxonomy]";
        private const string NodeNameExpression = "[Special: Node Name]";
        private const string OldTaxonomyExpression = "[Special: Old Taxonomy]";
        private const string SubAttributeValueOnlyExpression = "#ValueOnly#";
        private const string SubUomExpression = "#Uom#";
        private const string UomExpression = "[Special: Uom]";

        private static readonly Regex RxEqualTo = new Regex("(.+?)==(.+)", RegexOptions.Compiled);
        private static readonly Regex RxFormatAttributes = new Regex("(.+?):(.+)", RegexOptions.Compiled);
        private static readonly Regex RxIfelse = new Regex("(.+?)\\?(.+?):(.+)", RegexOptions.Compiled);
        private static readonly Regex RxIsnull = new Regex("(.+?)\\?\\?(.+)", RegexOptions.Compiled);
        private static readonly Regex RxMath = new Regex("{([^{}]+)}", RegexOptions.Compiled);
        private static readonly Regex RxParen = new Regex("\\(([^\\(\\)]+)\\)", RegexOptions.Compiled);

        private static readonly Regex RxQuotesAndBrackets = new Regex("(\"(?:\"\"|.)*?\")|(\\[[^\\[\\]]+\\])",
            RegexOptions.Compiled);

        private static readonly Regex RxWhiteSpace = new Regex("\\s+", RegexOptions.Compiled);

        private readonly Sku _sku;
        private readonly TaxonomyInfo sourceTaxonomy;

        private List<Token> _tokens;

        #endregion Fields

        #region Constructors

        public TaxAttValueCalculator(Sku sku)
        {
            _sku = sku;
            sourceTaxonomy = _sku.Taxonomy;
        }

        #endregion Constructors

        #region Enumerations

        public enum TokenType
        {
            Attribute = 1,
            AttributeValueOnly = 2,
            Constant = 4,
            Uom = 3,
            Special = 5
        }

        #endregion Enumerations

        #region Methods

        // Public Methods (3)
        public IEnumerable<string> GetAttributesInTokens()
        {
            //return
            //    _tokens.Where(t => t.Expression.StartsWith("[") && t.Expression.EndsWith("]")).Select(
            //        t => t.Expression.Substring(1, t.Expression.Length - 2));
            return
                _tokens.Where(p => p.CurrentTokenType == TokenType.Attribute)
                    .Select(p => p.Expression)
                    .Distinct()
                    .ToList();
        }

        public EntityData ProcessCalculatedAttribute(string expression, int maxResultLength)
        {
            //Replace strings and attributes with tokens
            _tokens = new List<Token>();
            try
            {
                var result = expression;
                //First replace all quoted strings and brackets
                result = RxQuotesAndBrackets.Replace(result, m => CreateToken(m.Value, false));

                //evaluate all the tokens in the order of enums
                _tokens.OrderBy(p => p.CurrentTokenType).ForEach(p => p.Evaluate(_tokens));

                result = RxWhiteSpace.Replace(result, string.Empty);
                result = ProcessExpression(result, maxResultLength);
                var tokenValue = GetTokenValue(result);
                var ed = new EntityData(false) {Value = RxWhiteSpace.Replace(tokenValue.Trim(), " ")};

                if (!string.IsNullOrWhiteSpace(ed.Value))
                {
                    var allUoms =
                        _tokens.Where(p => p.CurrentTokenType == TokenType.Attribute && !string.IsNullOrEmpty(p.Value))
                            .Select(p => p.Uom)
                            .Distinct()
                            .ToList();
                    ed.Uom = allUoms.Count == 1 ? allUoms[0] : null;
                }

                return ed;
            }
            catch (Exception)
            {
                return new EntityData(false) {Value = "<Error>"};
            }
        }

        public EntityData ProcessCalculatedAttribute(Attribute att, TaxonomyInfo tax,
            bool getDefaultForNullTaxonomy = true)
        {
            if (tax == null)
            {
                var globalDerivedAttribute = att.DerivedAttributes.FirstOrDefault(p => p.TaxonomyID == Guid.Empty);

                if (globalDerivedAttribute != null)
                {
                    return ProcessCalculatedAttribute(globalDerivedAttribute.Expression,
                        globalDerivedAttribute.MaxResultLength);
                }

                tax = _sku.Taxonomy;
                if (tax == null || !getDefaultForNullTaxonomy)
                    return new EntityData(false) {Value = String.Empty};
            }

            var calculatedAttribute =
                att.DerivedAttributes.FirstOrDefault(da => da.TaxonomyInfo != null && da.TaxonomyID == tax.ID);

            if (calculatedAttribute == null && att.DerivedAttributes.Any(da => da.TaxonomyInfo == null))
                calculatedAttribute = att.DerivedAttributes.First(da => da.TaxonomyInfo == null);

            //I wonder why we are setting the calculatedAttribute to NULL if an Active InSchema SchemaData exists
            //if (calculatedAttribute != null)
            //{
            //    if (sourceTaxonomy != null)
            //    {
            //        var schemaInfo = sourceTaxonomy.SchemaInfos.SingleOrDefault(p => p.AttributeID == att.ID);
            //        if (schemaInfo != null && !schemaInfo.SchemaDatas.Any(sd => sd.Active && sd.InSchema))
            //            calculatedAttribute = null;
            //    }
            //}

            if (calculatedAttribute == null)
                return ProcessCalculatedAttribute(att, tax.TaxonomyData.ParentTaxonomyInfo, false);

            return ProcessCalculatedAttribute(calculatedAttribute.Expression, calculatedAttribute.MaxResultLength);
        }

        // Private Methods (8)
        private string CreateToken(string expression, bool evaluate = true)
        {
            // A token starts with a 't' followed by the token number
            if (expression.Equals(AttributeNameExpression))
                return AttributeNameToken;
            if (expression.Equals(AttributeValueExpression))
                return AttributeValueToken;
            if (expression.Equals(AllOtherAttributesExpression))
                return AllOtherAttributesToken;
            if (expression.Equals(NavigationAttributesExpression))
                return NavigationAttributesToken;
            if (expression.Equals(DisplayAttributesExpression))
                return DisplayAttributesToken;
            if (expression.Equals(InSchemaAttributesExpression))
                return InSchemaAttributesToken;

            var token = _tokens.FirstOrDefault(t => t.Expression.Equals(expression));
            if (token != null)
                return token.Id;

            token = new Token(_tokens.Count, _sku, expression);
            _tokens.Add(token);
            if (evaluate)
                token.Evaluate(_tokens);
            return token.Id;
        }

        private string CreateToken(string expression, TokenType tokenType, bool evaluate = true)
        {
            // A token starts with a 't' followed by the token number
            if (expression.Equals(AttributeNameExpression))
                return AttributeNameToken;
            if (expression.Equals(AttributeValueExpression))
                return AttributeValueToken;
            if (expression.Equals(AllOtherAttributesExpression))
                return AllOtherAttributesToken;
            if (expression.Equals(NavigationAttributesExpression))
                return NavigationAttributesToken;
            if (expression.Equals(DisplayAttributesExpression))
                return DisplayAttributesToken;
            if (expression.Equals(InSchemaAttributesToken))
                return InSchemaAttributesToken;

            var token = _tokens.FirstOrDefault(t => t.Expression.Equals(expression));
            if (token != null)
                return token.Id;

            token = new Token(_tokens.Count, _sku, expression, tokenType);
            _tokens.Add(token);
            if (evaluate)
                token.Evaluate(_tokens);
            return token.Id;
        }

        private string GetAllOtherAttributes()
        {
            var attributesInTokens = GetAttributesInTokens();

            var allOtherSkuAttributes =
                _sku.EntityInfos.SelectMany(ei => ei.EntityDatas.Where(ed => ed.Active))
                    .Select(ed => ed.Attribute)
                    .Where(att => att.Type == AttributeTypeEnum.Sku)
                    .Distinct()
                    .Select(att => att.AttributeName)
                    .Where(attName => !attributesInTokens.Contains(attName))
                    .OrderBy(attName => attName)
                    .Distinct();

            var otherAttributeTokens = string.Empty;
            allOtherSkuAttributes.ForEach(
                att =>
                    otherAttributeTokens +=
                        string.Format("{0}{1}", string.IsNullOrEmpty(otherAttributeTokens) ? string.Empty : ",",
                            CreateToken(string.Format("[{0}]", att))));

            return otherAttributeTokens;
        }

        private string GetOtherDisplayAttributes()
        {
            var attributesInTokens = GetAttributesInTokens().ToList();

            var skuAttributes =
                _sku.EntityInfos.SelectMany(ei => ei.EntityDatas.Where(ed => ed.Active)).Select(ed => ed.Attribute);

            var dispAtts =
                skuAttributes.SelectMany(sa => sa.SchemaInfos)
                    .Where(
                        si =>
                            si.TaxonomyInfo.Equals(_sku.Taxonomy)
                            && !attributesInTokens.Contains(si.Attribute.AttributeName) && si.SchemaData != null)
                    .Where(si => si.SchemaData.DisplayOrder > 0)
                    .OrderBy(si => si.SchemaData.DisplayOrder)
                    .Select(si => si.Attribute.AttributeName)
                    .Distinct();

            var displayAttributes = string.Empty;

            dispAtts.ForEach(
                da =>
                    displayAttributes +=
                        string.Format("{0}{1}", string.IsNullOrEmpty(displayAttributes) ? string.Empty : ",",
                            CreateToken(string.Format("[{0}]", da))));

            return displayAttributes;
        }

        private string GetOtherInSchemaAttributes()
        {
            var attributesInTokens = GetAttributesInTokens().ToList();

            var skuAttributes =
                _sku.EntityInfos.SelectMany(ei => ei.EntityDatas.Where(ed => ed.Active)).Select(ed => ed.Attribute);

            var inschemaAtts =
                skuAttributes.SelectMany(sa => sa.SchemaInfos)
                    .Where(
                        si =>
                            si.TaxonomyInfo.Equals(_sku.Taxonomy)
                            && !attributesInTokens.Contains(si.Attribute.AttributeName) && si.SchemaData != null)
                    .Where(si => si.SchemaData.InSchema)
                    .OrderBy(si => si.SchemaData.DisplayOrder)
                    .Select(si => si.Attribute.AttributeName)
                    .Distinct();

            var inschemaAttributes = string.Empty;

            inschemaAtts.ForEach(
                da =>
                    inschemaAttributes +=
                        string.Format("{0}{1}", string.IsNullOrEmpty(inschemaAttributes) ? string.Empty : ",",
                            CreateToken(string.Format("[{0}]", da))));

            return inschemaAttributes;
        }

        private string GetOtherNavigationAttributes()
        {
            var attributesInTokens = GetAttributesInTokens().ToList();

            var skuAttributes =
                _sku.EntityInfos.SelectMany(ei => ei.EntityDatas.Where(ed => ed.Active)).Select(ed => ed.Attribute);

            var navAtts = skuAttributes.SelectMany(sa => sa.SchemaInfos).Where(si => // si.TaxonomyInfo !=null &&
                si.TaxonomyID == _sku.Taxonomy.ID && !attributesInTokens.Contains(si.Attribute.AttributeName)
                && si.SchemaData != null)
                .Where(si => si.SchemaData != null && si.SchemaData.NavigationOrder > 0)
                .OrderBy(si => si.SchemaData.NavigationOrder)
                .Select(si => si.Attribute.AttributeName)
                .Distinct();

            var navigationalAttributes = string.Empty;

            navAtts.ForEach(
                na =>
                    navigationalAttributes +=
                        string.Format("{0}{1}", string.IsNullOrEmpty(navigationalAttributes) ? string.Empty : ",",
                            CreateToken(string.Format("[{0}]", na))));

            return navigationalAttributes;
        }

        //private Token GetToken(string tokenToProcess)
        //{
        //    int pos;
        //    if (tokenToProcess.StartsWith("t") && tokenToProcess.Length > 1 &&
        //        int.TryParse(tokenToProcess.Substring(1), out pos))
        //        return _tokens[pos];
        //    return null;
        //}
        private string GetTokenValue(string tokenToProcess)
        {
            int pos;
            if (tokenToProcess.StartsWith("t") && tokenToProcess.Length > 1
                && int.TryParse(tokenToProcess.Substring(1), out pos))
                return _tokens[pos].Value;

            return string.Empty;
        }

        private string ProcessExpression(string exp, int maxLength)
        {
            //Whenever this function is modified, GenerateExpressionTree function MUST be modified accordingly

            // First, solve the parenthesis
            if (RxParen.IsMatch(exp))
            {
                var processed = RxParen.Replace(exp, m => ProcessExpression(m.Groups[1].Value, maxLength));
                return ProcessExpression(processed, maxLength);
            }

            // Solve Mathematical expression
            if (RxMath.IsMatch(exp))
            {
                var mathProcessed = RxMath.Replace(exp,
                    m =>
                        CreateToken(String.Format("\"{0}\"", ProcessMath(m.Groups[1].Value, maxLength)),
                            TokenType.Constant));
                return ProcessExpression(mathProcessed, maxLength);
            }

            // Solve the ternary operator ?:
            if (RxIfelse.IsMatch(exp))
            {
                var m = RxIfelse.Match(exp);
                var condition = ProcessExpression(m.Groups[1].Value, maxLength);
                return !string.IsNullOrEmpty(GetTokenValue(condition))
                    ? ProcessExpression(m.Groups[2].Value, maxLength)
                    : ProcessExpression(m.Groups[3].Value, maxLength);
            }

            // Solve the isnull operator ??
            if (RxIsnull.IsMatch(exp))
            {
                var m = RxIsnull.Match(exp);
                var left = ProcessExpression(m.Groups[1].Value, maxLength);
                return !string.IsNullOrEmpty(GetTokenValue(left))
                    ? left
                    : ProcessExpression(m.Groups[2].Value, maxLength);
            }

            // Solve the Format Attribute(s) operator [att1],[att2],[...]:.+
            if (RxFormatAttributes.IsMatch(exp))
            {
                var m = RxFormatAttributes.Match(exp);
                var atts = m.Groups[1].Value;
                if (atts.Contains(NavigationAttributesToken))
                    atts = atts.Replace(NavigationAttributesToken, GetOtherNavigationAttributes());
                if (atts.Contains(DisplayAttributesToken))
                    atts = atts.Replace(DisplayAttributesToken, GetOtherDisplayAttributes());
                if (atts.Contains(InSchemaAttributesToken))
                    atts = atts.Replace(InSchemaAttributesToken, GetOtherInSchemaAttributes());
                if (atts.Contains(AllOtherAttributesToken))
                    atts = atts.Replace(AllOtherAttributesToken, GetAllOtherAttributes());
                var attributes = atts.Split(new[] {','});

                var formatAttributeResult = String.Empty;
                foreach (var att in attributes)
                {
                    var attValue = GetTokenValue(att);
                    if (string.IsNullOrEmpty(attValue))
                        continue;

                    var currentAttribute = att;
                    var attributeToken = _tokens.First(t => t.Id.Equals(currentAttribute));
                    //string attributeNameStringToken =
                    //    CreateToken(
                    //        string.Format(
                    //            "\"{0}\"", attributeToken.Expression.Substring(1, attributeToken.Expression.Length - 2)));

                    var attributeNameStringToken =
                        CreateToken(string.Format("\"{0}\"", attributeToken.Expression.Trim()));

                    var newExpression =
                        m.Groups[2].Value.Replace(AttributeNameToken, attributeNameStringToken)
                            .Replace(AttributeValueToken, currentAttribute);

                    var additive = ProcessExpression(newExpression, 0);
                    formatAttributeResult += (string.IsNullOrEmpty(formatAttributeResult) ? string.Empty : "+")
                                             + additive;
                }
                return formatAttributeResult;
            }

            // Solve binary operators, currently only ==
            if (RxEqualTo.IsMatch(exp))
            {
                var m = RxEqualTo.Match(exp);
                var leftValue = GetTokenValue(ProcessExpression(m.Groups[1].Value, maxLength));
                var rightValue = GetTokenValue(ProcessExpression(m.Groups[2].Value, maxLength));
                var returnValue = leftValue.Equals(rightValue, StringComparison.OrdinalIgnoreCase)
                    ? "true"
                    : string.Empty;
                return CreateToken(String.Format("\"{0}\"", returnValue), TokenType.Constant);
            }

            // Finally solve the concatenate operators +
            var parts = exp.Split(new[] {'+'}, StringSplitOptions.None);
            var result = string.Empty;

            foreach (var t in parts)
            {
                if (maxLength == 0 || result.Length + GetTokenValue(t).Length <= maxLength)
                    result += GetTokenValue(t);
            }

            return CreateToken(String.Format("\"{0}\"", result), TokenType.Constant);
        }

        private double ProcessMath(string expression, int maxLength)
        {
            if (expression.Contains('%'))
            {
                var indexOfMod = expression.IndexOf('%');
                return ProcessMath(expression.Substring(0, indexOfMod), maxLength)
                       %(expression.Length > indexOfMod
                           ? ProcessMath(expression.Substring(indexOfMod + 1), maxLength)
                           : 1);
            }
            if (expression.Contains('/'))
            {
                var indexOfSlash = expression.IndexOf('/');
                return ProcessMath(expression.Substring(0, indexOfSlash), maxLength)
                       /(expression.Length > indexOfSlash
                           ? ProcessMath(expression.Substring(indexOfSlash + 1), maxLength)
                           : 1);
            }
            if (expression.Contains('*'))
            {
                var indexOfStar = expression.IndexOf('*');
                return ProcessMath(expression.Substring(0, indexOfStar), maxLength)
                       *(expression.Length > indexOfStar
                           ? ProcessMath(expression.Substring(indexOfStar + 1), maxLength)
                           : 1);
            }
            if (expression.Contains('+'))
            {
                var indexOfPlus = expression.IndexOf('+');
                return ProcessMath(expression.Substring(0, indexOfPlus), maxLength)
                       + (expression.Length > indexOfPlus
                           ? ProcessMath(expression.Substring(indexOfPlus + 1), maxLength)
                           : 0);
            }
            if (expression.Contains('-'))
            {
                var indexOfMinus = expression.IndexOf('-');
                return ProcessMath(expression.Substring(0, indexOfMinus), maxLength)
                       - (expression.Length > indexOfMinus
                           ? ProcessMath(expression.Substring(indexOfMinus + 1), maxLength)
                           : 0);
            }

            double doubleResult;
            var stringResult = GetTokenValue(expression);
            MathUtils.TryConvertToNumber(stringResult, out doubleResult);
            return doubleResult;
        }

        #endregion Methods

        #region Nested Types

        private class Token
        {
            #region Fields

            private readonly string _expression;
            private readonly string _id;
            private readonly Sku _sku;
            private readonly TokenType tokenType;

            private string _uom;
            private string _value;

            #endregion Fields

            #region Constructors

            public Token(int id, Sku sku, string expression)
            {
                _sku = sku;
                _id = "t" + id;

                if (expression.IndexOf("[Special:", StringComparison.Ordinal) > -1)
                    tokenType = TokenType.Special;
                else if (expression.StartsWith("[") && expression.EndsWith("]"))
                {
                    expression = expression.Substring(1, expression.Length - 2).Trim();
                    if (expression.Contains(SubUomExpression))
                    {
                        tokenType = TokenType.Uom;
                        expression = expression.Replace(SubUomExpression, string.Empty).Trim();
                    }
                    else if (expression.Contains(SubAttributeValueOnlyExpression))
                    {
                        tokenType = TokenType.AttributeValueOnly;
                        expression = expression.Replace(SubAttributeValueOnlyExpression, string.Empty).Trim();
                    }
                    else
                        tokenType = TokenType.Attribute;
                }
                else if (expression.StartsWith("\"") && expression.EndsWith("\""))
                    tokenType = TokenType.Constant;

                _expression = expression;
            }

            public Token(int id, Sku sku, string expression, TokenType tokenType)
            {
                _sku = sku;
                _id = "t" + id;
                _expression = expression;
                this.tokenType = tokenType;
            }

            #endregion Constructors

            #region Properties

            public TokenType CurrentTokenType
            {
                get { return tokenType; }
            }

            public string Expression
            {
                get { return _expression; }
            }

            public string Id
            {
                get { return _id; }
            }

            public string Uom
            {
                get { return _uom ?? (_uom = string.Empty); }
                private set { _uom = value; }
            }

            public string Value
            {
                get { return _value ?? (_value = string.Empty); }
                private set { _value = value; }
            }

            #endregion Properties

            #region Methods

            public void Evaluate(IEnumerable<Token> existingTokens) { CalculateValue(existingTokens); }

            private void CalculateValue(IEnumerable<Token> existingTokens)
            {
                var token = _expression;

                if (tokenType == TokenType.Attribute || tokenType == TokenType.AttributeValueOnly)
                {
                    var entities =
                        _sku.EntityInfos.SelectMany(ei => ei.EntityDatas.Where(ed => ed.Active))
                            .Where(
                                ed =>
                                    ed.Attribute.AttributeName != null
                                    && ed.Attribute.AttributeName.Equals(token, StringComparison.OrdinalIgnoreCase))
                            .OrderBy(ed => ed.Value)
                            .ToList();

                    if (entities.Count == 0)
                    {
                        var attribute = Attribute.GetAttributeFromName(token, false);
                        if (attribute != null && attribute.AttributeType == AttributeTypeEnum.Derived.ToString())
                        {
                            entities = new List<EntityData>
                                       {
                                           new TaxAttValueCalculator(_sku)
                                               .ProcessCalculatedAttribute(attribute, _sku.Taxonomy)
                                       };
                        }
                    }

                    if (entities.Count == 0)
                        return;

                    if (entities.Count == 1)
                    {
                        var ed = entities[0];
                        _value = ed.Value ?? string.Empty;
                        if (tokenType == TokenType.Attribute && !string.IsNullOrEmpty(ed.Uom))
                            Value += ed.Uom;
                        Uom = ed.Uom;
                        return;
                    }

                    var value = string.Empty;
                    var allUoms = new HashSet<string>();
                    foreach (var entityData in entities)
                    {
                        value += (string.IsNullOrEmpty(value) ? string.Empty : ", ") + entityData.Value;
                        allUoms.Add(entityData.Uom);
                        if (!string.IsNullOrEmpty(entityData.Uom))
                        {
                            if (tokenType == TokenType.Attribute)
                                value += entityData.Uom;
                        }
                    }

                    Value = value;

                    if (allUoms.Count == 1)
                        Uom = allUoms.First();

                    return;
                }

                if (tokenType == TokenType.Uom)
                {
                    var allUomValues =
                        _sku.EntityInfos.SelectMany(ei => ei.EntityDatas.Where(ed => ed.Active))
                            .Where(
                                ed =>
                                    ed.Attribute.AttributeName != null
                                    && ed.Attribute.AttributeName.Equals(token, StringComparison.OrdinalIgnoreCase))
                            .Select(p => p.Uom)
                            .Distinct()
                            .ToList();

                    Value = allUomValues.Count == 1 ? allUomValues[0] : string.Empty;
                    return;
                }

                if (tokenType == TokenType.Constant)
                {
                    Value = token.Substring(1, token.Length - 2).Replace("\"\"", "\"");
                    return;
                }

                if (tokenType == TokenType.Special)
                {
                    if (token.Equals(NewTaxonomyExpression))
                    {
                        var tax = _sku.Taxonomy;
                        Value = tax != null ? tax.ToString() : string.Empty;
                        return;
                    }

                    if (token.Equals(OldTaxonomyExpression))
                    {
                        Value = _sku.OldTaxonomy;
                        return;
                    }

                    if (token.Equals(UomExpression))
                    {
                        var allUoms =
                            existingTokens.Where(
                                p =>
                                    p._id != _id && p.CurrentTokenType == TokenType.Attribute
                                    && !string.IsNullOrEmpty(p.Value)).Select(p => p.Uom).Distinct().ToList();
                        Value = allUoms.Count == 1 ? allUoms[0] : null;
                        return;
                    }

                    if (token.Equals(NodeNameExpression))
                    {
                        var tax = _sku.Taxonomy;
                        Value = tax != null ? tax.TaxonomyData.NodeName : string.Empty;
                    }
                }
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}