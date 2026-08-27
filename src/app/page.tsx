"use client";

import Link from "next/link";
import { FormEvent, useState } from "react";

interface Property {
  id: string;
  name: string;
  description: string;
  propertyType: string;
  city: string;
  state: string;
  country: string;
  monthlyRent: number;
  bedrooms: number;
  bathrooms: number;
  furnishingType: string;
  ruleScore: number;
  semanticScore: number;
  hybridScore: number;
  reason: string;
}

interface SearchResponse {
  query: string;
  count: number;
  results: Property[];
}

export default function Home() {
  const [query, setQuery] = useState("");
  const [results, setResults] = useState<Property[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  // =========================================================
  // SEARCH
  // =========================================================

  async function handleSearch(
    event: FormEvent<HTMLFormElement>
  ) {
    event.preventDefault();

    const trimmedQuery = query.trim();

    if (!trimmedQuery) {
      setError(
        "Tell us what kind of property you're looking for."
      );
      return;
    }

    setLoading(true);
    setError("");
    setResults([]);

    try {
      /*
       * Production request flow:
       *
       * Browser
       *    ↓
       * /api/intelligence/search
       *    ↓
       * Next.js route.ts
       *    ↓
       * https://rentzintelligence.onrender.com
       *    ↓
       * ASP.NET Core
       *    ↓
       * Neon PostgreSQL + pgvector
       */

      const response = await fetch(
        "/api/intelligence/search",
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            query: trimmedQuery,
            limit: 10,
          }),
        }
      );

      if (!response.ok) {
        throw new Error(
          `Search request failed with status ${response.status}`
        );
      }

      const data: SearchResponse =
        await response.json();

      const searchResults = data.results ?? [];

      setResults(searchResults);

      if (searchResults.length === 0) {
        setError(
          "We couldn't find a matching property. Try describing your requirements differently."
        );
      }
    } catch (error) {
      console.error(
        "Property search failed:",
        error
      );

      setError(
        "Something went wrong while searching. Please try again."
      );

      setResults([]);
    } finally {
      setLoading(false);
    }
  }

  // =========================================================
  // EXAMPLE SEARCH
  // =========================================================

  function handleExampleSearch(example: string) {
    setQuery(example);
    setError("");
  }

  return (
    <main className="min-h-screen bg-slate-950 text-white">

      {/* =====================================================
          HEADER
      ====================================================== */}

      <header className="sticky top-0 z-50 border-b border-white/10 bg-slate-950/90 backdrop-blur-xl">
        <div className="mx-auto flex h-20 max-w-7xl items-center justify-between px-6">

          <Link
            href="/"
            className="text-2xl font-bold tracking-tight"
          >
            Rentz
            <span className="text-blue-500">
              .
            </span>
          </Link>

          <div className="flex items-center gap-6">

            <span className="hidden text-sm text-slate-400 sm:block">
              AI Property Intelligence
            </span>

            <div className="flex h-9 items-center gap-2 rounded-full border border-blue-500/20 bg-blue-500/10 px-3">

              <span className="h-2 w-2 rounded-full bg-blue-500" />

              <span className="text-xs font-medium text-blue-400">
                Intelligence Online
              </span>

            </div>
          </div>
        </div>
      </header>

      {/* =====================================================
          HERO
      ====================================================== */}

      <section className="relative overflow-hidden">

        <div className="pointer-events-none absolute left-1/2 top-0 h-[500px] w-[700px] -translate-x-1/2 rounded-full bg-blue-600/10 blur-3xl" />

        <div className="relative mx-auto max-w-5xl px-6 pb-20 pt-24 text-center sm:pt-32">

          {/* Badge */}

          <div className="mb-7 inline-flex items-center gap-2 rounded-full border border-blue-500/20 bg-blue-500/10 px-4 py-2">

            <span className="text-sm">
              🧠
            </span>

            <span className="text-sm font-medium text-blue-400">
              AI-powered property search
            </span>

          </div>

          {/* Heading */}

          <h1 className="text-5xl font-bold tracking-tight sm:text-6xl lg:text-7xl">
            Find a place that

            <span className="block text-blue-500">
              actually fits.
            </span>
          </h1>

          {/* Description */}

          <p className="mx-auto mt-7 max-w-2xl text-base leading-8 text-slate-400 sm:text-lg">
            Tell Rentz what you&apos;re looking for in
            normal language. Our intelligence engine
            understands your requirements and ranks
            the properties that best match.
          </p>

          {/* =================================================
              SEARCH
          ================================================== */}

          <form
            onSubmit={handleSearch}
            className="mx-auto mt-10 max-w-4xl"
          >

            <div className="rounded-2xl border border-white/10 bg-white/[0.04] p-2 shadow-2xl backdrop-blur-xl sm:flex sm:items-center">

              <div className="flex min-w-0 flex-1 items-center">

                <span className="pl-4 text-xl text-slate-500">
                  🔍
                </span>

                <input
                  value={query}
                  onChange={(event) => {
                    setQuery(event.target.value);

                    if (error) {
                      setError("");
                    }
                  }}
                  disabled={loading}
                  placeholder="Try: 2 bedroom apartment in Gangtok under 25000"
                  className="h-14 min-w-0 flex-1 bg-transparent px-4 text-base text-white outline-none placeholder:text-slate-500 disabled:cursor-not-allowed"
                />

              </div>

              <button
                type="submit"
                disabled={loading}
                className="h-14 w-full rounded-xl bg-blue-600 px-8 font-semibold transition hover:bg-blue-500 disabled:cursor-not-allowed disabled:opacity-60 sm:w-auto"
              >

                {loading ? (
                  <span className="flex items-center justify-center gap-2">

                    <span className="h-4 w-4 animate-spin rounded-full border-2 border-white/30 border-t-white" />

                    Searching

                  </span>
                ) : (
                  "Search Properties"
                )}

              </button>

            </div>

          </form>

          {/* =================================================
              EXAMPLES
          ================================================== */}

          <div className="mt-6">

            <p className="mb-3 text-xs uppercase tracking-wider text-slate-600">
              Try searching for
            </p>

            <div className="flex flex-wrap justify-center gap-2">

              {[
                "2 bedroom apartment in Gangtok under 25000",
                "cheap room in Gangtok",
                "good room in Gangtok",
              ].map((example) => (

                <button
                  key={example}
                  type="button"
                  onClick={() =>
                    handleExampleSearch(example)
                  }
                  disabled={loading}
                  className="rounded-full border border-white/10 bg-white/[0.03] px-4 py-2 text-xs text-slate-400 transition hover:border-blue-500/30 hover:bg-blue-500/10 hover:text-blue-400 disabled:cursor-not-allowed disabled:opacity-50"
                >
                  {example}
                </button>

              ))}

            </div>
          </div>

          {/* Error */}

          {error && (
            <div className="mx-auto mt-5 max-w-2xl rounded-xl border border-red-500/20 bg-red-500/5 px-4 py-3 text-sm text-red-400">
              {error}
            </div>
          )}

        </div>
      </section>

      {/* =====================================================
          RESULTS
      ====================================================== */}

      {results.length > 0 && (

        <section className="border-t border-white/5 bg-slate-950">

          <div className="mx-auto max-w-7xl px-6 py-16">

            {/* Results Header */}

            <div className="mb-10 flex flex-col justify-between gap-4 sm:flex-row sm:items-end">

              <div>

                <p className="text-sm font-medium text-blue-400">
                  Search results
                </p>

                <h2 className="mt-2 text-3xl font-bold tracking-tight">
                  Best matches
                </h2>

                <p className="mt-2 text-sm text-slate-500">
                  {results.length}{" "}
                  {results.length === 1
                    ? "property"
                    : "properties"}{" "}
                  ranked for your search.
                </p>

              </div>

              <div className="rounded-full border border-white/10 bg-white/[0.03] px-4 py-2 text-xs text-slate-400">
                Ranked by Rentz Intelligence
              </div>

            </div>

            {/* Property Grid */}

            <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">

              {results.map((property) => (

                <article
                  key={property.id}
                  className="group flex flex-col overflow-hidden rounded-3xl border border-white/10 bg-white/[0.04] transition duration-300 hover:-translate-y-1 hover:border-blue-500/30 hover:bg-white/[0.06]"
                >

                  {/* Property Visual */}

                  <div className="relative flex h-44 items-center justify-center overflow-hidden bg-gradient-to-br from-slate-900 via-slate-800 to-blue-950">

                    <div className="absolute inset-0 bg-blue-500/5 transition group-hover:bg-blue-500/10" />

                    <div className="relative text-5xl opacity-40">
                      🏠
                    </div>

                    {/* Match */}

                    <div className="absolute right-4 top-4 rounded-2xl border border-blue-400/20 bg-slate-950/80 px-3 py-2 text-center backdrop-blur">

                      <div className="text-lg font-bold text-blue-400">
                        {property.hybridScore.toFixed(1)}
                      </div>

                      <div className="text-[9px] font-medium uppercase tracking-widest text-slate-500">
                        Match
                      </div>

                    </div>

                  </div>

                  {/* Card Body */}

                  <div className="flex flex-1 flex-col p-6">

                    {/* Name */}

                    <div>

                      <h3 className="text-xl font-semibold tracking-tight">
                        {property.name}
                      </h3>

                      <p className="mt-1 text-sm text-slate-500">
                        {property.city},{" "}
                        {property.state}
                      </p>

                    </div>

                    {/* Rent */}

                    <div className="mt-5">

                      <span className="text-2xl font-bold">
                        ₹
                        {property.monthlyRent.toLocaleString(
                          "en-IN"
                        )}
                      </span>

                      <span className="ml-1 text-sm text-slate-500">
                        / month
                      </span>

                    </div>

                    {/* Details */}

                    <div className="mt-5 flex flex-wrap gap-2">

                      <span className="rounded-full bg-white/5 px-3 py-1.5 text-xs text-slate-400">
                        {property.bedrooms} Bed
                      </span>

                      <span className="rounded-full bg-white/5 px-3 py-1.5 text-xs text-slate-400">
                        {property.bathrooms} Bath
                      </span>

                      <span className="rounded-full bg-white/5 px-3 py-1.5 text-xs text-slate-400">
                        {property.propertyType}
                      </span>

                      <span className="rounded-full bg-white/5 px-3 py-1.5 text-xs text-slate-400">
                        {property.furnishingType}
                      </span>

                    </div>

                    {/* Description */}

                    <p className="mt-5 line-clamp-3 text-sm leading-6 text-slate-400">
                      {property.description}
                    </p>

                    {/* Intelligence */}

                    <div className="mt-6 rounded-2xl border border-white/5 bg-black/20 p-4">

                      <div className="flex items-center gap-2">

                        <span className="text-sm">
                          🧠
                        </span>

                        <p className="text-xs font-semibold uppercase tracking-wider text-slate-500">
                          Why this property?
                        </p>

                      </div>

                      <p className="mt-2 text-sm leading-6 text-slate-300">
                        {property.reason}
                      </p>

                    </div>

                    {/* Scores */}

                    <div className="mt-5 grid grid-cols-2 gap-2">

                      <div className="rounded-xl bg-white/[0.03] p-3">

                        <p className="text-[10px] uppercase tracking-wider text-slate-600">
                          Rule
                        </p>

                        <p className="mt-1 text-sm font-semibold text-slate-300">
                          {property.ruleScore.toFixed(1)}
                        </p>

                      </div>

                      <div className="rounded-xl bg-blue-500/5 p-3">

                        <p className="text-[10px] uppercase tracking-wider text-slate-600">
                          Semantic
                        </p>

                        <p className="mt-1 text-sm font-semibold text-blue-400">
                          {property.semanticScore.toFixed(1)}
                        </p>

                      </div>

                    </div>

                    {/* CTA */}

                    <Link
                      href={`/properties/${property.id}`}
                      className="mt-6 flex h-12 w-full items-center justify-center rounded-xl bg-blue-600 font-semibold transition hover:bg-blue-500"
                    >
                      View Property

                      <span className="ml-2 transition-transform group-hover:translate-x-1">
                        →
                      </span>

                    </Link>

                  </div>
                </article>

              ))}

            </div>
          </div>
        </section>
      )}

      {/* =====================================================
          EMPTY STATE
      ====================================================== */}

      {!loading &&
        results.length === 0 &&
        !error && (

          <section className="mx-auto max-w-3xl px-6 pb-24 text-center">

            <div className="rounded-3xl border border-white/10 bg-white/[0.03] p-10 sm:p-14">

              <div className="mx-auto flex h-16 w-16 items-center justify-center rounded-2xl border border-blue-500/20 bg-blue-500/10 text-2xl">
                🧠
              </div>

              <h2 className="mt-6 text-2xl font-semibold">
                Search naturally
              </h2>

              <p className="mx-auto mt-3 max-w-lg text-sm leading-7 text-slate-500">
                Describe the property you want in your
                own words. Rentz will understand your
                requirements and find the properties
                that best match.
              </p>

            </div>

          </section>
        )}

      {/* =====================================================
          LOADING STATE
      ====================================================== */}

      {loading && (

        <section className="mx-auto max-w-7xl px-6 pb-24">

          <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">

            {[1, 2, 3].map((item) => (

              <div
                key={item}
                className="h-[520px] animate-pulse rounded-3xl border border-white/10 bg-white/[0.03]"
              />

            ))}

          </div>

        </section>
      )}

      {/* =====================================================
          FOOTER
      ====================================================== */}

      <footer className="border-t border-white/10">

        <div className="mx-auto flex max-w-7xl flex-col gap-2 px-6 py-8 text-center text-xs text-slate-600 sm:flex-row sm:items-center sm:justify-between sm:text-left">

          <p>
            Rentz Intelligence
          </p>

          <p>
            Property search powered by hybrid AI ranking.
          </p>

        </div>

      </footer>

    </main>
  );
}