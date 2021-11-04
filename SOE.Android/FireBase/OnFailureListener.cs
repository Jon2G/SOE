using Android.Gms.Tasks;
using Java.Interop;
using System;
using Exception = Java.Lang.Exception;

namespace SOE.Droid.FireBase
{
    public class OnFailureListener : Java.Lang.Object, IOnFailureListener
    {
        public OnFailureListener()
        {
            
        }
        public JniManagedPeerStates JniManagedPeerState => throw new NotImplementedException();

        public void Disposed()
        {
            throw new NotImplementedException();
        }

        public void DisposeUnlessReferenced()
        {
            throw new NotImplementedException();
        }

        public void Finalized()
        {
        }

        public void OnFailure(Exception e)
        {
            if (e is not null)
            {

            }
        }

        public void SetJniManagedPeerState(JniManagedPeerStates value)
        {
            throw new NotImplementedException();
        }



    }
}