using System;
using System.Collections.Generic;
using System.Text;

namespace SOE.Services.ActionResponse
{
   public class TodoWidgetAction:PendingAction
   {
       public readonly int Id;
       public TodoWidgetAction(int Id)
       {
           this.Id = Id;
       }
   }
}
