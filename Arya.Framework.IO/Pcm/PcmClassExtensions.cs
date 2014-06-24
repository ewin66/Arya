using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Arya.Framework.IO.Pcm
{
    public partial class AttVal
    {
        #region Fields

        private Regex _rxAlphaOnly = new Regex("^[a-zA-Z]+$");

        #endregion Fields

        #region Methods

        public static AttVal FromValues(Guid nodeId, string attributeName, string attributeGroup, string value)
        {
            var attVal = new AttVal
            {
                NodeId = nodeId,
                AttributeName = attributeName,
                AttributeGroup = attributeGroup,
                Label = value,
                SortableValue = value
            };

            attVal.UpdateSearchTerms();
            return attVal;
        }

        private void UpdateSearchTerms()
        {
            var searchWords = new List<KeyValuePair<string, int>>();
            var searchPhrases = new List<string>();
            var parts = Label.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                searchWords.Add(new KeyValuePair<string, int>(part, i + 1));

                var currentPhrase = part;
                for (var j = i + 1; j < parts.Length; j++)
                {
                    currentPhrase += " " + parts[j];
                    searchPhrases.Add(currentPhrase);
                }
            }
            if (parts.Length > 1)
            {
                var acronym = parts.Aggregate(string.Empty, (curr, next) => curr + next.First());
                if (_rxAlphaOnly.IsMatch(acronym))
                    searchWords.Add(new KeyValuePair<string, int>(acronym, 0));
            }

            SearchWord = searchWords.Select(sw => new AttValSearchWord {Value = sw.Key, Rank = sw.Value}).ToArray();
            SearchPhrase = searchPhrases.ToArray();
        }

        #endregion Methods
    }

    public partial class Item
    {
        #region Methods

        public static Item FromValues(ItemNode node, Guid id, string itemId, string psp)
        {
            return new Item
            {
                Id = id,
                ItemId = itemId,
                Node = new[] {node},
                PrimaryDescription = psp
            };
        }

        #endregion Methods
    }

    public partial class ItemNode
    {
        #region Methods

        public static ItemNode FromValues(Guid id, Guid catalogId, string breadCrumb, string nodeName)
        {
            return new ItemNode
            {
                Id = id,
                CatalogId = catalogId,
                BreadCrumb = breadCrumb,
                NodeName = nodeName
            };
        }

        #endregion Methods
    }

    public partial class Node
    {
        #region Fields

        private Regex _rxAlphaOnly = new Regex("^[a-zA-Z]+$");

        #endregion Fields

        #region Methods

        public static Node FromValues(Guid id, Guid catalogId, Guid parentId, string breadCrumb, string[] taxonomyPaths,
            string nodeName)
        {
            return new Node
            {
                Id = id,
                CatalogId = catalogId,
                ParentId = parentId,
                BreadCrumb = breadCrumb,
                TaxonomyPath = taxonomyPaths,
                NodeName = nodeName
            };
        }

        private void UpdateSearchTerms()
        {
            var searchWords = new List<KeyValuePair<string, int>>();
            var searchPhrases = new List<string>();
            var parts = NodeName.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                searchWords.Add(new KeyValuePair<string, int>(part, i + 1));

                var currentPhrase = part;
                for (var j = i + 1; j < parts.Length; j++)
                {
                    currentPhrase += " " + parts[j];
                    searchPhrases.Add(currentPhrase);
                }
            }
            if (parts.Length > 1)
            {
                var acronym = parts.Aggregate(string.Empty, (curr, next) => curr + next.First());
                if (_rxAlphaOnly.IsMatch(acronym))
                    searchWords.Add(new KeyValuePair<string, int>(acronym, 0));
            }

            SearchWord = searchWords.Select(sw => new NodeSearchWord {Value = sw.Key, Rank = sw.Value}).ToArray();
            SearchPhrase = searchPhrases.ToArray();
        }

        #endregion Methods
    }

    public partial class NodeSchemaAttribute
    {
        #region Methods

        public static NodeSchemaAttribute FromValues(string attributeName, string attributeGroup, string dataType,
            decimal facetOrder, decimal displayOrder)
        {
            return new NodeSchemaAttribute
            {
                AttributeName = attributeName,
                AttributeGroup = attributeGroup,
                DataType = dataType,
                FacetOrder = facetOrder,
                DisplayOrder = displayOrder
            };
        }

        #endregion Methods
    }
}