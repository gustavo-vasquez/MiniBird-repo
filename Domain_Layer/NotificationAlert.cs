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
    
    public partial class NotificationAlert
    {
        public int NotificationAlertID { get; set; }
        public Nullable<bool> IsRead { get; set; }
        public Nullable<int> ID_Person { get; set; }
        public Nullable<int> ID_Post { get; set; }
        public Nullable<int> ID_RePost { get; set; }
        public Nullable<int> ID_LikePost { get; set; }
    
        public virtual LikePost LikePost { get; set; }
        public virtual Person Person { get; set; }
        public virtual Post Post { get; set; }
        public virtual RePost RePost { get; set; }
    }
}
