//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NIPS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Ledger
    {
        public int LedgerID { get; set; }
        public string GiverID { get; set; }
        public string GetterID { get; set; }
        public int Amount { get; set; }
        public string Reason { get; set; }
    }
}
