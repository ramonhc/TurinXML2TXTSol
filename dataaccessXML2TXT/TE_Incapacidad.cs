//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace dataaccessXML2TXT
{
    using System;
    using System.Collections.Generic;
    
    public partial class TE_Incapacidad
    {
        public int incapacidadId { get; set; }
        public Nullable<int> nominaId { get; set; }
        public Nullable<int> DiasIncapacidad { get; set; }
        public Nullable<double> c_TipoIncapacidad { get; set; }
        public Nullable<decimal> ImporteMonetario { get; set; }
        public string TipoIncapacidad { get; set; }
    
        public virtual c_TipoIncapacidad c_TipoIncapacidad1 { get; set; }
        public virtual TE_Nomina TE_Nomina { get; set; }
    }
}
