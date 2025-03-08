## Tailwind installation (RECOMMENDED: All commandline operations are done in Bash terminal- not CMD or Powershell)

## 1
```bash
npm init -y
```

## 2 Install Tailwind and dependencies
```bash
npm install -D tailwindcss@^3.4.1 postcss@^8.4.35 autoprefixer@^10.4.20 eslint@latest
```


## 3 Create a Tailwind config file `tailwind.config.js`:
```js
/** @type {import('tailwindcss').Config} */
module.exports = {
   purge: {
     enabled: true,
        content: ["./Pages/**/*.cshtml", "./Views/**/*.{html,js,jsx,ts,tsx,razor,cshtml}","./WebApp/Views/**/*.{cshtml,js,razor,html}"],
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

```
## 4) Create a `tailwindinput.css` file in the `wwwroot/css` folder with the following content:
```css
@tailwind base;
@tailwind components;
@tailwind utilities;
```

## 5) Update/COnfigure the `package.json` file to be like below, for `--watch` for changes and manual invokation for use as part of build process (done later):

```json
{
  "name": "webapp",
  "version": "1.0.0",
  "main": "index.js",
  "scripts": {
    "css:build": "npx tailwindcss -i ./wwwroot/css/tailwindinput.css -o ./wwwroot/css/tailwindoutput.css --watch",
    "tw:css": "npx tailwindcss -i ./wwwroot/css/tailwindinput.css -o ./wwwroot/css/tailwindoutput.css",
    "tw:css:prod": "npx tailwindcss -i ./wwwroot/css/tailwindinput.css -o ./wwwroot/css/tailwindoutput.css --minify"
  },
  "keywords": [],
  "author": "",
  "license": "ISC",
  "description": "",
  "devDependencies": {
    "autoprefixer": "^10.4.20",
    "eslint": "^9.22.0",
    "postcss": "^8.4.35",
    "tailwindcss": "^3.4.1"
  }
}

```json


## 6) [Optional] Create a `postcss.config.js` file in the root of the project with the following content:
```js
module.exports = {
  plugins: {
    tailwindcss: {},
    autoprefixer: {},
  }
}
```


## 7) Configure the Project to run the `tw:css` script as part of the build process. This is done in your `WeAPp.csproj` file:
```xml
<Target Name="Tailwind" BeforeTargets="Build">
    <Exec Command="npm run tw:css" />
</Target>
```


## [Optionals]
`postcss` and `autoprefixer` are not strictly required for basic Tailwind CSS functionality, since Tailwind CSS 3.x includes its own PostCSS processing. However, they are recommended if you:

1. Need to support older browsers (IE11, older versions of Safari)
2. Use custom CSS with modern features
3. Have specific PostCSS plugins you want to use