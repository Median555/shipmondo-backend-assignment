using Microsoft.EntityFrameworkCore;
using ShipmondoBackendAssignment.DB;
using ShipmondoBackendAssignment.ShipmondoApi;
using Shipment = ShipmondoBackendAssignment.DB.Models.Shipment;

namespace ShipmondoBackendAssignment.Services;

public class ShipmentService(ShipmondoDbContext db, IShipmondoApiClient apiClient)
{
	/// <summary>
	/// Create a new shipment at Shipmondo, and save it locally.
	/// </summary>
	public async Task<Shipment> CreateShipmentAsync()
	{
		ShipmondoApi.Shipment shipment = await apiClient.CreateShipmentAsync();
		
		await db.Shipments.AddAsync(new Shipment
		{
			id = shipment.id,
			packageNumber = shipment.pkgNo
		});
		await db.SaveChangesAsync();
		
		return await db.Shipments.SingleAsync(it => it.id == shipment.id);
	}
}