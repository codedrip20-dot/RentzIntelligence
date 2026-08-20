using Microsoft.EntityFrameworkCore;
using Rentz.Intelligence.Domain.Entities;

namespace Rentz.Intelligence.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(RentzDbContext context)
    {
        // Prevent duplicate seed data
        if (await context.Properties.AnyAsync())
        {
            return;
        }

        // =========================
        // AMENITIES
        // =========================

        var wifi = Amenity.Create(Guid.NewGuid(), "Wi-Fi");
        var parking = Amenity.Create(Guid.NewGuid(), "Parking");
        var powerBackup = Amenity.Create(Guid.NewGuid(), "Power Backup");
        var security = Amenity.Create(Guid.NewGuid(), "24/7 Security");
        var gym = Amenity.Create(Guid.NewGuid(), "Gym");
        var swimmingPool = Amenity.Create(Guid.NewGuid(), "Swimming Pool");
        var balcony = Amenity.Create(Guid.NewGuid(), "Balcony");
        var furnishedKitchen = Amenity.Create(Guid.NewGuid(), "Fully Equipped Kitchen");
        var laundry = Amenity.Create(Guid.NewGuid(), "Laundry");
        var cctv = Amenity.Create(Guid.NewGuid(), "CCTV");
        var elevator = Amenity.Create(Guid.NewGuid(), "Elevator");
        var petFriendly = Amenity.Create(Guid.NewGuid(), "Pet Friendly");
        var garden = Amenity.Create(Guid.NewGuid(), "Garden");
        var rooftop = Amenity.Create(Guid.NewGuid(), "Rooftop");
        var housekeeping = Amenity.Create(Guid.NewGuid(), "Housekeeping");

        var amenities = new[]
        {
            wifi,
            parking,
            powerBackup,
            security,
            gym,
            swimmingPool,
            balcony,
            furnishedKitchen,
            laundry,
            cctv,
            elevator,
            petFriendly,
            garden,
            rooftop,
            housekeeping
        };

        context.Amenities.AddRange(amenities);

        // =========================
        // PROPERTIES
        // =========================

        var himalayanHeights = Property.Create(
            Guid.NewGuid(),
            "Himalayan Heights",
            "A modern fully furnished apartment with panoramic mountain views, reliable power backup and secure parking.",
            "Apartment",
            "Gangtok",
            "Sikkim",
            "India",
            24000m,
            48000m,
            2,
            2,
            "Fully Furnished"
        );

        var greenValley = Property.Create(
            Guid.NewGuid(),
            "Green Valley Residency",
            "Spacious family apartment located in a quiet residential neighborhood with excellent natural light and balcony space.",
            "Apartment",
            "Gangtok",
            "Sikkim",
            "India",
            22000m,
            44000m,
            2,
            2,
            "Semi Furnished"
        );

        var urbanNest = Property.Create(
            Guid.NewGuid(),
            "Urban Nest",
            "Compact modern apartment designed for professionals looking for convenient city living close to commercial areas.",
            "Apartment",
            "Siliguri",
            "West Bengal",
            "India",
            18000m,
            36000m,
            1,
            1,
            "Fully Furnished"
        );

        var pineView = Property.Create(
            Guid.NewGuid(),
            "Pine View Residency",
            "Peaceful residential property surrounded by pine trees with spacious rooms, private parking and excellent views.",
            "House",
            "Namchi",
            "Sikkim",
            "India",
            20000m,
            40000m,
            3,
            2,
            "Semi Furnished"
        );

        var lakeview = Property.Create(
            Guid.NewGuid(),
            "Lakeview Residency",
            "Premium apartment with large windows and balcony views, suitable for families looking for a quiet and comfortable home.",
            "Apartment",
            "Nainital",
            "Uttarakhand",
            "India",
            28000m,
            56000m,
            3,
            2,
            "Fully Furnished"
        );

        var metroHeights = Property.Create(
            Guid.NewGuid(),
            "Metro Heights",
            "Modern city apartment with elevator access, security and convenient connectivity to shopping and transportation.",
            "Apartment",
            "Siliguri",
            "West Bengal",
            "India",
            16000m,
            32000m,
            2,
            1,
            "Unfurnished"
        );

        var sunriseResidency = Property.Create(
            Guid.NewGuid(),
            "Sunrise Residency",
            "Bright and affordable apartment with balcony space and essential amenities for small families or working professionals.",
            "Apartment",
            "Gangtok",
            "Sikkim",
            "India",
            19500m,
            39000m,
            2,
            1,
            "Semi Furnished"
        );

        var royalEnclave = Property.Create(
            Guid.NewGuid(),
            "Royal Enclave",
            "Luxury residential property featuring premium amenities, spacious interiors, security and recreational facilities.",
            "Apartment",
            "Kolkata",
            "West Bengal",
            "India",
            45000m,
            90000m,
            3,
            3,
            "Fully Furnished"
        );

        var cedarHomes = Property.Create(
            Guid.NewGuid(),
            "Cedar Homes",
            "Comfortable family home with a private garden, parking and generous living space in a peaceful neighborhood.",
            "House",
            "Kalimpong",
            "West Bengal",
            "India",
            25000m,
            50000m,
            3,
            2,
            "Semi Furnished"
        );

        var valleyVista = Property.Create(
            Guid.NewGuid(),
            "Valley Vista",
            "Affordable mountain apartment with beautiful valley views, essential amenities and a quiet residential atmosphere.",
            "Apartment",
            "Pelling",
            "Sikkim",
            "India",
            15000m,
            30000m,
            1,
            1,
            "Unfurnished"
        );

        var properties = new[]
        {
            himalayanHeights,
            greenValley,
            urbanNest,
            pineView,
            lakeview,
            metroHeights,
            sunriseResidency,
            royalEnclave,
            cedarHomes,
            valleyVista
        };

        context.Properties.AddRange(properties);

        // =========================
        // PROPERTY ↔ AMENITIES
        // =========================

        var propertyAmenities = new[]
        {
            // Himalayan Heights
            PropertyAmenity.Create(himalayanHeights.Id, wifi.Id),
            PropertyAmenity.Create(himalayanHeights.Id, parking.Id),
            PropertyAmenity.Create(himalayanHeights.Id, powerBackup.Id),
            PropertyAmenity.Create(himalayanHeights.Id, security.Id),
            PropertyAmenity.Create(himalayanHeights.Id, balcony.Id),
            PropertyAmenity.Create(himalayanHeights.Id, elevator.Id),

            // Green Valley Residency
            PropertyAmenity.Create(greenValley.Id, wifi.Id),
            PropertyAmenity.Create(greenValley.Id, parking.Id),
            PropertyAmenity.Create(greenValley.Id, balcony.Id),
            PropertyAmenity.Create(greenValley.Id, garden.Id),
            PropertyAmenity.Create(greenValley.Id, security.Id),

            // Urban Nest
            PropertyAmenity.Create(urbanNest.Id, wifi.Id),
            PropertyAmenity.Create(urbanNest.Id, security.Id),
            PropertyAmenity.Create(urbanNest.Id, elevator.Id),
            PropertyAmenity.Create(urbanNest.Id, laundry.Id),

            // Pine View Residency
            PropertyAmenity.Create(pineView.Id, parking.Id),
            PropertyAmenity.Create(pineView.Id, wifi.Id),
            PropertyAmenity.Create(pineView.Id, balcony.Id),
            PropertyAmenity.Create(pineView.Id, garden.Id),
            PropertyAmenity.Create(pineView.Id, petFriendly.Id),

            // Lakeview Residency
            PropertyAmenity.Create(lakeview.Id, wifi.Id),
            PropertyAmenity.Create(lakeview.Id, parking.Id),
            PropertyAmenity.Create(lakeview.Id, balcony.Id),
            PropertyAmenity.Create(lakeview.Id, swimmingPool.Id),
            PropertyAmenity.Create(lakeview.Id, gym.Id),
            PropertyAmenity.Create(lakeview.Id, security.Id),

            // Metro Heights
            PropertyAmenity.Create(metroHeights.Id, wifi.Id),
            PropertyAmenity.Create(metroHeights.Id, security.Id),
            PropertyAmenity.Create(metroHeights.Id, cctv.Id),
            PropertyAmenity.Create(metroHeights.Id, elevator.Id),
            PropertyAmenity.Create(metroHeights.Id, parking.Id),

            // Sunrise Residency
            PropertyAmenity.Create(sunriseResidency.Id, wifi.Id),
            PropertyAmenity.Create(sunriseResidency.Id, balcony.Id),
            PropertyAmenity.Create(sunriseResidency.Id, parking.Id),
            PropertyAmenity.Create(sunriseResidency.Id, cctv.Id),

            // Royal Enclave
            PropertyAmenity.Create(royalEnclave.Id, wifi.Id),
            PropertyAmenity.Create(royalEnclave.Id, parking.Id),
            PropertyAmenity.Create(royalEnclave.Id, security.Id),
            PropertyAmenity.Create(royalEnclave.Id, gym.Id),
            PropertyAmenity.Create(royalEnclave.Id, swimmingPool.Id),
            PropertyAmenity.Create(royalEnclave.Id, elevator.Id),
            PropertyAmenity.Create(royalEnclave.Id, housekeeping.Id),
            PropertyAmenity.Create(royalEnclave.Id, rooftop.Id),

            // Cedar Homes
            PropertyAmenity.Create(cedarHomes.Id, parking.Id),
            PropertyAmenity.Create(cedarHomes.Id, garden.Id),
            PropertyAmenity.Create(cedarHomes.Id, wifi.Id),
            PropertyAmenity.Create(cedarHomes.Id, petFriendly.Id),

            // Valley Vista
            PropertyAmenity.Create(valleyVista.Id, wifi.Id),
            PropertyAmenity.Create(valleyVista.Id, balcony.Id),
            PropertyAmenity.Create(valleyVista.Id, parking.Id)
        };

        context.PropertyAmenities.AddRange(propertyAmenities);

        // =========================
        // PROPERTY DOCUMENTS
        // =========================

        var documents = new[]
        {
            PropertyDocument.Create(
                Guid.NewGuid(),
                himalayanHeights.Id,
                "Himalayan Heights House Rules",
                "House Rules",
                "Residents must maintain quiet hours between 10 PM and 7 AM. Smoking is not permitted inside the apartment. Guests are allowed but long-term occupancy by guests requires owner approval. Pets are not permitted. Residents must keep shared areas clean."
            ),

            PropertyDocument.Create(
                Guid.NewGuid(),
                himalayanHeights.Id,
                "Himalayan Heights Maintenance Policy",
                "Maintenance Policy",
                "The owner is responsible for structural repairs, plumbing failures and electrical faults caused by normal wear and tear. Tenants are responsible for minor damages caused by negligence and replacement of consumable items."
            ),

            PropertyDocument.Create(
                Guid.NewGuid(),
                greenValley.Id,
                "Green Valley Rental Terms",
                "Rental Terms",
                "The monthly rent is 22000 INR with a security deposit of 44000 INR. Rent must be paid before the 5th day of each month. A minimum notice period of 30 days is required before vacating the property."
            ),

            PropertyDocument.Create(
                Guid.NewGuid(),
                greenValley.Id,
                "Green Valley House Rules",
                "House Rules",
                "Pets are not allowed. Loud music is prohibited after 9 PM. Parking is available for one vehicle per apartment. Visitors must follow building security procedures."
            ),

            PropertyDocument.Create(
                Guid.NewGuid(),
                urbanNest.Id,
                "Urban Nest Tenant Policy",
                "Tenant Policy",
                "Urban Nest is intended for residential use. Tenants may use the provided kitchen and laundry facilities. The property does not permit commercial activity from the apartment."
            ),

            PropertyDocument.Create(
                Guid.NewGuid(),
                pineView.Id,
                "Pine View Pet Policy",
                "Pet Policy",
                "Pine View Residency allows domestic pets. Tenants must ensure pets do not disturb neighboring residents and are responsible for cleaning any common areas affected by their pets."
            ),

            PropertyDocument.Create(
                Guid.NewGuid(),
                pineView.Id,
                "Pine View Maintenance Policy",
                "Maintenance Policy",
                "Routine plumbing and electrical maintenance caused by normal property usage is handled by the owner. Damage caused by tenant negligence will be charged to the tenant."
            ),

            PropertyDocument.Create(
                Guid.NewGuid(),
                lakeview.Id,
                "Lakeview Amenities Policy",
                "Amenities Policy",
                "Residents have access to the swimming pool and gym during designated hours. Pool access requires residents to follow building safety rules. Guests must be accompanied by a resident."
            ),

            PropertyDocument.Create(
                Guid.NewGuid(),
                lakeview.Id,
                "Lakeview House Rules",
                "House Rules",
                "Quiet hours are from 10 PM to 6 AM. No smoking is allowed in indoor common areas. Residents must follow security procedures when entering and exiting the building."
            ),

            PropertyDocument.Create(
                Guid.NewGuid(),
                royalEnclave.Id,
                "Royal Enclave Premium Services",
                "Services",
                "Royal Enclave provides housekeeping services, rooftop access, gym facilities, swimming pool access and 24/7 security. Certain premium services may require additional charges."
            ),

            PropertyDocument.Create(
                Guid.NewGuid(),
                cedarHomes.Id,
                "Cedar Homes Pet Policy",
                "Pet Policy",
                "Cedar Homes is pet friendly. Residents are responsible for pet safety, hygiene and any damage caused by pets to the property."
            ),

            PropertyDocument.Create(
                Guid.NewGuid(),
                valleyVista.Id,
                "Valley Vista Rental Terms",
                "Rental Terms",
                "Monthly rent is 15000 INR with a security deposit of 30000 INR. Tenants must provide one month's notice before leaving. Residential use only."
            )
        };

        context.PropertyDocuments.AddRange(documents);

        // =========================
        // PROPERTY IMAGES
        // =========================

        var images = new[]
        {
            PropertyImage.Create(
                Guid.NewGuid(),
                himalayanHeights.Id,
                "https://images.unsplash.com/photo-1600607687920-4e2a09cf159d",
                "Himalayan Heights modern living room"
            ),

            PropertyImage.Create(
                Guid.NewGuid(),
                himalayanHeights.Id,
                "https://images.unsplash.com/photo-1600566753190-17f0baa2a6c3",
                "Himalayan Heights apartment interior"
            ),

            PropertyImage.Create(
                Guid.NewGuid(),
                greenValley.Id,
                "https://images.unsplash.com/photo-1600585154340-be6161a56a0c",
                "Green Valley Residency exterior"
            ),

            PropertyImage.Create(
                Guid.NewGuid(),
                urbanNest.Id,
                "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688",
                "Urban Nest apartment"
            ),

            PropertyImage.Create(
                Guid.NewGuid(),
                pineView.Id,
                "https://images.unsplash.com/photo-1600607688969-a5bfcd646154",
                "Pine View Residency interior"
            ),

            PropertyImage.Create(
                Guid.NewGuid(),
                lakeview.Id,
                "https://images.unsplash.com/photo-1600566753086-00f18fb6b3ea",
                "Lakeview Residency interior"
            ),

            PropertyImage.Create(
                Guid.NewGuid(),
                metroHeights.Id,
                "https://images.unsplash.com/photo-1493809842364-78817add7ffb",
                "Metro Heights apartment"
            ),

            PropertyImage.Create(
                Guid.NewGuid(),
                sunriseResidency.Id,
                "https://images.unsplash.com/photo-1600210492486-724fe5c67fb0",
                "Sunrise Residency living room"
            ),

            PropertyImage.Create(
                Guid.NewGuid(),
                royalEnclave.Id,
                "https://images.unsplash.com/photo-1600607687920-4e2a09cf159d",
                "Royal Enclave luxury interior"
            ),

            PropertyImage.Create(
                Guid.NewGuid(),
                cedarHomes.Id,
                "https://images.unsplash.com/photo-1600585154526-990dced4db0d",
                "Cedar Homes exterior"
            ),

            PropertyImage.Create(
                Guid.NewGuid(),
                valleyVista.Id,
                "https://images.unsplash.com/photo-1505693416388-ac5ce068fe85",
                "Valley Vista apartment interior"
            )
        };

        context.PropertyImages.AddRange(images);

        // =========================
        // SAVE
        // =========================

        await context.SaveChangesAsync();
    }
}