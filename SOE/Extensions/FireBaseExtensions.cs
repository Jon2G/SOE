using Firebase.Database;
using Firebase.Database.Query;
using FirestoreLINQ;
using Google.Cloud.Firestore;
using SOE.Models;
using SOEWeb.Shared;
using System;
using System.Globalization;
using System.Reflection;
using DateTime = System.DateTime;
using GDateTime = Google.Type.DateTime;
using GTimeOfDay = Google.Type.TimeOfDay;

// ReSharper disable once CheckNamespace
namespace SOE
{
    public static class FireStoreExtensions
    {


        public static GDateTime ToDateTime(this DateTime dateTime)
        {
            return new GDateTime()
            {
                Year = dateTime.Year,
                Month = dateTime.Month,
                Day = dateTime.Day,
                Hours = dateTime.Hour,
                Minutes = dateTime.Minute,
                Seconds = dateTime.Second
            };
        }

        public static TimeSpan ToTimeSpan(this GTimeOfDay time)
        {
            return new TimeSpan(0, time.Hours, time.Minutes, time.Seconds, time.Nanos);
        }

        public static GTimeOfDay ToTimeOfDay(this TimeSpan time)
        {
            return new GTimeOfDay()
            {
                Hours = time.Hours,
                Minutes = time.Minutes,
                Seconds = time.Seconds,
                Nanos = time.Minutes
            };
        }
        private static string GetTableName<T>(this IFireBaseKey obj) where T : class
        {
            TypeInfo typeInfo = typeof(T).GetTypeInfo();
            FireStoreCollectionAttribute collectionAttribute = FireStoreCollectionAttribute.GetAttributes(typeInfo);
            return collectionAttribute?.CollectionPath ?? typeInfo.Name.ToLower();
        }

        public static ChildQuery GetQuery<T>(this IFireBaseKey obj, FirebaseClient client) where T : class
        {
            string tableName = obj.GetTableName<T>();
            return client.Child(tableName);
        }

        public static DocumentReference GetDocument<T>(this FirestoreDb db)
        {
            return db.Collection<School>().Document();
        }
        public static CollectionReference Collection<T>(this DocumentReference doc)
        {
            TypeInfo typeInfo = typeof(T).GetTypeInfo();
            FireStoreCollectionAttribute collectionAttribute = FireStoreCollectionAttribute.GetAttributes(typeInfo);
            string collectionName = collectionAttribute?.CollectionPath ?? typeInfo.Name.ToLower();
            return doc.Collection(collectionName);
        }
        //public static CollectionReference Collection<T>(this FirestoreDb db)
        //{
        //    TypeInfo typeInfo = typeof(T).GetTypeInfo();
        //    FireStoreCollectionAttribute collectionAttribute = FireStoreCollectionAttribute.GetAttributes(typeInfo);
        //    string collectionName = collectionAttribute?.CollectionPath ?? typeInfo.Name.ToLower();
        //    return db.Collection(collectionName);
        //}

        //public static IQueryable<T> AsQuerable<T>(this FirestoreDb db)
        //{
        //    return db.Collection<T>().AsQuerable<T>();
        //}

        private const string FirestoreTimeFormat = "hhmmss";

        public static TimeSpan ToFirestoreTime(int time)
        {
            string firetime = time.ToString();
            return TimeSpan.ParseExact(firetime, FirestoreTimeFormat, CultureInfo.InvariantCulture);
        }
        public static int ToFirestoreTime(this TimeSpan time)
        {
            string begin = time.ToString(FirestoreTimeFormat, CultureInfo.InvariantCulture);
            return int.Parse(begin);
        }
    }
}
