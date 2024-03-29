﻿using Android.Gms.Tasks;
using Android.Runtime;
using Firebase.DynamicLinks;
using Java.Interop;
using SOE.FireBase;
using SOE.Services.ActionResponse;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Uri = Android.Net.Uri;

namespace SOE.Droid.FireBase
{
    [Preserve]
    public class OnSuccessListener : Java.Lang.Object, IOnSuccessListener
    {
        private readonly Command<IActionResponse> ActionResponse;
        public OnSuccessListener(Command<IActionResponse> ActionResponse)
        {
            this.ActionResponse = ActionResponse;
        }


        public void OnSuccess(Java.Lang.Object result)
        {
            if (result is PendingDynamicLinkData data)
            {
                Uri deepLink = data.Link;
                FromDeepLink(deepLink);
            }
        }

        private string GetParameter(string query, string name)
        {
            //SAMPLE: verb=share?type=todo?id=fcedcddc-2712-4c9a-8c07-68a8a12550b6?
            Regex regex = new Regex($@"(?<parameter>{name}+\=(?<value>.+?)(?=\?))");
            Match match = regex.Match(query);
            if (match.Success)
            {
                return match.Groups["value"].Value;
            }
            return string.Empty;
        }
        public void FromDeepLink(Uri deepLink)
        {
            string link = deepLink.GetQueryParameter("link");
            if (!string.IsNullOrEmpty(link))
            {
                FromDeepLink(Uri.Parse(link));
                return;
            }

            string parameters = deepLink.GetQueryParameter("params");
            if (string.IsNullOrEmpty(parameters)) { return; }
            if (!parameters.EndsWith('?'))
            {
                parameters += '?';
            }

            string verb = GetParameter(parameters, "verb");
            IActionResponse action = null;
            switch (verb)
            {
                case "share":
                    action = OnShare(parameters);
                    break;
                case "asistant":
                    action = OnAssitant(parameters);
                    break;
            }
            if (action is not null)
                ActionResponse?.Execute(action);
        }

        public IActionResponse OnShare(string parameters)
        {
            string type = GetParameter(parameters, "type");
            string id = GetParameter(parameters, "id");
            if (!Guid.TryParse(id, out Guid shareGuid))
            {
                FirebaseAnalyticsService analytics = new FirebaseAnalyticsService();
                analytics.LogEvent("error", nameof(id), $"Guid '{shareGuid}' is not a valid guid");
                return null;
            }

            IActionResponse action = null;
            switch (type)
            {
                case "todo":
                    action = new ShareTodoAction(shareGuid);
                    break;
                case "reminder":
                    action = new ShareReminderAction(shareGuid);
                    break;
            }
            return action;
        }
        /// <summary>
        /// Este metodo lo cree ayer antes de dormir pero no se si esta bien 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IActionResponse OnAssitant(string parameters)
        {
            IActionResponse action = null;
            switch (parameters)
            {
                case "Que clases tengo":
                    action = (IActionResponse)Shell.Current.DisplayAlert("Prueba", "Que clases tengo", "Ok");
                    break;
                case "Crear Tarea":
                    action = (IActionResponse)Shell.Current.DisplayAlert("Prueba", "Crear Tarea", "Ok");
                    break;
                case "Crear recordatorio":
                    action = (IActionResponse)Shell.Current.DisplayAlert("Prueba", "Crear recordatorio", "Ok");
                    break;
                    
            }
            return action;
        }



    }
}
