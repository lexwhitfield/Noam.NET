//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Noam.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.Mentions = new HashSet<Mention>();
            this.Tweets = new HashSet<Tweet>();
        }
    
        public long UserId { get; set; }
        public string ScreenName { get; set; }
        public string FullName { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string Description { get; set; }
        public bool IsGeoEnabled { get; set; }
        public bool IsProtected { get; set; }
        public bool IsTranslator { get; set; }
        public bool IsVerified { get; set; }
        public string Language { get; set; }
        public string Location { get; set; }
        public string Timezone { get; set; }
        public string Url { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Mention> Mentions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tweet> Tweets { get; set; }
    }
}
