import Link from "next/link";

interface PropertyImage {
  id: string;
  url: string;
  altText?: string | null;
}

interface PropertyDocument {
  id: string;
  name: string;
  documentType: string;
  content?: string | null;
}

interface Property {
  id: string;
  name: string;
  description?: string | null;

  propertyType: string;
  city: string;
  state: string;
  country: string;

  monthlyRent: number;
  securityDeposit: number;

  bedrooms: number;
  bathrooms: number;

  furnishingType: string;

  amenities: string[];
  images: PropertyImage[];
  documents: PropertyDocument[];
}

interface PropertyPageProps {
  params: Promise<{
    id: string;
  }>;
}

const API_BASE_URL =
  process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5025";

async function getProperty(id: string): Promise<Property | null> {
  try {
    const response = await fetch(
      `${API_BASE_URL}/api/properties/${id}`,
      {
        cache: "no-store",
      }
    );

    if (!response.ok) {
      return null;
    }

    return await response.json();
  } catch (error) {
    console.error("Failed to fetch property:", error);
    return null;
  }
}

export default async function PropertyPage({
  params,
}: PropertyPageProps) {
  const { id } = await params;

  const property = await getProperty(id);

  // =========================================================
  // PROPERTY NOT FOUND
  // =========================================================

  if (!property) {
    return (
      <main className="min-h-screen bg-slate-50 px-6 py-12">
        <div className="mx-auto max-w-5xl">
          <Link
            href="/"
            className="text-sm font-medium text-blue-600 hover:text-blue-700"
          >
            ← Back to search
          </Link>

          <div className="mt-8 rounded-3xl border border-slate-200 bg-white p-10 text-center shadow-sm">
            <h1 className="text-2xl font-bold text-slate-900">
              Property not found
            </h1>

            <p className="mt-3 text-slate-500">
              We couldn't find a property with this ID.
            </p>

            <p className="mt-4 break-all font-mono text-xs text-slate-400">
              {id}
            </p>
          </div>
        </div>
      </main>
    );
  }

  const location = [
    property.city,
    property.state,
    property.country,
  ]
    .filter(Boolean)
    .join(", ");

  return (
    <main className="min-h-screen bg-slate-50 px-6 py-10">
      <div className="mx-auto max-w-6xl">

        {/* =====================================================
            BACK
        ===================================================== */}

        <Link
          href="/"
          className="inline-flex items-center text-sm font-medium text-blue-600 transition hover:text-blue-800"
        >
          ← Back to search
        </Link>

        {/* =====================================================
            HERO
        ===================================================== */}

        <section className="mt-6 overflow-hidden rounded-3xl border border-slate-200 bg-white shadow-sm">

          {/* Main Image */}

          {property.images.length > 0 ? (
            <div className="relative h-[420px] w-full bg-slate-100">
              <img
                src={property.images[0].url}
                alt={
                  property.images[0].altText ??
                  property.name
                }
                className="h-full w-full object-cover"
              />

              <div className="absolute inset-x-0 bottom-0 bg-gradient-to-t from-black/70 to-transparent p-8">
                <p className="text-sm font-medium text-white/80">
                  {property.propertyType}
                </p>

                <h1 className="mt-1 text-3xl font-bold text-white md:text-4xl">
                  {property.name}
                </h1>

                <p className="mt-2 text-sm text-white/80">
                  📍 {location}
                </p>
              </div>
            </div>
          ) : (
            <div className="flex h-72 items-center justify-center bg-slate-100">
              <p className="text-slate-400">
                No property images available
              </p>
            </div>
          )}

          {/* Property header */}

          <div className="flex flex-col gap-6 p-8 md:flex-row md:items-center md:justify-between">

            <div>
              <p className="text-sm font-medium uppercase tracking-wider text-blue-600">
                Property Details
              </p>

              <h2 className="mt-2 text-2xl font-bold text-slate-900">
                {property.name}
              </h2>

              <p className="mt-2 text-slate-500">
                📍 {location}
              </p>
            </div>

            <div className="rounded-2xl bg-slate-900 px-7 py-5 text-white">
              <p className="text-sm text-slate-400">
                Monthly Rent
              </p>

              <p className="mt-1 text-3xl font-bold">
                ₹{property.monthlyRent.toLocaleString("en-IN")}
              </p>

              <p className="mt-1 text-xs text-slate-400">
                per month
              </p>
            </div>
          </div>
        </section>

        {/* =====================================================
            QUICK INFORMATION
        ===================================================== */}

        <section className="mt-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-4">

          <InfoCard
            label="Bedrooms"
            value={property.bedrooms.toString()}
          />

          <InfoCard
            label="Bathrooms"
            value={property.bathrooms.toString()}
          />

          <InfoCard
            label="Property Type"
            value={property.propertyType}
          />

          <InfoCard
            label="Furnishing"
            value={property.furnishingType}
          />

        </section>

        {/* =====================================================
            DESCRIPTION
        ===================================================== */}

        <section className="mt-6 rounded-3xl border border-slate-200 bg-white p-8 shadow-sm">

          <h2 className="text-xl font-bold text-slate-900">
            About this property
          </h2>

          <p className="mt-4 whitespace-pre-line leading-7 text-slate-600">
            {property.description ||
              "No description has been provided for this property."}
          </p>

        </section>

        {/* =====================================================
            FINANCIAL DETAILS
        ===================================================== */}

        <section className="mt-6 rounded-3xl border border-slate-200 bg-white p-8 shadow-sm">

          <h2 className="text-xl font-bold text-slate-900">
            Financial Details
          </h2>

          <div className="mt-5 grid gap-4 md:grid-cols-2">

            <div className="rounded-2xl bg-slate-50 p-6">
              <p className="text-sm text-slate-500">
                Monthly Rent
              </p>

              <p className="mt-2 text-2xl font-bold text-slate-900">
                ₹{property.monthlyRent.toLocaleString("en-IN")}
              </p>
            </div>

            <div className="rounded-2xl bg-slate-50 p-6">
              <p className="text-sm text-slate-500">
                Security Deposit
              </p>

              <p className="mt-2 text-2xl font-bold text-slate-900">
                ₹{property.securityDeposit.toLocaleString("en-IN")}
              </p>
            </div>

          </div>

        </section>

        {/* =====================================================
            AMENITIES
        ===================================================== */}

        <section className="mt-6 rounded-3xl border border-slate-200 bg-white p-8 shadow-sm">

          <h2 className="text-xl font-bold text-slate-900">
            Amenities
          </h2>

          {property.amenities.length > 0 ? (
            <div className="mt-5 flex flex-wrap gap-3">

              {property.amenities.map((amenity) => (
                <span
                  key={amenity}
                  className="rounded-full bg-blue-50 px-4 py-2 text-sm font-medium text-blue-700"
                >
                  {amenity}
                </span>
              ))}

            </div>
          ) : (
            <p className="mt-4 text-slate-500">
              No amenities listed.
            </p>
          )}

        </section>

        {/* =====================================================
            ALL IMAGES
        ===================================================== */}

        {property.images.length > 1 && (
          <section className="mt-6 rounded-3xl border border-slate-200 bg-white p-8 shadow-sm">

            <h2 className="text-xl font-bold text-slate-900">
              Property Gallery
            </h2>

            <div className="mt-5 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">

              {property.images.map((image) => (
                <div
                  key={image.id}
                  className="overflow-hidden rounded-2xl bg-slate-100"
                >
                  <img
                    src={image.url}
                    alt={image.altText ?? property.name}
                    className="h-60 w-full object-cover transition duration-300 hover:scale-105"
                  />
                </div>
              ))}

            </div>

          </section>
        )}

        {/* =====================================================
            PROPERTY ID
        ===================================================== */}

        <section className="mt-6 rounded-2xl border border-slate-200 bg-white p-6">

          <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
            Property ID
          </p>

          <p className="mt-2 break-all font-mono text-sm text-slate-600">
            {property.id}
          </p>

        </section>

      </div>
    </main>
  );
}

// =============================================================
// INFO CARD
// =============================================================

function InfoCard({
  label,
  value,
}: {
  label: string;
  value: string;
}) {
  return (
    <div className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
      <p className="text-sm text-slate-500">
        {label}
      </p>

      <p className="mt-2 text-lg font-semibold capitalize text-slate-900">
        {value}
      </p>
    </div>
  );
}