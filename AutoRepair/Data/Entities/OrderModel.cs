using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoRepair.Data.Entities
{
    public class OrderModel
    {
        [Key]
        public Guid ID { get; set; }

        [Required(ErrorMessage = "The client of order is not specified")]
        public Guid ClientID { get; set; }
        [ForeignKey("ClientID")]
        public ClientModel? Client { get; set; }

        [Required(ErrorMessage = "The car of order is not specified")]
        public Guid CarID { get; set; }
        [ForeignKey("CarID")]
        public CarModel? Car { get; set; }

        public Guid? MasterID { get; set; }
        [ForeignKey("MasterID")]
        public MasterModel? Master { get; set; }

        [Required(ErrorMessage = "The service of order is not specified")]
        public Guid ServiceID { get; set; }
        [ForeignKey("ServiceID")]
        public ServiceModel? Service { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "The status of order is not specified")]
        public Guid StatusID { get; set; }
        [ForeignKey("StatusID")]
        public StatusModel? Status { get; set; }

        public bool IsPaidOut { get; set; } = false;
    }
}