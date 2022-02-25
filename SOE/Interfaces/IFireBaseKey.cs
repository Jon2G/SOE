using Google.Cloud.Firestore;
using Kit.Sql.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOEWeb.Shared
{
    public interface IFireBaseKey
    {
        [FirestoreProperty, FirestoreDocumentId]
        public string Id { get; set; }
    }
}
