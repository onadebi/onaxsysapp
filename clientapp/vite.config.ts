import { defineConfig } from 'vite'
import path from "path";
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
    server: {
        port: 3000,
    },
    build: {
        outDir: '../WebApp/wwwroot/portal',
        emptyOutDir: true,
        chunkSizeWarningLimit: 1000, // Adjust the chunk size warning limit
        rollupOptions: {
            output: {
                manualChunks(id) {
                    if (id.includes('node_modules')) {
                        return id.toString().split('node_modules/')[1].split('/')[0].toString();
                    }
                }
            }
        }
    },
    plugins: [react()],
    resolve: {
        alias: {
            "@": path.resolve(__dirname, "./src"),
        },
    },
})
