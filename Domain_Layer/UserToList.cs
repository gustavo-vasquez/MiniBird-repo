//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Domain_Layer
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserToList
    {
        public int ID_Person { get; set; }
        public int ID_List { get; set; }
        public System.DateTime DateOfAggregate { get; set; }
    
        public virtual List List { get; set; }
        public virtual Person Person { get; set; }
    }
}
