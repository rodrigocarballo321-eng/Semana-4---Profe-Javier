using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGNAL_DESK.Models
{
    public enum TicketPriority
    {
        P1, //critical  (1 Hour SLA)
        P2, //high  (4 Hours SLA)
        P3, //medium  (8 Hours SLA)
        P4  //low  (24 Hours SLA)
    }

    public enum TicketStatus
    {
        New,
        InProgress,
        Resolved
    }
    public class Ticket
    {
        public string Id { get; set; } = string.Empty;
        public string Client { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public TicketPriority Priority { get; set; }
        public TicketStatus Status { get; set; } = TicketStatus.New;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ResponseDueAt { get; set; }
        public string ResolutionNotes { get; set; } = string.Empty;
        public DateTime? ResolvedAt { get; set; }
        public bool IsNearSlaBreach
        {
            get
            {
                if (Status == TicketStatus.Resolved) return false;
                var timeRemaining = ResponseDueAt - DateTime.Now;
                return timeRemaining.TotalMinutes <= 30 && timeRemaining.TotalSeconds > 0;
            }
        }

        public bool IsSlaBreached => Status != TicketStatus.Resolved && DateTime.Now > ResponseDueAt;
    }
}