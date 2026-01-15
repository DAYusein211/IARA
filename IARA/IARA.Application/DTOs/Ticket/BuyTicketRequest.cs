using System.ComponentModel.DataAnnotations;
using IARA.Domain.Enums;

namespace IARA.Application.DTOs.Ticket;

public class BuyTicketRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int UserId { get; set; }

    [Required]
    public TicketType TicketType { get; set; }
}