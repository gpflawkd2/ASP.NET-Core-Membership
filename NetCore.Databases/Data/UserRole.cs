//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NetCore.Databases.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserRole
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserRole()
        {
            this.UserRolesByUsers = new HashSet<UserRolesByUser>();
        }
    
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public byte RolePriority { get; set; }
        public System.DateTime ModifiedUtcData { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserRolesByUser> UserRolesByUsers { get; set; }
    }
}