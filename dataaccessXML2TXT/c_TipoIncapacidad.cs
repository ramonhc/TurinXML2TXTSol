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
    
    public partial class c_TipoIncapacidad
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public c_TipoIncapacidad()
        {
            this.TE_Incapacidad = new HashSet<TE_Incapacidad>();
        }
    
        public double c_TipoIncapacidad1 { get; set; }
        public string Descripción { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TE_Incapacidad> TE_Incapacidad { get; set; }
    }
}
