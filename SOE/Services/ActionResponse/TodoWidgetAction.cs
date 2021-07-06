using System;
using System.Collections.Generic;
using System.Text;

namespace SOE.Services.ActionResponse
{
   public class TodoWidgetAction:PendingAction
   {
       public readonly Guid Id;
       public TodoWidgetAction(Guid Id)
       {
           this.Id = Id;
       }
   }
}
