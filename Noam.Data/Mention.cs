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
    
    public partial class Mention
    {
        public long MentionId { get; set; }
        public long TweetId { get; set; }
        public long UserId { get; set; }
    
        public virtual User User { get; set; }
        public virtual Tweet Tweet { get; set; }
    }
}
