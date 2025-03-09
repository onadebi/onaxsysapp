/** @type {import('tailwindcss').Config} */
module.exports = {
    purge: {
        enabled: true,
        content: ["./Pages/**/*.cshtml", "./Views/**/*.{html,js,jsx,ts,tsx,razor,cshtml}", "./WebApp/Views/**/*.{cshtml,js,razor,html}"],
    },
    content: [
        "./Views/**/*.{cshtml,js,razor,html}",
        "./WebApp/Views/**/*.{cshtml,js,razor,html}",
    ],
    theme: {
        extend: {
            colors: {
                onaxSky: "#C3EBFA",
                onaxSkyLight: "#EDF9FD",
                onaxBlue: "#2563eb",
                onaxPurple: "#CFCEFF",
                onaxPurpleLight: "#F1F0FF",
            },
        },
    },
    plugins: [],
};