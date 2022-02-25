using Kit.Sql.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

// ReSharper disable once CheckNamespace
namespace FirestoreLINQ
{
    public class FireStoreCollectionAttribute : Attribute
    {
        public string CollectionPath { get; set; }

        public FireStoreCollectionAttribute(string collectionPath)
        {
            CollectionPath = collectionPath;
        }
        internal static FireStoreCollectionAttribute GetAttributes(TypeInfo typeInfo)
        {
            return typeInfo.CustomAttributes
                .Where(x => x.AttributeType == typeof(FireStoreCollectionAttribute))
                .Select(x => (FireStoreCollectionAttribute)x.InflateAttribute())
                .FirstOrDefault();
        }
    }
}
