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
    
    public partial class TE_OtroPago
    {
        public int otropagoId { get; set; }
        public Nullable<int> nominaId { get; set; }
        public Nullable<double> c_TipoOtroPago { get; set; }
        public string Clave { get; set; }
        public string Concepto { get; set; }
        public Nullable<decimal> Importe { get; set; }
        public Nullable<decimal> SubsidioAlEmpleo_SubsidioCausado { get; set; }
        public Nullable<decimal> CompensacionSaldosAFavor_SaldoAFavor { get; set; }
        public Nullable<int> CompensacionSaldosAFavor_Año { get; set; }
        public Nullable<decimal> CompensacionSaldosAFavor_RemanenteSalFav { get; set; }
    
        public virtual c_TipoOtroPago c_TipoOtroPago1 { get; set; }
        public virtual TE_Nomina TE_Nomina { get; set; }
    }
}
