/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        'radio-black': '#0a0908',
        'radio-wood': '#1c140c',
      },
      fontFamily: {
        'display': ['"Courier New"', 'Courier', 'monospace'],
      }
    },
  },
  plugins: [],
}
