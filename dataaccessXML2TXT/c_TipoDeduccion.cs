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
    
    public partial class c_TipoDeduccion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public c_TipoDeduccion()
        {
            this.TE_Deduccion = new HashSet<TE_Deduccion>();
        }
    
        public double c_TipoDeduccion1 { get; set; }
        public string Descripcion { get; set; }
        public string Value { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TE_Deduccion> TE_Deduccion { get; set; }
    }
}
