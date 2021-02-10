using System;
using System.Collections.Generic;
using System.Text;

namespace OrganizadorEscolar.Widgets.Horario
{
    public class IntentData
    {
        public int RowId { get; private set; }
        public string NombreMateria { get; private set; }
        public IntentData(int RowId,string NombreMateria)
        {
            this.NombreMateria = NombreMateria;
            this.RowId = RowId;
        }
    }
}
