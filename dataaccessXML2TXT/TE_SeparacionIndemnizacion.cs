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
    
    public partial class TE_SeparacionIndemnizacion
    {
        public int SeparacionIndemnizacionId { get; set; }
        public Nullable<int> nominaId { get; set; }
        public Nullable<decimal> TotalPagado { get; set; }
        public Nullable<int> NumAñosServicio { get; set; }
        public Nullable<decimal> UltimoSueldoMensOrd { get; set; }
        public Nullable<decimal> IngresoAcumulable { get; set; }
        public Nullable<decimal> IngresoNoAcumulable { get; set; }
    
        public virtual TE_Nomina TE_Nomina { get; set; }
    }
}
