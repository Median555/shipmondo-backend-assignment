using System.ComponentModel.DataAnnotations.Schema;

namespace ShipmondoBackendAssignment.DB.Models;

public class Shipment
{
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int id { get; set; }
	public string packageNumber { get; set; }
}