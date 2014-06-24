using System;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using FreeHandFilters.Filters;
using LinqKit;
using Arya.Framework.Data;
using Arya.Framework.Data.AryaDb;
using Sku = Arya.Data.Sku;

namespace Arya.HelperClasses
{
    public class CustomFilters
    {
        #region Nested type: NewEqualToFilterOperator

        public class NewEqualToFilterOperator : EqualToFilterOperator
        {
            public override LambdaExpression AssignFilter(Identifier attributeName, StringLiteral value)
            {
                string attr = attributeName.GetValue().ToString();
                string val = value.GetValue().ToString();

                Expression<Func<Sku, bool>> result = p => false;

                switch (attributeName.GetIdentifierType())
                {
                    case IdentifierType.AttributeName:
                        result =
                            p =>
                            p.EntityInfos.Any(
                                q =>
                                q.EntityDatas.Any(r => r.Active && r.Attribute.AttributeName == attr && r.Value == val));
                        break;
                    case IdentifierType.Field1:
                        result =
                            p =>
                            p.EntityInfos.FirstOrDefault(q => q.EntityDatas.Any(r => r.Active && r.Field1 == val)) != null;
                        break;
                    case IdentifierType.Field2:
                        result =
                            p =>
                            p.EntityInfos.FirstOrDefault(q => q.EntityDatas.Any(r => r.Active && r.Field2 == val)) != null;
                        break;
                    case IdentifierType.Field3:
                        result =
                            p =>
                            p.EntityInfos.FirstOrDefault(q => q.EntityDatas.Any(r => r.Active && r.Field3 == val)) != null;
                        break;
                    case IdentifierType.Field4:
                        result =
                            p =>
                            p.EntityInfos.FirstOrDefault(q => q.EntityDatas.Any(r => r.Active && r.Field4 == val)) != null;
                        break;
                    case IdentifierType.Field5:
                        result =
                            p =>
                            p.EntityInfos.FirstOrDefault(q => q.EntityDatas.Any(r => r.Active && r.Field5OrStatus == val)) != null;
                        break;
                    case IdentifierType.ANameValue:
                        result =
                            p =>
                            p.EntityInfos.FirstOrDefault(q => q.EntityDatas.Any(r => r.Active && r.Attribute.AttributeName == val)) != null;
                        break;
                    case IdentifierType.ItemID:
                        result = p => p.ItemID == val;
                        break;
                    case IdentifierType.AType:

                        if (val.Equals("Extended", StringComparison.CurrentCultureIgnoreCase))
                        {
                            result =
                                p =>
                                p.EntityInfos.Any(
                                    q => q.EntityDatas.Any(r => r.Active && r.Attribute.Type == AttributeTypeEnum.Sku)) == false;
                        }
                        else
                        {
                            result =
                                p =>
                                p.EntityInfos.Any(
                                    q =>
                                    q.EntityDatas.Any(
                                        r =>
                                        r.Active && r.Attribute.Type != AttributeTypeEnum.Sku &&
                                        r.Attribute.AttributeType == val)) == false;
                        }
                        break;
                }


                return result;
            }
        }

        #endregion

        #region Nested type: NewLikeFilterOperator

        public class NewLikeFilterOperator : LikeFilterOperator
        {
            public override LambdaExpression AssignFilter(Identifier attributeName, StringLiteral value)
            {
                string attr = attributeName.GetValue().ToString();
                string val = value.GetValue().ToString();

                Expression<Func<Sku, bool>> result = p => false;

                switch (attributeName.GetIdentifierType())
                {
                    case IdentifierType.AttributeName:
                        result =
                            p =>
                            p.EntityInfos.FirstOrDefault(q => q.EntityDatas.Any(
                                r => r.Active && r.Attribute.AttributeName == attr && SqlMethods.Like(r.Value, val))) != null;
                        break;
                    case IdentifierType.Field1:
                        result =
                            p =>
                            p.EntityInfos.FirstOrDefault(q => q.EntityDatas.Any(r => r.Active && SqlMethods.Like(r.Field2, val))) != null;
                        break;
                    case IdentifierType.Field2:
                        result =
                            p =>
                            p.EntityInfos.FirstOrDefault(q => q.EntityDatas.Any(r => r.Active && SqlMethods.Like(r.Field2, val))) != null;
                        break;
                    case IdentifierType.Field3:
                        result =
                            p =>
                            p.EntityInfos.FirstOrDefault(q => q.EntityDatas.Any(r => r.Active && SqlMethods.Like(r.Field3, val))) != null;
                        break;
                    case IdentifierType.Field4:
                        result =
                            p =>
                            p.EntityInfos.FirstOrDefault(q => q.EntityDatas.Any(r => r.Active && SqlMethods.Like(r.Field4, val))) != null;
                        break;
                    case IdentifierType.Field5:
                        result =
                            p =>
                            p.EntityInfos.FirstOrDefault(q => q.EntityDatas.Any(r => r.Active && SqlMethods.Like(r.Field5OrStatus, val))) != null;
                        break;
                    case IdentifierType.ANameValue:
                        result =
                            p =>
                            p.EntityInfos.FirstOrDefault(q => q.EntityDatas.Any(r => r.Active && SqlMethods.Like(r.Attribute.AttributeName, val))) != null;
                        break;
                    case IdentifierType.ItemID:
                        result = p => SqlMethods.Like(p.ItemID, val);
                        break;

                    case IdentifierType.AType:

                        if (val.Contains("Extended"))
                        {
                            result =
                                p =>
                                p.EntityInfos.FirstOrDefault(q => q.EntityDatas.Any(r => r.Active && r.Attribute.Type == AttributeTypeEnum.Sku)) != null;
                        }
                        else
                        {
                            result =
                                p =>
                                p.EntityInfos.FirstOrDefault(q => q.EntityDatas.Any(
                                    r =>
                                    r.Active && SqlMethods.Like(r.Attribute.AttributeType, val) &&
                                    r.Attribute.Type != AttributeTypeEnum.Sku)) != null;
                        }
                        break;
                }

                return result;
            }
        }

        #endregion

        #region Nested type: NewNotEqualToFilterOperator

        public class NewNotEqualToFilterOperator : NotEqualToFilterOperator
        {
            public override LambdaExpression AssignFilter(Identifier attributeName, StringLiteral value)
            {
                string attr = attributeName.GetValue().ToString();
                string val = value.GetValue().ToString();

                Expression<Func<Sku, bool>> result = p => false;

                switch (attributeName.GetIdentifierType())
                {
                    case IdentifierType.AttributeName:

                        //Sku should have the attribute first and then value should match with the input.
                        //stops considering blanks
                        Expression<Func<Sku, bool>> x =
                            p =>
                            p.EntityInfos.Any(
                                q =>
                                q.EntityDatas.Any(r => r.Active && r.Attribute.AttributeName == attr && r.Value == val)) ==
                            false;
                        Expression<Func<Sku, bool>> y =
                            p =>
                            p.EntityInfos.FirstOrDefault(q => q.EntityDatas.Any(r => r.Active && r.Attribute.AttributeName == val)) != null;
                        result = x.And(y);
                        break;
                    case IdentifierType.Field1:
                        result =
                            p => p.EntityInfos.Any(q => q.EntityDatas.Any(r => r.Active && r.Field1 == val)) == false;
                        break;
                    case IdentifierType.Field2:
                        result =
                            p => p.EntityInfos.Any(q => q.EntityDatas.Any(r => r.Active && r.Field2 == val)) == false;
                        break;
                    case IdentifierType.Field3:
                        result =
                            p => p.EntityInfos.Any(q => q.EntityDatas.Any(r => r.Active && r.Field3 == val)) == false;
                        break;
                    case IdentifierType.Field4:
                        result =
                            p => p.EntityInfos.Any(q => q.EntityDatas.Any(r => r.Active && r.Field4 == val)) == false;
                        break;
                    case IdentifierType.Field5:
                        result =
                            p =>
                            p.EntityInfos.Any(q => q.EntityDatas.Any(r => r.Active && r.Field5OrStatus == val)) == false;
                        break;
                    case IdentifierType.ANameValue:
                        result =
                            p =>
                            p.EntityInfos.Any(q => q.EntityDatas.Any(r => r.Active && r.Attribute.AttributeName == val)) ==
                            false;
                        break;
                    case IdentifierType.ItemID:
                        result = p => p.ItemID != val;
                        break;

                    case IdentifierType.AType:

                        if (val.Equals("Extended", StringComparison.CurrentCultureIgnoreCase))
                        {
                            result =
                                p =>
                                p.EntityInfos.Any(
                                    q => q.EntityDatas.Any(r => r.Active && r.Attribute.Type != AttributeTypeEnum.Sku)) == false;
                        }
                        else
                        {
                            result =
                                p =>
                                p.EntityInfos.Any(
                                    q =>
                                    q.EntityDatas.Any(
                                        r =>
                                        r.Active && r.Attribute.Type != AttributeTypeEnum.Sku &&
                                        r.Attribute.AttributeType != val)) == false;
                        }
                        break;
                }

                return result;
            }
        }

        #endregion

        #region Nested type: NewNotLikeFilterOperator

        public class NewNotLikeFilterOperator : NotLikeFilterOperator
        {
            public override LambdaExpression AssignFilter(Identifier attributeName, StringLiteral value)
            {
                string attr = attributeName.GetValue().ToString();
                string val = value.GetValue().ToString();

                Expression<Func<Sku, bool>> result = p => false;


                switch (attributeName.GetIdentifierType())
                {
                    case IdentifierType.AttributeName:
                        result =
                            p =>
                            p.EntityInfos.FirstOrDefault(q => q.EntityDatas.Any(
                                r =>
                                r.Active && r.Attribute.AttributeName == attr &&
                                SqlMethods.Like(r.Value, val) == false)) != null;
                        break;
                    case IdentifierType.Field1:
                        result =
                            p =>
                            p.EntityInfos.FirstOrDefault(q => q.EntityDatas.Any(r => r.Active && SqlMethods.Like(r.Field2, val) == false)) != null;
                        break;
                    case IdentifierType.Field2:
                        result =
                            p =>
                            p.EntityInfos.FirstOrDefault(q => q.EntityDatas.Any(r => r.Active && SqlMethods.Like(r.Field2, val) == false)) != null;
                        break;
                    case IdentifierType.Field3:
                        result =
                            p =>
                            p.EntityInfos.FirstOrDefault(q => q.EntityDatas.Any(r => r.Active && SqlMethods.Like(r.Field3, val) == false)) != null;
                        break;
                    case IdentifierType.Field4:
                        result =
                            p =>
                            p.EntityInfos.FirstOrDefault(q => q.EntityDatas.Any(r => r.Active && SqlMethods.Like(r.Field4, val) == false)) != null;
                        break;
                    case IdentifierType.Field5:
                        result =
                            p =>
                            p.EntityInfos.FirstOrDefault(q => q.EntityDatas.Any(r => r.Active && SqlMethods.Like(r.Field5OrStatus, val) == false)) != null;
                        break;
                    case IdentifierType.ANameValue:
                        result =
                            p =>
                            p.EntityInfos.FirstOrDefault(q => q.EntityDatas.Any(
                                r => r.Active && SqlMethods.Like(r.Attribute.AttributeName, val) == false)) != null;
                        break;
                    case IdentifierType.ItemID:
                        result = p => SqlMethods.Like(p.ItemID, val) == false;
                        break;
                    case IdentifierType.AType:

                        if (val.Contains("Extended"))
                        {
                            result =
                                p =>
                                p.EntityInfos.FirstOrDefault(q => q.EntityDatas.Any(r => r.Active && r.Attribute.Type != AttributeTypeEnum.Sku)) != null;
                        }
                        else
                        {
                            result =
                                p =>
                                p.EntityInfos.FirstOrDefault(q => q.EntityDatas.Any(
                                    r => r.Active && SqlMethods.Like(r.Attribute.AttributeType, val) == false)) != null;
                        }
                        break;
                }

                return result;
            }
        }

        #endregion
    }
}