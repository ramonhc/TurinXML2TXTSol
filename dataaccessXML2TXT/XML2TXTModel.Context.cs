﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class AvantCraft_nomina2017Entities : DbContext
    {
        public AvantCraft_nomina2017Entities()
            : base("name=AvantCraft_nomina2017Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<c_Banco> c_Banco { get; set; }
        public virtual DbSet<c_Estado> c_Estado { get; set; }
        public virtual DbSet<c_OrigenRecurso> c_OrigenRecurso { get; set; }
        public virtual DbSet<c_PeriodicidadPago> c_PeriodicidadPago { get; set; }
        public virtual DbSet<c_RegimenFiscal> c_RegimenFiscal { get; set; }
        public virtual DbSet<c_RiesgoPuesto> c_RiesgoPuesto { get; set; }
        public virtual DbSet<c_TipoContrato> c_TipoContrato { get; set; }
        public virtual DbSet<c_TipoDeduccion> c_TipoDeduccion { get; set; }
        public virtual DbSet<c_TipoHoras> c_TipoHoras { get; set; }
        public virtual DbSet<c_TipoIncapacidad> c_TipoIncapacidad { get; set; }
        public virtual DbSet<c_TipoJornada> c_TipoJornada { get; set; }
        public virtual DbSet<c_TipoNomina> c_TipoNomina { get; set; }
        public virtual DbSet<c_TipoOtroPago> c_TipoOtroPago { get; set; }
        public virtual DbSet<c_TipoPercepcion> c_TipoPercepcion { get; set; }
        public virtual DbSet<c_TipoRegimen> c_TipoRegimen { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<TE_Incapacidad> TE_Incapacidad { get; set; }
        public virtual DbSet<TE_JubilacionPensionRetiro> TE_JubilacionPensionRetiro { get; set; }
        public virtual DbSet<TE_OtroPago> TE_OtroPago { get; set; }
        public virtual DbSet<TE_Percepcion_HorasExtra> TE_Percepcion_HorasExtra { get; set; }
        public virtual DbSet<TE_Receptor_Subcontratacion> TE_Receptor_Subcontratacion { get; set; }
        public virtual DbSet<TE_SeparacionIndemnizacion> TE_SeparacionIndemnizacion { get; set; }
        public virtual DbSet<TE_Nomina> TE_Nomina { get; set; }
        public virtual DbSet<TE_TXT_HEADER> TE_TXT_HEADER { get; set; }
        public virtual DbSet<TC_Subcontratacion> TC_Subcontratacion { get; set; }
        public virtual DbSet<TC_DatosFijosPorEmpleado> TC_DatosFijosPorEmpleado { get; set; }
        public virtual DbSet<TC_RfcExclusionList> TC_RfcExclusionList { get; set; }
        public virtual DbSet<TC_RfcIncludeList> TC_RfcIncludeList { get; set; }
        public virtual DbSet<TC_RFC> TC_RFC { get; set; }
        public virtual DbSet<TE_UUID2CANCEL> TE_UUID2CANCEL { get; set; }
        public virtual DbSet<TE_RfcTimbrado> TE_RfcTimbrado { get; set; }
        public virtual DbSet<TE_Deduccion> TE_Deduccion { get; set; }
        public virtual DbSet<TE_Percepcion> TE_Percepcion { get; set; }
    }
}
