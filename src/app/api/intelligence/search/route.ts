import { NextRequest, NextResponse } from "next/server";

const BACKEND_URL = "https://rentzintelligence.onrender.com";

export async function POST(request: NextRequest) {
  try {
    const body = await request.json();

    const response = await fetch(
      `${BACKEND_URL}/api/HybridPropertySearch/search`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(body),
        cache: "no-store",
      }
    );

    const data = await response.json();

    return NextResponse.json(data, {
      status: response.status,
    });
  } catch (error) {
    console.error("Intelligence search error:", error);

    return NextResponse.json(
      {
        message: "Unable to connect to Rentz Intelligence API.",
      },
      {
        status: 500,
      }
    );
  }
}