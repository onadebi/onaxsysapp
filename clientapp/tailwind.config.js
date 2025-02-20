/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors:{
        onaxSky: '#C3EBFA',
  			onaxSkyLight: '#EDF9FD',
			  onaxBlue: '#2563eb',
        onaxMenuBlue: '#045eff',
  			onaxPurple: '#CFCEFF',
  			onaxPurpleLight: '#F1F0FF',
      }
    },
  },
  plugins: [require("tailwindcss-animate")],
}

