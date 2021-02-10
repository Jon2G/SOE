using System;
using System.Collections.Generic;
using System.Text;

namespace OrganizadorEscolar.Widgets.Tareas
{
    public class IntentData
    {
        public int RowId { get; private set; }
        public string NombreTarea { get; private set; }
        public IntentData(int RowId, string NombreTarea)
        {
            this.NombreTarea = NombreTarea;
            this.RowId = RowId;
        }
    }
}
