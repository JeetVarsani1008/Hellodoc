using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Table("Chat")]
public partial class Chat
{
    [Key]
    public int Id { get; set; }

    public int? AdminId { get; set; }

    public int? ProviderId { get; set; }

    public int? RequestId { get; set; }

    [Column(TypeName = "character varying")]
    public string? Message { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    public int? SentBy { get; set; }

    [ForeignKey("AdminId")]
    [InverseProperty("Chats")]
    public virtual Admin? Admin { get; set; }

    [ForeignKey("ProviderId")]
    [InverseProperty("Chats")]
    public virtual Physician? Provider { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("Chats")]
    public virtual Request? Request { get; set; }
}
