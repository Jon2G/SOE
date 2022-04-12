using FirestoreLINQ;
using Plugin.CloudFirestore;
using SOE.Models;
using System.Reflection;


// ReSharper disable once CheckNamespace
namespace SOE
{
    public static class FireStoreExtensions
    {
        public static IDocumentReference GetDocument<T>(this IFirestore db)
        {
            return db.Collection<School>().Document();
        }
        public static ICollectionReference Collection<T>(this IDocumentReference doc)
        {
            TypeInfo typeInfo = typeof(T).GetTypeInfo();
            FireStoreCollectionAttribute collectionAttribute = FireStoreCollectionAttribute.GetAttributes(typeInfo);
            string collectionName = collectionAttribute?.CollectionPath ?? typeInfo.Name.ToLower();
            return doc.Collection(collectionName);
        }
        public static ICollectionReference Collection<T>(this IFirestore db)
        {
            TypeInfo typeInfo = typeof(T).GetTypeInfo();
            FireStoreCollectionAttribute collectionAttribute = FireStoreCollectionAttribute.GetAttributes(typeInfo);
            string collectionName = collectionAttribute?.CollectionPath ?? typeInfo.Name.ToLower();
            return db.Collection(collectionName);
        }
    }
}
